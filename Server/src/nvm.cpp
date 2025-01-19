#include <Arduino.h>
#include "main.h"
#include "dmx.h"
#include "nvm.h"
#include "credentials.h"

void initConf() {
  preferences.begin(NVM_NAMESPACE, false);

  ledCount = preferences.getUInt(NVM_LED_COUNT, DVC_NUM_LEDS);

  wifiMode = static_cast<DeviceWifiModeType>(preferences.getUInt(NVM_WIFI_MODE));
  wifiSsid = preferences.getString(NVM_WIFI_SSID, WIFI_SSID);
  wifiPassword = preferences.getString(NVM_WIFI_PASSWORD, WIFI_PASSWORD);
  wifiApSsid = preferences.getString(NVM_WIFI_AP_SSID, wifiApSsid);
  wifiApPassword = preferences.getString(NVM_WIFI_AP_PASSWORD, wifiApPassword);

  bootFadeIn = preferences.getBool(NVM_BOOT_FADE_IN, bootFadeIn);
  bootColor = preferences.getString(NVM_BOOT_COLOR, bootColor);
  bootWol = preferences.getBool(NVM_BOOT_WOL, bootWol);
  bootWolMac = preferences.getString(NVM_BOOT_WOL_MAC, bootWolMac);

  webSockEnabled = preferences.getBool(NVM_WEB_SOCK_ENABLED, true);

  dmxEnabled = preferences.getBool(NVM_DMX_ENABLED, true);
  dmxUnicast = preferences.getBool(NVM_DMX_UNICAST, true);
  dmxUniverse = preferences.getUInt(NVM_DMX_UNIVERSE, DVC_DMX_UNIVERSE);
  dmxUniverseCount = preferences.getUInt(NVM_DMX_UNIVERSE_COUNT, DMX_UNIVERSE_COUNT);

  deviceName = preferences.getString(NVM_DEVICE_NAME, deviceName);

  preferences.end();
}

void handleGetConf(AsyncWebServerRequest *request) {
  // Returns current configuration (not necessarily from NVM)
  JsonDocument jsonDoc;

  jsonDoc[NVM_LED_COUNT] = ledCount;

  jsonDoc[NVM_WIFI_MODE] = static_cast<int>(wifiMode);
  jsonDoc[NVM_WIFI_SSID] = wifiSsid;
  jsonDoc[NVM_WIFI_PASSWORD] = wifiPassword;
  jsonDoc[NVM_WIFI_AP_SSID] = wifiApSsid;
  jsonDoc[NVM_WIFI_AP_PASSWORD] = wifiApPassword;

  jsonDoc[NVM_BOOT_FADE_IN] = bootFadeIn;
  jsonDoc[NVM_BOOT_COLOR] = bootColor;
  jsonDoc[NVM_BOOT_WOL] = bootWol;
  jsonDoc[NVM_BOOT_WOL_MAC] = bootWolMac;

  jsonDoc[NVM_DEVICE_NAME] = deviceName;
  jsonDoc[NVM_WEB_SOCK_ENABLED] = webSockEnabled;
  jsonDoc[NVM_DMX_ENABLED] = dmxEnabled;
  jsonDoc[NVM_DMX_UNICAST] = dmxUnicast;
  jsonDoc[NVM_DMX_UNIVERSE] = dmxUniverse;
  jsonDoc[NVM_DMX_UNIVERSE_COUNT] = dmxUniverseCount;

  String jsonData;
  serializeJson(jsonDoc, jsonData);

  Serial.println("handleGetConf - jsonData: " + String(jsonData));

  request->send(200, "application/json", jsonData);
}

void handleSetConf(AsyncWebServerRequest *request, uint8_t *data, size_t len, size_t index, size_t total) {
  // Serial.println("handleSetConf");
  JsonDocument jsonDoc;
  DeserializationError error = deserializeJson(jsonDoc, (const char*)data);

  if (error) {
    const char* msg = "Deserialization of configuration failed";
    Serial.print(F(msg));
    Serial.println(error.f_str());
    request->send(400, "text/html", msg);
    return;
  }

  const int paramLedCount = jsonDoc[NVM_LED_COUNT].as<uint>();
  const int paramWifiMode = jsonDoc[NVM_WIFI_MODE].as<uint>();
  const char* paramWifiSsid = jsonDoc[NVM_WIFI_SSID].as<const char*>();
  const char* paramWifiPassword = jsonDoc[NVM_WIFI_PASSWORD].as<const char*>();
  const char* paramWifiApSsid = jsonDoc[NVM_WIFI_AP_SSID].as<const char*>();
  const char* paramWifiApPassword = jsonDoc[NVM_WIFI_AP_PASSWORD].as<const char*>();

  const bool paramBootFadeIn = jsonDoc[NVM_BOOT_FADE_IN].as<bool>();
  const char* paramBootColor = jsonDoc[NVM_BOOT_COLOR].as<const char*>();
  const bool paramBootWol = jsonDoc[NVM_BOOT_WOL].as<bool>();
  const char* paramBootWolMac = jsonDoc[NVM_BOOT_WOL_MAC].as<const char*>();

  const bool paramWebSockEnabled = jsonDoc[NVM_WEB_SOCK_ENABLED].as<bool>();
  
  const bool paramDmxEnabled = jsonDoc[NVM_DMX_ENABLED].as<bool>();
  const bool paramDmxUnicast = jsonDoc[NVM_DMX_UNICAST].as<bool>();
  const int paramDmxUniverse = jsonDoc[NVM_DMX_UNIVERSE].as<uint>();
  const int paramDmxUniverseCount = jsonDoc[NVM_DMX_UNIVERSE_COUNT].as<uint>();
  
  const char* paramDeviceName = jsonDoc[NVM_DEVICE_NAME].as<const char*>();

  preferences.begin(NVM_NAMESPACE, false);
  preferences.putUInt(NVM_LED_COUNT, paramLedCount);

  preferences.putUInt(NVM_WIFI_MODE, static_cast<int>(paramWifiMode));
  preferences.putString(NVM_WIFI_SSID, paramWifiSsid);
  preferences.putString(NVM_WIFI_AP_SSID, paramWifiApSsid);
  preferences.putString(NVM_WIFI_AP_PASSWORD, paramWifiApPassword);
  preferences.putString(NVM_WIFI_PASSWORD, paramWifiPassword);

  preferences.putBool(NVM_BOOT_FADE_IN, paramBootFadeIn);
  preferences.putString(NVM_BOOT_COLOR, paramBootColor);
  preferences.putBool(NVM_BOOT_WOL, paramBootWol);
  preferences.putString(NVM_BOOT_WOL_MAC, paramBootWolMac);
  
  preferences.putBool(NVM_WEB_SOCK_ENABLED, paramWebSockEnabled);
  
  preferences.putBool(NVM_DMX_ENABLED, paramDmxEnabled);
  preferences.putBool(NVM_DMX_UNICAST, paramDmxUnicast);
  preferences.putUInt(NVM_DMX_UNIVERSE, paramDmxUniverse);
  preferences.putUInt(NVM_DMX_UNIVERSE_COUNT, paramDmxUniverseCount);
  preferences.putString(NVM_DEVICE_NAME, paramDeviceName);

  preferences.end();

  request->send(200, "text/html", "Configuration saved. Device is rebooting. Hit ok and wait 10 seconds for refresh.");

  delay(2000);
  ESP.restart();
}