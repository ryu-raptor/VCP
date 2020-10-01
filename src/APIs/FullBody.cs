using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCP.API
{
    public struct Joint {
        public Quaternion Rotation {get; set;}
        public SpacialPosition? Position {get; set;}
    }

    public class FullBody : APIBase {
        public Dictionary<string, Joint> Joints {get; set;}

        public SpacialPosition? HipPosition {get; set;}

        public override string Type
        {
            get {
                return "fullBodyAPI";
            }
        }

        public Joint? QueryJoint(JointName name) {
            if (Joints.TryGetValue(name.ToString(), out var j)) {
                return j;
            }
            return null;
        }
    }

    /// <summary>
    /// camelCase for string query performance
    /// </summary>
    public enum JointName {
        spine,
        spine2,
        head,
        l_upperArm,
        l_lowerArm,
        l_wrist,
        l_upperLeg,
        l_lowerLeg,
        l_ankle,
        r_upperArm,
        r_lowerArm,
        r_wrist,
        r_upperLeg,
        r_lowerLeg,
        r_ankle
    }
}

