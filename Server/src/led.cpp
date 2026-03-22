#include "led.h"
// #include "main.h"

CRGB* leds = nullptr;

uint16_t ledCount = 0;

LedStripDvc* ledStrips[DVC_STRIP_COUNT] = {nullptr};

void initLeds() {
  // FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN, DVC_LED_COLOR_ORDER>(leds, DVC_NUM_LEDS);
  
  /*
  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    ledStrips[i] = new LedStripDvc(String("Strip_") + String(i + 1), DVC_NUM_LEDS_LIST[i]);
    if (i == 0) {
      leds = ledStrips[i]->leds;
      ledCount = ledStrips[i]->ledCount;
    }
    // FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN_LIST[i], DVC_LED_COLOR_ORDER>(ledStrips[i]->leds, ledStrips[i]->ledCount);
  }
  */

  // This is a workaround for the FastLED library limitation with multiple strips and dynamic pin assignments.
  ledStrips[0] = new LedStripDvc(0, ledCount /*DVC_NUM_LEDS_LIST[0]*/ );
  leds = ledStrips[0]->leds;
  // ledCount = ledStrips[0]->ledCount;
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

  // ledCount = DVC_NUM_LEDS;
  // leds = new CRGB[ledCount];
  // FastLED.addLeds<DVC_LED_TYPE, DVC_DATA_PIN, DVC_LED_COLOR_ORDER>(leds, ledCount);
  
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
/*
void serializeData(const LedStripConfig &data, String &output) {
  StaticJsonDocument<128> doc;
  doc["ledName"] = data.;
  doc["ledType"] = data.humidity;
  doc["ledCount"] = data.humidity;
  serializeJson(doc, output);
}
*/
