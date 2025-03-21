using OpenTK.Graphics.OpenGL;

namespace _3d_editor.Geometric_figures
{

    class Sphere : Figure
    {

        private static readonly float _t = (float)((1.0 + Math.Sqrt(5.0)) / 2.0);



        private static readonly float[] _vertices =
{

         0.5f,  0.5f, 0.0f, // top right
         0.5f, -0.5f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f,// bottom left
        -0.5f,  0.5f, 0.0f, // top left
        };

        private static readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };







        //private static readonly float[] _vertices =
        //{
        //    -1.0f,  _t, 0.0f,
        //     1.0f,  _t, 0.0f,
        //    -1.0f, -_t, 0.0f,
        //     1.0f, -_t, 0.0f,

        //     0.0f, -1.0f, _t,
        //     0.0f,  1.0f, _t,
        //     0.0f, -1.0f, _t,
        //     0.0f,  1.0f, _t,

        //     _t, 0.0f, -1.0f,
        //     _t, 0.0f,  1.0f,
        //    -_t, 0.0f, -1.0f,
        //    -_t, 0.0f,  1.0f,
        //};

        //private static readonly uint[] _indices =
        //{
        //    0,  11, 5,
        //    0,  5,  1,
        //    0,  1,  7,
        //    0,  7,  10,
        //    0,  10, 11,

        //    1,  5,  9,
        //    5,  11, 4,
        //    11, 10, 2,
        //    10, 7,  6,
        //    7,  1,  8,

        //    3,  9,  4,
        //    3,  4,  2,
        //    3,  2,  6,
        //    3,  6,  8,
        //    3,  8,  9,

        //    4,  9,  5,
        //    2,  4,  11,
        //    6,  2,  10,
        //    8,  6,  7,
        //    9,  8,  1,
        //};




        private float[] Vertices { get; init; }
        private uint[] Indices { get; init; }

        public Sphere(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {
            this.Vertices = _vertices;
            this.Indices = _indices;

            this.Shader.Use();

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

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
