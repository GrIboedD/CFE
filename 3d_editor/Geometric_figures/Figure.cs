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

        public abstract void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos);
        public abstract void Draw();

        protected static (float[] Vertices, uint[] Indices)? LoadMeshes(string directoryPath, string fileName)
        {
            Directory.CreateDirectory(directoryPath);

            string verticesFileName = fileName + "vertices.dat";
            string indicesFileName = fileName + "indices.dat";

            string verticesFilePath = Path.Combine(directoryPath, verticesFileName);
            string indicesFilePath = Path.Combine(directoryPath,indicesFileName);

            bool filesExist = File.Exists(verticesFilePath) && File.Exists(indicesFilePath);

            if (!filesExist) return null;

            List<float> Vertices = [];
            using (BinaryReader reader = new(File.Open(verticesFilePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                    Vertices.Add(reader.ReadSingle());
            }

            List<uint> Indices = [];
            using (BinaryReader reader = new(File.Open(indicesFilePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                    Indices.Add(reader.ReadUInt32());
            }

            return ([.. Vertices], [.. Indices]);
        }

        protected static void SaveMeshes(string directoryPath, string fileName,
            float[] Vertices, uint[] Indices)
        {
            Directory.CreateDirectory(directoryPath);

            string verticesFileName = fileName + "vertices.dat";
            string indicesFileName = fileName + "indices.dat";

            string verticesFilePath = Path.Combine(directoryPath, verticesFileName);
            string indicesFilePath = Path.Combine(directoryPath, indicesFileName);

            bool filesExist = File.Exists(verticesFilePath) && File.Exists(indicesFilePath);

            if (filesExist) return;

            using (BinaryWriter writer = new(File.Open(verticesFilePath, FileMode.Create)))
            {
                foreach (float value in Vertices)
                    writer.Write(value);
            }

            using (BinaryWriter writer = new(File.Open(indicesFilePath, FileMode.Create)))
            {
                foreach (uint value in Indices)
                    writer.Write(value);
            }
        }

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
