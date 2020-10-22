using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GK.Utils
{
    public class Model
    {
        public Matrix4 matrix;
        public List<Texture> textures = new List<Texture>();
        public List<Mesh> meshes = new List<Mesh>();

        public void Render()
        {
            int i = 0;
            foreach (var tex in textures)
            {
                if(i==0)
                    tex.Use();
                if(i==1)
                    tex.Use(TextureUnit.Texture1);
                i++;
            }
            foreach (var mesh in meshes)
            {
                mesh.Render();
            }
        }
    }
}