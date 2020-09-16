using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCP.API
{
    public class Control : APIBase
    {
        public enum ControlType {
            Open, Provide, Request, Response
        }

        public ControlType Subtype {get; set;}

        public ControlProvider Provide {get; set;}
        public JsonCompatible Response {get; set;}
    }
    public enum TypeInfo
    {
        Undefined, Number, String, Boolean
    }

    public class ControlRequest
    {
        public struct Argument
        {
            public string Name {get; set;}
            public string Value {get; set;}
            public TypeInfo Type {get; set;}
        }

        public string Name {get; set;}
        public IList<Argument> Args {get; set;}
    }

    public class ControlProvider
    {
        public class Command : IEquatable<Command>
        {
            public struct ArgInfo {
                public string Name {get; set;}
                public TypeInfo Type {get; set;}
                public IDictionary<string, string> Constraint {get; set;}
                public string Description {get; set;}
                public bool Required {get; set;}
            }

            public string Name {get; set;}
            public string Description {get; set;}
            public IList<ArgInfo> ArgInfos {get; set;}

            protected string GetSignature()
            {
                var sb = new StringBuilder($"{Name}:{Description}(");
                // 引数をくっつける
                foreach (var arg in ArgInfos)
                {
                    sb.Append($"{arg.Name}:{arg.Type} ");
                }
                sb.Append(')');
                return sb.ToString();
            }

            public override int GetHashCode()
            {
                // 関数シグネチャからハッシュ生成
                return GetSignature().GetHashCode();
            }

            public bool Equals(Command other)
            {
                if (Name != other.Name) return false;
                if (Description != other.Description) return false;
                if (GetSignature() != other.GetSignature()) return false;
                return true;
            }
        }

        public IList<Command> Commands {get; set;}
    }

    public static class RequestArgumentExtension
    {
        public static int? ToInt(this ControlRequest.Argument argument)
        {
            if (argument.Type != TypeInfo.Number) return null;

            if (int.TryParse(argument.Value, out var res)) {
                return res;
            }

            return (int?) argument.ToFloat();
        }

        public static double? ToFloat(this ControlRequest.Argument argument)
        {
            if (argument.Type != TypeInfo.Number) return null;

            if (double.TryParse(argument.Value, out var res)) {
                return res;
            }
            return null;
        }

        public static bool? ToBoolean(this ControlRequest.Argument argument)
        {
            if (argument.Type != TypeInfo.Boolean) return null;

            if (bool.TryParse(argument.Value, out var res)) {
                return res;
            }
            return null;
        }

        public static string ToString(this ControlRequest.Argument argument)
        {
            if (argument.Type != TypeInfo.String) return null;

            return argument.Value;
        }
    }
}