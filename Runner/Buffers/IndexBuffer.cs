using OpenTK.Graphics.OpenGL;

namespace Runner.Buffers
{
    public sealed class IndexBuffer : AbstractBuffer
    {
        public override BufferTarget BufferTarget { get; init; } = BufferTarget.ElementArrayBuffer;        
        protected override int MaxBufferCount { get; init; } = 250_000;

        public IndexBuffer(int indexCount, bool isStatic = true) : base(indexCount, sizeof(int), isStatic)
        {
        }
    }
}
