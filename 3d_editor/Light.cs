using OpenTK.Mathematics;

namespace _3d_editor
{
    static class Light
    {
        public static Vector3 Direction { get; private set; } = new Vector3(0, 1, 0);
        public static Vector3 Ambient { get; private set; } = new(1.0f);
        public static Vector3 Diffuse { get; private set; } = new(1.0f);
        public static Vector3 Specular { get; private set; } = new(1.0f);
    }
}
