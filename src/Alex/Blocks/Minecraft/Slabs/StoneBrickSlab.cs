namespace Alex.Blocks.Minecraft
{
	public class StoneBrickSlab : Slab
	{
		public StoneBrickSlab() : base()
		{
			BlockMaterial = Material.Stone.Clone().SetHardness(1.5f);
			//Hardness = 1.5f;
		}
	}
}
