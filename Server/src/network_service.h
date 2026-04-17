#ifndef NETWORK_SERVICE_H
#define NETWORK_SERVICE_H

#include <ESPAsyncWebServer.h>

int getWifiSignalStrength();
void initWifi();
void reconnectWifi();
void printHeartbeat();
void sendMagicPacket(const char* macAddress);
void handleWol(AsyncWebServerRequest* request);

#endif
