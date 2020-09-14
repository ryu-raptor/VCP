using System;
using System.Text.Json;

namespace VCP.API
{
    public struct EularAngle
    {
        public float Pitch {get; set;}
        public float Roll {get; set;}
        public float Yaw {get; set;}
    }
}