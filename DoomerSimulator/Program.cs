using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Assimp;
using GK.Utils;
using Camera = GK.Utils.Camera;

namespace GK
{
    class Game : GameWindow
    {
        private Dictionary<string, Model> _models = new Dictionary<string, Model>();
        private Shader _shader;

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        private Vector3 _sun = new Vector3(100, 50, -170);
        private Skybox _skybox;
        private Matrix4 _rot = Matrix4.CreateRotationY((float) Math.PI / 240);
        private int i;
        private float fog = 400;
        private int daytime;
        private int _cameraMode = 1;
        private float _yaw;
        private float _pitch;

        public Game()
            : base(800, 600, GraphicsMode.Default, "Doomer Simulator")
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _camera = new Camera(Vector3.UnitZ * 3, Width / (float) Height);
            _camera.Position = new Vector3(0, 18, -8);
            CursorVisible = false;

            _skybox = new Skybox();
            _models["canyon"] = ModelLoader.LoadFromFile("../../Resources/canyon.obj", PostProcessSteps.Triangulate);
            _models["canyon"].textures.Add(new Texture("../../Resources/canyon.png"));
            _models["canyon"].matrix = Matrix4.Identity * Matrix4.CreateRotationX((float) (-Math.PI / 2)) *
                                       Matrix4.CreateScale(1 / 10f);
            try
            {
                _models["ship"] = ModelLoader.LoadFromFile("../../Resources/ship.obj", PostProcessSteps.Triangulate);
                _models["ship"].textures.Add(new Texture("../../Resources/ship.jpg"));
                _models["ship"].matrix = Matrix4.CreateRotationY(-(float) Math.PI / 2);
                _models["ship"].matrix = Matrix4.Identity * Matrix4.CreateRotationY(-(float) Math.PI / 2) *
                                         Matrix4.CreateScale(1 / 500f) * Matrix4.CreateTranslation(0, 4, -58);
            }
            catch (Exception ex)
            {
                Title = ex.Message + (_models["ship"] is null);
            }

            SetShader("phongShader");

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.AspectRatio = (float) Width / Height;
            base.OnResize(e);
        }

        private void SetShader(string shaderName)
        {
            try
            {
                _shader = new Shader("../../Shaders/" + shaderName + ".vert", "../../Shaders/" + shaderName + ".frag");
                _shader.SetVector3("lightPos", _sun);
                _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetInt("lightingModel", 1);
            }
            catch (Exception exception)
            {
                Title = exception.Message;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused) // check to see if the window is focused
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.P))
            {
                _shader.SetInt("lightingModel", 1);
            }

            if (input.IsKeyDown(Key.B))
            {
                _shader.SetInt("lightingModel", 2);
            }

            if (input.IsKeyDown(Key.V))
            {
                SetShader("gouraudShader");
            }

            if (input.IsKeyDown(Key.F))
            {
                SetShader("phongShader");
            }

            if (input.IsKeyDown(Key.Number1) || input.IsKeyDown(Key.Keypad1))
            {
                _cameraMode = 1;
            }

            if (input.IsKeyDown(Key.Number2) || input.IsKeyDown(Key.Keypad2))
            {
                _cameraMode = 2;
            }

            if (input.IsKeyDown(Key.Number3) || input.IsKeyDown(Key.Keypad3))
            {
                _cameraMode = 3;
            }

            if (input.IsKeyDown(Key.Up))
            {
                _pitch -= MathHelper.PiOver6 / 12;
            }

            if (input.IsKeyDown(Key.Down))
            {
                _pitch += MathHelper.PiOver6 / 12;
            }

            if (input.IsKeyDown(Key.Right))
            {
                _yaw += MathHelper.PiOver6 / 12;
            }

            if (input.IsKeyDown(Key.Left))
            {
                _yaw -= MathHelper.PiOver6 / 12;
            }

            if (input.IsKeyDown(Key.Minus) || input.IsKeyDown(Key.KeypadMinus))
            {
                fog = (int) (fog * 1.1);
                if (fog > 10000)
                    fog = 10000;
            }

            if (input.IsKeyDown(Key.Plus) || input.IsKeyDown(Key.KeypadPlus))
            {
                fog = (int) (fog / 1.1);
                if (fog < 20)
                    fog = 20;
            }

            _shader.SetVector3("lightPos", _sun);
            _shader.SetInt("fogStrength", (int) fog);
            _skybox.Shader.SetFloat("fogStrength", fog > 1000 ? 1 : fog / 1000);

            if (_cameraMode == 1)
            {
                const float cameraSpeed = 15f;
                const float sensitivity = 0.2f;

                if (input.IsKeyDown(Key.W))
                {
                    _camera.Position += _camera.Front * cameraSpeed * (float) e.Time; // Forward
                }

                if (input.IsKeyDown(Key.S))
                {
                    _camera.Position -= _camera.Front * cameraSpeed * (float) e.Time; // Backwards
                }

                if (input.IsKeyDown(Key.A))
                {
                    _camera.Position -= _camera.Right * cameraSpeed * (float) e.Time; // Left
                }

                if (input.IsKeyDown(Key.D))
                {
                    _camera.Position += _camera.Right * cameraSpeed * (float) e.Time; // Right
                }

                if (input.IsKeyDown(Key.Space))
                {
                    _camera.Position += _camera.Up * cameraSpeed * (float) e.Time; // Up
                }

                if (input.IsKeyDown(Key.LShift))
                {
                    _camera.Position -= _camera.Up * cameraSpeed * (float) e.Time; // Down
                }

                // Get the mouse state
                var mouse = Mouse.GetState();

                if (_firstMove) // this bool variable is initially set to true
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    // Calculate the offset of the mouse position
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
                }
            }

            var p1 = _models["ship"].matrix.Row3.Xyz;
            MoveShip();
            var p2 = _models["ship"].matrix.Row3.Xyz;
            var spotlightDir = p2 - p1;
            spotlightDir.Y = 0;
            spotlightDir = Vector3.Normalize(spotlightDir);
            spotlightDir.Y = -(float) 1;
            spotlightDir = Vector3.Normalize(spotlightDir);
            spotlightDir *= Matrix3.CreateRotationY(-_yaw);
            var axis = Vector3.Normalize(new Vector3(spotlightDir.X, 0, spotlightDir.Z) *
                                         Matrix3.CreateRotationY(MathHelper.PiOver2));
            spotlightDir *= Matrix3.CreateFromAxisAngle(axis, _pitch);
            spotlightDir = Vector3.Normalize(spotlightDir);

            _shader.SetVector3("spotlight.direction", spotlightDir);
            _shader.SetVector3("spotlight.position", p2 + Vector3.Normalize(p2 - p1));
            _shader.SetFloat("spotlight.cutOff", (float) Math.Cos(MathHelper.DegreesToRadians(30)));

            if (_cameraMode == 2)
            {
                _camera.Pitch = 0;
                _camera.Yaw = 0;
                var front = p2 - p1;
                front.Y = 0;
                front = Vector3.Normalize(front);
                front.Y = -(float) 1 / 2;
                _camera.Front = front;
                _camera.Position = _models["ship"].matrix.Row3.Xyz + 2 * Vector3.UnitY - _camera.Front / 2;
            }

            if (_cameraMode == 3)
            {
                _camera.Pitch = 0;
                _camera.Yaw = 0;
                var c3 = new Vector3(11, 20, -40);
                _camera.Position = c3;
                _camera.Front = _models["ship"].matrix.Row3.Xyz - c3;
            }

            _skybox.Update();
            daytime = (daytime + 1) % 1440;
            float time = (float) Math.Abs(daytime - 720) / 720;
            _shader.SetFloat("time", 1 - time);
            base.OnUpdateFrame(e);
        }

        private void MoveShip()
        {
            i = (i + 1) % 720;
            if (i == 0)
                _models["ship"].matrix = Matrix4.Identity * Matrix4.CreateRotationY(-(float) Math.PI / 2) *
                                         Matrix4.CreateScale(1 / 500f) * Matrix4.CreateTranslation(0, 4, -58);
            if (i <= 240)
            {
                _models["ship"].matrix *= Matrix4.CreateTranslation(0, 0, 40);
                _models["ship"].matrix *= _rot;
                _models["ship"].matrix *= Matrix4.CreateTranslation(0, (float) 5 / 240, 0);
                _models["ship"].matrix *= Matrix4.CreateTranslation(0, 0, -40);
            }

            if (i > 240 && i <= 360)
            {
                _models["ship"].matrix *= Matrix4.CreateTranslation((float) 22 / 120, 0, 0);
            }

            if (i > 360 && i <= 600)
            {
                _models["ship"].matrix *= Matrix4.CreateTranslation(-22, 0, 40);
                _models["ship"].matrix *= _rot;
                _models["ship"].matrix *= Matrix4.CreateTranslation(0, -(float) 5 / 240, 0);
                _models["ship"].matrix *= Matrix4.CreateTranslation(22, 0, -40);
            }

            if (i > 600)
            {
                _models["ship"].matrix *= Matrix4.CreateTranslation(-(float) 22 / 120, 0, 0);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.SetVector3("viewPos", _camera.Position);

            try
            {
                _skybox.Shader.Use();
                var pos = _camera.Position;
                _camera.Position = Vector3.Zero;
                _skybox.Shader.SetMatrix4("view", _camera.GetViewMatrix());
                _skybox.Shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
                _skybox.Render();
                _camera.Position = pos;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            ////////////////////////////////////////////////////////////////////////////////////
            _shader.Use();
            foreach (var m in _models)
            {
                var model = m.Value;
                try
                {
                    _shader.SetMatrix4("model", model.matrix);
                    _shader.SetMatrix4("view", _camera.GetViewMatrix());
                    _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }

                model.Render();
            }

            GL.End();
            SwapBuffers();
        }

        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}