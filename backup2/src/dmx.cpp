#include <WiFi.h>
#include "main.h"
#include "dmx.h"
// #include <FastLED.h>

const uint8_t DMX_MAX_UNIVERSE_COUNT = 255;
const uint16_t DMX_UNIVERSE_COUNT = 5;
const uint16_t DMX_CHANNEL_RED = 1;
const uint16_t DMX_CHANNEL_GREEN = 2;
const uint16_t DMX_CHANNEL_BLUE = 3;
#ifdef ESP32S3
  ESPAsyncE131 e131(DMX_MAX_UNIVERSE_COUNT);
  e131_packet_t packet;
#endif
uint16_t dmxUniverseCount;
uint16_t dmxUniverse;
bool dmxUnicast = true;
bool dmxEnabled = true;

const uint16_t DMX_POINTER_UNIVERSE = 1;
const uint16_t DMX_POINTER_CHANNEL = 1;
const uint16_t DMX_CHANNELS_PER_UNIVERSE = 512;
const uint16_t DMX_MAX_LED_COUNT_PER_UNIVERSE = 170;
uint32_t dmxLastPacketNum;

const int udpPort = 5568; // E1.31 port
WiFiUDP udp;

#ifdef ESP32S3
  void initDmx() {
    bool dmxResult;
    if (dmxUnicast) {
      // Unicast
      dmxResult = e131.begin(E131_UNICAST, dmxUniverse);
    } else {
      // Multicast
      dmxResult = e131.begin(E131_MULTICAST, dmxUniverse, dmxUniverseCount);
    }
    if (dmxResult) {
      Serial.println(F("DMX initialized, listening for incoming data"));
    } else {
      Serial.println(F("DMX initialization failed"));
    }
  }

  void processDmx() {
    if (!e131.isEmpty()) {
      // Serial.println("e131 is not empty");

      e131_packet_t packet;
      e131.pull(&packet);

      uint16_t universeIdx = htons(packet.universe);
      // Serial.printf("Universe index: %u\n", universeIdx);

      if (universeIdx == DVC_DMX_UNIVERSE) {
        for (int i = 0; i < ledCount; i++) {
          const int dmxChannelIdx = round(normalize(i, 0, ledCount - 1) * DMX_MAX_LED_COUNT_PER_UNIVERSE);
          leds[i] = CRGB(packet.property_values[dmxChannelIdx * 3 + 1], packet.property_values[dmxChannelIdx * 3 + 2], packet.property_values[dmxChannelIdx * 3 + 3]);
        }

        // Serial.printf("Count: %d", packet.property_value_count);

        FastLedShow();

        Serial.printf("Universe %u / %u Channels | Packet#: %u / Errors: %u / CH1: %u\n",
            htons(packet.universe),                 // The Universe for this packet
            htons(packet.property_value_count) - 1, // Start code is ignored, we're interested in dimmer data
            e131.stats.num_packets,                 // Packet counter
            e131.stats.packet_errors,               // Packet error counter
            packet.property_values[1]);             // Dimmer data for Channel 1
      }
    }
  }

#endif
