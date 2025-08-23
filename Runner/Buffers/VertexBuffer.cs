using OpenTK.Graphics.OpenGL;

namespace Runner.Buffers
{
    public sealed class VertexBuffer : IDisposable
    {
        public readonly BufferTarget BufferTarget = BufferTarget.ArrayBuffer;

        public static readonly int MinVertextCount = 1;
        public static readonly int MaxVertextCount = 100_000;

        private bool disposed;

        public readonly int VertextBufferHandle;
        public readonly VertexInfo VertexInfo;
        public readonly int VertexCount;
        public readonly bool IsStatic;

        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true)
        {
            disposed = false;
            if (vertexCount < MinVertextCount || vertexCount > MaxVertextCount)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount));
            }

            VertexInfo = vertexInfo;
            VertexCount = vertexCount;
            IsStatic = isStatic;

            BufferUsageHint hint = IsStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw;

            VertextBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertextBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, nint.Zero, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        ~VertexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertextBufferHandle);

            disposed = true;

            GC.SuppressFinalize(this);
        }

        public void SetData<T>(T[] data, int count) where T : struct
        {
            if (typeof(T) != VertexInfo.Type)
            {
                throw new ArgumentException("Generic Type 'T' does not match the vertext type of the vertex buffer.");
            }
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(data));
            if (count <= 0 || 
                count > VertexCount || 
                count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            GL.BindBuffer(BufferTarget, VertextBufferHandle);
            GL.BufferSubData(BufferTarget, IntPtr.Zero, count * VertexInfo.SizeInBytes, data);
            GL.BindBuffer(BufferTarget, 0);
        }
    }
}
