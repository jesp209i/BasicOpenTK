using OpenTK.Graphics.OpenGL;

namespace Runner.Buffers
{
    public sealed class IndexBuffer : IDisposable
    {
        public readonly BufferTarget BufferTarget = BufferTarget.ElementArrayBuffer;

        public static readonly int MinIndexCount = 1;
        public static readonly int MaxIndexCount = 250_000;
        
        private bool disposed;

        public readonly int IndexBufferHandle;
        public readonly int IndexCount;
        public readonly bool IsStatic;

        public IndexBuffer(int indexCount, bool isStatic = true)
        {
            if (indexCount < IndexBuffer.MinIndexCount || indexCount > IndexBuffer.MaxIndexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            }
            disposed = false;
            IndexCount = indexCount;
            IsStatic = isStatic;

            BufferUsageHint hint = IsStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw;


            IndexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget, IndexBufferHandle);
            GL.BufferData(BufferTarget, IndexCount * sizeof(int), IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget, 0);

        }

        ~IndexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.BindBuffer(BufferTarget, 0);
            GL.DeleteBuffer(IndexBufferHandle);

            disposed = true;

            GC.SuppressFinalize(this);
        }


        public void SetData(int[] data, int count)
        {

            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(data));
            if (count <= 0 ||
                count > IndexCount ||
                count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            GL.BindBuffer(BufferTarget, IndexBufferHandle);
            GL.BufferSubData(BufferTarget, IntPtr.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget, 0);
        }
    }
}
