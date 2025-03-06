using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;

namespace _3d_editor
{
    class GL_Window
    {
        //Время в секундах
        private float frameTime = 0.0f;

        //Фпс
        private int fps = 0;

        //Буфер вертексов
        int VertexBufferObject;

        int VertexArrayObject;

        //Вертексы
        float[] vertices =
        {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f,  0.5f, 0.0f
        };

        //Шейдеры
        Shader shader;

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

            //Создание буфера
            VertexBufferObject = GL.GenBuffer();

            //Привязка буфера
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            //Кладем данные в буфер
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(0);


            //Отвязка
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

        }

        //Выполняется при изменении размеров окна
        public void Resize(int Width, int Height)
        {
            //GL.Viewport(0, 0, Width, Height);
        }

        //Выполняется при обновлении фрейма(для расчетов)
        public void UpdateFrame()
        {
        }

        //Выполняется при обновлении фрейма (для отрисовки)
        public void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        //Видимо при закрытии окна
        public void Unload()
        {
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            //Отвязка от шейдеров
            GL.UseProgram(0);
            shader.Dispose();
        }

    }
}
