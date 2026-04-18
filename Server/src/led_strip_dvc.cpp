#include "led_strip_dvc.h"
#include "effects.h"
#include "led.h"
#include "render_service.h"

// Implementation of LedStripDvc methods

LedStripDvc::LedStripDvc(int idx, uint16_t count)
    : taskHandle(nullptr), terminateTaskFlag(false), ledIdx(idx), ledCount(count)
{
    Serial.printf("Led count: %d\n", ledCount);
    leds = new CRGB[ledCount];
}

void LedStripDvc::terminateCurrTask() {
    requestAbortRender(ledIdx);
}


void LedStripDvc::runEffectStrobe(CRGB color, int delay1, int delay2) {
    requestEffectStrobe(ledIdx, color, delay1, delay2);
}

void LedStripDvc::runEffectStrobeRandom(CRGB color) {
    requestEffectStrobeRandom(ledIdx, color);
}

void LedStripDvc::runEffectRunningRainbow(int delay, int step, int delta) {
    requestEffectRunningRainbow(ledIdx, delay, step, delta);
}

void LedStripDvc::runEffectRunningGradient(CRGB color1, CRGB color2, int delay, float step) {
    requestEffectRunningGradient(ledIdx, color1, color2, delay, step);
}

void LedStripDvc::runEffectNoise() {
    requestEffectNoise(ledIdx);
}

void LedStripDvc::runEffectBlend(CRGB color, int duration) {
    requestEffectBlend(ledIdx, color, duration);
}

void LedStripDvc::runEffectFadeIn(CRGB color, int duration) {
    requestEffectFadeIn(ledIdx, color, duration);
}

void LedStripDvc::runEffectFadeOut(int duration) {
    requestEffectFadeOut(ledIdx, duration);
}

void LedStripDvc::runEffectMid2Out(CRGB color, int duration) {
    requestEffectMid2Out(ledIdx, color, duration);
}

void LedStripDvc::runEffectOut2Mid(CRGB color, int duration) {
    requestEffectOut2Mid(ledIdx, color, duration);
}

void LedStripDvc::fillSolid(CRGB color) {
    requestFillSolid(ledIdx, color);
}

void LedStripDvc::fillGradientHSV(CHSV chsvStart, CHSV chsvEnd) {
    requestFillGradientHSV(ledIdx, chsvStart, chsvEnd);
}

void LedStripDvc::off() {
    requestOff(ledIdx);
}

LedStripDvc::~LedStripDvc() {
    delete[] leds;
}
