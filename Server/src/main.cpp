// https://wiki.seeedstudio.com/xiao_esp32s3_getting_started/
// https://www.freertos.org/a00125.html
// https://fastled.io/docs/

#include <Arduino.h>
#include "SPIFFS.h"
#include <WiFi.h>
#include <WebSocketsServer.h>
#include <FastLED.h>
// #include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
#include <FreeRTOS.h>
#include <task.h>

#include "modes.h"
#include "credentials.h"

#define NUM_LEDS 109 * 2
#define DATA_PIN 3
CRGB leds[NUM_LEDS];

AsyncWebServer server(80);

WebSocketsServer webSocket = WebSocketsServer(81);

const char *ssid = WIFI_SSID;
const char *password = WIFI_PASSWORD;

String htmlContent = "";
// String htmlContent = "<!-- Based on: https://github.com/mdbootstrap/bootstrap-5-admin-template/ --><!-- Single file, lot of CDN, low size. --><!-- Minify: https://medium.com/@panklokka/free-html-minifier-online-no-limitations-totally-free-syscotools-cc915aaeb75a --><!DOCTYPE html><html lang=\"en\" data-mdb-theme=\"dark\"><head>  <meta charset=\"UTF-8\" />  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\" />  <meta http-equiv=\"x-ua-compatible\" content=\"ie=edge\" />  <title>Visualedizer</title>  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css\" rel=\"stylesheet\"/>  <link href=\"https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap\" rel=\"stylesheet\"/>  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/7.1.0/mdb.min.css\" rel=\"stylesheet\"/>  <style>    @media (min-width: 991.98px) {      main {        padding-left: 240px;      }    }    .sidebar {      position: fixed;      top: 0;      bottom: 0;      left: 0;      padding: 58px 0 0;      box-shadow: 0 2px 5px 0 rgb(0 0 0 / 5%), 0 2px 10px 0 rgb(0 0 0 / 5%);      width: 240px;      z-index: 600;    }    @media (max-width: 991.98px) {      .sidebar {        width: 100%;      }    }    .sidebar .active {      border-radius: 5px;      box-shadow: 0 2px 5px 0 rgb(0 0 0 / 16%), 0 2px 10px 0 rgb(0 0 0 / 12%);    }    .sidebar-sticky {      position: relative;      top: 0;      height: calc(100vh - 48px);      padding-top: 0.5rem;      overflow-x: hidden;      overflow-y: auto;    }    .content {      display: none;    }    .custom-range::-webkit-slider-runnable-track {      background-image: var(--mdb-gradient);    }    .hue-colors {      border: 1px solid #000;      border-radius: 5px;      width: 100%;      height: 1em;          background: linear-gradient(to right,         hsl(0, 100%, 50%) 0%,         hsl(30, 100%, 50%) 8.33%,         hsl(60, 100%, 50%) 16.66%,         hsl(120, 100%, 50%) 33.33%,         hsl(240, 100%, 50%) 66.66%,         hsl(300, 100%, 50%) 83.33%,         hsl(360, 100%, 50%) 100%       );    }  </style></head><body>  <header>    <nav id=\"sidebarMenu\" class=\"collapse d-lg-block sidebar collapse\" style=\"background-color: rgb(48, 48, 48);\">      <div class=\"position-sticky\">        <div class=\"list-group list-group-flush mx-3 mt-4\">          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"solid-color\">            <i class=\"fas fa-solid fa-palette fa-fw me-3\"></i>            <span>Solid color</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"gradient\">            <i class=\"fas fa-circle-half-stroke fa-fw me-3\"></i>            <span>Gradient</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"rainbow\">            <i class=\"fas fa-rainbow fa-fw me-3\"></i>            <span>Rainbow</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"running-rainbow\">            <i class=\"fas fa-person-running fa-fw me-3\"></i>            <span>Running rainbow</span>          </a>          <a href=\"#\" class=\"list-group-item list-group-item-action py-2 ripple\" data-content-id=\"strobe\">            <i class=\"fas fa-person-running fa-fw me-3\"></i>            <span>Strobe</span>          </a>        </div>      </div>    </nav>    <nav id=\"main-navbar\" class=\"navbar navbar-expand-lg navbar-light fixed-top\">      <div class=\"container-fluid\">        <button data-mdb-collapse-init class=\"navbar-toggler\" type=\"button\" data-mdb-toggle=\"collapse\" data-mdb-target=\"#sidebarMenu\"          aria-controls=\"sidebarMenu\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">          <i class=\"fas fa-bars\"></i>        </button>        <a class=\"navbar-brand\" href=\"#\">          <i class=\"far fa-regular fa-lightbulb fa-2x\"></i>        </a>                <div class=\"form-check form-switch\">          <input class=\"form-check-input\" type=\"checkbox\" role=\"switch\" id=\"chbMain\" />          <label class=\"form-check-label\" for=\"chbMain\">On/off</label>        </div>        <ul class=\"navbar-nav ms-auto d-flex flex-row\">          <li class=\"nav-item me-3 me-lg-0\">            <a class=\"nav-link\" href=\"#\">              <i class=\"fab fa-github\"></i>            </a>          </li>        </ul>      </div>    </nav>  </header>  <main style=\"margin-top: 58px\">    <div class=\"container pt-4\">      <section class=\"mb-4\">        <div class=\"card\">          <div class=\"card-header py-3\">            <h5 class=\"mb-0 text-center\" id=\"cardTitle\"><strong>Title</strong></h5>          </div>          <div class=\"card-body\">                        <div class=\"content\" id=\"solid-color\">              <section class=\"w-100 p-4\">                <label for=\"colorPicker\" class=\"form-label\">Select color</label>                <input type=\"color\" class=\"form-control form-control-color\" id=\"colorPicker\" value=\"#563d7c\" title=\"Choose your color\" data-param-name=\"color\">              </section>            </div>                <div class=\"content\" id=\"gradient\">              <h2>Gradient</h2>              <label class=\"form-label\" for=\"rngHueStart\">Hue start</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"0\" max=\"360\" step=\"5\" id=\"rngHueStart\" data-param-name=\"hueStart\" />              </div>              <div class=\"hue-colors\"></div>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"0\" max=\"360\" step=\"5\" id=\"rngHueEnd\" data-param-name=\"hueEnd\" />              </div>              <label class=\"form-label\" for=\"rngHueEnd\">Hue end</label>              <br><br>              <label class=\"form-label\" for=\"rngBrightness\">Brightness</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" id=\"rngBrightness\" data-param-name=\"brightness\"/>              </div>            </div>                <div class=\"content\" id=\"rainbow\">              <h2>Rainbow</h2>              <p>-._</p>            </div>                <div class=\"content\" id=\"running-rainbow\">              <h2>Running rainbow</h2>              <label class=\"form-label\" for=\"rngDelay\">Delay</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"0\" max=\"500\" step=\"10\" id=\"rngDelay\" data-param-name=\"delay\" />              </div>              <label class=\"form-label\" for=\"rngStep\">Step</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"1\" max=\"50\" step=\"1\" id=\"rngStep\" data-param-name=\"step\" />              </div>              <label class=\"form-label\" for=\"rngDelta\">Delta</label>              <div class=\"range\" data-mdb-range-init>                <input type=\"range\" class=\"form-range\" min=\"1\" max=\"50\" step=\"1\" id=\"rngDelta\" data-param-name=\"delta\" />              </div>            </div>            <div class=\"content\" id=\"strobe\">              <h2>Strobe</h2>              <p>.---.-..-.--..----.---...-.-</p>              <input type=\"color\" class=\"form-control form-control-color\" id=\"colorStrobe\" value=\"#563d7c\" title=\"Choose your color\" data-param-name=\"color\">            </div>          </div>        </div>      </section>    </div>  </main>    <script type=\"text/javascript\" src=\"https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/7.1.0/mdb.umd.min.js\"></script>  <script>    /* Menu */    const menuItems = document.querySelectorAll('.list-group-item');    const contents = document.querySelectorAll('.content');    const mainSwitch = document.getElementById('chbMain');    menuItems.forEach((item) => {      item.addEventListener('click', () => {        const clickedItem = item;        menuItems.forEach(item => item.classList.remove('active'));        clickedItem.classList.add('active');        contents.forEach(content => content.style.display = 'none');        const correspondingContent = document.getElementById(`${clickedItem.getAttribute('data-content-id')}`);        correspondingContent.style.display = 'block';        document.querySelector('#cardTitle').textContent = clickedItem.textContent;        const mediaQuery = window.matchMedia('(max-width: 992px)');        if (mediaQuery.matches) {          sidebarMenu.classList.remove('show');        }      });    });    const defaultItem = document.querySelector('[data-content-id=\"solid-color\"]');    defaultItem.click();    mainSwitch.addEventListener('input', () => {      if (mainSwitch.checked) {        const input = document.querySelector('div.content[style*=\"display: block\"] input');        const event = new Event('input', {          bubbles: true,          cancelable: true,        });        input.dispatchEvent(event);      } else {        let queryParams = `command=off`;        updateColors(queryParams);      }    });    /* Inputs -> commands + params */    contents.forEach((content) => {       const inputs = content.querySelectorAll('input');       inputs.forEach((input) => {        input.addEventListener('input', () => {          onChange(content, inputs);        })       });    });    function onChange(content, inputs) {      if (mainSwitch.checked) {        let queryParams = `command=${encodeURIComponent(content.id)}`;        inputs.forEach(input => {          queryParams += `&${input.getAttribute('data-param-name')}=${encodeURIComponent(input.value)}`;        });        updateColors(queryParams);      } else {        console.log('The main switch is in OFF state.');      }    }    /* API calling */    function updateColors(params) {      const apiUrl = `/update?${params}`;      console.log(apiUrl);      fetch(apiUrl)        .then(response => {          if (!response.ok) {            throw new Error('Network response was not ok');          }          return response.text; /* response.json(); */        })        .then(data => {          console.log('Data received:', data);        })        .catch(error => {          console.error('There was a problem with the fetch operation:', error);        });    }  </script></body></html>";

String taskCommand = "";
TaskHandle_t taskHandle;

static uint8_t hue = 0;

void terminateCurrTask() {
  if (taskHandle != NULL) {
    // vTaskDelete(taskHandle);
    terminateTaskFlag = true;
    taskHandle = NULL;
    delay(100);
  }
}

int mapRange(int value, int fromLow, int fromHigh, int toLow, int toHigh) {
    return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
}

void update(AsyncWebServerRequest *request) {
  if (request->hasParam("command", false, false)) {
    AsyncWebParameter* command = request->getParam("command", false, false);

    String cmd = command->value();
    if (cmd.length() > 0) {

      const uint32_t StackSize = 2048; // What is the good value for my operations?

      // Static effects are directly here, for dynamic effects (animations) 
      // is started new thread using a task
      if (cmd.equals("off")) {
        terminateCurrTask();
        fill_solid(leds, NUM_LEDS, CRGB::Black);      
        FastLED.show();
      } else if (cmd.equals("running-rainbow")) {
        terminateCurrTask();

        Serial.println("Starting task: " + cmd);

        TaskRunningRainbowParams *params = new TaskRunningRainbowParams;
        params->ledCount = NUM_LEDS;
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
        params->ledCount = NUM_LEDS;
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
        params->ledCount = NUM_LEDS;
        params->leds = leds;
        // params.delay1 = request->getParam("delay1", false, false)->value().toInt();
        // params.delay2 = request->getParam("delay2", false, false)->value().toInt();
        params->color = htmlColor2Crgb(request->getParam("color", false, false)->value());
        
        xTaskCreate(taskStrobe, "CurrentLedTask", StackSize, params, 10, &taskHandle);

      } else if (cmd.equals("solid-color")) {
        terminateCurrTask();
        CRGB color = htmlColor2Crgb(request->getParam("color", false, false)->value());
        fill_solid(leds, NUM_LEDS, color);
        FastLED.show();       
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

        fill_gradient_HSV(leds, NUM_LEDS, chsvStart, chsvEnd, FORWARD_HUES );
        FastLED.show();
      } else if (cmd.equals("noise")) {

      } else if (cmd.equals("abort")) {
        vTaskDelete(taskHandle);
      } else {
        Serial.println("Unknown command: " + cmd);  
      }

      Serial.println("Received led command: " + cmd);

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

void initHttpServer() {
  server.on("/test", [](AsyncWebServerRequest *request) {
    request->send(200, "text/plain", "Hello from server.");
  });

  server.on("/update", HTTP_GET, update);

  server.on("/", HTTP_GET, handleRootRequest);

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

void webSocketEvent(uint8_t num, WStype_t type, uint8_t *payload, size_t length) {
  if (type == WStype_BIN) {
    if (length == NUM_LEDS * 3) {
      for (int i = 0; i < NUM_LEDS; i++) {
        leds[i] = CRGB(payload[i * 3], payload[i * 3 + 1], payload[i * 3 + 2]);
      }
      FastLED.show();
    } else {
      Serial.println("Incorrect data length");
    }
  }

  if (type == WStype_DISCONNECTED) {
    fill_solid(leds, NUM_LEDS, CRGB(0, 0, 0));
    FastLED.show();
  }
}

void wifiConnect() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED) {
      Serial.print('.');
      delay(1000);
  }
  digitalWrite(LED_BUILTIN, HIGH);
  Serial.println();
  Serial.println(WiFi.localIP());
}

void setup() {
  Serial.begin(115200);
  while (!Serial);
  delay(5000);

  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);

  initSpiffs();

  WiFi.mode(WIFI_STA);
  delay(100);
  wifiConnect();
  initHttpServer();

  FastLED.addLeds<WS2813, DATA_PIN>(leds, NUM_LEDS);

  webSocket.begin();
  webSocket.onEvent(webSocketEvent);
}

void loop() {
  webSocket.loop();

  /*
  if (taskCommand.equals("white")) {
    leds[0] = CRGB::White;
    leds[1] = CRGB::White;
    leds[2] = CRGB::White;
    FastLED.show();
    taskCommand = "";
  }

  if (taskCommand.equals("off")) {
    fill_solid(leds, NUM_LEDS, CRGB::Black);
    FastLED.show();
    taskCommand = "";
  }

  if (taskCommand.equals("blink")) {
    leds[0] = CRGB::Black;
    leds[1] = CRGB::Black;
    leds[2] = CRGB::Black;
    FastLED.show();
    delay(500);
    leds[0] = CRGB::Red;
    leds[1] = CRGB::Blue;
    leds[2] = CRGB::Green;
    FastLED.show();
    delay(500);
  }

  if (taskCommand.equals("rainbow")) {
    fill_solid(leds, NUM_LEDS, CHSV(hue, 255, 255));
    FastLED.show();
    hue++;
    delay(20);
  }

  if (taskCommand.equals("test")) {
    fill_solid(leds, NUM_LEDS, CHSV(random(255), 255, 255));
    FastLED.show();
    delay(50);
  }
  */

  // delay(1);
}