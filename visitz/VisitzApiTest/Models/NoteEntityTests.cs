using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models;

namespace VisitzApiTest.Models;

public class NoteEntityTests
{
    private const string IncidentNote = @"
{
  ""notePeriod"": """",
  ""createdDate"": ""2024-Apr-29 12:00:00 PM"",
  ""notes"": ""Incident notes in april""
}";

    private const string CaseNote = @"
{
  ""notePeriod"": ""Apr 2024"",
  ""createdDate"": ""2024-Apr-29 12:00:00 PM"",
  ""notes"": ""Case notes in april""
}";

    private const string ArbitraryNotePeriod = "Jun 2024";

    private const string ArbitraryCreatedDate = "2024-Jan-13 12:00:00 PM";

    private const string ArbitraryNoteContent = "Some notes";

    private static NoteEntity? ParseNoteJson(string noteJson)
    {
        return JsonSerializer.Deserialize<NoteEntity>(noteJson, PayloadOptions.Default);
    }

    [Theory]
    [InlineData(IncidentNote)]
    [InlineData(CaseNote)]
    public void CanParseNoteJson(string noteJson)
    {
        Assert.NotNull(ParseNoteJson(noteJson));
    }

    [Fact]
    public void NotePeriodDateTimeTransform_ParsesCorrectly()
    {
        var arbitraryAscending = true;
        var note = new NoteEntity()
        {
            NotePeriod = ArbitraryNotePeriod,
            CreatedDate = ArbitraryCreatedDate,
            Content = ArbitraryNoteContent,
        };

        var expectedDateTime = DateTime.Parse(ArbitraryNotePeriod);
        var transformedNotePeriod = NoteEntity.NotePeriodDateTimeTransform(note, arbitraryAscending);

        Assert.Equal(expectedDateTime, transformedNotePeriod);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NotePeriodDateTimeTransform_DefaultAscending(bool ascending)
    {
        string emptyNotePeriod = string.Empty;

        var note = new NoteEntity()
        {
            NotePeriod = emptyNotePeriod,
            CreatedDate = ArbitraryCreatedDate,
            Content = ArbitraryNoteContent,
        };

        var transformedNotePeriod = NoteEntity.NotePeriodDateTimeTransform(note, ascending);

        var expectedDateTime = ascending
            ? DateTime.MinValue
            : DateTime.MaxValue;

        Assert.Equal(expectedDateTime, transformedNotePeriod);
    }

    [Fact]
    public void CreatedDateTimeTransform_ParsesCorrectly()
    {
        var arbitraryAscending = true;
        var note = new NoteEntity()
        {
            NotePeriod = ArbitraryNotePeriod,
            CreatedDate = ArbitraryCreatedDate,
            Content = ArbitraryNoteContent,
        };

        var expectedDateTime = DateTime.Parse(ArbitraryCreatedDate);
        var transformedDateCreated = NoteEntity.CreatedDateTimeTransform(note, arbitraryAscending);

        Assert.Equal(expectedDateTime, transformedDateCreated);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreatedDateTimeTransform_DefaultAscending(bool ascending)
    {
        string emptyDateCreated = string.Empty;

        var note = new NoteEntity()
        {
            NotePeriod = ArbitraryNotePeriod,
            CreatedDate = emptyDateCreated,
            Content = ArbitraryNoteContent,
        };

        var transformedDateCreated = NoteEntity.CreatedDateTimeTransform(note, ascending);

        var expectedDateTime = ascending
            ? DateTime.MinValue
            : DateTime.MaxValue;

        Assert.Equal(expectedDateTime, transformedDateCreated);
    }
}
