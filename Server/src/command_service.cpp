#include "command_service.h"

#include <algorithm>

#include "common.h"
#include "controller.h"
#include "effects.h"
#include "led.h"
#include "led_strip_dvc.h"
#include "main.h"
#include "runtime_service.h"

namespace {
using CommandHandler = void (*)(AsyncWebServerRequest*);

void handleOff(AsyncWebServerRequest* request);

int getTotalLedCount() {
  int totalLedCount = 0;
  forEachLedStrip([&](LedStripDvc& dvc) {
    totalLedCount += dvc.ledCount;
  });

  return totalLedCount;
}

void updateSection(int sectionCount, int sectionIdx, CRGB color) {
  forEachLedStrip([&](LedStripDvc& dvc) {
    fill_solid(dvc.leds, dvc.ledCount, CRGB::Black);

    int sectionLength = dvc.ledCount / sectionCount;
    int startIndex = sectionIdx * sectionLength;
    int endIndex = startIndex + sectionLength;

    for (int i = startIndex; i < endIndex; i++) {
      dvc.leds[i] = color;
    }
  });
}

void startTaskRunningRainbow(AsyncWebServerRequest* request) {
  int delay = request->getParam("delay", false, false)->value().toInt();
  int step = request->getParam("step", false, false)->value().toInt();
  int delta = request->getParam("delta", false, false)->value().toInt();

  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting running rainbow effect on strip %d\n", dvc.ledIdx);
    dvc.runEffectRunningRainbow(delay, step, delta);
  });
}

void startTaskStrobe(AsyncWebServerRequest* request) {
  int delay1 = request->getParam("delay1", false, false)->value().toInt();
  int delay2 = request->getParam("delay2", false, false)->value().toInt();
  CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

  if (request->hasParam("stripIdx", false, false)) {
    int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
    Serial.printf("Starting strobe effect on strip %d\n", stripIdx);
    ledStrips[stripIdx]->runEffectStrobe(color, delay1, delay2);
    return;
  }

  Serial.println("Starting strobe effect on all strips");
  forEachLedStrip([&](LedStripDvc& dvc) {
    dvc.runEffectStrobe(color, delay1, delay2);
  });
}

void startTaskStrobeRandom(AsyncWebServerRequest* request) {
  CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting strobe random effect on strip %d\n", dvc.ledIdx);
    dvc.runEffectStrobeRandom(color);
  });
}

void startTaskSolidColor(AsyncWebServerRequest* request) {
  if (request->hasParam("color")) {
    CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      updateSection(sectionCount, sectionIdx, color);
      FastLedShow();
      return;
    }

    Serial.println("Solid color via ledStrips object.");
    forEachLedStrip([&](LedStripDvc& dvc) {
      Serial.printf("Filling strip idx %d with color (R:%d, G:%d, B:%d)\n",
          dvc.ledIdx, color.r, color.g, color.b);
      dvc.fillSolid(color);
    });
    return;
  }

  if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value")) {
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();
    CHSV color = CHSV(hue, saturation, value);

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      updateSection(sectionCount, sectionIdx, color);
      FastLedShow();
      return;
    }

    if (request->hasParam("stripIdx", false, false)) {
      int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
      ledStrips[stripIdx]->fillSolid(color);
      return;
    }

    forEachLedStrip([&](LedStripDvc& dvc) {
      dvc.fillSolid(color);
    });
    return;
  }

  request->send(200, "text/plain", "Command solid-color: Missing required parameters.");
}

void startTaskFadeIn(AsyncWebServerRequest* request) {
  CRGB color;
  int duration = request->getParam("duration", false, false)->value().toInt();

  if (request->hasParam("color") && request->hasParam("duration")) {
    color = htmlColor2Crgb(request->getParam("color", false, false)->value());
  } else if (request->hasParam("hue") && request->hasParam("saturation")
      && request->hasParam("value") && request->hasParam("duration")) {
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();
    color = CHSV(hue, saturation, value);
  } else {
    request->send(200, "text/plain", "Command fade-in: Missing required parameters.");
    return;
  }

  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting fade-in effect on strip %d\n", dvc.ledIdx);
    dvc.runEffectFadeIn(color, duration);
  });
}

void startTaskFadeOut(AsyncWebServerRequest* request) {
  if (!request->hasParam("duration")) {
    request->send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }

  int duration = request->getParam("duration", false, false)->value().toInt();
  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting fade-out effect on strip %d\n", dvc.ledIdx);
    dvc.runEffectFadeOut(duration);
  });
}

void startTaskBlend(AsyncWebServerRequest* request) {
  int duration = request->getParam("duration", false, false)->value().toInt();
  CRGB color;

  if (request->hasParam("color") && request->hasParam("duration")) {
    color = htmlColor2Crgb(request->getParam("color", false, false)->value());
  } else if (request->hasParam("hue") && request->hasParam("saturation")
      && request->hasParam("value") && request->hasParam("duration")) {
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();
    color = CHSV(hue, saturation, value);
  } else {
    request->send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }

  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting blend effect on strip %d\n", dvc.ledIdx);
    dvc.runEffectBlend(color, duration);
  });
}

void startTaskGradient(AsyncWebServerRequest* request) {
  int hueStart = request->getParam("hueStart", false, false)->value().toInt();
  int hueEnd = request->getParam("hueEnd", false, false)->value().toInt();
  int brightness = request->getParam("brightness", false, false)->value().toInt();

  CHSV chsvStart;
  chsvStart.hue = mapRange(std::min(hueStart, hueEnd), 0, 360, 0, 255);
  chsvStart.value = mapRange(brightness, 0, 100, 0, 255);
  chsvStart.saturation = 255;

  CHSV chsvEnd;
  chsvEnd.hue = mapRange(std::max(hueStart, hueEnd), 0, 360, 0, 255);
  chsvEnd.value = mapRange(brightness, 0, 100, 0, 255);
  chsvEnd.saturation = 255;

  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting gradient effect on strip %d\n", dvc.ledIdx);
    dvc.fillGradientHSV(chsvStart, chsvEnd);
  });
}

void handleOff(AsyncWebServerRequest* request) {
  forEachLedStrip([&](LedStripDvc& dvc) {
    dvc.off();
  });
}

void handleNoise(AsyncWebServerRequest* request) {
  (void)request;
}

void handleAbort(AsyncWebServerRequest* request) {
  (void)request;
  forEachLedStrip([&](LedStripDvc& dvc) {
    dvc.terminateCurrTask();
  });
}

const CommandEntry commandTable[] = {
  {"off", handleOff},
  {"rainbow", startTaskRunningRainbow},
  {"strobe", startTaskStrobe},
  {"strobe-random", startTaskStrobeRandom},
  {"solid-color", startTaskSolidColor},
  {"fade-in", startTaskFadeIn},
  {"fade-out", startTaskFadeOut},
  {"blend", startTaskBlend},
  {"gradient", startTaskGradient},
  {"noise", handleNoise},
  {"abort", handleAbort},
};
}  // namespace

void handleUpdateRequest(AsyncWebServerRequest* request) {
  if (!request->hasParam("command", false, false)) {
    request->send(400, "text/plain", "No 'command' query parameter received");
    return;
  }

  AsyncWebParameter* command = request->getParam("command", false, false);
  String cmd = command->value();

  if (cmd.length() == 0) {
    request->send(400, "text/plain", "Empty 'command' value received");
    return;
  }

  updateActivity();
  if (request->hasParam("activity-timeout")) {
    Serial.printf("Setting activity timeout");
    tmpActivityTimeout = request->getParam("activity-timeout", false, false)->value().toInt();
    activityTimeoutRequested = true;

    if (request->hasParam("fadeout-duration")) {
      fadeOutDuration = request->getParam("fadeout-duration", false, false)->value().toInt();
    }
  }

  Serial.println("Executing command: " + cmd);

  bool found = false;
  for (const auto& entry : commandTable) {
    if (cmd.equals(entry.cmd)) {
      entry.handler(request);
      found = true;
      break;
    }
  }

  if (!found) {
    Serial.println("Unknown command: " + cmd);
  }

  request->send(200, "text/plain", "Processing command: " + cmd);
}

void processWebSocketMessage(const String& message) {
  Serial.println("WebSockets message: " + message);

  int idx = message.indexOf('?');
  String urlPath = (idx != -1) ? message.substring(0, idx) : message;
  Serial.println("URL path of WebSocket message: " + urlPath);

  if (urlPath.equals("ctrl")) {
    handleCtrlSignalWs(message);
    return;
  }

  if (!urlPath.equals("update")) {
    return;
  }

  String cmd = getQueryParameterValue(message, "command");
  if (cmd.length() == 0) {
    return;
  }

  Serial.println("WebSockets command: " + cmd);

  if (cmd.equals("off")) {
    handleOff(nullptr);
  } else if (cmd.equals("solid-color")) {
    CRGB color = htmlColor2Crgb(getQueryParameterValue(message, "color"));
    forEachLedStrip([&](LedStripDvc& dvc) {
      dvc.fillSolid(color);
    });
  }
}

void processWebSocketBinary(uint8_t* payload, size_t length) {
  const int totalLedCount = getTotalLedCount();
  if (length != totalLedCount * 3) {
    Serial.println("Incorrect data length");
    return;
  }

  int payloadOffset = 0;
  forEachLedStrip([&](LedStripDvc& dvc) {
    dvc.terminateCurrTask();
    for (int i = 0; i < dvc.ledCount; i++) {
      dvc.leds[i] = CRGB(payload[payloadOffset], payload[payloadOffset + 1], payload[payloadOffset + 2]);
      payloadOffset += 3;
    }
  });
  FastLedShow();
}

void echoWebSocketMessage(WebSocketsServer& webSocket, uint8_t clientNum, const String& message) {
  String echo = message;
  webSocket.sendTXT(clientNum, echo);
}
