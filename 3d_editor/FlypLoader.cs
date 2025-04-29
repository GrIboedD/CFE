using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using OpenTK.Mathematics;

namespace _3d_editor
{
    class FlypLoader(OpenGL_Window window)
    {
        XmlDocument doc = new XmlDocument();
        OpenGL_Window window = window;

        private const float scale = 0.01f;
        
        public void openFlypFile(string path)
        {
            doc.Load(path);
            XmlElement? root = doc.DocumentElement;
            if (root is null)
            {
                return;
            }

            proceedSpheres(root);


        }

        private void proceedSpheres(XmlElement root)
        {
            foreach(XmlElement sphere in root.GetElementsByTagName("sphere"))
            {
                string pos = sphere.GetAttribute("pos");
                string pattern = @"[^\s]+(?=,|$)";
                Vector3 position = Vector3.Zero;
                MatchCollection mathes = Regex.Matches(pos, pattern);
                for (int i = 0; i <= 2; i++)
                {
                    position[i] = float.Parse(mathes[i].Value, CultureInfo.InvariantCulture) * scale;
                }

                string r = sphere.GetAttribute("r");
                float radius = float.Parse(r) * scale;

                string col = sphere.GetAttribute("color");
                Color color = Color.FromName(col);

                string text = sphere.GetAttribute("image");

                window.AddSphere(position, radius, color, text);


            }
        }
    }
}
