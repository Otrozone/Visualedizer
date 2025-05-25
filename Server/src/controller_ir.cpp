#include <controller_ir.h>
#include <IRrecv.h>
#include <IRutils.h>

#include "controller.h"

#define IR_RECV_PIN 3

IRrecv irrecv(IR_RECV_PIN);
decode_results results;

enum IrBtnMapping {
    IR_BTN_UP = 0x807F609F,
    IR_BTN_RIGHT = 0x807FD827,
    IR_BTN_DOWN = 0x807F6897,
    IR_BTN_LEFT = 0x807F5AA5,
    IR_BTN_OK = 0x807F58A7,
    IR_BTN_PLUS = 0x807F708F,
    IR_BTN_MINUS = 0x807F48B7,
    IR_BTN_POWER = 0x807F9A65,
    IR_BTN_INFO = 0x807F1AE5,
    IR_BTN_SLEEP = 0x807F18E7,
    IR_BTN_MUTE = 0x807F9867,
    IR_BTN_MENU = 0x807FA25D,
    IR_BTN_EXIT = 0x807FA05F,
    IR_BTN_GOTO = 0x807FAA55,
    IR_BTN_FAV = 0x807FA857,
    IR_BTN_1 = 0x807F4AB5,
    IR_BTN_2 = 0x807F0AF5,
    IR_BTN_3 = 0x807F08F7,
    IR_BTN_4 = 0x807F6A95,
    IR_BTN_5 = 0x807F2AD5,
    IR_BTN_6 = 0x807F28D7,
    IR_BTN_7 = 0x807F728D,
    IR_BTN_8 = 0x807F32CD,
    IR_BTN_9 = 0x807F30CF,
    IR_BTN_0 = 0x807FF00F,
    IR_BTN_RED = 0x807F42BD,
    IR_BTN_GREEN = 0x807F02FD,
    IR_BTN_YELLOW = 0x807F00FF,
    IR_BTN_BLUE = 0x807FC03F
};

void initIr() {
    if (irEnabled) {
        irrecv.enableIRIn();
        Serial.println("IR receiver initialization done");
    } else {
        Serial.println("IR receiver disabled");
        return;
    }
}


void processIrResult(decode_results *results) {
    if (results->repeat) {
        // Serial.println("Repeat signal received");
        return;
    }
    
    switch (results->value) {
        case IR_BTN_POWER:
            Serial.println("Button POWER pressed");
            switchLight();
            break;
        case IR_BTN_UP:
            Serial.println("Button UP pressed");
            ctrlPlus();
            break;
        case IR_BTN_RIGHT:
            Serial.println("Button RIGHT pressed");
            ctrlPlus();
            break;
        case IR_BTN_DOWN:
            Serial.println("Button DOWN pressed");
            ctrlMinus();
            break;
        case IR_BTN_LEFT:
            Serial.println("Button LEFT pressed");
            ctrlMinus();
            break;
        case IR_BTN_OK:
            Serial.println("Button OK pressed");
            ctrlOk();
            break;
        case IR_BTN_MENU:
            Serial.println("Button MENU pressed");
            ctrlMenu();
            break;
        default:
            Serial.println("Unknown button pressed");
            break;
    }
}

void processIr() {
    if (irrecv.decode(&results)) {
        Serial.println(resultToHumanReadableBasic(&results));
        processIrResult(&results);
        irrecv.resume();
    }
}