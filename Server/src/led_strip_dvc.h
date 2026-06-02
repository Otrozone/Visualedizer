#ifndef LED_STRIP_DVC_H
#define LED_STRIP_DVC_H

#include <FastLED.h>
#include <Devices.h>

class LedStripDvc {
public:
    int ledIdx;
    uint16_t ledCount;
    uint16_t ledOffset;
    uint16_t ledShift;
    CRGB *leds;
    uint16_t physicalLedCount;
    CRGB *physicalLeds;

    LedStripDvc(int idx, uint16_t count, uint16_t offset, uint16_t shift);
    ~LedStripDvc();
};

#endif
