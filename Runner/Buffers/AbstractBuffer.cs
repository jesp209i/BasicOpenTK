using OpenTK.Graphics.OpenGL;

namespace Runner.Buffers
{
    public abstract class AbstractBuffer : IDisposable
    {
        public virtual BufferTarget BufferTarget { get; init; }

        public readonly int MinBufferCount = 1;
        public virtual int MaxBufferCount { get; init; }

        public int Handle { get; }
        public int BufferCount { get; }
        public int SizeInfo { get; }
        public bool IsStatic { get; }

        protected bool disposed;


        protected AbstractBuffer(int bufferCount, int sizeInfo, bool isStatic = true)
        {
            if (bufferCount < MinBufferCount || bufferCount > MaxBufferCount)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferCount));
            }

            BufferCount = bufferCount;
            disposed = false;
            SizeInfo = sizeInfo;
            IsStatic = isStatic;

            var bufferUsageHint = IsStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw;

            Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget, Handle);
            GL.BufferData(BufferTarget, BufferCount * SizeInfo, IntPtr.Zero, bufferUsageHint);
            GL.BindBuffer(BufferTarget, 0);
        }

        ~AbstractBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.BindBuffer(BufferTarget, 0);
            GL.DeleteBuffer(Handle);

            disposed = true;

            GC.SuppressFinalize(this);
        }

        public virtual void SetData<T>(T[] data, int count) where T : struct
        {

            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(data));
            if (count <= 0 ||
                count > BufferCount ||
                count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            GL.BindBuffer(BufferTarget, Handle);
            GL.BufferSubData(BufferTarget, IntPtr.Zero, count * SizeInfo, data);
            GL.BindBuffer(BufferTarget, 0);
        }
    }
}
