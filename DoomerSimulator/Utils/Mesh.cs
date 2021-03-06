﻿using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace GK.Utils
{
    public class Mesh
    {
        public List<int> vboIdList = new List<int>(1000);
        int vertexCount;
        int vaoId;
        float boundingRadius;


        public void Init()
        {
            GL.BindVertexArray(vaoId);
        }

        public void End()
        {
            GL.BindVertexArray(0);
        }

        public void Render()
        {
            Init();

            GL.DrawElements(PrimitiveType.Triangles, vertexCount, DrawElementsType.UnsignedInt, 0);
            GL.Finish();
            End();
        }

        public Mesh(List<float> positions, List<float> textCoords, List<float> normals, List<int> indices)
        {
            vertexCount = indices.Count;

            vaoId = GL.GenVertexArray();

            GL.BindVertexArray(vaoId);

            int vboId = GL.GenBuffer();
            vboIdList.Add(vboId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer,
                positions.Count * sizeof(float),
                positions.ToArray(),
                BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);


            vboId = GL.GenBuffer();
            vboIdList.Add(vboId);


            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer,
                textCoords.Count * sizeof(float),
                textCoords.ToArray(),
                BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);


            vboId = GL.GenBuffer();
            vboIdList.Add(vboId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer,
                normals.Count * sizeof(float),
                normals.ToArray(),
                BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);


            vboId = GL.GenBuffer();
            vboIdList.Add(vboId);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                indices.Count * sizeof(int),
                indices.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public static void processVertices(Assimp.Mesh mesh, List<float> vertices)
        {
            foreach (var v in mesh.Vertices)
            {
                vertices.Add(v.X);
                vertices.Add(v.Y);
                vertices.Add(v.Z);
            }
        }

        public static void processTextCoord(Assimp.Mesh mesh, List<float> textCoord)
        {
            if (!mesh.HasTextureCoords(0)) return;
            foreach (var tc in mesh.TextureCoordinateChannels[0])
            {
                textCoord.Add(tc.X);
                textCoord.Add(1 - tc.Y);
            }
        }

        public static void processNormals(Assimp.Mesh mesh, List<float> normals)
        {
            foreach (var n in mesh.Normals)
            {
                normals.Add(n.X);
                normals.Add(n.Y);
                normals.Add(n.Z);
            }
        }

        public static void processIndices(Assimp.Mesh mesh, List<int> indices)
        {
            foreach (var f in mesh.Faces)
            {
                foreach (var ind in f.Indices)
                {
                    indices.Add(ind);
                }
            }
        }

        public static Mesh processMesh(Assimp.Mesh mesh)
        {
            List<float> positions = new List<float>();
            List<float> textures = new List<float>();
            List<float> normals = new List<float>();
            List<int> indices = new List<int>();

            processIndices(mesh, indices);
            processNormals(mesh, normals);
            processVertices(mesh, positions);
            processTextCoord(mesh, textures);

            return new Mesh(positions, textures, normals, indices);
        }
    }
}