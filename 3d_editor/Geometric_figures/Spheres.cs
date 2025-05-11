using Microsoft.VisualBasic.Logging;
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

            public string Text { get; private set; }

            public Matrix4 ModelMatrix { get; private set; }

            public Matrix3 NormalMatrix { get; private set; }

            public OneSphere(Vector3 position, float radius, Vector4 color, string text)
            {
                Position = position;
                Radius = radius;
                Color = color;
                Text = text;
                Texture = Textures.GetTexture(Text);
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

            public void SetText(string text)
            {
                Text = text;
                Texture = Textures.GetTexture(Text);
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

        private bool isTheSphereOverlappingAnother(OneSphere targetSphere)
        {
            Vector3 targetSpherePos = targetSphere.Position;
            float targetSphereradius = targetSphere.Radius;
            foreach (var aSphere in SpheresList)
            {
                if (targetSphere == aSphere)
                {
                    continue;
                }

                Vector3 spherePos = aSphere.Position;
                float sphereRadius = aSphere.Radius;

                float distance = (spherePos - targetSpherePos).Length;

                if (distance < targetSphereradius + sphereRadius)
                {
                    return true;
                }

            }
            return false;
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

        public void DrawPickedSphereOutlining(int index)
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();

            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

            var sphere = SpheresList[index];
            var tempSphere = new OneSphere(
                sphere.Position,
                sphere.Radius + 0.04f,
                new Vector4(0, 1, 0, 1),
                ""
                );

            tempSphere.Texture.Use();
            Shader.SetVec("material.color", tempSphere.Color);
            Shader.SetMatrix("model", tempSphere.ModelMatrix);
            Shader.SetMatrix("normalMatrix", tempSphere.NormalMatrix);


            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.StencilTest);
        }

        public override void Draw(int indexNoDraw = -1)
        {
            if (SpheresList.Count == 0) return;

            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Enable(EnableCap.CullFace);

            for (int i = 0; i < SpheresList.Count; i++)
            {
                if (indexNoDraw == i)
                {
                    continue;
                }
                var sphere = SpheresList[i];
                sphere.Texture.Use();
                Shader.SetVec("material.color", sphere.Color);
                Shader.SetMatrix("model", sphere.ModelMatrix);
                Shader.SetMatrix("normalMatrix", sphere.NormalMatrix);
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);

            }
        }

        public void DrawPickedSphere(int index)
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Replace, StencilOp.Replace);
            var sphere = SpheresList[index];
            sphere.Texture.Use();
            Shader.SetVec("material.color", sphere.Color);
            Shader.SetMatrix("model", sphere.ModelMatrix);
            Shader.SetMatrix("normalMatrix", sphere.NormalMatrix);
            GL.StencilFunc(StencilFunction.Always, 1, 0x00);
            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.StencilTest);
        }

        public (int, float) RayCasting(Vector3 rayOrigin, Vector3 rayDirection)
        {
            List<(int index, float distance)> nearestSpheres = [];

            for (int i = 0; i < SpheresList.Count; i++)
            {
                OneSphere sphere = SpheresList[i];

                Vector3 center = sphere.Position;
                float r = sphere.Radius;

                Vector3 distanceVector = center - rayOrigin;

                if (distanceVector.Length <= r) return (i, 0);

                float distanceAlongRay = Vector3.Dot(rayDirection, distanceVector);

                if (distanceAlongRay < 0) continue;

                float distance = r * r + distanceAlongRay * distanceAlongRay - distanceVector.LengthSquared;
                if (distance >= 0)
                {
                    float t = distanceAlongRay - (float)Math.Sqrt(distance);
                    nearestSpheres.Add((i, t));
                }
            }

            if (nearestSpheres.Count == 0) return (-1, -1);

            return nearestSpheres.MinBy(x => x.distance);

        }

        public Vector3 GetSpheresCenterCord(int index)
        {
            return SpheresList[index].Position;
        }

        public void SetSpheresCenterCord(int index, Vector3 pos)
        {
            Vector3 oldPos = SpheresList[index].Position;
            SpheresList[index].SetPosition(pos.X, pos.Y, pos.Z);
            if (isTheSphereOverlappingAnother(SpheresList[index]))
            {
                SpheresList[index].SetPosition(oldPos.X, oldPos.Y, oldPos.Z);
                throw new InvalidOperationException("The sphere overlaps another sphere");
            }
        }

        public float GetSpheresRadius(int index)
        {
            return SpheresList[index].Radius;
        }

        public void SetSpheresRadius(int index, float radius)
        {
            float oldRadius = SpheresList[index].Radius;
            SpheresList[index].SetRadius(radius);
            if (isTheSphereOverlappingAnother(SpheresList[index]))
            {
                SpheresList[index].SetRadius(oldRadius);
                throw new InvalidOperationException("The sphere overlaps another sphere");
            }
        }

        public Vector4 GetSpheresColor(int index)
        {
            return SpheresList[index].Color;
        }

        public void SetSphereColor(int index, Vector4 color)
        {
            SpheresList[index].Color = color;
        }

        public string GetSpheresText(int index)
        {
            return SpheresList[index].Text;
        }

        public void SetSpheresText(int index, string text)
        {
            SpheresList[index].SetText(text);
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
