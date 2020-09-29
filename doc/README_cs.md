---
title: VCP for C# Manual
author: ryu-raptor
---

# VCP Reference Implementation for C\#

## TOC
- Build
- Sample
- Archtecture
- API Documentation
- WebSocket Documentation

## Build

### Requirement
- [dotnet]()
- [netstandard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

### Dependencies
- [sta/websocket-sharp](https://github.com/sta/websocket-sharp)
    - Currently using [chenzuo/WebSocketSharp.Standard](https://www.nuget.org/packages/WebSocketSharp.Standard)
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)

Dependencies are written in `VCP.csproj`, you don't need to manually install them.

### Build commands

#### Build NuGet package
```
% dotnet pack -c Release
```
NuGet package is placed as `bin/Release/VCP.[Version].nupkg`.

- Using local NuGet packages.
```
% nuget add package [Your NuGet Package Path] -s [Local Repository Path (usually ./packages)]
% dotnet add [Your NuGet Package Name] -s [Local Repository Path (same as above)]
```

#### Manual dll build
```
% dotnet build -c Release
% dotnet publish -c Release [Your Deploy Path]
```

## Sample

### Sink
``` csharp
using System;

var client = new VCP.WebSocket.SinkClient("ws://localhost:3000");
client.AddProcessor(new HeadPoseSink());
/* Here Main Loop */

class HeadPoseSink : VCP.WebSocket.APISink<VCP.API.HeadPose>
{
    public override void OnMessage(VCP.API.HeadPose pose)
    {
        var rot = pose.Rotation;
        Console.WriteLine($"({rot.Pitch}, {rot.Roll}, {rot.Yaw})");
    }
}
```

### Source
``` csharp
using System;

var client = new SourceClient("ws://localhost:3000");
client.Send(new HeadPose() {
    Rotation = new EularAngle() { Pitch = 3.1, Roll = 0, Yaw = 0.43 },
    Position = new SpacialPosition() { X = 0, Y = 1, Z = 0 }
})
```

## Archtecture
See VCP Specification.

### Namespaces

#### `VCP`
Virtual Control Protocol root namespace

#### `VCP.API`
C# Native and JSON/MessagePack compatible API interfaces.

#### `VCP.WebSocket`
WebSocket interface

## API Documentation

See source code in VCP.API namespace.
