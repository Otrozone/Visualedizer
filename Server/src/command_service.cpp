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
bool hasQueryParameter(const String& url, const String& parameterName);

class CommandRequest {
 public:
  explicit CommandRequest(AsyncWebServerRequest* httpRequest)
      : request(httpRequest), message(nullptr), responseSent(false) {
  }

  explicit CommandRequest(const String& webSocketMessage)
      : request(nullptr), message(&webSocketMessage), responseSent(false) {
  }

  bool hasParam(const char* name) const {
    if (request != nullptr) {
      return request->hasParam(name, false, false);
    }

    return message != nullptr && hasQueryParameter(*message, name);
  }

  String param(const char* name) const {
    if (request != nullptr) {
      if (!request->hasParam(name, false, false)) {
        return "";
      }

      return request->getParam(name, false, false)->value();
    }

    if (message == nullptr) {
      return "";
    }

    return getQueryParameterValue(*message, name);
  }

  void send(int code, const char* contentType, const String& body) const {
    if (request == nullptr || responseSent) {
      return;
    }

    request->send(code, contentType, body);
    responseSent = true;
  }

  bool hasSentResponse() const {
    return responseSent;
  }

 private:
  AsyncWebServerRequest* request;
  const String* message;
  mutable bool responseSent;
};

using CommandHandler = void (*)(CommandRequest&);

struct CommandEntry {
  const char* cmd;
  CommandHandler handler;
};

void handleOff(CommandRequest& request);

int getTotalLedCount() {
  int totalLedCount = 0;
  forEachLedStrip([&](LedStripDvc& dvc) {
    totalLedCount += dvc.ledCount;
  });

  return totalLedCount;
}

bool hasQueryParameter(const String& url, const String& parameterName) {
  return url.indexOf("?" + parameterName + "=") != -1
      || url.indexOf("&" + parameterName + "=") != -1;
}

void forEachRequestedStrip(CommandRequest& request, const std::function<void(int)>& fn) {
  if (request.hasParam("stripIdx")) {
    fn(request.param("stripIdx").toInt());
    return;
  }

  forEachLedStrip([&](LedStripDvc& dvc) {
    fn(dvc.ledIdx);
  });
}

void updateSection(CommandRequest& request, int sectionCount, int sectionIdx, CRGB color) {
  forEachRequestedStrip(request, [&](int stripIdx) {
    requestFillSection(stripIdx, sectionCount, sectionIdx, color);
  });
}

void startTaskRunningRainbow(CommandRequest& request) {
  int delay = request.param("delay").toInt();
  int step = request.param("step").toInt();
  int delta = request.param("delta").toInt();

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting running rainbow effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestEffectRunningRainbow(stripIdx, delay, step, delta);
  });
}

void startTaskStrobe(CommandRequest& request) {
  int delay1 = request.param("delay1").toInt();
  int delay2 = request.param("delay2").toInt();
  CRGB color = htmlColor2Crgb(request.param("color"));

  if (request.hasParam("stripIdx")) {
    int stripIdx = request.param("stripIdx").toInt();
    Serial.printf("Starting strobe effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestEffectStrobe(stripIdx, color, delay1, delay2);
    return;
  }

  Serial.println("Starting strobe effect on all strips");
  forEachLedStrip([&](LedStripDvc& dvc) {
    requestEffectStrobe(dvc.ledIdx, color, delay1, delay2);
  });
}

void startTaskStrobeRandom(CommandRequest& request) {
  CRGB color = htmlColor2Crgb(request.param("color"));

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting strobe random effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestEffectStrobeRandom(stripIdx, color);
  });
}

void startTaskSolidColor(CommandRequest& request) {
  if (request.hasParam("color")) {
    CRGB color = htmlColor2Crgb(request.param("color"));

    if (request.hasParam("section-count") && request.hasParam("section-index")) {
      int sectionCount = request.param("section-count").toInt();
      int sectionIdx = request.param("section-index").toInt();
      updateSection(request, sectionCount, sectionIdx, color);
      return;
    }

    Serial.println("Solid color via ledStrips object.");
    forEachRequestedStrip(request, [&](int stripIdx) {
      Serial.printf("Filling strip idx %d [%d] with color (R:%d, G:%d, B:%d)\n",
          stripIdx, DVC_DATA_PIN_LIST[stripIdx], color.r, color.g, color.b);
      requestFillSolid(stripIdx, color);
    });
    return;
  }

  if (request.hasParam("hue") && request.hasParam("saturation") && request.hasParam("value")) {
    int hue = request.param("hue").toInt();
    int saturation = request.param("saturation").toInt();
    int value = request.param("value").toInt();
    CHSV color = CHSV(hue, saturation, value);

    if (request.hasParam("section-count") && request.hasParam("section-index")) {
      int sectionCount = request.param("section-count").toInt();
      int sectionIdx = request.param("section-index").toInt();
      updateSection(request, sectionCount, sectionIdx, color);
      return;
    }

    if (request.hasParam("stripIdx")) {
      int stripIdx = request.param("stripIdx").toInt();
      Serial.printf("Filling strip idx %d [%d] with color (H:%d, S:%d, V:%d)\n",
          stripIdx, DVC_DATA_PIN_LIST[stripIdx], color.h, color.s, color.v);
      requestFillSolid(stripIdx, color);
      return;
    }

    forEachLedStrip([&](LedStripDvc& dvc) {
      requestFillSolid(dvc.ledIdx, color);
    });
    return;
  }

  request.send(200, "text/plain", "Command solid-color: Missing required parameters.");
}

void startTaskFadeIn(CommandRequest& request) {
  CRGB color;
  int duration = request.param("duration").toInt();

  if (request.hasParam("color") && request.hasParam("duration")) {
    color = htmlColor2Crgb(request.param("color"));
  } else if (request.hasParam("hue") && request.hasParam("saturation")
      && request.hasParam("value") && request.hasParam("duration")) {
    int hue = request.param("hue").toInt();
    int saturation = request.param("saturation").toInt();
    int value = request.param("value").toInt();
    color = CHSV(hue, saturation, value);
  } else {
    request.send(200, "text/plain", "Command fade-in: Missing required parameters.");
    return;
  }

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting fade-in effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestEffectFadeIn(stripIdx, color, duration);
  });
}

void startTaskFadeOut(CommandRequest& request) {
  if (!request.hasParam("duration")) {
    request.send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }

  int duration = request.param("duration").toInt();
  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting fade-out effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestEffectFadeOut(stripIdx, duration);
  });
}

void startTaskBlend(CommandRequest& request) {
  int duration = request.param("duration").toInt();
  CRGB color;

  if (request.hasParam("color") && request.hasParam("duration")) {
    color = htmlColor2Crgb(request.param("color"));
  } else if (request.hasParam("hue") && request.hasParam("saturation")
      && request.hasParam("value") && request.hasParam("duration")) {
    int hue = request.param("hue").toInt();
    int saturation = request.param("saturation").toInt();
    int value = request.param("value").toInt();
    color = CHSV(hue, saturation, value);
  } else {
    request.send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting blend effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestEffectBlend(stripIdx, color, duration);
  });
}

void startTaskGradient(CommandRequest& request) {
  int hueStart = request.param("hueStart").toInt();
  int hueEnd = request.param("hueEnd").toInt();
  int brightness = request.param("brightness").toInt();

  CHSV chsvStart;
  chsvStart.hue = mapRange(std::min(hueStart, hueEnd), 0, 360, 0, 255);
  chsvStart.value = mapRange(brightness, 0, 100, 0, 255);
  chsvStart.saturation = 255;

  CHSV chsvEnd;
  chsvEnd.hue = mapRange(std::max(hueStart, hueEnd), 0, 360, 0, 255);
  chsvEnd.value = mapRange(brightness, 0, 100, 0, 255);
  chsvEnd.saturation = 255;

  forEachRequestedStrip(request, [&](int stripIdx) {
    Serial.printf("Starting gradient effect on strip %d [%d]\n", stripIdx, DVC_DATA_PIN_LIST[stripIdx]);
    requestFillGradientHSV(stripIdx, chsvStart, chsvEnd);
  });
}

void handleOff(CommandRequest& request) {
  forEachRequestedStrip(request, [&](int stripIdx) {
    requestOff(stripIdx);
  });
}

void handleNoise(CommandRequest& request) {
  (void)request;
}

void handleAbort(CommandRequest& request) {
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

void handleUpdateCommand(CommandRequest& request) {
  if (!request.hasParam("command")) {
    request.send(400, "text/plain", "No 'command' query parameter received");
    return;
  }

  String cmd = request.param("command");

  if (cmd.length() == 0) {
    request.send(400, "text/plain", "Empty 'command' value received");
    return;
  }

  updateActivity();
  if (request.hasParam("activity-timeout")) {
    Serial.printf("Setting activity timeout");
    tmpActivityTimeout = request.param("activity-timeout").toInt();
    activityTimeoutRequested = true;

    if (request.hasParam("fadeout-duration")) {
      fadeOutDuration = request.param("fadeout-duration").toInt();
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

  if (!request.hasSentResponse()) {
    request.send(200, "text/plain", "Processing command: " + cmd);
  }
}
}  // namespace

void handleUpdateRequest(AsyncWebServerRequest* request) {
  CommandRequest commandRequest(request);
  handleUpdateCommand(commandRequest);
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

  CommandRequest commandRequest(message);
  handleUpdateCommand(commandRequest);
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
