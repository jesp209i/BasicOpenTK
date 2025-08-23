using OpenTK.Graphics.OpenGL;

namespace Runner.Buffers
{
    public sealed class VertexBuffer : AbstractBuffer
    {
        public override BufferTarget BufferTarget { get; init; } = BufferTarget.ArrayBuffer;

        public override int MaxBufferCount { get; init; } = 100_000;


        public readonly VertexInfo VertexInfo;

        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true) : base(vertexCount, vertexInfo.SizeInBytes, isStatic)
        {
            VertexInfo = vertexInfo;
        }


        public override void SetData<T>(T[] data, int count) where T : struct
        {
            if (typeof(T) != VertexInfo.Type)
            {
                throw new ArgumentException("Generic Type 'T' does not match the vertext type of the vertex buffer.");
            }
            base.SetData(data, count);

        }
    }
}
