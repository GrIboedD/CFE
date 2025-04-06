using _3d_editor.Shaders;
using _3d_editor.View;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    abstract class Figure(string vertexPath, string fragmentPath) : IDisposable
    {
        protected readonly int VAO = GL.GenVertexArray();
        protected readonly int VBO = GL.GenBuffer();
        protected readonly int EBO = GL.GenBuffer();

        protected readonly Shader Shader = new(vertexPath, fragmentPath);

        private bool _disposedValue = false;

        public abstract void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix);
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
                GL.DeleteVertexArray(VAO);
                GL.DeleteBuffer(VBO);
                GL.DeleteBuffer(EBO);
                Shader.Dispose();
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
