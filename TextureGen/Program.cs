using System.Drawing;

namespace TextureGen
{
    class Program
    {
        static void Main()
        {
            var textureGen = new TextureGen(
                outputPath: "D:\\c#projects\\CFE\\3d_editor\\Textures\\Images\\",
                width: 1000,
                height: 1000,
                fontSize: 200,
                color: Color.Black
                );

            textureGen.CreateTransparentTextImage("H");
        }
    }
}
