using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray1Map.Jade {
	public class AI_FunctionDef {
		public uint Key { get; set; }
		public string Name { get; set; }

		public AI_FunctionDef(uint key, string name) {
			Key = key;
			Name = name;
		}
	}
}
