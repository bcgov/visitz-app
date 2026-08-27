using System.Text.Json.Serialization;

namespace VisitzApi.Models.Attachments;

public class AttachmentJson
{
    public string Id { get; set; } = string.Empty;
    public string ApplicationNo { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ClientFlag { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string FinalFlag { get; set; } = string.Empty;
    public string FormDescription { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;
    public string IncidentNo { get; set; } = string.Empty;
    public string Internal { get; set; } = string.Empty;
    public string NoIntervention { get; set; } = string.Empty;
    public string PortalVisible { get; set; } = string.Empty;
    public string ShowOnContact { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("Sub-Category")]
    public string SubCategory { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;

    [JsonPropertyName("FileAutoUpdFlg")]
    public string FileAutoUpdFlg { get; set; } = string.Empty;

    [JsonPropertyName("FileDate")]
    public string FileDate { get; set; } = string.Empty;

    [JsonPropertyName("FileDeferFlg")]
    public string FileDeferFlg { get; set; } = string.Empty;

    [JsonPropertyName("FileDockReqFlg")]
    public string FileDockReqFlg { get; set; } = string.Empty;

    [JsonPropertyName("FileDockStatFlg")]
    public string FileDockStatFlg { get; set; } = string.Empty;

    [JsonPropertyName("FileExt")]
    public string FileExt { get; set; } = string.Empty;

    [JsonPropertyName("FileSize")]
    public string FileSize { get; set; } = string.Empty;

    [JsonPropertyName("FileSrcPath")]
    public string FileSrcPath { get; set; } = string.Empty;

    [JsonPropertyName("FileSrcType")]
    public string FileSrcType { get; set; } = string.Empty;

    [JsonPropertyName("FileName")]
    public string FileName { get; set; } = string.Empty;
    public string MemoId { get; set; } = string.Empty;

    [JsonPropertyName("MemoNumber")]
    public string MemoNumber { get; set; } = string.Empty;

    [JsonPropertyName("SRId")]
    public string SrId { get; set; } = string.Empty;

    [JsonPropertyName("CreatedByName")]
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;

    [JsonPropertyName("UpdatedByName")]
    public string UpdatedByName { get; set; } = string.Empty;
    public string LastUpdatedDate { get; set; } = string.Empty;
    public string AttachmentId { get; set; } = string.Empty;
}
