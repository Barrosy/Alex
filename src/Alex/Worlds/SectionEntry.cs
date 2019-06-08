using System;
using Microsoft.Xna.Framework.Graphics;

namespace Alex.Worlds
{

    public partial class ChunkColumn
    {
        private class SectionEntry : IDisposable
	    {
	        public IndexBuffer SolidIndexBuffer { get; set; }
	        public IndexBuffer TransparentIndexBuffer { get; set; }

	        public VertexBuffer SolidBuffer { get; set; }
	        public VertexBuffer TransparentBuffer { get; set; }

	        public object _lock = new object();

	        public void Dispose()
	        {
	            lock (_lock)
	            {
	                SolidIndexBuffer?.Dispose();
	                TransparentIndexBuffer?.Dispose();
	                SolidBuffer?.Dispose();
	                TransparentBuffer?.Dispose();
	            }
	        }
	    }
	}
}
