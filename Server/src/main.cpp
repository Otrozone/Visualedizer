// https://wiki.seeedstudio.com/xiao_esp32s3_getting_started/
// https://www.freertos.org/a00125.html
// https://fastled.io/docs/

#include <Arduino.h>
#include "SPIFFS.h"
#include <WiFi.h>
// #include <EEPROM.h>
// #include <WebServer.h>
#include <WebSocketsServer.h>
#include <FastLED.h>
// #include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
// #include <FreeRTOS.h>
// #include <task.h>
#include <Preferences.h>
#include <ArduinoJson.h>

#include "modes.h"
#include "credentials.h"

#include <ESPAsyncE131.h>

#ifdef ESP32S3_FURNITURE
  #define DATA_PIN 3
  #define NUM_LEDS 218 // 109 * 2
  #define OFFSET 0
  #define DMX_UNIVERSE 1
#endif
#ifdef ESP32S3_DESK 
  #define DATA_PIN 3
  #define NUM_LEDS 274 // (93 + 44) * 2
  #define OFFSET 184
  #define DMX_UNIVERSE 2
#endif
#ifdef ESP32S3_COUCH
  #define DATA_PIN 3
  #define NUM_LEDS 0
  #define OFFSET 0
  #define DMX_UNIVERSE 3
#endif
#ifdef ESP32C3_WARDROBE
  #define DATA_PIN 4 // D2
  #define NUM_LEDS 269
  #define OFFSET 0
  #define DMX_UNIVERSE 99
#endif

/*
#define EEPROM_SIZE 512
#define EEPROM_NUM_LEDS_ADDR 0
#define EEPROM_MAX_SSID_LENGTH 32
#define EEPROM_MAX_PASSWORD_LENGTH 64

#define EEPROM_SSID_START_ADDRESS 1
#define EEPROM_PASSWORD_START_ADDRESS (EEPROM_SSID_START_ADDRESS + EEPROM_MAX_SSID_LENGTH + 1) // +1 for the \0 terminator
*/

#define NVM_NAMESPACE "visualedizer"
#define NVM_WIFI_SSID "ssid"
#define NVM_WIFI_PASSWORD "password"
#define NVM_LED_COUNT "ledCount"
#define NVM_DMX_ENABLED "dmxEnabled"
#define NVM_DMX_UNICAST "dmxUnicast"
#define NVM_DMX_UNIVERSE "dmxUniverse"
#define NVM_DMX_UNIVERSE_COUNT "dmxUniCount"
#define NVM_WEB_SOCK_ENABLED "webSockEnabled"

Preferences preferences;

// CRGB leds[NUM_LEDS];
uint16_t ledCount = 0;
CRGB* leds = nullptr;

AsyncWebServer server(80);

WebSocketsServer webSocket = WebSocketsServer(81);

// const char *ssid = WIFI_SSID;
// const char *password = WIFI_PASSWORD;

String wifiSsid;
String wifiPassword;

String wifiApSsid = "Visualedizer";
String wifiApPassword = "";

const uint8_t wifiTimeout = 15;

String htmlContent = "";
// String htmlContent = "<!-- Based on: https://github.com/mdbootstrap/bootstrap-5-admin-template/ --><!-- Single file, lot of CDN, low size. --><!-- Minify: https://medium.com/@panklokka/free-html-minifier-online-no-limitations-totally-free-syscotools-cc915aaeb75a --><!DOCTYPE html><html lang=\"en\" data-mdb-theme=\"dark\"><head>  <meta charset=\"UTF-8\" />  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\" />  <meta http-equiv=\"x-ua-compatible\" content=\"ie=edge\" />  <title>Visualedizer</title>  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css\" rel=\"stylesheet\"/>  <link href=\"https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap\" rel=\"stylesheet\"/>  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/7.1.0/mdb.min.css\" rel=\"stylesheet\"/>  <style>    @media (min-width: 991.98px) {      main {        padding-left: 240px;      }    }    .sidebar {      position: fixed;      top: 0;      bottom: 0;      left: 0;      padding: 58px 0 0;      box-shadow: 0 2px 5px 0 rgb(0 0 0 / 5%), 0 2px 10px 0 rgb(0 0 0 / 5%);      width: 240px;      z-index: 600;    }    @media (max-width: 991.98px) {      .sidebar {        width: 100%;      }    }    .sidebar .active {      border-radius: 5px;      box-shadow: 0 2px 5px 0 rgb(0 0 0 / 16%), 0 2px 10px 0 rgb(0 0 0 / 12%);    }    .sidebar-sticky {      position: relative;      top: 0;      height: calc(100vh - 48px);      padding-top: 0.5rem;      overflow-x: hidden;      overflow-y: auto;    }    .content {      display: none;    }    .custom-range::-webkit-slider-runnable-track {      background-image: var(--mdb-gradient);    }    .hue-colors {      border: 1px solid #000;      border-radius: 5px;      width: 100%;      height: 1em;          background: linear-gradient(to right,         hsl(0, 100%, 50%) 0%,         hsl(30, 100%, 50%) 8.33%,         hsl(60, 100%, 50%) 16.66%,         hsl(120, 100%, 50%) 33.33%,         hsl(240, 100%, 50%) 66.66%,         hsl(300, 100%, 50%) 83.33%,         hsl(360, 100%, 50%) 100%       );    }  </style></head><body>  <header>    <nav id=\"sidebarMenu\" class=\"collapse d-lg-block sidebar collapse\" style=\"background-color: rgb(48, 48, 48);\">      <div class=\"position-sticky\">        <div class=\"list-group list-group-flush mx-3 mt-4\">          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"solid-color\">            <i class=\"fas fa-solid fa-palette fa-fw me-3\"></i>            <span>Solid color</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"gradient\">            <i class=\"fas fa-circle-half-stroke fa-fw me-3\"></i>            <span>Gradient</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"rainbow\">            <i class=\"fas fa-rainbow fa-fw me-3\"></i>            <span>Rainbow</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"running-rainbow\">            <i class=\"fas fa-person-running fa-fw me-3\"></i>            <span>Running rainbow</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"strobe\">            <i class=\"fas fa-person-running fa-fw me-3\"></i>            <span>Strobe</span>          </a>        </div>      </div>    </nav>    <nav id=\"main-navbar\" class=\"navbar navbar-expand-lg navbar-light fixed-top\">      <div class=\"container-fluid\">        <button data-mdb-collapse-init class=\"navbar-toggler\" type=\"button\" data-mdb-toggle=\"collapse\" data-mdb-target=\"#sidebarMenu\"          aria-controls=\"sidebarMenu\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">          <i class=\"fas fa-bars\"></i>        </button>        <a class=\"navbar-brand\" href=\"#\">          <i class=\"far fa-regular fa-lightbulb fa-2x\"></i>        </a>                <div class=\"form-check form-switch\">          <input class=\"form-check-input\" type=\"checkbox\" role=\"switch\" id=\"chbMain\" />          <label class=\"form-check-label\" for=\"chbMain\">On/off</label>        </div>        <ul class=\"navbar-nav ms-auto d-flex flex-row\">          <li class=\"nav-item me-3 me-lg-0\">            <a class=\"nav-link\" href=\"#\">              <i class=\"fab fa-github\"></i>            </a>          </li>        </ul>      </div>    </nav>  </header>  <main style=\"margin-top: 58px\">    <div class=\"container pt-4\">      <section class=\"mb-4\">        <div class=\"card\">          <div class=\"card-header py-3\">            <h5 class=\"mb-0 text-center\" id=\"cardTitle\"><strong>Title</strong></h5>          </div>          <div class=\"card-body\">                        <div class=\"content\" id=\"solid-color\">              <section class=\"w-100 p-4\">                <label for=\"colorPicker\" class=\"form-label\">Select color</label>                <input type=\"color\" class=\"form-control form-control-color\" id=\"colorPicker\" value=\"#563d7c\" title=\"Choose your color\" data-param-name=\"color\">              </section>            </div>                <div class=\"content\" id=\"gradient\">              <h2>Gradient</h2>              <label class=\"form-label\" for=\"rngHueStart\">Hue start</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"0\" max=\"360\" step=\"5\" id=\"rngHueStart\" data-param-name=\"hueStart\" />              </div>              <div class=\"hue-colors\"></div>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"0\" max=\"360\" step=\"5\" id=\"rngHueEnd\" data-param-name=\"hueEnd\" />              </div>              <label class=\"form-label\" for=\"rngHueEnd\">Hue end</label>              <br><br>              <label class=\"form-label\" for=\"rngBrightness\">Brightness</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" id=\"rngBrightness\" data-param-name=\"brightness\"/>              </div>            </div>                <div class=\"content\" id=\"rainbow\">              <h2>Rainbow</h2>              <p>-._</p>            </div>                <div class=\"content\" id=\"running-rainbow\">              <h2>Running rainbow</h2>              <label class=\"form-label\" for=\"rngDelay\">Delay</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"0\" max=\"500\" step=\"10\" id=\"rngDelay\" data-param-name=\"delay\" />              </div>              <label class=\"form-label\" for=\"rngStep\">Step</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"1\" max=\"50\" step=\"1\" id=\"rngStep\" data-param-name=\"step\" />              </div>              <label class=\"form-label\" for=\"rngDelta\">Delta</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"1\" max=\"50\" step=\"1\" id=\"rngDelta\" data-param-name=\"delta\" />              </div>            </div>            <div class=\"content\" id=\"strobe\">              <h2>Strobe</h2>              <p>.---.-..-.--..----.---...-.-</p>              <input type=\"color\" class=\"form-control form-control-color\" id=\"colorStrobe\" value=\"#563d7c\" title=\"Choose your color\" data-param-name=\"color\">            </div>          </div>        </div>      </section>    </div>  </main>    <script type=\"text/javascript\" src=\"https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/7.1.0/mdb.umd.min.js\"></script>  <script>    /* Menu */    const menuItems = document.querySelectorAll('.list-group-item');    const contents = document.querySelectorAll('.content');    const mainSwitch = document.getElementById('chbMain');    menuItems.forEach((item) => {      item.addEventListener('click', () => {        const clickedItem = item;        menuItems.forEach(item => item.classList.remove('active'));        clickedItem.classList.add('active');        contents.forEach(content => content.style.display = 'none');        const correspondingContent = document.getElementById(`${clickedItem.getAttribute('data-content-id')}`);        correspondingContent.style.display = 'block';        document.querySelector('#cardTitle').textContent = clickedItem.textContent;        const mediaQuery = window.matchMedia('(max-width: 992px)');        if (mediaQuery.matches) {          sidebarMenu.classList.remove('show');        }      });    });    const defaultItem = document.querySelector('[data-content-id=\"solid-color\"]');    defaultItem.click();    mainSwitch.addEventListener('input', () => {      if (mainSwitch.checked) {        const input = document.querySelector('div.content[style*=\"display: block\"] input');        const event = new Event('input', {          bubbles: true,          cancelable: true,        });        input.dispatchEvent(event);      } else {        let queryParams = `command=off`;        updateColors(queryParams);      }    });    /* Inputs -> commands + params */    contents.forEach((content) => {       const inputs = content.querySelectorAll('input');       inputs.forEach((input) => {        input.addEventListener('input', () => {          onChange(content, inputs);        })       });    });    function onChange(content, inputs) {      if (mainSwitch.checked) {        let queryParams = `command=${encodeURIComponent(content.id)}`;        inputs.forEach(input => {          queryParams += `&${input.getAttribute('data-param-name')}=${encodeURIComponent(input.value)}`;        });        updateColors(queryParams);      } else {        console.log('The main switch is in OFF state.');      }    }    /* API calling */    function updateColors(params) {      const apiUrl = `/update?${params}`;      console.log(apiUrl);      fetch(apiUrl)        .then(response => {          if (!response.ok) {            throw new Error('Network response was not ok');          }          return response.text; /* response.json(); */        })        .then(data => {          console.log('Data received:', data);        })        .catch(error => {          console.error('There was a problem with the fetch operation:', error);        });    }  </script></body></html>";

String taskCommand = "";
TaskHandle_t taskHandle;

static uint8_t hue = 0;

// DMX Configuration
const uint8_t DMX_MAX_UNIVERSE_COUNT = 255;
const uint16_t DMX_UNIVERSE_COUNT = 5;
const uint16_t DMX_CHANNEL_RED = 1;
const uint16_t DMX_CHANNEL_GREEN = 2;
const uint16_t DMX_CHANNEL_BLUE = 3;
ESPAsyncE131 e131(DMX_MAX_UNIVERSE_COUNT);
e131_packet_t packet;
uint16_t dmxUniverseCount;
uint16_t dmxUniverse;
bool dmxUnicast;
bool dmxEnabled;

bool webSockEnabled;

const uint16_t DMX_POINTER_UNIVERSE = 1;
const uint16_t DMX_POINTER_CHANNEL = 1;
const uint16_t DMX_CHANNELS_PER_UNIVERSE = 512;
const uint16_t DMX_MAX_LED_COUNT_PER_UNIVERSE = 170;
uint32_t dmxLastPacketNum;

const int udpPort = 5568; // E1.31 port
WiFiUDP udp;

void initLeds() {
  leds = new CRGB[ledCount];
  FastLED.addLeds<WS2813, DATA_PIN>(leds, ledCount);
}

void initConf() {
  preferences.begin(NVM_NAMESPACE, false);

  ledCount = preferences.getUInt(NVM_LED_COUNT, NUM_LEDS);
  wifiSsid = preferences.getString(NVM_WIFI_SSID, WIFI_SSID); 
  wifiPassword = preferences.getString(NVM_WIFI_PASSWORD, WIFI_PASSWORD);

  dmxUnicast = preferences.getBool(NVM_DMX_UNICAST, true);
  dmxUniverse = preferences.getUInt(NVM_DMX_UNIVERSE, DMX_UNIVERSE);
  dmxUniverseCount = preferences.getUInt(NVM_DMX_UNIVERSE_COUNT, DMX_UNIVERSE_COUNT);

  preferences.end();
}

/*
void writeIntToEEPROM(int address, const int &val) {
  EEPROM.write(address, val);
  EEPROM.commit();
}

void writeStringToEEPROM(int startAddress, const String &str, int maxLength) {
  int i;
  for (i = 0; i < str.length() && i < maxLength; ++i) {
    EEPROM.write(startAddress + i, str[i]);
  }
  EEPROM.write(startAddress + i, '\0');
  EEPROM.commit();
}

String readStringFromEEPROM(int startAddress, int maxLength) {
  char data[maxLength + 1]; // buffer
  int len = 0;
  unsigned char k = EEPROM.read(startAddress);
  while (k != '\0' && len < maxLength) {
    data[len] = k;
    len++;
    k = EEPROM.read(startAddress + len);
  }
  data[len] = '\0';
  return String(data);
}*/

void circularShift() {
  CRGB tempArray[ledCount];

  for (int i = 0; i < ledCount; i++) {
    tempArray[(i + OFFSET) % ledCount] = leds[i];
  }

  for (int i = 0; i < ledCount; i++) {
    leds[i] = tempArray[i];
  }
}

void FastLedShow() {
  if (OFFSET > 0) {
    circularShift();
  }

  FastLED.show();
}

void terminateCurrTask() {
  if (taskHandle != NULL) {
    TaskHandle_t xCurrentTask = xTaskGetCurrentTaskHandle();
    if (xCurrentTask == taskHandle) {
      Serial.println("Delete task using handle...");
      vTaskDelete(taskHandle);
      taskHandle = NULL;
    } else {
      Serial.println("Delete task using flag...");
      terminateTaskFlag = true;
      delay(50);
      terminateTaskFlag = false;
    }
  }
}

int mapRange(int value, int fromLow, int fromHigh, int toLow, int toHigh) {
    return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
}

String urlDecode(const String& input) {
    String decoded = "";
    char c;
    int len = input.length();
    int i = 0;

    while (i < len) {
        c = input.charAt(i);
        if (c == '%') {
            char hex[3];
            hex[0] = input.charAt(++i);
            hex[1] = input.charAt(++i);
            hex[2] = '\0';
            decoded += char(strtol(hex, nullptr, 16));
        } else if (c == '+') {
            decoded += ' ';
        } else {
            decoded += c;
        }
        i++;
    }

    return decoded;
}

String getQueryParameterValue(String url, String parameterName) {
  int startIdx = url.indexOf("?" + parameterName + "=");
  if (startIdx == -1) {
    startIdx = url.indexOf("&" + parameterName + "=");
    if (startIdx == -1) {
      return "";
    }
  }
  startIdx += parameterName.length() + 2; // Skip parameter name, '?' or '&', and '='

  int endIdx = url.indexOf('&', startIdx + 1);
  if (endIdx == -1) {
    endIdx = url.length();
  }

  String encVal = url.substring(startIdx, endIdx);
  Serial.println("Query parameter encoding value: " + encVal);

  return urlDecode(encVal);
}

void processCommand(String message) {
  Serial.println("WebSockets message: " + message);
  String cmd = getQueryParameterValue(message, "command");
  Serial.println("WebSockets command: " + cmd);
  if (cmd.equals("off")) {
    terminateCurrTask();
    fill_solid(leds, ledCount, CRGB::Black);
    FastLedShow();
  } else if (cmd.equals("solid-color")) {
    terminateCurrTask();
    CRGB color = htmlColor2Crgb(getQueryParameterValue(message, "color"));
    fill_solid(leds, ledCount, color);
    FastLedShow();
  }
}

void update(AsyncWebServerRequest *request) {
  if (request->hasParam("command", false, false)) {
    AsyncWebParameter* command = request->getParam("command", false, false);

    String cmd = command->value();
    
    // Serial.println("Received command: " + cmd);
    
    if (cmd.length() > 0) {
      const uint32_t StackSize = 2048; // What is the good value for my operations?

      // Static effects are directly here, for dynamic effects (animations) 
      // is started new thread using a task
      if (cmd.equals("off")) {
        terminateCurrTask();
        fill_solid(leds, ledCount, CRGB::Black);      
        FastLedShow();
      } else if (cmd.equals("running-rainbow")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
        params->ledCount = ledCount;
        params->leds = leds;
        params->delay = request->getParam("delay", false, false)->value().toInt();
        params->step = request->getParam("step", false, false)->value().toInt();
        params->delta = request->getParam("delta", false, false)->value().toInt();
        
        xTaskCreate(taskRunningRainbow, "CurrentLedTask", StackSize, params, 10, &taskHandle);
      } else if (cmd.equals("strobe")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskStrobeParams *params = new TaskStrobeParams;
        // TaskStrobeParams params;
        params->ledCount = ledCount;
        params->leds = leds;
        params->delay1 = request->getParam("delay1", false, false)->value().toInt();
        params->delay2 = request->getParam("delay2", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());

        Serial.println("Delay1: " + String(params->delay1));
        Serial.println("Delay2: " + String(params->delay2));
        
        xTaskCreate(taskStrobe, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("strobe-random")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskStrobeParams *params = new TaskStrobeParams;
        params->ledCount = ledCount;
        params->leds = leds;
        // params.delay1 = request->getParam("delay1", false, false)->value().toInt();
        // params.delay2 = request->getParam("delay2", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());
        
        xTaskCreate(taskStrobe, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("solid-color")) {
        terminateCurrTask();
        CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());
        fill_solid(leds, ledCount, color);
        FastLedShow();
      } else if (cmd.equals("fade-in")) {
        terminateCurrTask();

        TaskFadeParams *params = new TaskFadeParams;
        params->ledCount = ledCount;
        params->leds = leds;
        params->duration = request->getParam("duration", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());

        xTaskCreate(taskFadeIn, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("fade-out")) {
        terminateCurrTask();

        TaskFadeParams *params = new TaskFadeParams;
        params->ledCount = ledCount;
        params->leds = leds;
        params->duration = request->getParam("duration", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());

        xTaskCreate(taskFadeOut, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("gradient")) {
        terminateCurrTask();

        int hueStart = request->getParam("hueStart", false, false)->value().toInt();
        int hueEnd = request->getParam("hueEnd", false, false)->value().toInt();
        int brightness = request->getParam("brightness", false, false)->value().toInt();

        CHSV chsvStart;
        chsvStart.hue = mapRange(std::min(hueStart, hueEnd), 0, 360, 0, 255);
        chsvStart.value = mapRange(brightness, 0, 100, 0, 255);
        chsvStart.saturation = 255;

        CHSV chsvEnd;
        chsvEnd.hue = mapRange(std::max(hueStart, hueEnd), 0, 360, 0, 255);
        chsvEnd.value = mapRange(brightness, 0, 100, 0, 255);
        chsvEnd.saturation = 255;

        fill_gradient_HSV(leds, ledCount, chsvStart, chsvEnd, FORWARD_HUES );
        FastLedShow();
      } else if (cmd.equals("noise")) {
        // Not yet implemented
      } else if (cmd.equals("abort")) {
        vTaskDelete(taskHandle);
      } else {
        Serial.println("Unknown command: " + cmd);  
      }

      taskCommand = cmd;

      request->send(200, "text/plain", "Processing command: " + cmd);

    } else {
      request->send(400, "text/plain", "Empty 'command' value received");
    }
  } else {
    request->send(400, "text/plain", "No 'command' query parameter received");
  }
}

void handleRootRequest(AsyncWebServerRequest *request) {
  request->send(200, "text/html", htmlContent);
}

void handleGetConf(AsyncWebServerRequest *request) {
  JsonDocument jsonDoc;

  jsonDoc[NVM_LED_COUNT] = ledCount;
  jsonDoc[NVM_WIFI_SSID] = wifiSsid;
  jsonDoc[NVM_WIFI_PASSWORD] = wifiPassword;
  jsonDoc[NVM_WEB_SOCK_ENABLED] = webSockEnabled;
  jsonDoc[NVM_DMX_ENABLED] = dmxEnabled;
  jsonDoc[NVM_DMX_UNICAST] = dmxUnicast;
  jsonDoc[NVM_DMX_UNIVERSE] = dmxUniverse;
  jsonDoc[NVM_DMX_UNIVERSE_COUNT] = dmxUniverseCount;

  String jsonData;
  serializeJson(jsonDoc, jsonData);

  request->send(200, "text/html", jsonData);
}

void handleSetConf(AsyncWebServerRequest *request, uint8_t *data, size_t len, size_t index, size_t total) {
  JsonDocument jsonDoc;
  DeserializationError error = deserializeJson(jsonDoc, (const char*)data);

  if (error) {
    Serial.print(F("Deserialization of configuration failed"));
    Serial.println(error.f_str());
    request->send(400, "application/json", "{\"error\":\"Invalid JSON\"}");
    return;
  }

  const int paramLedCount = jsonDoc[NVM_LED_COUNT];
  const char* paramWifiSsid = jsonDoc[NVM_WIFI_SSID];
  const char* paramWifiPassword = jsonDoc[NVM_WIFI_PASSWORD];
  const bool paramWebSockEnabled = jsonDoc[NVM_WEB_SOCK_ENABLED];
  const bool paramDmxEnabled = jsonDoc[NVM_DMX_ENABLED];
  const bool paramDmxUnicast = jsonDoc[NVM_DMX_UNICAST];
  const int paramDmxUniverse = jsonDoc[NVM_DMX_UNIVERSE];
  const int paramDmxUniverseCount = jsonDoc[NVM_DMX_UNIVERSE_COUNT];

  preferences.putUInt(NVM_LED_COUNT, paramLedCount);
  preferences.putString(NVM_WIFI_SSID, paramWifiSsid);
  preferences.putString(NVM_WIFI_PASSWORD, paramWifiPassword);

  preferences.putBool(NVM_WEB_SOCK_ENABLED, paramWebSockEnabled);
  preferences.putBool(NVM_DMX_ENABLED, paramDmxEnabled);
  preferences.putBool(NVM_DMX_UNICAST, paramDmxUnicast);
  preferences.putUInt(NVM_DMX_UNIVERSE, paramDmxUniverse);
  preferences.putUInt(NVM_DMX_UNIVERSE_COUNT, paramDmxUniverseCount);

  /*
  if (request->hasParam(NVM_LED_COUNT, true)) {
    int paramLedCount = request->getParam(NVM_LED_COUNT, true)->value().toInt();
    preferences.putUInt(NVM_LED_COUNT, paramLedCount);
  }

  if (request->hasParam(NVM_WIFI_SSID, true)) {
    String paramSsid = request->getParam(NVM_WIFI_SSID, true)->value();
    preferences.putString(NVM_WIFI_SSID, paramSsid);
  }

  if (request->hasParam(NVM_WIFI_PASSWORD, true)) {
    String paramPassword = request->getParam(NVM_WIFI_PASSWORD, true)->value();
    preferences.putString(NVM_WIFI_PASSWORD, paramPassword);
  }
  */

  request->send(200, "text/html", "Configuration saved. Rebooting...");

  delay(2000);
  ESP.restart();
}

void initHttpServer() {
  server.on("/test", [](AsyncWebServerRequest *request) {
    request->send(200, "text/plain", "Hello from server.");
  });

  server.on("/", HTTP_GET, handleRootRequest);
  server.on("/get-conf", HTTP_GET, handleGetConf);
  // server.on("/set-conf", HTTP_GET, handleSetConf);
  // server.on("/set-conf", HTTP_POST, [](AsyncWebServerRequest *request) {}, NULL, handleSetConf);
  server.on("/set-conf", HTTP_POST, NULL, NULL, handleSetConf);
  server.on("/update", HTTP_GET, update);

  server.begin();
  Serial.println("HTTP server started");
}

void initSpiffs() {
  Serial.println("SPIFFS initialization");

  if (!SPIFFS.begin(true)) {
    Serial.println("An Error has occurred while mounting SPIFFS");
    return;
  }

  File file = SPIFFS.open("/index.html");
  if (!file) {
    Serial.println("Failed to open file for reading");
    return;
  }

  Serial.println("Loading html content from SPIFFS");
  htmlContent = "";
  while (file.available()) {
    char c = file.read(); 
    htmlContent += c;
  }
  file.close();
}

void processWsTxtPayload(uint8_t num, uint8_t *payload, size_t length) {
  String message = "";
  for (size_t i = 0; i < length; i++) {
    message += (char)payload[i];
  }

  Serial.print("Received web socket message: ");
  Serial.println(message);

  processCommand(message);

  webSocket.sendTXT(num, message);
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t *payload, size_t length) {
  if (type == WStype_CONNECTED) {
    IPAddress ip = webSocket.remoteIP(num);
    Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);
  }

  if (type == WStype_BIN) {
    if (length == ledCount * 3) {
      for (int i = 0; i < ledCount; i++) {
        leds[i] = CRGB(payload[i * 3], payload[i * 3 + 1], payload[i * 3 + 2]);
      }
      FastLedShow();
    } else {
      Serial.println("Incorrect data length");
    }
  }

  if (type == WStype_TEXT) {
    processWsTxtPayload(num, payload, length);
  }  

  if (type == WStype_DISCONNECTED) {
    fill_solid(leds, ledCount, CRGB(0, 0, 0));
    FastLedShow();
  }
}

void initWifi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(wifiSsid, wifiPassword);

  uint8_t tryCount = 0;
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED && tryCount < wifiTimeout) {
      tryCount++;
      Serial.print('.');
      delay(1000);
  }
  if (WiFi.status() != WL_CONNECTED) {
    // Wifi connection timeout, switch to AP
    WiFi.mode(WIFI_AP);
    WiFi.softAP(wifiApSsid, wifiApPassword);
    Serial.print("Unable to connect to WiFi, switching to AP mode");
  }

  // digitalWrite(LED_BUILTIN, HIGH);

  Serial.println();
  Serial.println(WiFi.localIP());
}

/*
void initEeprom() {
  EEPROM.begin(EEPROM_SIZE);

  String eepromSsid = readStringFromEEPROM(EEPROM_SSID_START_ADDR, EEPROM_MAX_SSID_LENGTH);
  String eepromPassword = readStringFromEEPROM(EEPROM_PASSWORD_START_ADDR, EEPROM_MAX_PASSWORD_LENGTH);

  if (eepromSsid.length() > 0) {
    wifiSsid = eepromSsid;
  } else {
    wifiSsid = ssid;
  }

  if (eepromPassword.length() > 0) {
    wifiPassword = eepromPassword;
  } else {
    wifiPassword = ssid;
  }
}
*/

void initDmx() {
  bool dmxResult;
  if (dmxUnicast) {
    // Unicast
    dmxResult = e131.begin(E131_UNICAST, dmxUniverse);
  } else {
    // Multicast
    dmxResult = e131.begin(E131_MULTICAST, dmxUniverse, dmxUniverseCount);
  }
  if (dmxResult) {
    Serial.println(F("DMX initialized, listening for incoming data"));
  } else {
    Serial.println(F("DMX initialization failed"));
  }
}

void initWebSockets() {
  webSocket.begin();
  webSocket.onEvent(webSocketEvent);
}

void setup() {
  Serial.begin(115200);
  delay(2000);

  // pinMode(LED_BUILTIN, OUTPUT);
  // digitalWrite(LED_BUILTIN, LOW);

  pinMode(DATA_PIN, OUTPUT);

  initConf();
  initLeds();
  initSpiffs();
  // initEeprom();
  initWifi();
  initHttpServer();
  initWebSockets();
  initDmx();
  /*udp.begin(udpPort);
  Serial.printf("Listening for E1.31 packets on port %d\n", udpPort);*/
}

float normalize(float x, float min, float max) {
  return (x - min) / (max - min);
}

void processDmx() {
  if (!e131.isEmpty()) {
    // Serial.println("e131 is not empty");

    e131_packet_t packet;
    e131.pull(&packet);

    uint16_t universeIdx = htons(packet.universe);
    // Serial.printf("Universe index: %u\n", universeIdx);

    if (universeIdx == DMX_UNIVERSE) {
      for (int i = 0; i < ledCount; i++) {
        const int dmxChannelIdx = round(normalize(i, 0, ledCount - 1) * DMX_MAX_LED_COUNT_PER_UNIVERSE);
        leds[i] = CRGB(packet.property_values[dmxChannelIdx * 3 + 1], packet.property_values[dmxChannelIdx * 3 + 2], packet.property_values[dmxChannelIdx * 3 + 3]);
      }
      /*Serial.println();
    
      for (int i = 0; i < 10; i++) {
        Serial.printf("%d/%d/%d", leds[i].r, leds[i].g, leds[i].b);
        if (i < 10 - 1) {
          Serial.print(", ");
        }
      }
      Serial.println();*/

      // Serial.printf("Count: %d", packet.property_value_count);
      /* for (int i = 0; i < 10; i++) {
        Serial.printf("%d", packet.property_values[i]);
        if (i < 10 - 1) {
          Serial.print(", ");
        }
      }
      Serial.println();*/

      FastLedShow();

      Serial.printf("Universe %u / %u Channels | Packet#: %u / Errors: %u / CH1: %u\n",
          htons(packet.universe),                 // The Universe for this packet
          htons(packet.property_value_count) - 1, // Start code is ignored, we're interested in dimmer data
          e131.stats.num_packets,                 // Packet counter
          e131.stats.packet_errors,               // Packet error counter
          packet.property_values[1]);             // Dimmer data for Channel 1
    }
  }
}

void loop() {
  if (webSockEnabled) {
    webSocket.loop();
  }

  if (dmxEnabled) {
    processDmx();
  }
}