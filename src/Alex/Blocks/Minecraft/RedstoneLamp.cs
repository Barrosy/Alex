namespace Alex.Blocks.Minecraft
{
	public class RedstoneLamp : Block
	{
		public RedstoneLamp() : base()
		{
			Solid = true;
			Transparent = false;

			BlockMaterial = Material.RedstoneLight;
		}

		public override byte LightValue
		{
			get
			{
				if (BlockState.TryGetValue("lit", out string lit))
				{
					if (lit == "true")
					{
						return 15;
					}
				}

				return 0;
			}
			set
			{
				
			}
		}
	}
}
