#include "auxiliary_output.h"

#include <Arduino.h>

#include "main.h"

#ifdef ESP32S3
  #include <esp_dmx.h>
#endif

namespace {
constexpr uint8_t AUX_MAGIC[] = {'V', 'A', 'U', 'X'};
constexpr size_t AUX_HEADER_LENGTH = 7;
constexpr uint8_t AUX_VERSION = 1;
constexpr size_t AUX_DMX_PACKET_SIZE = 513;

#ifdef ESP32S3
const dmx_port_t dmxPort = DMX_NUM_1;
uint8_t dmxData[DMX_PACKET_SIZE] = {0};
bool dmxDriverInstalled = false;

void initLaserOutput() {
  if (!laserEnabled || dmxDriverInstalled) {
    return;
  }

  dmx_config_t config = DMX_CONFIG_DEFAULT;
  const int personalityCount = 0;
  dmx_driver_install(dmxPort, &config, nullptr, personalityCount);
  dmx_set_pin(dmxPort, laserTxPin, laserRxPin, laserEnablePin);
  dmxDriverInstalled = true;
}

void sendDmxState() {
  if (!laserEnabled || !dmxDriverInstalled) {
    return;
  }

  dmx_write(dmxPort, dmxData, DMX_PACKET_SIZE);
  dmx_send_num(dmxPort, DMX_PACKET_SIZE);
  dmx_wait_sent(dmxPort, DMX_TIMEOUT_TICK);
}
#else
uint8_t dmxData[AUX_DMX_PACKET_SIZE] = {0};
#endif

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
}  // namespace

bool isAuxiliaryBinaryFrame(const uint8_t* payload, size_t length) {
  if (payload == nullptr || length < AUX_HEADER_LENGTH) {
    return false;
  }

  for (size_t i = 0; i < sizeof(AUX_MAGIC); i++) {
    if (payload[i] != AUX_MAGIC[i]) {
      return false;
    }
  }

  return payload[4] == AUX_VERSION;
}

void initAuxiliaryOutputs() {
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
  if (!isAuxiliaryBinaryFrame(payload, length)) {
    return;
  }

  const uint8_t strobeState = payload[5];
  const uint8_t pairCount = payload[6];
  const size_t expectedLength = AUX_HEADER_LENGTH + static_cast<size_t>(pairCount) * 3;
  if (length != expectedLength) {
    return;
  }

  clearDmxState();

  if (laserEnabled) {
    for (size_t i = 0; i < pairCount; i++) {
      const size_t offset = AUX_HEADER_LENGTH + i * 3;
      const uint16_t channel = static_cast<uint16_t>(payload[offset])
          | (static_cast<uint16_t>(payload[offset + 1]) << 8);

      if (channel == 0 || channel >= sizeof(dmxData)) {
        continue;
      }

      dmxData[channel] = payload[offset + 2];
    }
  }

#ifdef ESP32S3
  sendDmxState();
#endif

  setStrobeState(strobeState > 0);
}

void clearAuxiliaryOutputs() {
  clearDmxState();

#ifdef ESP32S3
  sendDmxState();
#endif

  setStrobeState(false);
}
