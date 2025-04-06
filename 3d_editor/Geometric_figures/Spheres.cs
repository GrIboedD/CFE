using _3d_editor.Shaders;
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

            public float[] GetVertices()
            {
                List<float> outVertices = [];
                foreach(var vertex in vertices)
                {
                    outVertices.Add(vertex.X);
                    outVertices.Add(vertex.Y);
                    outVertices.Add(vertex.Z);
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

        private class Sphere(Vector3 position, float radius, Vector4 color)
        {
            public Vector3 Position { get; private set; } = position;
            public float Radius { get; private set; } = radius;

            public Vector4 Color { get; set; } = color;

            private Matrix4 modelMatrix = Matrix4.CreateScale(radius) * Matrix4.CreateTranslation(position);

            private void CalculateModelMatrix()
            {
                modelMatrix = Matrix4.CreateScale(Radius) * Matrix4.CreateTranslation(Position);
            }

            public void SetPosition(float x, float y, float z)
            {
                Position = new(x, y, z);
                CalculateModelMatrix();
            }

            public void SetRadius(float radius)
            {
                this.Radius = radius;
                CalculateModelMatrix();
            }

            public Matrix4 GetModelMatrix()
            {
                return modelMatrix;
            }
        }

        private float[] Vertices { get; init; }
        private uint[] Indices { get; init; }

        private const int recursionLevel = 5;

        private readonly List<Sphere> SpheresList = [];

        private readonly Texture texture = new("D:\\c#projects\\CFE\\3d_editor\\Textures\\Images\\H.png");

        public void CreateNewSphere(Vector3 position, float radius, Vector4 color)
        {
            var sphere = new Sphere(position, radius, color);
            SpheresList.Add(sphere);
        }

        public Spheres(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
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
                3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);
        }

        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            this.Shader.SetMatrix4("view", viewMatrix);
            this.Shader.SetMatrix4("projection", projectionMatrix);
        }
        
        public override void Draw()
        {
            if (SpheresList.Count == 0) return;

            this.texture.Use();
            this.Shader.Use();
            GL.Enable(EnableCap.CullFace);

            foreach (var sphere in SpheresList)
            {
                Shader.SetVec4("color", sphere.Color);
                Shader.SetMatrix4("model", sphere.GetModelMatrix());
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        public int RayCasting(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Console.WriteLine();
            Console.WriteLine("O: " + rayOrigin.ToString());
            Console.WriteLine("D: " + rayDirection.ToString());
            
            foreach(var sphere in SpheresList)
            {
                Vector3 center = sphere.Position;
                float r = sphere.Radius;

                Console.WriteLine();
                Console.WriteLine("Center: " + center.ToString());
                Console.WriteLine("Radius: " + r.ToString());

                Vector3 distanceVector = center - rayOrigin;

                Console.WriteLine("distence to center " + distanceVector.Length.ToString());

                if (distanceVector.Length <= r)
                {
                    Console.WriteLine($"t: {0}");
                    sphere.Color = new Vector4(0, 1, 0, 1);
                }

                float distanceAlongRay = Vector3.Dot(rayDirection, distanceVector);
                Console.WriteLine("DistAlongRay: " + distanceAlongRay.ToString());

                if (distanceAlongRay < 0) continue;

                float distance = r * r + distanceAlongRay * distanceAlongRay - distanceVector.LengthSquared;
                Console.WriteLine("Dist: " + distance.ToString());
                if (distance >= 0)
                {
                    float t = distanceAlongRay - (float)Math.Sqrt(distance);
                    Console.WriteLine($"t: {t}");
                    sphere.Color = new Vector4(0, 1, 0, 1);
                }

            }
            return 1;
        }

    }
}
