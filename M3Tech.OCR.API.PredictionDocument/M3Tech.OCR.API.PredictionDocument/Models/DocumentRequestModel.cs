namespace M3Tech.OCR.API.PredictionDocument.Models
{
    public class DocumentRequestModel
    {
        public string ServerAddress { get; set; } = null;
        public string ServerFolder { get; set; } = null;
        public string DocumentName { get; set; }  
        public DateTime CategorizationDatetime { get; set; }
    }
}
