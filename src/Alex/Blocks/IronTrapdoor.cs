using System.Net;
using Alex.API.Utils;
using Alex.API.World;
using Alex.Blocks.Properties;
using Alex.Entities;
using Alex.ResourcePackLib.Json;
using Alex.Utils;
using Alex.Worlds;

namespace Alex.Blocks
{
	public class IronTrapdoor : Block
	{
		public IronTrapdoor() : base(6419)
		{
			Solid = true;
			Transparent = true;
			IsReplacible = false;
		}
	}

	public class Trapdoor : Block
	{
		private static PropertyBool OPEN = new PropertyBool("open");
		private static PropertyBool HALF = new PropertyBool("half", "top", "bottom");
		private static PropertyFace FACING = new PropertyFace("facing");

		public Trapdoor(string name) : base(name)
		{
			Solid = true;
			IsFullCube = false;
		}

		public override void Interact(IWorld world, BlockCoordinates position, BlockFace face, Entity sourceEntity)
		{
			if (BlockState.GetTypedValue(OPEN))
			{
				world.SetBlockState(position.X, position.Y, position.Z, BlockState.WithProperty(OPEN, false));
			}
			else
			{
				world.SetBlockState(position.X, position.Y, position.Z, BlockState.WithProperty(OPEN, true));
			}
		}
	}
}
