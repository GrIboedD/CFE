using _3d_editor.Geometric_figures;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _3d_editor
{
    static class Json_OpenGL_Window
    {
        // Structs
        private struct Sphere
        {
            public float[] position { get; init; }
            public float radius { get; init; }
            public float[] color { get; init; }
            public string text { get; init; }
        }

        private struct Cylinder
        {
            public float[] point1 { get; init; }
            public float[] point2 { get; init; }
            public float radius { get; init; }
            public float[] color { get; init; }
        }

        private struct Data
        {
            public List<Sphere> sphereData { get; init; }
            public List<Cylinder> cylinderData { get; init; }
        }

        // Public metods
        public static string? GetJson(Spheres spheres, Cylinders cylinders)
        {
            List<Sphere> sphereData = GetSpheresData(spheres);
            List<Cylinder> cylinderData = GetCylindersData(cylinders);
            if (sphereData.Count == 0 && cylinderData.Count == 0)
            {
                return null;
            }

            Data data = new()
            {
                sphereData = sphereData,
                cylinderData = cylinderData
            };

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(data, options);
        }

        public static void ReadJson(string filePath, Spheres spheres, Cylinders cylinders)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            string json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            Data data = JsonSerializer.Deserialize<Data>(json, options);
            ProcessData(data, spheres, cylinders);
        }

        // Private metods
        private static void ProcessData(Data data, Spheres spheres, Cylinders cylinders)
        {
            List<Sphere> sphereData = data.sphereData;
            List<Cylinder> cylinderData = data.cylinderData;
            foreach (var sphere in sphereData)
            {
                Vector3 position = ConvertFloatArrayToVector3(sphere.position);
                float radius = sphere.radius;
                Vector4 color = ConvertFloatArrayToVector4(sphere.color);
                string text = sphere.text;
                spheres.CreateNewSphere(position, radius, color, text);
            }

            foreach (var cylinder in cylinderData)
            {
                Vector3 point1 = ConvertFloatArrayToVector3(cylinder.point1);
                Vector3 point2 = ConvertFloatArrayToVector3(cylinder.point2);
                float radius = cylinder.radius;
                Vector4 color = ConvertFloatArrayToVector4(cylinder.color);
                cylinders.CreateNewCylinder(point1, point2, radius, color);
            }
        }
        private static Vector3 ConvertFloatArrayToVector3(float[] array)
        {
            return new Vector3(array[0], array[1], array[2]);
        }

        private static Vector4 ConvertFloatArrayToVector4(float[] array)
        {
            return new Vector4(array[0], array[1], array[2], array[3]);
        }
        private static float[] ConvertVectorToFloatArray(Vector3 vector)
        {
            var array = new float[3];
            array[0] = vector.X;
            array[1] = vector.Y;
            array[2] = vector.Z;
            return array;
        }

        private static float[] ConvertVectorToFloatArray(Vector4 vector)
        {
            var array = new float[4];
            array[0] = vector.X;
            array[1] = vector.Y;
            array[2] = vector.Z;
            array[3] = vector.W;
            return array;
        }

        private static List<Sphere> GetSpheresData(Spheres Spheres)
        {
            List<Sphere> list = [];
            int lastIndex = Spheres.GetLastSphereIndex();
            for (int i = 0; i <= lastIndex; i++)
            {
                Sphere sphere = new()
                {
                    position = ConvertVectorToFloatArray(Spheres.GetSpheresCenterCord(i)),
                    radius = Spheres.GetSpheresRadius(i),
                    color = ConvertVectorToFloatArray(Spheres.GetSpheresColor(i)),
                    text = Spheres.GetSpheresText(i)
                };

                list.Add(sphere);
            }
            
            return list;
        }

        private static List<Cylinder> GetCylindersData(Cylinders Cylinders)
        {
            List<Cylinder> list = [];
            int lastIndex = Cylinders.GetLastCylinderIndex();
            for (int i = 0; i <= lastIndex; i++)
            {
                Cylinder cylinder = new()
                {
                    point1 = ConvertVectorToFloatArray(Cylinders.GetPoint1(i)),
                    point2 = ConvertVectorToFloatArray(Cylinders.GetPoint2(i)),
                    radius = Cylinders.GetCylinderRadius(i),
                    color = ConvertVectorToFloatArray(Cylinders.GetCylinderColor(i))
                };

                list.Add(cylinder);
            }

            return list;
        }
    }
}
