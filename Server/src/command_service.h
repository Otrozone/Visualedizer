#ifndef COMMAND_SERVICE_H
#define COMMAND_SERVICE_H

#include <ESPAsyncWebServer.h>
#include <WebSocketsServer.h>

void handleUpdateRequest(AsyncWebServerRequest* request);
void processWebSocketMessage(const String& message);
void processWebSocketBinary(uint8_t* payload, size_t length);
void echoWebSocketMessage(WebSocketsServer& webSocket, uint8_t clientNum, const String& message);

#endif
