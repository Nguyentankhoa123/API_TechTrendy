using System.Text.Json.Serialization;

namespace API.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiscountApply
    {
        All,
        Specific
    }
}
