using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{

    class Spheres : Figure
    {

        private static class Material
        {
            public static readonly Vector3 ambient = new(0.4f);
            public static readonly Vector3 diffuse = new(0.7f);
            public static readonly Vector3 specular = new(0.3f);
            public const float shininess = 64;
        }

        private class OneSphere
        {
            public Vector3 Position { get; private set; }
            public float Radius { get; private set; }

            public Vector4 Color { get; set; }
            public Texture Texture { get; private set; }

            public Matrix4 ModelMatrix { get; private set; }

            public Matrix3 NormalMatrix { get; private set; }

            public OneSphere(Vector3 position, float radius, Vector4 color, string text)
            {
                Position = position;
                Radius = radius;
                Color = color;
                Texture = Textures.GetTexture(text);
                ModelMatrix = Matrix4.CreateScale(radius) * Matrix4.CreateTranslation(position);
                NormalMatrix = new Matrix3(Matrix4.Transpose(Matrix4.Invert(ModelMatrix)));
            }

            private void CalculateModelAndNormalMatrix()
            {
                ModelMatrix = Matrix4.CreateScale(Radius) * Matrix4.CreateTranslation(Position);
                NormalMatrix = new Matrix3(Matrix4.Transpose(Matrix4.Invert(ModelMatrix)));
            }

            public void SetPosition(float x, float y, float z)
            {
                Position = new(x, y, z);
                CalculateModelAndNormalMatrix();
            }

            public void SetRadius(float radius)
            {
                this.Radius = radius;
                CalculateModelAndNormalMatrix();
            }

        }

        private float[] Vertices { get; init; }
        private uint[] Indices { get; init; }

        private const int recursionLevel = 5;

        private readonly string directoryPath = Path.Combine("cache", "meshes");

        private readonly string fileName = $"R{recursionLevel}ISOSphere";

        private readonly List<OneSphere> SpheresList = [];

        private static readonly SpheresTexturesManager Textures = new();

        public Spheres(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {

            var loadedData = LoadMeshes(directoryPath, fileName);
            if (loadedData.HasValue)
            {
                this.Vertices = loadedData.Value.Vertices;
                this.Indices = loadedData.Value.Indices;
            }
            else
            {
                ISOSphereGeometry geometry = new(recursionLevel);
                this.Vertices = geometry.GetVertices();
                this.Indices = geometry.GetIndices();
            }

            SaveMeshes(directoryPath, fileName, this.Vertices, this.Indices);

            BindBuffers(Vertices, Indices);

            int vertexLocation = this.Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            int normalsLocation = this.Shader.GetAttribLocation("aNormal");
            GL.VertexAttribPointer(normalsLocation, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(normalsLocation);


        }

        public void CreateNewSphere(Vector3 position, float radius, Color color, string text = "")
        {
            var vec4Color = new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1);
            var sphere = new OneSphere(position, radius, vec4Color, text);
            SpheresList.Add(sphere);
        }
        public override void Update(Matrix4 projectionMatrix, Matrix4 viewMatrix, Vector3 CameraPos)
        {
            Shader.Use();

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
            if (SpheresList.Count == 0) return;

            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Enable(EnableCap.CullFace);

            foreach (var sphere in SpheresList)
            {
                sphere.Texture.Use();
                Shader.SetVec("material.color", sphere.Color);
                Shader.SetMatrix("model", sphere.ModelMatrix);
                Shader.SetMatrix("normalMatrix", sphere.NormalMatrix);
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        public int RayCasting(Vector3 rayOrigin, Vector3 rayDirection)
        {
            List<(int index, float distance)> nearestSpheres = [];

            for (int i = 0; i < SpheresList.Count; i++)
            {
                OneSphere sphere = SpheresList[i];

                Vector3 center = sphere.Position;
                float r = sphere.Radius;

                Vector3 distanceVector = center - rayOrigin;

                if (distanceVector.Length <= r) return i;

                float distanceAlongRay = Vector3.Dot(rayDirection, distanceVector);

                if (distanceAlongRay < 0) continue;

                float distance = r * r + distanceAlongRay * distanceAlongRay - distanceVector.LengthSquared;
                if (distance >= 0)
                {
                    float t = distanceAlongRay - (float)Math.Sqrt(distance);
                    nearestSpheres.Add((i, t));
                }
            }

            if (nearestSpheres.Count == 0) return -1;

            return nearestSpheres.MinBy(x => x.distance).index;

        }

        public Vector3 GetSpheresCenterCord(int index)
        {
            return SpheresList[index].Position;
        }

        public float GetSpheresRadius(int index)
        {
            return SpheresList[index].Radius;
        }

        public List<Vector3>? GetListOfSpheresPositions()
        {
            if (SpheresList is null)
                return null;

            List<Vector3> positions = [];
            foreach (var sphere in SpheresList)
            {
                positions.Add(sphere.Position);
            }
            return positions;
        }

        public void DelAllSpheres()
        {
            SpheresList.Clear();
        }

    }
}
