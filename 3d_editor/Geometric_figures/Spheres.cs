﻿using _3d_editor.Shaders;
using _3d_editor.Textures;
using _3d_editor.View;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{

    class Spheres : Figure
    {

        private class Geometry
        {

            private struct TriangleIndices(int v1, int v2, int v3)
            {
                public int v1 = v1;
                public int v2 = v2;
                public int v3 = v3;
            }

            private readonly List<Vector3> vertices = [];

            private readonly List<TriangleIndices> indices = [];

            private readonly Dictionary<Int64, int> middlePointIndexCache = [];

            private static readonly float t = (float)((1.0 + Math.Sqrt(5.0)) / 2.0);

            private int AddVertex(float x, float y, float z)
            {
                float length = (float)Math.Sqrt(x * x + y * y + z * z);
                vertices.Add(new Vector3(x, y, z) / length);
                return vertices.Count - 1;
            }

            private int GetMiddlePoint(int v1, int v2)
            {
                bool firstSmaller = v1 < v2;
                Int64 smallerIndex = firstSmaller ? v1 : v2;
                Int64 greaterIndex = firstSmaller ? v2 : v1;
                Int64 key = (smallerIndex << 32) + greaterIndex;

                if (middlePointIndexCache.TryGetValue(key, out int value))
                {
                    return value;
                }

                Vector3 vertex1 = vertices[v1];
                Vector3 vertex2 = vertices[v2];
                float x = (vertex1.X + vertex2.X) / 2.0f;
                float y = (vertex1.Y + vertex2.Y) / 2.0f;
                float z = (vertex1.Z + vertex2.Z) / 2.0f;


                int index = AddVertex(x, y, z);
                middlePointIndexCache.Add(key, index);
                return index;
            }

            public Geometry(int recursionLevel)
            {

                AddVertex(-1.0f, t, 0.0f);
                AddVertex(1.0f, t, 0.0f);
                AddVertex(-1.0f, -t, 0.0f);
                AddVertex(1.0f, -t, 0.0f);

                AddVertex(0.0f, -1.0f, t);
                AddVertex(0.0f, 1.0f, t);
                AddVertex(0.0f, -1.0f, -t);
                AddVertex(0.0f, 1.0f, -t);

                AddVertex(t, 0.0f, -1.0f);
                AddVertex(t, 0.0f, 1.0f);
                AddVertex(-t, 0.0f, -1.0f);
                AddVertex(-t, 0.0f, 1.0f);

                List<TriangleIndices> faces = [];
                faces.Add(new TriangleIndices(0, 11, 5));
                faces.Add(new TriangleIndices(0, 5, 1));
                faces.Add(new TriangleIndices(0, 1, 7));
                faces.Add(new TriangleIndices(0, 7, 10));
                faces.Add(new TriangleIndices(0, 10, 11));

                faces.Add(new TriangleIndices(1, 5, 9));
                faces.Add(new TriangleIndices(5, 11, 4));
                faces.Add(new TriangleIndices(11, 10, 2));
                faces.Add(new TriangleIndices(10, 7, 6));
                faces.Add(new TriangleIndices(7, 1, 8));

                faces.Add(new TriangleIndices(3, 9, 4));
                faces.Add(new TriangleIndices(3, 4, 2));
                faces.Add(new TriangleIndices(3, 2, 6));
                faces.Add(new TriangleIndices(3, 6, 8));
                faces.Add(new TriangleIndices(3, 8, 9));

                faces.Add(new TriangleIndices(4, 9, 5));
                faces.Add(new TriangleIndices(2, 4, 11));
                faces.Add(new TriangleIndices(6, 2, 10));
                faces.Add(new TriangleIndices(8, 6, 7));
                faces.Add(new TriangleIndices(9, 8, 1));

                for (int i = 0; i < recursionLevel; i++)
                {
                    List<TriangleIndices> newFaces = [];
                    foreach (var tri in faces)
                    {
                        int a = GetMiddlePoint(tri.v1, tri.v2);
                        int b = GetMiddlePoint(tri.v2, tri.v3);
                        int c = GetMiddlePoint(tri.v3, tri.v1);

                        newFaces.Add(new TriangleIndices(tri.v1, a, c));
                        newFaces.Add(new TriangleIndices(tri.v2, b, a));
                        newFaces.Add(new TriangleIndices(tri.v3, c, b));
                        newFaces.Add(new TriangleIndices(a, b, c));
                    }
                    faces = newFaces;
                }

                indices = faces;

            }
                
            private Vector2 GetTextureCoordinates(Vector3 vertex)
            {

                float u = (float)(Math.Asin(vertex.X) / Math.PI + 0.5f);
                float v = (float)(Math.Asin(vertex.Y) / Math.PI + 0.5f);
                return new Vector2(u, v);
            }

            public float[] GetVertices()
            {
                List<float> outVertices = [];
                foreach(var vertex in vertices)
                {
                    outVertices.Add(vertex.X);
                    outVertices.Add(vertex.Y);
                    outVertices.Add(vertex.Z);
                    Vector2 textureCoord = GetTextureCoordinates(vertex);
                    outVertices.Add(textureCoord.X);
                    outVertices.Add(textureCoord.Y);
                }
                return [.. outVertices];
            }

            public uint[] GetIndices()
            {
                List<uint> outIndices = [];
                foreach(var tri in indices)
                {
                    outIndices.Add((uint)tri.v1);
                    outIndices.Add((uint)tri.v2);
                    outIndices.Add((uint)tri.v3);
                }
                return [.. outIndices];
            }

        }

        private class Sphere(float x, float y, float z, float radius)
        {
            private Vector3 position = new(x, y, z);
            private float radius = radius;
            private Matrix4 modelMatrix = Matrix4.CreateScale(radius) * Matrix4.CreateTranslation(x, y, z);

            private void CalculateModelMatrix()
            {
                modelMatrix = Matrix4.CreateScale(radius) * Matrix4.CreateTranslation(position);
            }

            public void SetPosition(float x, float y, float z)
            {
                position = new(x, y, z);
                CalculateModelMatrix();
            }

            public void SetRadius(float radius)
            {
                this.radius = radius;
                CalculateModelMatrix();
            }

            public Matrix4 GetModelMatrix()
            {
                return modelMatrix;
            }
        }

        private float[] Vertices { get; init; }
        private uint[] Indices { get; init; }

        private const int recursionLevel = 0;

        private readonly List<Sphere> SpheresList = [];

        private readonly Texture texture = new("D:\\c#projects\\CFE\\3d_editor\\awesomeface.png");

        public void CreateNewSphere(float x, float y, float z, float radius)
        {
            var sphere = new Sphere(x, y, z, radius);
            SpheresList.Add(sphere);
        }

        public Spheres(string vertexPath, string fragmentPath, Camera Camera) : base(vertexPath, fragmentPath, Camera)
        {

            Geometry geometry = new(recursionLevel);
            this.Vertices = geometry.GetVertices();
            this.Indices = geometry.GetIndices();

            GL.BindVertexArray(this.VAO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.Indices.Length * sizeof(uint),
                this.Indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, this.Vertices.Length * sizeof(float),
                this.Vertices, BufferUsageHint.StaticDraw);

            int vertexLocation = this.Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
                5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            int texCoordLocation = this.Shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false,
                5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

        }

        public override void Update(int width, int height)
        {
            Matrix4 view = Camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(float.Pi / 4.0f, width / (float)height, 0.1f, 100.0f);
            this.Shader.SetMatrix4("view", view);
            this.Shader.SetMatrix4("projection", projection);
        }

        public override void Draw()
        {
            if (SpheresList.Count == 0) return;

            this.texture.Use();
            this.Shader.Use();
            GL.Enable(EnableCap.CullFace);
            //GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);

            foreach (var sphere in SpheresList)
            {
                Shader.SetMatrix4("model", sphere.GetModelMatrix());
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }
    }
}
