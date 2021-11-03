using System.Collections.Generic;
using UnityEngine;

namespace Ray1Map {
	public class MeshInProgress {
		public string name;
		public List<Vector3> vertices = new List<Vector3>();
		public List<Vector3> normals = new List<Vector3>();
		public List<Vector2> uvs = new List<Vector2>();
		public List<Color> colors = new List<Color>();
		public List<int> triangles = new List<int>();
		public Texture2D texture;
		public MeshInProgress(string name, Texture2D texture = null) {
			this.name = name;
			this.texture = texture;
		}
	}
}
