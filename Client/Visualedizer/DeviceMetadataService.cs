using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Ledqualizer
{
    internal sealed class DeviceMetadata
    {
        public string Name { get; init; } = "Device";
        public int StripCount { get; init; }
        public int TotalLedCount { get; init; }
        public IReadOnlyList<int> StripLedCounts { get; init; } = Array.Empty<int>();
    }

    internal sealed class DeviceMetadataService
    {
        private static readonly HttpClient HttpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(4)
        };

        public async Task<DeviceMetadata> FetchAsync(string host, int webSocketPort, CancellationToken cancellationToken)
        {
            foreach (Uri uri in BuildCandidateUris(host, webSocketPort))
            {
                try
                {
                    DeviceMetadataResponse? response = await HttpClient.GetFromJsonAsync<DeviceMetadataResponse>(uri, cancellationToken).ConfigureAwait(false);
                    if (response == null)
                    {
                        continue;
                    }

                    List<int> ledCounts = response.Strips?
                        .Select(strip => strip.LedCount)
                        .Where(count => count > 0)
                        .ToList() ?? new List<int>();
                    int stripCount = ledCounts.Count;
                    int totalLedCount = ledCounts.Sum();

                    if (totalLedCount <= 0)
                    {
                        List<int> legacyLedCounts = response.LedCounts?.Where(count => count > 0).ToList() ?? new List<int>();
                        ledCounts = legacyLedCounts;
                        stripCount = response.StripCount > 0 ? response.StripCount : legacyLedCounts.Count;
                        totalLedCount = response.TotalLedCount > 0 ? response.TotalLedCount : legacyLedCounts.Sum();
                        if (totalLedCount <= 0)
                        {
                            totalLedCount = response.LedCount;
                        }
                    }

                    if (totalLedCount <= 0)
                    {
                        continue;
                    }

                    return new DeviceMetadata
                    {
                        Name = string.IsNullOrWhiteSpace(response.DeviceName) ? host : response.DeviceName,
                        StripCount = stripCount,
                        TotalLedCount = totalLedCount,
                        StripLedCounts = ledCounts
                    };
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    // Try the next candidate URI.
                }
            }

            throw new InvalidOperationException("Unable to retrieve device metadata.");
        }

        private static IEnumerable<Uri> BuildCandidateUris(string host, int webSocketPort)
        {
            yield return new Uri($"http://{host}/get-conf");

            if (webSocketPort > 0 && webSocketPort != 80)
            {
                yield return new Uri($"http://{host}:{webSocketPort}/get-conf");
            }
        }

        private sealed class DeviceMetadataResponse
        {
            [JsonPropertyName("deviceName")]
            public string? DeviceName { get; set; }

            [JsonPropertyName("strips")]
            public List<StripMetadataResponse>? Strips { get; set; }

            [JsonPropertyName("ledCount")]
            public int LedCount { get; set; }

            [JsonPropertyName("ledCounts")]
            public List<int>? LedCounts { get; set; }

            [JsonPropertyName("stripCount")]
            public int StripCount { get; set; }

            [JsonPropertyName("totalLedCount")]
            public int TotalLedCount { get; set; }
        }

        private sealed class StripMetadataResponse
        {
            [JsonPropertyName("index")]
            public int Index { get; set; }

            [JsonPropertyName("ledCount")]
            public int LedCount { get; set; }
        }
    }
}
