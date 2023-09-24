using Tesseract;

using System.Text;
using System.Text.RegularExpressions;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using M3Tech.OCR.API.PredictionDocument.Models;

namespace M3Tech.OCR.API.PredictionDocument.Services
{
    public interface IDocumentExtractionService
    {
        /// <summary>
        /// Check And Extract Base64 From PDF
        /// </summary>
        /// <param name="pdfBytes"></param>
        /// <returns></returns>
        string ExtractBase64FromPDF(byte[] pdfBytes);
    }

    public class DocumentExtractionService : IDocumentExtractionService
    {
        /// <summary>
        /// Check and extract Text from scanned PDF
        /// </summary>
        /// <param name="pdfBytes"></param>
        /// <returns></returns>
        public string ExtractBase64FromPDF(byte[] pdfBytes)
        {
            StringBuilder text = new();
            var extractionStrategy = new TextExtractionStrategy();

            using PdfReader pdfReader = new(pdfBytes);
            using TesseractEngine engine = new(@"tessdata-main", "eng", EngineMode.Default);

            for (var iPage = 1; iPage <= pdfReader.NumberOfPages; iPage++)
            {
                var imageContentLocations = new List<ContentLocation>();

                var pdfParser = new PdfReaderContentParser(pdfReader);
                var content = pdfParser.ProcessContent(iPage, extractionStrategy);

                // get texts and images from page
                var textLocations = content.GetTextLocations().Where(x => !string.IsNullOrWhiteSpace(x.Content));
                var imageLocations = content.GetImageLocations();

                var imagesAsBase64 = imageLocations.Where(x => IsBase64String(x.Content) && !string.IsNullOrWhiteSpace(x.Content));

                // OCR extract text from images
                foreach (var imageAsBase64 in imagesAsBase64)
                {
                    var imageContent = imageAsBase64.Content;

                    using var pix = Pix.LoadFromMemory(Convert.FromBase64String(imageContent));
                    using var pageResult = engine.Process(pix);

                    var resultText = pageResult.GetText();
                    if (!string.IsNullOrEmpty(resultText))
                    {
                        imageContentLocations.Add(new ContentLocation
                        {
                            X = imageAsBase64.X,
                            Y = imageAsBase64.Y,
                            Content = resultText
                        });
                    }
                }

                var contentLocations = textLocations
                    .Concat(imageContentLocations)
                    .Where(x => !string.IsNullOrEmpty(x.Content))
                    .OrderByDescending(x => x.Y)
                    .ToList();

                contentLocations.ForEach(x => { text.AppendLine(x.Content); });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text.ToString()));
        }

        static bool IsBase64String(string input)
        {
            // Regular expression pattern to match a Base64 string
            string base64Pattern = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";

            // Check if the input string matches the pattern
            return Regex.IsMatch(input, base64Pattern);
        }
    }
}
