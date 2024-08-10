// #include <FreeRTOS.h>
// #include <task.h>
#include <FastLED.h>
#include <modes.h>

volatile bool terminateTaskFlag = false;

// html color -> CRGB
CRGB htmlColor2Crgb(String htmlColor) {
  unsigned long colorCode = strtoul(&htmlColor[1], NULL, 16);
  return CRGB::HTMLColorCode(colorCode);
}

CHSV htmlColor2Chsv(String htmlColor) {
  CHSV hsvColor = rgb2hsv_approximate(htmlColor2Crgb(htmlColor));
  return hsvColor;
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
    Serial.println(String(params->delay) + ", " + String(params->step) + ", " + String(params->delta) + " | ");
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

void taskFadeIn(void *pvParameters) {
  // Serial.println("termFlag: " + String(terminateTaskFlag));

  TaskFadeParams *params = static_cast<TaskFadeParams *>(pvParameters);

  uint16_t time = millis();

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
  TaskFadeParams *params = static_cast<TaskFadeParams *>(pvParameters);

  uint16_t time = millis();

  int delay = 10;
  int steps = params->duration / delay;

  for (int i = 0; !terminateTaskFlag && i < steps; i++) {
    for (int j = 0; j < params->ledCount; j++) {
      params->leds[j] = params->color;
      params->leds[j].fadeLightBy(round(i * ((float)255 / steps)));
    }
    FastLED.show();

    vTaskDelay(delay / portTICK_RATE_MS);
  }

  fill_solid(params->leds, params->ledCount, params->color);
  FastLED.show();

  terminateTaskFlag = false;

  vTaskDelete(NULL); 
}