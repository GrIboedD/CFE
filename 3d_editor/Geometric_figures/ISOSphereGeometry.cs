﻿
using OpenTK.Mathematics;

namespace _3d_editor.Geometric_figures
{
    class ISOSphereGeometry
    {

        private struct TriangleIndices(int v1, int v2, int v3)
        {
            public int v1 = v1;
            public int v2 = v2;
            public int v3 = v3;
        }

        private readonly List<Vector3> vertices = [];

        private readonly List<TriangleIndices> indices = [];

        private readonly Dictionary<Int64, int> middlePointIndexCache = [];

        private static readonly float t = (float)((1.0 + Math.Sqrt(5.0)) / 2.0);

        private int AddVertex(float x, float y, float z)
        {
            float length = (float)Math.Sqrt(x * x + y * y + z * z);
            vertices.Add(new Vector3(x, y, z) / length);
            return vertices.Count - 1;
        }

        private int GetMiddlePoint(int v1, int v2)
        {
            bool firstSmaller = v1 < v2;
            Int64 smallerIndex = firstSmaller ? v1 : v2;
            Int64 greaterIndex = firstSmaller ? v2 : v1;
            Int64 key = (smallerIndex << 32) + greaterIndex;

            if (middlePointIndexCache.TryGetValue(key, out int value))
            {
                return value;
            }

            Vector3 vertex1 = vertices[v1];
            Vector3 vertex2 = vertices[v2];
            float x = (vertex1.X + vertex2.X) / 2.0f;
            float y = (vertex1.Y + vertex2.Y) / 2.0f;
            float z = (vertex1.Z + vertex2.Z) / 2.0f;


            int index = AddVertex(x, y, z);
            middlePointIndexCache.Add(key, index);
            return index;
        }

        public ISOSphereGeometry(int recursionLevel)
        {

            AddVertex(-1.0f, t, 0.0f);
            AddVertex(1.0f, t, 0.0f);
            AddVertex(-1.0f, -t, 0.0f);
            AddVertex(1.0f, -t, 0.0f);

            AddVertex(0.0f, -1.0f, t);
            AddVertex(0.0f, 1.0f, t);
            AddVertex(0.0f, -1.0f, -t);
            AddVertex(0.0f, 1.0f, -t);

            AddVertex(t, 0.0f, -1.0f);
            AddVertex(t, 0.0f, 1.0f);
            AddVertex(-t, 0.0f, -1.0f);
            AddVertex(-t, 0.0f, 1.0f);

            List<TriangleIndices> faces = [];
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));

            for (int i = 0; i < recursionLevel; i++)
            {
                List<TriangleIndices> newFaces = [];
                foreach (var tri in faces)
                {
                    int a = GetMiddlePoint(tri.v1, tri.v2);
                    int b = GetMiddlePoint(tri.v2, tri.v3);
                    int c = GetMiddlePoint(tri.v3, tri.v1);

                    newFaces.Add(new TriangleIndices(tri.v1, a, c));
                    newFaces.Add(new TriangleIndices(tri.v2, b, a));
                    newFaces.Add(new TriangleIndices(tri.v3, c, b));
                    newFaces.Add(new TriangleIndices(a, b, c));
                }
                faces = newFaces;
            }

            indices = faces;

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
                outIndices.Add((uint)tri.v1);
                outIndices.Add((uint)tri.v2);
                outIndices.Add((uint)tri.v3);
            }
            return [.. outIndices];
        }

    }
}
