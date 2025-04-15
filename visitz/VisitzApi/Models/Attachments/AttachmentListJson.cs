using System.Text.Json.Serialization;

namespace VisitzApi.Models.Attachments;

public class AttachmentJson
{
    public string Id { get; set; }
    public string ApplicationNo { get; set; }
    public string Categorie { get; set; }
    public string Category { get; set; }
    public string ClientFlag { get; set; }
    public string EndDate { get; set; }
    public string FinalFlag { get; set; }
    public string FormDescription { get; set; }
    public string IncidentId { get; set; }
    public string IncidentNo { get; set; }
    public string Internal { get; set; }
    public string NoIntervention { get; set; }
    public string PortalVisible { get; set; }
    public string ShowOnContact { get; set; }
    public string Status { get; set; }
    [JsonPropertyName("Sub-Category")]
    public string SubCategory { get; set; }
    public string Template { get; set; }
    public string TemplateType { get; set; }
    public string CaseId { get; set; }
    public string Comments { get; set; }
    [JsonPropertyName("FileAutoUpdFlg")]
    public string FileAutoUpdFlg { get; set; }
    [JsonPropertyName("FileDate")]
    public string FileDate { get; set; }
    [JsonPropertyName("FileDeferFlg")]
    public string FileDeferFlg { get; set; }
    [JsonPropertyName("FileDockReqFlg")]
    public string FileDockReqFlg { get; set; }
    [JsonPropertyName("FileDockStatFlg")]
    public string FileDockStatFlg { get; set; }
    [JsonPropertyName("FileExt")]
    public string FileExt { get; set; }
    [JsonPropertyName("FileSize")]
    public string FileSize { get; set; }
    [JsonPropertyName("FileSrcPath")]
    public string FileSrcPath { get; set; }
    [JsonPropertyName("FileSrcType")]
    public string FileSrcType { get; set; }
    [JsonPropertyName("FileName")]
    public string FileName { get; set; }
    public string MemoId { get; set; }
    [JsonPropertyName("MemoNumber")]
    public string MemoNumber { get; set; }
    [JsonPropertyName("SRId")]
    public string SrId { get; set; }
    [JsonPropertyName("CreatedByName")]
    public string CreatedByName { get; set; }
    public string CreatedDate { get; set; }
    [JsonPropertyName("UpdatedByName")]
    public string UpdatedByName { get; set; }
    public string LastUpdatedDate { get; set; }
    public string AttachmentId { get; set; }
}
