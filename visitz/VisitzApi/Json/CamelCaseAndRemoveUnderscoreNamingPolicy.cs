using System.Text.Json;

namespace VisitzApi.Json;

internal class CamelCaseAndRemoveUnderscoreNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name)
	{
		return CamelCase.ConvertName(name).Trim('_');
	}
}
