﻿using System;
using System.Collections.Generic;
using Alex.CoreRT.API.Graphics;
using Alex.CoreRT.API.World;
using Alex.CoreRT.Blocks;
using Alex.CoreRT.Utils;
using Alex.CoreRT.Worlds;
using log4net;
using Microsoft.Xna.Framework;
using MiNET.Utils;
using MiNET.Worlds;
using ResourcePackLib.CoreRT.Json;
using ResourcePackLib.CoreRT.Json.BlockStates;
using ResourcePackLib.CoreRT.Json.Models;
using Axis = ResourcePackLib.CoreRT.Json.Axis;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;
using V3 = Microsoft.Xna.Framework.Vector3;

namespace Alex.CoreRT.Graphics.Models
{
    public class ResourcePackModel : BlockModel
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ResourcePackModel));
		
        protected BlockStateModel Variant { get; }
		protected ResourceManager Resources { get; }

		public ResourcePackModel(ResourceManager resources, BlockStateModel variant)
		{
			Resources = resources;
            Variant = variant;

			CalculateBoundingBox();
        }

		private void CalculateBoundingBox()
		{
			foreach (var element in Variant.Model.Elements)
			{
				var faceStart = new V3((element.From.X), (element.From.Y),
					(element.From.Z)) / 16f;

				var faceEnd = new V3((element.To.X), (element.To.Y),
					(element.To.Z)) / 16f;

				if (faceEnd.X > Max.X)
				{
					Max.X = faceEnd.X;
				}

				if (faceEnd.Y > Max.Y)
				{
					Max.Y = faceEnd.Y;
				}

				if (faceEnd.Z > Max.Z)
				{
					Max.Z = faceEnd.Z;
				}

				if (faceStart.X < Min.X)
				{
					Min.X = faceStart.X;
				}

				if (faceStart.Y < Min.Y)
				{
					Min.Y = faceStart.Y;
				}

				if (faceStart.Z < Min.Z)
				{
					Min.Z = faceStart.Z;
				}
			}
		}

		protected Matrix GetElementRotationMatrix(BlockModelElementRotation elementRotation, out float rescale)
		{
			Matrix faceRotationMatrix = Matrix.Identity;
			float ci = 0f;
			
			if (elementRotation.Axis != Axis.Undefined)
			{
				var elementRotationOrigin = new Vector3(elementRotation.Origin.X, elementRotation.Origin.Y, elementRotation.Origin.Z);

				var elementAngle =
					MathUtils.ToRadians((float)(elementRotation.Axis == Axis.X ? -elementRotation.Angle : elementRotation.Angle));
				elementAngle = elementRotation.Axis == Axis.Z ? elementAngle : -elementAngle;
				ci = 1f / (float)Math.Cos(elementAngle);

				faceRotationMatrix = Matrix.CreateTranslation(-elementRotationOrigin);
				if (elementRotation.Axis == Axis.X)
				{
					faceRotationMatrix *= Matrix.CreateRotationX(elementAngle);
				}
				else if (elementRotation.Axis == Axis.Y)
				{
					faceRotationMatrix *= Matrix.CreateRotationY(elementAngle);
				}
				else if (elementRotation.Axis == Axis.Z)
				{
					faceRotationMatrix *= Matrix.CreateRotationZ(elementAngle);
				}

				faceRotationMatrix *= Matrix.CreateTranslation(elementRotationOrigin);
			}

			rescale = ci;
			return faceRotationMatrix;
		}

		protected void GetFaceValues(string facename, BlockFace originalFace, out BlockFace face, out V3 offset)
		{
			V3 cullFace = V3.Zero;

			BlockFace cull;
			if (!Enum.TryParse(facename, out cull))
			{
				cull = originalFace;
			}
			switch (cull)
			{
				case BlockFace.Up:
					cullFace = V3.Up;
					break;
				case BlockFace.Down:
					cullFace = V3.Down;
					break;
				case BlockFace.North:
					cullFace = V3.Backward;
					break;
				case BlockFace.South:
					cullFace = V3.Forward;
					break;
				case BlockFace.West:
					cullFace = V3.Left;
					break;
				case BlockFace.East:
					cullFace = V3.Right;
					break;
			}

			offset = cullFace;
			face = cull;
		}

		protected Matrix GetModelRotationMatrix()
		{
			return Matrix.CreateRotationX((float)MathUtils.ToRadians(360f - Variant.X)) *
			       Matrix.CreateRotationY((float)MathUtils.ToRadians(360f - Variant.Y));
		}

		private V3 Min = V3.Zero;
		private V3 Max = V3.One / 16f;
		public override VertexPositionNormalTextureColor[] GetVertices(IWorld world, V3 position, Block baseBlock)
        {
	        var verts = new List<VertexPositionNormalTextureColor>();

	        var modelRotationMatrix = GetModelRotationMatrix();

			// MaxY = 0;
			V3 worldPosition = new V3(position.X, position.Y, position.Z);

			foreach (var element in Variant.Model.Elements)
            {
	            var c = new V3(8f, 8f, 8f);

				var elementFrom = new V3((element.From.X), (element.From.Y),
		           (element.From.Z));

	            var elementTo = new V3((element.To.X), (element.To.Y) ,
		            (element.To.Z));

				var elementModelRotation = Matrix.CreateTranslation(-c) * modelRotationMatrix *
	                            Matrix.CreateTranslation(c);

				foreach (var face in element.Faces)
				{
					var faceStart = elementFrom;
					var faceEnd = elementTo;

					string textureName = "no_texture";
					if (!Variant.Model.Textures.TryGetValue(face.Value.Texture.Replace("#", ""), out textureName))
					{
						textureName = face.Value.Texture;
					}

					if (textureName.StartsWith("#"))
					{
						if (!Variant.Model.Textures.TryGetValue(textureName.Replace("#", ""), out textureName))
						{
							textureName = "no_texture";
						}
					}

					var uv = face.Value.UV;
					var uvmap = GetTextureUVMap(Resources, textureName, uv.X1, uv.X2, uv.Y1, uv.Y2);

					GetFaceValues(face.Value.CullFace, face.Key, out var cull, out var cullFace);

					cullFace = V3.Transform(cullFace, modelRotationMatrix);

					if (cullFace != V3.Zero && !CanRender(world, baseBlock, worldPosition + cullFace))
						continue;

					var elementRotation = element.Rotation;
					Matrix faceRotationMatrix = GetElementRotationMatrix(elementRotation, out float ci);

					VertexPositionNormalTextureColor[] faceVertices = GetFaceVertices(face.Key, faceStart, faceEnd, uvmap, face.Value.Rotation);

					Color faceColor = faceVertices[0].Color;

					if (face.Value.TintIndex >= 0)
					{
						World w = (World)world;

						if (w.RenderingManager.TryGetChunk(
							new ChunkCoordinates(new PlayerLocation(worldPosition.X, 0, worldPosition.Z)),
							out IChunkColumn column))
						{
							Worlds.ChunkColumn realColumn = (Worlds.ChunkColumn)column;
							var biome = BiomeUtils.GetBiomeById(realColumn.GetBiome((int)worldPosition.X & 0xf, (int)worldPosition.Z & 0xf));

							if (baseBlock.BlockId == 2)
							{
								faceColor = Resources.ResourcePack.GetGrassColor(biome.Temperature, biome.Downfall, (int)worldPosition.Y);
							}
							else
							{
								faceColor = Resources.ResourcePack.GetFoliageColor(biome.Temperature, biome.Downfall, (int)worldPosition.Y);
							}
						}
					}

					faceColor = UvMapHelp.AdjustColor(faceColor, cull, GetLight(world, worldPosition + cullFace), element.Shade);

					for (var index = 0; index < faceVertices.Length; index++)
					{
						var vert = faceVertices[index];
						vert.Color = faceColor;

						if (elementRotation.Axis != Axis.Undefined)
						{
							vert.Position = V3.Transform(vert.Position, faceRotationMatrix);

							//if (elementRotation.Rescale)
							{
								if (elementRotation.Axis == Axis.X || elementRotation.Axis == Axis.Z)
								{
									vert.Position.Y *= ci;
								}

								if (elementRotation.Axis == Axis.Y || elementRotation.Axis == Axis.Z)
								{
									vert.Position.X *= ci;
								}

								if (elementRotation.Axis == Axis.Y || elementRotation.Axis == Axis.X)
								{
									vert.Position.Z *= ci;
								}
							}						
						}
						
						vert.Position = V3.Transform(vert.Position, elementModelRotation);

						vert.Position = worldPosition + (vert.Position / 16f);

						verts.Add(vert);
					}
				}
			}

	        return verts.ToArray();
        }

		public override BoundingBox GetBoundingBox(V3 position, Block requestingBlock)
		{
			return new BoundingBox(position + Min, position + Max);
		}
	}

	public static class VectorExtension {
		public static V3 From(V3 x, V3 y, V3 z)
		{
			return new V3(x.X, y.Y, z.Z);
		}
	}
}
