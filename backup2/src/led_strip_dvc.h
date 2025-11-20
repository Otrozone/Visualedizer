#ifndef LED_STRIP_DVC_H
#define LED_STRIP_DVC_H

#include <Arduino.h>
#include <FastLED.h>
#include <Devices.h>
#include <ESPAsyncWebServer.h>

#include <unordered_map>
#include <functional>


class LedStripDvc {
    TaskHandle_t taskHandle;
    volatile bool terminateTaskFlag;
    const uint32_t StackSize = 2048;

public:
    String ledName;
    uint16_t ledCount;
    CRGB *leds;

    using EffectFunc = std::function<void(AsyncWebServerRequest*)>;
    std::unordered_map<std::string, EffectFunc> effectMap;

    LedStripDvc(const String &name, uint16_t count);
    
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
    
    void fillSolid(CRGB color);
    void fillSolid(AsyncWebServerRequest *request);
    void off(AsyncWebServerRequest* request);

    void updateSection(int sectionCount, int sectionIdx, CRGB color);
    void executeCommand(const std::string& cmd, AsyncWebServerRequest *request);

    ~LedStripDvc();
};

#endif