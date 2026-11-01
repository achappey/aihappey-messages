using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Messages;

public sealed class MessagesResponse
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }

    [JsonPropertyName("container")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesContainer? Container { get; set; }

    [JsonPropertyName("content")]
    public List<MessageContentBlock> Content { get; set; } = [];

    [JsonPropertyName("diagnostics")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesDiagnostics? Diagnostics { get; set; }

    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Model { get; set; }

    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; }

    [JsonPropertyName("stop_details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesStopDetails? StopDetails { get; set; }

    [JsonPropertyName("stop_reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? StopReason { get; set; }

    [JsonPropertyName("stop_sequence")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? StopSequence { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    [JsonPropertyName("usage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesUsage? Usage { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }

    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, JsonElement>? Metadata { get; set; }
}

public sealed class MessagesErrorResponse
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("error")]
    public required MessagesApiError Error { get; set; }

    [JsonPropertyName("request_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public required string? RequestId { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesApiError
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesContainer
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }

    [JsonPropertyName("expires_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ExpiresAt { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesStopDetails
{
    [JsonPropertyName("category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Category { get; set; }

    [JsonPropertyName("explanation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Explanation { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesUsage
{
    [JsonPropertyName("cache_creation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesCacheCreation? CacheCreation { get; set; }

    [JsonPropertyName("cache_creation_input_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CacheCreationInputTokens { get; set; }

    [JsonPropertyName("cache_read_input_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CacheReadInputTokens { get; set; }

    [JsonPropertyName("inference_geo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InferenceGeo { get; set; }

    [JsonPropertyName("input_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? InputTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? OutputTokens { get; set; }

    [JsonPropertyName("output_tokens_details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesOutputTokensDetails? OutputTokensDetails { get; set; }

    [JsonPropertyName("server_tool_use")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MessagesServerToolUsage? ServerToolUse { get; set; }

    [JsonPropertyName("service_tier")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ServiceTier { get; set; }

    [JsonPropertyName("speed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Speed { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesCacheCreation
{
    [JsonPropertyName("ephemeral_1h_input_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Ephemeral1hInputTokens { get; set; }

    [JsonPropertyName("ephemeral_5m_input_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Ephemeral5mInputTokens { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesServerToolUsage
{
    [JsonPropertyName("web_fetch_requests")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? WebFetchRequests { get; set; }

    [JsonPropertyName("web_search_requests")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? WebSearchRequests { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

[JsonConverter(typeof(MessagesDiagnosticsJsonConverter))]
public sealed class MessagesDiagnostics
{
    private string? _previousMessageId;
    private string? _cacheMissReason;
    private MessagesCacheMissReason? _cacheMissReasonDetails;
    private JsonElement? _cacheMissReasonRaw;

    public string? PreviousMessageId
    {
        get => _previousMessageId;
        set
        {
            _previousMessageId = value;
            HasPreviousMessageId = true;
        }
    }

    public string? CacheMissReason
    {
        get => _cacheMissReason;
        set
        {
            _cacheMissReason = value;
            HasCacheMissReason = true;
        }
    }

    public MessagesCacheMissReason? CacheMissReasonDetails
    {
        get => _cacheMissReasonDetails;
        set
        {
            _cacheMissReasonDetails = value;
            HasCacheMissReason = true;
        }
    }

    public JsonElement? CacheMissReasonRaw
    {
        get => _cacheMissReasonRaw;
        set
        {
            _cacheMissReasonRaw = value;
            HasCacheMissReason = true;
        }
    }

    internal bool HasPreviousMessageId { get; private set; }

    internal bool HasCacheMissReason { get; private set; }

    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesCacheMissReason
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    [JsonPropertyName("cache_missed_input_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CacheMissedInputTokens { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessagesOutputTokensDetails
{
    [JsonPropertyName("thinking_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ThinkingTokens { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
