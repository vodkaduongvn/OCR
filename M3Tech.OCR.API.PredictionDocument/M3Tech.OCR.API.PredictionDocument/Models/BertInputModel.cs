namespace M3Tech.OCR.API.PredictionDocument.Models
{
    public class BertInputModel
    {
        public long[] InputIds { get; set; }
        public long[] AttentionMask { get; set; }
        public long[] TypeIds { get; set; }
    }
}
