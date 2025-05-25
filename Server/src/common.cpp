#include "common.h"

String urlDecode(const String& input) {
    String decoded = "";
    char c;
    int len = input.length();
    int i = 0;

    while (i < len) {
        c = input.charAt(i);
        if (c == '%') {
            char hex[3];
            hex[0] = input.charAt(++i);
            hex[1] = input.charAt(++i);
            hex[2] = '\0';
            decoded += char(strtol(hex, nullptr, 16));
        } else if (c == '+') {
            decoded += ' ';
        } else {
            decoded += c;
        }
        i++;
    }

    return decoded;
}

String getQueryParameterValue(String url, String parameterName) {
  int startIdx = url.indexOf("?" + parameterName + "=");
  if (startIdx == -1) {
    startIdx = url.indexOf("&" + parameterName + "=");
    if (startIdx == -1) {
      return "";
    }
  }
  startIdx += parameterName.length() + 2; // Skip parameter name, '?' or '&', and '='

  int endIdx = url.indexOf('&', startIdx + 1);
  if (endIdx == -1) {
    endIdx = url.length();
  }

  String encVal = url.substring(startIdx, endIdx);
  Serial.println("Query parameter encoding value: " + encVal);

  return urlDecode(encVal);
}

int mapRange(int value, int fromLow, int fromHigh, int toLow, int toHigh) {
    return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
}

float normalize(float x, float min, float max) {
  return (x - min) / (max - min);
}

// html color -> CRGB
CRGB htmlColor2Crgb(String htmlColor) {
  if (htmlColor.length() != 7 || htmlColor[0] != '#') {
    Serial.println("Invalid HTML color format");
    return CRGB(255, 223, 191); // default color (warm white)
  }
  unsigned long colorCode = strtoul(&htmlColor[1], NULL, 16);
  return CRGB(colorCode);
}

CHSV htmlColor2Chsv(String htmlColor) {
  if (htmlColor.length() != 7 || htmlColor[0] != '#') {
    Serial.println("Invalid HTML color format");
    return CHSV(25, 255, 255); // default color (warm white)
  }
  CHSV hsvColor = rgb2hsv_approximate(htmlColor2Crgb(htmlColor));
  return hsvColor;
}
