using System.Text.Json;

namespace VisitzApi.Json;

public static class PayloadOptions
{
	public static readonly JsonSerializerOptions Default = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};
}
