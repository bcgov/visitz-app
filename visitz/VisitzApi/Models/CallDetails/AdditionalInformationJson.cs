using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisitzApi.Models.CallDetails;

public class AdditionalInformationJson
{
    public string Id { get; set; }
    public string AdditionalInformation { get; set; }
    public string Created { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    public string Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
    public string IncidentId { get; set; }
    public string SRId { get; set; }
    public string MemoId { get; set; }
}
