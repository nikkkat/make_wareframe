using System;
using System.Numerics;


namespace lab1_lines
{
    
    public class Transform3D
    {
        public Matrix4x4 Matrix { get; private set; } = Matrix4x4.Identity;


        // вращение по X
        public void RotateX(float degrees)
        {
            float radians = (float)(Math.PI * degrees / 180.0);
            var rotation = Matrix4x4.CreateRotationX(radians);
            Matrix = Matrix4x4.Multiply(Matrix, rotation); 
        }
        
        // вращение по Y
        public void RotateY(float degrees)
        {
            float radians = (float)(Math.PI * degrees / 180.0);
            var rotation = Matrix4x4.CreateRotationY(radians);
            Matrix = Matrix4x4.Multiply(Matrix, rotation); 
        }

        // вращение по Z
        public void RotateZ(float degrees)
        {
            float radians = (float)(Math.PI * degrees / 180.0);
            var rotation = Matrix4x4.CreateRotationZ(radians);
            Matrix = Matrix4x4.Multiply(Matrix, rotation); 
        }

        // масштабирование
        public void Scale(float scale)
        {
            var scaling = Matrix4x4.CreateScale(scale, scale, scale);
            Matrix = Matrix4x4.Multiply(Matrix, scaling);
        }



        public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            // оси нового базиса
            Vector3 zAxis = Vector3.Normalize(eye - target); // направление назад (из камеры)
            Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis)); // правая ось камеры
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis); // вертикальная ось камеры

            // матрица перехода из мирового пространства в пространство камеры
            return new Matrix4x4(
                xAxis.X, yAxis.X, zAxis.X, 0,
                xAxis.Y, yAxis.Y, zAxis.Y, 0,
                xAxis.Z, yAxis.Z, zAxis.Z, 0,
                -Vector3.Dot(xAxis, eye), -Vector3.Dot(yAxis, eye), -Vector3.Dot(zAxis, eye), 1
            );
        }


        // перспективная проекция 
        public static Matrix4x4 PerspectiveFov(float fovY, float aspect, float zNear, float zFar)
        {
            float tanHalfFov = (float)Math.Tan(fovY / 2.0);
            return new Matrix4x4(
                1.0f / (aspect * tanHalfFov), 0, 0, 0,
                0, 1.0f / tanHalfFov, 0, 0,
                0, 0, zFar / (zNear - zFar), -1,
                0, 0, (zNear * zFar) / (zNear - zFar), 0
            );
        }
        
        
    }
}
