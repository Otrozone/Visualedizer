#include "effects.h"

volatile bool terminateTaskFlag = false;

TaskHandle_t taskHandle;
const uint32_t StackSize = 2048; // What is the good value for my operations?

void terminateCurrTask() {
  Serial.println("Terminating current task");
  if (taskHandle != NULL) {
    TaskHandle_t xCurrentTask = xTaskGetCurrentTaskHandle();
    if (xCurrentTask == taskHandle) {
      Serial.println("Delete task using handle...");
      vTaskDelete(taskHandle);
      taskHandle = NULL;
    } else {
      Serial.println("Delete task using flag...");
      terminateTaskFlag = true;
      delay(50);
      terminateTaskFlag = false;
    }
  }
}

void taskStrobe(void *pvParameters) {
  TaskStrobeParams *params = static_cast<TaskStrobeParams *>(pvParameters);

  CRGB *leds = params->leds;

  while (!terminateTaskFlag) {
    fill_solid(leds, params->ledCount, params->color);
    FastLED.show();
    Serial.println("Delay1: " + String(params->delay1) + " -> " + String(params->delay1 / portTICK_RATE_MS));
    vTaskDelay(params->delay1 / portTICK_RATE_MS);

    fill_solid(leds, params->ledCount, CRGB::Black);
    FastLED.show();
    Serial.println("Delay2: " + String(params->delay2) + " -> " + String(params->delay2 / portTICK_RATE_MS));
    vTaskDelay(params->delay2 / portTICK_RATE_MS);
  }

  delete params;
  terminateTaskFlag = false;
  vTaskDelete(NULL);
}

void taskStrobeRandom(void *pvParameters) {
  TaskStrobeParams *params = static_cast<TaskStrobeParams *>(pvParameters);

  CRGB *leds = params->leds;

  randomSeed(analogRead(0));

  while (!terminateTaskFlag) {
    fill_solid(leds, params->ledCount, params->color);
    FastLED.show();
    int delay1 = random(1, 500);
    vTaskDelay(delay1 / portTICK_RATE_MS);

    fill_solid(leds, params->ledCount, CRGB::Black);
    FastLED.show();
    int delay2 = random(1, 500);
    vTaskDelay(delay2 / portTICK_RATE_MS);
  }

  delete params;
  terminateTaskFlag = false;
  vTaskDelete(NULL);
}

void taskRunningRainbow(void *pvParameters) {
  TaskRunningRainbowParams *params = static_cast<TaskRunningRainbowParams *>(pvParameters);
  static uint8_t hue = 0;
  
  CRGB *leds = params->leds;
  while (!terminateTaskFlag) {
    fill_rainbow(leds, params->ledCount, hue, params->delta);
    FastLED.show();
    hue += params->step;
    // Serial.println(String(params->delay) + ", " + String(params->step) + ", " + String(params->delta) + " | ");
    vTaskDelay(params->delay / portTICK_RATE_MS);
  }

  delete params;
  terminateTaskFlag = false;
  vTaskDelete(NULL);
};

void taskNoise(void *pvParameters) {
  TaskParams *params = static_cast<TaskParams *>(pvParameters);

  uint8_t octaves = 3;
  int scale = 80;
  uint8_t hue_octaves = 2;
  int hue_scale = 80;
  uint16_t time = millis();
  uint16_t noise_x = 0;

  while (!terminateTaskFlag) {
    noise_x += 1;
    fill_noise8(params->leds, params->ledCount, octaves, noise_x, scale, hue_octaves, noise_x, hue_scale, time);
    vTaskDelay(100 / portTICK_RATE_MS);
  }

  delete params;
  terminateTaskFlag = false;
  vTaskDelete(NULL);
}

void taskBlend(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  int delay = 10;
  int steps = params->duration / delay;

  for (int i = 0; !terminateTaskFlag && i < steps; i++) {
    for (int j = 0; j < params->ledCount; j++) {
      CRGB currentColor = params->leds[j];
      CRGB targetColor = params->color;
      uint8_t blendAmount = round(i * (255.0 / steps));
      params->leds[j] = blend(currentColor, targetColor, blendAmount);
    }
    FastLED.show();
    vTaskDelay(delay / portTICK_RATE_MS);
  }

  fill_solid(params->leds, params->ledCount, params->color);
  FastLED.show();

  terminateTaskFlag = false;

  vTaskDelete(NULL); 
}

void taskFadeIn(void *pvParameters) {
  // Serial.println("termFlag: " + String(terminateTaskFlag));

  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  int delay = 10;
  int steps = params->duration / delay;

  for (int i = 0; !terminateTaskFlag && i < steps; i++) {
    for (int j = 0; j < params->ledCount; j++) {
      params->leds[j] = params->color;
      params->leds[j].fadeLightBy(255 - round(i * ((float)255 / steps)));
    }
    FastLED.show();

    vTaskDelay(delay / portTICK_RATE_MS);
  }

  // Serial.println("termFlag: " + String(terminateTaskFlag));

  fill_solid(params->leds, params->ledCount, params->color);
  FastLED.show();

  terminateTaskFlag = false;

  vTaskDelete(NULL); 
}

void taskFadeOut(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  int delay = 10;
  int steps = params->duration / delay;

  for (int i = 0; !terminateTaskFlag && i < steps; i++) {
    uint8_t brightness = 255 - round(i * ((float)255 / steps));
    for (int j = 0; j < params->ledCount; j++) {
      params->leds[j].nscale8_video(brightness);
    }
    FastLED.show();

    vTaskDelay(delay / portTICK_RATE_MS);
  }

  fill_solid(params->leds, params->ledCount, CRGB::Black);
  FastLED.show();

  terminateTaskFlag = false;

  vTaskDelete(NULL); 
}

void taskMid2Out(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  int delay = 10;
  int steps = params->duration / delay;
  int center = params->ledCount / 2;

  for (int i = 0; !terminateTaskFlag && i < steps; i++) {
    int range = round(i * ((float)center / steps));
    for (int j = 0; j <= range; j++) {
      if (center + j < params->ledCount) {
        params->leds[center + j] = params->color;
      }
      if (center - j >= 0) {
        params->leds[center - j] = params->color;
      }
    }
    FastLED.show();
    vTaskDelay(delay / portTICK_RATE_MS);
  }

  fill_solid(params->leds, params->ledCount, params->color);
  FastLED.show();

  terminateTaskFlag = false;

  vTaskDelete(NULL);
}

void taskOut2Mid(void *pvParameters) {
  TaskColorAndDurationParams *params = static_cast<TaskColorAndDurationParams *>(pvParameters);

  int delay = 10;
  int steps = params->duration / delay;
  int center = params->ledCount / 2;

  for (int i = 0; !terminateTaskFlag && i < steps; i++) {
    int range = center - round(i * ((float)center / steps));
    for (int j = 0; j < center; j++) {
      if (j <= range) {
        if (center + j < params->ledCount) {
          params->leds[center + j] = params->color;
        }
        if (center - j >= 0) {
          params->leds[center - j] = params->color;
        }
      } else {
        if (center + j < params->ledCount) {
          params->leds[center + j] = CRGB::Black;
        }
        if (center - j >= 0) {
          params->leds[center - j] = CRGB::Black;
        }
      }
    }
    FastLED.show();
    vTaskDelay(delay / portTICK_RATE_MS);
  }

  fill_solid(params->leds, params->ledCount, CRGB::Black);
  FastLED.show();

  terminateTaskFlag = false;

  vTaskDelete(NULL);
}

// -----------------------------------------------

void runEffectStrobe(CRGB color, int delay1, int delay2) {
  terminateCurrTask();

  TaskStrobeParams *params = new TaskStrobeParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->color = color;
  params->delay1 = delay1;
  params->delay2 = delay2;

  xTaskCreate(taskStrobe, "StrobeTask", StackSize, params, 10, &taskHandle);
}

void runEffectStrobeRandom(CRGB color) {
  terminateCurrTask();

  TaskStrobeParams *params = new TaskStrobeParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->color = color;

  xTaskCreate(taskStrobeRandom, "StrobeRandomTask", StackSize, params, 10, &taskHandle);
}

void runEffectRunningRainbow(int delay, int step, int delta) {
  terminateCurrTask();

  TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->delay = delay;
  params->step = step;
  params->delta = delta;

  xTaskCreate(taskRunningRainbow, "RunningRainbowTask", StackSize, params, 10, &taskHandle);
}

void runEffectNoise() {
  terminateCurrTask();

  TaskParams *params = new TaskParams;
  params->ledCount = ledCount;
  params->leds = leds;

  xTaskCreate(taskNoise, "NoiseTask", StackSize, params, 10, &taskHandle);
}

void runEffectBlend(CRGB color, int duration) {
  terminateCurrTask();

  TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  params->color = color;

  xTaskCreate(taskBlend, "BlendTask", StackSize, params, 10, &taskHandle);
}

void runEffectFadeIn(CRGB color, int duration) {
  terminateCurrTask();

  TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  params->color = color;

  xTaskCreate(taskFadeIn, "FadeInTask", StackSize, params, 10, &taskHandle);
}

void runEffectFadeOut(int duration) {
  terminateCurrTask();

  TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  params->color = CRGB::Black;

  xTaskCreate(taskFadeOut, "FadeOutTask", StackSize, params, 10, &taskHandle);
}

void runEffectMid2Out(CRGB color, int duration) {
  terminateCurrTask();

  TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  params->color = color;

  xTaskCreate(taskMid2Out, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void runEffectOut2Mid(CRGB color, int duration) {
  terminateCurrTask();

  TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  params->color = color;

  xTaskCreate(taskOut2Mid, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}