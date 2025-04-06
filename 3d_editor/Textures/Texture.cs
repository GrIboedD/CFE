using System;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace _3d_editor.Textures
{
    class Texture
    {
        private int Handle { get; init; }

        public Texture(string pathToTexture)
        {
            Handle = GL.GenTexture();
            Use();
            StbImage.stbi_set_flip_vertically_on_load(1);



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
}
