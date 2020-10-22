/*
The MIT License (MIT)

Copyright (c) 2015 Kenneth Lewis

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace GK.Utils {
	public class Skybox
	{

		public Model box;
		private float angle;
		private int time = 0;

		public void Update() {
			angle += 0.005f;
			angle %= 360f;
			time = (time + 1) % 1440;
		}

		public Shader Shader;
		
		private float[] vertices = {
			//forward      
			-1, -1, 1,
			-1, 1, 1,
			1, 1, 1,
			1, -1, 1,
			//top
			-1, 1, 1,
			-1,1, -1,
			1, 1, -1,
			1,1, 1,
			//left
			-1, -1, -1,
			-1, 1, -1,
			-1, 1, 1,
			-1, -1, 1,
			//right
			1, -1, 1,
			1, 1, 1,
			1, 1, -1,
			1, -1, -1,
			//back
			1, -1, -1,
			1, 1, -1,
			-1, 1, -1,
			-1, -1, -1,
		};
		
		private float[] texCoords = {
			//forward      
			0.25f, 2f / 3f,
			0.25f, 1f / 3f,
			0.5f, 1f / 3f,
			0.5f, 2f / 3f,
			//top
			0.255f, 1f / 3f,
			0.255f, 0f / 3f,
			0.495f, 0f / 3f,
			0.495f, 1f / 3f,
			//left
			0.00f, 2f / 3f,
			0.00f, 1f / 3f,
			0.25f, 1f / 3f,
			0.25f, 2f / 3f,
			//right
			0.50f, 2f / 3f,
			0.50f, 1f / 3f,
			0.75f, 1f / 3f,
			0.75f, 2f / 3f,
			//back
			0.75f, 2f / 3f,
			0.75f, 1f / 3f,
			1f, 1f / 3f,
			1f, 2f / 3f,
		};

		public Skybox() {
			box = new Model();
			box.textures.Add( new Texture("../../Resources/skybox5.png"));
			box.textures.Add( new Texture("../../Resources/night.jpg"));
			List<int> indicesList = new List<int>();
			Shader = new Shader("../../Shaders/skyboxShader.vert", "../../Shaders/skyboxShader.frag");
			Shader.SetInt("texture0", 0);
			Shader.SetInt("texture1", 1);

			//forward

			indicesList.Add(0);
			indicesList.Add(1);
			indicesList.Add(2);

			indicesList.Add(2);
			indicesList.Add(3);
			indicesList.Add(0);

			//top

			indicesList.Add(4);
			indicesList.Add(5);
			indicesList.Add(6);

			indicesList.Add(6);
			indicesList.Add(7);
			indicesList.Add(4);

			//left

			indicesList.Add(8);
			indicesList.Add(9);
			indicesList.Add(10);

			indicesList.Add(10);
			indicesList.Add(11);
			indicesList.Add(8);

			//right

			indicesList.Add(12);
			indicesList.Add(13);
			indicesList.Add(14);

			indicesList.Add(14);
			indicesList.Add(15);
			indicesList.Add(12);

			//back

			indicesList.Add(16);
			indicesList.Add(17);
			indicesList.Add(18);

			indicesList.Add(18);
			indicesList.Add(19);
			indicesList.Add(16);
			
			box.meshes.Add(new Mesh(vertices.ToList(), texCoords.ToList(), new List<float>(), indicesList));
			box.matrix=Matrix4.Identity*((float)12);
		}

		public void Use()
		{
			Shader.Use();
		}
		public void Render() {
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.CullFace);
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Fog);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.PushMatrix();
			GL.Disable(EnableCap.DepthTest);
			float scale = 20000f;
			GL.Scale(new Vector3(scale, scale, scale));
			GL.Translate(new Vector3(0f, 0.2f, 0f));

			Shader.SetMatrix4("model",  box.matrix);
			Shader.SetFloat("time",(float)Math.Abs(time-720)/720);
			box.Render();
			GL.PopMatrix();
			GL.Enable(EnableCap.Fog);
			GL.Enable(EnableCap.DepthTest);
		}
	}
}