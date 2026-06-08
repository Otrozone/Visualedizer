#ifndef AUXILIARY_OUTPUT_H
#define AUXILIARY_OUTPUT_H

#include <cstddef>
#include <cstdint>

bool isAuxiliaryBinaryFrame(const uint8_t* payload, size_t length);
void initAuxiliaryOutputs();
void applyAuxiliaryBinaryFrame(const uint8_t* payload, size_t length);
void clearAuxiliaryOutputs();

#endif  // AUXILIARY_OUTPUT_H
