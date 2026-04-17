#include "led_strip_dvc.h"
#include "effects.h"
#include "led.h"

// Implementation of LedStripDvc methods

LedStripDvc::LedStripDvc(int idx, uint16_t count)
    : taskHandle(nullptr), terminateTaskFlag(false), ledIdx(idx), ledCount(count)
{
    Serial.printf("Led count: %d\n", ledCount);
    leds = new CRGB[ledCount];
}

void LedStripDvc::terminateCurrTask() {
    Serial.println("Requesting task termination");
    if (taskHandle != nullptr) {
        Serial.println("Terminating task");
        terminateTaskFlag = true;

        // Wait for task to really die
        while (taskHandle != nullptr) {
            vTaskDelay(5);
        }

        Serial.println("Task terminated");
    }
}


void LedStripDvc::runEffectStrobe(CRGB color, int delay1, int delay2) {
    terminateCurrTask();

    TaskStrobeParams *params = new TaskStrobeParams;
    params->ledStrip = this;
    params->color = color;
    params->delay1 = delay1;
    params->delay2 = delay2;

    terminateTaskFlag = false;

    xTaskCreate(taskStrobe, "StrobeTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectStrobeRandom(CRGB color) {
    terminateCurrTask();

    TaskStrobeParams *params = new TaskStrobeParams;
    params->ledStrip = this;
    params->color = color;

    terminateTaskFlag = false;

    xTaskCreate(taskStrobeRandom, "StrobeRandomTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectRunningRainbow(int delay, int step, int delta) {
    terminateCurrTask();

    TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
    params->ledStrip = this;
    params->delay = delay;
    params->step = step;
    params->delta = delta;

    terminateTaskFlag = false;

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

    terminateTaskFlag = false;

    xTaskCreate(taskRunningGradient, "RunningGradientTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectNoise() {
    terminateCurrTask();

    TaskBaseParams *params = new TaskBaseParams;
    params->ledStrip = this;

    terminateTaskFlag = false;

    xTaskCreate(taskNoise, "NoiseTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectBlend(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    terminateTaskFlag = false;

    xTaskCreate(taskBlend, "BlendTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectFadeIn(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    terminateTaskFlag = false;

    xTaskCreate(taskFadeIn, "FadeInTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectFadeOut(int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = CRGB::Black;

    terminateTaskFlag = false;

    xTaskCreate(taskFadeOut, "FadeOutTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectMid2Out(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    terminateTaskFlag = false;

    xTaskCreate(taskMid2Out, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::runEffectOut2Mid(CRGB color, int duration) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->duration = duration;
    params->color = color;

    terminateTaskFlag = false;

    xTaskCreate(taskOut2Mid, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::fillSolid(CRGB color) {
    terminateCurrTask();

    TaskColorAndDurationParams *params = new TaskColorAndDurationParams;
    params->ledStrip = this;
    params->color = color;
    params->duration = 0;

    terminateTaskFlag = false;

    xTaskCreate(taskFillSolid, "FillSolidTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::fillGradientHSV(CHSV chsvStart, CHSV chsvEnd) {
    terminateCurrTask();

    TaskGradientFillParams *params = new TaskGradientFillParams;
    params->ledStrip = this;
    params->start = chsvStart;
    params->end = chsvEnd;

    terminateTaskFlag = false;

    xTaskCreate(taskFillGradientHSV, "FillGradientTask", StackSize, params, 10, &taskHandle);
}

void LedStripDvc::off() {
    fillSolid(CRGB::Black);
}

LedStripDvc::~LedStripDvc() {
    delete[] leds;
}
