// https://wiki.seeedstudio.com/xiao_esp32s3_getting_started/
// https://www.freertos.org/a00125.html
// https://fastled.io/docs/

#include <SPIFFS.h>
#include <WiFi.h>
#include "controller_ir.h"
#include "dmx.h"
#include "http_service.h"
#include "led.h"
#include "main.h"
#include "network_service.h"
#include "nvm.h"
#include "runtime_service.h"
#include "websocket_service.h"

String taskCommand = "";

void initSpiffs() {
  Serial.println("SPIFFS initialization");

  if (!SPIFFS.begin(true)) {
    Serial.println("An Error has occurred while mounting SPIFFS");
    return;
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
    pollWebSocket();
  }

  if (dmxEnabled) {
    #ifdef ESP32S3
      // processDmx();
    #endif
  }

  checkActivityTimeout();
  processIr();
  printHeartbeat();
  reconnectWifi();

  delay(10);
}
