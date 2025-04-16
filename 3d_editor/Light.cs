using OpenTK.Mathematics;

namespace _3d_editor
{
    static class Light
    {
        public static Vector3 Position { get; private set; } = new(0, 100000, 0);
        public static Vector3 Ambient { get; private set; } = new(1.0f);
        public static Vector3 Diffuse { get; private set; } = new(1.0f);
        public static Vector3 Specular { get; private set; } = new(1.0f);
    }
}
