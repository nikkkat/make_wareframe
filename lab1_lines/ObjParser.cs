using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.IO;

namespace lab1_lines
{
    internal class ObjParser
    {
        public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
        public List<Vector3> Normals { get; private set; } = new List<Vector3>();
        public List<Vector2> TextureCoords { get; private set; } = new List<Vector2>();
        public List<Face> Faces { get; private set; } = new List<Face>();

        public void Load(string path)
        {
            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0 || parts[0].StartsWith("#")) continue;

                try
                {
                    switch (parts[0])
                    {
                        case "v":
                            Vertices.Add(ParseVector3(parts.Skip(1).ToArray()));
                            break;
                        case "vt":
                            TextureCoords.Add(ParseVector2(parts.Skip(1).ToArray()));
                            break;
                        case "vn":
                            Normals.Add(ParseVector3(parts.Skip(1).ToArray()));
                            break;
                        case "f":
                            Faces.Add(ParseFace(parts.Skip(1).ToArray()));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке строки: {line} -> {ex.Message}");
                }
            }
        }

        private Vector3 ParseVector3(string[] parts)
        {
            return new Vector3(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                parts.Length > 2 ? float.Parse(parts[2], CultureInfo.InvariantCulture) : 0);
        }

        private Vector2 ParseVector2(string[] parts)
        {
            return new Vector2(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                parts.Length > 1 ? float.Parse(parts[1], CultureInfo.InvariantCulture) : 0);
        }

        private Face ParseFace(string[] parts)
        {
            var vertices = parts.Select(ParseFaceVertex).ToList();
            return new Face(vertices);
        }
        
        private int ParseIndex(string indexStr, int count)
        {
            int index = int.Parse(indexStr);
            return index < 0 ? count + index : index - 1;
        }

        private FaceVertex ParseFaceVertex(string part)
        {
            var indices = part.Split('/');
            int v = ParseIndex(indices[0], Vertices.Count);
            int vt = (indices.Length > 1 && indices[1] != "") ? ParseIndex(indices[1], TextureCoords.Count) : -1;
            int vn = (indices.Length > 2 && indices[2] != "") ? ParseIndex(indices[2], Normals.Count) : -1;

            return new FaceVertex(v, vt, vn);
        }
    }

    public class Face
    {
        public List<FaceVertex> Vertices { get; }

        public Face(List<FaceVertex> vertices)
        {
            Vertices = vertices;
        }
    }

    public class FaceVertex
    {
        public int VertexIndex { get; }
        public int TextureIndex { get; }
        public int NormalIndex { get; }

        public FaceVertex(int vertexIndex, int textureIndex, int normalIndex)
        {
            VertexIndex = vertexIndex;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
        }
    }
}
