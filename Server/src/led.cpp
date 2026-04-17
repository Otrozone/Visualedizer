#include <freertos/FreeRTOS.h>
#include <freertos/semphr.h>

#include "led.h"

namespace {
SemaphoreHandle_t renderMutex = nullptr;
}

LedStripDvc* ledStrips[DVC_STRIP_COUNT] = {nullptr};

void initLedRenderer() {
  if (renderMutex == nullptr) {
    renderMutex = xSemaphoreCreateMutex();
  }
}

void initLeds() {
  initLedRenderer();

  // This is a workaround for the FastLED library limitation with multiple strips and dynamic pin assignments.
  ledStrips[0] = new LedStripDvc(0, DVC_NUM_LEDS_LIST[0]);
  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_LIST[0], DVC_LED_COLOR_ORDER>(ledStrips[0]->leds, ledStrips[0]->ledCount);

  #if DVC_STRIP_COUNT > 1
  ledStrips[1] = new LedStripDvc(1, DVC_NUM_LEDS_LIST[1]);
  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_LIST[1], DVC_LED_COLOR_ORDER>(ledStrips[1]->leds, ledStrips[1]->ledCount);
  #endif

  #if DVC_STRIP_COUNT > 2
  ledStrips[2] = new LedStripDvc(2, DVC_NUM_LEDS_LIST[2]);
  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_LIST[2], DVC_LED_COLOR_ORDER>(ledStrips[2]->leds, ledStrips[2]->ledCount);
  #endif  

  #if DVC_STRIP_COUNT > 3
  ledStrips[3] = new LedStripDvc(3, DVC_NUM_LEDS_LIST[3]);
  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_LIST[3], DVC_LED_COLOR_ORDER>(ledStrips[3]->leds, ledStrips[3]->ledCount);
  #endif

  #if DVC_STRIP_COUNT > 4
  ledStrips[4] = new LedStripDvc(4, DVC_NUM_LEDS_LIST[4]);
  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_LIST[4], DVC_LED_COLOR_ORDER>(ledStrips[4]->leds, ledStrips[4]->ledCount);
  #endif
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
