// https://wiki.seeedstudio.com/xiao_esp32s3_getting_started/
// https://www.freertos.org/a00125.html
// https://fastled.io/docs/

// #include <Arduino.h>
#include <WiFi.h>
// #include <FastLED.h>
#include <ESPAsyncWebServer.h>
#include <WebSocketsServer.h>
// #include <Preferences.h>
#include <ArduinoJson.h>
#include <HTTPClient.h>
#include "SPIFFS.h"

#include "main.h"
// #include "devices.h"
#include "credentials.h"
#include "nvm.h"
#include "modes.h"
#include "dmx.h"
// #include "controller.h"

Preferences preferences;

// uint16_t ledCount = 0;
// CRGB* leds = nullptr;

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
TaskHandle_t taskHandle;
const uint32_t StackSize = 2048; // What is the good value for my operations?

static uint8_t hue = 0;

bool bootFadeIn = false;
String bootColor = "#FFFAFA";
bool bootWol = false;
String bootWolMac = "00:00:00:00:00:00";

bool webSockEnabled;

unsigned long lastActivity = 0;
unsigned long activityTimeout = 0;
bool activityTimeoutEnabled = false;
int fadeOutDuration = 10000;

void updateActivity() {
    lastActivity = millis();
    Serial.printf("Updating last activity (%d)", lastActivity);
}

/*
void initLeds() {
  leds = new CRGB[ledCount];

  FastLED.addLeds<LED_TYPE, DATA_PIN, LED_COLOR_ORDER>(leds, ledCount);
}

void circularShift() {
  CRGB tempArray[ledCount];

  for (int i = 0; i < ledCount; i++) {
    tempArray[(i + OFFSET) % ledCount] = leds[i];
  }

  for (int i = 0; i < ledCount; i++) {
    leds[i] = tempArray[i];
  }
}

void FastLedShow() {
  if (OFFSET > 0) {
    circularShift();
  }

  FastLED.show();
}*/

void terminateCurrTask() {
  Serial.println("Terminating current task");
  if (taskHandle != NULL) {
    TaskHandle_t xCurrentTask = xTaskGetCurrentTaskHandle();
    if (xCurrentTask == taskHandle) {
      Serial.println("Delete task using handle...");
      vTaskDelete(taskHandle);
      taskHandle = NULL;
    } else {
      Serial.println("Delete task using flag...");
      terminateTaskFlag = true;
      delay(50);
      terminateTaskFlag = false;
    }
  }
}

int mapRange(int value, int fromLow, int fromHigh, int toLow, int toHigh) {
    return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
}

float normalize(float x, float min, float max) {
  return (x - min) / (max - min);
}

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

void processCommand(String message) {
  Serial.println("WebSockets message: " + message);
  String cmd = getQueryParameterValue(message, "command");
  Serial.println("WebSockets command: " + cmd);
  if (cmd.equals("off")) {
    terminateCurrTask();
    fill_solid(leds, ledCount, CRGB::Black);
    FastLedShow();
  } else if (cmd.equals("solid-color")) {
    terminateCurrTask();
    CRGB color = htmlColor2Crgb(getQueryParameterValue(message, "color"));
    fill_solid(leds, ledCount, color);
    FastLedShow();
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
  fill_solid(leds, ledCount, CRGB::Black);

  int sectionLength = ledCount / sectionCount;
  int sectionToLight = sectionIdx;

  int startIndex = sectionToLight * sectionLength;
  int endIndex = startIndex + sectionLength;

  for (int i = startIndex; i < endIndex; i++) {
    leds[i] = color;
  }
}

void update(AsyncWebServerRequest *request) {
  if (request->hasParam("command", false, false)) {
    AsyncWebParameter* command = request->getParam("command", false, false);

    String cmd = command->value();
    
    // Serial.println("Received command: " + cmd);
    
    if (cmd.length() > 0) {
      updateActivity();
      if (request->hasParam("activity-timeout")) {
        Serial.printf("Setting activity timeout");
        activityTimeout = request->getParam("activity-timeout", false, false)->value().toInt();
        activityTimeoutEnabled = true;

        if (request->hasParam("fadeout-duration")) {
          fadeOutDuration = request->getParam("fadeout-duration", false, false)->value().toInt();
        }
      }

      // Static effects are directly here, for dynamic effects (animations) 
      // is started new thread using a task
      if (cmd.equals("off")) {
        terminateCurrTask();
        fill_solid(leds, ledCount, CRGB::Black);      
        FastLedShow();
      } else if (cmd.equals("running-rainbow")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
        params->ledCount = ledCount;
        params->leds = leds;
        params->delay = request->getParam("delay", false, false)->value().toInt();
        params->step = request->getParam("step", false, false)->value().toInt();
        params->delta = request->getParam("delta", false, false)->value().toInt();
        
        xTaskCreate(taskRunningRainbow, "CurrentLedTask", StackSize, params, 10, &taskHandle);
      } else if (cmd.equals("strobe")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskStrobeParams *params = new TaskStrobeParams;
        // TaskStrobeParams params;
        params->ledCount = ledCount;
        params->leds = leds;
        params->delay1 = request->getParam("delay1", false, false)->value().toInt();
        params->delay2 = request->getParam("delay2", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());

        Serial.println("Delay1: " + String(params->delay1));
        Serial.println("Delay2: " + String(params->delay2));
        
        xTaskCreate(taskStrobe, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("strobe-random")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskStrobeParams *params = new TaskStrobeParams;
        params->ledCount = ledCount;
        params->leds = leds;
        // params.delay1 = request->getParam("delay1", false, false)->value().toInt();
        // params.delay2 = request->getParam("delay2", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());
        
        xTaskCreate(taskStrobe, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("solid-color")) {
        if (request->hasParam("color")) {
          // RGB
          terminateCurrTask();
          CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());

          if (request->hasParam("section-count") && request->hasParam("section-index")) {
            int sectionCount = request->getParam("section-count", false, false)->value().toInt();
            int sectionIdx = request->getParam("section-index", false, false)->value().toInt();
            updateSection(sectionCount, sectionIdx, color);
          } else {
            fill_solid(leds, ledCount, color);
          }

          FastLedShow();
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
            fill_solid(leds, ledCount, color);
          }
        } else {
          request->send(200, "text/plain", "Command solid-color: Missing required parameters.");
          return;
        }
      } else if (cmd.equals("fade-in")) {
        if (request->hasParam("color") && request->hasParam("duration")) {
          // RGB
          terminateCurrTask();

          TaskFadeParams *params = new TaskFadeParams;
          params->ledCount = ledCount;
          params->leds = leds;
          params->duration = request->getParam("duration", false, false)->value().toInt();
          params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());

          xTaskCreate(taskFadeIn, "CurrentLedTask", StackSize, params, 10, &taskHandle);
        } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value") && request->hasParam("duration")) {
          // HSV
          terminateCurrTask();

          int hue = request->getParam("hue", false, false)->value().toInt();
          int saturation = request->getParam("saturation", false, false)->value().toInt();
          int value = request->getParam("value", false, false)->value().toInt();

          TaskFadeParams *params = new TaskFadeParams;
          params->ledCount = ledCount;
          params->leds = leds;
          params->duration = request->getParam("duration", false, false)->value().toInt();
          params->color = CHSV(hue, saturation, value);

          xTaskCreate(taskFadeIn, "CurrentLedTask", StackSize, params, 10, &taskHandle);
        } else {
          request->send(200, "text/plain", "Command fade-in: Missing required parameters.");
          return;
        }
      } else if (cmd.equals("fade-out")) {
        if (request->hasParam("color") && request->hasParam("duration")) {
          // RGB
          terminateCurrTask();

          TaskFadeParams *params = new TaskFadeParams;
          params->ledCount = ledCount;
          params->leds = leds;
          params->duration = request->getParam("duration", false, false)->value().toInt();
          params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());

          xTaskCreate(taskFadeOut, "CurrentLedTask", StackSize, params, 10, &taskHandle);
        } else if (request->hasParam("hue") && request->hasParam("saturation") && request->hasParam("value") && request->hasParam("duration")) {
          // HSV
          terminateCurrTask();

          int hue = request->getParam("hue", false, false)->value().toInt();
          int saturation = request->getParam("saturation", false, false)->value().toInt();
          int value = request->getParam("value", false, false)->value().toInt();

          TaskFadeParams *params = new TaskFadeParams;
          params->ledCount = ledCount;
          params->leds = leds;
          params->duration = request->getParam("duration", false, false)->value().toInt();
          params->color = CHSV(hue, saturation, value);

          xTaskCreate(taskFadeOut, "CurrentLedTask", StackSize, params, 10, &taskHandle);
        } else {
          request->send(200, "text/plain", "Command fade-out: Missing required parameters.");
          return;
        }
      } else if (cmd.equals("gradient")) {
        terminateCurrTask();

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

        fill_gradient_HSV(leds, ledCount, chsvStart, chsvEnd, FORWARD_HUES );
        FastLedShow();
      } else if (cmd.equals("noise")) {
        // Not yet implemented
      } else if (cmd.equals("abort")) {
        vTaskDelete(taskHandle);
      } else {
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
  // server.on("/ctrl", HTTP_GET, handleCtrlCmd);

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

  processCommand(message);

  webSocket.sendTXT(num, message);
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t *payload, size_t length) {
  if (type == WStype_CONNECTED) {
    IPAddress ip = webSocket.remoteIP(num);
    Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);
  }

  if (type == WStype_BIN) {
    if (length == ledCount * 3) {
      for (int i = 0; i < ledCount; i++) {
        leds[i] = CRGB(payload[i * 3], payload[i * 3 + 1], payload[i * 3 + 2]);
      }
      FastLedShow();
    } else {
      Serial.println("Incorrect data length");
    }
  }

  if (type == WStype_TEXT) {
    processWsTxtPayload(num, payload, length);
  }

  if (type == WStype_DISCONNECTED) {
    fill_solid(leds, ledCount, CRGB(0, 0, 0));
    FastLedShow();
  }
}

void initWifi() {
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

  uint8_t tryCount = 0;
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED && tryCount < wifiTimeout) {
    tryCount++;
    Serial.print('.');
    delay(1000);
  }
  Serial.println();

  IPAddress ipAddress;
  if (WiFi.status() == WL_CONNECTED) {
    ipAddress = WiFi.localIP();
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
  fill_solid(leds, ledCount, CRGB::White);
  FastLedShow();
}

void fadeIn(CRGB color, int duration) {
  terminateCurrTask();

  TaskFadeParams *params = new TaskFadeParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  params->color = color;

  xTaskCreate(taskFadeIn, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

void fadeOut(int duration) {
  terminateCurrTask();

  TaskFadeParams *params = new TaskFadeParams;
  params->ledCount = ledCount;
  params->leds = leds;
  params->duration = duration;
  // params->color = color;

  xTaskCreate(taskFadeOut, "CurrentLedTask", StackSize, params, 10, &taskHandle);
}

uint8_t getLedMaxBrightness() {
  uint8_t maxBrightness = 0;
  for (int i = 0; i < ledCount; i++) {
    maxBrightness = max(maxBrightness, max(leds[i].r, max(leds[i].g, leds[i].b)));
  }

  return maxBrightness;
}

/*void setLedColorRgb(int idx, int r, int g, int b) {
  leds[i] = CRGB(r, g, b);
}*/

/*
void lightFadeOut(int duration) {
  Serial.println(F("Fading out"));
  uint8_t maxBrightness = getLedMaxBrightness();
  int steps = maxBrightness;
  int delayTime = duration / steps;

  for (int brightness = maxBrightness; brightness > 0; brightness--) {
    for (int i = 0; i < ledCount; i++) {
      leds[i].nscale8_video(brightness);
    }
    FastLedShow();
    delay(delayTime);
  }
  Serial.println(F("Fade out loop ended, setting all leds to black"));
  fill_solid(leds, ledCount, CRGB::Black);
  FastLedShow();
  Serial.println(F("Fading out done"));
}

void lightFadeIn(CRGB color, int duration) {
  Serial.println(F("Fading in"));
  int steps = 255;
  int delayTime = duration / steps;

  for (int brightness = 0; brightness <= 255; brightness++) {
    for (int i = 0; i < ledCount; i++) {
      leds[i] = color;
      leds[i].nscale8_video(brightness);
    }
    FastLedShow();
    delay(delayTime);
  }
  Serial.println(F("Fading in done"));
}*/

void checkActivityTimout() {
  Serial.printf("Last activity timestamp: %d\n", lastActivity);
  Serial.printf("Current timestamp: %d\n", millis());
  uint diff = millis() - lastActivity;
  Serial.printf("Time diff: %d\n", diff);
  Serial.printf("activityTimeout: %d\n", activityTimeout);
  Serial.printf("activityTimerTimeoutEnable: %s\n", activityTimeoutEnabled ? "true" : "false");

  if (activityTimeoutEnabled && millis() - lastActivity > activityTimeout) {
    activityTimeoutEnabled = false;
    fadeOut(fadeOutDuration);
  }
  Serial.println(F("Activity timeout check done"));
}

// Boot procedure - phase one (before WiFi connection)
void startBootFadeIn() {
  Serial.println("Starting boot fade in");
  CRGB warmWhite = htmlColor2Crgb(bootColor);
  fadeIn(warmWhite, 3000);
}

// Boot procedure - phase two (after WiFi connection)
void startBootWol() {
  Serial.println("Start boot WOL");
  sendMagicPacket(bootWolMac.c_str());
}

void setup() {
  Serial.begin(115200);

  if (bootFadeIn) {
    startBootFadeIn();
  }

  pinMode(DVC_DATA_PIN, OUTPUT);

  initConf();
  initLeds();
  initSpiffs();
  initWifi();
  initHttpServer();
  initWebSockets();
  #ifdef ESP32S3
    // initDmx();
  #endif

  if (bootWol) {
    startBootWol();
  }
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

  // delay(200);
}