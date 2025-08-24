using OpenTK.Graphics.OpenGL;

namespace Runner.Common
{
    public sealed class Shader : IDisposable
    {
        private bool disposed;
        public ShaderUniform[] Uniforms { get; private set; }
        public ShaderAttribute[] Attributes { get; private set; }

        public readonly int Handle;

        public Shader(string vertexShaderPath, string fragmentShaderPath)
        {
            disposed = false;

            var vertexShaderCode = File.ReadAllText(vertexShaderPath);

            if (this.CompileVertexShader(vertexShaderCode, out var vertexShaderHandle, out var vertexShaderCompileError) == false)
            {
                throw new ArgumentException(vertexShaderCompileError);
            }

            var fragmentShaderCode = File.ReadAllText(fragmentShaderPath);
            if (this.CompilePixelShader(fragmentShaderCode, out var fragmentShaderHandle, out var fragmentShaderCompileError) == false)
            {
                throw new ArgumentException(fragmentShaderCompileError);
            }

            Handle = this.LinkProgram([vertexShaderHandle, fragmentShaderHandle]);

            CreateUniformList();
            CreateAttributeList();
        }

        ~Shader() {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.UseProgram(0);
            GL.DeleteProgram(Handle);

            disposed = true;
            GC.SuppressFinalize(this);
        }


        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            uniform = new ShaderUniform();
            for (int i = 0; i < Uniforms.Length; i++)
            {
                uniform = Uniforms[i];
                if (name == uniform.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }


        public void CreateUniformList()
        {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);

            ShaderUniform[] uniforms = new ShaderUniform[numberOfUniforms];

            for (int i = 0; i < numberOfUniforms; i++)
            {
                GL.GetActiveUniform(Handle, i, 256, out _, out _, out var type, out var name);
                var location = GL.GetUniformLocation(Handle, name);
                uniforms[i] = new ShaderUniform(name, location, type);
            }

            Uniforms = uniforms;
        }

        public void CreateAttributeList()
        {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int numberOfAttributes);

            ShaderAttribute[] attributes = new ShaderAttribute[numberOfAttributes];

            for (int i = 0; i < numberOfAttributes; i++)
            {
                GL.GetActiveAttrib(Handle, i, 256, out _, out _, out var type, out var name);
                var location = GL.GetAttribLocation(Handle, name);
                attributes[i] = new ShaderAttribute(name, location, type);
            }

            Attributes = attributes;
        }

        public void SetUniform(string name, float v1)
            => InternalSet(name, ActiveUniformType.Float, (uniformLocation) => GL.Uniform1(uniformLocation, v1));
        

        public void SetUniform(string name, float v1, float v2)
            => InternalSet(name, ActiveUniformType.FloatVec2, (uniformLocation) => GL.Uniform2(uniformLocation, v1, v2));
        

        private void InternalSet(string name, ActiveUniformType uniformType, Action<int> setAction)
        {
            if (GetShaderUniform(name, out var uniform) == false)
            {
                throw new ArgumentException("Name was not found");
            }

            if (uniform.Type != uniformType)
            {
                throw new ArgumentException($"Uniform type is not {uniformType}.");
            }

            GL.UseProgram(Handle);
            setAction.Invoke(uniform.Location);
            GL.UseProgram(0);
        }
    }
}
