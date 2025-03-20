using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace lab1_lines
{
    public partial class WireframeRenderer : Form
    {
        private Bitmap canvas;
        private Graphics g;
        private List<Vector3> vertices; // вершины 3D модели
        private List<Vector2[]> edges;  // рёбра 2D модели после проекции
        private Transform3D transform3D;
        private Matrix4x4 projectionMatrix;
        private Matrix4x4 modelMatrix;
        private Matrix4x4 viewMatrix;
        private float aspectRatio;

        public WireframeRenderer(List<Vector3> modelVertices, List<Vector2[]> modelEdges)
        {
            this.Text = "Wireframe 3D Model";
            this.Width = 700;
            this.Height = 500;

            this.vertices = modelVertices;
            this.edges = modelEdges;
            this.canvas = new Bitmap(800, 600);
            this.g = Graphics.FromImage(canvas);
            this.transform3D = new Transform3D();

            // настройка соотношения для перспективы
            this.aspectRatio = (float)this.Width / this.Height;

            // проекционная матрица 
            this.projectionMatrix = Transform3D.PerspectiveFov((float)Math.PI / 4, aspectRatio, 0.1f, 100f);

            this.viewMatrix = Transform3D.LookAt(
                new Vector3(0, 0, 5), // позиция камеры
                new Vector3(0, 0, 0),   // точка, на которую смотрит камера
                new Vector3(0, 1, 0)    // вектор "вверх"
            );

            
            this.modelMatrix = Matrix4x4.Identity;

            this.Paint += new PaintEventHandler(Render);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
        }

       
        private void Render(object sender, PaintEventArgs e)
        {
           
           // e.Graphics.Clear(Color.White);
            g.Clear(Color.White);

            // применение трансформации
            var transformedVertices = vertices.Select(v => {
                var v4 = new Vector4(v, 1.0f); // преобразуем в Vector4
                var world = Vector4.Transform(v4, modelMatrix); // модель в мировое пр-во
                var view = Vector4.Transform(world, viewMatrix); // вид, пр-во камеры
                var projected = Vector4.Transform(view, projectionMatrix); // проекция

                // перспективное деление
                if (projected.W != 0)
                {
                    projected.X /= projected.W;
                    projected.Y /= projected.W;
                    projected.Z /= projected.W;
                }

                // в координаты экрана
                float screenX = (projected.X + 1) * 0.5f * this.Width;
                float screenY = (1 - projected.Y) * 0.5f * this.Height;

                return new Vector2(screenX, screenY);
            }).ToList();

            // рисуем рёбра
            foreach (var edge in edges)
            {
                int index1 = (int)edge[0].X;
                int index2 = (int)edge[1].X;

                if (index1 >= 0 && index1 < transformedVertices.Count && index2 >= 0 && index2 < transformedVertices.Count)
                {
                    var p1 = transformedVertices[index1];
                    var p2 = transformedVertices[index2];

                    DrawDDALine(e.Graphics, p1, p2);
                }
            }

            // Обновляем графику на форме
            e.Graphics.DrawImage(canvas, 0, 0);
        }

        // Метод DDA для отрисовки линии
        private void DrawDDALine(Graphics graphics, Vector2 p1, Vector2 p2)
        {
            int x1 = (int)p1.X, y1 = (int)p1.Y;
            int x2 = (int)p2.X, y2 = (int)p2.Y;

            // разница по осям
            int dx = x2 - x1;
            int dy = y2 - y1;

            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            // приращение на каждом шаге по осям
            float xInc = dx / (float)steps;
            float yInc = dy / (float)steps;

            float x = x1, y = y1;
            for (int i = 0; i <= steps; i++)
            {
                int xIndex = (int)Math.Round(x);
                int yIndex = (int)Math.Round(y);

                // ограничиваем индексы в пределах экрана
                xIndex = Math.Max(0, Math.Min(this.Width - 1, xIndex));
                yIndex = Math.Max(0, Math.Min(this.Height - 1, yIndex));

                canvas.SetPixel(xIndex, yIndex, Color.Black);

                x += xInc;
                y += yInc;
            }
            
        }

        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            float rotationStep = 5.0f; // шаг вращения в градусах
            float scaleStep = 1.1f;    // шаг масштабирования

            if (e.KeyCode == Keys.Q) transform3D.RotateX(rotationStep);  // вращение по Y
            if (e.KeyCode == Keys.E) transform3D.RotateX(-rotationStep); 
            if (e.KeyCode == Keys.A) transform3D.RotateY(rotationStep);  // вращение по X
            if (e.KeyCode == Keys.D) transform3D.RotateY(-rotationStep); 
            if (e.KeyCode == Keys.Z) transform3D.RotateZ(rotationStep);  // вращение по Z
            if (e.KeyCode == Keys.C) transform3D.RotateZ(-rotationStep); 

            if (e.KeyCode == Keys.Add) transform3D.Scale(scaleStep);      // масштабирование 
            if (e.KeyCode == Keys.Subtract) transform3D.Scale(1 / scaleStep); 

            // обновление матрицы модели
            this.modelMatrix = transform3D.Matrix;

            Invalidate(); 
        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
