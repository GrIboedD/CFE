using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using OpenTK.Mathematics;
using _3d_editor.Geometric_figures;

namespace _3d_editor
{
    class FlypLoader(OpenGL_Window window)
    {
        readonly XmlDocument doc = new();
        readonly OpenGL_Window window = window;

        private const float scale = 0.01f;
        
        public void OpenFlypFile(string path)
        {
            doc.Load(path);
            XmlElement? root = doc.DocumentElement;
            if (root is null)
            {
                return;
            }

            ProceedSpheres(root);
            ProceedCylinders(root);

        }

        private Vector3 ConverteStringToVector3(string value)
        {
            string pattern = @"\s*([^\s]+)\s*,\s*([^\s]+)\s*,\s*([^\s]+)\s*$";
            Vector3 vector = Vector3.Zero;
            Match match = Regex.Match(value, pattern);
            for (int i = 0; i <= 2; i++)
            {
                vector[i] = float.Parse(match.Groups[i+1].Value, CultureInfo.InvariantCulture);
            }
            return vector;
        }

        private void ProceedSpheres(XmlElement root)
        {
            foreach(XmlElement sphere in root.GetElementsByTagName("sphere"))
            {
                string pos = sphere.GetAttribute("pos");

                Vector3 position = ConverteStringToVector3(pos) * scale;

                string r = sphere.GetAttribute("r");
                float radius = float.Parse(r) * scale;

                string col = sphere.GetAttribute("color");
                Color color = Color.FromName(col);

                string text = sphere.GetAttribute("image");

                window.AddSphere(position, radius, color, text);

            }
        }

        private void ProceedCylinders(XmlElement root)
        {
            foreach (XmlElement cylinder in root.GetElementsByTagName("cylindrical"))
            {
                string pos = cylinder.GetAttribute("pos");
                Vector3 Point1 = ConverteStringToVector3(pos) * scale;

                string zort = cylinder.GetAttribute("zort");
                Vector3 zDir = Vector3.Normalize(ConverteStringToVector3(zort));

                string h = cylinder.GetAttribute("vh");
                float height = float.Parse(Regex.Match(h, @"(?<=,\s)(.+)(?=$)").Value) * scale;

                Vector3 Point2 = Point1 + zDir * height;

                string r = cylinder.GetAttribute("r");
                float radius = float.Parse(r) * scale;

                string col = cylinder.GetAttribute("color");
                Color color = Color.FromName(col);

                window.AddCylinder(Point1, Point2, radius, color);

            }
        }
    }
}
