using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Runner.Buffers;
using Runner.Common;


namespace Runner
{
    public class Game : GameWindow
    {
        private VertexBuffer? vertexBuffer;
        private IndexBuffer? indexBuffer;
        private VertexArray? vertexArray;
        private Shader? shaderProgram;

        private Camera _camera;

        private float colorFactor = 1f;
        private float deltaColorFactor = 1f / 8024f;
        private readonly Random _random;
        private string vertexShaderPath;
        private string fragmentShaderPath;

        float speed = 1.5f;

        Vector3 position = new Vector3(0.0f, 0.0f, 3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, GameOptions options) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            this.CenterWindow();
            _random = new Random();
            vertexShaderPath = options.VertexShaderPath;
            fragmentShaderPath = options.FragmentShaderPath;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0,0,e.Width, e.Height);
            Console.WriteLine($"New width: {e.Width} - new height: {e.Height}");
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            IsVisible = true;
            GL.ClearColor(GetRandomColor4());

            int boxCount = _random.Next(5, 250);
            Console.WriteLine($"boxCount: {boxCount}");

            int windowWidth = ClientSize.X;
            int windowHeight = ClientSize.Y;

            vertexBuffer = CreateVertexBuffer(boxCount, windowWidth, windowHeight);

            indexBuffer = CreateIndexBuffer(boxCount);

            vertexArray = new VertexArray(vertexBuffer);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            shaderProgram = CreateAndConfigureShaderProgram(viewport);

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);


        }

        protected override void OnUnload()
        {
            vertexArray?.Dispose();
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            KeyboardState input = KeyboardState;

            colorFactor += deltaColorFactor;
            if (colorFactor >= 1f)
            {
                colorFactor = 1f;
                deltaColorFactor *= -1f;
            }
            if (colorFactor <= 0f)
            {
                colorFactor = 0f;
                deltaColorFactor *= -1f;
            }

            shaderProgram?.SetUniform("ColorFactor", colorFactor);

            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Console.WriteLine("Escape");
                Close();
            }
            if (input.IsKeyDown(Keys.W))
            {
                Console.WriteLine("W");
                position += front * speed; //Forward 
            }

            if (input.IsKeyDown(Keys.S))
            {
                Console.WriteLine("S");
                position -= front * speed; //Backwards
            }

            if (input.IsKeyDown(Keys.A))
            {
                Console.WriteLine("A");
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed; //Left
            }

            if (input.IsKeyDown(Keys.D))
            {
                Console.WriteLine("D");
                position += Vector3.Normalize(Vector3.Cross(front, up)) * speed; //Right
            }

            if (input.IsKeyDown(Keys.Space))
            {
                Console.WriteLine("Space");
                position += up * speed; //Up 
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                Console.WriteLine("LeftShift");
                position -= up * speed; //Down
            }

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            if (shaderProgram is null || vertexArray is null || indexBuffer is null)
            {
                throw new ArgumentNullException("ShaderProgram, VertexArray or IndexBuffer is null");
            }

            shaderProgram.Use();
            GL.BindVertexArray(vertexArray.VertexArrayHandle);
            GL.BindBuffer(indexBuffer.BufferTarget, indexBuffer.Handle);

            GL.DrawElements(PrimitiveType.Triangles, indexBuffer.BufferCount, DrawElementsType.UnsignedInt, 0);
            Context.SwapBuffers();
        }

        private static Color4 GetRandomColor4(float alpha = 1f)
        {
            Random rand = new Random();

            float r = (float)rand.NextDouble();
            float g = (float)rand.NextDouble();
            float b = (float)rand.NextDouble();

            return new Color4(r, g, b, alpha);
        }

        private IndexBuffer CreateIndexBuffer(int boxCount)
        {
            int[] indicies = new int[boxCount * 6];
            var indexCount = 0;
            var vertexCount = 0;
            for (int i = 0; i < boxCount; i++)
            {
                indicies[indexCount++] = 0 + vertexCount;
                indicies[indexCount++] = 1 + vertexCount;
                indicies[indexCount++] = 2 + vertexCount;
                indicies[indexCount++] = 0 + vertexCount;
                indicies[indexCount++] = 2 + vertexCount;
                indicies[indexCount++] = 3 + vertexCount;

                vertexCount += 4;
            }

            var indexBuffer = new IndexBuffer(indicies.Length, true);
            indexBuffer.SetData(indicies, indicies.Length);
            return indexBuffer;
        }

        private VertexBuffer CreateVertexBuffer(int boxCount, int windowWidth, int windowHeight)
        {
            VertexPositionColor[] verticies = new VertexPositionColor[boxCount * 4];

            var vertexCount = 0;
            for (int i = 0; i < boxCount; i++)
            {
                int w = _random.Next(32, 128);
                int h = _random.Next(32, 128);
                int x = _random.Next(0, windowWidth - w);
                int y = _random.Next(0, windowHeight - h);

                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x, y + h), GetRandomColor4());
                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x + w, y + h), GetRandomColor4());
                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x + w, y), GetRandomColor4());
                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x, y), GetRandomColor4());

            }

            var vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, verticies.Length, true);
            vertexBuffer.SetData(verticies, verticies.Length);

            return vertexBuffer;
        }

        private Shader CreateAndConfigureShaderProgram(int[] viewport)
        {

            var shaderProgram = new Shader(vertexShaderPath, fragmentShaderPath);

            shaderProgram.SetUniform("ViewportSize", (float)viewport[2], (float)viewport[3]);
            shaderProgram.SetUniform("ColorFactor", colorFactor);

            return shaderProgram;
        }
    }
}
