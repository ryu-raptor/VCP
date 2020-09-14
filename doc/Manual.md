---
title: Virtual Control Protocol API for C# Manual
author: ryu-raptor
date: v.1
---

# Virtual Control Protocol API for C# Manual

## TOC
- Sample
- Archtecture
- API Documentation
- WebSocket Documentation

## Sample

### Sink
```{.cs}
using System;

var client = new SinkClientDispatcher("ws://localhost:3000");
client.AddProcessor(new HeadPoseSink());
/* Here Main Loop */

class HeadPoseSink : VCP.WebSocket.SinkClient<VCP.API.HeadPose>
{
    public override void OnMessage(VCP.API.HeadPose pose)
    {
        var rot = pose.Rotation;
        Console.WriteLine($"({rot.Pitch}, {rot.Roll}, {rot.Yaw})");
    }
}
```

### Source
```{.cs}
using System;

var client = new SourceClient("ws://localhost:3000");
client.Send(new HeadPose() {
    Rotation = new EularAngle() { Pitch = 3.1, Roll = 0, Yaw = 0.43 },
    Position = new SpacialPosition() { X = 0, Y = 1, Z = 0 }
})
```

## Archtecture

```
| VCP.WebSocket |
Client <= JSON/MessagePack =>
SinkClient : Client <= Dispatch =>
SourceClient : Client <= Send =>

| VCP.API |
ISinkProcessor
APISink<T> : ISinkProcessor
```

### Namespaces

#### `VCP`
Virtual Control Protocol root namespace

#### `VCP.API`
APIs

#### `VCP.WebSocket`
WebSocket interface

## API Documentation

Compatible with YAML API Docs

And JsonCompatible and APIBase are implemented to support JSON/MessagePack compatible

## WebSocket Documentation

```
class Client(url::string)
    +a OnMessage(sender::object, e::Message)

class Source(url::string) : Client(url)
    + Send(api::JsonCompatible)
    + Send(api::APIBase)

class Sink(url::string) : Client(url)
    + AddProcessor(p::IClientProcessor)
```