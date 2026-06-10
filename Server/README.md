# Visualedizer Server (Firmware)

ESP32 firmware for controlling addressable LED strips with a web interface, REST/WebSocket API, optional DMX/E1.31 input, and IR remote support.

The firmware accepts **binary RGB frames** over WebSocket from the [Windows client](../Client/README.md), and can also render built-in effects locally via HTTP or text WebSocket commands.

## Features

### Control Interfaces

- **WebSocket (port 81)** — binary RGB frame streaming from the client; text commands for built-in effects
- **HTTP REST API (port 80)** — configuration, effect commands, Wake-on-LAN
- **Web UI** — served from SPIFFS (`data/index.html`)
- **IR remote** — preset color and effect control
- **DMX / E1.31 (sACN)** — unicast and multicast (ESP32-S3 only; initialization currently commented out in `main.cpp`)

### Built-in Effects

Solid color, gradient, running rainbow, strobe (sync and random), fade in/out, color blend, noise, and section-based control. Effects are processed on a dedicated FreeRTOS render task (`render_service.cpp`).

### Network

- WiFi station, access point, or both
- Automatic fallback to AP mode when station connection fails
- Default AP: `Visualedizer` / `Rezidelausiv7331`
- Wake-on-LAN support
- Persistent configuration in NVM (Preferences)

## Hardware

### Supported Controllers

| Board | Typical use |
|-------|-------------|
| Seeed Studio XIAO ESP32S3 | Furniture, desk, ceiling profiles |
| M5Stack Stamp S3 | Kubis profile |
| M5Stack Stamp C3 / Adafruit QT Py ESP32-C3 | Wardrobe, printer profiles |

### Supported LED Strips

WS2812B, WS2813, WS2815 (WS2815 treated as WS2813 in configuration).

### Power Notes

- **WS2815:** 12 V strip supply; step down to 5 V for the controller
- **WS2813 / WS2812B:** 5 V for both controller and strip
- Size the supply for peak draw (~60 mA per LED at full white)

## Device Profiles

Build profiles are selected with `-D ID_*` flags in `platformio.ini`. Hardware constants live in `src/devices.h`.

| Profile | Board env | Strips | LEDs | Type | DMX universe |
|---------|-----------|--------|------|------|--------------|
| `ID_FURNITURE` | `seeed_xiao_esp32s3_furniture` | 1 | 218 | WS2813 | 1 |
| `ID_DESK` | `seeed_xiao_esp32s3_desk` | 3 | 30 + 274 + 30 | WS2813 | 2 |
| `ID_CEILING` | `seeed_xiao_esp32s3_ceiling` | 1 | 265 | WS2812B (GRB) | 3 |
| `ID_KUBIS` | `m5stack_m5stamp_s3_kubis` | 1 | 300 | WS2813 | 1 |
| `ID_WARDROBE` | `seeed_xiao_esp32c3_wardrobe` | 1 | 30 | WS2813 | 99 |
| `ID_PRINTER` | `m5stack_m5stamp_c3u_printer` | 1 | 60 | WS2812B | 1 |

Desk devices expose three strips on GPIO 2, 3, and 4 with per-strip LED offsets for composite layouts.

## Requirements

- [PlatformIO](https://platformio.org/)
- Arduino framework for ESP32 (`espressif32` platform)

## Dependencies

Managed via `platformio.ini`:

- `fastled/FastLED@3.6.0`
- `ottowinter/ESPAsyncWebServer-esphome@^3.3.0`
- `links2004/WebSockets@^2.4.1`
- `forkineye/ESPAsyncE131@^1.0.4` (ESP32-S3 builds)
- `bblanchon/ArduinoJson@^7.1.0`
- `crankyoldgit/IRremoteESP8266@^2.8.6`

## Installation

1. Clone the repository and enter the firmware directory:

   ```bash
   cd Visualedizer/Server
   ```

2. Create or edit `src/credentials.h`:

   ```cpp
   #define WIFI_SSID "YourWiFiSSID"
   #define WIFI_PASSWORD "YourWiFiPassword"
   ```

3. Choose the target environment in `platformio.ini` (or pass `-e` on the command line).

4. Build, upload, and monitor:

   ```bash
   pio run -e seeed_xiao_esp32s3_desk -t upload
   pio device monitor
   ```

5. Upload SPIFFS web assets when the UI changes:

   ```bash
   pio run -e seeed_xiao_esp32s3_desk -t uploadfs
   ```

Serial baud rate: **115200**.

## Configuration

### Web Interface

After flashing:

1. If WiFi connects, browse to the IP shown in the serial monitor.
2. If not, join AP `Visualedizer` and open `http://192.168.4.1`.

Configurable items include LED counts, WiFi mode and credentials, boot fade-in, activity timeout, DMX universe settings, IR enable, and device name.

### Client Integration

The Windows client discovers devices through:

```
GET http://<device-ip>/get-conf
```

It streams frames to:

```
ws://<device-ip>:81   (binary messages, length = totalLedCount × 3)
```

On WebSocket disconnect, all strips are turned off.

## API Reference

### HTTP Endpoints

#### Effect update

```
GET /update?command=<command>&<params>
```

Commands include `off`, `solid-color`, `gradient`, `rainbow`, `strobe`, `strobe-random`, `fade-in`, `fade-out`, and `blend`. Parameters vary by command (RGB/HSV values, delays, durations, optional `stripIdx`).

#### Configuration

```
GET  /get-conf          # Current configuration and strip metadata
POST /set-conf          # Update configuration (JSON body)
```

#### Other

```
GET /ctrl?signal=<signal>   # Control signal
GET /wol?mac=<mac-address>  # Wake-on-LAN magic packet
GET /test                   # Health check
```

### WebSocket (port 81)

| Message type | Usage |
|--------------|-------|
| **Binary** | Raw RGB frame from the client (`totalLedCount × 3` bytes) |
| **Text** | Same query-string format as `/update` (e.g. `update?command=off`) |

Text commands are echoed back to the sender. Built-in effects run on the firmware render task; binary frames replace the strip buffer directly.

## Project Structure

```
Server/
├── platformio.ini           # Build environments and library deps
├── src/
│   ├── main.cpp             # Setup / loop, service initialization
│   ├── devices.h            # Per-profile hardware constants
│   ├── credentials.h        # WiFi credentials (local, not in repo)
│   ├── app_state.cpp        # Shared runtime state
│   ├── network_service.cpp  # WiFi station / AP / hostname
│   ├── http_service.cpp     # Async HTTP server and SPIFFS static files
│   ├── websocket_service.cpp# WebSocket server (binary + text)
│   ├── command_service.cpp  # Parse HTTP/WS commands
│   ├── render_service.cpp   # FreeRTOS render task and built-in effects
│   ├── led.cpp / led_strip_dvc.cpp  # FastLED strip drivers
│   ├── nvm.cpp              # Persistent Preferences storage
│   ├── controller.cpp       # High-level control orchestration
│   ├── controller_ir.cpp    # IR remote handling
│   ├── runtime_service.cpp  # Activity timeout, boot events
│   └── dmx.cpp              # E1.31 / DMX input (ESP32-S3)
└── data/                    # SPIFFS content
    ├── index.html           # Web UI
    └── res/                 # CSS, JS, fonts
```

## Development

### Adding a Device Profile

1. Add an `#ifdef ID_YOUR_DEVICE` block in `src/devices.h` with strip count, data pins, LED counts, offsets, type, and color order.
2. Add a PlatformIO environment:

   ```ini
   [env:your_board_your_device]
   platform = espressif32
   board = your_board
   framework = arduino
   build_flags =
     -D ID_YOUR_DEVICE
     -D ESP32S3   # or ESP32C3
   lib_deps = ...  # copy from an existing env
   ```

### Adding a Built-in Effect

1. Add a `RenderCommandType` and handler in `render_service.cpp`
2. Expose a `requestEffect...()` function in `render_service.h`
3. Parse the command in `command_service.cpp`
4. Update the web UI in `data/index.html` if needed

## Troubleshooting

| Issue | Remedy |
|-------|--------|
| WiFi will not connect | Check `src/credentials.h`; device falls back to AP mode |
| Web UI missing styles | Run `uploadfs` to deploy `data/` to SPIFFS |
| Strip flicker or wrong colors | Verify `DVC_LED_TYPE`, color order, and data pin in `devices.h` |
| Client frames ignored | Confirm total byte count matches `totalLedCount × 3` |
| DMX inactive | Requires ESP32-S3; uncomment `initDmx()` / `processDmx()` in `main.cpp` |
| Insufficient power | Reduce brightness or add capacity; calculate ~60 mA × LED count |
