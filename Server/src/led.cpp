#include <led.h>

uint16_t ledCount = 0;
CRGB* leds = nullptr;

void initLeds() {
  leds = new CRGB[ledCount];

  FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN, DVC_LED_COLOR_ORDER>(leds, ledCount);
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
