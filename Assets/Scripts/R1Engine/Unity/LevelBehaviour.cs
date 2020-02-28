using UnityEngine;

namespace R1Engine.Unity {
    public class LevelBehaviour : MonoBehaviour {
        public Level level;

        Texture2D tileset;
        public GameObject visRoot, colRoot;
        public MeshRenderer visMr, colMr, bgOvrMr;
        public MeshFilter visMf, colMf, bgOvrMf;

        public void LoadLevel(World world, int levelIndex) {
            visMr.sharedMaterial.mainTexture =
                tileset = ReplaceThisClass.PS1_LoadTileset(world);
            level = ReplaceThisClass.PS1_LoadLevel(world, levelIndex);

            var verts = new Vector3[level.width * level.height * 4];
            var quads = new int[level.width * level.height * 4];
            var uv = new Vector2[verts.Length];
            var uvCol = new Vector2[verts.Length];
            for (int y = 0; y < level.height; y++) {
                for (int x = 0; x < level.width; x++) {
                    int i = x + y * level.width;

                    verts[0 + i * 4] = new Vector3(0 + x, 0 - y);
                    verts[1 + i * 4] = new Vector3(1 + x, 0 - y);
                    verts[2 + i * 4] = new Vector3(1 + x, -1 - y);
                    verts[3 + i * 4] = new Vector3(0 + x, -1 - y);

                    var uvs = GetTypeUV(level.types[i]);
                    var uvsCol = GetTypeUV_col(level.types[i]);

                    for (int u = 0; u < 4; u++) {
                        quads[u + i * 4] = u + i * 4;
                        uv[u + i * 4] = uvs[u];
                        uvCol[u + i * 4] = uvsCol[u];
                    }
                }
            }

            var vm = new Mesh();
            vm.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            vm.SetVertices(verts);
            vm.SetIndices(quads, MeshTopology.Quads, 0);
            vm.uv = uv;
            visMf.sharedMesh = vm;

            var cm = Instantiate(vm);
            cm.uv = uvCol;
            colMf.sharedMesh = cm;

            // level bounds overlay
            var mo = new Mesh();
            mo.vertices = new Vector3[] {
                new Vector3(0, 0),
                new Vector3(level.width, 0),
                new Vector3(level.width, -level.height),
                new Vector3(0, -level.height)
            };
            mo.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
            bgOvrMf.sharedMesh = mo;


            // Events
            foreach (var e in level.events) {
                Instantiate(EventBehaviour.resource).GetComponent<EventBehaviour>().ev = e;
            }
        }





        Vector2[] GetTypeUV(Type type) {
            return new Vector2[]
            {
            GetTypeUV_corner(0 + type.gX, 0 + type.gY),
            GetTypeUV_corner(1 + type.gX, 0 + type.gY),
            GetTypeUV_corner(1 + type.gX, 1 + type.gY),
            GetTypeUV_corner(0 + type.gX, 1 + type.gY)
            };
        }
        Vector2 GetTypeUV_corner(float x, float y) {
            return new Vector2(x / (16), y / (16 * ((float)tileset.height / tileset.width)));
        }

        Vector2[] GetTypeUV_col_f(float x, float y) {
            Vector2 div = new Vector2(0.125f, 0.25f);
            return new Vector2[]
            {
            new Vector2(x + 0, -y + 0) * div,
            new Vector2(x + 1, -y + 0) * div,
            new Vector2(x + 1, -y - 1) * div,
            new Vector2(x + 0, -y - 1) * div,
            };
        }

        Vector2[] GetTypeUV_col(Type tile) {
            switch (tile.col) {
                default:
                    return GetTypeUV_col_f(0, 0);
                case TypeCollision.Solid:
                    return GetTypeUV_col_f(1, 0);
                case TypeCollision.Passthrough:
                    return GetTypeUV_col_f(2, 0);
                case TypeCollision.Hill_Slight_Left_1:
                    return GetTypeUV_col_f(3, 0);
                case TypeCollision.Hill_Slight_Left_2:
                    return GetTypeUV_col_f(4, 0);
                case TypeCollision.Hill_Slight_Right_2:
                    return GetTypeUV_col_f(5, 0);
                case TypeCollision.Hill_Slight_Right_1:
                    return GetTypeUV_col_f(6, 0);
                case TypeCollision.Hill_Sleep_Left:
                    return GetTypeUV_col_f(7, 0);
                case TypeCollision.Hill_Steep_Right:
                    return GetTypeUV_col_f(0, 1);
                case TypeCollision.Slippery:
                    return GetTypeUV_col_f(1, 1);
                case TypeCollision.Slippery_Slight_Left_1:
                    return GetTypeUV_col_f(2, 1);
                case TypeCollision.Slippery_Slight_Left_2:
                    return GetTypeUV_col_f(3, 1);
                case TypeCollision.Slippery_Slight_Right_2:
                    return GetTypeUV_col_f(4, 1);
                case TypeCollision.Slippery_Slight_Right_1:
                    return GetTypeUV_col_f(5, 1);
                case TypeCollision.Slippery_Steep_Left:
                    return GetTypeUV_col_f(6, 1);
                case TypeCollision.Slippery_Steep_Right:
                    return GetTypeUV_col_f(7, 1);
                case TypeCollision.Bounce:
                    return GetTypeUV_col_f(0, 2);
                case TypeCollision.Climb:
                    return GetTypeUV_col_f(1, 2);
                case TypeCollision.Damage:
                    return GetTypeUV_col_f(2, 2);
                case TypeCollision.Water:
                    return GetTypeUV_col_f(3, 2);
                case TypeCollision.Spikes:
                    return GetTypeUV_col_f(4, 2);
                case TypeCollision.Cliff:
                    return GetTypeUV_col_f(5, 2);
                case TypeCollision.Reactionary:
                    return GetTypeUV_col_f(6, 2);
            }
        }
    }
}
