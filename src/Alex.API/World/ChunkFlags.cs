using System;
using System.Collections.Generic;
using System.Text;

namespace Alex.API.World
{
    [Flags]
    public enum ChunkFlags
    {
        None = 0,
        ClearRam = 1,
        ClearGPU = 2,
        InRam = 4,
        InGpu = 8,
        SendToRam =16,
        SendToGpu = 32
    }
}
