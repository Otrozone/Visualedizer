#include "websocket_service.h"

#include <WebSocketsServer.h>

#include "command_service.h"
#include "led.h"
#include "main.h"
#include "render_service.h"

namespace {
WebSocketsServer webSocket = WebSocketsServer(81);

void processWsTxtPayload(uint8_t clientNum, uint8_t* payload, size_t length) {
  String message = "";
  for (size_t i = 0; i < length; i++) {
    message += static_cast<char>(payload[i]);
  }

  Serial.print("Received web socket message: ");
  Serial.println(message);

  processWebSocketMessage(message);
  echoWebSocketMessage(webSocket, clientNum, message);
}

void webSocketEvent(uint8_t clientNum, WStype_t type, uint8_t* payload, size_t length) {
  if (type == WStype_CONNECTED) {
    IPAddress ip = webSocket.remoteIP(clientNum);
    Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n",
        clientNum, ip[0], ip[1], ip[2], ip[3], payload);
    return;
  }

  if (type == WStype_BIN) {
    processWebSocketBinary(payload, length);
    return;
  }

  if (type == WStype_TEXT) {
    processWsTxtPayload(clientNum, payload, length);
    return;
  }

  if (type == WStype_DISCONNECTED) {
    forEachLedStrip([](LedStripDvc& dvc) {
      requestOff(dvc.ledIdx);
    });
  }
}
}  // namespace

void initWebSockets() {
  webSocket.begin();
  webSocket.onEvent(webSocketEvent);
}

void pollWebSocket() {
  webSocket.loop();
}
