#include "main.h"
#include "controller.h"
#include "common.h"
#include "effects.h"
#include "render_service.h"

unsigned long menuTimeout = 0;

ControllerConf controllerConfig;
bool lightState = false;

OperationMode operationMode = MODE_MENU_INACTIVE;

int calculateByteValue(int value, int min, int max) {
    return (255 * (value - min)) / (max - min);
}

static uint8_t hueDegreesToFastLed(int degrees) {
    return mapRange(degrees, 0, 360, 0, 255);
}

void lightRgbColor(CRGB color) {
    int duration = 1000; // milliseconds

    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectBlend(color, duration);
    });

    lightState = true;
}

void ctrlLightOn() {
    Serial.println("Light on");

    int hueVal = calculateByteValue(controllerConfig.hue.value, controllerConfig.hue.min, controllerConfig.hue.max);
    int satVal = calculateByteValue(controllerConfig.saturation.value, controllerConfig.saturation.min, controllerConfig.saturation.max);
    int valVal = calculateByteValue(controllerConfig.value.value, controllerConfig.value.min, controllerConfig.value.max);

    CRGB color = CHSV(hueVal, satVal, valVal);
    lightRgbColor(color);
}

void ctrlLightOff() {
    Serial.println("Light off");

    forEachLedStrip([](LedStripDvc& dvc) {
        dvc.runEffectFadeOut(1000);
    });

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

    forEachLedStrip([&](LedStripDvc& dvc) {
        const int ledCount = dvc.ledCount;
        int segmentStart = 0;
        int segmentEnd = 0;

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

        requestFillRangeHSV(dvc.ledIdx, segmentStart, segmentEnd, CHSV(hueVal, satVal, valVal));
    });
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
}

void enterOptions() {
    Serial.println("Enter options");
    operationMode = MODE_MENU_HUE;

    forEachLedStrip([](LedStripDvc& dvc) {
        dvc.runEffectMid2Out(CRGB::White, 1000);
    });
    delay(1000);

    drawOption();
}

void finishOptions() {
    Serial.println("Finish options");
    operationMode = MODE_MENU_INACTIVE;

    forEachLedStrip([](LedStripDvc& dvc) {
        dvc.runEffectOut2Mid(CRGB::White, 1000);
    });
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

void ctrlRed() {
    lightRgbColor(CRGB(123, 14, 14));
}

void ctrlGreen() {
    lightRgbColor(CRGB(14, 123, 14));
}

void ctrlYellow() {
    lightRgbColor(CRGB(23, 12, 2));
}

void ctrlBlue() {
    lightRgbColor(CRGB(14, 14, 123));
}

void ctrlBtn0() {
    lightRgbColor(CRGB(14, 14, 123));
}

void ctrlBtn1() {
    forEachLedStrip([](LedStripDvc& dvc) {
        dvc.runEffectRunningRainbow(50, 4, 7);
    });
    lightState = true;
}

void ctrlBtn2() {
    CHSV chsvStart;
    chsvStart.hue = hueDegreesToFastLed(360);
    chsvStart.value = 50;
    chsvStart.saturation = 255;

    CHSV chsvEnd;
    chsvEnd.hue = hueDegreesToFastLed(285);
    chsvEnd.value = 50;
    chsvEnd.saturation = 255;

    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(360), 255, 50);
    color2 = CHSV(hueDegreesToFastLed(285), 255, 50);

    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.fillGradientHSV(chsvStart, chsvEnd);
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlBtn3() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(0), 255, 50);
    color2 = CHSV(hueDegreesToFastLed(128), 255, 50);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlBtn4() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(128), 200, 200);
    color2 = CHSV(hueDegreesToFastLed(255), 200, 200);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlBtn5() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(0), 255, 50);
    color2 = CHSV(hueDegreesToFastLed(25), 255, 50);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;

}

void ctrlBtn6() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(140), 255, 50);
    color2 = CHSV(hueDegreesToFastLed(225), 255, 50);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlBtn7() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(205), 255, 100);
    color2 = CHSV(hueDegreesToFastLed(180), 255, 100);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlBtn8() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(145), 255, 200);
    color2 = CHSV(hueDegreesToFastLed(320), 255, 200);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlBtn9() {
    CRGB color1;
    CRGB color2;
    color1 = CHSV(hueDegreesToFastLed(128), 180, 180);
    color2 = CHSV(hueDegreesToFastLed(255), 180, 180);
    forEachLedStrip([&](LedStripDvc& dvc) {
        dvc.runEffectRunningGradient(color1, color2, 50, 0.03f);
    });
    lightState = true;
}

void ctrlMute() {
    lightRgbColor(CRGB(5, 1, 0));
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
