#include "http_service.h"

#include <SPIFFS.h>

#include "command_service.h"
#include "controller.h"
#include "main.h"
#include "network_service.h"
#include "nvm.h"

namespace {
AsyncWebServer server(80);

String getContentType(String filename) {
  String ext = filename.substring(filename.lastIndexOf('.') + 1);
  Serial.println("File extension for content type: " + ext);
  if (ext == "html") return "text/html";
  if (ext == "css") return "text/css";
  if (ext == "js") return "text/javascript";
  if (ext == "png") return "image/png";
  if (ext == "jpg") return "image/jpeg";
  if (ext == "ico") return "image/x-icon";
  if (ext == "xml") return "text/xml";
  if (ext == "pdf") return "application/pdf";
  if (ext == "zip") return "application/zip";
  if (ext == "ttf") return "application/octet-stream";

  return "text/plain";
}

void handleStaticResource(AsyncWebServerRequest* request) {
  String path = request->url();

  Serial.println("A static resource requested: " + path);
  if (SPIFFS.exists(path)) {
    request->send(SPIFFS, path, getContentType(path));
  } else {
    Serial.println("File not found in SPIFFS: " + path);
    request->send(404, "text/plain", "File Not Found");
  }
}

void handleRootRequest(AsyncWebServerRequest* request) {
  request->send(SPIFFS, "/index.html", "text/html");
}
}  // namespace

void initHttpServer() {
  server.on("/test", [](AsyncWebServerRequest* request) {
    request->send(200, "text/plain", "Hello from server.");
  });

  server.on("/", HTTP_GET, handleRootRequest);
  server.on("/res/*", HTTP_GET, handleStaticResource);

  server.on("/get-conf", HTTP_GET, handleGetConf);
  server.on("/set-conf", HTTP_POST, [](AsyncWebServerRequest* request) {}, NULL, handleSetConf);
  server.on("/update", HTTP_GET, handleUpdateRequest);
  server.on("/wol", HTTP_GET, handleWol);
  server.on("/ctrl", HTTP_GET, handleCtrlSignalHttp);

  server.begin();
  Serial.println("HTTP server started");
}
