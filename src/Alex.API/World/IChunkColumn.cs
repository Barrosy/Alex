﻿using System;
using System.Threading.Tasks;
using Alex.API.Blocks.State;
using Alex.API.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Alex.API.World
{
	public interface IChunkColumn : IDisposable
	{
		int X { get; }
		int Z { get; }

        ChunkFlags Flags { get; set; }
       // bool MarkedForGpu { get; set; }
        bool HighPriority { get; set; }
     //   bool NeedCleanup { get; set; }

        IBlockState GetBlockState(int x, int y, int z);
		void SetBlockState(int x, int y, int z, IBlockState state);
		IBlock GetBlock(int bx, int by, int bz);
		void SetBlock(int bx, int by, int bz, IBlock block);
		void SetHeight(int bx, int bz, short h);
		byte GetHeight(int bx, int bz);
		void SetBiome(int bx, int bz, int biome);
		int GetBiome(int bx, int bz);
		byte GetBlocklight(int bx, int by, int bz);
		void SetBlocklight(int bx, int by, int bz, byte data);
		byte GetSkylight(int bx, int by, int bz);
		void SetSkyLight(int bx, int by, int bz, byte data);
		Task<ChunkMesh> GenerateMeshes(IWorld world);
		void UpdateChunk(GraphicsDevice device, IWorld world);
	    void SendToGpu(GraphicsDevice device);

        object VertexLock { get; set; }
		object UpdateLock { get; set; }
		bool IsDirty { get; set; }
		bool SkyLightDirty { get; set; }
		ScheduleType Scheduled { get; set; }
		int GetHeighest();
		//void SetBlockState(int x, int y, int z, IBlockState blockState);
		bool IsTransparent(int x, int y, int z);
		bool IsSolid(int bx, int by, int bz);

		void GetBlockData(int bx, int by, int bz, out bool transparent, out bool solid);
		void ScheduleBlockUpdate(int x, int y, int z);
		void DrawOpaque(GraphicsDevice device, BasicEffect effect, out int drawnIndices, out int indexSize);
		void DrawTransparent(GraphicsDevice device, AlphaTestEffect effect, out int drawnIndices, out int indexSize);
		
		bool HasDirtySubChunks { get; }
	 //   bool RamReady { get; }
	    bool IsWallSectionSolid(int face, int y);

	    void HandleFlags();

	}

	[Flags]
	public enum ScheduleType
	{
		Unscheduled = 1,
		Full = 2,
		Border = 4,
		Scheduled = 8,
		Lighting = 16
	}
}