using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EX.Core.Services
{
    public class GeminiAiClient : IGenerativeAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleAIOptions _options;
        private readonly ILogger<GeminiAiClient> _logger;

        public GeminiAiClient(IOptions<GoogleAIOptions> options, ILogger<GeminiAiClient> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_options.BaseUrl)
            };
        }

        public async Task<string> GenerateAsync(
            IList<(string role, string content)> messages,
            double temperature,
            int maxTokens,
            CancellationToken cancellationToken = default)
        {
            var model = string.IsNullOrWhiteSpace(_options.Model) ? "gemini-1.5-flash" : _options.Model;
            var requestUri = $"/v1beta/models/{model}:generateContent?key={_options.ApiKey}";

            var contents = messages.Select(m => new
            {
                role = m.role,
                parts = new[] { new { text = m.content } }
            }).ToArray();

            var payload = new
            {
                contents,
                generationConfig = new
                {
                    temperature = temperature,
                    maxOutputTokens = maxTokens
                }
            };

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = JsonContent.Create(payload, options: new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                };

                var res = await _httpClient.SendAsync(req, cancellationToken);
                var json = await res.Content.ReadAsStringAsync(cancellationToken);

                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Gemini API error: {StatusCode} - {Body}", (int)res.StatusCode, json);
                    throw new Exception($"Gemini API request failed: {(int)res.StatusCode}");
                }

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var candidates = root.GetProperty("candidates");
                if (candidates.ValueKind != JsonValueKind.Array || candidates.GetArrayLength() == 0)
                    return string.Empty;

                var content = candidates[0].GetProperty("content");
                var parts = content.GetProperty("parts");
                foreach (var part in parts.EnumerateArray())
                {
                    if (part.TryGetProperty("text", out var textNode))
                    {
                        return textNode.GetString() ?? string.Empty;
                    }
                }

                return string.Empty;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini GenerateAsync failed");
                throw;
            }
        }
    }
}