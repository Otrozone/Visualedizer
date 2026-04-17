#include "effects.h"
#include "led.h"

void taskStrobe(void *pvParameters) {
  TaskStrobeParams *params = static_cast<TaskStrobeParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;
  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  for (;;) {
    if (dvc->terminateTaskFlag) {
      break;
    }

    fill_solid(leds, ledCount, params->color);
    FastLedShow();
    Serial.println("Delay1: " + String(params->delay1) + " -> " + String(pdMS_TO_TICKS(params->delay1)));
    vTaskDelay(pdMS_TO_TICKS(params->delay1));

    fill_solid(leds, ledCount, CRGB::Black);
    FastLedShow();
    Serial.println("Delay2: " + String(params->delay2) + " -> " + String(pdMS_TO_TICKS(params->delay2)));
    vTaskDelay(pdMS_TO_TICKS(params->delay2));
  }

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;
  vTaskDelete(nullptr);
}

void taskStrobeRandom(void *pvParameters) {
  TaskStrobeParams *params = static_cast<TaskStrobeParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;
  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  randomSeed(analogRead(0));

  for (;;) {
    if (dvc->terminateTaskFlag) {
      break;
    }

    fill_solid(leds, ledCount, params->color);
    FastLedShow();
    int delay1 = random(1, 500);
    vTaskDelay(pdMS_TO_TICKS(delay1));

    fill_solid(leds, ledCount, CRGB::Black);
    FastLedShow();
    int delay2 = random(1, 500);
    vTaskDelay(pdMS_TO_TICKS(delay2));
  }

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;
  vTaskDelete(nullptr);
}

void taskRunningRainbow(void *pvParameters) {
  TaskRunningRainbowParams *params = static_cast<TaskRunningRainbowParams *>(pvParameters);
  static uint8_t hue = 0;

  LedStripDvc *dvc = params->ledStrip;
  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  Serial.printf("Starting rainbow effect on strip index %d\n", dvc->ledIdx);

  for (;;) {
    if (dvc->terminateTaskFlag) {
      break;
    }

    fill_rainbow(leds, ledCount, hue, params->delta);
    FastLedShow();
    hue += params->step;

    vTaskDelay(pdMS_TO_TICKS(params->delay));
  }

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;

  vTaskDelete(nullptr);
}

void taskRunningGradient(void *pvParameters) {
  TaskRunningGradientParams *params = static_cast<TaskRunningGradientParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;
  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  float shift = 0.0f;

  for (;;) {
    if (dvc->terminateTaskFlag) {
      break;
    }

    for (int i = 0; i < ledCount; i++) {
      float t = (float)i / (ledCount - 1);
      leds[i] = blend(params->color1, params->color2, (uint8_t)(t * 255));
    }

    shift += params->step;
    if (shift > 1.0f)
      shift -= 1.0f; // wrap around

    FastLedShow();
    vTaskDelay(pdMS_TO_TICKS(params->delay));
  }

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;

  vTaskDelete(nullptr);
}

void taskNoise(void *pvParameters) {
  TaskBaseParams *params = static_cast<TaskBaseParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;

  uint8_t octaves = 3;
  int scale = 80;
  uint8_t hue_octaves = 2;
  int hue_scale = 80;
  uint16_t time = millis();
  uint16_t noise_x = 0;

  for (;;) {
    if (dvc->terminateTaskFlag) {
      break;
    }

    noise_x += 1;
    fill_noise8(dvc->leds, dvc->ledCount, octaves, noise_x, scale, hue_octaves, noise_x, hue_scale, time);
    vTaskDelay(pdMS_TO_TICKS(100));
  }

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;
  vTaskDelete(nullptr);
}

void taskBlend(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;

  int delay = 10;
  int steps = params->duration / delay;

  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  for (int i = 0; i < steps; i++) {
    if (dvc->terminateTaskFlag) break;
    for (int j = 0; j < ledCount; j++) {
      CRGB currentColor = leds[j];
      CRGB targetColor = params->color;
      uint8_t blendAmount = round(i * (255.0 / steps));
      leds[j] = blend(currentColor, targetColor, blendAmount);
    }
    FastLedShow();
    vTaskDelay(pdMS_TO_TICKS(delay));
  }

  fill_solid(leds, ledCount, params->color);
  FastLedShow();

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;

  vTaskDelete(nullptr);
}

void taskFadeIn(void *pvParameters) {
  // Serial.println("termFlag: " + String(terminateTaskFlag));

  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;

  int delay = 10;
  int steps = params->duration / delay;

  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  for (int i = 0; i < steps; i++) {
    if (dvc->terminateTaskFlag) break;
    for (int j = 0; j < ledCount; j++) {
      leds[j] = params->color;
      leds[j].fadeLightBy(255 - round(i * ((float)255 / steps)));
    }
    FastLedShow();

    vTaskDelay(pdMS_TO_TICKS(delay));
  }

  fill_solid(leds, ledCount, params->color);
  FastLedShow();

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;

  vTaskDelete(nullptr); 
}

void taskFadeOut(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;

  int delay = 10;
  int steps = (params->duration * 2) / delay;

  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  // Gamma correction factor (to get the smoothness)
  float gamma = 15.0f;

  for (int i = 0; i < steps; i++) {
    if (dvc->terminateTaskFlag) break;
    float t = (float)i / (float)steps;

    float factor = powf(1.0f - t, gamma);
    uint8_t brightness = (uint8_t)roundf(factor * 255.0f);

    // Clamp tiny brightness values to 0 to avoid flicker
    if (brightness < 150)
      brightness = 0;

    for (int j = 0; j < ledCount; j++)
    {
      leds[j].nscale8_video(brightness);
    }

    FastLedShow();
    vTaskDelay(pdMS_TO_TICKS(delay));
  }

  fill_solid(leds, ledCount, CRGB::Black);
  FastLedShow();

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;
  vTaskDelete(nullptr);
}

void taskMid2Out(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;
  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  int delay = 10;
  int steps = params->duration / delay;
  int center = ledCount / 2;

  for (int i = 0; i < steps; i++) {
    if (dvc->terminateTaskFlag) break;
    int range = round(i * ((float)center / steps));
    for (int j = 0; j <= range; j++) {
      if (center + j < ledCount) {
        leds[center + j] = params->color;
      }
      if (center - j >= 0) {
        leds[center - j] = params->color;
      }
    }
    FastLedShow();
    vTaskDelay(pdMS_TO_TICKS(delay));
  }

  fill_solid(leds, ledCount, params->color);
  FastLedShow();

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;

  vTaskDelete(nullptr);
}

void taskOut2Mid(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  LedStripDvc *dvc = params->ledStrip;
  CRGB *leds = dvc->leds;
  int ledCount = dvc->ledCount;

  int delay = 10;
  int steps = params->duration / delay;
  int center = ledCount / 2;

  for (int i = 0; i < steps; i++) {
    if (dvc->terminateTaskFlag) break;
    int range = center - round(i * ((float)center / steps));
    for (int j = 0; j < center; j++) {
      if (j <= range) {
        if (center + j < ledCount) {
          leds[center + j] = params->color;
        }
        if (center - j >= 0) {
          leds[center - j] = params->color;
        }
      } else {
        if (center + j < ledCount) {
          leds[center + j] = CRGB::Black;
        }
        if (center - j >= 0) {
          leds[center - j] = CRGB::Black;
        }
      }
    }
    FastLedShow();
    vTaskDelay(pdMS_TO_TICKS(delay));
  }

  fill_solid(leds, ledCount, CRGB::Black);
  FastLedShow();

  delete params;
  dvc->taskHandle = nullptr;
  dvc->terminateTaskFlag = false;

  vTaskDelete(nullptr);
}
