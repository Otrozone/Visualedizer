// These task functions are like threads. For that reason is the
// leds array passed as a parameter to the task functions. It 
// ensures, that the concurrent access to the leds array is safe.

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

struct TaskFadeParams {
  int ledCount;
  CRGB *leds;
  int duration; // ms
  CRGB color;
};

struct TaskBlendParams {
  int ledCount;
  CRGB *leds;
  int duration; // ms
  CRGB color;
};

void taskStrobe(void *pvParameters);
void taskStrobeRandom(void *pvParameters);
void taskRunningRainbow(void *pvParameters);
void taskFadeIn(void *pvParameters);
void taskFadeOut(void *pvParameters);
void taskBlend(void *pvParameters);

CRGB htmlColor2Crgb(String htmlColor);
CHSV htmlColor2Chsv(String htmlColor);

#endif /* MODES_H */