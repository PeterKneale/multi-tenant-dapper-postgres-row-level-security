namespace Demo.Infrastructure.Repositories;

internal interface IJsonSerializer
{
    string ToJson(object listing);
    T FromJson<T>(string json);
}