using Newtonsoft.Json;

namespace Demo.Infrastructure.Repositories;

internal class JsonSerializer : IJsonSerializer
{
    public string ToJson(object o)
    {
        return JsonConvert.SerializeObject(o);
    }

    public T FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings());
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        return new()
        {
            ContractResolver = new JsonPrivateContractResolver()
        };
    }
}