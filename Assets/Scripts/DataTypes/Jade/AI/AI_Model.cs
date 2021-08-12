﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Model: Jade_File {
		public override string Export_Extension => "omd";

		public Jade_GenericReference[] References { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			References = s.SerializeObjectArray<Jade_GenericReference>(References, FileSize / 8, name: nameof(References));
			foreach (var reference in References) {
				switch (reference.Type) {
					case Jade_FileType.FileType.AI_TT:
						reference.Resolve(flags: LOA_Loader.ReferenceFlags.DontCache);
						break;
					default:
						reference.Resolve();
						break;
				}
			}
		}

		public AI_Vars Vars => References?.FirstOrDefault(r => !r.IsNull && r.Type == Jade_FileType.FileType.AI_Vars)?.Value as AI_Vars;
	}
}
