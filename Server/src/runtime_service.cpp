#include "runtime_service.h"

#include "common.h"
#include "led.h"
#include "led_strip_dvc.h"
#include "main.h"
#include "network_service.h"
#include "render_service.h"

void updateActivity() {
  Serial.printf("Updating last activity (%d)\n", lastActivity);
  lastActivity = millis();

  if (activityTimeoutEnabled) {
    activityTimeoutRequested = true;
  }
}

static void startBootFadeIn() {
  Serial.println("Starting boot fade in");
  CRGB warmWhite = htmlColor2Crgb(bootColor);
  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Starting fade-in effect on strip %d\n", dvc.ledIdx);
    requestEffectFadeIn(dvc.ledIdx, warmWhite, 3000);
  });
}

void startBootWol() {
  Serial.println("Start boot WOL");
  sendMagicPacket(bootWolMac.c_str());
}

void initBootEvents() {
  if (bootFadeIn) {
    startBootFadeIn();
  }
}

void checkActivityTimeout() {
  const bool tmpTimeoutReached =
      tmpActivityTimeout > 0 && (millis() - lastActivity > tmpActivityTimeout * 1000);
  const bool defaultTimeoutReached =
      activityTimeoutRequested && (millis() - lastActivity > activityTimeout * 1000);

  if (!activityTimeoutRequested || (!tmpTimeoutReached && !defaultTimeoutReached)) {
    return;
  }

  Serial.printf("tmpActivityTimeout: %d, millis - last activity: %d \n",
      tmpActivityTimeout, (millis() - lastActivity));

  Serial.println("Activity timeout reached, fading out LEDs");
  activityTimeoutRequested = false;
  tmpActivityTimeout = 0;

  forEachLedStrip([&](LedStripDvc& dvc) {
    Serial.printf("Running fade-out effect on strip %d\n", dvc.ledIdx);
    requestEffectFadeOut(dvc.ledIdx, fadeOutDuration);
  });
}
