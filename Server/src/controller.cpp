#include <main.h>
#include <controller.h>
#include <modes.h>

unsigned long menuTimeout = 0;

ControllerConf controllerConfig;
bool lightState = false;

void ctrlLightOn() {
    terminateCurrTask();

    TaskBlendParams *params = new TaskBlendParams;
    params->leds = leds;
    params->ledCount = ledCount;
    int hueVal = (255 * (controllerConfig.hue.value - controllerConfig.hue.min)) / (controllerConfig.hue.max - controllerConfig.hue.min);
    int satVal = (255 * (controllerConfig.saturation.value - controllerConfig.saturation.min)) / (controllerConfig.saturation.max - controllerConfig.saturation.min);
    int valVal = (255 * (controllerConfig.value.value - controllerConfig.value.min)) / (controllerConfig.value.max - controllerConfig.value.min);

    params->color = CHSV(hueVal, satVal, valVal);
    params->duration = 1000; // milliseconds

    xTaskCreate(taskBlend, "BlendTask", 2048, params, 1, NULL);

    lightState = true;
}

void ctrlLightOff() {
    terminateCurrTask();

    TaskFadeParams *params = new TaskFadeParams;
    params->leds = leds;
    params->duration = 1000;
    params->ledCount = ledCount;
    params->color = CRGB::Black;

    xTaskCreate(taskFadeOut, "FadeOutTask", 2048, params, 1, NULL);

    lightState = false;
}

void switchLight() {
    if (lightState) {
        ctrlLightOff();
    } else {
        ctrlLightOn();
    }
}

void drawOptionSegment(ControllerOption ctrlOpt) {
    int segmentStart = 0;
    int segmentEnd = 0;
    int segmentCount = ctrlOpt.max - ctrlOpt.min;

    switch (mode) {
    case OPTMODE_COLOR:
    case OPTMODE_SEGMENT:
        segmentStart = round(ledCount * normalize(ctrlOpt.value, ctrlOpt.min, ctrlOpt.max));
        segmentEnd = round(ledCount * normalize(ctrlOpt.value + 1, ctrlOpt.min, ctrlOpt.max));
        break;
    case OPTMODE_RANGE:
        segmentStart = 0;
        segmentEnd = round(ledCount * normalize(ctrlOpt.value, ctrlOpt.min, ctrlOpt.max));
        break;
    }

    Serial.println("Segment start: " + String(segmentStart));
    Serial.println("Segment end: " + String(segmentEnd));

    int hueVal;
    int satVal;
    int valVal = 255;
    switch (mode) {
    case OPTMODE_COLOR:
        hueVal = (255 * (controllerConfig.hue.value - controllerConfig.hue.min)) / (controllerConfig.hue.max - controllerConfig.hue.min);
        satVal = 255;
        break;
    default:
        hueVal = 0;
        satVal = 0;
    }
    Serial.println("HSV: " + String(hueVal) + " " + String(satVal) + "" + String(valVal));
    
    fill_solid(leds, ledCount, CRGB::Black);
    for (int i = segmentStart; i < segmentEnd; i++) {
        leds[i] = CHSV(hueVal, satVal, valVal);
    }
}

void drawOption() {
    switch (mode) {
    case MODE_MENU_HUE:
        drawOptionSegment(controllerConfig.hue);
        Serial.println("Hue: " + String(controllerConfig.hue.value));
        break;
    case MODE_MENU_SATURATION:
        drawOptionSegment(controllerConfig.saturation);
        Serial.println("Saturation: " + String(controllerConfig.saturation.value));
        break;
    case MODE_MENU_VALUE:
        drawOptionSegment(controllerConfig.value);
        Serial.println("Value: " + String(controllerConfig.value.value));
        break;
    }

    FastLedShow();
}

void finishOptions() {
    mode = MODE_MENU_INACTIVE;
    ctrlLightOn();
}

void confirmOption() {
    mode = MODE_MENU_INACTIVE;
    switch (mode) {
    case MODE_MENU_HUE:
        controllerConfig.hue.value = min(controllerConfig.hue.value + 1, controllerConfig.hue.max);
        mode = MODE_MENU_SATURATION;
        drawOption();
        break;
    case MODE_MENU_SATURATION:
        controllerConfig.saturation.value = min(controllerConfig.saturation.value + 1, controllerConfig.saturation.max);
        mode = MODE_MENU_VALUE;
        drawOption();
        break;
    case MODE_MENU_VALUE:
        controllerConfig.value.value = min(controllerConfig.value.value + 1, controllerConfig.value.max);
        finishOptions();
        break;
    }
}

void incrementOption() {
    switch (mode) {
    case MODE_MENU_HUE:
        controllerConfig.hue.value = min(controllerConfig.hue.value + 1, controllerConfig.hue.max);
        break;
    case MODE_MENU_SATURATION:
        controllerConfig.saturation.value = min(controllerConfig.saturation.value + 1, controllerConfig.saturation.max);
        break;
    case MODE_MENU_VALUE:
        controllerConfig.value.value = min(controllerConfig.value.value + 1, controllerConfig.value.max);
        switchLight();
        break;
    }
    drawOption();
}

void decrementOption() {
    switch (mode) {
    case MODE_MENU_HUE:
        controllerConfig.hue.value = max(controllerConfig.hue.value - 1, controllerConfig.hue.min);
        break;
    case MODE_MENU_SATURATION:
        controllerConfig.saturation.value = max(controllerConfig.saturation.value - 1, controllerConfig.saturation.min);
        break;
    case MODE_MENU_VALUE:
        controllerConfig.value.value = max(controllerConfig.value.value - 1, controllerConfig.value.min);
        switchLight();
        break;
    }
    drawOption();
}

void handleCtrlCmd(AsyncWebServerRequest *request) {
    AsyncWebParameter* command = request->getParam("signal", false, false);

    String signal = command->value();

    if (signal.length() > 0) {
        if (signal == "btn1" || signal == "push") {
            if (mode == MODE_MENU_INACTIVE) {
                switchLight();
            } else {
                confirmOption();
            }
        }

        if (signal == "btn2" || signal == "hold") {
            finishOptions();
        }

        if (signal == "inc" || signal == "cw") {
            incrementOption();
        }

        if (signal == "dec" || signal == "ccw") {
            decrementOption();
        }
    }

    request->send(200, "text/html", "Received signal: ");
}