namespace Visitz.Controls;

/// <summary>
/// <para>Options for how EditorEx should count characters.</para>
/// <para>Some upstream APIs are configured at the database field level to count
/// varchar bytes instead of chars. Meaning: they count the literal JSON
/// string, including the backslashes used for escape characters.</para>
/// </summary>
internal enum CharacterCountStyle
{
    Unknown = 0,
    Characters = 1,
    JsonForRestApi = 2,
}
