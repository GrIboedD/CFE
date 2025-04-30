using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class Cylinders : Figure
    {

        private static class Material
        {
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

        private readonly List<OneCylinder> CylindersList = [];

       public Cylinders(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
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
            if (CylindersList.Count == 0)
                return;

            BindBuffers(Vertices, Indices);

            Shader.Use();
            GL.Enable(EnableCap.CullFace);

            foreach(var cylinder in CylindersList)
            {
                Shader.SetMatrix("model", cylinder.ModelMatrix);
                Shader.SetMatrix("normalMatrix", cylinder.NormalMatrix);
                Shader.SetVec("material.color", cylinder.Color);
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        private static bool IsVecOneLessVecTwo(Vector3 vecOne, Vector3 vecTwo)
        {
            if (vecOne.X < vecTwo.X)
                return true;
            if (vecOne.X > vecTwo.X)
                return false;

            if (vecOne.Y < vecTwo.Y)
                return true;
            if (vecOne.Y > vecTwo.Y)
                return false;

            if (vecOne.Z < vecTwo.Z)
                return true;
            //The Last check is unnecessary couse the vecs are equal or the vecOne is greater
            return false;
        }

        private bool IsCylinderExists(Vector3 Point1, Vector3 Point2)
        {

            for (int i = 0; i < CylindersList.Count; i++)
            {
                OneCylinder cylinder = CylindersList[i];
                Vector3 cylinderPoint1 = cylinder.Point1;
                Vector3 cylinerPoint2 = cylinder.Point2;

                if (cylinderPoint1 == Point1 && cylinerPoint2 == Point2)
                {
                    return true;
                }

            }

            return false;
        }

        public void CreateNewCylinder(Vector3 Point1, Vector3 Point2, float radius, Color color)
        {
            Vector3 aPoint1 = IsVecOneLessVecTwo(Point1, Point2) ? Point1 : Point2;
            Vector3 aPoint2 = IsVecOneLessVecTwo(Point1, Point2) ? Point2 : Point1;
            if (!IsCylinderExists(aPoint1, aPoint2))
            {
                var vec4Color = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1);
                var cylinder = new OneCylinder(aPoint1, aPoint2, radius, vec4Color);
                CylindersList.Add(cylinder);
            }
        }
        

        private class OneCylinder
        {
            public float Radius { get; private set; }
            public Vector4 Color { get; private set; }

            public Matrix4 ModelMatrix { get; private set; }
            public Matrix3 NormalMatrix { get; private set; }

            public Vector3 Point1 { get; init; }
            public Vector3 Point2 { get; init; }

            public OneCylinder(Vector3 Point1, Vector3 Point2, float radius, Vector4 color)
            {
                this.Point1 = Point1;
                this.Point2 = Point2;
                this.Radius = radius;
                this.Color = color;
                CalculateModelAndNormalMatrix();
            }

            private void CalculateModelAndNormalMatrix()
            {
                Vector3 direction = Point2 - Point1;
                float length = direction.Length / 2;
                Vector3 position = (Point1 + Point2) / 2f;

                Matrix4 modelScale = Matrix4.CreateScale(length, Radius, Radius);
                Matrix4 modelTranslation = Matrix4.CreateTranslation(position);
                Matrix4 modelRotation = GetRotationMatrixFromUnitXtoVector(direction);

                ModelMatrix = modelScale * modelRotation * modelTranslation;
                NormalMatrix = new(Matrix4.Transpose(Matrix4.Invert(ModelMatrix)));
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
}
