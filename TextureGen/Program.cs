using System.Drawing;

namespace TextureGen
{
    class Program
    {
        static void Main()
        {
            var textureGen = new TextureGen(
                outputPath: "D:\\c#projects\\CFE\\3d_editor\\Textures\\Images\\",
                width: 200,
                height: 200,
                fontSize: 48,
                color: Color.Black
                );

            textureGen.CreateTransparentTextImage("H");
        }
    }
}
