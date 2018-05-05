﻿using fNbt.Tags;

namespace Alex.API.Data
{
    public class SlotData
    {
	    public short ItemID = -1;
	    public short ItemDamage = 0;
	    public byte Count = 0;

		public NbtCompound Nbt = null;

		public SlotData()
	    {
		    
	    }
    }
}
