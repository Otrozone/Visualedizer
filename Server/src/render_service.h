#ifndef RENDER_SERVICE_H
#define RENDER_SERVICE_H

#include <Arduino.h>
#include <FastLED.h>

void initRenderService();

void requestAbortRender(int stripIdx = -1);
void requestFillSolid(int stripIdx, CRGB color);
void requestFillGradientHSV(int stripIdx, CHSV chsvStart, CHSV chsvEnd);
void requestFillRangeHSV(int stripIdx, int startIndex, int endIndex, CHSV color);
void requestFillSection(int stripIdx, int sectionCount, int sectionIdx, CRGB color);
void requestOff(int stripIdx = -1);
void requestApplyBinaryFrame(const uint8_t* payload, size_t length);

void requestEffectStrobe(int stripIdx, CRGB color, int delay1, int delay2);
void requestEffectStrobeRandom(int stripIdx, CRGB color);
void requestEffectRunningRainbow(int stripIdx, int delay, int step, int delta);
void requestEffectRunningGradient(int stripIdx, CRGB color1, CRGB color2, int delay, float step);
void requestEffectNoise(int stripIdx);
void requestEffectBlend(int stripIdx, CRGB color, int duration);
void requestEffectFadeIn(int stripIdx, CRGB color, int duration);
void requestEffectFadeOut(int stripIdx, int duration);
void requestEffectMid2Out(int stripIdx, CRGB color, int duration);
void requestEffectOut2Mid(int stripIdx, CRGB color, int duration);

#endif
