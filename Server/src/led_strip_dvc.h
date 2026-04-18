#ifndef LED_STRIP_DVC_H
#define LED_STRIP_DVC_H

#include <FastLED.h>
#include <Devices.h>

class LedStripDvc {
public:
    int ledIdx;
    uint16_t ledCount;
    CRGB *leds;

    LedStripDvc(int idx, uint16_t count);
    ~LedStripDvc();
};

#endif
