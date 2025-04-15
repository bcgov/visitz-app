using System.Text.Json.Serialization;

namespace VisitzApi.Models.Attachments;

public class SubmitAttachmentEntity
{
    public string AttachmentId { get; set; }
    public string EntityNumber { get; set; }
    public string EntityType { get; set; }
    public string CaseType { get; set; }
    public string FormName { get; set; }
    public string FileName { get; set; }
    public string FormDescription { get; set; }
    public string FormCategory { get; set; }
    public string Section13Exists { get; set; }
    public string InvestigationResponse { get; set; }
    public Section13Payload Section13 { get; set; }
    public CfaDetailsPayload CfaDetails { get; set; }
    public AttachmentBlock Attachment { get; set; }

    public class AttachmentBlock
    {
        [JsonPropertyName("PDFString")]
        public string PdfString { get; set; }
    }
}
