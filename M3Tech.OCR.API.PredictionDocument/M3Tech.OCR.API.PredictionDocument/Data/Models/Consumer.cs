using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class Consumer
{
    public int ConsumerId { get; set; }

    public string ConsumerNameEn { get; set; }

    public string ConsumerNameFr { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}
