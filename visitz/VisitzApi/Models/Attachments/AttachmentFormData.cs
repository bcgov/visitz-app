using System.Net.Http.Headers;

namespace VisitzApi.Models.Attachments;

#nullable enable

public class AttachmentFormData : IDisposable
{
    static readonly string ContentName = "Attachment Id";
    static readonly string CategoryName = "Category";
    static readonly string DescriptionName = "Form Description";
    static readonly string StatusName = "Status";
    static readonly string TemplateName = "Template";

    public static readonly string DefaultStatus = "Complete";
    public static readonly string DefaultTemplate = "GENERICDOCUMENT";

    public string Filename { get; set; }

    public Stream FileContent { get; set; }

    public string ContentType { get; set; } = "";

    public string Category { get; set; } = "";

    public string Description { get; set; } = "";

    public string Status { get; set; } = DefaultStatus;

    public string Template { get; set; } = DefaultTemplate;


    public AttachmentFormData(
        string filename,
        Stream contentStream,
        string? contentType = null,
        string? category = null,
        string? description = null,
        string? status = null,
        string? template = null)
    {
        Filename = filename;
        FileContent = contentStream;

        if (contentType == null)
            ContentType = MimeTypes.GetMimeType(filename);

        if (category != null)
            Category = category;

        if (description != null)
            Description = description;

        if (status != null)
            Status = status;

        if (template != null)
            Template = template;
    }

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

    bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                FileContent?.Dispose();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
