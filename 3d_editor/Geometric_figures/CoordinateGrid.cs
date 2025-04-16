using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class CoordinateGrid : Figure
    {
        private readonly float[] Vertices;

        private readonly uint[] Indices;

        private readonly uint sectorsCount = 3;

        private readonly float width = 0.006f;

        private readonly float step = 0.25f;

        private readonly float yCord = 0.0f;

        private Vector4 color = new(0.0f, 0.0f, 0.0f, 0.2f);

        private const float maxCord = 100;

        private Matrix4 xModelMatrix;

        private Matrix4 zModelMatrix;

        public CoordinateGrid(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {

            CylinderGeometry cylinderGeometry = new(sectorsCount);
            Vertices = cylinderGeometry.GetVertices();
            Indices = cylinderGeometry.GetIndices();

            BindBuffers(Vertices, Indices);

            int vertexLocation = Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vertexLocation);

            xModelMatrix = Matrix4.CreateScale(maxCord, width, width);
            zModelMatrix = xModelMatrix * Matrix4.CreateRotationY(float.Pi / 2);

            Shader.Use();
            Shader.SetVec("color", color);
        }

        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos)
        {
            Shader.Use();
            Shader.SetMatrix("view", viewMatrix);
            Shader.SetMatrix("projection", projectionMatrix);
        }

        public override void Draw()
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Enable(EnableCap.CullFace);
            for (float i = 0.0f; i <= maxCord; i += step)
            {
                Shader.SetMatrix("model", xModelMatrix * Matrix4.CreateTranslation(0.0f, yCord, i));
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

                Shader.SetMatrix("model", xModelMatrix * Matrix4.CreateTranslation(0.0f, yCord, -i));
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

                Shader.SetMatrix("model", zModelMatrix * Matrix4.CreateTranslation(i, yCord, 0.0f));
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

                Shader.SetMatrix("model", zModelMatrix * Matrix4.CreateTranslation(-i, yCord, 0.0f));
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }
    }


}
