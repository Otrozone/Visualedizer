#include <Arduino.h>
#include <algorithm>

#include "main.h"
#include "dmx.h"
#include "controller_ir.h"
#include "nvm.h"
#include "credentials.h"

namespace {
String getStripLedCountKey(int stripIdx) {
  return "strip" + String(stripIdx) + "LedCount";
}

uint16_t getStoredStripLedCount(int stripIdx) {
  String key = getStripLedCountKey(stripIdx);
  return preferences.getUInt(key.c_str(), DVC_NUM_LEDS_LIST[stripIdx]);
}
}

void initConf() {
  preferences.begin(NVM_NAMESPACE, false);

  wifiMode = static_cast<DeviceWifiModeType>(preferences.getUInt(NVM_WIFI_MODE));
  wifiSsid = preferences.getString(NVM_WIFI_SSID, WIFI_SSID);
  wifiPassword = preferences.getString(NVM_WIFI_PASSWORD, WIFI_PASSWORD);
  wifiApSsid = preferences.getString(NVM_WIFI_AP_SSID, wifiApSsid);
  wifiApPassword = preferences.getString(NVM_WIFI_AP_PASSWORD, wifiApPassword);

  bootFadeIn = preferences.getBool(NVM_BOOT_FADE_IN, bootFadeIn);
  bootColor = preferences.getString(NVM_BOOT_COLOR, bootColor);
  bootWol = preferences.getBool(NVM_BOOT_WOL, bootWol);
  bootWolMac = preferences.getString(NVM_BOOT_WOL_MAC, bootWolMac);

  activityTimeoutEnabled = preferences.getBool(NVM_ACTIVITY_TIMEOUT_ENABLED, activityTimeoutEnabled);
  activityTimeout = preferences.getUInt(NVM_ACTIVITY_TIMEOUT, activityTimeout);

  webUiUseWebSockets = preferences.getBool(
      NVM_WEB_UI_USE_WEB_SOCKETS,
      preferences.getBool(NVM_WEB_SOCK_ENABLED_LEGACY, true));
  turnOffOnLeave = preferences.getBool(NVM_TURN_OFF_ON_LEAVE, turnOffOnLeave);

  dmxEnabled = preferences.getBool(NVM_DMX_ENABLED, true);
  dmxUnicast = preferences.getBool(NVM_DMX_UNICAST, true);
  dmxUniverse = preferences.getUInt(NVM_DMX_UNIVERSE, DVC_DMX_UNIVERSE);
  dmxUniverseCount = preferences.getUInt(NVM_DMX_UNIVERSE_COUNT, DMX_UNIVERSE_COUNT);

  deviceName = preferences.getString(NVM_DEVICE_NAME, deviceName);

  irEnabled = preferences.getBool(NVM_IR_ENABLED, irEnabled);
  irUnrecognizedAsOnOff = preferences.getBool(NVM_IR_UNRECOGNIZED_AS_ONOFF, irUnrecognizedAsOnOff);

  preferences.end();
}

void handleGetConf(AsyncWebServerRequest *request) {
  // Returns current configuration (not necessarily from NVM)
  JsonDocument jsonDoc;

  JsonArray strips = jsonDoc[DEVICE_STRIPS].to<JsonArray>();
  preferences.begin(NVM_NAMESPACE, true);
  for (int i = 0; i < DVC_STRIP_COUNT; i++) {
    const int ledCount = getStoredStripLedCount(i);

    JsonObject strip = strips.add<JsonObject>();
    strip["index"] = i;
    strip[NVM_LED_COUNT] = ledCount;
    strip[NVM_DATA_PIN] = DVC_DATA_PIN_LIST[i];
  }
  preferences.end();

  jsonDoc[NVM_WIFI_MODE] = static_cast<int>(wifiMode);
  jsonDoc[NVM_WIFI_SSID] = wifiSsid;
  jsonDoc[NVM_WIFI_PASSWORD_CONFIGURED] = wifiPassword.length() > 0;
  jsonDoc[NVM_WIFI_AP_SSID] = wifiApSsid;
  jsonDoc[NVM_WIFI_AP_PASSWORD_CONFIGURED] = wifiApPassword.length() > 0;

  jsonDoc[NVM_BOOT_FADE_IN] = bootFadeIn;
  jsonDoc[NVM_BOOT_COLOR] = bootColor;
  jsonDoc[NVM_BOOT_WOL] = bootWol;
  jsonDoc[NVM_BOOT_WOL_MAC] = bootWolMac;

  jsonDoc[NVM_ACTIVITY_TIMEOUT_ENABLED] = activityTimeoutEnabled;
  jsonDoc[NVM_ACTIVITY_TIMEOUT] = activityTimeout;

  jsonDoc[NVM_DEVICE_NAME] = deviceName;
  jsonDoc[NVM_WEB_UI_USE_WEB_SOCKETS] = webUiUseWebSockets;
  jsonDoc[NVM_TURN_OFF_ON_LEAVE] = turnOffOnLeave;
  jsonDoc[NVM_DMX_ENABLED] = dmxEnabled;
  jsonDoc[NVM_DMX_UNICAST] = dmxUnicast;
  jsonDoc[NVM_DMX_UNIVERSE] = dmxUniverse;
  jsonDoc[NVM_DMX_UNIVERSE_COUNT] = dmxUniverseCount;

  jsonDoc[NVM_IR_ENABLED] = irEnabled;
  jsonDoc[NVM_IR_UNRECOGNIZED_AS_ONOFF] = irUnrecognizedAsOnOff;

  jsonDoc[WIFI_SIGNAL_STRENGTH] = getWifiSignalStrength();


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

  const int paramWifiMode = jsonDoc[NVM_WIFI_MODE].as<uint>();
  const char* paramWifiSsid = jsonDoc[NVM_WIFI_SSID].as<const char*>();
  const char* paramWifiApSsid = jsonDoc[NVM_WIFI_AP_SSID].as<const char*>();

  const bool paramBootFadeIn = jsonDoc[NVM_BOOT_FADE_IN].as<bool>();
  const char* paramBootColor = jsonDoc[NVM_BOOT_COLOR].as<const char*>();
  const bool paramBootWol = jsonDoc[NVM_BOOT_WOL].as<bool>();
  const char* paramBootWolMac = jsonDoc[NVM_BOOT_WOL_MAC].as<const char*>();

  const bool paramActivityTimeoutEnabled = jsonDoc[NVM_ACTIVITY_TIMEOUT_ENABLED].as<bool>();
  const int paramActivityTimeout = jsonDoc[NVM_ACTIVITY_TIMEOUT].as<uint>();

  const bool paramWebUiUseWebSockets = jsonDoc[NVM_WEB_UI_USE_WEB_SOCKETS].as<bool>();
  const bool paramTurnOffOnLeave = jsonDoc[NVM_TURN_OFF_ON_LEAVE].as<bool>();
  
  const bool paramDmxEnabled = jsonDoc[NVM_DMX_ENABLED].as<bool>();
  const bool paramDmxUnicast = jsonDoc[NVM_DMX_UNICAST].as<bool>();
  const int paramDmxUniverse = jsonDoc[NVM_DMX_UNIVERSE].as<uint>();
  const int paramDmxUniverseCount = jsonDoc[NVM_DMX_UNIVERSE_COUNT].as<uint>();
  
  const char* paramDeviceName = jsonDoc[NVM_DEVICE_NAME].as<const char*>();

  const bool paramIrEnabled = jsonDoc[NVM_IR_ENABLED].as<bool>();
  const bool paramIrUnrecognizedAsOnOff = jsonDoc[NVM_IR_UNRECOGNIZED_AS_ONOFF].as<bool>();

  preferences.begin(NVM_NAMESPACE, false);

  preferences.putUInt(NVM_WIFI_MODE, static_cast<int>(paramWifiMode));
  preferences.putString(NVM_WIFI_SSID, paramWifiSsid);
  preferences.putString(NVM_WIFI_AP_SSID, paramWifiApSsid);

  if (jsonDoc[NVM_WIFI_PASSWORD].is<const char*>()) {
    const char* paramWifiPassword = jsonDoc[NVM_WIFI_PASSWORD].as<const char*>();
    if (paramWifiPassword != nullptr && strlen(paramWifiPassword) > 0) {
      preferences.putString(NVM_WIFI_PASSWORD, paramWifiPassword);
    }
  }

  if (jsonDoc[NVM_WIFI_AP_PASSWORD].is<const char*>()) {
    const char* paramWifiApPassword = jsonDoc[NVM_WIFI_AP_PASSWORD].as<const char*>();
    if (paramWifiApPassword != nullptr && strlen(paramWifiApPassword) > 0) {
      preferences.putString(NVM_WIFI_AP_PASSWORD, paramWifiApPassword);
    }
  }

  preferences.putBool(NVM_BOOT_FADE_IN, paramBootFadeIn);
  preferences.putString(NVM_BOOT_COLOR, paramBootColor);
  preferences.putBool(NVM_BOOT_WOL, paramBootWol);
  preferences.putString(NVM_BOOT_WOL_MAC, paramBootWolMac);

  preferences.putBool(NVM_ACTIVITY_TIMEOUT_ENABLED, paramActivityTimeoutEnabled);
  preferences.putUInt(NVM_ACTIVITY_TIMEOUT, paramActivityTimeout);
  
  preferences.putBool(NVM_WEB_UI_USE_WEB_SOCKETS, paramWebUiUseWebSockets);
  preferences.putBool(NVM_TURN_OFF_ON_LEAVE, paramTurnOffOnLeave);
  
  preferences.putBool(NVM_DMX_ENABLED, paramDmxEnabled);
  preferences.putBool(NVM_DMX_UNICAST, paramDmxUnicast);
  preferences.putUInt(NVM_DMX_UNIVERSE, paramDmxUniverse);
  preferences.putUInt(NVM_DMX_UNIVERSE_COUNT, paramDmxUniverseCount);
  preferences.putString(NVM_DEVICE_NAME, paramDeviceName);

  preferences.putBool(NVM_IR_ENABLED, paramIrEnabled);
  preferences.putBool(NVM_IR_UNRECOGNIZED_AS_ONOFF, paramIrUnrecognizedAsOnOff);

  if (jsonDoc[DEVICE_STRIPS].is<JsonArray>()) {
    for (int i = 0; i < DVC_STRIP_COUNT; i++) {
      String key = getStripLedCountKey(i);
      preferences.putUInt(key.c_str(), 0);
    }

    JsonArray strips = jsonDoc[DEVICE_STRIPS].as<JsonArray>();
    for (JsonObject strip : strips) {
      const int stripIdx = strip["index"] | -1;
      const int ledCount = std::max(0, strip[NVM_LED_COUNT] | 0);
      if (stripIdx < 0 || stripIdx >= DVC_STRIP_COUNT) {
        continue;
      }

      String key = getStripLedCountKey(stripIdx);
      preferences.putUInt(key.c_str(), ledCount);
    }
  }

  preferences.end();

  request->send(200, "text/html", "Configuration saved. Device is rebooting. Hit ok and wait 10 seconds for refresh.");

  delay(2000);
  ESP.restart();
}
