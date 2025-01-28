namespace VisitzModel.Interfaces;

public interface IApiJson<JsonType>
{
    JsonType ToApiJson(string dateFormat = "s");
}
