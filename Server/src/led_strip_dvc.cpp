#include "led_strip_dvc.h"

// Implementation of LedStripDvc methods

LedStripDvc::LedStripDvc(int idx, uint16_t count, uint16_t offset, uint16_t shift)
    : ledIdx(idx),
      ledCount(count),
      ledOffset(offset),
      ledShift(shift),
      physicalLedCount(static_cast<uint16_t>(offset + count))
{
    Serial.printf(
        "Led count: %d, led offset: %d, led shift: %d, physical led count: %d\n",
        ledCount,
        ledOffset,
        ledShift,
        physicalLedCount);
    leds = new CRGB[ledCount];
    physicalLeds = new CRGB[physicalLedCount];
    fill_solid(leds, ledCount, CRGB::Black);
    fill_solid(physicalLeds, physicalLedCount, CRGB::Black);
}

LedStripDvc::~LedStripDvc() {
    delete[] leds;
    delete[] physicalLeds;
}
