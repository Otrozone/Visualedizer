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

OperationMode mode = MODE_MENU_INACTIVE;

enum OptionMode {
    OPTMODE_SEGMENT = 0,
    OPTMODE_RANGE = 1,
    OPTMODE_COLOR = 2
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
    ControllerOption hue = {OPTMODE_SEGMENT, 0, 9, 0};
    ControllerOption saturation = {OPTMODE_SEGMENT, 0, 9, 9};
    ControllerOption value = {OPTMODE_SEGMENT, 1, 10, 10};
};

void handleCtrlCmd(AsyncWebServerRequest *request);

#endif