using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class Cylinders : Figure
    {
        private float[] Vertices { get; init; }
        private uint[] Indices { get; init; }

        private const int sectorsCount = 200;

        private readonly string directoryPath = Path.Combine("cache", "meshes");

        private readonly string filename = $"S{sectorsCount}Cylinder";

       public Cylinders(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {
            var loadedData = LoadMeshes(directoryPath, filename);
            if (loadedData.HasValue)
            {
                this.Vertices = loadedData.Value.Vertices;
                this.Indices = loadedData.Value.Indices;
            }
            else
            {
                CylinderGeometry gemetry = new(sectorsCount);
                this.Vertices = gemetry.GetVertices();
                this.Indices = gemetry.GetIndices();
            }

            SaveMeshes(directoryPath, filename, Vertices, Indices);

            BindBuffers(Vertices, Indices);

            int vertexLocation = Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            Shader.Use();
            Shader.SetMatrix("model", Matrix4.CreateScale(1, 0.15f, 0.15f));
        }

        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos)
        {
            this.Shader.Use();
            this.Shader.SetMatrix("view", viewMatrix);
            this.Shader.SetMatrix("projection", projectionMatrix);
        }

        public override void Draw()
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Enable(EnableCap.CullFace);
            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
