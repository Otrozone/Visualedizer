#ifndef MAIN_H
#define MAIN_H

#include <Arduino.h>
#include <ESPAsyncWebServer.h>
#include <FastLED.h>
#include <Preferences.h>

#include "led.h"
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

extern unsigned long lastActivity;
extern bool activityTimeoutEnabled;
extern unsigned long activityTimeout;
extern bool activityTimeoutRequested;
extern unsigned long tmpActivityTimeout;
extern int fadeOutDuration;

extern bool webUiUseWebSockets;
extern bool turnOffOnLeave;
extern bool dmxEnabled;
extern bool dmxUnicast;
extern uint16_t dmxUniverse;
extern uint16_t dmxUniverseCount;
extern bool laserEnabled;
extern int laserTxPin;
extern int laserRxPin;
extern int laserEnablePin;
extern bool strobeEnabled;
extern int strobePin;
extern String deviceName;

extern bool irEnabled;

float normalize(float x, float min, float max);

int getWifiSignalStrength();

#endif
