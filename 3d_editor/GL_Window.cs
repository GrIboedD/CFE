using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;
using OpenTK.GLControl;

namespace _3d_editor
{
    class GL_Window
    {

        //Буфер вертексов
        int VBO;

        int VAO;

        int EBO;

        //Вертексы
        float[] vertices =
        {

         0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f ,1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f, // top left
        };

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        //Шейдеры
        Shader shader;
        Texture texture1, texture2;
        public GL_Window()
        {
            //Console.WriteLine(GL.GetString(StringName.Version));
            //Console.WriteLine(GL.GetString(StringName.Vendor));
            //Console.WriteLine(GL.GetString(StringName.Renderer));
            //Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
        }
        public void Load()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            //Использование шейдеров
            shader = new Shader("../../../shader.vert", "../../../shader.frag");
            shader.Use();
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StreamDraw);

            //Создание буфера
            VBO = GL.GenBuffer();
            //Привязка буфера
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //Кладем данные в буфер
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

            int vertexLocation = shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            texture1 = new Texture("../../../container.jpg");
            texture2 = new Texture("../../../awesomeface.png");
            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);
            int texCoordLocation = shader.GetAttribLocation("aTexCoord"); 
            GL.VertexAttribPointer(shader.GetAttribLocation("aTexCoord"), 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            //Отвязка
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            //GL.BindVertexArray(0);

        }

        //Выполняется при изменении размеров окна
        public void Resize(GLControl glControl)
        {
            glControl.MakeCurrent();
            GL.Viewport(0, 0, 800, 800);
        }

        //Выполняется при обновлении фрейма(для расчетов)
        public void UpdateFrame(float totalTime)
        {
            //shader.Use();
            //float greenValue = (float)Math.Sin(totalTime) / 2.0f + 0.5f;
            //int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            //GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
            float anlge = (float)Math.Sin(totalTime) * 2 * float.Pi;
            float scale_x = (float)Math.Sin(totalTime) + 1.0f;
            Matrix4 rotation = Matrix4.CreateRotationZ(anlge);
            Matrix4 scale = Matrix4.CreateScale(scale_x, scale_x, scale_x);
            Matrix4 trans = rotation * scale;
            shader.Use();
            int location = GL.GetUniformLocation(shader.Handle, "transform");
            GL.UniformMatrix4(location, true, ref trans);
        }

        //Выполняется при обновлении фрейма (для отрисовки)
        public void RenderFrame(GLControl glControl)
        {
            glControl.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            texture1.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            glControl.SwapBuffers();
        }

        //Видимо при закрытии окна
        public void Unload()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);

            //Отвязка от шейдеров
            GL.UseProgram(0);
            shader.Dispose();
        }

    }
}
