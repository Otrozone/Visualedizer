# Visualedizer

A feature-rich ESP32-based LED strip controller firmware with web interface, DMX support, IR remote control, and multiple lighting effects.

## Overview

Visualedizer is an embedded firmware solution for controlling addressable LED strips (WS2812B, WS2813, WS2815) using ESP32 microcontrollers. It provides a modern web-based interface, multiple control protocols, and a variety of lighting effects for home automation and entertainment lighting applications.

## Features

### Core Functionality
- **Multi-Protocol Support**
  - HTTP REST API
  - WebSocket for real-time control
  - DMX/E1.31 (sACN) support (unicast and multicast)
  - IR remote control

### Lighting Effects
- Solid color (RGB and HSV)
- Gradient (HSV-based)
- Running rainbow
- Strobe (synchronized and random)
- Fade in/out
- Color blending
- Noise effect

### Network Features
- WiFi connectivity (Station, Access Point, or both)
- Automatic fallback to AP mode if WiFi connection fails
- Wake-on-LAN (WOL) support
- Web-based configuration interface
- Device naming and identification

### Advanced Features
- Activity timeout with automatic fade-out
- Boot fade-in effect
- Multiple LED strip support (up to 5 strips per device)
- Persistent configuration storage (NVM)
- Real-time effect control
- Section-based lighting control

## Hardware Requirements

### Supported Controllers
- **Seeed Studio XIAO ESP32S3** (recommended)
- **M5Stack ESP32-C3 Stamp-C3**
- **M5Stack M5Stamp S3**

### Supported LED Strips
- WS2812B
- WS2813
- WS2815

### Power Configuration
- **WS2815**: 11V for LED strip, 11V → 5V step-down converter for controller
- **WS2813**: 5V power source for both controller and LED strip

## Device Configurations

The firmware supports multiple pre-configured device profiles:

| Device ID | Description | LED Count | DMX Universe | LED Type |
|-----------|-------------|-----------|--------------|----------|
| `ID_FURNITURE` | Furniture lighting | 218 | 1 | WS2813 |
| `ID_DESK` | Desk lighting | 30 | 2 | WS2813 |
| `ID_CEILING` | Ceiling lighting | 265 | 3 | WS2812B |
| `ID_KUBIS` | Kubis lighting | 300 | 1 | WS2813 |
| `ID_WARDROBE` | Wardrobe lighting | 30 | 99 | WS2813 |
| `ID_PRINTER` | Printer lighting | 60 | 1 | WS2812B |

## Software Requirements

- **PlatformIO**
- **Arduino Framework** for ESP32
- **Espressif32**

## Dependencies

The project uses the following libraries (managed via PlatformIO):

- `fastled/FastLED@3.6.0` - LED strip control
- `ottowinter/ESPAsyncWebServer-esphome@^3.3.0` - Async HTTP server
- `links2004/WebSockets@^2.4.1` - WebSocket support
- `forkineye/ESPAsyncE131@^1.0.4` - DMX/E1.31 support
- `bblanchon/ArduinoJson@^7.1.0` - JSON parsing
- `crankyoldgit/IRremoteESP8266@^2.8.6` - IR remote control

## Installation

### Using PlatformIO

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Visualedizer/Server
   ```

2. Configure your device in `src/devices.h`.

3. Set up WiFi credentials in `src/credentials.h`:
   ```cpp
   #define WIFI_SSID "YourWiFiSSID"
   #define WIFI_PASSWORD "YourWiFiPassword"
   ```

4. Select your target device environment in `platformio.ini`:
   ```ini
   [env:seeed_xiao_esp32s3_furniture]  # or your device
   ```

5. Build and upload:
   ```bash
   pio run -e seeed_xiao_esp32s3_furniture -t upload
   ```

6. Monitor serial output:
   ```bash
   pio device monitor
   ```

### Building for Different Devices

The project includes multiple build environments in `platformio.ini`.

## Configuration

### Web Interface

After flashing, connect to the device:
1. If WiFi connection succeeds, access via the device's IP address (shown in serial monitor)
2. If WiFi connection fails, connect to the AP: `Visualedizer` (password: `Rezidelausiv7331`)
3. Open a web browser and navigate to `http://<device-ip>` or `http://192.168.4.1` (AP mode)

### Configuration Options

- **LED Count**: Number of LEDs in the strip
- **WiFi Mode**: Auto, Station, Access Point, or Station+AP
- **WiFi Credentials**: SSID and password
- **Boot Settings**: Fade-in on boot, WOL on boot
- **Activity Timeout**: Automatic fade-out after inactivity
- **DMX Settings**: Universe, universe count, unicast/multicast
- **IR Settings**: Enable/disable IR control
- **Device Name**: Custom device identification

## API Reference

### HTTP Endpoints

#### Update Command
```
GET /update?command=<command>&<params>
```

Available commands:
- `off` - Turn off all LEDs
- `solid-color` - Set solid color (`color` or `hue`, `saturation`, `value`)
- `gradient` - Set gradient (`hueStart`, `hueEnd`, `brightness`)
- `rainbow` - Running rainbow effect (`delay`, `step`, `delta`)
- `strobe` - Strobe effect (`color`, `delay1`, `delay2`, optional `stripIdx`)
- `strobe-random` - Random strobe (`color`)
- `fade-in` - Fade in effect (`color` or `hue`, `saturation`, `value`, `duration`)
- `fade-out` - Fade out effect (`duration`)
- `blend` - Blend to color (`color` or `hue`, `saturation`, `value`, `duration`)

#### Configuration
```
GET /get-conf - Get current configuration
POST /set-conf - Set configuration (JSON body)
```

#### Control Signals
```
GET /ctrl?signal=<signal> - Send control signal
```

#### Wake-on-LAN
```
GET /wol?mac=<mac-address> - Send magic packet
```

### WebSocket

Connect to `ws://<device-ip>:81`

Message format: `update?command=<command>&<params>` (same as HTTP update endpoint)

## Project Structure

```
Server/
├── src/                   # Source code
│   ├── main.cpp           # Main application logic
│   ├── main.h             # Main header
│   ├── devices.h          # Device configurations
│   ├── credentials.h      # WiFi credentials (create this)
│   ├── controller.h       # IR controller interface
│   ├── controller_ir.cpp  # IR control implementation
│   ├── effects.h          # Lighting effects definitions
│   ├── effects.cpp        # Effects implementation
│   ├── dmx.h              # DMX interface
│   ├── dmx.cpp            # DMX implementation
│   ├── led_strip_dvc.h    # LED strip device class
│   ├── led_strip_dvc.cpp  # LED strip implementation
│   ├── nvm.h              # Non-volatile memory interface
│   ├── nvm.cpp            # NVM implementation
│   └── common.h           # Common utilities
├── data/                  # Web interface files
│   ├── index.html         # Main web interface
│   └── res/               # Static resources (CSS, JS, fonts)
├── platformio.ini         # PlatformIO configuration
└── README.md              # This file
```

## Development

### Adding a New Device

1. Add device configuration in `src/devices.h`:
   ```cpp
   #ifdef ID_YOUR_DEVICE
     #define DVC_STRIP_COUNT 1
     #define DVC_DATA_PIN 3
     #define DVC_NUM_LEDS 100
     // ... other settings
   #endif
   ```

2. Add build environment in `platformio.ini`:
   ```ini
   [env:your_board_your_device]
   platform = espressif32
   board = your_board
   build_flags = -D ID_YOUR_DEVICE
   ```

### Adding a New Effect

1. Define effect parameters in `src/effects.h`
2. Implement effect task function in `src/effects.cpp`
3. Add effect method to `LedStripDvc` class
4. Add command handler in `src/main.cpp`
5. Update web interface in `data/index.html`

## Troubleshooting

### WiFi Connection Issues
- Check credentials in `src/credentials.h`
- Device will fall back to AP mode if connection fails
- Default AP: `Visualedizer` / `Rezidelausiv7331`

### LED Strip Not Working
- Verify power supply capacity (calculate: LEDs × 60mA)
- Check data pin configuration in `devices.h`
- Verify LED type and color order settings
- Ensure proper grounding between controller and strip

### DMX Not Working
- DMX support requires ESP32S3 (not available on ESP32C3)
- Check universe configuration
- Verify network connectivity for multicast mode

### Serial Monitor
- Baud rate: 115200
- Monitor output for connection status and error messages
