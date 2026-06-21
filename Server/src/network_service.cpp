#include "network_service.h"

#include <HTTPClient.h>
#include <WiFi.h>

#include "main.h"

namespace {
constexpr uint8_t kWifiTimeoutSeconds = 15;
unsigned long lastReconnectAttempt = 0;
bool autoFallbackApConfigured = false;

void configureHostname() {
  if (deviceName != "Unnamed") {
    String hostname = deviceName;
    hostname.toLowerCase();
    WiFi.setHostname(hostname.c_str());
  }
}

bool startSoftAp() {
  if (WiFi.softAP(wifiApSsid.c_str(), wifiApPassword.c_str())) {
    Serial.println("AP started successfully.");
    return true;
  }

  Serial.println("AP start failed.");
  return false;
}

void startAutoFallbackAp() {
  WiFi.mode(WIFI_AP);
  autoFallbackApConfigured = true;
  startSoftAp();
}

void ensureAutoFallbackAp() {
  if (autoFallbackApConfigured) {
    return;
  }

  Serial.println("WiFi disconnected in AUTO mode, enabling fallback AP.");
  startAutoFallbackAp();
}

void startAutoReconnectWithFallbackAp() {
  configureHostname();
  WiFi.mode(WIFI_AP_STA);
  autoFallbackApConfigured = true;
  startSoftAp();
  WiFi.begin(wifiSsid.c_str(), wifiPassword.c_str());
  WiFi.reconnect();
}

void switchAutoToStaMode() {
  configureHostname();
  WiFi.mode(WIFI_STA);
  WiFi.setSleep(false);
  autoFallbackApConfigured = false;
}

void confWifi() {
  configureHostname();

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
}

void waitForWifiConnection(uint8_t timeoutSeconds) {
  uint8_t tryCount = 0;
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED && tryCount < timeoutSeconds) {
    tryCount++;
    Serial.print('.');
    delay(1000);
  }
  Serial.println();

  if (WiFi.status() == WL_CONNECTED) {
    IPAddress ipAddress = WiFi.localIP();
    Serial.println("IP address: http://" + ipAddress.toString());
  } else {
    Serial.println("Failed to reconnect to WiFi.");
  }
}

void checkInternetAccess() {
  Serial.println("Checking internet access");
  HTTPClient http;
  http.setTimeout(3000);
  http.begin("http://www.google.com");
  int httpCode = http.GET();

  if (httpCode > 0) {
    Serial.println("Internet access is available");
  } else {
    Serial.println("No internet access");
  }

  http.end();
}
}  // namespace

int getWifiSignalStrength() {
  long rssi = WiFi.RSSI();

  if (rssi >= -50) return 3;
  if (rssi >= -67) return 2;
  return 1;
}

void sendMagicPacket(const char* macAddress) {
  byte mac[6];
  sscanf(macAddress, "%hhx:%hhx:%hhx:%hhx:%hhx:%hhx",
      &mac[0], &mac[1], &mac[2], &mac[3], &mac[4], &mac[5]);

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

void handleWol(AsyncWebServerRequest* request) {
  if (request->hasParam("mac")) {
    String macAddress = request->getParam("mac")->value();
    sendMagicPacket(macAddress.c_str());
    request->send(200, "text/plain", "Magic packet sent to " + macAddress);
  } else {
    request->send(400, "text/plain", "Missing 'mac' query parameter");
  }
}

void initWifi() {
  confWifi();
  waitForWifiConnection(kWifiTimeoutSeconds);

  IPAddress ipAddress;
  if (WiFi.status() == WL_CONNECTED) {
    ipAddress = WiFi.localIP();
    WiFi.setSleep(false);
  } else if (wifiMode == DEVICE_WIFI_MODE_AUTO || wifiMode == DEVICE_WIFI_MODE_AP_STA) {
    Serial.println("Unable to connect to WiFi, switching to AP mode");
    if (wifiMode == DEVICE_WIFI_MODE_AUTO) {
      startAutoFallbackAp();
      ipAddress = WiFi.softAPIP();
    } else {
      WiFi.mode(WIFI_AP);
      if (startSoftAp()) {
        ipAddress = WiFi.softAPIP();
      }
    }
  } else if (wifiMode == DEVICE_WIFI_MODE_AP) {
    ipAddress = WiFi.softAPIP();
  }

  Serial.println("IP address: http://" + ipAddress.toString());
}

void reconnectWifi() {
  if (WiFi.status() == WL_CONNECTED) {
    if (wifiMode == DEVICE_WIFI_MODE_AUTO && autoFallbackApConfigured) {
      switchAutoToStaMode();
    }
    return;
  }

  if (wifiMode == DEVICE_WIFI_MODE_AP) {
    return;
  }

  if (wifiMode == DEVICE_WIFI_MODE_AUTO) {
    ensureAutoFallbackAp();
  }

  unsigned long now = millis();
  if (now - lastReconnectAttempt < 60000) {
    return;
  }

  lastReconnectAttempt = now;
  Serial.println("Attempting to reconnect to WiFi...");

  if (wifiMode == DEVICE_WIFI_MODE_AUTO) {
    startAutoReconnectWithFallbackAp();
    waitForWifiConnection(kWifiTimeoutSeconds);

    if (WiFi.status() == WL_CONNECTED) {
      switchAutoToStaMode();
    } else {
      startAutoFallbackAp();
    }
    return;
  }

  confWifi();
  WiFi.reconnect();
  waitForWifiConnection(kWifiTimeoutSeconds);
}

void printHeartbeat() {
  static unsigned long lastSerialPrint = 0;
  static int heartbeatCount = 0;

  unsigned long diff = millis() - lastSerialPrint;
  if (diff < 10000) {
    return;
  }

  Serial.print('*');
  lastSerialPrint = millis();
  heartbeatCount++;

  if (heartbeatCount < 6) {
    return;
  }

  Serial.println();
  Serial.print("Connection status: ");
  if (WiFi.status() == WL_CONNECTED) {
    Serial.print("connected");
    IPAddress ipAddress = WiFi.localIP();
    Serial.println(" (http://" + ipAddress.toString() + ")");
    checkInternetAccess();
  } else {
    Serial.println("disconnected");
  }
  heartbeatCount = 0;
}
