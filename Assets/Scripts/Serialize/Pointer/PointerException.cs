using System;
using System.Collections.Generic;

namespace R1Engine
{
    class PointerException : Exception {

        readonly string _excludeFromStackTrace;

        public PointerException(string message, string excludeFromStackTrace) {
            this.Message = message;
            _excludeFromStackTrace = excludeFromStackTrace;
        }

        public override string Message { get; }

        public override string StackTrace {
            get {
                List<string> stackTrace = new List<string>();
                stackTrace.AddRange(base.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                stackTrace.RemoveAll(x => x.Contains(_excludeFromStackTrace));
                return string.Join(Environment.NewLine, stackTrace.ToArray());
            }
        }
    }
}