#ifndef LED_H
#define LED_H

// #include "main.h"
// #include <FastLED.h>
#include "devices.h"
#include "led_strip_dvc.h"

extern uint16_t ledCount;
extern CRGB* leds;

extern LedStripDvc* ledStrips[DVC_STRIP_COUNT];

void initLeds();
void FastLedShow();

#endif