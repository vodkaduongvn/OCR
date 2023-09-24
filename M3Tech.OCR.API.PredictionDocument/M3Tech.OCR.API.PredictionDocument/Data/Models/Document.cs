using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public string ServerAddress { get; set; }

    public string ServerFolder { get; set; }

    public string DocumentName { get; set; }

    public string EncodedContent { get; set; }

    public DateTime? ExtractionStartDatetime { get; set; }

    public DateTime? ExtractionFinishDatetime { get; set; }

    public int DocumentCategoryId { get; set; }

    public DateTime? CategorizationDatetime { get; set; }

    public bool? IsDeleted { get; set; }

    public int? LastCategorizationLogId { get; set; }

    public virtual ICollection<DocumentCategorizationLog> DocumentCategorizationLogs { get; set; } = new List<DocumentCategorizationLog>();

    public virtual DocumentCategory DocumentCategory { get; set; }
}
