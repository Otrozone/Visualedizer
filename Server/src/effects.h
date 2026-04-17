#ifndef EFFECTS_H
#define EFFECTS_H

#include <Arduino.h>

#include "main.h"

struct TaskBaseParams {
  LedStripDvc *ledStrip;
};

struct TaskStrobeParams : public TaskBaseParams {
  int delay1;
  int delay2;
  CRGB color;
};

struct TaskRunningRainbowParams : public TaskBaseParams {
  int delay;
  int step;
  int delta;
};

struct TaskColorAndDurationParams : public TaskBaseParams {
  int duration; // ms
  CRGB color;
};

struct TaskRunningGradientParams : public TaskBaseParams {
  CRGB color1;
  CRGB color2;
  int delay;
  float step; // 0.0 to 1.0
};

/*void taskStrobe(void *pvParameters);
void taskStrobeRandom(void *pvParameters);
void taskRunningRainbow(void *pvParameters);
void taskFadeIn(void *pvParameters);
void taskFadeOut(void *pvParameters);
void taskBlend(void *pvParameters);
void taskMid2Out(void *pvParameters);
void taskOut2Mid(void *pvParameters);*/

void taskStrobe(void *pvParameters);
void taskStrobeRandom(void *pvParameters);
void taskRunningRainbow(void *pvParameters);
void taskRunningGradient(void *pvParameters);
void taskNoise(void *pvParameters);
void taskBlend(void *pvParameters);
void taskFadeIn(void *pvParameters);
void taskFadeOut(void *pvParameters);
void taskMid2Out(void *pvParameters);
void taskOut2Mid(void *pvParameters);

CRGB htmlColor2Crgb(String htmlColor);
CHSV htmlColor2Chsv(String htmlColor);

#endif /* EFFECTS_H */
