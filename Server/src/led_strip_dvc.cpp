#include "led_strip_dvc.h"
#include "effects.h"

// Implementation of LedStripDvc methods

LedStripDvc::LedStripDvc(int idx, uint16_t count)
    : taskHandle(nullptr), terminateTaskFlag(false), ledIdx(idx), ledCount(count)
{
    Serial.printf("Led count: %d\n", ledCount);
    leds = new CRGB[ledCount];
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
    params->ledStrip = this;
    params->color = color;
    params->delay1 = delay1;
    params->delay2 = delay2;

    xTaskCreate(taskStrobe, "StrobeTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectStrobeRandom(CRGB color) {
    terminateCurrTask();

    TaskStrobeParams *params = new TaskStrobeParams;
    params->ledStrip = this;
    params->color = color;

    xTaskCreate(taskStrobeRandom, "StrobeRandomTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectRunningRainbow(int delay, int step, int delta) {
    terminateCurrTask();

    TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
    params->ledStrip = this;
    params->delay = delay;
    params->step = step;
    params->delta = delta;

    xTaskCreate(taskRunningRainbow, "RunningRainbowTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectRunningGradient(CRGB color1, CRGB color2, int delay, float step) {
    terminateCurrTask();

    TaskRunningGradientParams *params = new TaskRunningGradientParams;
    params->ledStrip = this;
    params->delay = delay;
    params->step = step;
    params->color1 = color1;
    params->color2 = color2;

    xTaskCreate(taskRunningGradient, "RunningGradientTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectNoise() {
    terminateCurrTask();

    TaskBaseParams *params = new TaskBaseParams;
    params->ledStrip = this;

    xTaskCreate(taskNoise, "NoiseTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectBlend(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskBlend, "BlendTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectFadeIn(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskFadeIn, "FadeInTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectFadeOut(int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = CRGB::Black;

    xTaskCreate(taskFadeOut, "FadeOutTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectMid2Out(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskMid2Out, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectOut2Mid(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    xTaskCreate(taskOut2Mid, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::fillSolid(CRGB color) {
    terminateCurrTask();

    fill_solid(this->leds, this->ledCount, color);

    // FastLED[this->ledIdx].showLeds();
    FastLED.show();
}

void LedStripDvc::fillGradientHSV(CHSV chsvStart, CHSV chsvEnd) {
    terminateCurrTask();

    fill_gradient_HSV(this->leds, this->ledCount, chsvStart, chsvEnd, FORWARD_HUES);

    // FastLED[this->ledIdx].showLeds();
    FastLED.show();
}

void LedStripDvc::off() {
    terminateCurrTask();

    fill_solid(this->leds, this->ledCount, CRGB::Black);

    // FastLED[this->ledIdx].showLeds();
    FastLED.show();
}

LedStripDvc::~LedStripDvc() {
    delete[] leds;
}
