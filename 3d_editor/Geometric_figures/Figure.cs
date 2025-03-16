using OpenTK.Graphics.OpenGL;

namespace _3d_editor.Geometric_figures
{
    abstract class Figure(float[] Vertices, uint[] Indices, Shader shader) : IDisposable
    {
        protected readonly int _VAO = GL.GenVertexArray();
        protected readonly int _VBO = GL.GenBuffer();
        protected readonly int _EBO = GL.GenBuffer();

        protected float[] Vertices { get; init; } = Vertices;
        protected uint[] Indices { get; init; } = Indices;

        protected Shader Figure_Shader { get; init; } = shader;

        private bool _disposedValue = false;

        public abstract void Update();
        public abstract void Draw();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                GL.DeleteVertexArray(_VAO);
                GL.DeleteBuffer(_VBO);
                GL.DeleteBuffer(_EBO);
                Figure_Shader.Dispose();
                _disposedValue = true;
            }
        }

        ~Figure()
        {
            if (!_disposedValue)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }



    }
}
