#include "render_service.h"

#include <cmath>
#include <cstring>

#include <freertos/FreeRTOS.h>
#include <freertos/queue.h>
#include <freertos/task.h>

#include "led.h"

namespace {
constexpr uint32_t kRenderTaskStackSize = 6144;
constexpr uint32_t kRenderTaskPriority = 10;
constexpr uint32_t kRenderTickMs = 10;
constexpr uint32_t kRenderQueueLength = 16;

enum class RenderCommandType {
  Abort,
  FillSolid,
  FillGradient,
  FillRangeHSV,
  FillSection,
  Off,
  ApplyBinaryFrame,
  EffectStrobe,
  EffectStrobeRandom,
  EffectRunningRainbow,
  EffectRunningGradient,
  EffectNoise,
  EffectBlend,
  EffectFadeIn,
  EffectFadeOut,
  EffectMid2Out,
  EffectOut2Mid,
};

enum class EffectType {
  None,
  Strobe,
  StrobeRandom,
  RunningRainbow,
  RunningGradient,
  Noise,
  Blend,
  FadeIn,
  FadeOut,
  Mid2Out,
  Out2Mid,
};

struct RenderCommand {
  RenderCommandType type;
  int stripIdx;
  CRGB color1;
  CRGB color2;
  CHSV hsv1;
  CHSV hsv2;
  int intParam1;
  int intParam2;
  int intParam3;
  float floatParam1;
  uint8_t* payload;
  size_t payloadLength;
};

struct StripRenderState {
  EffectType effect;
  uint32_t effectStartMs;
  uint32_t lastUpdateMs;
  CRGB color1;
  CRGB color2;
  int delay1;
  int delay2;
  int step;
  int delta;
  int duration;
  float shift;
  float stepFloat;
  uint8_t hue;
  bool phaseOn;
  uint32_t currentDelayMs;
  uint16_t noiseTime;
  uint16_t noiseX;
  CRGB* transitionStart;
};

QueueHandle_t renderQueue = nullptr;
TaskHandle_t renderTaskHandle = nullptr;
StripRenderState stripStates[DVC_STRIP_COUNT];

bool isValidStripIdx(int stripIdx) {
  return stripIdx >= 0 && stripIdx < DVC_STRIP_COUNT && ledStrips[stripIdx] != nullptr;
}

void resetState(int stripIdx) {
  if (!isValidStripIdx(stripIdx)) {
    return;
  }

  StripRenderState& state = stripStates[stripIdx];
  state.effect = EffectType::None;
  state.effectStartMs = 0;
  state.lastUpdateMs = 0;
  state.shift = 0.0f;
  state.phaseOn = false;
  state.currentDelayMs = 0;
}

void copyCurrentStripToTransitionStart(int stripIdx) {
  if (!isValidStripIdx(stripIdx)) {
    return;
  }

  LedStripDvc* dvc = ledStrips[stripIdx];
  StripRenderState& state = stripStates[stripIdx];
  if (state.transitionStart == nullptr) {
    return;
  }

  for (int i = 0; i < dvc->ledCount; i++) {
    state.transitionStart[i] = dvc->leds[i];
  }
}

void fillSection(LedStripDvc* dvc, int sectionCount, int sectionIdx, CRGB color) {
  fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);

  if (sectionCount <= 0) {
    return;
  }

  int sectionLength = dvc->ledCount / sectionCount;
  int startIndex = sectionIdx * sectionLength;
  int endIndex = startIndex + sectionLength;

  for (int i = startIndex; i < endIndex && i < dvc->ledCount; i++) {
    if (i >= 0) {
      dvc->leds[i] = color;
    }
  }
}

void applyCommandToStrip(const RenderCommand& cmd, int stripIdx, uint32_t now, bool& needsShow) {
  if (!isValidStripIdx(stripIdx)) {
    return;
  }

  LedStripDvc* dvc = ledStrips[stripIdx];
  StripRenderState& state = stripStates[stripIdx];

  switch (cmd.type) {
    case RenderCommandType::Abort:
      resetState(stripIdx);
      break;
    case RenderCommandType::FillSolid:
      resetState(stripIdx);
      fill_solid(dvc->leds, dvc->ledCount, cmd.color1);
      needsShow = true;
      break;
    case RenderCommandType::FillGradient:
      resetState(stripIdx);
      fill_gradient_HSV(dvc->leds, dvc->ledCount, cmd.hsv1, cmd.hsv2, FORWARD_HUES);
      needsShow = true;
      break;
    case RenderCommandType::FillRangeHSV:
      resetState(stripIdx);
      fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
      for (int i = cmd.intParam1; i < cmd.intParam2 && i < dvc->ledCount; i++) {
        if (i >= 0) {
          dvc->leds[i] = cmd.hsv1;
        }
      }
      needsShow = true;
      break;
    case RenderCommandType::FillSection:
      resetState(stripIdx);
      fillSection(dvc, cmd.intParam1, cmd.intParam2, cmd.color1);
      needsShow = true;
      break;
    case RenderCommandType::Off:
      resetState(stripIdx);
      fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
      needsShow = true;
      break;
    case RenderCommandType::EffectStrobe:
      resetState(stripIdx);
      state.effect = EffectType::Strobe;
      state.color1 = cmd.color1;
      state.delay1 = cmd.intParam1;
      state.delay2 = cmd.intParam2;
      state.phaseOn = true;
      state.lastUpdateMs = now;
      state.effectStartMs = now;
      fill_solid(dvc->leds, dvc->ledCount, state.color1);
      needsShow = true;
      break;
    case RenderCommandType::EffectStrobeRandom:
      resetState(stripIdx);
      state.effect = EffectType::StrobeRandom;
      state.color1 = cmd.color1;
      state.phaseOn = true;
      state.currentDelayMs = random(1, 500);
      state.lastUpdateMs = now;
      state.effectStartMs = now;
      fill_solid(dvc->leds, dvc->ledCount, state.color1);
      needsShow = true;
      break;
    case RenderCommandType::EffectRunningRainbow:
      resetState(stripIdx);
      state.effect = EffectType::RunningRainbow;
      state.delay1 = cmd.intParam1;
      state.step = cmd.intParam2;
      state.delta = cmd.intParam3;
      state.hue = 0;
      state.lastUpdateMs = 0;
      state.effectStartMs = now;
      break;
    case RenderCommandType::EffectRunningGradient:
      resetState(stripIdx);
      state.effect = EffectType::RunningGradient;
      state.color1 = cmd.color1;
      state.color2 = cmd.color2;
      state.delay1 = cmd.intParam1;
      state.stepFloat = cmd.floatParam1;
      state.shift = 0.0f;
      state.lastUpdateMs = 0;
      state.effectStartMs = now;
      break;
    case RenderCommandType::EffectNoise:
      resetState(stripIdx);
      state.effect = EffectType::Noise;
      state.noiseTime = millis();
      state.noiseX = 0;
      state.lastUpdateMs = 0;
      state.effectStartMs = now;
      break;
    case RenderCommandType::EffectBlend:
      resetState(stripIdx);
      state.effect = EffectType::Blend;
      state.color1 = cmd.color1;
      state.duration = cmd.intParam1;
      state.effectStartMs = now;
      state.lastUpdateMs = now;
      copyCurrentStripToTransitionStart(stripIdx);
      break;
    case RenderCommandType::EffectFadeIn:
      resetState(stripIdx);
      state.effect = EffectType::FadeIn;
      state.color1 = cmd.color1;
      state.duration = cmd.intParam1;
      state.effectStartMs = now;
      state.lastUpdateMs = now;
      fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
      needsShow = true;
      break;
    case RenderCommandType::EffectFadeOut:
      resetState(stripIdx);
      state.effect = EffectType::FadeOut;
      state.duration = cmd.intParam1;
      state.effectStartMs = now;
      state.lastUpdateMs = now;
      copyCurrentStripToTransitionStart(stripIdx);
      break;
    case RenderCommandType::EffectMid2Out:
      resetState(stripIdx);
      state.effect = EffectType::Mid2Out;
      state.color1 = cmd.color1;
      state.duration = cmd.intParam1;
      state.effectStartMs = now;
      state.lastUpdateMs = now;
      fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
      needsShow = true;
      break;
    case RenderCommandType::EffectOut2Mid:
      resetState(stripIdx);
      state.effect = EffectType::Out2Mid;
      state.color1 = cmd.color1;
      state.duration = cmd.intParam1;
      state.effectStartMs = now;
      state.lastUpdateMs = now;
      fill_solid(dvc->leds, dvc->ledCount, state.color1);
      needsShow = true;
      break;
    case RenderCommandType::ApplyBinaryFrame:
      break;
  }
}

bool applyBinaryFrame(const RenderCommand& cmd) {
  if (cmd.payload == nullptr) {
    return false;
  }

  int payloadOffset = 0;
  forEachLedStrip([&](LedStripDvc& dvc) {
    resetState(dvc.ledIdx);
    for (int i = 0; i < dvc.ledCount && payloadOffset + 2 < static_cast<int>(cmd.payloadLength); i++) {
      dvc.leds[i] = CRGB(cmd.payload[payloadOffset], cmd.payload[payloadOffset + 1], cmd.payload[payloadOffset + 2]);
      payloadOffset += 3;
    }
  });

  return true;
}

bool processCommand(const RenderCommand& cmd) {
  const uint32_t now = millis();
  bool needsShow = false;

  if (cmd.type == RenderCommandType::ApplyBinaryFrame) {
    needsShow = applyBinaryFrame(cmd);
    return needsShow;
  }

  if (cmd.stripIdx >= 0) {
    applyCommandToStrip(cmd, cmd.stripIdx, now, needsShow);
    return needsShow;
  }

  forEachLedStrip([&](LedStripDvc& dvc) {
    applyCommandToStrip(cmd, dvc.ledIdx, now, needsShow);
  });
  return needsShow;
}

bool updateRainbow(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.delay1 > 0 && state.lastUpdateMs != 0 && now - state.lastUpdateMs < static_cast<uint32_t>(state.delay1)) {
    return false;
  }

  fill_rainbow(dvc->leds, dvc->ledCount, state.hue, state.delta);
  state.hue += state.step;
  state.lastUpdateMs = now;
  return true;
}

bool updateStrobe(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  const uint32_t waitMs = state.phaseOn ? state.delay1 : state.delay2;
  if (now - state.lastUpdateMs < waitMs) {
    return false;
  }

  state.phaseOn = !state.phaseOn;
  fill_solid(dvc->leds, dvc->ledCount, state.phaseOn ? state.color1 : CRGB::Black);
  state.lastUpdateMs = now;
  return true;
}

bool updateStrobeRandom(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (now - state.lastUpdateMs < state.currentDelayMs) {
    return false;
  }

  state.phaseOn = !state.phaseOn;
  fill_solid(dvc->leds, dvc->ledCount, state.phaseOn ? state.color1 : CRGB::Black);
  state.currentDelayMs = random(1, 500);
  state.lastUpdateMs = now;
  return true;
}

bool updateRunningGradient(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.delay1 > 0 && state.lastUpdateMs != 0 && now - state.lastUpdateMs < static_cast<uint32_t>(state.delay1)) {
    return false;
  }

  const float ledDenominator = dvc->ledCount > 1 ? static_cast<float>(dvc->ledCount - 1) : 1.0f;
  for (int i = 0; i < dvc->ledCount; i++) {
    float t = static_cast<float>(i) / ledDenominator;
    t += state.shift;
    if (t > 1.0f) {
      t -= floorf(t);
    }

    dvc->leds[i] = blend(state.color1, state.color2, static_cast<uint8_t>(t * 255.0f));
  }

  state.shift += state.stepFloat;
  if (state.shift > 1.0f) {
    state.shift -= floorf(state.shift);
  }

  state.lastUpdateMs = now;
  return true;
}

bool updateNoise(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.lastUpdateMs != 0 && now - state.lastUpdateMs < 100) {
    return false;
  }

  const uint8_t octaves = 3;
  const int scale = 80;
  const uint8_t hueOctaves = 2;
  const int hueScale = 80;

  state.noiseX += 1;
  fill_noise8(dvc->leds, dvc->ledCount, octaves, state.noiseX, scale, hueOctaves, state.noiseX, hueScale, state.noiseTime);
  state.lastUpdateMs = now;
  return true;
}

bool updateBlend(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.duration <= 0) {
    fill_solid(dvc->leds, dvc->ledCount, state.color1);
    resetState(dvc->ledIdx);
    return true;
  }

  float progress = static_cast<float>(now - state.effectStartMs) / static_cast<float>(state.duration);
  if (progress > 1.0f) {
    progress = 1.0f;
  }

  uint8_t blendAmount = static_cast<uint8_t>(roundf(progress * 255.0f));
  for (int i = 0; i < dvc->ledCount; i++) {
    dvc->leds[i] = blend(stripStates[dvc->ledIdx].transitionStart[i], state.color1, blendAmount);
  }

  if (progress >= 1.0f) {
    resetState(dvc->ledIdx);
  }

  return true;
}

bool updateFadeIn(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.duration <= 0) {
    fill_solid(dvc->leds, dvc->ledCount, state.color1);
    resetState(dvc->ledIdx);
    return true;
  }

  float progress = static_cast<float>(now - state.effectStartMs) / static_cast<float>(state.duration);
  if (progress > 1.0f) {
    progress = 1.0f;
  }

  uint8_t brightness = static_cast<uint8_t>(roundf(progress * 255.0f));
  for (int i = 0; i < dvc->ledCount; i++) {
    dvc->leds[i] = state.color1;
    dvc->leds[i].fadeLightBy(255 - brightness);
  }

  if (progress >= 1.0f) {
    resetState(dvc->ledIdx);
  }

  return true;
}

bool updateFadeOut(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  const int totalDuration = state.duration * 2;
  if (totalDuration <= 0) {
    fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
    resetState(dvc->ledIdx);
    return true;
  }

  float progress = static_cast<float>(now - state.effectStartMs) / static_cast<float>(totalDuration);
  if (progress > 1.0f) {
    progress = 1.0f;
  }

  const float gamma = 15.0f;
  float factor = powf(1.0f - progress, gamma);
  uint8_t brightness = static_cast<uint8_t>(roundf(factor * 255.0f));
  if (brightness < 150) {
    brightness = 0;
  }

  for (int i = 0; i < dvc->ledCount; i++) {
    dvc->leds[i] = stripStates[dvc->ledIdx].transitionStart[i];
    dvc->leds[i].nscale8_video(brightness);
  }

  if (progress >= 1.0f) {
    fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
    resetState(dvc->ledIdx);
  }

  return true;
}

bool updateMid2Out(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.duration <= 0) {
    fill_solid(dvc->leds, dvc->ledCount, state.color1);
    resetState(dvc->ledIdx);
    return true;
  }

  const int center = dvc->ledCount / 2;
  float progress = static_cast<float>(now - state.effectStartMs) / static_cast<float>(state.duration);
  if (progress > 1.0f) {
    progress = 1.0f;
  }

  int range = roundf(progress * center);
  fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
  for (int j = 0; j <= range; j++) {
    if (center + j < dvc->ledCount) {
      dvc->leds[center + j] = state.color1;
    }
    if (center - j >= 0) {
      dvc->leds[center - j] = state.color1;
    }
  }

  if (progress >= 1.0f) {
    resetState(dvc->ledIdx);
  }

  return true;
}

bool updateOut2Mid(LedStripDvc* dvc, StripRenderState& state, uint32_t now) {
  if (state.duration <= 0) {
    fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
    resetState(dvc->ledIdx);
    return true;
  }

  const int center = dvc->ledCount / 2;
  float progress = static_cast<float>(now - state.effectStartMs) / static_cast<float>(state.duration);
  if (progress > 1.0f) {
    progress = 1.0f;
  }

  int range = center - roundf(progress * center);
  fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
  for (int j = 0; j < center; j++) {
    if (j <= range) {
      if (center + j < dvc->ledCount) {
        dvc->leds[center + j] = state.color1;
      }
      if (center - j >= 0) {
        dvc->leds[center - j] = state.color1;
      }
    }
  }

  if (progress >= 1.0f) {
    fill_solid(dvc->leds, dvc->ledCount, CRGB::Black);
    resetState(dvc->ledIdx);
  }

  return true;
}

bool updateStripAnimation(int stripIdx, uint32_t now) {
  if (!isValidStripIdx(stripIdx)) {
    return false;
  }

  LedStripDvc* dvc = ledStrips[stripIdx];
  StripRenderState& state = stripStates[stripIdx];

  switch (state.effect) {
    case EffectType::None:
      return false;
    case EffectType::Strobe:
      return updateStrobe(dvc, state, now);
    case EffectType::StrobeRandom:
      return updateStrobeRandom(dvc, state, now);
    case EffectType::RunningRainbow:
      return updateRainbow(dvc, state, now);
    case EffectType::RunningGradient:
      return updateRunningGradient(dvc, state, now);
    case EffectType::Noise:
      return updateNoise(dvc, state, now);
    case EffectType::Blend:
      return updateBlend(dvc, state, now);
    case EffectType::FadeIn:
      return updateFadeIn(dvc, state, now);
    case EffectType::FadeOut:
      return updateFadeOut(dvc, state, now);
    case EffectType::Mid2Out:
      return updateMid2Out(dvc, state, now);
    case EffectType::Out2Mid:
      return updateOut2Mid(dvc, state, now);
  }

  return false;
}

void renderTask(void* pvParameters) {
  (void)pvParameters;
  RenderCommand cmd{};

  for (;;) {
    bool needsShow = false;
    if (xQueueReceive(renderQueue, &cmd, pdMS_TO_TICKS(kRenderTickMs)) == pdTRUE) {
      needsShow |= processCommand(cmd);
      if (cmd.type == RenderCommandType::ApplyBinaryFrame) {
        delete[] cmd.payload;
      }

      while (xQueueReceive(renderQueue, &cmd, 0) == pdTRUE) {
        needsShow |= processCommand(cmd);
        if (cmd.type == RenderCommandType::ApplyBinaryFrame) {
          delete[] cmd.payload;
        }
      }
    }

    const uint32_t now = millis();
    forEachLedStrip([&](LedStripDvc& dvc) {
      needsShow |= updateStripAnimation(dvc.ledIdx, now);
    });

    if (needsShow) {
      FastLedShow();
    }
  }
}

bool enqueueCommand(const RenderCommand& command) {
  initRenderService();
  if (renderQueue == nullptr) {
    return false;
  }

  if (xQueueSend(renderQueue, &command, 0) == pdTRUE) {
    return true;
  }

  Serial.println("Render queue full, dropping command");
  if (command.type == RenderCommandType::ApplyBinaryFrame) {
    delete[] command.payload;
  }
  return false;
}

RenderCommand makeCommand(RenderCommandType type, int stripIdx = -1) {
  RenderCommand cmd{};
  cmd.type = type;
  cmd.stripIdx = stripIdx;
  return cmd;
}
}  // namespace

void initRenderService() {
  if (renderQueue == nullptr) {
    renderQueue = xQueueCreate(kRenderQueueLength, sizeof(RenderCommand));
  }

  if (renderTaskHandle == nullptr && renderQueue != nullptr) {
    forEachLedStrip([&](LedStripDvc& dvc) {
      StripRenderState& state = stripStates[dvc.ledIdx];
      if (state.transitionStart == nullptr) {
        state.transitionStart = new CRGB[dvc.ledCount];
      }
      resetState(dvc.ledIdx);
    });

    xTaskCreate(renderTask, "LedRenderTask", kRenderTaskStackSize, nullptr, kRenderTaskPriority, &renderTaskHandle);
  }
}

void requestAbortRender(int stripIdx) {
  enqueueCommand(makeCommand(RenderCommandType::Abort, stripIdx));
}

void requestFillSolid(int stripIdx, CRGB color) {
  RenderCommand cmd = makeCommand(RenderCommandType::FillSolid, stripIdx);
  cmd.color1 = color;
  enqueueCommand(cmd);
}

void requestFillGradientHSV(int stripIdx, CHSV chsvStart, CHSV chsvEnd) {
  RenderCommand cmd = makeCommand(RenderCommandType::FillGradient, stripIdx);
  cmd.hsv1 = chsvStart;
  cmd.hsv2 = chsvEnd;
  enqueueCommand(cmd);
}

void requestFillRangeHSV(int stripIdx, int startIndex, int endIndex, CHSV color) {
  RenderCommand cmd = makeCommand(RenderCommandType::FillRangeHSV, stripIdx);
  cmd.intParam1 = startIndex;
  cmd.intParam2 = endIndex;
  cmd.hsv1 = color;
  enqueueCommand(cmd);
}

void requestFillSection(int stripIdx, int sectionCount, int sectionIdx, CRGB color) {
  RenderCommand cmd = makeCommand(RenderCommandType::FillSection, stripIdx);
  cmd.intParam1 = sectionCount;
  cmd.intParam2 = sectionIdx;
  cmd.color1 = color;
  enqueueCommand(cmd);
}

void requestOff(int stripIdx) {
  enqueueCommand(makeCommand(RenderCommandType::Off, stripIdx));
}

void requestApplyBinaryFrame(const uint8_t* payload, size_t length) {
  RenderCommand cmd = makeCommand(RenderCommandType::ApplyBinaryFrame, -1);
  cmd.payloadLength = length;
  cmd.payload = new uint8_t[length];
  memcpy(cmd.payload, payload, length);
  enqueueCommand(cmd);
}

void requestEffectStrobe(int stripIdx, CRGB color, int delay1, int delay2) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectStrobe, stripIdx);
  cmd.color1 = color;
  cmd.intParam1 = delay1;
  cmd.intParam2 = delay2;
  enqueueCommand(cmd);
}

void requestEffectStrobeRandom(int stripIdx, CRGB color) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectStrobeRandom, stripIdx);
  cmd.color1 = color;
  enqueueCommand(cmd);
}

void requestEffectRunningRainbow(int stripIdx, int delay, int step, int delta) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectRunningRainbow, stripIdx);
  cmd.intParam1 = delay;
  cmd.intParam2 = step;
  cmd.intParam3 = delta;
  enqueueCommand(cmd);
}

void requestEffectRunningGradient(int stripIdx, CRGB color1, CRGB color2, int delay, float step) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectRunningGradient, stripIdx);
  cmd.color1 = color1;
  cmd.color2 = color2;
  cmd.intParam1 = delay;
  cmd.floatParam1 = step;
  enqueueCommand(cmd);
}

void requestEffectNoise(int stripIdx) {
  enqueueCommand(makeCommand(RenderCommandType::EffectNoise, stripIdx));
}

void requestEffectBlend(int stripIdx, CRGB color, int duration) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectBlend, stripIdx);
  cmd.color1 = color;
  cmd.intParam1 = duration;
  enqueueCommand(cmd);
}

void requestEffectFadeIn(int stripIdx, CRGB color, int duration) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectFadeIn, stripIdx);
  cmd.color1 = color;
  cmd.intParam1 = duration;
  enqueueCommand(cmd);
}

void requestEffectFadeOut(int stripIdx, int duration) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectFadeOut, stripIdx);
  cmd.intParam1 = duration;
  enqueueCommand(cmd);
}

void requestEffectMid2Out(int stripIdx, CRGB color, int duration) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectMid2Out, stripIdx);
  cmd.color1 = color;
  cmd.intParam1 = duration;
  enqueueCommand(cmd);
}

void requestEffectOut2Mid(int stripIdx, CRGB color, int duration) {
  RenderCommand cmd = makeCommand(RenderCommandType::EffectOut2Mid, stripIdx);
  cmd.color1 = color;
  cmd.intParam1 = duration;
  enqueueCommand(cmd);
}
