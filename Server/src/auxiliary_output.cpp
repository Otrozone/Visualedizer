#include "auxiliary_output.h"

#include <Arduino.h>

#include "main.h"

#ifdef ESP32S3
  #include <esp_dmx.h>
#endif

namespace {
constexpr uint8_t AUX_MAGIC[] = {'V', 'A', 'U', 'X'};
constexpr size_t AUX_V1_HEADER_LENGTH = 7;
constexpr size_t AUX_V2_HEADER_LENGTH = 9;
constexpr uint8_t AUX_VERSION_1 = 1;
constexpr uint8_t AUX_VERSION_2 = 2;
constexpr uint8_t AUX_FLAG_STROBE_ENABLED = 0x01;
constexpr uint8_t AUX_FLAG_SEND_FULL_DMX = 0x02;
constexpr size_t AUX_DMX_PACKET_SIZE = 513;
constexpr uint16_t AUX_DMX_MAX_CHANNEL = 512;
constexpr uint16_t AUX_DMX_DEFAULT_COMPACT_CHANNEL_COUNT = 6;
constexpr unsigned long AUX_DMX_REFRESH_INTERVAL_MS = 25;
constexpr unsigned long AUX_DMX_ERROR_LOG_INTERVAL_MS = 1000;
constexpr uint16_t AUX_DMX_STREAM_LOG_TICK_INTERVAL = 200;
constexpr size_t AUX_DMX_STREAM_LOG_CHANNEL_COUNT = 10;

struct AuxiliaryFrameHeader {
  uint8_t version = 0;
  bool strobeEnabled = false;
  bool sendFullDmx = false;
  uint8_t pairCount = 0;
  uint16_t requestedChannelCount = AUX_DMX_DEFAULT_COMPACT_CHANNEL_COUNT;
  size_t headerLength = 0;
};

size_t dmxSendSize = static_cast<size_t>(AUX_DMX_DEFAULT_COMPACT_CHANNEL_COUNT) + 1;

#ifdef ESP32S3
const dmx_port_t dmxPort = DMX_NUM_1;
uint8_t dmxData[DMX_PACKET_SIZE] = {0};
bool dmxDriverInstalled = false;
unsigned long lastDmxSendMs = 0;
unsigned long lastDmxErrorLogMs = 0;
uint16_t dmxStreamTickCount = 0;
bool dmxErrorLogged = false;

void initLaserOutput() {
  if (!laserEnabled || dmxDriverInstalled) {
    return;
  }

  Serial.print(F("Initializing laser DMX output on TX="));
  Serial.print(laserTxPin);
  Serial.print(F(", RX disabled, enable="));
  Serial.print(laserEnablePin >= 0 ? String(laserEnablePin) : F("disabled"));
  Serial.println();

  dmx_config_t config = DMX_CONFIG_DEFAULT;
  const int personalityCount = 0;
  if (!dmx_driver_install(dmxPort, &config, nullptr, personalityCount)) {
    Serial.println(F("Laser DMX driver installation failed"));
    return;
  }

  const int enablePin = laserEnablePin >= 0 ? laserEnablePin : DMX_PIN_NO_CHANGE;
  if (!dmx_set_pin(dmxPort, laserTxPin, DMX_PIN_NO_CHANGE, enablePin)) {
    Serial.println(F("Laser DMX pin assignment failed"));
    dmx_driver_delete(dmxPort);
    return;
  }

  dmxDriverInstalled = true;
  Serial.println(F("Laser DMX output initialized"));
}

bool shouldLogDmxError() {
  const unsigned long now = millis();
  if (dmxErrorLogged && now - lastDmxErrorLogMs < AUX_DMX_ERROR_LOG_INTERVAL_MS) {
    return false;
  }

  lastDmxErrorLogMs = now;
  dmxErrorLogged = true;
  return true;
}

bool sendDmxState(bool logSuccess) {
  if (!laserEnabled || !dmxDriverInstalled) {
    return false;
  }

  size_t sendSize = dmxSendSize;
  if (sendSize < 1) {
    sendSize = 1;
  }
  if (sendSize > DMX_PACKET_SIZE) {
    sendSize = DMX_PACKET_SIZE;
  }

  if (logSuccess) {
    Serial.print(F("Auxiliary output sending DMX data, bytes: "));
    Serial.println(sendSize);
  }

  const size_t written = dmx_write(dmxPort, dmxData, sendSize);
  if (written != sendSize) {
    if (shouldLogDmxError()) {
      Serial.print(F("Laser DMX write failed, written bytes: "));
      Serial.println(written);
    }
    return false;
  }

  const size_t scheduledSize = dmx_send_num(dmxPort, sendSize);
  if (scheduledSize != sendSize) {
    if (shouldLogDmxError()) {
      Serial.print(F("Laser DMX send failed, scheduled bytes: "));
      Serial.println(scheduledSize);
    }
    return false;
  }

  if (!dmx_wait_sent(dmxPort, DMX_TIMEOUT_TICK)) {
    if (shouldLogDmxError()) {
      Serial.println(F("Laser DMX wait_sent timed out"));
    }
    return false;
  }

  lastDmxSendMs = millis();
  dmxErrorLogged = false;
  return true;
}
#else
uint8_t dmxData[AUX_DMX_PACKET_SIZE] = {0};
#endif

bool hasAuxiliaryMagic(const uint8_t* payload, size_t length) {
  if (payload == nullptr || length < AUX_V1_HEADER_LENGTH) {
    return false;
  }

  for (size_t i = 0; i < sizeof(AUX_MAGIC); i++) {
    if (payload[i] != AUX_MAGIC[i]) {
      return false;
    }
  }

  return true;
}

bool parseAuxiliaryFrameHeader(const uint8_t* payload, size_t length, AuxiliaryFrameHeader& frame) {
  if (!hasAuxiliaryMagic(payload, length)) {
    return false;
  }

  frame.version = payload[4];
  if (frame.version == AUX_VERSION_1) {
    frame.strobeEnabled = payload[5] != 0;
    frame.sendFullDmx = true;
    frame.pairCount = payload[6];
    frame.requestedChannelCount = AUX_DMX_MAX_CHANNEL;
    frame.headerLength = AUX_V1_HEADER_LENGTH;
    return true;
  }

  if (frame.version == AUX_VERSION_2) {
    if (length < AUX_V2_HEADER_LENGTH) {
      return false;
    }

    const uint8_t flags = payload[5];
    frame.strobeEnabled = (flags & AUX_FLAG_STROBE_ENABLED) != 0;
    frame.sendFullDmx = (flags & AUX_FLAG_SEND_FULL_DMX) != 0;
    frame.pairCount = payload[6];
    frame.requestedChannelCount = static_cast<uint16_t>(payload[7])
        | (static_cast<uint16_t>(payload[8]) << 8);
    frame.headerLength = AUX_V2_HEADER_LENGTH;
    return true;
  }

  return false;
}

uint16_t resolveDmxChannelCount(
    bool sendFullDmx,
    uint16_t requestedChannelCount,
    uint16_t highestPairChannel) {
  if (sendFullDmx) {
    return AUX_DMX_MAX_CHANNEL;
  }

  uint16_t channelCount = requestedChannelCount;
  if (channelCount < AUX_DMX_DEFAULT_COMPACT_CHANNEL_COUNT) {
    channelCount = AUX_DMX_DEFAULT_COMPACT_CHANNEL_COUNT;
  }
  if (channelCount < highestPairChannel) {
    channelCount = highestPairChannel;
  }
  if (channelCount > AUX_DMX_MAX_CHANNEL) {
    channelCount = AUX_DMX_MAX_CHANNEL;
  }

  return channelCount;
}

size_t transmittedDmxChannelCount() {
  size_t sendSize = dmxSendSize;
  if (sendSize > sizeof(dmxData)) {
    sendSize = sizeof(dmxData);
  }

  return sendSize > 0 ? sendSize - 1 : 0;
}

void setStrobeState(bool enabled) {
  if (!strobeEnabled) {
    return;
  }

  digitalWrite(strobePin, enabled ? HIGH : LOW);
}

void clearDmxState() {
  for (size_t i = 0; i < sizeof(dmxData); i++) {
    dmxData[i] = 0;
  }
}

void logDmxPayload() {
  const size_t channelLimit = transmittedDmxChannelCount();
  Serial.print(F("Auxiliary DMX payload: ["));
  for (size_t channel = 1; channel <= channelLimit; channel++) {
    Serial.print(dmxData[channel]);
    if (channel < channelLimit) {
      Serial.print(',');
    }
  }
  Serial.println(']');
}

void logDmxStreamPayload() {
  size_t channelLimit = transmittedDmxChannelCount();
  if (channelLimit > AUX_DMX_STREAM_LOG_CHANNEL_COUNT) {
    channelLimit = AUX_DMX_STREAM_LOG_CHANNEL_COUNT;
  }

  Serial.print(F("Auxiliary DMX stream ch1-"));
  Serial.print(channelLimit);
  Serial.print(F(": ["));
  for (size_t channel = 1; channel <= channelLimit; channel++) {
    Serial.print(dmxData[channel]);
    if (channel < channelLimit) {
      Serial.print(',');
    }
  }
  Serial.println(']');
}
}  // namespace

bool isAuxiliaryBinaryFrame(const uint8_t* payload, size_t length) {
  AuxiliaryFrameHeader frame;
  return parseAuxiliaryFrameHeader(payload, length, frame);
}

void initAuxiliaryOutputs() {
  Serial.println(F("Initializing auxiliary outputs"));

  if (strobeEnabled) {
    pinMode(strobePin, OUTPUT);
    digitalWrite(strobePin, LOW);
  }

#ifdef ESP32S3
  initLaserOutput();
#endif

  clearAuxiliaryOutputs();
}

void applyAuxiliaryBinaryFrame(const uint8_t* payload, size_t length) {
  AuxiliaryFrameHeader frame;
  if (!parseAuxiliaryFrameHeader(payload, length, frame)) {
    return;
  }

  const size_t expectedLength = frame.headerLength + static_cast<size_t>(frame.pairCount) * 3;
  if (length != expectedLength) {
    return;
  }

  clearDmxState();

  uint16_t highestPairChannel = 0;
  if (laserEnabled) {
    for (size_t i = 0; i < frame.pairCount; i++) {
      const size_t offset = frame.headerLength + i * 3;
      const uint16_t channel = static_cast<uint16_t>(payload[offset])
          | (static_cast<uint16_t>(payload[offset + 1]) << 8);

      if (channel == 0 || channel >= sizeof(dmxData)) {
        continue;
      }

      dmxData[channel] = payload[offset + 2];
      if (channel > highestPairChannel) {
        highestPairChannel = channel;
      }
    }
  }

  const uint16_t effectiveChannelCount = resolveDmxChannelCount(
      frame.sendFullDmx,
      frame.requestedChannelCount,
      highestPairChannel);
  dmxSendSize = static_cast<size_t>(effectiveChannelCount) + 1;

  Serial.print(F("Auxiliary output received frame, length: "));
  Serial.print(length);
  Serial.print(F(", version: "));
  Serial.print(frame.version);
  Serial.print(F(", pairs: "));
  Serial.print(frame.pairCount);
  Serial.print(F(", mode: "));
  Serial.print(frame.sendFullDmx ? F("full") : F("compact"));
  Serial.print(F(", requested channels: "));
  Serial.print(frame.requestedChannelCount);
  Serial.print(F(", send bytes: "));
  Serial.println(dmxSendSize);

  logDmxPayload();

#ifdef ESP32S3
  sendDmxState(true);
#endif

  setStrobeState(frame.strobeEnabled);
}

void clearAuxiliaryOutputs() {
  clearDmxState();
  dmxSendSize = static_cast<size_t>(AUX_DMX_DEFAULT_COMPACT_CHANNEL_COUNT) + 1;

#ifdef ESP32S3
  sendDmxState(true);
#endif

  setStrobeState(false);
}

void pollAuxiliaryOutputs() {
#ifdef ESP32S3
  if (!laserEnabled || !dmxDriverInstalled) {
    return;
  }

  const unsigned long now = millis();
  if (now - lastDmxSendMs >= AUX_DMX_REFRESH_INTERVAL_MS) {
    if (sendDmxState(false)) {
      dmxStreamTickCount++;
      if (dmxStreamTickCount >= AUX_DMX_STREAM_LOG_TICK_INTERVAL) {
        dmxStreamTickCount = 0;
        logDmxStreamPayload();
      }
    }
  }
#endif
}
