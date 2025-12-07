#ifndef LED_STRIP_DVC_H
#define LED_STRIP_DVC_H

#include <Arduino.h>
#include <FastLED.h>
#include <Devices.h>

class LedStripDvc {
    const uint32_t StackSize = 2048;

public:
    int ledIdx;
    uint16_t ledCount;
    CRGB *leds;

    volatile bool terminateTaskFlag;
    TaskHandle_t taskHandle;

    LedStripDvc(int idx, uint16_t count);

    void terminateCurrTask();

    void runEffectStrobe(CRGB color, int delay1, int delay2);
    void runEffectStrobeRandom(CRGB color);
    void runEffectRunningRainbow(int delay, int step, int delta);
    void runEffectRunningGradient(CRGB color1, CRGB color2, int delay, float step);
    void runEffectNoise();
    void runEffectBlend(CRGB color, int duration);
    void runEffectFadeIn(CRGB color, int duration);
    void runEffectFadeOut(int duration);
    void runEffectMid2Out(CRGB color, int duration);
    void runEffectOut2Mid(CRGB color, int duration);
    void fillSolid(CRGB color);

    void fillGradientHSV(CHSV chsvStart, CHSV chsvEnd);
    void off();


    ~LedStripDvc();
};

#endif