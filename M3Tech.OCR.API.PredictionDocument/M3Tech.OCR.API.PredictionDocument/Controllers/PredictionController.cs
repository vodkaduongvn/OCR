using M3Tech.OCR.API.PredictionDocument.Models;
using M3Tech.OCR.API.PredictionDocument.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace M3Tech.OCR.API.PredictionDocument.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : Controller
    {
        private Dictionary<string, string> _validExtensions = new Dictionary<string, string>
        {
            [".pdf"] = ""
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDocumentProcessingService _documentProcessingService;

        public PredictionController(
            IDocumentProcessingService documentProcessingService
            , IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
            _documentProcessingService = documentProcessingService;
        }

        [HttpPost("file")]
        public async Task<IActionResult> Post([FromForm] IFormFile[] formFiles)
        {
            if (!Request.HasFormContentType || Request.Form.Files.Count() <= 0)
            {
                return BadRequest("Invalid request content.");
            }

            var predictedResults = new List<PredictedResultModel>();
            var validationErrors = new List<string>();
            var files = Request.Form.Files;

            foreach (var file in files)
            {
                try
                {
                    var fileName = file.FileName;
                    if (!_validExtensions.ContainsKey(Path.GetExtension(fileName).ToLower()))
                    {
                        validationErrors.Add($"File '{file.FileName}' Invalid file extension");
                        continue;
                    }

                    if (!IsApplicationPdfContent(file))
                    {
                        validationErrors.Add($"File '{file.FileName}' is not in application/pdf format.");
                        continue;
                    }

                    byte[] fileBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    var callModel = new CallModel
                    {
                        callTypeName = "File",
                        CallReceivedDatetime = DateTime.UtcNow
                    };
                    var predictedResult = await _documentProcessingService.StorageAndPredictDocument(
                        fileBytes
                        , fileName
                        , fileName
                        , callModel);

                    predictedResults.Add(predictedResult);
                }
                catch (Exception ex)
                {
                    validationErrors.Add(ex.Message);
                }
            }

            return Ok(new { predictedResults, validationErrors });
        }


        [HttpPost("server-document")]
        public async Task<IActionResult> Post([FromBody] DocumentRequestModel documentRequestModel)
        {
            if (!_validExtensions.ContainsKey(Path.GetExtension(documentRequestModel.DocumentName).ToLower()))
            {
                var validationErrors = new List<string>
                {
                    $"File '{documentRequestModel.DocumentName}' Invalid file extension"
                };
                return BadRequest(validationErrors);
            }
            try
            {
                PredictedResultModel predictedResult = null;

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{documentRequestModel.ServerAddress}/{documentRequestModel.ServerFolder}/{documentRequestModel.DocumentName}");

                if (response.IsSuccessStatusCode)
                {
                    var fileByte = await response.Content.ReadAsByteArrayAsync();

                    var callModel = new CallModel
                    {
                        callTypeName = "Server",
                        CallReceivedDatetime = DateTime.UtcNow
                    };
                    predictedResult = await _documentProcessingService.StorageAndPredictDocument(
                        fileByte
                        , documentRequestModel.DocumentName
                        , JsonConvert.SerializeObject(documentRequestModel)
                        , callModel);

                    return Ok(predictedResult);
                }
                return BadRequest(response.Content);

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        private bool IsApplicationPdfContent(IFormFile file)
        {
            return file.ContentType.StartsWith("application/pdf");
        }
    }
}
