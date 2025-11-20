#include "main.h"
#include "controller.h"
#include "effects.h"

unsigned long menuTimeout = 0;

ControllerConf controllerConfig;
bool lightState = false;

OperationMode operationMode = MODE_MENU_INACTIVE;

int calculateByteValue(int value, int min, int max) {
    return (255 * (value - min)) / (max - min);
}

void ctrlLightOn() {
    Serial.println("Light on");

    int hueVal = calculateByteValue(controllerConfig.hue.value, controllerConfig.hue.min, controllerConfig.hue.max);
    int satVal = calculateByteValue(controllerConfig.saturation.value, controllerConfig.saturation.min, controllerConfig.saturation.max);
    int valVal = calculateByteValue(controllerConfig.value.value, controllerConfig.value.min, controllerConfig.value.max);

    CRGB color = CHSV(hueVal, satVal, valVal);
    int duration = 1000; // milliseconds

    runEffectBlend(color, duration);

    lightState = true;
}

void ctrlLightOff() {
    Serial.println("Light off");
    
    runEffectFadeOut(1000);

    lightState = false;
}

void switchLight() {
    Serial.println("Switch light");
    if (lightState) {
        ctrlLightOff();
    } else {
        ctrlLightOn();
    }
}

void drawOptionSegment(ControllerOption ctrlOpt) {
    Serial.println("Draw option segment");
    int segmentStart = 0;
    int segmentEnd = 0;
    int segmentCount = ctrlOpt.max - ctrlOpt.min;

    switch (ctrlOpt.mode) {
    case OPTMODE_COLOR:
        segmentStart = 0;
        segmentEnd = ledCount;
        break;
    case OPTMODE_SATURATION:
    case OPTMODE_SEGMENT:
        segmentStart = round(ledCount * normalize(ctrlOpt.value - 1, ctrlOpt.min - 1 , ctrlOpt.max));
        segmentEnd = round(ledCount * normalize(ctrlOpt.value, ctrlOpt.min - 1, ctrlOpt.max));
        break;
    case OPTMODE_VALUE:
    case OPTMODE_RANGE:
        segmentStart = 0;
        segmentEnd = round(ledCount * normalize(ctrlOpt.value, ctrlOpt.min, ctrlOpt.max));
        break;
    }

    Serial.println("Segment start: " + String(segmentStart));
    Serial.println("Segment end: " + String(segmentEnd));

    int hueVal;
    switch (ctrlOpt.mode) {
    case OPTMODE_SATURATION:
    case OPTMODE_COLOR:
        hueVal = calculateByteValue(controllerConfig.hue.value, controllerConfig.hue.min, controllerConfig.hue.max);
        break;
    case OPTMODE_VALUE:
        hueVal = controllerConfig.hue.value;
    default:
        hueVal = 0;
    }

    int satVal;
    switch (ctrlOpt.mode) {
        case OPTMODE_SATURATION:
            satVal = calculateByteValue(controllerConfig.saturation.value, controllerConfig.saturation.min, controllerConfig.saturation.max);
            break;
        case OPTMODE_COLOR:
            satVal = 255;
            break;
        default:
            satVal = 0;
            break;
    }

    int valVal;
    switch (ctrlOpt.mode) {
        case OPTMODE_VALUE:
            valVal = calculateByteValue(controllerConfig.value.value, controllerConfig.value.min, controllerConfig.value.max);
            break;
        default:
            valVal = 255;
            break;
    }

    Serial.println("HSV: " + String(hueVal) + " " + String(satVal) + " " + String(valVal));
    
    fill_solid(leds, ledCount, CRGB::Black);
    for (int i = segmentStart; i < segmentEnd; i++) {
        leds[i] = CHSV(hueVal, satVal, valVal);
    }
}

void drawOption() {
    Serial.println("Draw option");
    switch (operationMode) {
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

void enterOptions() {
    Serial.println("Enter options");
    operationMode = MODE_MENU_HUE;

    runEffectMid2Out(CRGB::White, 1000);
    delay(1000);

    drawOption();
}

void finishOptions() {
    Serial.println("Finish options");
    operationMode = MODE_MENU_INACTIVE;

    runEffectOut2Mid(CRGB::White, 1000);
    delay(1000);

    ctrlLightOn();
}

void confirmOption() {
    Serial.println("Confirm option");
    switch (operationMode) {
    case MODE_MENU_HUE:
        // controllerConfig.hue.value = min(controllerConfig.hue.value + 1, controllerConfig.hue.max);
        Serial.println("Hue: " + String(controllerConfig.hue.value));
        operationMode = MODE_MENU_SATURATION;
        drawOption();
        break;
    case MODE_MENU_SATURATION:
        // controllerConfig.saturation.value = min(controllerConfig.saturation.value + 1, controllerConfig.saturation.max);
        Serial.println("Saturation: " + String(controllerConfig.saturation.value));
        operationMode = MODE_MENU_VALUE;
        drawOption();
        break;
    case MODE_MENU_VALUE:
        // controllerConfig.value.value = min(controllerConfig.value.value + 1, controllerConfig.value.max);
        Serial.println("Value: " + String(controllerConfig.value.value));
        finishOptions();
        break;
    }
}

void incrementOption() {
    Serial.println("Increment option");
    switch (operationMode) {
    case MODE_MENU_HUE:
        controllerConfig.hue.value = min(controllerConfig.hue.value + 1, controllerConfig.hue.max);
        break;
    case MODE_MENU_SATURATION:
        controllerConfig.saturation.value = min(controllerConfig.saturation.value + 1, controllerConfig.saturation.max);
        break;
    case MODE_MENU_VALUE:
        controllerConfig.value.value = min(controllerConfig.value.value + 1, controllerConfig.value.max);
        break;
    }
    drawOption();
}

void decrementOption() {
    Serial.println("Decrement option");
    switch (operationMode) {
    case MODE_MENU_HUE:
        controllerConfig.hue.value = max(controllerConfig.hue.value - 1, controllerConfig.hue.min);
        break;
    case MODE_MENU_SATURATION:
        controllerConfig.saturation.value = max(controllerConfig.saturation.value - 1, controllerConfig.saturation.min);
        break;
    case MODE_MENU_VALUE:
        controllerConfig.value.value = max(controllerConfig.value.value - 1, controllerConfig.value.min);
        break;
    }
    drawOption();
}

void ctrlOk() {
    if (operationMode == MODE_MENU_INACTIVE) {
        switchLight();
    } else {
        confirmOption();
    }
}

void ctrlMenu() {
    if (operationMode == MODE_MENU_INACTIVE) {
        enterOptions();
    } else {
        finishOptions();
    }
}

void ctrlPlus() {
    if (operationMode != MODE_MENU_INACTIVE) {
        incrementOption();
    }
}

void ctrlMinus() {
    if (operationMode != MODE_MENU_INACTIVE) {
        decrementOption();
    }
}

void handleCtrlSignal(String signal) {
    if (signal.length() > 0) {
        if (signal == "btn1" || signal == "press") {
            ctrlOk();
        }

        if (signal == "btn2" || signal == "hold") {
            ctrlMenu();
        }

        if (signal == "inc" || signal == "cw") {
            ctrlPlus();
        }

        if (signal == "dec" || signal == "ccw") {
            ctrlMinus();
        }
    }
}

void handleCtrlSignalHttp(AsyncWebServerRequest *request) {
    AsyncWebParameter* command = request->getParam("signal", false, false);
    String signal = command->value();

    handleCtrlSignal(signal);

    request->send(200, "text/html", "Received signal: " + signal);
}

void handleCtrlSignalWs(String queryStr) {
    String signal = getQueryParameterValue(queryStr, "signal");
    Serial.println("Received signal: " + signal);
    handleCtrlSignal(signal);
}