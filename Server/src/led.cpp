#include <freertos/FreeRTOS.h>
#include <freertos/semphr.h>

#include "led.h"
#include "main.h"
#include "nvm.h"
#include "render_service.h"

namespace {
SemaphoreHandle_t renderMutex = nullptr;

String getStripLedCountKey(int stripIdx) {
  return "strip" + String(stripIdx) + "LedCount";
}

String getStripLedOffsetKey(int stripIdx) {
  return "strip" + String(stripIdx) + "LedOffset";
}

String getStripLedShiftKey(int stripIdx) {
  return "strip" + String(stripIdx) + "LedShift";
}

uint16_t getConfiguredStripLedCount(int stripIdx) {
  preferences.begin(NVM_NAMESPACE, true);
  String key = getStripLedCountKey(stripIdx);
  const uint16_t ledCount = preferences.getUInt(key.c_str(), DVC_NUM_LEDS_LIST[stripIdx]);
  preferences.end();
  return ledCount;
}

uint16_t getConfiguredStripLedOffset(int stripIdx) {
  preferences.begin(NVM_NAMESPACE, true);
  String key = getStripLedOffsetKey(stripIdx);
  const uint16_t ledOffset = preferences.getUInt(key.c_str(), DVC_LED_OFFSET_LIST[stripIdx]);
  preferences.end();
  return ledOffset;
}

uint16_t getConfiguredStripLedShift(int stripIdx) {
  preferences.begin(NVM_NAMESPACE, true);
  String key = getStripLedShiftKey(stripIdx);
  const uint16_t ledShift = preferences.getUInt(key.c_str(), DVC_LED_SHIFT_LIST[stripIdx]);
  preferences.end();
  return ledShift;
}

template <uint16_t DataPin>
void initStripSlot(int stripIdx) {
  const uint16_t ledCount = getConfiguredStripLedCount(stripIdx);
  const uint16_t ledOffset = getConfiguredStripLedOffset(stripIdx);
  const uint16_t ledShift = getConfiguredStripLedShift(stripIdx);
  if (ledCount == 0) {
    ledStrips[stripIdx] = nullptr;
    return;
  }

  ledStrips[stripIdx] = new LedStripDvc(stripIdx, ledCount, ledOffset, ledShift);
  FastLED.addLeds<DVC_LED_TYPE, DataPin, DVC_LED_COLOR_ORDER>(
      ledStrips[stripIdx]->physicalLeds,
      ledStrips[stripIdx]->physicalLedCount);
}
}

LedStripDvc* ledStrips[DVC_STRIP_COUNT] = {nullptr};

void initLedRenderer() {
  if (renderMutex == nullptr) {
    renderMutex = xSemaphoreCreateMutex();
  }
}

void initLeds() {
  initLedRenderer();

  // Pins remain compile-time, but per-strip LED counts can be persisted and disabled with 0.
  initStripSlot<DVC_DATA_PIN_LIST[0]>(0);

  #if DVC_STRIP_COUNT > 1
  initStripSlot<DVC_DATA_PIN_LIST[1]>(1);
  #endif

  #if DVC_STRIP_COUNT > 2
  initStripSlot<DVC_DATA_PIN_LIST[2]>(2);
  #endif  

  #if DVC_STRIP_COUNT > 3
  initStripSlot<DVC_DATA_PIN_LIST[3]>(3);
  #endif

  #if DVC_STRIP_COUNT > 4
  initStripSlot<DVC_DATA_PIN_LIST[4]>(4);
  #endif

  initRenderService();
}

static void preparePhysicalLeds(LedStripDvc* dvc) {
  if (dvc == nullptr || dvc->ledCount == 0 || dvc->physicalLedCount == 0) {
    return;
  }

  fill_solid(dvc->physicalLeds, dvc->physicalLedCount, CRGB::Black);

  const uint16_t ledShift = dvc->ledShift % dvc->ledCount;
  for (int i = 0; i < dvc->ledCount; i++) {
    const uint16_t shiftedIndex = (i + ledShift) % dvc->ledCount;
    dvc->physicalLeds[dvc->ledOffset + shiftedIndex] = dvc->leds[i];
  }
}

void FastLedShow() {
  initLedRenderer();

  if (renderMutex != nullptr) {
    xSemaphoreTake(renderMutex, portMAX_DELAY);
  }

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    preparePhysicalLeds(ledStrips[i]);
  }

  FastLED.show();

  if (renderMutex != nullptr) {
    xSemaphoreGive(renderMutex);
  }
}
