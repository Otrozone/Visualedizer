#include "led.h"

uint16_t ledCount = 0;

// CRGB* leds[DVC_STRIP_COUNT];
// CRGB** leds; 

CRGB* leds = nullptr;

void initLeds() {

  leds = new CRGB[ledCount];

  // leds = new CRGB*[DVC_STRIP_COUNT]
  
  /*for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    leds[i] = new CRGB[DVC_NUM_LEDS_LIST[i]];
    leds = new CRGB*[DVC_STRIP_COUNT];
  }*/

  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN, DVC_LED_COLOR_ORDER>(leds, ledCount);

  // Example for initializing additional LED strips on different GPIO pins
  // FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_2, DVC_LED_COLOR_ORDER>(leds + ledCount, ledCount);
}

void circularShift() {
  CRGB tempArray[ledCount];

  for (int i = 0; i < ledCount; i++) {
    tempArray[(i + DVC_OFFSET) % ledCount] = leds[i];
  }

  for (int i = 0; i < ledCount; i++) {
    leds[i] = tempArray[i];
  }
}

void FastLedShow() {
  if (DVC_OFFSET > 0) {
    circularShift();
  }

  FastLED.show();
}
