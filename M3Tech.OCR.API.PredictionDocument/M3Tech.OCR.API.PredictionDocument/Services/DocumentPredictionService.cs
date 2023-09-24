using BERTTokenizers;
using M3Tech.OCR.API.PredictionDocument.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Buffers.Text;
using System.Text;
using TorchSharp;

namespace M3Tech.OCR.API.PredictionDocument.Services
{
    public interface IDocumentPredictionService
    {
        PredictedResultModel PredictDocument(string contentPdf, string modelPath);
    }

    public class DocumentPredictionService:IDocumentPredictionService
    {
        private readonly IDocumentExtractionService _documentExtractionService;
        public DocumentPredictionService(
            IDocumentExtractionService documentExtractionService
            ) 
        {
            _documentExtractionService = documentExtractionService;
        }

        public PredictedResultModel PredictDocument(string contentPdf, string modelPath)
        {
            var predictedLabel = "";
            PredictedResultModel predictedResult = null;
            int maxSequenceLength = 512;
            var tokenizer = new BertUncasedLargeTokenizer();

            try
            {
                var tokens = tokenizer.Tokenize(contentPdf);
                var encoded = tokenizer.Encode(tokens.Count(), contentPdf);

                int inputLength = Math.Min(encoded.Count, maxSequenceLength);

                var bertInput = new BertInputModel()
                {
                    InputIds = encoded.Select(t => t.Item1).Take(inputLength).ToArray(),
                    AttentionMask = encoded.Select(t => t.Item3).Take(inputLength).ToArray(),
                    TypeIds = encoded.Select(t => t.Item2).Take(inputLength).ToArray(),
                };

                var input_ids = ConvertToTensor(bertInput.InputIds, bertInput.InputIds.Length);
                var attention_mask = ConvertToTensor(bertInput.AttentionMask, bertInput.InputIds.Length);
                var token_type_ids = ConvertToTensor(bertInput.TypeIds, bertInput.InputIds.Length);

                var session = new InferenceSession(modelPath);

                using var results = session.Run(new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("input_ids", input_ids),
                    NamedOnnxValue.CreateFromTensor("attention_mask", attention_mask),
                    NamedOnnxValue.CreateFromTensor("token_type_ids", token_type_ids)
                });
                predictedResult = new PredictedResultModel();
                var logits = results.First().AsTensor<float>().ToArray();

                var probs = torch.softmax(logits, -1);

                int k = 3;

                var (values, indices) = probs.topk(k);

                predictedResult.CategoryID1 = (int)indices[0].ToSingle();
                predictedResult.CategoryID2 = (int)indices[1].ToSingle();
                predictedResult.CategoryID3 = (int)indices[2].ToSingle();
                predictedResult.Probability1 = values[0].ToSingle();
                predictedResult.Probability2 = values[1].ToSingle();
                predictedResult.Probability3 = values[2].ToSingle();

                Console.WriteLine($"\nPredicted Label: {predictedLabel}");
            }
            catch (Exception ex)
            {
                throw;
            }

            return predictedResult;
        }

        private Tensor<long> ConvertToTensor(long[] inputArray, int inputDimension)
        {
            Tensor<long> input = new DenseTensor<long>(new[] { 1, inputDimension });

            for (var i = 0; i < inputArray.Length; i++)
            {
                input[0, i] = inputArray[i];
            }
            return input;
        }
    }
}
