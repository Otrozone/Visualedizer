#ifndef DEVICES_H
#define DEVICES_H

// WS2815 use as WS2813

#ifdef ID_FURNITURE
  #define DVC_DATA_PIN 3
  #define DVC_NUM_LEDS 218 // 109 * 2
  #define DVC_OFFSET 0
  #define DVC_DMX_UNIVERSE 1
  #define DVC_LED_TYPE WS2813 
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_DESK 
  #define DVC_DATA_PIN 3
  #define DVC_NUM_LEDS 274 // (93 + 44) * 2
  #define DVC_OFFSET 184
  #define DVC_DMX_UNIVERSE 2
  #define DVC_LED_TYPE WS2813
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_CEILING
  #define DVC_DATA_PIN 3
  #define DVC_NUM_LEDS 265
  #define DVC_OFFSET 0
  #define DVC_DMX_UNIVERSE 3
  #define DVC_LED_TYPE WS2812B
  #define DVC_LED_COLOR_ORDER GRB
#endif
#ifdef ID_KUBIS
  #define DVC_DATA_PIN 2
  #define DVC_NUM_LEDS 300
  #define DVC_OFFSET 0
  #define DVC_DMX_UNIVERSE 1
  #define DVC_LED_TYPE WS2813
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_WARDROBE
  #define DVC_DATA_PIN 4 // D2
  #define DVC_NUM_LEDS 269
  #define DVC_OFFSET 0
  #define DVC_DMX_UNIVERSE 99
  #define DVC_LED_TYPE WS2813
  #define DVC_LED_COLOR_ORDER RGB
#endif
#ifdef ID_PRINTER
  #define DVC_DATA_PIN 3
  #define DVC_NUM_LEDS 60
  #define DVC_OFFSET 0
  #define DVC_DMX_UNIVERSE 1
  #define DVC_LED_TYPE WS2812B
  #define DVC_LED_COLOR_ORDER RGB
#endif

#endif