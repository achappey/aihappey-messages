using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Messages;

public sealed class MessageCaller
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("tool_id")]
    public string? ToolId { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessageToolChoice
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "auto";

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("disable_parallel_tool_use")]
    public bool? DisableParallelToolUse { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessageToolDefinition
{
    [JsonPropertyName("input_schema")]
    public JsonElement? InputSchema { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("allowed_callers")]
    public List<string>? AllowedCallers { get; set; }

    [JsonPropertyName("cache_control")]
    public CacheControlEphemeral? CacheControl { get; set; }

    [JsonPropertyName("defer_loading")]
    public bool? DeferLoading { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("eager_input_streaming")]
    public bool? EagerInputStreaming { get; set; }

    [JsonPropertyName("input_examples")]
    public List<JsonElement>? InputExamples { get; set; }

    [JsonPropertyName("strict")]
    public bool? Strict { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("allowed_domains")]
    public List<string>? AllowedDomains { get; set; }

    [JsonPropertyName("blocked_domains")]
    public List<string>? BlockedDomains { get; set; }

    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    [JsonPropertyName("max_content_tokens")]
    public int? MaxContentTokens { get; set; }

    [JsonPropertyName("max_characters")]
    public int? MaxCharacters { get; set; }

    [JsonPropertyName("use_cache")]
    public bool? UseCache { get; set; }

    [JsonPropertyName("citations")]
    public CitationsConfigParam? Citations { get; set; }

    [JsonPropertyName("user_location")]
    public MessageUserLocation? UserLocation { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class MessageUserLocation
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class CitationsConfigParam
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
