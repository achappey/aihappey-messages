using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Messages;

public sealed class MessagesDiagnosticsJsonConverter : JsonConverter<MessagesDiagnostics>
{
    public override MessagesDiagnostics? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException("Messages diagnostics must be a JSON object.");
        }

        var diagnostics = new MessagesDiagnostics();
        foreach (var property in root.EnumerateObject())
        {
            switch (property.Name)
            {
                case "previous_message_id":
                    diagnostics.PreviousMessageId = property.Value.ValueKind is JsonValueKind.Null
                        ? null
                        : property.Value.GetString();
                    break;
                case "cache_miss_reason":
                    ReadCacheMissReason(property.Value, diagnostics, options);
                    break;
                default:
                    diagnostics.AdditionalProperties ??= [];
                    diagnostics.AdditionalProperties[property.Name] = property.Value.Clone();
                    break;
            }
        }

        return diagnostics;
    }

    public override void Write(
        Utf8JsonWriter writer,
        MessagesDiagnostics value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.HasPreviousMessageId)
        {
            writer.WritePropertyName("previous_message_id");
            if (value.PreviousMessageId is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.PreviousMessageId);
            }
        }

        if (value.HasCacheMissReason)
        {
            writer.WritePropertyName("cache_miss_reason");
            if (value.CacheMissReasonDetails is not null)
            {
                JsonSerializer.Serialize(writer, value.CacheMissReasonDetails, options);
            }
            else if (value.CacheMissReasonRaw is { } raw)
            {
                raw.WriteTo(writer);
            }
            else if (value.CacheMissReason is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.CacheMissReason);
            }
        }

        if (value.AdditionalProperties is not null)
        {
            foreach (var property in value.AdditionalProperties)
            {
                if (property.Key is "previous_message_id" or "cache_miss_reason")
                {
                    continue;
                }

                writer.WritePropertyName(property.Key);
                property.Value.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
    }

    private static void ReadCacheMissReason(
        JsonElement element,
        MessagesDiagnostics diagnostics,
        JsonSerializerOptions options)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                diagnostics.CacheMissReason = element.GetString();
                break;
            case JsonValueKind.Object:
                diagnostics.CacheMissReasonDetails =
                    element.Deserialize<MessagesCacheMissReason>(options);
                diagnostics.CacheMissReason = diagnostics.CacheMissReasonDetails?.Type;
                break;
            default:
                diagnostics.CacheMissReasonRaw = element.Clone();
                break;
        }
    }
}
