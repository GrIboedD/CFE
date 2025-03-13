﻿using System;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace _3d_editor
{
    class Texture
    {
        int Handle;

        public Texture(string path_to_texture)
        {
            Handle = GL.GenTexture();
            Use();
            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult image = ImageResult.FromStream(File.OpenRead(path_to_texture), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height,
                0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, this.Handle);
        }
    }
}
