using System.Text.Json.Serialization;

namespace API.Services.Common
{
    public class ObjectResponse<T> : SuccessResponse where T : class
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
    }
}
