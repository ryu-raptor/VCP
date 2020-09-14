using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCP.API
{
    public class HeadPose : APIBase {
        [JsonPropertyName("rotation")]
        public EularAngle Rotation {get; set;}

        [JsonPropertyName("position")]
        public SpacialPosition Position {get; set;}

        public override string Type
        {
            get {
                return "headPoseAPI";
            }
        }
    }
}