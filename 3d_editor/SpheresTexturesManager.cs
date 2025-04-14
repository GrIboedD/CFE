using System;
using System.Runtime.InteropServices.Marshalling;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace _3d_editor
{

    class Texture
    {
        private int Handle { get; init; }

        public Texture(string pathToTexture)
        {
            Handle = GL.GenTexture();
            Use();



            ImageResult image = ImageResult.FromStream(File.OpenRead(pathToTexture),
                ColorComponents.RedGreenBlueAlpha);

            for (int i = 0; i < 6; i++)
            {
                TextureTarget target = TextureTarget.TextureCubeMapPositiveX + i;
                GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, image.Width, image.Height,
                    0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

;
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);


        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
    }


    class SpheresTexturesManager

    {
      
        private readonly Dictionary<string, Texture> ListOfTextures = [];
        private readonly string directoryPath = Path.Combine("cache", "textures");

        public SpheresTexturesManager()
        {
            if (!Directory.Exists(directoryPath))
                return;

            string[] textureFiles = Directory.GetFiles(directoryPath, "*.png");
            
            foreach (string filePath in textureFiles)
            {
                string fileName = Path.GetFileName(filePath);
                Texture texture = new(filePath);
                ListOfTextures[fileName] = texture;
            }
        }

        private Texture GenerateTexture(
            string textureName,
            string text,
            int size,
            int fontSize,
            Color color)
        {
            Directory.CreateDirectory(directoryPath);

            string texturePath = Path.Combine(directoryPath, textureName);

            using var textureGen = new TextureGen(
                outputPath: directoryPath,
                width: size,
                height: size,
                fontSize: fontSize,
                color: (Color)color);
            textureGen.CreateTransparentTextImage(text, textureName);

            Texture newTexture = new(texturePath);
            ListOfTextures[textureName] = newTexture;
            return newTexture;
        }

        public Texture GetTexture(
            string text,
            int size = 500,
            int fontSize = 144,
            Color? inColor = null)
        {
            Color color = inColor ??Color.Black;

            string textureName = text + $"{size}{fontSize}" + color.ToString()[6..] + ".png";
            Texture? texture = ListOfTextures.GetValueOrDefault(textureName);
            if (texture is not null) return texture;

            return GenerateTexture(textureName, text, size, fontSize, color);

        }
        
       
    }
}
