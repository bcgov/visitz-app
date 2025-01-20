namespace VisitzModel.Models;

public interface IApiJson<JsonType>
{
    JsonType ToApiJson(string dateFormat = "s");
}
