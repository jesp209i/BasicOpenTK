namespace Runner
{
    public class GameOptions
    {
        public string VertexShaderPath { get; }
        public string FragmentShaderPath { get; }

        public GameOptions(string vertexShaderPath = "Shaders/vertex-shader.vert", string fragmentShaderPath = "Shaders/fragment-shader.frag")
        {
            VertexShaderPath = vertexShaderPath;
            FragmentShaderPath = fragmentShaderPath;
        }
    }
}
