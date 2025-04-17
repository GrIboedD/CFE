﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class CoordinateGrid : Figure
    {
        private readonly float[] Vertices = [-1, 0, 1, 1, 0, 1, 1, 0, -1, -1, 0, -1];

        private readonly uint[] Indices = [0, 1, 2, 2, 3, 0];

        private readonly float width = 0.05f;

        private readonly float step = 1.0f;

        private readonly float yCord = 0.0f;

        private Vector4 color = new(0.0f, 0.0f, 0.0f, 1.0f);

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
            Shader.SetValue("gridStep", step);
            Shader.SetValue("lineWidth", width);
            Shader.SetValue("fogDistance", fogDistance);
            Shader.SetValue("fogFactor", fogFactor);

            Shader.SetMatrix("model", Matrix4.CreateScale(maxCord) * Matrix4.CreateTranslation(0, yCord, 0));


        }

        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos)
        {
            Shader.Use();
            Shader.SetMatrix("view", viewMatrix);
            Shader.SetMatrix("projection", projectionMatrix);
            Shader.SetVec("cameraPos", CameraPos);
        }

        public override void Draw()
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Disable(EnableCap.CullFace);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }


}
