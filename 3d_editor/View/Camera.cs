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
           viewMatrix = Matrix4.Invert(scaleMatrix * cameraPositionMatrix * rotateMatrix * targetPositionMartix);
        }

        public void RotateCamera(float xValue, float yValue)
        {
            var cameraTransformMatrix = Matrix4.Invert(viewMatrix);
            var cameraFront = new Vector3(cameraTransformMatrix[2, 0], cameraTransformMatrix[2, 1], cameraTransformMatrix[2, 2]);
            var cameraUp = new Vector3(cameraTransformMatrix[1, 0], cameraTransformMatrix[1, 1], cameraTransformMatrix[1, 2]);
            var cameraRight = new Vector3(cameraTransformMatrix[0, 0], cameraTransformMatrix[0, 1], cameraTransformMatrix[0, 2]);


            var rotatorX = Quaternion.FromAxisAngle(cameraRight, xValue);
            var rotatorY = Quaternion.FromAxisAngle(cameraUp, yValue);

            rotateMatrix *= Matrix4.CreateFromQuaternion(Quaternion.Add(rotatorX, rotatorY));
            CalculateViewMatrix();
        }
    }
}
