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

uint16_t getConfiguredStripLedCount(int stripIdx) {
  preferences.begin(NVM_NAMESPACE, true);
  String key = getStripLedCountKey(stripIdx);
  const uint16_t ledCount = preferences.getUInt(key.c_str(), DVC_NUM_LEDS_LIST[stripIdx]);
  preferences.end();
  return ledCount;
}

template <uint16_t DataPin>
void initStripSlot(int stripIdx) {
  const uint16_t ledCount = getConfiguredStripLedCount(stripIdx);
  if (ledCount == 0) {
    ledStrips[stripIdx] = nullptr;
    return;
  }

  ledStrips[stripIdx] = new LedStripDvc(stripIdx, ledCount);
  FastLED.addLeds<DVC_LED_TYPE, DataPin, DVC_LED_COLOR_ORDER>(ledStrips[stripIdx]->leds, ledStrips[stripIdx]->ledCount);
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

static void circularShift(LedStripDvc* dvc) {
  if (dvc == nullptr || dvc->ledCount == 0) {
    return;
  }

  CRGB tempArray[dvc->ledCount];

  for (int i = 0; i < dvc->ledCount; i++) {
    tempArray[(i + DVC_OFFSET) % dvc->ledCount] = dvc->leds[i];
  }

  for (int i = 0; i < dvc->ledCount; i++) {
    dvc->leds[i] = tempArray[i];
  }
}

void FastLedShow() {
  initLedRenderer();

  if (renderMutex != nullptr) {
    xSemaphoreTake(renderMutex, portMAX_DELAY);
  }

  if (DVC_OFFSET > 0) {
    for (int i = 0; i < DVC_STRIP_COUNT; i++) {
      circularShift(ledStrips[i]);
    }
  }

  FastLED.show();

  if (renderMutex != nullptr) {
    xSemaphoreGive(renderMutex);
  }
}
