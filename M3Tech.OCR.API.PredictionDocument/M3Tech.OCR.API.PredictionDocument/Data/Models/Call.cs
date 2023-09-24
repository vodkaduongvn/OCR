using System;
using System.Collections.Generic;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class Call
{
    public int CallId { get; set; }

    public int? ConsumerId { get; set; }

    public int? CallTypeId { get; set; }

    public DateTime? CallReceivedDatetime { get; set; }

    public DateTime? CallRespondedDatetime { get; set; }

    public string JsonReceived { get; set; }

    public string JsonProvided { get; set; }

    public string ErrorFound { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual CallType CallType { get; set; }

    public virtual Consumer Consumer { get; set; }
}
