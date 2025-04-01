using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows.Forms.VisualStyles;
using OpenTK.Mathematics;

namespace _3d_editor.View
{
    public class Camera
    {
        private Vector3 cameraPosition;
        private Vector3 cameraTarget;

        private enum Direction
        {
            UpDown,
            LeftRight,
            ForwardBackward
        }

        public Camera(float[] cameraPosition, float[] cameraTarget)
        {
            this.cameraPosition = new(cameraPosition[0], cameraPosition[1], cameraPosition[2]);
            this.cameraTarget = new(cameraTarget[0], cameraTarget[1], cameraTarget[2]);
        }

        public Camera()
        {
            this.cameraPosition = new(0, 0, -3.0f);
            this.cameraTarget = new(0, 0, 0);
        }

        public void SetCameraPosition(float x, float y, float z)
        {
            cameraPosition = new Vector3(x, y, z);
        }

        public void SetCameraTarget(float x, float y, float z)
        {
            cameraTarget = new Vector3(x, y, z);
        }


        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(cameraPosition, cameraTarget, Vector3.UnitY);
        }

        private void MoveCamera(Direction direction, float value)
        {
            Vector3 cameraDirection = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 move;
            if (direction == Direction.ForwardBackward)
            {
                move = -cameraDirection * value;
            }
            else
            {
                bool isUpDown = direction == Direction.UpDown;
                Vector3 vector = isUpDown ? -Vector3.UnitX : -Vector3.UnitY;
                move = Vector3.Normalize(Vector3.Cross(cameraDirection, vector)) * value;
            }

            cameraPosition += move;
            cameraTarget += move;
        }

        public void MoveCameraUpDown(float value)
        {
            MoveCamera(Direction.UpDown, value);
        }

        public void MoveCameraLeftRight(float value)
        {
            MoveCamera(Direction.LeftRight, value);
        }

        public void MoveCameraForwardBackward(float value)
        {
            MoveCamera(Direction.ForwardBackward, value);
        }

        public void RotateCamera(float xValue, float yValue)
        {

            Matrix4 rotateXMatrix = Matrix4.CreateRotationX(xValue);
            Matrix4 rotateYMatrix = Matrix4.CreateRotationY(yValue);
            Vector4 newCameraPosition = new(cameraPosition, 1.0f);
            newCameraPosition = rotateXMatrix * rotateYMatrix * newCameraPosition;
            cameraPosition = newCameraPosition.Xyz;

        }
    }
}