using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Messages;

public static class MessagesJson
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new MessagesContentJsonConverter()
        }
    };
}
