

using OpenTK.Mathematics;

namespace _3d_editor.View
{
    class Light
    {
        public Vector3 LightsColor { get; private set; } = Vector3.One;
        public Vector3 LightsPosition { get; private set; } = new(0, 0, 0);

        public float AmbientStrength { get; private set; } = 0.0f;

    }
}
