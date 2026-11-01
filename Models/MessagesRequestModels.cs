using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Messages;

public sealed class MessagesRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    [JsonPropertyName("max_tokens")]
    public required int MaxTokens { get; set; }

    [JsonPropertyName("messages")]
    public required List<MessageParam> Messages { get; set; }

    [JsonPropertyName("cache_control")]
    public CacheControlEphemeral? CacheControl { get; set; }

    [JsonPropertyName("container")]
    public JsonElement? Container { get; set; }

    [JsonPropertyName("inference_geo")]
    public string? InferenceGeo { get; set; }

    [JsonPropertyName("metadata")]
    public MessagesRequestMetadata? Metadata { get; set; }

    [JsonIgnore]
    public Dictionary<string, string>? Headers { get; set; }

    [JsonPropertyName("output_config")]
    public MessagesOutputConfig? OutputConfig { get; set; }

    [JsonPropertyName("service_tier")]
    public string? ServiceTier { get; set; }

    [JsonPropertyName("speed")]
    public string? Speed { get; set; }

    [JsonPropertyName("stop_sequences")]
    public List<string>? StopSequences { get; set; }

    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }

    [JsonPropertyName("system")]
    public MessagesContent? System { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("thinking")]
    public MessagesThinkingConfig? Thinking { get; set; }

    [JsonPropertyName("tool_choice")]
    public MessageToolChoice? ToolChoice { get; set; }

    [JsonPropertyName("context_management")]
    public MessagesContextManagementConfig? ContextManagement { get; set; }

    [JsonPropertyName("diagnostics")]
    public MessagesDiagnostics? Diagnostics { get; set; }

    [JsonPropertyName("compaction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesCompactionConfig? Compaction { get; set; }

    [JsonPropertyName("tools")]
    public List<MessageToolDefinition>? Tools { get; set; }

    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }

}

public sealed class MessageParam
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("content")]
    public required MessagesContent Content { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

[JsonConverter(typeof(MessagesContentJsonConverter))]
public sealed class MessagesContent
{
    public MessagesContent()
    {
    }

    public MessagesContent(string text)
    {
        Text = text;
    }

    public MessagesContent(List<MessageContentBlock> blocks)
    {
        Blocks = blocks;
    }

    public MessagesContent(JsonElement raw)
    {
        Raw = raw;
    }

    public string? Text { get; init; }

    public List<MessageContentBlock>? Blocks { get; init; }

    public JsonElement? Raw { get; init; }

    public bool IsText => Text is not null;

    public bool IsBlocks => Blocks is not null;

    public bool IsRaw => Raw is not null && Raw.Value.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined;
}

public sealed class MessagesRequestMetadata
{
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesOutputConfig
{
    [JsonPropertyName("effort")]
    public string? Effort { get; set; }

    [JsonPropertyName("format")]
    public MessagesOutputFormat? Format { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesOutputFormat
{
    [JsonPropertyName("schema")]
    public JsonElement? Schema { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesThinkingConfig
{
    [JsonPropertyName("budget_tokens")]
    public int? BudgetTokens { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "disabled";

    [JsonPropertyName("display")]
    public string? Display { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesCompactionConfig
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("instructions")]
    public string? Instructions { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesContextManagementConfig
{
    [JsonPropertyName("edits")]
    public List<MessagesContextManagementEdit>? Edits { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesContextManagementEdit
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("trigger")]
    public JsonElement? Trigger { get; set; }

    [JsonPropertyName("keep")]
    public JsonElement? Keep { get; set; }

    [JsonPropertyName("clear_at_least")]
    public JsonElement? ClearAtLeast { get; set; }

    [JsonPropertyName("clear_tool_inputs")]
    public JsonElement? ClearToolInputs { get; set; }

    [JsonPropertyName("exclude_tools")]
    public List<string>? ExcludeTools { get; set; }

    [JsonPropertyName("instructions")]
    public string? Instructions { get; set; }

    [JsonPropertyName("pause_after_compaction")]
    public bool? PauseAfterCompaction { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
