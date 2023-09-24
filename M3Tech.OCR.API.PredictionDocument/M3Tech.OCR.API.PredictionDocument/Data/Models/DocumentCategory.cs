using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class DocumentCategory
{
    public int DocumentCategoryId { get; set; }

    public string DocumentNbcIdentifier { get; set; }

    public string DocumentCategoryNameEn { get; set; }

    public string DocumentCategoryNameFr { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<DocumentCategorizationLog> DocumentCategorizationLogs { get; set; } = new List<DocumentCategorizationLog>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();
}
