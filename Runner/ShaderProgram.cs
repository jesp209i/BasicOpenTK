using OpenTK.Graphics.OpenGL;

namespace Runner
{
    public readonly struct ShaderUniform
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveUniformType Type;

        public ShaderUniform(string name, int location, ActiveUniformType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    
    public readonly struct ShaderAttribute
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveAttribType Type;

        public ShaderAttribute(string name, int location, ActiveAttribType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public sealed class ShaderProgram : IDisposable
    {
        private bool disposed;
        private readonly ShaderUniform[] _uniforms;
        private readonly ShaderAttribute[] _attributes;


        public readonly int ShaderProgramHandle;
        public readonly int VertexShaderHandle;
        public readonly int PixelShaderHandle;


        public ShaderProgram(string vertexShaderCode, string pixelShaderCode)
        {
            disposed = false;

            if (ShaderProgram.CompileVertexShader(vertexShaderCode, out VertexShaderHandle, out var vertexShaderCompileError) == false)
            {
                throw new ArgumentException(vertexShaderCompileError);
            }

            if (ShaderProgram.CompilePixelShader(pixelShaderCode, out PixelShaderHandle, out var pixelShaderCompileError) == false)
            {
                throw new ArgumentException(pixelShaderCompileError);
            }

            ShaderProgramHandle = CreateLinkProgram(VertexShaderHandle, PixelShaderHandle);

            _uniforms = ShaderProgram.CreateUniformList(ShaderProgramHandle);
            _attributes = ShaderProgram.CreateAttributeList(ShaderProgramHandle);
        }

        ~ShaderProgram() {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(PixelShaderHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(ShaderProgramHandle);

            disposed = true;
            GC.SuppressFinalize(this);
        }

        public ShaderUniform[] GetUniformList()
        {
            var result = new ShaderUniform[_uniforms.Length];
            Array.Copy(_uniforms, result, _uniforms.Length);
            return result;
        }

        public ShaderAttribute[] GetAttributeList()
        {
            var result = new ShaderAttribute[_attributes.Length];
            Array.Copy(_attributes, result, _attributes.Length);
            return result;
        }

        public void SetUniform(string name, float v1)
        {
            if (GetShaderUniform(name, out var uniform) == false)
            {
                throw new ArgumentException("Name was not found");
            }

            if (uniform.Type != ActiveUniformType.Float)
            {
                throw new ArgumentException("Uniform type is not Float.");
            }

            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform1(uniform.Location,v1);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2)
        {
            if (GetShaderUniform(name, out var uniform) == false)
            {
                throw new ArgumentException("Name was not found");
            }

            if (uniform.Type != ActiveUniformType.FloatVec2)
            {
                throw new ArgumentException("Uniform type is not FloatVec2.");
            }

            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform2(uniform.Location, v1, v2);
            GL.UseProgram(0);
        }

        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            uniform = new ShaderUniform();
            for (int i = 0; i < _uniforms.Length; i++)
            {
                uniform = _uniforms[i];
                if (name == uniform.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CompileVertexShader(string vertexShaderCode, out int vertexShaderHandle, out string errorMessage)
        {
            return CompileShader(vertexShaderCode, ShaderType.VertexShader, out vertexShaderHandle, out errorMessage);
        }

        public static bool CompilePixelShader(string pixelShaderCode, out int pixelShaderHandle, out string errorMessage)
        {
            return CompileShader(pixelShaderCode, ShaderType.FragmentShader, out pixelShaderHandle, out errorMessage);
        }

        private static bool CompileShader(string shaderCode, ShaderType shaderType, out int shaderHandle, out string errorMessage)
        {
            errorMessage = string.Empty;
            shaderHandle = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderHandle, shaderCode);
            GL.CompileShader(shaderHandle);

            var vertexShaderInfo = GL.GetShaderInfoLog(shaderHandle);
            if (string.IsNullOrWhiteSpace(vertexShaderInfo) == false)
            {
                errorMessage = vertexShaderInfo;
                return false;
            }
            return true;
        }

        public static int CreateLinkProgram(int vertexShaderHandle, int pixelShaderHandle)
        {

            int shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(shaderProgramHandle);
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, pixelShaderHandle);

            return shaderProgramHandle;
        }

        public static ShaderUniform[] CreateUniformList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            ShaderUniform[] uniforms = new ShaderUniform[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(shaderProgramHandle, i, 256, out _, out _, out var type, out var name);
                var location = GL.GetUniformLocation(shaderProgramHandle, name);
                uniforms[i] = new ShaderUniform(name, location, type);
            }

            return uniforms;
        }

        public static ShaderAttribute[] CreateAttributeList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveAttributes, out int attributeCount);

            ShaderAttribute[] attributes = new ShaderAttribute[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(shaderProgramHandle, i, 256, out _, out _, out var type, out var name);
                var location = GL.GetUniformLocation(shaderProgramHandle, name);
                attributes[i] = new ShaderAttribute(name, location, type);
            }

            return attributes;
        }
    }
}
