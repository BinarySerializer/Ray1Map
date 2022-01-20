using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ray1Map
{
    public static class GeometryHelpers 
    {
        public static Mesh CreateBox(float size_x, float size_y, float size_z) {
            Mesh mesh = new Mesh();

            Vector3 p0 = new Vector3(-size_x * .5f, -size_y * .5f, size_z * .5f);
            Vector3 p1 = new Vector3(size_x * .5f, -size_y * .5f, size_z * .5f);
            Vector3 p2 = new Vector3(size_x * .5f, -size_y * .5f, -size_z * .5f);
            Vector3 p3 = new Vector3(-size_x * .5f, -size_y * .5f, -size_z * .5f);
            Vector3 p4 = new Vector3(-size_x * .5f, size_y * .5f, size_z * .5f);
            Vector3 p5 = new Vector3(size_x * .5f, size_y * .5f, size_z * .5f);
            Vector3 p6 = new Vector3(size_x * .5f, size_y * .5f, -size_z * .5f);
            Vector3 p7 = new Vector3(-size_x * .5f, size_y * .5f, -size_z * .5f);

            Vector3[] vertices = new Vector3[] {
                p0, p1, p2, p3,
                p7, p4, p0, p3,
                p4, p5, p1, p0,
                p6, p7, p3, p2,
                p5, p6, p2, p1,
                p7, p6, p5, p4
            };

            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normales = new Vector3[] {
                down, down, down, down,
                left, left, left, left,
                front, front, front, front,
                back, back, back, back,
                right, right, right, right,
                up, up, up, up
            };

            Vector2 _00 = new Vector2(0f, 0f);
            Vector2 _10 = new Vector2(1f, 0f);
            Vector2 _01 = new Vector2(0f, 1f);
            Vector2 _11 = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
            };

            int[] triangles = new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            return mesh;
        }
        public static Mesh CreateBoxDifferentHeights(float sz, float h0, float h1, float h2, float h3, Color? color = null) =>
            CreateBoxDifferentHeights(sz, sz, h0, h1, h2, h3, color);
        public static Mesh CreateBoxDifferentHeights(float xSize, float zSize, float h0, float h1, float h2, float h3, Color? color = null) {
            return CreateBoxDifferentHeights(new Vector3[]
            {
                new Vector3(-xSize * .5f, 0,  zSize * .5f),
                new Vector3(xSize * .5f,  0,  zSize * .5f),
                new Vector3(xSize * .5f,  0,  -zSize * .5f),
                new Vector3(-xSize * .5f, 0,  -zSize * .5f),
                new Vector3(-xSize * .5f, h0, zSize * .5f),
                new Vector3(xSize * .5f,  h1, zSize * .5f),
                new Vector3(xSize * .5f,  h2, -zSize * .5f),
                new Vector3(-xSize * .5f, h3, -zSize * .5f),
            }, color);
        }

        public static Mesh CreateBoxDifferentHeights(Vector3[] points, Color? color = null) 
        {
            Mesh mesh = new Mesh();

            if (points.Length != 8)
                throw new Exception($"Invalid points count of {points.Length}, should be 8");

            Vector3[] vertices = {
                points[0], points[1], points[2], points[3],
                points[7], points[4], points[0], points[3],
                points[4], points[5], points[1], points[0],
                points[6], points[7], points[3], points[2],
                points[5], points[6], points[2], points[1],
                points[7], points[6], points[5], points[4]
            };

            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normals = new Vector3[] {
                down, down, down, down,
                left, left, left, left,
                front, front, front, front,
                back, back, back, back,
                right, right, right, right,
                up, up, up, up
            };

            Vector2 _00 = new Vector2(0f, 0f);
            Vector2 _10 = new Vector2(1f, 0f);
            Vector2 _01 = new Vector2(0f, 1f);
            Vector2 _11 = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
                _11, _01, _00, _10,
            };

            int[] triangles = new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            if (color.HasValue) {
                Color[] cols = new Color[vertices.Length];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateSplitBox(float sz, float height, int corner, Color? color = null) {
            Mesh mesh = new Mesh();

            float xSize = sz;
            float zSize = sz;

            Vector3 p0b = corner == 0 ? new Vector3(-xSize * .5f, 0, -zSize * .5f) : new Vector3(-xSize * .5f, 0,  zSize * .5f);
            Vector3 p1b = corner == 1 ? new Vector3(-xSize * .5f, 0,  zSize * .5f) : new Vector3( xSize * .5f, 0,  zSize * .5f);
            Vector3 p2b = corner == 2 ? new Vector3( xSize * .5f, 0,  zSize * .5f) : new Vector3( xSize * .5f, 0, -zSize * .5f);
            Vector3 p3b = corner == 3 ? new Vector3( xSize * .5f, 0, -zSize * .5f) : new Vector3(-xSize * .5f, 0, -zSize * .5f);
            
            Vector3 p0t = corner == 0 ? new Vector3(-xSize * .5f, height, -zSize * .5f) : new Vector3(-xSize * .5f, height,  zSize * .5f);
            Vector3 p1t = corner == 1 ? new Vector3(-xSize * .5f, height,  zSize * .5f) : new Vector3( xSize * .5f, height,  zSize * .5f);
            Vector3 p2t = corner == 2 ? new Vector3( xSize * .5f, height,  zSize * .5f) : new Vector3( xSize * .5f, height, -zSize * .5f);
            Vector3 p3t = corner == 3 ? new Vector3( xSize * .5f, height, -zSize * .5f) : new Vector3(-xSize * .5f, height, -zSize * .5f);

            Vector3[] vertices = new Vector3[] {
                corner <= 0 ? p1b : p0b, // Bottom
                corner <= 1 ? p2b : p1b,
                corner <= 2 ? p3b : p2b,

                corner <= 0 ? p1t : p0t, // Top
                corner <= 1 ? p2t : p1t,
                corner <= 2 ? p3t : p2t,

                corner == 0 ? p0b : corner == 1 ? p1b : corner == 2 ? p2b : p3b, // Diagonal
                corner == 0 ? p1b : corner == 1 ? p2b : corner == 2 ? p3b : p0b,
                corner == 0 ? p0t : corner == 1 ? p1t : corner == 2 ? p2t : p3t,
                corner == 0 ? p1t : corner == 1 ? p2t : corner == 2 ? p3t : p0t,

                corner == 0 ? p1b : corner == 1 ? p2b : corner == 2 ? p3b : p0b, // Side 1
                corner == 0 ? p2b : corner == 1 ? p3b : corner == 2 ? p0b : p1b,
                corner == 0 ? p1t : corner == 1 ? p2t : corner == 2 ? p3t : p0t,
                corner == 0 ? p2t : corner == 1 ? p3t : corner == 2 ? p0t : p1t,

                corner == 0 ? p2b : corner == 1 ? p3b : corner == 2 ? p0b : p1b, // Side 2
                corner == 0 ? p3b : corner == 1 ? p0b : corner == 2 ? p1b : p2b,
                corner == 0 ? p2t : corner == 1 ? p3t : corner == 2 ? p0t : p1t,
                corner == 0 ? p3t : corner == 1 ? p0t : corner == 2 ? p1t : p2t,
            };

            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 diagonal = Quaternion.Euler(0, 45 + 90 * corner, 0) * Vector3.forward;
            Vector3 side1 = Quaternion.Euler(0, 90 * corner, 0) * Vector3.right;
            Vector3 side2 = Quaternion.Euler(0, 90 * corner, 0) * Vector3.back;

            /* Rotation: (corner = i => that corner is missing)
             * 0: UP LEFT
             * 1: UP RIGHT
             * 2: DOWN RIGHT
             * 3: DOWN LEFT
             * */

            Vector3[] normals = new Vector3[] {
                down, down, down,
                up, up, up,
                diagonal, diagonal, diagonal, diagonal,
                side1, side1, side1, side1,
                side2, side2, side2, side2,
            };

            int[] triangles = new int[] {
                0,1,2, // Down
                3,4,5, // Up
                6 + 4*0 + 0, 6 + 4*0 + 1, 6 + 4*0 + 2,
                6 + 4*0 + 1, 6 + 4*0 + 2, 6 + 4*0 + 3,

                6 + 4*1 + 0, 6 + 4*1 + 1, 6 + 4*1 + 2,
                6 + 4*1 + 1, 6 + 4*1 + 2, 6 + 4*1 + 3,

                6 + 4*2 + 0, 6 + 4*2 + 1, 6 + 4*2 + 2,
                6 + 4*2 + 1, 6 + 4*2 + 2, 6 + 4*2 + 3,
            };

            mesh.vertices = vertices;
            mesh.normals = normals;
            //mesh.uv = uvs;
            mesh.triangles = triangles;

            if (color.HasValue) {
                Color[] cols = new Color[vertices.Length];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateDoubleSplitBox(float sz, float height1, float height2, int corner, Color? color = null) {
            Mesh mesh = new Mesh();

            float xSize = sz;
            float zSize = sz;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Main
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 p0 = new Vector3(-xSize * .5f, 0,  zSize * .5f);
            Vector3 p1 = new Vector3( xSize * .5f, 0,  zSize * .5f);
            Vector3 p2 = new Vector3(-xSize * .5f, 0, -zSize * .5f);
            Vector3 p3 = new Vector3( xSize * .5f, 0, -zSize * .5f);
            vertices.Add(p0); // Bottom
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            normals.Add(down);
            normals.Add(down);
            normals.Add(down);
            normals.Add(down);
            triangles.Add(1);
            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(3);

            for (int i = 0; i < 2; i++) {
                int curCorner = (corner + i * 2) % 4;
                float rotate = 90f * (2 - curCorner);
                float curHeight = i == 0 ? height1 : height2;
                Vector3 p0b = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, 0, -zSize * .5f);
                Vector3 p1b = Quaternion.Euler(0, rotate, 0) * new Vector3( xSize * .5f, 0,  zSize * .5f);
                Vector3 p2b = Quaternion.Euler(0, rotate, 0) * new Vector3( xSize * .5f, 0, -zSize * .5f);
                Vector3 p3b = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, 0, -zSize * .5f);

                Vector3 p0t = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, curHeight, -zSize * .5f);
                Vector3 p1t = Quaternion.Euler(0, rotate, 0) * new Vector3( xSize * .5f, curHeight,  zSize * .5f);
                Vector3 p2t = Quaternion.Euler(0, rotate, 0) * new Vector3( xSize * .5f, curHeight, -zSize * .5f);
                Vector3 p3t = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, curHeight, -zSize * .5f);

                int st = vertices.Count;
                vertices.AddRange(new Vector3[] {
                    p1t, p2t, p3t, // Top
                    p0b, p1b, p0t, p1t, // Diagonal
                    p1b, p2b, p1t, p2t, // Side 1
                    p2b, p3b, p2t, p3t, // Side 2
                });

                Vector3 diagonal = Quaternion.Euler(0, 45 + rotate, 0) * Vector3.forward;
                Vector3 side1 = Quaternion.Euler(0, rotate, 0) * Vector3.right;
                Vector3 side2 = Quaternion.Euler(0, rotate, 0) * Vector3.back;

                /* Rotation: (corner = i => that corner is missing)
                 * 0: UP LEFT
                 * 1: UP RIGHT
                 * 2: DOWN RIGHT
                 * 3: DOWN LEFT
                 * */
                normals.AddRange(new Vector3[] {
                    up, up, up,
                    diagonal, diagonal, diagonal, diagonal,
                    side1, side1, side1, side1,
                    side2, side2, side2, side2,
                });

                triangles.AddRange(new int[] {
                    st + 0, st + 1, st + 2, // Up
                    st+3 + 4*0 + 0, st+3 + 4*0 + 1, st+3 + 4*0 + 2,
                    st+3 + 4*0 + 2, st+3 + 4*0 + 1, st+3 + 4*0 + 3,

                    st+3 + 4*1 + 0, st+3 + 4*1 + 1, st+3 + 4*1 + 2,
                    st+3 + 4*1 + 2, st+3 + 4*1 + 1, st+3 + 4*1 + 3,

                    st+3 + 4*2 + 0, st+3 + 4*2 + 1, st+3 + 4*2 + 2,
                    st+3 + 4*2 + 2, st+3 + 4*2 + 1, st+3 + 4*2 + 3,
                });
            }

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            //mesh.uv = uvs;
            mesh.SetTriangles(triangles,0);

            if (color.HasValue) {
                Color[] cols = new Color[vertices.Count];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateSlopeCornerOutward(float sz, float height1, float height2, int rot, Color? color = null) {
            Mesh mesh = new Mesh();

            float xSize = sz;
            float zSize = sz;
            float rotate = 90f * rot;
            Quaternion rotq = Quaternion.Euler(0, rotate, 0);
            Vector3 center = new Vector3(0.5f * xSize, 0, 0.5f * zSize);

            // Main
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;
            Vector3 p0 = new Vector3(0, 0, zSize);
            Vector3 p1 = new Vector3(xSize, 0, zSize);
            Vector3 p2 = new Vector3(xSize, 0, 0);
            Vector3 p3 = new Vector3(0, 0, 0);
            Vector3 p4 = new Vector3(0, height1, zSize);
            Vector3 p5 = new Vector3(xSize, height1, zSize);
            Vector3 p6 = new Vector3(xSize, height1, 0);
            Vector3 p7 = new Vector3(0, height2, 0);
            Vector3 normal1 = Vector3.Cross(p5 - p6, p5 - p7).normalized;
            Vector3 normal2 = Vector3.Cross(p5 - p7, p5 - p4).normalized;

            Vector3[] vertices = new Vector3[] {
                p0, p1, p2, p3,
                p7, p4, p0, p3,
                p4, p5, p1, p0,
                p6, p7, p3, p2,
                p5, p6, p2, p1,
                p7, p6, p5,
                p7, p4, p5,
            };
            Vector3[] normals = new Vector3[] {
                down, down, down, down,
                left, left, left, left,
                front, front, front, front,
                back, back, back, back,
                right, right, right, right,
                normal1, normal1, normal1,
                normal2, normal2, normal2
            };
            int[] triangles = new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                2 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                4 + 4 * 5, 5 + 4 * 5, 3 + 4 * 5,
            };

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = rotq * (vertices[i] - center);
                normals[i] = rotq * normals[i];
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            //mesh.uv = uvs;
            mesh.triangles = triangles;

            if (color.HasValue) {
                Color[] cols = new Color[vertices.Length];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }


        public static Mesh CreateRamp(float sz, float height1, float height2, int parts, int rot, Color? color = null) {
            Mesh mesh = new Mesh();

            float xSize = sz;
            float zSize = sz;
            float rotate = 90f * (2 + rot);

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Main
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Quaternion.Euler(0, rotate, 0) * Vector3.forward;
            Vector3 back = Quaternion.Euler(0, rotate, 0) * Vector3.back;
            Vector3 left = Quaternion.Euler(0, rotate, 0) * Vector3.left;
            Vector3 right = Quaternion.Euler(0, rotate, 0) * Vector3.right;
            Vector3 p0 = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, 0, zSize * .5f);
            Vector3 p1 = Quaternion.Euler(0, rotate, 0) * new Vector3(xSize * .5f, 0, zSize * .5f);
            Vector3 p2 = Quaternion.Euler(0, rotate, 0) * new Vector3(xSize * .5f, 0, -zSize * .5f);
            Vector3 p3 = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, 0, -zSize * .5f);
            Vector3 p4 = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, height1, zSize * .5f);
            Vector3 p5 = Quaternion.Euler(0, rotate, 0) * new Vector3(xSize * .5f, height1, zSize * .5f);
            Vector3 p6 = Quaternion.Euler(0, rotate, 0) * new Vector3(xSize * .5f, height1, -zSize * .5f);
            Vector3 p7 = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, height1, -zSize * .5f);

            const int vertStartIndex = 5 * 4; // Fill 4 extra sides
            const int backVertIndexForLeftSide = 5;
            const int backVertIndexForRightSide = 16;
            const int leftVertIndexForBackSide = 8;
            const int rightVertIndexForBackSide = 9;
            vertices.AddRange(new Vector3[] {
                p0, p1, p2, p3,
                p7, p4, p0, p3,
                p4, p5, p1, p0,
                p6, p7, p3, p2,
                p5, p6, p2, p1,
            });
            normals.AddRange(new Vector3[] {
                down, down, down, down,
                left, left, left, left,
                front, front, front, front,
                back, back, back, back,
                right, right, right, right,
            });
            triangles.AddRange(new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
            });


            for(int i = 0; i < parts+1; i++) {
                float cos = Mathf.Cos(Mathf.Deg2Rad * (-90f + (i / (float)parts) * 90f));
                float sin = Mathf.Sin(Mathf.Deg2Rad * (-90f + (i / (float)parts) * 90f));
                float curHeight = height2 + (height2 - height1) * sin;
                Vector3 p0v = Quaternion.Euler(0, rotate, 0) * new Vector3(-xSize * .5f, curHeight, -zSize * .5f + zSize * cos);
                Vector3 p1v = Quaternion.Euler(0, rotate, 0) * new Vector3( xSize * .5f, curHeight, -zSize * .5f + zSize * cos);
                vertices.Add(p0v); // Ramp
                vertices.Add(p1v);
                vertices.Add(p0v); // Side 1
                vertices.Add(p1v); // Side 2
                Vector3 p0n = Quaternion.Euler((parts - i) * 90f / parts, rotate, 0) * Vector3.back;
                Vector3 p1n = Quaternion.Euler((parts - i) * 90f / parts, rotate, 0) * Vector3.back;
                normals.Add(p0n); // Ramp
                normals.Add(p1n);
                normals.Add(left);
                normals.Add(right);
                if (i > 0) {
                    // Ramp
                    triangles.Add(vertStartIndex + (i * 4) + 0);
                    triangles.Add(vertStartIndex + (i * 4) + 1);
                    triangles.Add(vertStartIndex + ((i - 1) * 4) + 0);
                    triangles.Add(vertStartIndex + ((i - 1) * 4) + 0);
                    triangles.Add(vertStartIndex + (i * 4) + 1);
                    triangles.Add(vertStartIndex + ((i - 1) * 4) + 1);
                    // Left side
                    triangles.Add(backVertIndexForLeftSide);
                    triangles.Add(vertStartIndex + (i * 4) + 2);
                    triangles.Add(vertStartIndex + ((i-1) * 4) + 2);
                    // Right side
                    triangles.Add(backVertIndexForRightSide);
                    triangles.Add(vertStartIndex + ((i - 1) * 4) + 3);
                    triangles.Add(vertStartIndex + (i * 4) + 3);
                }
                if (i == parts) {
                    vertices.Add(p0v); // Back
                    vertices.Add(p1v); // Back
                    normals.Add(front);
                    normals.Add(front);
                    triangles.Add(vertStartIndex + (i * 4) + 4);
                    triangles.Add(leftVertIndexForBackSide);
                    triangles.Add(vertStartIndex + (i * 4) + 5);
                    triangles.Add(vertStartIndex + (i * 4) + 5);
                    triangles.Add(leftVertIndexForBackSide);
                    triangles.Add(rightVertIndexForBackSide);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            //mesh.uv = uvs;
            mesh.SetTriangles(triangles, 0);

            if (color.HasValue) {
                Color[] cols = new Color[vertices.Count];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateRampCornerInward(float sz, float height1, float height2, int parts, int rot, Color? color = null) {
            Mesh mesh = new Mesh();

            float xSize = sz;
            float zSize = sz;
            float rotate = 90f * (1 + rot);
            Quaternion rotq = Quaternion.Euler(0, rotate, 0);
            Vector3 center = new Vector3(0.5f * xSize, 0, 0.5f * zSize);

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Main
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;
            Vector3 p0 = new Vector3(0, 0, zSize);
            Vector3 p1 = new Vector3(xSize, 0, zSize);
            Vector3 p2 = new Vector3(xSize, 0, 0);
            Vector3 p3 = new Vector3(0, 0, 0);
            Vector3 p4 = new Vector3(0, height1, zSize);
            Vector3 p5 = new Vector3(xSize, height1, zSize);
            Vector3 p6 = new Vector3(xSize, height1, 0);
            Vector3 p7 = new Vector3(0, height1, 0);
            Vector3 heightDiff = new Vector3(0, height2 - height1, 0);

            const int vertStartIndex = 5 * 4; // Fill 5 extra sides
            vertices.AddRange(new Vector3[] {
                p0, p1, p2, p3,
                p7, p4, p0, p3,
                p4 + heightDiff, p5 + heightDiff, p1, p0,
                p6, p7, p3, p2,
                p5 + heightDiff, p6 + heightDiff, p2, p1,
            });
            normals.AddRange(new Vector3[] {
                down, down, down, down,
                left, left, left, left,
                front, front, front, front,
                back, back, back, back,
                right, right, right, right,
            });
            triangles.AddRange(new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
            });


            for (int j = 0; j < parts + 1; j++) {
                for (int i = 0; i < parts + 1; i++) {
                    float cos = Mathf.Cos(Mathf.Deg2Rad * (-90f + (i / (float)parts) * 90f));
                    float sin = Mathf.Sin(Mathf.Deg2Rad * (-90f + (i / (float)parts) * 90f));
                    float curHeight = height2 + (height2 - height1) * sin;
                    Vector3 p0v = Quaternion.Euler(0, j * 90f / parts, 0) * new Vector3(0, curHeight, zSize * cos);
                    vertices.Add(p0v); // Ramp
                    Vector3 p0n = Quaternion.Euler((parts - i) * 90f / parts, j * 90f / parts, 0) * Vector3.back;
                    normals.Add(p0n); // Ramp
                    if (j > 0 && i > 0) {
                        triangles.Add(vertStartIndex + (j - 1) * (parts + 1) + (i - 1));
                        triangles.Add(vertStartIndex + (j - 1) * (parts + 1) + i);
                        triangles.Add(vertStartIndex + j * (parts + 1) + (i - 1));

                        triangles.Add(vertStartIndex + j * (parts + 1) + i);
                        triangles.Add(vertStartIndex + j * (parts + 1) + (i - 1));
                        triangles.Add(vertStartIndex + (j - 1) * (parts + 1) + i);
                    }
                }
            }
            // Top
            vertices.Add(p5 + heightDiff);
            normals.Add(up);
            int currentIndex = vertStartIndex + (parts + 1) * (parts + 1);
            for (int i = 0; i < parts + 1; i++) {
                vertices.Add(vertices[vertStartIndex + i * (parts + 1) + parts]);
                normals.Add(up);
            }
            for (int i = 0; i < parts; i++) {
                triangles.Add(currentIndex);
                triangles.Add(currentIndex + i + 2);
                triangles.Add(currentIndex + i + 1);
            }
            currentIndex += parts + 2;

            Vector3[] vertArray = vertices.ToArray();
            Vector3[] normalArray = normals.ToArray();

            for (int i = 0; i < vertArray.Length; i++) {
                vertArray[i] = rotq * (vertArray[i] - center);
                normalArray[i] = rotq * normalArray[i];
            }

            mesh.vertices = vertArray;
            mesh.normals = normalArray;
            //mesh.uv = uvs;
            mesh.SetTriangles(triangles, 0);

            if (color.HasValue) {
                Color[] cols = new Color[vertArray.Length];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateRampCornerOutward(float sz, float height1, float height2, int parts, int rot, Color? color = null) {
            Mesh mesh = new Mesh();

            float xSize = sz;
            float zSize = sz;
            float rotate = 90f * (1 + rot);
            Quaternion rotq = Quaternion.Euler(0, rotate, 0);
            Vector3 center = new Vector3(0.5f * xSize, 0, 0.5f * zSize);

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Main
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;
            Vector3 p0 = new Vector3(0, 0, zSize);
            Vector3 p1 = new Vector3(xSize, 0, zSize);
            Vector3 p2 = new Vector3(xSize, 0, 0);
            Vector3 p3 = new Vector3(0, 0, 0);
            Vector3 p4 = new Vector3(0, height1, zSize);
            Vector3 p5 = new Vector3(xSize, height1, zSize);
            Vector3 p6 = new Vector3(xSize, height1, 0);
            Vector3 p7 = new Vector3(0, height1, 0);

            const int vertStartIndex = 5 * 4; // Fill 5 extra sides
            vertices.AddRange(new Vector3[] {
                p0, p1, p2, p3,
                p7, p4, p0, p3,
                p4, p5, p1, p0,
                p6, p7, p3, p2,
                p5, p6, p2, p1,
            });
            normals.AddRange(new Vector3[] {
                down, down, down, down,
                left, left, left, left,
                front, front, front, front,
                back, back, back, back,
                right, right, right, right,
            });
            triangles.AddRange(new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
            });


            for (int j = 0; j < parts + 1; j++) {
                for (int i = 0; i < parts + 1; i++) {
                    float cos = Mathf.Cos(Mathf.Deg2Rad * (-90f + (i / (float)parts) * 90f));
                    float sin = Mathf.Sin(Mathf.Deg2Rad * (-90f + (i / (float)parts) * 90f));
                    float curHeight = height2 + (height2 - height1) * sin;
                    Vector3 p0v = Quaternion.Euler(0, j * 90f / parts, 0) * new Vector3(0, curHeight, zSize - zSize * cos);
                    vertices.Add(p0v); // Ramp
                    Vector3 p0n = Quaternion.Euler(i * 90f / parts, j * 90f / parts, 0) * Vector3.up;
                    normals.Add(p0n); // Ramp
                    if (j > 0 && i > 0) {
                        triangles.Add(vertStartIndex + (j - 1) * (parts + 1) + i);
                        triangles.Add(vertStartIndex + (j - 1) * (parts + 1) + (i - 1));
                        triangles.Add(vertStartIndex + j * (parts + 1) + (i - 1));

                        triangles.Add(vertStartIndex + j * (parts + 1) + (i - 1));
                        triangles.Add(vertStartIndex + j * (parts + 1) + i);
                        triangles.Add(vertStartIndex + (j - 1) * (parts + 1) + i);
                    }
                }
            }
            // Bottom
            vertices.Add(p5);
            normals.Add(up);
            int currentIndex = vertStartIndex + (parts + 1) * (parts + 1);
            for (int i = 0; i < parts; i++) {
                triangles.Add(currentIndex);
                triangles.Add(vertStartIndex + (i + 1) * (parts + 1));
                triangles.Add(vertStartIndex + i * (parts + 1));
            }
            currentIndex++;

            // Side 1
            vertices.Add(p7);
            normals.Add(left);
            for (int i = 0; i < parts + 1; i++) {
                vertices.Add(vertices[vertStartIndex+i]);
                normals.Add(left);
            }
            for (int i = 0; i < parts; i++) {
                triangles.Add(currentIndex);
                triangles.Add(currentIndex + i + 1);
                triangles.Add(currentIndex + i + 2);
            }
            currentIndex += parts + 2;

            // Side 2
            vertices.Add(p7);
            normals.Add(back);
            for (int i = 0; i < parts + 1; i++) {
                vertices.Add(vertices[vertStartIndex + parts * (parts + 1) + i]);
                normals.Add(back);
            }
            for (int i = 0; i < parts; i++) {
                triangles.Add(currentIndex);
                triangles.Add(currentIndex + i + 2);
                triangles.Add(currentIndex + i + 1);
            }

            Vector3[] vertArray = vertices.ToArray();
            Vector3[] normalArray = normals.ToArray();

            for (int i = 0; i < vertArray.Length; i++) {
                vertArray[i] = rotq * (vertArray[i] - center);
                normalArray[i] = rotq * normalArray[i];
            }

            mesh.vertices = vertArray;
            mesh.normals = normalArray;
            //mesh.uv = uvs;
            mesh.SetTriangles(triangles, 0);

            if (color.HasValue) {
                Color[] cols = new Color[vertArray.Length];
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = color.Value;
                }
                mesh.colors = cols;
            }

            mesh.RecalculateBounds();
            return mesh;
        }


        #region Textures

        public static Texture2D CreateDummyTexture() {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f));
            texture.Apply();
            return texture;
        }

        public static Texture2D WhiteTexture() {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return tex;
        }
        public static Texture2D GrayTexture() {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.gray);
            tex.Apply();
            return tex;
        }

        public static Texture2D CreateDummyCheckerTexture() {
            Texture2D texture = new Texture2D(2, 2);
            Color col1 = Color.white;
            Color col2 = new Color(0.9f, 0.9f, 0.9f, 1f); // very light grey
            texture.SetPixel(0, 0, col1);
            texture.SetPixel(1, 1, col1);
            texture.SetPixel(0, 1, col2);
            texture.SetPixel(1, 0, col2);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        public static Texture2D CreateDummyLineTexture() {
            Texture2D texture = new Texture2D(2, 2);
            Color col1 = Color.white;
            Color col2 = new Color(0.9f, 0.9f, 0.9f, 1f); // very light grey
            texture.SetPixel(0, 0, col1);
            texture.SetPixel(1, 1, col2);
            texture.SetPixel(0, 1, col1);
            texture.SetPixel(1, 0, col2);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

		#endregion
	}
}