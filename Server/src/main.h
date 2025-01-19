#ifndef MAIN_H
#define MAIN_H

#include <Arduino.h>
#include <Preferences.h>
#include "devices.h"
#include <FastLED.h>
#include <led.h>

enum DeviceWifiModeType {
    DEVICE_WIFI_MODE_AUTO = 0,
    DEVICE_WIFI_MODE_STA = 1,
    DEVICE_WIFI_MODE_AP = 2,
    DEVICE_WIFI_MODE_AP_STA = 3
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

extern bool webSockEnabled;
extern bool dmxEnabled;
extern bool dmxUnicast;
extern uint16_t dmxUniverse;
extern uint16_t dmxUniverseCount;
extern String deviceName;

float normalize(float x, float min, float max);
void terminateCurrTask();

#endif