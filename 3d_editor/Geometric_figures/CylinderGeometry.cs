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

        private readonly List<TriangleIndices> indices = [];


        public CylinderGeometry(uint sectorsCount)
        {
            float step = float.Pi * 2.0f / sectorsCount;
            float angle = -float.Pi;

            Vector3 centerLeft = new(1.0f, 0.0f, 0.0f);
            Vector3 firstPointLeft = new(1.0f, 0.0f, -1.0f);
            vertices.Add(centerLeft);
            vertices.Add(firstPointLeft);

            Vector3 CenterRight = new(-1.0f, 0.0f, 0.0f);
            Vector3 firstPointRight = new(-1.0f, 0.0f, -1.0f);
            vertices.Add(CenterRight);
            vertices.Add(firstPointRight);


            uint centerLeftIndex = 0;
            uint previousPointLeftIndex = 1;

            uint centerRightIndex = 2;
            uint previousPointRightIndex = 3;

            for (int i = 0; i < sectorsCount; i++)
            {
                float y = (float)MathHelper.Sin(angle + step);
                float z = (float)MathHelper.Cos(angle + step);

                if (i == sectorsCount - 1)
                {
                    indices.Add(new(centerLeftIndex, 1, previousPointLeftIndex));
                    indices.Add(new(centerRightIndex, previousPointRightIndex, 3));
                    indices.Add(new(previousPointLeftIndex, 1, previousPointRightIndex));
                    indices.Add(new(previousPointRightIndex, 1, 3));

                    break;
                }

                Vector3 pointLeft = new(1.0f, y, z);
                vertices.Add(pointLeft);
                uint pointLeftIndex = (uint)vertices.Count - 1;
                indices.Add(new(centerLeftIndex, pointLeftIndex, previousPointLeftIndex));

                Vector3 pointRight = new(-1.0f, y, z);
                vertices.Add(pointRight);
                uint pointRightIndex = (uint)vertices.Count - 1;
                indices.Add(new(centerRightIndex, previousPointRightIndex, pointRightIndex));

                indices.Add(new(previousPointLeftIndex, pointLeftIndex, previousPointRightIndex));
                indices.Add(new(previousPointRightIndex, pointLeftIndex, pointRightIndex));


                previousPointLeftIndex = pointLeftIndex;
                previousPointRightIndex = pointRightIndex;
                angle += step;
            }
        }

        public float[] GetVertices()
        {
            List<float> outVertices = [];
            foreach (var vertex in vertices)
            {
                outVertices.Add(vertex.X);
                outVertices.Add(vertex.Y);
                outVertices.Add(vertex.Z);
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
