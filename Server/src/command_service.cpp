#include "command_service.h"

#include <algorithm>
#include <functional>

#include "common.h"
#include "controller.h"
#include "led.h"
#include "led_strip_dvc.h"
#include "main.h"
#include "render_service.h"
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

void forEachRequestedStrip(AsyncWebServerRequest* request, const std::function<void(int)>& fn) {
  if (request != nullptr && request->hasParam("stripIdx", false, false)) {
    fn(request->getParam("stripIdx", false, false)->value().toInt());
    return;
  }

  forEachLedStrip([&](LedStripDvc& dvc) {
    fn(dvc.ledIdx);
  });
}

void updateSection(AsyncWebServerRequest* request, int sectionCount, int sectionIdx, CRGB color) {
  forEachRequestedStrip(request, [&](int stripIdx) {
    requestFillSection(stripIdx, sectionCount, sectionIdx, color);
  });
}

void startTaskRunningRainbow(AsyncWebServerRequest* request) {
  int delay = request->getParam("delay", false, false)->value().toInt();
  int step = request->getParam("step", false, false)->value().toInt();
  int delta = request->getParam("delta", false, false)->value().toInt();

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting running rainbow effect on strip %d\n", stripIdx);
    requestEffectRunningRainbow(stripIdx, delay, step, delta);
  });
}

void startTaskStrobe(AsyncWebServerRequest* request) {
  int delay1 = request->getParam("delay1", false, false)->value().toInt();
  int delay2 = request->getParam("delay2", false, false)->value().toInt();
  CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

  if (request->hasParam("stripIdx", false, false)) {
    int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
    Serial.printf("Starting strobe effect on strip %d\n", stripIdx);
    requestEffectStrobe(stripIdx, color, delay1, delay2);
    return;
  }

  Serial.println("Starting strobe effect on all strips");
  forEachLedStrip([&](LedStripDvc& dvc) {
    requestEffectStrobe(dvc.ledIdx, color, delay1, delay2);
  });
}

void startTaskStrobeRandom(AsyncWebServerRequest* request) {
  CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting strobe random effect on strip %d\n", stripIdx);
    requestEffectStrobeRandom(stripIdx, color);
  });
}

void startTaskSolidColor(AsyncWebServerRequest* request) {
  if (request->hasParam("color")) {
    CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      updateSection(request, sectionCount, sectionIdx, color);
      return;
    }

    Serial.println("Solid color via ledStrips object.");
    forEachRequestedStrip(request, [&](int stripIdx) {
      Serial.printf("Filling strip idx %d with color (R:%d, G:%d, B:%d)\n",
          stripIdx, color.r, color.g, color.b);
      requestFillSolid(stripIdx, color);
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
      updateSection(request, sectionCount, sectionIdx, color);
      return;
    }

    if (request->hasParam("stripIdx", false, false)) {
      int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
      requestFillSolid(stripIdx, color);
      return;
    }

    forEachLedStrip([&](LedStripDvc& dvc) {
      requestFillSolid(dvc.ledIdx, color);
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

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting fade-in effect on strip %d\n", stripIdx);
    requestEffectFadeIn(stripIdx, color, duration);
  });
}

void startTaskFadeOut(AsyncWebServerRequest* request) {
  if (!request->hasParam("duration")) {
    request->send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }

  int duration = request->getParam("duration", false, false)->value().toInt();
  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting fade-out effect on strip %d\n", stripIdx);
    requestEffectFadeOut(stripIdx, duration);
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

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting blend effect on strip %d\n", stripIdx);
    requestEffectBlend(stripIdx, color, duration);
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

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting gradient effect on strip %d\n", stripIdx);
    requestFillGradientHSV(stripIdx, chsvStart, chsvEnd);
  });
}

void handleOff(AsyncWebServerRequest* request) {
  forEachRequestedStrip(request, [&](int stripIdx) {
    requestOff(stripIdx);
  });
}

void handleNoise(AsyncWebServerRequest* request) {
  (void)request;
}

void handleAbort(AsyncWebServerRequest* request) {
  (void)request;
  forEachRequestedStrip(request, [&](int stripIdx) {
    requestAbortRender(stripIdx);
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
      requestFillSolid(dvc.ledIdx, color);
    });
  }
}

void processWebSocketBinary(uint8_t* payload, size_t length) {
  const int totalLedCount = getTotalLedCount();
  if (length != totalLedCount * 3) {
    Serial.println("Incorrect data length");
    return;
  }

  requestApplyBinaryFrame(payload, length);
}

void echoWebSocketMessage(WebSocketsServer& webSocket, uint8_t clientNum, const String& message) {
  String echo = message;
  webSocket.sendTXT(clientNum, echo);
}
