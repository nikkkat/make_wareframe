using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;

namespace lab1_lines
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
            openFileDialog.Title = "Select a 3D Model File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // загружаем модель из файла
                ObjParser objParser = new ObjParser();
                objParser.Load(openFileDialog.FileName);

                // рёбра модели 
                List<Vector2[]> edges = new List<Vector2[]>();

                // 3D координаты в 2D
                foreach (var face in objParser.Faces)
                {
                    for (int i = 0; i < face.Vertices.Count; i++)
                    {
                        var v1 = face.Vertices[i].VertexIndex;
                        var v2 = face.Vertices[(i + 1) % face.Vertices.Count].VertexIndex;

                        edges.Add(new Vector2[] { new Vector2(v1, 0), new Vector2(v2, 0) });
                    }
                }

                List<Vector3> vertices = objParser.Vertices;
                
                Application.Run(new WireframeRenderer(vertices, edges));
            }
        }
    }
}
