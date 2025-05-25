// These task functions are like threads. For that reason is the
// leds array passed as a parameter to the task functions. It 
// ensures, that the concurrent access to the leds array is safe.

#ifndef EFFECTS_H
#define EFFECTS_H

#include <Arduino.h>
#include <freertos/semphr.h>

#include "main.h"

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

struct TaskColorAndDurationParams {
  int ledCount;
  CRGB *leds;
  int duration; // ms
  CRGB color;
};

/*void taskStrobe(void *pvParameters);
void taskStrobeRandom(void *pvParameters);
void taskRunningRainbow(void *pvParameters);
void taskFadeIn(void *pvParameters);
void taskFadeOut(void *pvParameters);
void taskBlend(void *pvParameters);
void taskMid2Out(void *pvParameters);
void taskOut2Mid(void *pvParameters);*/

void terminateCurrTask();

void runEffectStrobe(CRGB color, int delay1, int delay2);
void runEffectStrobeRandom(CRGB color);
void runEffectRunningRainbow(int delay, int step, int delta);
void runEffectNoise();
void runEffectBlend(CRGB color, int duration);
void runEffectFadeIn(CRGB color, int duration);
void runEffectFadeOut(int duration);
void runEffectMid2Out(CRGB color, int duration);
void runEffectOut2Mid(CRGB color, int duration);


CRGB htmlColor2Crgb(String htmlColor);
CHSV htmlColor2Chsv(String htmlColor);

extern TaskHandle_t taskHandle;

#endif /* EFFECTS_H */