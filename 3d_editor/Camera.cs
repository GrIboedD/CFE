using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using System.Windows.Forms.VisualStyles;
using OpenTK.Mathematics;

namespace _3d_editor
{
    public class Camera
    {
     
        Matrix4 viewMatrix;
        Matrix4 cameraPositionMatrix;
        Matrix4 rotateMatrix;
        Matrix4 targetPositionMartix;

        public Camera()
        {
            var cameraPosition = new Vector3(0, 0, 5);
            var cameraTarget = new Vector3(0, 0, 0);

            cameraPositionMatrix = Matrix4.CreateTranslation(cameraPosition);
            rotateMatrix = Matrix4.CreateFromQuaternion(Quaternion.Identity);
            targetPositionMartix = Matrix4.CreateTranslation(cameraTarget);
            CalculateViewMatrix();
            RotateCamera(pitch: -float.Pi / 4);
        }

        public void ResetCamera()
        {
            var cameraPosition = new Vector3(0, 0, 5);
            var cameraTarget = new Vector3(0, 0, 0);

            cameraPositionMatrix = Matrix4.CreateTranslation(cameraPosition);
            rotateMatrix = Matrix4.CreateFromQuaternion(Quaternion.Identity);
            targetPositionMartix = Matrix4.CreateTranslation(cameraTarget);
            CalculateViewMatrix();
            RotateCamera(pitch: -float.Pi / 4);
        }

        public Matrix4 GetViewMatrix()
        {
            return viewMatrix;
        }

        private void CalculateViewMatrix()
        {
           viewMatrix = Matrix4.Invert(cameraPositionMatrix * rotateMatrix * targetPositionMartix);
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
        
        public void MoveCamera(float x, float y)
        {
            Vector3 moveVector = new(x, y, 0);
            cameraPositionMatrix *= Matrix4.CreateTranslation(moveVector);
            CalculateViewMatrix();
        }

        public Vector3 GetCameraPositionVector()
        {
            var cameraTransformMatrix = Matrix4.Invert(viewMatrix);
            return new Vector3(cameraTransformMatrix[3, 0], cameraTransformMatrix[3, 1], cameraTransformMatrix[3, 2]);

        }

        public Vector3 GetCameraUpDirection()
        {
            var cameraTransformMatrix = Matrix4.Invert(viewMatrix);
            return new Vector3(cameraTransformMatrix[1, 0], cameraTransformMatrix[1, 1], cameraTransformMatrix[1, 2]);
        }

        public Vector3 GetCameraFrontDirection()
        {
            var cameraTransformMatrix = Matrix4.Invert(viewMatrix);
            return new Vector3(cameraTransformMatrix[2, 0], cameraTransformMatrix[2, 1], cameraTransformMatrix[2, 2]);
        }

        public void SetTargetPosition(Vector3 position)
        {
            targetPositionMartix = Matrix4.CreateTranslation(position);
            CalculateViewMatrix();
        }
    }
}
