#ifndef COMMON_H
#define COMMON_H

#include <Arduino.h>
#include <FastLED.h>

String urlDecode(const String& input);
String getQueryParameterValue(String url, String parameterName);
int mapRange(int value, int fromLow, int fromHigh, int toLow, int toHigh);
float normalize(float x, float min, float max);
CRGB htmlColor2Crgb(String htmlColor);
CHSV htmlColor2Chsv(String htmlColor);

#endif
