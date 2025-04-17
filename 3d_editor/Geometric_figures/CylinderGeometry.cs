using OpenTK.Mathematics;
using System.Security.Cryptography.X509Certificates;

namespace _3d_editor.Geometric_figures
{
    class CylinderGeometry
    {
        private struct TriangleIndices(uint v1, uint v2, uint v3)
        {
            public uint v1 = v1;
            public uint v2 = v2;
            public uint v3 = v3;
        }


        private readonly List<Vector3> vertices = [];

        private readonly List<Vector3> normals = [];

        private readonly List<TriangleIndices> indices = [];


        private static Vector3 CalcNormal(Vector3 circlePoint)
        {
            Vector3 planeNormal = Vector3.Normalize(new Vector3(0, circlePoint.Y, circlePoint.Z));
            return planeNormal;

        }

        public CylinderGeometry(uint sectorsCount)
        {
            float step = float.Pi * 2.0f / sectorsCount;
            float angle = -float.Pi;

            //left
            Vector3 centerLeft = new(1.0f, 0.0f, 0.0f);
            Vector3 firstPointLeft = new(1.0f, 0.0f, -1.0f);
            vertices.Add(centerLeft);
            normals.Add(centerLeft);

            vertices.Add(firstPointLeft);
            normals.Add(centerLeft);

            //right
            Vector3 CenterRight = new(-1.0f, 0.0f, 0.0f);
            Vector3 firstPointRight = new(-1.0f, 0.0f, -1.0f);
            vertices.Add(CenterRight);
            normals.Add(CenterRight);

            vertices.Add(firstPointRight);
            normals.Add(CenterRight);

            //body
            vertices.Add(firstPointLeft);
            vertices.Add(firstPointRight);
            normals.Add(CalcNormal(firstPointLeft));
            normals.Add(CalcNormal(firstPointRight));

            //left
            uint centerLeftIndex = 0;
            uint previousPointLeftIndex = 1;

            //right
            uint centerRightIndex = 2;
            uint previousPointRightIndex = 3;

            //body
            uint bodyPrevLeftIndex = 4;
            uint bodyPrevRightIndex = 5;



            for (int i = 0; i < sectorsCount; i++)
            {
                float y = (float)MathHelper.Sin(angle + step);
                float z = (float)MathHelper.Cos(angle + step);

                if (i == sectorsCount - 1)
                {
                    indices.Add(new(centerLeftIndex, 1, previousPointLeftIndex));
                    indices.Add(new(centerRightIndex, previousPointRightIndex, 3));

                    indices.Add(new(bodyPrevLeftIndex, 4, bodyPrevRightIndex));
                    indices.Add(new(bodyPrevRightIndex, 4, 5));

                    break;
                }


                //left
                Vector3 pointLeft = new(1.0f, y, z);
                vertices.Add(pointLeft);
                normals.Add(centerLeft);
                uint pointLeftIndex = (uint)vertices.Count - 1;
                indices.Add(new(centerLeftIndex, pointLeftIndex, previousPointLeftIndex));


                //Right
                Vector3 pointRight = new(-1.0f, y, z);
                vertices.Add(pointRight);
                normals.Add(CenterRight);
                uint pointRightIndex = (uint)vertices.Count - 1;
                indices.Add(new(centerRightIndex, previousPointRightIndex, pointRightIndex));

                //body
                Vector3 bodyLeft = pointLeft;
                vertices.Add(bodyLeft);
                normals.Add(CalcNormal(bodyLeft));

                Vector3 bodyRight = pointRight;
                vertices.Add(bodyRight);
                normals.Add(CalcNormal(bodyRight));

                uint bodyLeftIndex = (uint)vertices.Count - 2;
                uint bodyRightIndex = (uint)vertices.Count - 1;
                indices.Add(new(bodyPrevLeftIndex, bodyLeftIndex, bodyPrevRightIndex));
                indices.Add(new(bodyPrevRightIndex, bodyLeftIndex, bodyRightIndex));


                previousPointLeftIndex = pointLeftIndex;
                previousPointRightIndex = pointRightIndex;

                bodyPrevLeftIndex = bodyLeftIndex;
                bodyPrevRightIndex = bodyRightIndex;

                angle += step;
            }
        }

        public float[] GetVertices()
        {
            List<float> outVertices = [];
            for (int i = 0; i < vertices.Count; i++)
            {
                outVertices.Add(vertices[i].X);
                outVertices.Add(vertices[i].Y);
                outVertices.Add(vertices[i].Z);

                outVertices.Add(normals[i].X);
                outVertices.Add(normals[i].Y);
                outVertices.Add(normals[i].Z);
            }

            return [.. outVertices];
        }

        public uint[] GetIndices()
        {
            List<uint> outIndices = [];
            foreach (var tri in indices)
            {
                outIndices.Add(tri.v1);
                outIndices.Add(tri.v2);
                outIndices.Add(tri.v3);
            }
            return [.. outIndices];
        }

    }
}
