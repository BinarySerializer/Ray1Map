using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Serialize {
	public class SerializerLog {
		private StringBuilder log = new StringBuilder();

		public void Log(object obj) {
			if (Settings.Log)
				log.AppendLine(obj != null ? obj.ToString() : "");
		}

		public void WriteLog() {
			if (Settings.Log && Settings.LogFile.Trim() != "") {
				using (StreamWriter writer = new StreamWriter(Settings.LogFile)) {
					writer.WriteLine(log.ToString());
				}
			}
		}
	}
}
