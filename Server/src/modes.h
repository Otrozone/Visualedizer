#ifndef MODES_H
#define MODES_H

#include <Arduino.h>
#include <freertos/semphr.h>
extern volatile bool terminateTaskFlag;

extern SemaphoreHandle_t taskDeletedSemaphore;

struct TaskStrobeParams {
  int ledCount;
  CRGB *leds;
  int delay1;
  int delay2;
  CRGB color;
};

struct TaskRunningRainbowParams {
  int ledCount;
  CRGB *leds;
  int delay;
  int step;
  int delta;
};

// Common param structure
struct TaskParams {
  int ledCount;
  CRGB *leds;
};

void taskStrobe(void *pvParameters);
void taskStrobeRandom(void *pvParameters);
void taskRunningRainbow(void *pvParameters);
CRGB htmlColor2Crgb(String htmlColor);
CHSV htmlColor2Chsv(String htmlColor);

#endif /* MODES_H */