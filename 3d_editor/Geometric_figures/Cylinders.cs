using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class Cylinders : Figure
    {

        private static class Material
        {
            public static readonly Vector4 color = new(0.5f, 0.5f, 0.5f, 1);
            public static readonly Vector3 ambient = new(0.4f);
            public static readonly Vector3 diffuse = new(0.7f);
            public static readonly Vector3 specular = new(0.2f);
            public const float shininess = 64;
        }

        private float[] Vertices { get; init; }
        private uint[] Indices { get; init; }

        private const int sectorsCount = 200;

        private readonly string directoryPath = Path.Combine("cache", "meshes");

        private readonly string filename = $"S{sectorsCount}Cylinder";

       public Cylinders(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {
            //var loadedData = LoadMeshes(directoryPath, filename);
            //if (loadedData.HasValue)
            //{
            //    this.Vertices = loadedData.Value.Vertices;
            //    this.Indices = loadedData.Value.Indices;
            //}
            //else
            //{
            //    CylinderGeometry gemetry = new(sectorsCount);
            //    this.Vertices = gemetry.GetVertices();
            //    this.Indices = gemetry.GetIndices();
            //}

            CylinderGeometry gemetry = new(sectorsCount);
            this.Vertices = gemetry.GetVertices();
            this.Indices = gemetry.GetIndices();

            SaveMeshes(directoryPath, filename, Vertices, Indices);

            BindBuffers(Vertices, Indices);

            int vertexLocation = Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
                6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            int normalsLocation = Shader.GetAttribLocation("aNormal");
            GL.VertexAttribPointer(normalsLocation, 3, VertexAttribPointerType.Float, false,
                6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(normalsLocation);

            Matrix4 model = Matrix4.CreateScale(1, 0.15f, 0.15f);
            Matrix3 normal = new(Matrix4.Transpose(Matrix4.Invert(model)));

            Shader.Use();
            Shader.SetMatrix("model", model);
            Shader.SetMatrix("normalMatrix", normal);
            Shader.SetVec("material.color", Material.color);
        }

        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos)
        {
            this.Shader.Use();

            this.Shader.SetMatrix("view", viewMatrix);
            this.Shader.SetMatrix("projection", projectionMatrix);

            this.Shader.SetVec("viewPos", CameraPos);

            this.Shader.SetVec("light.direction", Light.Direction);
            this.Shader.SetVec("light.ambient", Light.Ambient);
            this.Shader.SetVec("light.diffuse", Light.Diffuse);
            this.Shader.SetVec("light.specular", Light.Specular);

            this.Shader.SetVec("material.ambient", Material.ambient);
            this.Shader.SetVec("material.diffuse", Material.diffuse);
            this.Shader.SetVec("material.specular", Material.specular);
            this.Shader.SetValue("material.shininess", Material.shininess);
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
