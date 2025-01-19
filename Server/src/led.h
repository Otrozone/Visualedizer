#ifndef LED_H
#define LED_H

#include <main.h>

extern CRGB* leds;
extern uint16_t ledCount;

void initLeds();
void FastLedShow();

#endif