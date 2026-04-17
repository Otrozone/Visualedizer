#ifndef LED_H
#define LED_H

// #include "main.h"
// #include <FastLED.h>
#include "devices.h"
#include "led_strip_dvc.h"

extern LedStripDvc* ledStrips[DVC_STRIP_COUNT];

template <typename Func>
inline void forEachLedStrip(Func&& fn) {
  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    LedStripDvc* dvc = ledStrips[i];
    if (dvc != nullptr) {
      fn(*dvc);
    }
  }
}

void initLeds();
void FastLedShow();

#endif
