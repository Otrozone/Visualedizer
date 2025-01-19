#ifndef DMX_H
#define DMX_H

#ifdef ESP32S3
  #include <ESPAsyncE131.h>
#endif

extern const uint16_t DMX_UNIVERSE_COUNT;

void initDmx();
void processDmx();

#endif // DMX_H