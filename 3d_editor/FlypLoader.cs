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
using OpenTK.Audio.OpenAL;

namespace _3d_editor
{
    partial class FlypLoader(OpenGL_Window window)
    {

        [GeneratedRegex(@"(?<=,\s)(.+)(?=$)")]
        private static partial Regex GetVector3FromString();

        readonly XmlDocument doc = new();
        readonly OpenGL_Window window = window;

        private readonly List<(Vector3, Matrix3)> layers = [];

        private const float scale = 0.01f;
        
        public void OpenFlypFile(string path)
        {
            doc.Load(path);
            XmlElement? root = doc.DocumentElement;
            if (root is null)
            {
                return;
            }

            XmlNode? bodyNode = root.SelectSingleNode("body");
            if (bodyNode == null)
                return;

            foreach (XmlElement element in bodyNode.ChildNodes)
            {
                if (element.Name == "sphere")
                {
                    ProceedSphere(element);
                }

                if (element.Name == "cylindrical")
                {
                    ProceedCylinder(element);
                }

                if (element.Name == "element")
                {
                    string pos = element.GetAttribute("pos");
                    Vector3 position = ConverteStringToVector3(pos) * scale;

                    string xortString = element.GetAttribute("xort");
                    Vector3 xort = Vector3.Normalize(ConverteStringToVector3(xortString));

                    string yortString = element.GetAttribute("yort");
                    Vector3 yort = Vector3.Normalize(ConverteStringToVector3(yortString));

                    string zortString = element.GetAttribute("zort");
                    Vector3 zort = Vector3.Normalize(ConverteStringToVector3(zortString));

                    Matrix3 rotationMatrix = new (xort, yort, zort);

                    string prevString = element.GetAttribute("prev");
                    int prev = int.Parse(prevString);

                    if(layers.Count > 0 && prev == 1)
                    {
                        Vector3 prevPos = layers[^1].Item1;
                        Matrix3 prevRotation = layers[^1].Item2;

                        position = position * prevRotation + prevPos;
                        rotationMatrix *= prevRotation;
                    }

                    XmlNode? SubBodyNode = element.SelectSingleNode("body");
                    if (SubBodyNode == null)
                        continue;
;
                    foreach (XmlElement subElement in SubBodyNode)
                    {
                        if (subElement.Name == "sphere")
                        {
                            ProceedSphere(subElement, position, rotationMatrix);
                        }

                        if (subElement.Name == "cylindrical")
                        {
                            ProceedCylinder(subElement, position, rotationMatrix);
                        }
                    }


                    layers.Add((position, rotationMatrix));

                }
            }

        }

        private static Vector3 ConverteStringToVector3(string value)
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


        private void ProceedSphere(XmlElement sphere)
        {

            ProceedSphere(sphere, Vector3.Zero, Matrix3.Identity);
        }

        private void ProceedSphere(XmlElement sphere, Vector3 translate, Matrix3 rotate)
        {
            string pos = sphere.GetAttribute("pos");

            Vector3 position = ConverteStringToVector3(pos) * scale;

            string r = sphere.GetAttribute("r");
            float radius = float.Parse(r) * scale;

            string col = sphere.GetAttribute("color");
            col = GreyToGray(col);
            Color color = Color.FromName(col);

            string text = sphere.GetAttribute("image");

            window.AddSphere(position * rotate + translate, radius, color, text);
        }

        private static string GreyToGray(string color)
        {
            if (color == "grey")
                return "gray";

            return color;
        }

        private void ProceedCylinder(XmlElement cylinder)
        {
            ProceedCylinder(cylinder, Vector3.Zero, Matrix3.Identity);
        }

        private void ProceedCylinder(XmlElement cylinder, Vector3 translate, Matrix3 rotate)
        {
            string pos = cylinder.GetAttribute("pos");
            Vector3 Point1 = ConverteStringToVector3(pos) * scale;

            string zort = cylinder.GetAttribute("zort");
            Vector3 zDir = Vector3.Normalize(ConverteStringToVector3(zort));

            string h = cylinder.GetAttribute("vh");
            float height = float.Parse(GetVector3FromString().Match(h).Value, CultureInfo.InvariantCulture) * scale;

            Vector3 Point2 = Point1 + zDir * height;

            string r = cylinder.GetAttribute("r");
            float radius = float.Parse(r) * scale;

            string col = cylinder.GetAttribute("color");
            col = GreyToGray(col);
            Color color = Color.FromName(col);

            window.AddCylinder(Point1 * rotate + translate, Point2 * rotate + translate, radius, color);
        }



    }
}
