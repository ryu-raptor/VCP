
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
            Provide, Request, Response
        }

        public ControlType Subtype {get; set;}

        public ControlProvider Provide {get; set;}
    }
    public enum TypeInfo
    {
        Number, String, Boolean
    }

    public class ControlRequest
    {
        public struct Argument
        {
            public string Name {get; set;}
            public string Value {get; set;}
        }

        public string Name {get; set;}
        public IList<Argument> Args {get; set;}
    }

    public class ControlProvider
    {
        public class Command
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
        }

        public IList<Command> Commands {get; set;}
    }
}