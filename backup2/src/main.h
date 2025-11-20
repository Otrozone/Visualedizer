#ifndef MAIN_H
#define MAIN_H

#include <Arduino.h>
#include <Preferences.h>
#include <FastLED.h>
#include <ESPAsyncWebServer.h>

#include "led.h"
#include "common.h"
#include "devices.h"

enum DeviceWifiModeType {
    DEVICE_WIFI_MODE_AUTO = 0,
    DEVICE_WIFI_MODE_STA = 1,
    DEVICE_WIFI_MODE_AP = 2,
    DEVICE_WIFI_MODE_AP_STA = 3
};

// For a mapping between command strings and handler functions
typedef void (*CommandHandler)(AsyncWebServerRequest*);

struct CommandEntry {
  const char* cmd;
  CommandHandler handler;
};

extern Preferences preferences;
extern DeviceWifiModeType wifiMode;
extern String wifiSsid;
extern String wifiPassword;
extern String wifiApSsid;
extern String wifiApPassword;

extern bool bootFadeIn;
extern String bootColor;
extern bool bootWol;
extern String bootWolMac;

extern bool activityTimeoutEnabled;
extern unsigned long activityTimeout;

extern bool webSockEnabled;
extern bool dmxEnabled;
extern bool dmxUnicast;
extern uint16_t dmxUniverse;
extern uint16_t dmxUniverseCount;
extern String deviceName;

extern bool irEnabled;

float normalize(float x, float min, float max);

#endif