using _3d_editor.Shaders;
using _3d_editor.View;
using OpenTK.Graphics.OpenGL;

namespace _3d_editor.Geometric_figures
{
    abstract class Figure(string vertexPath, string fragmentPath, Camera Camera) : IDisposable
    {
        protected readonly int VAO = GL.GenVertexArray();
        protected readonly int VBO = GL.GenBuffer();
        protected readonly int EBO = GL.GenBuffer();

        protected readonly Shader Shader = new(vertexPath, fragmentPath);
        protected readonly Camera Camera = Camera;

        private bool _disposedValue = false;

        public abstract void Update(int width, int height, float deltaTime);
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
