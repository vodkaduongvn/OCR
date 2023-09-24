using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class DocumentCategorizationLog
{
    public int CategorizationLogId { get; set; }

    public int DocumentId { get; set; }

    public int DocumentCategoryId { get; set; }

    public string CategorizationType { get; set; }

    public DateTime? CategorizationModelCalledDatetime { get; set; }

    public DateTime? CategorizationModelResponseDatetime { get; set; }

    public float CategorizationProbability { get; set; }

    public float CategorizationConfidence { get; set; }

    public string UpdateSource { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Document Document { get; set; }

    public virtual DocumentCategory DocumentCategory { get; set; }
}
