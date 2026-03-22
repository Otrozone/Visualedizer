#ifndef CONTROLLER_H
#define CONTROLLER_H

#include <ESPAsyncWebServer.h>

enum OperationMode {
    MODE_MENU_INACTIVE = 0,
    MODE_MENU_HUE = 1,
    MODE_MENU_SATURATION = 2,
    MODE_MENU_VALUE = 3,
    MODE_MENU_TIMEOUT = 4
};

enum OptionMode {
    OPTMODE_SEGMENT = 0,
    OPTMODE_RANGE = 1,
    OPTMODE_COLOR = 2,
    OPTMODE_SATURATION = 3,
    OPTMODE_VALUE = 4
};

struct ControllerOption {
    OptionMode mode;
    int min;
    int max;
    int value;
};

/*struct ControllerConf {
    ControllerOption hue;
    ControllerOption saturation;
    ControllerOption value;
};*/

struct ControllerConf {
    ControllerOption hue = {OPTMODE_COLOR, 1, 12, 0};
    ControllerOption saturation = {OPTMODE_SATURATION, 1, 9, 9};
    ControllerOption value = {OPTMODE_VALUE, 0, 10, 10};
};

extern OperationMode operationMode;

void handleCtrlSignalHttp(AsyncWebServerRequest *request);
void handleCtrlSignalWs(String queryStr);

void switchLight();
void ctrlOk();
void ctrlMenu();
void ctrlPlus();
void ctrlMinus();
void ctrlMute();

void ctrlRed();
void ctrlGreen();
void ctrlYellow();
void ctrlBlue();

void ctrlBtn0();
void ctrlBtn1();
void ctrlBtn2();
void ctrlBtn3();
void ctrlBtn4();
void ctrlBtn5();
void ctrlBtn6();
void ctrlBtn7();
void ctrlBtn8();
void ctrlBtn9();

#endif