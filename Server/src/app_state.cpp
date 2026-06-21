#include "main.h"
#include "credentials.h"

Preferences preferences;

String wifiSsid = WIFI_SSID;
String wifiPassword = WIFI_PASSWORD;
DeviceWifiModeType wifiMode = DEVICE_WIFI_MODE_AUTO;

String wifiApSsid = "Visualedizer";
String wifiApPassword = "Rezidelausiv7331";

String deviceName = "Unnamed";

bool bootFadeIn = false;
String bootColor = "#FFFAFA";
bool bootWol = false;
String bootWolMac = "00:00:00:00:00:00";

bool webUiUseWebSockets = true;
bool turnOffOnLeave = false;
bool laserEnabled = true;
int laserTxPin = 43;
int laserEnablePin = -1;
bool strobeEnabled = true;
int strobePin = 4;

unsigned long lastActivity = 0;
bool activityTimeoutEnabled = false;
unsigned long activityTimeout = 0;
bool activityTimeoutRequested = false;
unsigned long tmpActivityTimeout = 0;
int fadeOutDuration = 10000;

bool irEnabled = false;
