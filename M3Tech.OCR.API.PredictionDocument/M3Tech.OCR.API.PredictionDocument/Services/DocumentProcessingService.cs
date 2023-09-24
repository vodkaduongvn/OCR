using M3Tech.OCR.API.PredictionDocument.Data.Models;
using M3Tech.OCR.API.PredictionDocument.Models;
using M3Tech.OCR.API.PredictionDocument.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace M3Tech.OCR.API.PredictionDocument.Services
{
    public interface IDocumentProcessingService
    {
        /// <summary>
        /// Storage And Predict Document
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        /// <param name="jsonProvided"></param>
        /// <param name="callModel"></param>
        /// <returns></returns>
        Task<PredictedResultModel> StorageAndPredictDocument(byte[] fileBytes, string fileName, string jsonProvided, CallModel callModel);
    }

    public class DocumentProcessingService:IDocumentProcessingService
    {
        private readonly IDocumentPredictionService _documentPredictionService;
        private readonly IDocumentExtractionService _documentExtractionService;

        private readonly IDocumentCategorizationLogRepository _documentCategorizationLogRepository;
        private readonly IDocumentCategoryRepository _documentCategoryRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICallTypeRepository _callTypeRepository;
        private readonly IConsumerRepository _consumerRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly ICallRepository _callRepository;

        private readonly IAzureStorageService _azureStorageService;


        public DocumentProcessingService(
            IDocumentCategorizationLogRepository documentCategorizationLogRepository
            ,IDocumentCategoryRepository documentCategoryRepository
            , IDocumentRepository documentRepository
            , ICallTypeRepository callTypeRepository
            , IConsumerRepository consumerRepository
            , ILabelRepository labelRepository
            , ICallRepository callRepository

            , IDocumentExtractionService documentExtractionService
            , IDocumentPredictionService documentPredictionService

            , IAzureStorageService azureStorageService
            ) 
        {
            _documentExtractionService = documentExtractionService; 
            _documentPredictionService = documentPredictionService;

            _documentCategorizationLogRepository = documentCategorizationLogRepository;
            _documentCategoryRepository = documentCategoryRepository;
            _documentRepository = documentRepository;
            _callTypeRepository = callTypeRepository;
            _consumerRepository = consumerRepository;
            _labelRepository = labelRepository;
            _callRepository = callRepository;

            _azureStorageService = azureStorageService;
        }

        /// <summary>
        /// Storage And Predict Document
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<PredictedResultModel> StorageAndPredictDocument(byte[] fileBytes, string fileName,string jsonProvided, CallModel callModel)
        {

            PredictedResultModel predictedResult = null;
            Consumer consumer = null;

            try
            {
                var extractionStartDatetime = DateTime.UtcNow;

                var extractedResult = _documentExtractionService.ExtractBase64FromPDF(fileBytes);

                var extractionFinishDatetime = DateTime.UtcNow;


                var documentCategories = await _documentCategoryRepository.GetDocumentCategoriesAsync();
                var labels = await _labelRepository.GetLabelsAsync();
                consumer = await _consumerRepository.GetConsumerByNameAsync("BOSS");

                // get model from local api webhost foler
                var modelPath = await GetOrDownloadModel();

                if (!string.IsNullOrEmpty(modelPath))
                {
                    #region predict

                    var categorizationModelCalledDatetime = DateTime.UtcNow;

                    predictedResult = _documentPredictionService.PredictDocument(extractedResult, modelPath);
                    var ratio = predictedResult.Probability1 / predictedResult.Probability2 - 1;
                    var confidence = ratio > 1 ? 100 : ratio * 100;

                    var categorizationModelResponseDatetime = DateTime.UtcNow;

                    var label1 = labels.FirstOrDefault(x => x.LabelId == predictedResult.CategoryID1);
                    var documentCategory1 = documentCategories.FirstOrDefault(x => x.DocumentCategoryId == label1.DocumentCategoryId);

                    var label2 = labels.FirstOrDefault(x => x.LabelId == predictedResult.CategoryID2);
                    var documentCategory2 = documentCategories.FirstOrDefault(x => x.DocumentCategoryId == label2.DocumentCategoryId);

                    var label3 = labels.FirstOrDefault(x => x.LabelId == predictedResult.CategoryID3);
                    var documentCategory3 = documentCategories.FirstOrDefault(x => x.DocumentCategoryId == label3.DocumentCategoryId);

                    predictedResult.NBCIdentifier = documentCategory1.DocumentNbcIdentifier;
                    predictedResult.CategoryNameEN1 = documentCategory1.DocumentCategoryNameEn;
                    predictedResult.CategoryNameEN2 = documentCategory2.DocumentCategoryNameEn;
                    predictedResult.CategoryNameEN3 = documentCategory3.DocumentCategoryNameEn;
                    predictedResult.CategoryNameFR = documentCategory1.DocumentCategoryNameFr;
                    predictedResult.Confidence = $"{confidence}%";

                    #endregion


                    #region add records to DB

                    // Documents table
                    var document = new Document()
                    {
                        DocumentCategoryId = label1.DocumentCategoryId,
                        DocumentName = fileName,
                        EncodedContent = extractedResult,
                        ExtractionFinishDatetime = extractionFinishDatetime,
                        ExtractionStartDatetime = extractionStartDatetime,
                        CategorizationDatetime = DateTime.UtcNow,
                    };

                    var existingDoc = await _documentRepository.GetExistingDocumentAsync(document);
                    var categorizationType = existingDoc == null ? "Creation" : "Update";

                    await _documentRepository.AddOrUpdateDocumentAsync(document);

                    // Document_categorization_logs table
                    var documentCategorizationLog = new DocumentCategorizationLog
                    {
                        DocumentId = document.DocumentId,
                        DocumentCategoryId = document.DocumentCategoryId,
                        CategorizationType = categorizationType,
                        CategorizationModelCalledDatetime = categorizationModelCalledDatetime,
                        CategorizationModelResponseDatetime = categorizationModelResponseDatetime,
                        CategorizationProbability = predictedResult.Probability1.Value,
                        CategorizationConfidence = confidence.Value,
                        UpdateSource = "Categorization model caller" //consumer?.ConsumerNameEn
                    };
                    await _documentCategorizationLogRepository.AddDocumentCategorizationLogAsync(documentCategorizationLog);

                    // update Document table after adding logs
                    document.LastCategorizationLogId = documentCategorizationLog.CategorizationLogId;
                    await _documentRepository.AddOrUpdateDocumentAsync(document);

                    // Call table
                    await AddCallAsync(JsonConvert.SerializeObject(predictedResult), jsonProvided, callModel, consumer);

                    #endregion

                }
            }
            catch (Exception ex)
            {
                await AddCallAsync(JsonConvert.SerializeObject(predictedResult), jsonProvided, callModel, consumer, ex);

                throw;
            }
            return predictedResult;
        }

        private async Task AddCallAsync(
            string jsonProvided
            , string jsonReceived
            , CallModel callModel
            , Consumer consumer
            , Exception ex = null)
        {
            // call table
            callModel.CallRespondedDatetime = DateTime.UtcNow;

            //var consumer = await _consumerRepository.GetConsumerByNameAsync("BOSS");

            var callType = await _callTypeRepository.GetCallTypeByNameAsync(callModel.callTypeName);

            var call = new Call
            {
                CallTypeId = callType.CallTypeId,
                ConsumerId = consumer?.ConsumerId,
                CallReceivedDatetime = callModel.CallReceivedDatetime,
                CallRespondedDatetime = callModel.CallRespondedDatetime,
                JsonProvided = jsonProvided,
                JsonReceived = jsonReceived,
                ErrorFound = ex?.Message
            };
            await _callRepository.AddCallAsync(call);
        }

        async Task<string> GetOrDownloadModel()
        {
            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrainedModel", "model.onnx");

            if (!File.Exists(modelPath))
            {
                byte[] modelByteArr = await _azureStorageService.GetLatestDocumentAsync();
                Directory.CreateDirectory(Path.GetDirectoryName(modelPath));
                File.WriteAllBytes(modelPath, modelByteArr);
            }

            return modelPath;
        }
    }
}
