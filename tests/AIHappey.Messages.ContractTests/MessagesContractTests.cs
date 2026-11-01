using System.Text.Json;
using AIHappey.Messages;
using Xunit;

namespace AIHappey.Messages.ContractTests;

public sealed class MessagesContractTests
{
    [Fact]
    public void New_optional_fields_do_not_change_legacy_serialization()
    {
        var request = new MessagesRequest
        {
            Model = "claude-sonnet-4-5",
            MaxTokens = 100,
            Messages = []
        };

        var json = JsonSerializer.Serialize(request, MessagesJson.Default);

        using var document = JsonDocument.Parse(json);
        Assert.False(document.RootElement.TryGetProperty("speed", out _));
        Assert.False(document.RootElement.TryGetProperty("diagnostics", out _));
    }

    [Fact]
    public void Request_roundtrips_current_fields_content_blocks_and_unknown_properties()
    {
        const string json = """
            {
              "model": "claude-sonnet-4-5",
              "max_tokens": 250,
              "messages": [
                {
                  "role": "user",
                  "content": [
                    {
                      "type": "text",
                      "text": "Hi",
                      "future_block_value": { "x": 1 }
                    },
                    {
                      "type": "tool_result",
                      "tool_use_id": "toolu_1",
                      "content": "done",
                      "is_error": false
                    }
                  ]
                }
              ],
              "speed": "fast",
              "diagnostics": {
                "previous_message_id": "msg_previous",
                "future_diagnostic": "preserved"
              },
              "cache_control": {
                "type": "ephemeral",
                "ttl": "1h",
                "future_cache_setting": true
              },
              "future_request_option": { "enabled": true }
            }
            """;

        var request = JsonSerializer.Deserialize<MessagesRequest>(json, MessagesJson.Default)!;

        Assert.Equal("fast", request.Speed);
        Assert.Equal("msg_previous", request.Diagnostics!.PreviousMessageId);
        var blocks = request.Messages[0].Content.Blocks!;
        Assert.Equal("Hi", blocks[0].Text);
        Assert.Equal("done", blocks[1].Content!.Text);

        var roundTrip = JsonSerializer.Serialize(request, MessagesJson.Default);
        using var document = JsonDocument.Parse(roundTrip);
        var root = document.RootElement;
        Assert.Equal("fast", root.GetProperty("speed").GetString());
        Assert.Equal("preserved", root.GetProperty("diagnostics").GetProperty("future_diagnostic").GetString());
        Assert.True(root.GetProperty("cache_control").GetProperty("future_cache_setting").GetBoolean());
        Assert.Equal(1, root.GetProperty("messages")[0].GetProperty("content")[0]
            .GetProperty("future_block_value").GetProperty("x").GetInt32());
        Assert.True(root.GetProperty("future_request_option").GetProperty("enabled").GetBoolean());
    }

    [Fact]
    public void Response_roundtrips_diagnostics_usage_thinking_tools_and_unknown_fields()
    {
        const string json = """
            {
              "id": "msg_1",
              "type": "message",
              "role": "assistant",
              "content": [
                {
                  "type": "thinking",
                  "thinking": "Reasoning",
                  "signature": "signed"
                },
                {
                  "type": "tool_use",
                  "id": "toolu_1",
                  "name": "lookup",
                  "input": { "query": "weather" },
                  "future_tool_field": "preserved"
                },
                {
                  "type": "text",
                  "text": "Done",
                  "citations": [
                    { "type": "char_location", "cited_text": "Done" }
                  ]
                }
              ],
              "diagnostics": {
                "cache_miss_reason": "prefix_mismatch"
              },
              "usage": {
                "input_tokens": 10,
                "output_tokens": 5,
                "output_tokens_details": { "thinking_tokens": 2 },
                "speed": "fast",
                "cache_creation": {
                  "ephemeral_1h_input_tokens": 1,
                  "ephemeral_5m_input_tokens": 0,
                  "future_cache_metric": 3
                }
              },
              "future_response_field": "preserved"
            }
            """;

        var response = JsonSerializer.Deserialize<MessagesResponse>(json, MessagesJson.Default)!;

        Assert.Equal("prefix_mismatch", response.Diagnostics!.CacheMissReason);
        Assert.Equal(2, response.Usage!.OutputTokensDetails!.ThinkingTokens);
        Assert.Equal("fast", response.Usage.Speed);
        Assert.Equal("toolu_1", response.Content[1].Id);
        Assert.Equal("weather", response.Content[1].Input!.Value.GetProperty("query").GetString());

        var roundTrip = JsonSerializer.Serialize(response, MessagesJson.Default);
        using var document = JsonDocument.Parse(roundTrip);
        var root = document.RootElement;
        Assert.Equal("fast", root.GetProperty("usage").GetProperty("speed").GetString());
        Assert.Equal(2, root.GetProperty("usage").GetProperty("output_tokens_details")
            .GetProperty("thinking_tokens").GetInt32());
        Assert.Equal("prefix_mismatch", root.GetProperty("diagnostics")
            .GetProperty("cache_miss_reason").GetString());
        Assert.Equal(3, root.GetProperty("usage").GetProperty("cache_creation")
            .GetProperty("future_cache_metric").GetInt32());
        Assert.Equal("preserved", root.GetProperty("content")[1].GetProperty("future_tool_field").GetString());
        Assert.Equal("preserved", root.GetProperty("future_response_field").GetString());
    }

    [Fact]
    public void Structured_cache_diagnostics_preserve_details_and_explicit_null_request_id()
    {
        const string responseJson = """
            {
              "diagnostics": {
                "previous_message_id": "msg_previous",
                "cache_miss_reason": {
                  "type": "messages_changed",
                  "cache_missed_input_tokens": 128,
                  "future_detail": true
                }
              }
            }
            """;

        var response = JsonSerializer.Deserialize<MessagesResponse>(
            responseJson, MessagesJson.Default)!;
        var diagnostics = response.Diagnostics!;
        Assert.Equal("messages_changed", diagnostics.CacheMissReason);
        Assert.Equal(128, diagnostics.CacheMissReasonDetails!.CacheMissedInputTokens);

        using var responseRoundTrip = JsonDocument.Parse(
            JsonSerializer.Serialize(response, MessagesJson.Default));
        var responseDiagnostics = responseRoundTrip.RootElement.GetProperty("diagnostics");
        Assert.Equal("messages_changed", responseDiagnostics.GetProperty("cache_miss_reason")
            .GetProperty("type").GetString());
        Assert.True(responseDiagnostics.GetProperty("cache_miss_reason")
            .GetProperty("future_detail").GetBoolean());

        var pendingResponse = JsonSerializer.Deserialize<MessagesResponse>(
            """{"diagnostics":{"cache_miss_reason":null}}""", MessagesJson.Default)!;
        using var pendingRoundTrip = JsonDocument.Parse(
            JsonSerializer.Serialize(pendingResponse, MessagesJson.Default));
        Assert.Equal(JsonValueKind.Null, pendingRoundTrip.RootElement.GetProperty("diagnostics")
            .GetProperty("cache_miss_reason").ValueKind);

        var request = new MessagesRequest
        {
            Model = "claude-sonnet-4-5",
            MaxTokens = 100,
            Messages = [],
            Diagnostics = new MessagesDiagnostics { PreviousMessageId = null }
        };

        using var requestJson = JsonDocument.Parse(
            JsonSerializer.Serialize(request, MessagesJson.Default));
        Assert.True(requestJson.RootElement.GetProperty("diagnostics")
            .TryGetProperty("previous_message_id", out var previousMessageId));
        Assert.Equal(JsonValueKind.Null, previousMessageId.ValueKind);
    }

    [Fact]
    public void File_sources_and_api_errors_roundtrip_known_and_unknown_fields()
    {
        const string responseJson = """
            {
              "content": [{
                "type": "document",
                "source": {
                  "type": "file",
                  "file_id": "file_123",
                  "future_source_field": "preserved"
                }
              }]
            }
            """;

        var response = JsonSerializer.Deserialize<MessagesResponse>(
            responseJson, MessagesJson.Default)!;
        Assert.Equal("file_123", response.Content[0].Source!.FileId);

        using var responseRoundTrip = JsonDocument.Parse(
            JsonSerializer.Serialize(response, MessagesJson.Default));
        var source = responseRoundTrip.RootElement.GetProperty("content")[0]
            .GetProperty("source");
        Assert.Equal("file_123", source.GetProperty("file_id").GetString());
        Assert.Equal("preserved", source.GetProperty("future_source_field").GetString());

        const string errorJson = """
            {
              "type": "error",
              "error": {
                "type": "future_error_type",
                "message": "Request failed",
                "future_error_detail": { "retryable": true }
              },
              "request_id": "req_123",
              "future_envelope_field": 7
            }
            """;

        var error = JsonSerializer.Deserialize<MessagesErrorResponse>(
            errorJson, MessagesJson.Default)!;
        Assert.Equal("future_error_type", error.Error!.Type);
        Assert.Equal("req_123", error.RequestId);

        using var errorRoundTrip = JsonDocument.Parse(
            JsonSerializer.Serialize(error, MessagesJson.Default));
        var errorRoot = errorRoundTrip.RootElement;
        Assert.Equal("req_123", errorRoot.GetProperty("request_id").GetString());
        Assert.True(errorRoot.GetProperty("error").GetProperty("future_error_detail")
            .GetProperty("retryable").GetBoolean());
        Assert.Equal(7, errorRoot.GetProperty("future_envelope_field").GetInt32());

        var errorWithoutRequestId = JsonSerializer.Deserialize<MessagesErrorResponse>(
            """{"type":"error","error":{"type":"api_error","message":"Unavailable"},"request_id":null}""",
            MessagesJson.Default)!;
        using var errorWithoutRequestIdRoundTrip = JsonDocument.Parse(
            JsonSerializer.Serialize(errorWithoutRequestId, MessagesJson.Default));
        Assert.Equal(JsonValueKind.Null, errorWithoutRequestIdRoundTrip.RootElement
            .GetProperty("request_id").ValueKind);
    }

    [Fact]
    public void Stream_events_roundtrip_delta_usage_and_event_specific_fields()
    {
        const string json = """
            {
              "type": "message_delta",
              "delta": {
                "stop_reason": "end_turn",
                "stop_sequence": null,
                "stop_details": {
                  "type": "refusal",
                  "category": "general_harms",
                  "explanation": "Policy refusal"
                }
              },
              "usage": {
                "output_tokens": 4,
                "output_tokens_details": { "thinking_tokens": 1 }
              },
              "future_event_field": 9
            }
            """;

        var part = JsonSerializer.Deserialize<MessageStreamPart>(json, MessagesJson.Default)!;

        Assert.Equal("message_delta", part.Type);
        Assert.Equal("end_turn", part.Delta!.StopReason);
        Assert.Equal("general_harms", part.Delta.StopDetails!.Category);
        Assert.Equal(1, part.Usage!.OutputTokensDetails!.ThinkingTokens);

        var roundTrip = JsonSerializer.Serialize(part, MessagesJson.Default);
        using var document = JsonDocument.Parse(roundTrip);
        Assert.Equal(9, document.RootElement.GetProperty("future_event_field").GetInt32());
        Assert.Equal("end_turn", document.RootElement.GetProperty("delta").GetProperty("stop_reason").GetString());
        Assert.Equal("general_harms", document.RootElement.GetProperty("delta")
            .GetProperty("stop_details").GetProperty("category").GetString());
    }

    [Fact]
    public void Required_request_fields_are_enforced_during_deserialization()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<MessagesRequest>(
            """{"model":"claude-sonnet-4-5","max_tokens":100}""", MessagesJson.Default));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<MessagesRequest>(
            """{"max_tokens":100,"messages":[]}""", MessagesJson.Default));
    }

    [Fact]
    public void Named_tool_choice_and_toolset_name_roundtrip()
    {
        const string json = """
            {
              "model": "claude-sonnet-4-5",
              "max_tokens": 100,
              "messages": [{ "role": "user", "content": "Use the browser." }],
              "tool_choice": { "type": "tool", "name": "find", "disable_parallel_tool_use": true }
            }
            """;

        var request = JsonSerializer.Deserialize<MessagesRequest>(json, MessagesJson.Default)!;
        Assert.Equal("find", request.ToolChoice!.Name);

        var response = new MessagesResponse
        {
            Content =
            [
                new MessageContentBlock
                {
                    Type = "tool_use",
                    ToolsetName = "browser",
                    Name = "find",
                    Input = JsonDocument.Parse("""{"query":"search bar"}""").RootElement.Clone()
                }
            ]
        };

        var roundTrip = JsonSerializer.Serialize(response, MessagesJson.Default);
        using var document = JsonDocument.Parse(roundTrip);
        Assert.Equal("browser", document.RootElement.GetProperty("content")[0]
            .GetProperty("toolset_name").GetString());
    }

    [Fact]
    public void Beta_context_management_and_compaction_configs_roundtrip()
    {
        const string contextManagementJson = """
            {
              "model": "claude-sonnet-4-5",
              "max_tokens": 100,
              "messages": [{ "role": "user", "content": "Continue." }],
              "context_management": {
                "edits": [{
                  "type": "clear_tool_uses_20250919",
                  "trigger": { "type": "tool_uses", "value": 3 },
                  "keep": { "type": "tool_uses", "value": 1 },
                  "clear_tool_inputs": true,
                  "future_edit_option": "preserved"
                }]
              }
            }
            """;

        var contextRequest = JsonSerializer.Deserialize<MessagesRequest>(
            contextManagementJson, MessagesJson.Default)!;
        Assert.Equal("clear_tool_uses_20250919", contextRequest.ContextManagement!.Edits![0].Type);
        Assert.Equal(3, contextRequest.ContextManagement.Edits[0].Trigger!.Value
            .GetProperty("value").GetInt32());

        using var contextRoundTrip = JsonDocument.Parse(
            JsonSerializer.Serialize(contextRequest, MessagesJson.Default));
        Assert.Equal("preserved", contextRoundTrip.RootElement.GetProperty("context_management")
            .GetProperty("edits")[0].GetProperty("future_edit_option").GetString());

        const string compactionJson = """
            {
              "model": "claude-sonnet-4-5",
              "max_tokens": 100,
              "messages": [{ "role": "user", "content": "Summarize this context." }],
              "compaction": { "type": "summarize", "instructions": "Preserve decisions." }
            }
            """;

        var compactionRequest = JsonSerializer.Deserialize<MessagesRequest>(
            compactionJson, MessagesJson.Default)!;
        Assert.Equal("summarize", compactionRequest.Compaction!.Type);
        Assert.Equal("Preserve decisions.", compactionRequest.Compaction.Instructions);
    }

    [Fact]
    public void Messages_content_converter_preserves_string_array_and_raw_object_shapes()
    {
        var text = JsonSerializer.Deserialize<MessagesContent>(""" "hello" """, MessagesJson.Default)!;
        var blocks = JsonSerializer.Deserialize<MessagesContent>(
            """[{"type":"text","text":"hello"}]""", MessagesJson.Default)!;
        var raw = JsonSerializer.Deserialize<MessagesContent>(
            """{"future_shape":true}""", MessagesJson.Default)!;

        Assert.Equal("\"hello\"", JsonSerializer.Serialize(text, MessagesJson.Default));
        Assert.Equal("hello", blocks.Blocks![0].Text);
        Assert.Equal("""{"future_shape":true}""", JsonSerializer.Serialize(raw, MessagesJson.Default));
    }
}
