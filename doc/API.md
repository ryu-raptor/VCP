# VCP API Tips

## API Combinations
VCP API is well componented and can be combine them.
However some API includes all infomation of some other APIs.

This table shows components relations.

| API         | Component                                                            |
|:------------|:---------------------------------------------------------------------|
| HeadPoseAPI | HeadPose [+ Emotion + LipSync]                                       |
| BustAPI     | BodyCapture(only upper body)                                         |
| FullBodyAPI | BodyCapture(entire)                                                  |
| InteractAPI | Interaction                                                          |
| ControlAPI  | ControlPresentation + ControlInterface<br>(Can include interactions) |
| EmotionAPI  | Emotion + LipSync                                                    |

Be careful about confliction.
(ex. Using both BustAPI and FullBodyAPI cause control confliction on upper body.)
See [Control Mask]()
