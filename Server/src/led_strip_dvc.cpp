#include "led_strip_dvc.h"

// Implementation of LedStripDvc methods

LedStripDvc::LedStripDvc(int idx, uint16_t count)
    : ledIdx(idx), ledCount(count)
{
    Serial.printf("Led count: %d\n", ledCount);
    leds = new CRGB[ledCount];
}

LedStripDvc::~LedStripDvc() {
    delete[] leds;
}
