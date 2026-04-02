using System.Text.Json.Serialization;

namespace VisitzApi.Models.Attachments;

public class SubmitAttachmentEntity
{
    public string AttachmentId { get; set; } = string.Empty;
    public string EntityNumber { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public string FormName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FormDescription { get; set; } = string.Empty;
    public string FormCategory { get; set; } = string.Empty;
    public string Section13Exists { get; set; } = string.Empty;
    public string InvestigationResponse { get; set; } = string.Empty;
    public Section13Payload? Section13 { get; set; }
    public CfaDetailsPayload? CfaDetails { get; set; }
    public AttachmentBlock? Attachment { get; set; }

    public class AttachmentBlock
    {
        [JsonPropertyName("PDFString")]
        public string PdfString { get; set; } = string.Empty;
    }
}
