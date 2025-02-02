using System;

namespace Alex.Blocks.Storage.Palette
{
	public interface IHasKey
	{
		uint Id { get; }
	}

	public class DirectPalette<TValue> : IPalette<TValue> where TValue : class, IHasKey
	{
		private readonly Func<uint, TValue> _getById;

		public DirectPalette(Func<uint, TValue> getById)
		{
			_getById = getById;
		}

		public uint GetId(TValue state)
		{
			return state.Id;
		}

		public uint Add(TValue state)
		{
			return state.Id;
		}

		public TValue Get(uint id)
		{
			return _getById(id); // BlockFactory.GetBlockState(id);
		}

		public void Put(TValue objectIn, uint intKey) { }

		/// <inheritdoc />
		public void Dispose() { }
	}
}