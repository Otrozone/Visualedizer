#ifndef DEVICES_H
#define DEVICES_H

#include <cstdint>

// WS2815 use as WS2813

#ifdef ID_FURNITURE
  #define DVC_STRIP_COUNT 1
  constexpr uint16_t DVC_DATA_PIN_LIST[] = {3};
  constexpr int DVC_NUM_LEDS_LIST[] = {218};
  constexpr uint16_t DVC_LED_OFFSET_LIST[] = {0};
  constexpr uint16_t DVC_LED_SHIFT_LIST[] = {0};
  #define DVC_DMX_UNIVERSE 1
  #define DVC_LED_TYPE WS2813 
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_DESK 
  // #define DVC_DATA_PIN 3
  // #define DVC_NUM_LEDS 274 // (93 + 44) * 2
  // #define DVC_NUM_LEDS 30 // (93 + 44) * 2
  constexpr int DVC_NUM_LEDS_LIST[] = {30, 274, 30};
  constexpr uint16_t DVC_DATA_PIN_LIST[] = {2, 3, 4};
  constexpr uint16_t DVC_LED_OFFSET_LIST[] = {0, 137, 0};
  constexpr uint16_t DVC_LED_SHIFT_LIST[] = {0, 0, 0};
  // constexpr int DVC_NUM_LEDS_LIST[] = {274};
  // constexpr uint16_t DVC_DATA_PIN_LIST[] = {3};
  #define DVC_STRIP_COUNT 3 //sizeof(DVC_NUM_LEDS_LIST) / sizeof(DVC_NUM_LEDS_LIST[0]);
  // Max strip count is 5
  
  // constexpr uint16_t DVC_LED_OFFSET_LIST[] = {184, 184};
  #define DVC_DMX_UNIVERSE 2
  #define DVC_LED_TYPE WS2813
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_CEILING
  #define DVC_STRIP_COUNT 1
  constexpr uint16_t DVC_DATA_PIN_LIST[] = {3};
  constexpr int DVC_NUM_LEDS_LIST[] = {265};
  constexpr uint16_t DVC_LED_OFFSET_LIST[] = {0};
  constexpr uint16_t DVC_LED_SHIFT_LIST[] = {0};
  #define DVC_DMX_UNIVERSE 3
  #define DVC_LED_TYPE WS2812B
  #define DVC_LED_COLOR_ORDER GRB
#endif
#ifdef ID_KUBIS
  #define DVC_STRIP_COUNT 1
  constexpr uint16_t DVC_DATA_PIN_LIST[] = {2};
  constexpr int DVC_NUM_LEDS_LIST[] = {300};
  constexpr uint16_t DVC_LED_OFFSET_LIST[] = {0};
  constexpr uint16_t DVC_LED_SHIFT_LIST[] = {0};
  #define DVC_DMX_UNIVERSE 1
  #define DVC_LED_TYPE WS2813
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_WARDROBE
  #define DVC_STRIP_COUNT 1
  // #define DVC_DATA_PIN 4 // D2
  // #define DVC_NUM_LEDS 269

  constexpr int DVC_NUM_LEDS_LIST[] = {30};
  constexpr uint16_t DVC_DATA_PIN_LIST[] = {4};
  constexpr uint16_t DVC_LED_OFFSET_LIST[] = {0};
  constexpr uint16_t DVC_LED_SHIFT_LIST[] = {0};

  #define DVC_DMX_UNIVERSE 99
  #define DVC_LED_TYPE WS2813
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_PRINTER
  #define DVC_STRIP_COUNT 1
  constexpr uint16_t DVC_DATA_PIN_LIST[] = {3};
  constexpr int DVC_NUM_LEDS_LIST[] = {60};
  constexpr uint16_t DVC_LED_OFFSET_LIST[] = {0};
  constexpr uint16_t DVC_LED_SHIFT_LIST[] = {0};
  #define DVC_DMX_UNIVERSE 1
  #define DVC_LED_TYPE WS2812B
  #define DVC_LED_COLOR_ORDER RGB
#endif

#endif
