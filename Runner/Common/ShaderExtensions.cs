using OpenTK.Graphics.OpenGL;

namespace Runner.Common
{
    internal static class ShaderExtensions
    {
        internal static bool CompileVertexShader(this Shader self, string vertexShaderCode, out int vertexShaderHandle, out string errorMessage)
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            return self.CompileShader(vertexShaderCode, vertexShaderHandle, out errorMessage);
        }

        internal static bool CompilePixelShader(this Shader self, string pixelShaderCode, out int pixelShaderHandle, out string errorMessage)
        {
            pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            return self.CompileShader(pixelShaderCode, pixelShaderHandle, out errorMessage);
        }

        internal static bool CompileShader(this Shader self, string shaderCode, int shaderHandle, out string errorMessage)
        {
            errorMessage = string.Empty;

            GL.ShaderSource(shaderHandle, shaderCode);
            GL.CompileShader(shaderHandle);
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out var statusCode);
            if (statusCode != (int)All.True)
            {
                var vertexShaderInfo = GL.GetShaderInfoLog(shaderHandle);
                errorMessage = vertexShaderInfo;

                return false;
            }

            return true;
        }

        internal static int LinkProgram(this Shader self, int[] shaders)
        {

            int shaderProgramHandle = GL.CreateProgram();
            for (int i = 0; i < shaders.Length; i++)
            {
                GL.AttachShader(shaderProgramHandle, shaders[i]);
            }

            GL.LinkProgram(shaderProgramHandle);
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.LinkStatus, out var statusCode);
            if (statusCode != (int)All.True)
            {
                throw new Exception($"Error occurred while linking Program({shaderProgramHandle})");
            }

            for (int i = 0; i < shaders.Length; i++)
            {
                GL.DetachShader(shaderProgramHandle, shaders[i]);
                GL.DeleteShader(shaders[i]);
            }
            return shaderProgramHandle;
        }

        public static ShaderUniform[] GetUniformList(this Shader self)
        {
            var result = new ShaderUniform[self.Uniforms.Length];
            Array.Copy(self.Uniforms, result, self.Uniforms.Length);
            return result;
        }

        public static ShaderAttribute[] GetAttributeList(this Shader self)
        {
            var result = new ShaderAttribute[self.Attributes.Length];
            Array.Copy(self.Attributes, result, self.Attributes.Length);
            return result;
        }
    }
}
