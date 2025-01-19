#ifndef NVM_H
#define NVM_H

#include <Arduino.h>
//#include <Preferences.h>
#include <ESPAsyncWebServer.h>
#include <ArduinoJson.h>
//#include "device_wifi_mode.h"

#define NVM_NAMESPACE "visualedizer"
#define NVM_LED_COUNT "ledCount"
#define NVM_WIFI_MODE "wifiMode"
#define NVM_WIFI_SSID "wifiSsid"
#define NVM_WIFI_PASSWORD "wifiPassword"
#define NVM_WIFI_AP_SSID "wifiApSsid"
#define NVM_WIFI_AP_PASSWORD "wifiApPassword"
#define NVM_DMX_ENABLED "dmxEnabled"
#define NVM_DMX_UNICAST "dmxUnicast"
#define NVM_DMX_UNIVERSE "dmxUniverse"
#define NVM_DMX_UNIVERSE_COUNT "dmxUniCount"
#define NVM_WEB_SOCK_ENABLED "webSockEnabled"
#define NVM_DEVICE_NAME "deviceName"
#define NVM_BOOT_FADE_IN "bootFadeIn"
#define NVM_BOOT_COLOR "bootColor"
#define NVM_BOOT_WOL "bootWol"
#define NVM_BOOT_WOL_MAC "bootWolMac"

void initConf();
void handleGetConf(AsyncWebServerRequest *request);
void handleSetConf(AsyncWebServerRequest *request, uint8_t *data, size_t len, size_t index, size_t total);

#endif // NVM_H
/*
Preferences preferences;
uint16_t ledCount = 0;
String wifiSsid = WIFI_SSID;
String wifiPassword = WIFI_PASSWORD;
DeviceWifiModeType wifiMode = DEVICE_WIFI_MODE_AUTO;
String wifiApSsid = "Visualedizer";
String wifiApPassword = "Rezidelausiv1337";
bool webSockEnabled = true;
bool dmxEnabled = true;
bool dmxUnicast = true;
uint16_t dmxUniverse = DMX_UNIVERSE;
uint16_t dmxUniverseCount = DMX_UNIVERSE_COUNT;
String deviceName = "Unnamed";
*/