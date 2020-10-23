using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

namespace R1Engine.Serialize {
	public class SerializerLog {
		private StringBuilder log = new StringBuilder();
		public string OverrideLogPath { get; set; }
		public string LogFile => OverrideLogPath ?? Settings.LogFile;

		public void Log(object obj) {
			if (Settings.Log)
				log.AppendLine(obj != null ? obj.ToString() : "");
		}

		public void WriteLog() {
			if (Settings.Log && LogFile.Trim() != "") {
				using (StreamWriter writer = new StreamWriter(LogFile)) {
					writer.WriteLine(log.ToString());
				}
			}
		}
	}
}
