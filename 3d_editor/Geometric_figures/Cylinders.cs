using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Diagnostics;

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

        public override void Draw(int indexNoDraw = -1)
        {
            if (CylindersList.Count == 0)
                return;

            BindBuffers(Vertices, Indices);

            Shader.Use();
            GL.Enable(EnableCap.CullFace);

            for (int i = 0; i < CylindersList.Count; i++)
            {
                if (indexNoDraw == i)
                {
                    continue;
                }
                var cylinder = CylindersList[i];
                Shader.SetMatrix("model", cylinder.ModelMatrix);
                Shader.SetMatrix("normalMatrix", cylinder.NormalMatrix);
                Shader.SetVec("material.color", cylinder.Color);
                GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        public void DrawPickedCylinderOutlining(int index)
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();

            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

            var cylinder = CylindersList[index];

            var tempCylinder = new OneCylinder(
                cylinder.Point1,
                cylinder.Point2,
                cylinder.Radius + 0.04f,
                new Vector4(0, 1, 0, 1)
                );

            Shader.SetMatrix("model", tempCylinder.ModelMatrix);
            Shader.SetMatrix("normalMatrix", tempCylinder.NormalMatrix);
            Shader.SetVec("material.color", tempCylinder.Color);

            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.StencilTest);
        }
        public void DrawPickedCylinder(int index)
        {
            BindBuffers(Vertices, Indices);
            Shader.Use();
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Replace, StencilOp.Replace);
            var cylinder = CylindersList[index];
            Shader.SetMatrix("model", cylinder.ModelMatrix);
            Shader.SetMatrix("normalMatrix", cylinder.NormalMatrix);
            Shader.SetVec("material.color", cylinder.Color);
            GL.StencilFunc(StencilFunction.Always, 1, 0x00);
            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.StencilTest);

        }



        private static Vector3? IntersectionLinePlane(Vector3 lineP, Vector3 lineDir, Vector3 planeP, Vector3 planeNorm)
        {
            lineDir = Vector3.Normalize(lineDir);
            planeNorm = Vector3.Normalize(planeNorm);

            float denom = Vector3.Dot(lineDir, planeNorm);
            if (Math.Abs(denom) > 0.001)
            {
                float t = Vector3.Dot(planeP - lineP, planeNorm) / denom;

                Vector3 point = lineP + lineDir * t;
                return point;
            }

            return null;
        }

        private static bool IsPointOnCylinderSurf(Vector3 cylinderPoint, Vector3 cylinedrAxe, Vector3 surfPoint)
        {
            Vector3 subVector = surfPoint - cylinderPoint;
            float dist = Vector3.Dot(subVector, Vector3.Normalize(cylinedrAxe));
            return dist >= 0 && dist <= cylinedrAxe.Length;
        }

        public (int, float) RayCasting(Vector3 rayOrigin, Vector3 rayDirection)
        {
            List<(int index, float distance)> nearestCylinders = [];

            for (int i = 0; i < CylindersList.Count; i++)
            {
                OneCylinder cylinder = CylindersList[i];

                Vector3 Point1 = cylinder.Point1;
                Vector3 Point2 = cylinder.Point2;
                float radius = cylinder.Radius;

                //check if rayOrigin inside cylinder
                Vector3 cylinderAxes = Point2 - Point1;

                Vector3 cylinderAxesNormalize = Vector3.Normalize(Point2 - Point1);

                Vector3 fromPoint1ToRayOrigin = rayOrigin - Point1;

                Vector3 projectionVector = Vector3.Dot(fromPoint1ToRayOrigin, cylinderAxesNormalize) * cylinderAxesNormalize;

                if (Vector3.Dot(cylinderAxesNormalize, Vector3.Normalize(projectionVector)) > 0)
                {
                    if (projectionVector.Length <= cylinderAxes.Length)
                    {
                        Vector3 distanceFromAxe = fromPoint1ToRayOrigin - projectionVector;
                        if (distanceFromAxe.Length <= radius)
                        {
                            Console.WriteLine("Inside");
                            return (i, 0);
                        }
                           
                    }
                }

                List<float> tList = [];


                Vector3? nullableBasePoint1 = IntersectionLinePlane(rayOrigin, rayDirection, Point1, cylinderAxesNormalize);
                Vector3? nullableBasePoint2 = IntersectionLinePlane(rayOrigin, rayDirection, Point2, cylinderAxesNormalize);

                if (nullableBasePoint1 is not null && nullableBasePoint2 is not null)
                {
                    Vector3 basePoint1 = nullableBasePoint1.Value;
                    Vector3 basePoint2 = nullableBasePoint2.Value;

                    if ((basePoint1 - Point1).Length <= radius)
                    {
                        tList.Add((basePoint1 - rayOrigin).Length);
                    }

                    if ((basePoint2 - Point2).Length <= radius)
                    {
                        tList.Add((basePoint2 - rayOrigin).Length);
                    }
                }



                Vector3 v = rayOrigin - Point1;
                Vector3 d = rayDirection;

                Vector3 crossD = Vector3.Cross(d, cylinderAxesNormalize);
                Vector3 crossV = Vector3.Cross(v, cylinderAxesNormalize);

                float a = Vector3.Dot(crossD, crossD);
                float b = 2 * Vector3.Dot(crossV, crossD);
                float c = Vector3.Dot(crossV, crossV) - radius * radius;

                float discriminat = b * b - 4 * a * c;
                if (discriminat >= 0)
                {
                    if (Math.Abs(discriminat) <= 0.001)
                    {
                        float t = -b / (2 * a);
                        Vector3 pointOnSurf = rayOrigin + rayDirection * t;
                        if (IsPointOnCylinderSurf(Point1, cylinderAxes, pointOnSurf))
                        {
                            tList.Add(t);
                        }
                    }
                    else
                    {
                        float sqrtD = (float)Math.Sqrt(discriminat);
                        float t1 = (-b - sqrtD) / (2 * a);
                        float t2 = (-b + sqrtD) / (2 * a);

                        Vector3 pointOnSurf1 = rayOrigin + rayDirection * t1;
                        Vector3 pointOnSurf2 = rayOrigin + rayDirection * t2;
                        
                        if (IsPointOnCylinderSurf(Point1, cylinderAxes, pointOnSurf1))
                        {
                            tList.Add(t1);
                        }

                        if (IsPointOnCylinderSurf(Point1, cylinderAxes, pointOnSurf2))
                        {
                            tList.Add(t2);
                        }
                    }
                }

                List<float> positiveT = [.. tList.Where(x => x >= 0)];
                if (positiveT.Count > 0)
                {
                    nearestCylinders.Add((i, positiveT.Min()));
                }

            }

            if (nearestCylinders.Count == 0) return (-1, -1);

            return nearestCylinders.MinBy(x => x.distance);
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

        public void DelAllCylinders()
        {
            CylindersList.Clear();
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
        
        private List<(int index, bool isPoint1)> GetCylindersPointsInSphere(Vector3 spherePos, float radius)
        {
            var list = new List<(int index, bool isPoint1)> ();
            for (int i = 0; i < CylindersList.Count; i++)
            {
                var aOneCylinder = CylindersList[i];
                Vector3 point1 = aOneCylinder.Point1;
                Vector3 point2 = aOneCylinder.Point2;

                if ((point1 - spherePos).Length < radius)
                {
                    list.Add((i, true));
                }

                if ((point2 - spherePos).Length < radius)
                {
                    list.Add((i, false));
                }
            }
            return list;
        }

        public void MoveCylindersWithSphere(Vector3 spherePos, float radius, Vector3 moveVector)
        {
            List<(int index, bool isPoint1)> list = GetCylindersPointsInSphere(spherePos, radius);
            foreach (var item in list)
            {
                int index = item.index;
                bool isPoint1 = item.isPoint1;
                Vector3 point = isPoint1 ? CylindersList[index].Point1 : CylindersList[index].Point2;
                Vector3 newPoint = point + moveVector;
                
                if (isPoint1)
                {
                    CylindersList[index].SetPoint1(newPoint);
                }
                else
                {
                    CylindersList[index].SetPoint2(newPoint);
                }
            }
        }

        public Vector4 GetCylinderColor(int index)
        {
            return CylindersList[index].Color;
        }

        public float GetCylinderRadius(int index)
        {
            return CylindersList[index].Radius;
        }

        public void SetCylinderColor(int index, Vector4 color)
        {
            CylindersList[index].Color = color;
        }

        public void SetCylinderRadius(int index, float radius)
        {
            CylindersList[index].SetRadius(radius);
        }

        private class OneCylinder
        {
            public float Radius { get; private set; }
            public Vector4 Color { get; set; }

            public Matrix4 ModelMatrix { get; private set; }
            public Matrix3 NormalMatrix { get; private set; }

            public Vector3 Point1 { get; private set; }
            public Vector3 Point2 { get; private set; }

            public OneCylinder(Vector3 Point1, Vector3 Point2, float radius, Vector4 color)
            {
                this.Point1 = Point1;
                this.Point2 = Point2;
                this.Radius = radius;
                this.Color = color;
                CalculateModelAndNormalMatrix();
            }

            public void SetPoint1(Vector3 newPoint)
            {
                this.Point1 = newPoint;
                CalculateModelAndNormalMatrix();
            }

            public void SetPoint2(Vector3 newPoint)
            {
                this.Point2 = newPoint;
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

            public void SetRadius(float radius)
            {
                this.Radius = radius;
                CalculateModelAndNormalMatrix();
            }
        }
    }
}
