namespace VisitzApi.Models.People;

public class SubmitContactLanguageJson
{
    public required string LanguageName { get; set; }

    public string? OtherLanguage { get; set; }

    public string? TranslatorReq { get; set; }

    public string? ICMType { get; set; }

    public string? Comments { get; set; }
}
