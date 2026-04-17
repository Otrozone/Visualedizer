// https://wiki.seeedstudio.com/xiao_esp32s3_getting_started/
// https://www.freertos.org/a00125.html
// https://fastled.io/docs/

// #include <Arduino.h>
#include <WiFi.h>
// #include <FastLED.h>
#include <WebSocketsServer.h>
// #include <Preferences.h>
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include <SPIFFS.h>

#include "main.h"
// #include "devices.h"
#include "credentials.h"
#include "nvm.h"
#include "effects.h"
#include "dmx.h"
#include "controller.h"
#include "controller_ir.h"

Preferences preferences;

AsyncWebServer server(80);

WebSocketsServer webSocket = WebSocketsServer(81);

String wifiSsid = WIFI_SSID;
String wifiPassword = WIFI_PASSWORD;
DeviceWifiModeType wifiMode = DEVICE_WIFI_MODE_AUTO;

String wifiApSsid = "Visualedizer";
String wifiApPassword = "Rezidelausiv7331";

const uint8_t wifiTimeout = 15;

String deviceName = "Unnamed";

String htmlContent = "";

String taskCommand = "";
// TaskHandle_t taskHandle;
// const uint32_t StackSize = 2048; // What is the good value for my operations?

static uint8_t hue = 0;

bool bootFadeIn = false;
String bootColor = "#FFFAFA";
bool bootWol = false;
String bootWolMac = "00:00:00:00:00:00";

bool webSockEnabled;

unsigned long lastActivity = 0;
bool activityTimeoutEnabled = false;
unsigned long activityTimeout = 0;
bool activityTimeoutRequested = false;
unsigned long tmpActivityTimeout = 0;
int fadeOutDuration = 10000;

bool irEnabled = false;

static unsigned long lastReconnectAttempt = 0;

static int getTotalLedCount() {
  int totalLedCount = 0;
  forEachLedStrip([&](LedStripDvc& dvc) {
    totalLedCount += dvc.ledCount;
  });

  return totalLedCount;
}

void handleOff(AsyncWebServerRequest* request);

void updateActivity() {
  Serial.printf("Updating last activity (%d)\n", lastActivity);
  lastActivity = millis();

  if (activityTimeoutEnabled) {
    activityTimeoutRequested = true;
  }
}

void processMessage(String message) {
  Serial.println("WebSockets message: " + message);

  int idx = message.indexOf('?');
  String urlPath = (idx != -1) ? message.substring(0, idx) : message;
  Serial.println("URL path of WebSocket message: " + urlPath);

  if (urlPath.equals("ctrl")) {
    handleCtrlSignalWs(message);
  }

  if (urlPath.equals("update")) {
    String cmd = getQueryParameterValue(message, "command");
    if (cmd.length() > 0) {
      Serial.println("WebSockets command: " + cmd);

      if (cmd.equals("off")) {
        terminateCurrTask();
        handleOff(nullptr);
      } else if (cmd.equals("solid-color")) {
        terminateCurrTask();
        CRGB color = htmlColor2Crgb(getQueryParameterValue(message, "color"));
        forEachLedStrip([&](LedStripDvc& dvc) {
          fill_solid(dvc.leds, dvc.ledCount, color);
        });
        FastLedShow();
      }
    }
  }
}

String sendHttpGetRequest(String uri) {
  Serial.println("Sending HTTP GET request");
  HTTPClient http;
  http.setTimeout(3000);
  http.begin(uri);
  int httpCode = http.GET();

  String result;
  if (httpCode > 0) {
    result = http.getString();
  } else {
    result = "HTTP GET Request failed";
  }

  Serial.println("HTTP GET Response: " + result);
  http.end();

  return result;
}

void sendMagicPacket(const char* macAddress) {
  byte mac[6];
  sscanf(macAddress, "%hhx:%hhx:%hhx:%hhx:%hhx:%hhx", &mac[0], &mac[1], &mac[2], &mac[3], &mac[4], &mac[5]);

  byte packet[102];
  for (int i = 0; i < 6; i++) {
    packet[i] = 0xFF;
  }
  for (int i = 1; i <= 16; i++) {
    memcpy(&packet[i * 6], &mac, 6 * sizeof(byte));
  }

  WiFiUDP udp;
  udp.beginPacket(IPAddress(255, 255, 255, 255), 9);
  udp.write(packet, sizeof(packet));
  udp.endPacket();
}

void handleWol(AsyncWebServerRequest *request) {
  if (request->hasParam("mac")) {
    String macAddress = request->getParam("mac")->value();
    sendMagicPacket(macAddress.c_str());
    request->send(200, "text/plain", "Magic packet sent to " + macAddress);
  } else {
    request->send(400, "text/plain", "Missing 'mac' query parameter");
  }
}

void updateSection(int sectionCount, int sectionIdx, CRGB color) {
  forEachLedStrip([&](LedStripDvc& dvc) {
    fill_solid(dvc.leds, dvc.ledCount, CRGB::Black);

    int sectionLength = dvc.ledCount / sectionCount;
    int sectionToLight = sectionIdx;

    int startIndex = sectionToLight * sectionLength;
    int endIndex = startIndex + sectionLength;

    for (int i = startIndex; i < endIndex; i++) {
      dvc.leds[i] = color;
    }
  });
}

void startTaskRunningRainbow(AsyncWebServerRequest *request) {
  int delay = request->getParam("delay", false, false)->value().toInt();
  int step = request->getParam("step", false, false)->value().toInt();
  int delta = request->getParam("delta", false, false)->value().toInt();

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      Serial.printf("Starting running rainbow effect on strip %d\n", i);
      ledStrips[i]->runEffectRunningRainbow(delay, step, delta);
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }
}

void startTaskStrobe(AsyncWebServerRequest *request) {
  int delay1 = request->getParam("delay1", false, false)->value().toInt();
  int delay2 = request->getParam("delay2", false, false)->value().toInt();
  CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

  if (request->hasParam("stripIdx", false, false)) {
    int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
    Serial.printf("Starting strobe effect on strip %d\n", stripIdx);
    ledStrips[stripIdx]->runEffectStrobe(color, delay1, delay2);
  } else {
    Serial.println("Starting strobe effect on all strips");
    for (int i = 0; i < DVC_STRIP_COUNT; i++) {
      if (ledStrips[i] != nullptr) {
        ledStrips[i]->runEffectStrobe(color, delay1, delay2);
      }
    }
  }
}

void startTaskStrobeRandom(AsyncWebServerRequest *request) {
  CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      Serial.printf("Starting strobe random effect on strip %d\n", i);
      ledStrips[i]->runEffectStrobeRandom(color);
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }
}

/*CRGB getColorDefinition(AsyncWebServerRequest *request) {
  CRGB color;

  if (request->hasParam("color")) {
    // RGB
    color = htmlColor2Crgb(request->getParam("color", false, false)->value());
  } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value")) {
    // HSV
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();
    color = CHSV(hue, saturation, value);
  } else {
    Serial.println("Error: Missing color parameters");
  }

  return color;
}*/

void startTaskSolidColor(AsyncWebServerRequest *request) {
  if (request->hasParam("color")) {
    // RGB
    // terminateCurrTask();
    CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      updateSection(sectionCount, sectionIdx, color);
    } else {
      
      Serial.println("Solid color via ledStrips object.");
      for (int i = 0; i < DVC_STRIP_COUNT; i++) {
        Serial.printf("Solid-color - Led strip %d count: %d\n", i, ledStrips[i]->ledCount);

        if (ledStrips[i] != nullptr) {
          Serial.printf("Filling strip idx %d with color (R:%d, G:%d, B:%d)\n", ledStrips[i]->ledIdx, color.r, color.g, color.b);
          ledStrips[i]->fillSolid(color);
        } else {
          Serial.printf("Led strip %d is null\n", i);
        }
      }
      
    }

  } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value")) {
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();
    CHSV color = CHSV(hue, saturation, value);

    if (request->hasParam("section-count") && request->hasParam("section-index")) {
      int sectionCount = request->getParam("section-count", false, false)->value().toInt();
      int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
      
      updateSection(sectionCount, sectionIdx,  color);
    } else {
      if (request->hasParam("stripIdx", false, false)) {
        int stripIdx = request->getParam("stripIdx", false, false)->value().toInt();
        ledStrips[stripIdx]->fillSolid(color);
      } else {
        Serial.println("Solid color command HSV");
        
        for (int i = 0; i < DVC_STRIP_COUNT; i++) {
          if (ledStrips[i] != nullptr) {
            ledStrips[i]->fillSolid(color);
          } else {
            Serial.printf("Led strip %d is null\n", i);
          }
        }
      }
    }
  } else {
    request->send(200, "text/plain", "Command solid-color: Missing required parameters.");
    return;
  }
}

void startTaskFadeIn(AsyncWebServerRequest *request) {
  CRGB color;
  int duration = request->getParam("duration", false, false)->value().toInt();

  if (request->hasParam("color") && request->hasParam("duration")) {
    // RGB
    color = htmlColor2Crgb(request->getParam("color", false, false)->value());
  } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value") && request->hasParam("duration")) {
    // HSV

    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();

    color = CHSV(hue, saturation, value);
  } else {
    request->send(200, "text/plain", "Command fade-in: Missing required parameters.");
    return;
  }

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      Serial.printf("Starting fade-in effect on strip %d\n", i);
      ledStrips[i]->runEffectFadeIn(color, duration);
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }
}

void startTaskFadeOut(AsyncWebServerRequest *request) {
  if (request->hasParam("duration")) {
    int duration = request->getParam("duration", false, false)->value().toInt();
    for (int i = 0; i < DVC_STRIP_COUNT; i++) {
      if (ledStrips[i] != nullptr) {
        Serial.printf("Starting fade-out effect on strip %d\n", i);
        ledStrips[i]->runEffectFadeOut(duration);
      } else {
        Serial.printf("Led strip %d is null\n", i);
      }
    }
  } else {
    request->send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }
}

void startTaskBlend(AsyncWebServerRequest *request) {
  int duration = request->getParam("duration", false, false)->value().toInt();
  CRGB color;

  if (request->hasParam("color") && request->hasParam("duration")) {
    // RGB
    color = htmlColor2Crgb(request->getParam("color", false, false)->value());
  } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value") && request->hasParam("duration")) {
    // HSV
    int hue = request->getParam("hue", false, false)->value().toInt();
    int saturation = request->getParam("saturation", false, false)->value().toInt();
    int value = request->getParam("value", false, false)->value().toInt();

    color = CHSV(hue, saturation, value);
  } else {
    request->send(200, "text/plain", "Command fade-out: Missing required parameters.");
    return;
  }

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      Serial.printf("Starting blend effect on strip %d\n", i);
      ledStrips[i]->runEffectBlend(color, duration);
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }
}

void startTaskGradient(AsyncWebServerRequest *request) {
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

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      Serial.printf("Starting gradient effect on strip %d\n", i);
      ledStrips[i]->fillGradientHSV(chsvStart, chsvEnd);
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }
}

void handleOff(AsyncWebServerRequest* request) {
  terminateCurrTask();

  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      ledStrips[i]->off();
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }

  FastLedShow();
}

void handleNoise(AsyncWebServerRequest* request) {
  // Not yet implemented
}

void handleAbort(AsyncWebServerRequest* request) {
  vTaskDelete(taskHandle);
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

void update(AsyncWebServerRequest *request) {
  if (request->hasParam("command", false, false)) {
    AsyncWebParameter* command = request->getParam("command", false, false);
    String cmd = command->value();

    if (cmd.length() > 0) {
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

      // Search for the command in the command table
      // and call the corresponding handler function
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

      taskCommand = cmd;
      request->send(200, "text/plain", "Processing command: " + cmd);

    } else {
      request->send(400, "text/plain", "Empty 'command' value received");
    }
  } else {
    request->send(400, "text/plain", "No 'command' query parameter received");
  }
}

String getContentType(String filename) {
  String ext = filename.substring(filename.lastIndexOf('.') + 1);
  Serial.println("File extension for content type: " + ext);
  if (ext == "html") return "text/html";
  else if (ext == "css") return "text/css";
  else if (ext == "js") return "text/javascript";
  else if (ext == "png") return "image/png";
  else if (ext == "jpg") return "image/jpeg";
  else if (ext == "ico") return "image/x-icon";
  else if (ext == "xml") return "text/xml";
  else if (ext == "pdf") return "application/pdf";
  else if (ext == "zip") return "application/zip";
  else if (ext == "ttf") return "application/octet-stream";

  return "text/plain";
}

void handleStaticResource(AsyncWebServerRequest *request) {
  String path = request->url();

  Serial.println("A static resource requested: " + path);
  if (SPIFFS.exists(path)) {
    request->send(SPIFFS, path, getContentType(path));
  } else {
    Serial.println("File not found in SPIFFS: " + path);
    request->send(404, "text/plain", "File Not Found");
  }
}

void handleRootRequest(AsyncWebServerRequest *request) {
  request->send(SPIFFS, "/index.html", "text/html");
}

void initHttpServer() {
  server.on("/test", [](AsyncWebServerRequest *request) {
    request->send(200, "text/plain", "Hello from server.");
  });

  // Serve static resources
  server.on("/", HTTP_GET, handleRootRequest);
  server.on("/res/*", HTTP_GET, handleStaticResource);

  // Rest API endpoints
  server.on("/get-conf", HTTP_GET, handleGetConf);
  server.on("/set-conf", HTTP_POST, [](AsyncWebServerRequest *request) {}, NULL, handleSetConf);
  server.on("/update", HTTP_GET, update);
  server.on("/wol", HTTP_GET, handleWol);
  server.on("/ctrl", HTTP_GET, handleCtrlSignalHttp);

  server.begin();
  Serial.println("HTTP server started");
}

void initSpiffs() {
  Serial.println("SPIFFS initialization");

  if (!SPIFFS.begin(true)) {
    Serial.println("An Error has occurred while mounting SPIFFS");
    return;
  }

  File file = SPIFFS.open("/index.html");
  if (!file) {
    Serial.println("Failed to open file for reading");
    return;
  }

  Serial.println("Loading html content from SPIFFS");
  htmlContent = "";
  while (file.available()) {
    char c = file.read(); 
    htmlContent += c;
  }
  file.close();
}

void processWsTxtPayload(uint8_t num, uint8_t *payload, size_t length) {
  String message = "";
  for (size_t i = 0; i < length; i++) {
    message += (char)payload[i];
  }

  Serial.print("Received web socket message: ");
  Serial.println(message);

  processMessage(message);

  webSocket.sendTXT(num, message);
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t *payload, size_t length) {
  if (type == WStype_CONNECTED) {
    IPAddress ip = webSocket.remoteIP(num);
    Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);
  }

  if (type == WStype_BIN) {
    const int totalLedCount = getTotalLedCount();
    if (length == totalLedCount * 3) {
      int payloadOffset = 0;
      forEachLedStrip([&](LedStripDvc& dvc) {
        for (int i = 0; i < dvc.ledCount; i++) {
          dvc.leds[i] = CRGB(payload[payloadOffset], payload[payloadOffset + 1], payload[payloadOffset + 2]);
          payloadOffset += 3;
        }
      });
      FastLedShow();
    } else {
      Serial.println("Incorrect data length");
    }
  }

  if (type == WStype_TEXT) {
    processWsTxtPayload(num, payload, length);
  }

  if (type == WStype_DISCONNECTED) {
    forEachLedStrip([](LedStripDvc& dvc) {
      fill_solid(dvc.leds, dvc.ledCount, CRGB(0, 0, 0));
    });
    FastLedShow();
  }
}

int getWifiSignalStrength() {
  long rssi = WiFi.RSSI();
  
  if (rssi >= -50)      return 3;
  else if (rssi >= -67) return 2;
  else                  return 1;
}

void confWifi() {
  if (deviceName != "Unnamed") {
    String hostname = deviceName;
    hostname.toLowerCase();
    WiFi.setHostname(hostname.c_str());
  }

  switch (wifiMode) {
    case DEVICE_WIFI_MODE_STA:
      WiFi.mode(WIFI_STA);
      WiFi.begin(wifiSsid.c_str(), wifiPassword.c_str());
      break;

    case DEVICE_WIFI_MODE_AP:
      WiFi.mode(WIFI_AP);
      WiFi.softAP(wifiApSsid.c_str(), wifiApPassword.c_str());
      break;

    case DEVICE_WIFI_MODE_AP_STA:
      WiFi.mode(WIFI_AP_STA);
      WiFi.softAP(wifiApSsid.c_str(), wifiApPassword.c_str());
      WiFi.begin(wifiSsid.c_str(), wifiPassword.c_str());
      break;

    case DEVICE_WIFI_MODE_AUTO:
    default:
      WiFi.mode(WIFI_STA);
      WiFi.begin(wifiSsid.c_str(), wifiPassword.c_str());
      break;
  }
}

void waitForWifiConnection(uint8_t timeout) {
  uint8_t tryCount = 0;
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED && tryCount < timeout) {
    tryCount++;
    Serial.print('.');
    delay(1000);
  }
  Serial.println();

  if (WiFi.status() == WL_CONNECTED) {
    IPAddress ipAddress = WiFi.localIP();
    Serial.println("IP address: http://" + ipAddress.toString());
  } else {
    Serial.println("Failed to reconnect to WiFi.");
  }
}

void reconnectWifi() {
  if (WiFi.status() != WL_CONNECTED) {
    unsigned long now = millis();
    if (now - lastReconnectAttempt >= 60000) { // 1 minute
      lastReconnectAttempt = now;
      Serial.println("Attempting to reconnect to WiFi...");
      confWifi();
      WiFi.reconnect();

      waitForWifiConnection(wifiTimeout);
    }
  }
}

void initWifi() {
  confWifi();

  uint8_t tryCount = 0;
  Serial.print("Connecting to WiFi ..");
  waitForWifiConnection(wifiTimeout);

  IPAddress ipAddress;
  if (WiFi.status() == WL_CONNECTED) {
    ipAddress = WiFi.localIP();
    // Disable WiFi sleep mode
    WiFi.setSleep(false); 
  } else if (wifiMode == DEVICE_WIFI_MODE_AUTO || wifiMode == DEVICE_WIFI_MODE_AP_STA) {
    // Wifi connection timeout, switch to AP
    Serial.print("Unable to connect to WiFi, switching to AP mode");
    WiFi.mode(WIFI_AP);
    if (WiFi.softAP(wifiApSsid.c_str(), wifiApPassword.c_str())) {
      Serial.println("AP started successfully.");
      ipAddress = WiFi.softAPIP();
    } else {
      Serial.println("AP start failed.");
    }
  } else if (wifiMode == DEVICE_WIFI_MODE_AP) {
    ipAddress = WiFi.softAPIP();
  }

  Serial.println("IP address: http://" + ipAddress.toString());
}

void initWebSockets() {
  webSocket.begin();
  webSocket.onEvent(webSocketEvent);
}

void lightOn() {
  Serial.println("Turning light on");
  forEachLedStrip([](LedStripDvc& dvc) {
    fill_solid(dvc.leds, dvc.ledCount, CRGB::White);
  });
  FastLedShow();
}

uint8_t getLedMaxBrightness() {
  uint8_t maxBrightness = 0;
  forEachLedStrip([&](LedStripDvc& dvc) {
    for (int i = 0; i < dvc.ledCount; i++) {
      maxBrightness = max(maxBrightness, max(dvc.leds[i].r, max(dvc.leds[i].g, dvc.leds[i].b)));
    }
  });

  return maxBrightness;
}

void checkActivityTimout() {
  if (activityTimeoutRequested && 
      ((tmpActivityTimeout > 0 && (millis() - lastActivity > tmpActivityTimeout * 1000)) || 
       (activityTimeoutRequested && (millis() - lastActivity > activityTimeout * 1000)))) {

    Serial.printf("tmpActivityTimeout: %d, millis - last activity: %d \n",
    tmpActivityTimeout, (millis() - lastActivity));

    Serial.println("Activity timeout reached, fading out LEDs");
    activityTimeoutRequested = false;
    tmpActivityTimeout = 0;

    for (int i = 0; i < DVC_STRIP_COUNT; i++) {
      if (ledStrips[i] != nullptr) {
        Serial.printf("Running fade-out effect on strip %d\n", i);
        ledStrips[i]->runEffectFadeOut(fadeOutDuration);
      } else {
        Serial.printf("Led strip %d is null\n", i);
      }
    }
  }
}

void checkInternetAccess() {
  Serial.println("Checking internet access");
  HTTPClient http;
  http.setTimeout(3000);
  http.begin("http://www.google.com");
  int httpCode = http.GET();

  if (httpCode > 0) {
    Serial.println("Internet access is available");
  } else {
    Serial.println("No internet access");
  }

  http.end();
}

void printHearthbeat() {
  static unsigned long lastSerialPrint = 0;
  unsigned long diff = millis() - lastSerialPrint;
  if (diff >= 10000) {
    Serial.print('*');
    lastSerialPrint = millis();

    static int heartbeatCount = 0;
    heartbeatCount++;
    if (heartbeatCount >= 6) {
      Serial.println();
      Serial.print("Connection status: ");
      if (WiFi.status() == WL_CONNECTED) {
        Serial.print("connected");
        IPAddress ipAddress = WiFi.localIP();
        Serial.println(" (http://" + ipAddress.toString() + ")");
        checkInternetAccess();
      } else {
        Serial.println("disconnected");
      }
      heartbeatCount = 0;
    }
    
  }
}

// Boot procedure - phase one (before WiFi connection)
void startBootFadeIn() {
  Serial.println("Starting boot fade in");
  CRGB warmWhite = htmlColor2Crgb(bootColor);
  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    if (ledStrips[i] != nullptr) {
      Serial.printf("Starting fade-in effect on strip %d\n", i);
      ledStrips[i]->runEffectFadeIn(warmWhite, 3000);
    } else {
      Serial.printf("Led strip %d is null\n", i);
    }
  }
}

// Boot procedure - phase two (after WiFi connection)
void startBootWol() {
  Serial.println("Start boot WOL");
  sendMagicPacket(bootWolMac.c_str());
}

void initBootEvents() {
  if (bootFadeIn) {
    startBootFadeIn();
  }
}

void initPins() {
  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    pinMode(DVC_DATA_PIN_LIST[i], OUTPUT);
    digitalWrite(DVC_DATA_PIN_LIST[i], LOW);
  } 
}

void initOnBoardLed() {
  #ifdef ONBOARD_LED_PIN
    Serial.printf("Onboard LED pin defined: %d\n", ONBOARD_LED_PIN);
    pinMode(ONBOARD_LED_PIN, OUTPUT);
    digitalWrite(ONBOARD_LED_PIN, LOW);
  #endif
}

void setup() {
  Serial.begin(115200);

  initConf();
  initPins();
  initLeds();
  initBootEvents();
  initSpiffs();
  initWifi();
  initHttpServer();
  initWebSockets();
  initIr();
  initOnBoardLed();

  #ifdef ESP32S3
    // initDmx();
  #endif

  if (bootWol) {
    startBootWol();
  }

  updateActivity();
}

void loop() {

  if (webSockEnabled) {
    webSocket.loop();
  }

  if (dmxEnabled) {
    #ifdef ESP32S3
      // processDmx();
    #endif
  }

  checkActivityTimout();

  processIr();

  printHearthbeat();

  reconnectWifi();

  delay(10);
}
