using System.Net.Http.Headers;

namespace VisitzApi.Models.Attachments;

public class AttachmentFormData
{
    static readonly string ContentName = "Attachment Id";
    static readonly string CategoryName = "Category";
    static readonly string DescriptionName = "Form Description";
    static readonly string StatusName = "Status";
    static readonly string TemplateName = "Template";

    public static readonly string DefaultTemplate = "GENERICDOCUMENT";

    public string Filename { get; set; }

    public Stream FileContent { get; set; }

    public string ContentType { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public string Template { get; set; } = DefaultTemplate;

    public MultipartFormDataContent ToFormDataContent()
    {
        var streamContent = new StreamContent(FileContent);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

        return new MultipartFormDataContent()
        {
            { streamContent, ContentName, Filename },
            { new StringContent(Category), CategoryName },
            { new StringContent(Description), DescriptionName },
            { new StringContent(Status), StatusName },
            { new StringContent(Template), TemplateName },
        };
    }
}
