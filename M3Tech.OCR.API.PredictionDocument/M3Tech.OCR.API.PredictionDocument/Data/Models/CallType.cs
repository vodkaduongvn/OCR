using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class CallType
{
    public int CallTypeId { get; set; }

    public string CallNameEn { get; set; }

    public string CallNameFr { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}
