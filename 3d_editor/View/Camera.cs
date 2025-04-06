using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using System.Windows.Forms.VisualStyles;
using OpenTK.Mathematics;

namespace _3d_editor.View
{
    public class Camera
    {
     
        Matrix4 viewMatrix;
        Matrix4 scaleMatrix;
        Matrix4 cameraPositionMatrix;
        Matrix4 rotateMatrix;
        Matrix4 targetPositionMartix;

        public Camera()
        {
            var cameraPosition = new Vector3(0, 0, 5);
            var cameraTarget = new Vector3(0, 0, 0);

            scaleMatrix = Matrix4.Identity;
            cameraPositionMatrix = Matrix4.CreateTranslation(cameraPosition);
            rotateMatrix = Matrix4.CreateFromQuaternion(Quaternion.Identity);
            targetPositionMartix = Matrix4.CreateTranslation(cameraTarget);
            CalculateViewMatrix();
        }

        public Matrix4 GetViewMatrix()
        {
            return viewMatrix;
        }

        private void CalculateViewMatrix()
        {
           viewMatrix = scaleMatrix * Matrix4.Invert(cameraPositionMatrix * rotateMatrix * targetPositionMartix);
        }

        public void RotateCamera(float pitch = 0.0f, float yaw = 0.0f, float roll = 0.0f)
        {
            var cameraTransformMatrix = Matrix4.Invert(viewMatrix);
            var cameraFront = new Vector3(cameraTransformMatrix[2, 0], cameraTransformMatrix[2, 1], cameraTransformMatrix[2, 2]);
            var cameraUp = new Vector3(cameraTransformMatrix[1, 0], cameraTransformMatrix[1, 1], cameraTransformMatrix[1, 2]);
            var cameraRight = new Vector3(cameraTransformMatrix[0, 0], cameraTransformMatrix[0, 1], cameraTransformMatrix[0, 2]);

            var rotatorX = Quaternion.FromAxisAngle(cameraRight, pitch);
            var rotatorY = Quaternion.FromAxisAngle(cameraUp, yaw);
            var rotatorZ = Quaternion.FromAxisAngle(cameraFront, roll);

            rotateMatrix *= Matrix4.CreateFromQuaternion(Quaternion.Add(Quaternion.Add(rotatorX, rotatorY), rotatorZ));
            CalculateViewMatrix();
        }

        public void MoveCamera(float upDown = 0.0f, float leftRight = 0.0f, float backForward = 0.0f)
        {
            cameraPositionMatrix *= Matrix4.CreateTranslation(leftRight, upDown, backForward);
            CalculateViewMatrix();
        }

        public void SetZoom(float zoomFactor)
        {
            scaleMatrix = Matrix4.CreateScale(zoomFactor, zoomFactor, zoomFactor);
            CalculateViewMatrix();
        }

        public Vector3 GetCameraPositionVector()
        {
            var cameraTransformMatrix = Matrix4.Invert(viewMatrix);
            return new Vector3(cameraTransformMatrix[3, 0], cameraTransformMatrix[3, 1], cameraTransformMatrix[3, 2]);
        }
    }
}
