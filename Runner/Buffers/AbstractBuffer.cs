using OpenTK.Graphics.OpenGL;

namespace Runner.Buffers
{
    public abstract class AbstractBuffer : IDisposable
    {
        public virtual BufferTarget BufferTarget { get; init; }
        public int Handle { get; }
        public bool IsStatic { get; }

        public int BufferCount { get;  }
        private int _sizeInfo;
        private bool _disposed;

        protected readonly int _minBufferCount = 1;
        protected virtual int MaxBufferCount { get; init; }

        protected AbstractBuffer(int bufferCount, int sizeInfo, bool isStatic = true)
        {
            if (bufferCount < _minBufferCount || bufferCount > MaxBufferCount)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferCount));
            }

            BufferCount = bufferCount;
            _disposed = false;
            _sizeInfo = sizeInfo;
            IsStatic = isStatic;

            var bufferUsageHint = IsStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw;

            Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget, Handle);
            GL.BufferData(BufferTarget, BufferCount * _sizeInfo, IntPtr.Zero, bufferUsageHint);
            GL.BindBuffer(BufferTarget, 0);
        }

        ~AbstractBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;

            GL.BindBuffer(BufferTarget, 0);
            GL.DeleteBuffer(Handle);

            _disposed = true;

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
            GL.BufferSubData(BufferTarget, IntPtr.Zero, count * _sizeInfo, data);
            GL.BindBuffer(BufferTarget, 0);
        }
    }
}
