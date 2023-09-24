namespace M3Tech.OCR.API.PredictionDocument.Models
{
    public class PredictedResultModel
    {
        public string CategoryNameFR { get; set; }
        public string CategoryNameEN1 { get; set; }
        public string CategoryNameEN2 { get; set; }
        public string CategoryNameEN3 { get; set; }

        public string NBCIdentifier { get; set; }
        public int? CategoryID1 { get; set; }
        public int? CategoryID2 { get; set; }
        public int? CategoryID3 { get; set; }

        public float? Probability1 { get; set; }
        public float? Probability2 { get; set; }
        public float? Probability3 { get; set; }
        public string Confidence { get; set; }
    }
}
