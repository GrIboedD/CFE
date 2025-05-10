using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class CoordinateGrid : Figure
    {
        private readonly float[] Vertices = [-1, 0, 1, 1, 0, 1, 1, 0, -1, -1, 0, -1];

        private readonly uint[] Indices = [0, 1, 2, 2, 3, 0];

        private readonly float usualWidth = 0.02f;

        public float step = 0.5f;

        public float yCord = 0.0f;

        private Vector4 color = new(0.1f, 0.1f, 0.1f, 1.0f);

        private const float maxCord = 1000;

        private readonly float fogDistance = 1;

        private readonly float fogFactor = 0.1f;


        public CoordinateGrid(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {

            BindBuffers(Vertices, Indices);

            int vertexLocation = Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vertexLocation);

            Shader.Use();
            Shader.SetVec("color", color);
            Shader.SetValue("fogDistance", fogDistance);
            Shader.SetValue("fogFactor", fogFactor);



        }

        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos)
        {
            Shader.Use();
            Shader.SetMatrix("view", viewMatrix);
            Shader.SetMatrix("projection", projectionMatrix);
            Shader.SetVec("cameraPos", CameraPos);

            float width = (step / 0.5f) * usualWidth;
            Shader.SetValue("lineWidth", width);
            Shader.SetValue("gridStep", step);
            Shader.SetMatrix("model", Matrix4.CreateScale(maxCord) * Matrix4.CreateTranslation(0, yCord, 0));
        }

        public override void Draw(int index = -1)
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Disable(EnableCap.CullFace);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public Vector3? RayCasting(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float denom = Vector3.Dot(rayDirection, Vector3.UnitY);
            if (Math.Abs(denom) > 0.01)
            {
                Vector3 p0 = new(0, yCord, 0);
                float t = Vector3.Dot(p0 - rayOrigin, Vector3.UnitY) / denom;

                if (t <= 0) return null;

                Vector3 point = rayOrigin + rayDirection * t;
                int xSteps = (int)Math.Round(point.X / step);
                int zSteps = (int)Math.Round(point.Z / step);
                point.X = step * xSteps;
                point.Y = yCord;
                point.Z = step * zSteps;
                Console.WriteLine(point);
                return point;
            }

            return null;
        }
    }


}
