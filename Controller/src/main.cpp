#include <Arduino.h>
#include <esp_sleep.h>

#include <WiFi.h>
#include <HTTPClient.h>
#include <WebSocketsClient.h>

#include "credentials.h"

#define ENC_A D2
#define ENC_B D3
#define ENC_BTN D1

RTC_DATA_ATTR uint bootCount = 0;
RTC_DATA_ATTR uint hue;
RTC_DATA_ATTR uint saturation;
RTC_DATA_ATTR uint value;

// String targetHost = "198.168.4.1";
String targetHost = "10.0.1.50";
// String targetHost = "10.0.1.200";
// String targetHost = "10.0.1.202";

int counter = 0;
int currentStateCLK;
int lastStateCLK;
String currentDir = "";
unsigned long btnPressLast = 0;
unsigned long btnHoldStart = 0;
unsigned long btnHoldLast = 0;

unsigned long lastActivity = 0;

const char *ssid = WIFI_SSID;
const char *password = WIFI_PASSWORD;
int wifiConnectionTimeout = 30;

bool lightOn = false;

enum EncoderAction {
    ENC_NONE = 0,
    ENC_PRESS = 1,
    ENC_HOLD = 2,
    ENC_CW = 3,
    ENC_CCW = 4
};

typedef void (*CallbackAction)();

unsigned long sleepTimeout = 600000;

WebSocketsClient webSocket;
bool useWebSockets = true;

void initEncoder() {
    pinMode(ENC_A, INPUT_PULLUP);
    pinMode(ENC_B, INPUT_PULLUP);
    pinMode(ENC_BTN, INPUT_PULLUP);

    lastStateCLK = digitalRead(ENC_B);
}

void goSleep() {
    //Go to sleep now
    Serial.println("Going to sleep now...");
    esp_deep_sleep_start();
}

void printWakeupReason() {
    esp_sleep_wakeup_cause_t wakeup_reason;

    wakeup_reason = esp_sleep_get_wakeup_cause();
    if (esp_sleep_get_wakeup_cause() == ESP_SLEEP_WAKEUP_GPIO) {
        Serial.println("Rise and shine Mr. Freeman. Wake up and smell the ashes...");
    }
}

void wifiConnect() {
    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, password);
    Serial.print("Connecting to WiFi ..");
    unsigned long startAttemptTime = millis();
    while (WiFi.status() != WL_CONNECTED && millis() - startAttemptTime < wifiConnectionTimeout * 1000) {
        Serial.print('.');
        delay(1000);
    }

    if (WiFi.status() != WL_CONNECTED) {
        Serial.println("\nFailed to connect to WiFi, going to sleep...");
        goSleep();
    }
    Serial.print('.');
    delay(1000);

    Serial.println();
    Serial.println(WiFi.localIP());
}

void wsLightUpdate(String urlPath) {
    String message = "{\"command\": \"" + urlPath + "\"}";
    webSocket.sendTXT(message);
}


void httpGetRequest(String urlPath) {
  String url = "http://" + targetHost + "/" + urlPath;
  HTTPClient http;
  http.begin(url.c_str());
  int httpResponseCode = http.GET();
  if (httpResponseCode >= 200 && httpResponseCode <= 299) {
    Serial.print("HTTP Response code: ");
    Serial.println(httpResponseCode);
    String payload = http.getString();
    Serial.println(payload);
  } else {
    Serial.print("Error code: ");
    Serial.println(httpResponseCode);
  }
  http.end();
}

void lightUpdate(String urlPath) {
    if (useWebSockets) {
        wsLightUpdate(urlPath);
    } else {
        httpGetRequest(urlPath);
    }
}

void sendSignal(String signal) {
    String signalUrlPath = "ctrl?signal=" + signal;
    if (useWebSockets) {
        webSocket.sendTXT(signalUrlPath);
    } else {
        httpGetRequest(signalUrlPath);
    }
}

void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
    switch(type) {
        case WStype_DISCONNECTED:
            Serial.println("WebSocket Disconnected");
            break;
        case WStype_CONNECTED:
            Serial.println("WebSocket Connected");
            break;
        case WStype_TEXT:
            Serial.printf("WebSocket Message: %s\n", payload);
            break;
        case WStype_BIN:
            Serial.println("WebSocket Binary Message");
            break;
    }
}

void webSocketSetup() {
    webSocket.begin(targetHost, 81, "/");
    webSocket.onEvent(webSocketEvent);
    // webSocket.setAuthorization("user", "pwd");
    webSocket.setReconnectInterval(1000);
}

void lightOff() {
    lightUpdate("update?command=off");

    lightOn = false;
}

void lightOnWarmWhite() {
    lightUpdate("update?command=solid-color&color=%238a6438");

    lightOn = true;
}

void lightOnSoftPink() {
  lightUpdate("update?command=solid-color&color=%231c0a03");

  lightOn = true;
}

void lightOnHsv(int h, int s, int v) {
    lightUpdate("update?command=solid-color&hue=" + String(h) + "&saturation=" + String(s) + "&value=" + String(v));
}

void lightOnHsvFadeIn(int h, int s, int v) {
    lightUpdate("update?command=fade-in&hue=" + String(h) + "&saturation=" + String(s) + "&value=" + String(v));
}

void lightOnHsvFadeOut(int h, int s, int v) {
    lightUpdate("update?command=fade-out&hue=" + String(h) + "&saturation=" + String(s) + "&value=" + String(v));
}

void lightSwitch() {
    if (lightOn) {
        lightOff();
    } else {
        lightOnSoftPink();
    }
}

void lightOffAndSleep() {
  lightOff();
  goSleep();
}

void updateActivity() {
    lastActivity = millis();
}

EncoderAction processEncoder() {
    EncoderAction result = ENC_NONE;

    // Read the current state of ENC_B
    currentStateCLK = digitalRead(ENC_B);

    // If last and current state of ENC_B are different, then pulse occurred
    // React to only 1 state change to avoid double count
    if (currentStateCLK != lastStateCLK && currentStateCLK == 1) {
        // If the ENC_A state is different than the ENC_B state then
        // the encoder is rotating CCW so decrement
        if (digitalRead(ENC_A) != currentStateCLK) {
            counter--;
            currentDir = "CCW";
            result = ENC_CCW;
        } else {
            // Encoder is rotating CW so increment
            counter++;
            currentDir = "CW";
            result = ENC_CW;
        }

        Serial.print("Direction: ");
        Serial.print(currentDir);
        Serial.print(" | Counter: ");
        Serial.println(counter);

        updateActivity();
    }

    // Remember last ENC_B state
    lastStateCLK = currentStateCLK;

    // Read the button state
    int btnState = digitalRead(ENC_BTN);

    // If we detect LOW signal, button is pressed
    if (btnState == LOW) {
        // if 50ms have passed since last LOW pulse, it means that the
        // button has been pressed, released and pressed again
        if (millis() - btnPressLast > 50)
        {
            Serial.println("Button pressed!");
            result = ENC_PRESS;
            btnHoldStart = millis();
            updateActivity();
        }

        if (millis() - btnHoldStart > 1000) {
             if (millis() - btnHoldLast > 50) {
                Serial.println("Button hold!");
                result = ENC_HOLD;
            }
            btnHoldLast = millis(); 

            updateActivity();
        }
        // Remember last button press event
        btnPressLast = millis();
    }

    return result;
}

void setup() {
    Serial.begin(115200);
    
    ++bootCount;

    // delay(10000);
    Serial.println("Boot number: " + String(bootCount));

    printWakeupReason();
    esp_deep_sleep_enable_gpio_wakeup(BIT(ENC_BTN), ESP_GPIO_WAKEUP_GPIO_LOW);

    initEncoder();

    wifiConnect();

    webSocketSetup();

    // lightOnSoftPink();
}

void loop() {
    // https://wiki.dfrobot.com/EC11_Rotary_Encoder_Module_SKU__SEN0235

    EncoderAction encAction = processEncoder();

    switch (encAction) {
    case ENC_PRESS:
        Serial.println("ENC_PRESS");
        sendSignal("press");
        break;

    case ENC_HOLD:
        Serial.println("ENC_HOLD");
        sendSignal("hold");
        break;

    case ENC_CW:
        Serial.println("ENC_CW");
        sendSignal("cw");
        break;

    case ENC_CCW:
        Serial.println("ENC_CCW");
        sendSignal("ccw");
        break;
    }


    if (lastActivity != 0 && millis() - lastActivity > sleepTimeout) {
        lightOffAndSleep();
    }

    webSocket.loop();

    // Put in a slight delay to help debounce the reading
    delay(1);
}