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

        private readonly float sizeFactor = 0.3f;

        private readonly Spheres Spheres;

       public Cylinders(string vertexPath, string fragmentPath, Spheres spheres) : base(vertexPath, fragmentPath)
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

            Spheres = spheres;

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

            HashSet<(int, int)> connectedSpheres = Spheres.GetHashSetOfConnectedSpheres();

            foreach(var couple in connectedSpheres)
            {
                Vector3 vec1 = Spheres.GetSpheresCenterCord(couple.Item1);
                Vector3 vec2 = Spheres.GetSpheresCenterCord(couple.Item2);
                float r = Math.Min(Spheres.GetSpheresRadius(couple.Item1), Spheres.GetSpheresRadius(couple.Item2));

                Vector3 direction = vec2 - vec1;
                float length = direction.Length / 2;
                Vector3 position = (vec1 + vec2) / 2f;
                float size = r * sizeFactor;

                Matrix4 modelScale = Matrix4.CreateScale(length, size, size);
                Matrix4 modelTranslation = Matrix4.CreateTranslation(position);
                Matrix4 modelRotation = GetRotationMatrixFromUnitXtoVector(direction);

                Matrix4 model = modelScale * modelRotation * modelTranslation;
                Matrix3 normal = new(Matrix4.Transpose(Matrix4.Invert(model)));

                Shader.Use();
                Shader.SetMatrix("model", model);
                Shader.SetMatrix("normalMatrix", normal);
                Shader.SetVec("material.color", Material.color);

                GL.Enable(EnableCap.CullFace);
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        private static Matrix4 GetRotationMatrixFromUnitXtoVector(Vector3 vector)
        {
            Vector3 direction = Vector3.Normalize(vector);
            Vector3 rotationAxis = Vector3.Cross(direction, Vector3.UnitX);
            float anlge = (float)MathHelper.Acos(Vector3.Dot(direction, -Vector3.UnitX));
            Quaternion rotator = Quaternion.FromAxisAngle(rotationAxis, anlge);
            return Matrix4.CreateFromQuaternion(rotator);
        }
    }
}
