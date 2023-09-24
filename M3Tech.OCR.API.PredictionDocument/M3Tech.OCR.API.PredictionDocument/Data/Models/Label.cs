using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class Label
{
    public int LabelId { get; set; }

    public int DocumentCategoryId { get; set; }

    public virtual DocumentCategory DocumentCategory { get; set; }
}
