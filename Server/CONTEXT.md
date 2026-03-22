# Project Context
- Programming language: C++ (Arduino)
- Environment: ESP32, Arduino framework (PlatformIO)
- Hardware
    - Supported controllers: ESP32 (Seeed Studio XIAO ESP32S3, M5Stack ESP32-C3 Stamp-C3)
    - Supported led strips: WS2815, WS2813
    - Power source configuration
        - For WS2815: 11V for led strip, 11v -> 5V converter (step-down) for controller
        - For WS2813: 5V power source for both, the controller and led strip