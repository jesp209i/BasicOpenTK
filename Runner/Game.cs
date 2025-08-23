using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runner.Buffers;

namespace Runner
{
    public class Game : GameWindow
    {
        private VertexBuffer? vertexBuffer;
        private IndexBuffer? indexBuffer;
        private VertexArray? vertexArray;
        private ShaderProgram? shaderProgram;

        private int indexCount;

        private float colorFactor = 1f;
        private float deltaColorFactor = 1f / 8024f;


        public Game(int width = 1280, int height = 768, string title = "Hello Hest") : base(GameWindowSettings.Default, 
            new NativeWindowSettings
            {
                Title = title,
                ClientSize = new Vector2i(width, height),
                WindowBorder = WindowBorder.Fixed,
                StartVisible = false,
                StartFocused = true,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(3,3)
            })
        {
            this.CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0,0,e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            IsVisible = true;
            GL.ClearColor(GetRandomColor4());

            Random rand = new Random();

            int boxCount = rand.Next(5, 250);
            Console.WriteLine($"boxCount: {boxCount}");

            int windowWidth = ClientSize.X;
            int windowHeight = ClientSize.Y;

            VertexPositionColor[] verticies = new VertexPositionColor[boxCount * 4];

            var vertexCount = 0;
            for (int i = 0; i < boxCount; i++)
            {
                int w = rand.Next(32, 128);
                int h = rand.Next(32, 128);
                int x = rand.Next(0, windowWidth - w );
                int y = rand.Next(0, windowHeight - h );

                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x, y + h), GetRandomColor4());
                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x + w, y + h), GetRandomColor4());
                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x + w, y), GetRandomColor4());
                verticies[vertexCount++] = new VertexPositionColor(new Vector2(x, y), GetRandomColor4());

            }

            int[] indicies = new int[boxCount * 6];
            indexCount = 0;
            vertexCount = 0;
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

            vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, verticies.Length, true);
            vertexBuffer.SetData(verticies, verticies.Length);

            indexBuffer = new IndexBuffer(indicies.Length, true);
            indexBuffer.SetData(indicies, indicies.Length);

            vertexArray = new VertexArray(vertexBuffer);


            string vertexShaderCode = File.ReadAllText("ShaderPrograms/box-vertex-shader.vert");
            string pixelShaderCode = File.ReadAllText("ShaderPrograms/box-color-shader.frag");

            shaderProgram = new ShaderProgram(vertexShaderCode, pixelShaderCode);

            
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            shaderProgram.SetUniform("ViewportSize", (float)viewport[2], (float)viewport[3]);
            shaderProgram.SetUniform("ColorFactor", colorFactor);

            base.OnLoad();
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
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            if (shaderProgram is null || vertexArray is null || indexBuffer is null)
            {
                throw new ArgumentNullException("ShaderProgram, VertexArray or IndexBuffer is null");
            }

            GL.UseProgram(shaderProgram.ShaderProgramHandle);
            GL.BindVertexArray(vertexArray.VertexArrayHandle);
            GL.BindBuffer(indexBuffer.BufferTarget, indexBuffer.Handle);

            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        private static Color4 GetRandomColor4(float alpha = 1f)
        {

            Random rand = new Random();

            float r = (float)rand.NextDouble();
            float g = (float)rand.NextDouble();
            float b = (float)rand.NextDouble();

            return new Color4(r, g, b, alpha);
        }
    }
}
