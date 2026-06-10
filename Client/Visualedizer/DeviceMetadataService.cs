using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Ledqualizer
{
    internal sealed class DeviceMetadata
    {
        public string Name { get; init; } = "Device";
        public int StripCount { get; init; }
        public int TotalLedCount { get; init; }
        public IReadOnlyList<DeviceStripMetadata> Strips { get; init; } = Array.Empty<DeviceStripMetadata>();
    }

    internal sealed class DeviceStripMetadata
    {
        public int Index { get; init; }
        public int LedCount { get; init; }
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

                    List<DeviceStripMetadata> strips = response.Strips?
                        .Where(strip => strip.LedCount > 0)
                        .Select(strip => new DeviceStripMetadata
                        {
                            Index = strip.Index,
                            LedCount = strip.LedCount
                        })
                        .OrderBy(strip => strip.Index)
                        .ToList() ?? new List<DeviceStripMetadata>();
                    int stripCount = strips.Count;
                    int totalLedCount = strips.Sum(strip => strip.LedCount);

                    if (totalLedCount <= 0)
                    {
                        List<int> legacyLedCounts = response.LedCounts?.Where(count => count > 0).ToList() ?? new List<int>();
                        strips = legacyLedCounts
                            .Select((count, index) => new DeviceStripMetadata
                            {
                                Index = index,
                                LedCount = count
                            })
                            .ToList();
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

                    if (strips.Count == 0)
                    {
                        stripCount = Math.Max(stripCount, 1);
                        strips = BuildFallbackStrips(totalLedCount, stripCount);
                    }
                    else
                    {
                        stripCount = strips.Count;
                    }

                    return new DeviceMetadata
                    {
                        Name = string.IsNullOrWhiteSpace(response.DeviceName) ? host : response.DeviceName,
                        StripCount = stripCount,
                        TotalLedCount = totalLedCount,
                        Strips = strips
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

        private static List<DeviceStripMetadata> BuildFallbackStrips(int totalLedCount, int stripCount)
        {
            int normalizedStripCount = Math.Max(stripCount, 1);
            int baseLedCount = totalLedCount / normalizedStripCount;
            int remainder = totalLedCount % normalizedStripCount;
            var strips = new List<DeviceStripMetadata>();

            for (int index = 0; index < normalizedStripCount; index++)
            {
                int ledCount = baseLedCount + (index < remainder ? 1 : 0);
                if (ledCount <= 0)
                {
                    continue;
                }

                strips.Add(new DeviceStripMetadata
                {
                    Index = index,
                    LedCount = ledCount
                });
            }

            return strips;
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

            [JsonPropertyName("ledOffset")]
            public int LedOffset { get; set; }

            [JsonPropertyName("ledShift")]
            public int LedShift { get; set; }

            [JsonPropertyName("dataPin")]
            public int DataPin { get; set; }
        }
    }
}
