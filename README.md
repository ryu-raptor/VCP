# VCP

![Easy, Simple, Flexible](doc/VCP_logo.png)

A WebSocket/JSON (also MessagePack) based VR-Control (Mo-Cap, Emotion, Lip-Sync, General-Control) communication protocol.

VCP stands for Virtual Communication Protocol or Virtual Control Protocol

Other languages: [ja](README.ja.md)

## Reference Implementation
- C# : This repository. See `README_cs.md` ([link](doc/README_cs.md)) for more info.
- TypeScript : [vcp-ts](github.com/ryu-raptor/vcp-ts)

## Protocol Spec (Brief/Draft)
Complete specs: [here (WIP)](doc/ProtocolSpec.md)

**Note**

In VCP, a word **API** refers "data container specifications".

The author understands it confusing and will change this keyword to some other word (like *Schema*).
However the word are already used widely so refactoring may take long time.

So, under v1 series, the word **API** will be used as a "data container specification".

### Communication Spec
WebSocket based communication.

VCP defines some APIs to communicate. See JSON schema.

All communications are written in JSON/MessagePack formats.

### JSON Schema
JSON Schema : under `/schema`, written in yaml.

Belows are some important APIs.

#### GitHub Style Table
| API              | Role                                                                                            |
|:-----------------|:------------------------------------------------------------------------------------------------|
| handshake        | Tell relay server who you are (Sink, Source, Control)                                           |
| headPose         | Send head pose captured with some face detection program                                        |
| fullBody         | Send full body tracking data (or a part of them)                                                |
| bust             | Send spine, arms, head poses (bust and above poses)                                             |
| interact         | Send interaction control (grab an object, throw it, etc.)                                       |
| controlAPI suite | Exchange general purpose controls<br>(includes interation, recordings, lighting controls, etc.) |

#### Pandoc Style Table (for reading with local text editor)
+------------------+------------------------------------------------------------+
| API              | Role                                                       |
+==================+============================================================+
| handshake        | Tell relay server who you are (Sink, Source, Control)      |
+------------------+------------------------------------------------------------+
| headPose         | Send head pose captured with some face detection program   |
+------------------+------------------------------------------------------------+
| fullBody         | Send full body tracking data (or a part of them)           |
+------------------+------------------------------------------------------------+
| bust             | Send spine, arms, head poses (bust and above poses)        |
+------------------+------------------------------------------------------------+
| interact         | Send interaction control (grab an object, throw it, etc.)  |
+------------------+------------------------------------------------------------+
| controlAPI suite | Exchange general purpose controls                          |
|                  | (includes interation, recordings, lighting controls, etc.) |
+------------------+------------------------------------------------------------+


### MessagePack Support
VCP also supports MessagePack.

Reference C# VCP implementation supports direct MessagePack communications, which means you can send MessagePack binaries without some JSON handshakes in advance.
(The implementation checks every arrived message whether it is text or binary, then convert it in a C# native structure.)

Direct communication is an optional feature and is not required for your implementations.

## License

**VCP C# Reference Implementation** and **VCP Protocol Specification** is licensed under MIT License.

See `LICENSE`
