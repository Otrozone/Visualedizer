#include "led_strip_dvc.h"
#include "effects.h"

#include <unordered_map>
#include <functional>
#include <string>


using EffectFunc = std::function<void(AsyncWebServerRequest*)>;

// Remove global effectMap

LedStripDvc::LedStripDvc(const String& name, uint16_t count)
    : taskHandle(nullptr), terminateTaskFlag(false), ledName(name), ledCount(count)
{
    Serial.printf("Led count: %d\n", ledCount);
    effectMap = {
        {std::string("solid-color"), [this](AsyncWebServerRequest *request) { this->fillSolid(request); }},
        {std::string("off"), [this](AsyncWebServerRequest *request) { this->off(request); }},
    };
}

void LedStripDvc::terminateCurrTask() {
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

void LedStripDvc::runEffectStrobe(CRGB color, int delay1, int delay2) {
    terminateCurrTask();

    TaskStrobeParams *params = new TaskStrobeParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->color = color;
    params->delay1 = delay1;
    params->delay2 = delay2;

    xTaskCreate(taskStrobe, "StrobeTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectStrobeRandom(CRGB color) {
    terminateCurrTask();

    TaskStrobeParams *params = new TaskStrobeParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->color = color;

    xTaskCreate(taskStrobeRandom, "StrobeRandomTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectRunningRainbow(int delay, int step, int delta) {
    terminateCurrTask();

    TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->delay = delay;
    params->step = step;
    params->delta = delta;

    xTaskCreate(taskRunningRainbow, "RunningRainbowTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectNoise() {
    terminateCurrTask();

    TaskParams *params = new TaskParams;
    params->ledCount = ledCount;
    params->leds = leds;

    xTaskCreate(taskNoise, "NoiseTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectBlend(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskBlend, "BlendTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectFadeIn(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskFadeIn, "FadeInTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectFadeOut(int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->duration = duration;
    params->color = CRGB::Black;

    xTaskCreate(taskFadeOut, "FadeOutTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectMid2Out(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskMid2Out, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectOut2Mid(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledCount = ledCount;
    params->leds = leds;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskOut2Mid, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::fillSolid(CRGB color) {
    fill_solid(this->leds, this->ledCount, color);
}

void LedStripDvc::updateSection(int sectionCount, int sectionIdx, CRGB color) {
  fill_solid(leds, ledCount, CRGB::Black);

  int sectionLength = ledCount / sectionCount;
  int sectionToLight = sectionIdx;

  int startIndex = sectionToLight * sectionLength;
  int endIndex = startIndex + sectionLength;

  for (int i = startIndex; i < endIndex; i++) {
    leds[i] = color;
  }
}

void LedStripDvc::fillSolid(AsyncWebServerRequest *request) {
  terminateCurrTask();
  if (request->hasParam("color")) {
    // RGB
    CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      updateSection(sectionCount, sectionIdx, color);
    } else {
      Serial.println("Solid color via ledStrips object.");
      fill_solid(leds, ledCount, color);
    }
  } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value")) {
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();
    CHSV color = CHSV(hue, saturation, value);

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      
      updateSection(sectionCount, sectionIdx,  color);
    } else {
      if (request->hasParam("stripIdx", false, false)) {
        int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
        ledStrips[stripIdx]->fillSolid(color);
      } else {
        Serial.println("Solid color command HSV");
        fill_solid(leds, ledCount, color);
      }
    }
  } else {
    request->send(200, "text/plain", "Command solid-color: Missing required parameters.");
    return;
  }
}

void LedStripDvc::off(AsyncWebServerRequest* request) {
    terminateCurrTask();
    Serial.println("Turning off LED strip");
    fill_solid(leds, ledCount, CRGB::Black);
    request->send(200, "text/plain", "LED strip turned off.");
    FastLedShow();
}

void LedStripDvc::executeCommand(const std::string& cmd, AsyncWebServerRequest *request) {
    auto it = effectMap.find(cmd);
    if (it != effectMap.end()) {
        terminateCurrTask();
        it->second(request);
        FastLedShow();
    } else {
        Serial.println("Unknown command");
    }
}

LedStripDvc::~LedStripDvc() {
    delete[] leds;
}
