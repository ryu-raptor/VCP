# Release Note

## 0.1.0

Initial Release

1. Added these APIs
    - APIBase
        - Supports JSON
    - Handshake
    - HeadPose
    - Control (Provide)

2. Basic WebSocket Supports
    - WebSocket relay server
    - Sink/Source client
    - Sink Listener (Event Driven)
    - JSON support

## 0.1.1

1. Modify WebSocket sink client
    - Queue based non-async process listener
        - Switch `SinkClient.ProcessSync` to `true` to enable this feature.
        - Call `SinkClient.ProcessQueue()` on main thread to process queued events.
    - Omit unneeded console output
        - Switch `RelayServer.DebugMode` to `true` to log on console.
