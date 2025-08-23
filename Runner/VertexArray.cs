using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Runner.Buffers;

namespace Runner
{
    public sealed class VertexArray : IDisposable
    {
        public readonly BufferTarget BufferTarget = BufferTarget.ArrayBuffer;

        private bool disposed;

        public readonly int VertexArrayHandle;
        public readonly VertexBuffer VertexBuffer;


        public VertexArray(VertexBuffer vertexBuffer)
        {
            if (vertexBuffer is null)
            {
                throw new ArgumentNullException(nameof(vertexBuffer));
            }

            disposed = false;
            VertexBuffer = vertexBuffer;

            int vertexSizeInBytes = vertexBuffer.VertexInfo.SizeInBytes;

            VertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayHandle);

            GL.BindBuffer(BufferTarget, vertexBuffer.Handle);

            VertexAttribute[] attributes = VertexBuffer.VertexInfo.VertexAttributes;

            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];

                GL.VertexAttribPointer(attribute.Index, attribute.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attribute.Offset);
                GL.EnableVertexAttribArray(attribute.Index);

            }


            GL.BindVertexArray(0);
        }

        ~VertexArray()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VertexArrayHandle);

            disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
