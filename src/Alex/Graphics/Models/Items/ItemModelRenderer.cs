﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Alex.API.Entities;
using Alex.API.Graphics;
using Alex.API.Utils;
using Alex.ResourcePackLib;
using Alex.ResourcePackLib.Json;
using Alex.ResourcePackLib.Json.Models.Items;
using Alex.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Alex.Graphics.Models.Items
{
    public class ItemModelRenderer : Model, IAttachable
    {
	    public ResourcePackItem Model { get; }
	    public VertexPositionColor[] Vertices { get; set; } = null;
		public short[] Indexes { get; set; } = null;

		private BasicEffect Effect { get; set; } = null;

		public Vector3 Rotation { get; set; } = Vector3.Zero;
		public Vector3 Translation { get; set; }= Vector3.Zero;
		public Vector3 Scale { get; set; }= Vector3.Zero;
		
		public Vector3 Pivot    { get; private set; } = Vector3.Zero;
		public Vector3 Origin   { get; private set; } = Vector3.Zero;
		public bool ApplyPitch   { get; set; } = true;
		public bool ApplyYaw     { get; set; } = true;
		public bool ApplyHeadYaw { get; set; } = false;
		
		public ItemModelRenderer(ResourcePackItem model, McResourcePack resourcePack)
		{
			Model = model;
			Cache(resourcePack);
		}

		private Matrix ParentMatrix = Matrix.Identity;
		public void Update(Matrix parentMatrix)
		{
			ParentMatrix = parentMatrix;
		}

		public void Render(IRenderArgs args)
		{
			Render(args.GraphicsDevice);
		}
		
		public void Update(IUpdateArgs args, PlayerLocation knownPosition)
		{
			if (Effect == null)
			{
				Effect = new BasicEffect(args.GraphicsDevice);
				Effect.VertexColorEnabled = true;
			}

			Effect.Projection = args.Camera.ProjectionMatrix;
			Effect.View = args.Camera.ViewMatrix;

			var scale = Scale * 16f;
			//scale *= new Vector3(4, 12, 4);
			Vector3 origin = new Vector3(0.4f, 0.8f, -0.125f); // *16 = {X: 6.4, Y: 12.8, Z: 2}
		
			/*var pieceMatrix =
				Matrix.CreateTranslation(origin) *
				Matrix.CreateScale(scale * 16f) *
				//Matrix.CreateTranslation(4f, 0f, 1f) *0f

				Matrix.CreateTranslation(Translation.X, Translation.Y, Translation.Z) *
				Matrix.CreateRotationZ(MathUtils.ToRadians(-Rotation.Z)) *
				Matrix.CreateRotationY(MathUtils.ToRadians(-Rotation.Y));
				//Matrix.CreateTranslation(Translation.X, Translation.Y, Translation.Z);
				//Matrix.CreateTranslation(origin);*/
			//var pieceMatrix = Matrix.CreateTranslation(origin) *
			                //  Matrix.CreateRotationZ(MathUtils.ToRadians(-Rotation.Z)) *
			               //   Matrix.CreateRotationY(MathUtils.ToRadians(-Rotation.Y)) * Matrix.CreateScale(scale * 16f);
			//	Effect.World = pieceMatrix * ParentMatrix;
			/*Effect.World = (
				               Matrix.CreateTranslation(-origin) *
				               Matrix.CreateScale(scale) *
				               Matrix.CreateRotationZ(MathUtils.ToRadians(-Rotation.Z)) *
				               Matrix.CreateRotationY(MathUtils.ToRadians(-Rotation.Y)) *
				               Matrix.CreateTranslation(origin) *
				               Matrix.CreateTranslation(Translation)
			               ) * ParentMatrix;*/
			
			Debug.WriteLine("ItemRenderer: (Origin: {0}, Scale: {1}, Rotation: {2}, Translation: {3})", origin.ToString(), scale, Rotation.ToString(), Translation.ToString());
			
//			var offset = new Vector3(4f, -4f, 2f );
//			var offset = new Vector3(Translation.X, Translation.Y, Translation.Z );
//			// Basically working
//			Effect.World = (
//							     Matrix.CreateScale(scale) 
////							   * Matrix.CreateTranslation(-(origin * 16f))
//								 * Matrix.CreateTranslation(-offset)
//								* Matrix.CreateRotationZ(MathUtils.ToRadians(90f-Rotation.Z + knownPosition.Pitch)) 
//								* Matrix.CreateRotationY(MathUtils.ToRadians(-Rotation.Y - (knownPosition.HeadYaw - knownPosition.Yaw))) 
//							   //* Matrix.CreateTranslation(origin * scale) 
////								* Matrix.CreateTranslation(-(origin * 16f) + Translation)
//								* Matrix.CreateTranslation(offset)
//			               ) 
//						   * ParentMatrix
////						   * Matrix.CreateTranslation(knownPosition)
//				;

			origin = Vector3.Zero;
			var pivot = new Vector3(1.5f, 2f, 2f);
			var rot = Rotation;
			var pos = new Vector3(Translation.X, Translation.Y + 12f, Translation.Z);
			
			Matrix rotMatrix = Matrix.CreateTranslation(-pivot) 
							   * Matrix.CreateFromYawPitchRoll(
															   MathUtils.ToRadians(rot.Y), 
															   MathUtils.ToRadians(180f - rot.X),
															   MathUtils.ToRadians(90f+(180f+rot.Z))
															  )  
//* Matrix.CreateRotationY(MathUtils.ToRadians((180f - knownPosition.HeadYaw) - (180f - knownPosition.Yaw) - rot.Y))
//* Matrix.CreateRotationZ(MathUtils.ToRadians(rot.Z))
							   * Matrix.CreateTranslation(pivot);
			
			var rotMatrix2 = Matrix.CreateTranslation(-pivot)
//							 * Matrix.CreateFromYawPitchRoll(
//															 MathUtils.ToRadians(180f - knownPosition.HeadYaw), 
//															 90f,
//															 0f)
//							 * Matrix.CreateRotationY(-MathUtils.ToRadians(180f - knownPosition.Yaw))
							 //* Matrix.CreateFromAxisAngle(Vector3.Up, MathUtils.ToRadians(180f - knownPosition.HeadYaw))
//							 * Matrix.CreateRotationX(MathUtils.ToRadians(180f - rot.Z))
//							 * Matrix.CreateRotationY(MathUtils.ToRadians(rot.X))
//							 * Matrix.CreateRotationZ(MathUtils.ToRadians(90f-rot.Y))
//							 * Matrix.CreateRotationX(MathUtils.ToRadians(rot.Z))
							 * Matrix.CreateTranslation(pivot);
			
			var rotateMatrix = Matrix.CreateScale(scale) 
							   * rotMatrix 
							   * Matrix.CreateTranslation(pos);

			Effect.World = rotateMatrix * ParentMatrix;
			//Effect.World = ParentMatrix;
		}

		private void DrawLine(GraphicsDevice device, Vector3 start, Vector3 end, Color color)
		{
			var vertices = new[] { new VertexPositionColor(start, color),  new VertexPositionColor(end, color) };
			device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
		}
		
		public void Render(GraphicsDevice device)
		{
			if (Effect == null || Vertices == null || Vertices.Length == 0)
				return;
			
			foreach (var a in Effect.CurrentTechnique.Passes)
			{
				a.Apply();

				device.DrawTriad();
				
				device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indexes, 0, Indexes.Length / 3);
			}
		}
		
	    private void Cache(McResourcePack pack)
	    {
		    var t = Model.Textures.FirstOrDefault(x => x.Value != null);

		    if (t.Value == default) return;
		    
		    List<VertexPositionColor> vertices = new List<VertexPositionColor>(); 
		    List<short> indexes = new List<short>();
		    
		    if (pack.TryGetBitmap(t.Value, out Bitmap texture))
		    {
			    int i = 0;
			    float toolPosX = 0.0f;
			    float toolPosY = 0.0f;
			    float toolPosZ = 0.0f;
			    int verticesPerTool = texture.Width * texture.Height * 36;
			    
				    
			    for (int y = 0; y < texture.Height; y++)
			    {
				    for (int x = 0; x < texture.Width; x++)
				    {
					    var pixel = texture.GetPixel(x, y);
					    if (pixel.A == 0)
					    {
						    continue;
					    }

					    Color color = new Color(pixel.R, pixel.G, pixel.B, pixel.A);

					    ItemModelCube built = new ItemModelCube(new Vector3(1f / texture.Width));
					    built.BuildCube(color);

					    var origin = new Vector3(toolPosX + (1f / texture.Width) * x, toolPosY + 1 - (1f / texture.Height) * y, toolPosZ);
						
					    vertices = ModifyCubeIndexes(vertices, ref built.Front, origin);
					    vertices = ModifyCubeIndexes(vertices, ref built.Back, origin);
					    vertices = ModifyCubeIndexes(vertices, ref built.Top, origin);
					    vertices = ModifyCubeIndexes(vertices, ref built.Bottom, origin);
					    vertices = ModifyCubeIndexes(vertices, ref built.Left, origin);
					    vertices = ModifyCubeIndexes(vertices, ref built.Right, origin);

					    var indices = built.Front.indexes
						    .Concat(built.Back.indexes)
						    .Concat(built.Top.indexes)
						    .Concat(built.Bottom.indexes)
						    .Concat(built.Left.indexes)
						    .Concat(built.Right.indexes)
						    .ToArray();

					    indexes.AddRange(indices);
				    }
			    }
		    }

		    Vertices = vertices.ToArray();

		    for (var index = 0; index < Vertices.Length; index++)
		    {
			    var vertice = Vertices[index];
			    Vertices[index] = vertice;
		    }

		    Indexes = indexes.ToArray();
	    }
	    
	    private List<VertexPositionColor> ModifyCubeIndexes(List<VertexPositionColor> vertices,
		    ref (VertexPositionColor[] vertices, short[] indexes) data, Vector3 offset)
	    {
		    var startIndex = (short)vertices.Count;
		    foreach (var vertice in data.vertices)
		    {
			    var vertex = vertice;
			    vertex.Position += offset;
			    vertices.Add(vertex);
		    }
			
		    //vertices.AddRange(data.vertices);
			
		    for (int i = 0; i < data.indexes.Length; i++)
		    {
			    data.indexes[i] += startIndex;
		    }

		    return vertices;
	    }
    }

    public sealed class ItemModelCube
    {
	    public Vector3 Size;
	    
	    public bool Mirrored { get; set; } = false;

	    public ItemModelCube(Vector3 size)
	    {
		    this.Size = size;

		    //front verts with position and texture stuff
		    _topLeftFront = new Vector3(0.0f, 1.0f, 0.0f) * Size;
		    _topLeftBack = new Vector3(0.0f, 1.0f, 1.0f) * Size;
		    _topRightFront = new Vector3(1.0f, 1.0f, 0.0f) * Size;
		    _topRightBack = new Vector3(1.0f, 1.0f, 1.0f) * Size;

		    // Calculate the position of the vertices on the bottom face.
		    _btmLeftFront = new Vector3(0.0f, 0.0f, 0.0f) * Size;
		    _btmLeftBack = new Vector3(0.0f, 0.0f, 1.0f) * Size;
		    _btmRightFront = new Vector3(1.0f, 0.0f, 0.0f) * Size;
		    _btmRightBack = new Vector3(1.0f, 0.0f, 1.0f) * Size;
	    }

	    public (VertexPositionColor[] vertices, short[] indexes) Front, Back, Left, Right, Top, Bottom;

	    private readonly Vector3 _topLeftFront;
	    private readonly Vector3 _topLeftBack;
	    private readonly Vector3 _topRightFront;
	    private readonly Vector3 _topRightBack;
	    private readonly Vector3 _btmLeftFront;
	    private readonly Vector3 _btmLeftBack;
	    private readonly Vector3 _btmRightFront;
	    private readonly Vector3 _btmRightBack;

	    public void BuildCube(Color uv)
	    {
		    Front = GetFrontVertex(uv);
		    Back = GetBackVertex(uv);
		    Left = GetLeftVertex(uv);
		    Right = GetRightVertex(uv);
		    Top = GetTopVertex(uv);
		    Bottom = GetBottomVertex(uv);
	    }
	    
	    private (VertexPositionColor[] vertices, short[] indexes) GetLeftVertex(Color color)
	    {
		    // Add the vertices for the RIGHT face. 
		    return (new VertexPositionColor[]
		    {
			    new VertexPositionColor(_topLeftFront, color),
			    new VertexPositionColor(_btmLeftFront, color),
			    new VertexPositionColor(_btmLeftBack, color),
			    new VertexPositionColor(_topLeftBack, color),
			    //new VertexPositionNormalTexture(_topLeftFront , normal, map.TopLeft),
			    //new VertexPositionNormalTexture(_btmLeftBack, normal, map.BotRight),
		    }, new short[]
		    {
			    0, 1, 2,
			    3, 0, 2
			    //0, 1, 2, 3, 0, 2
		    });
	    }

	    private (VertexPositionColor[] vertices, short[] indexes) GetRightVertex(Color color)
	    {
		    // Add the vertices for the RIGHT face. 
		    return (new VertexPositionColor[]
		    {
			    new VertexPositionColor(_topRightFront, color),
			    new VertexPositionColor(_btmRightBack, color),
			    new VertexPositionColor(_btmRightFront, color),
			    new VertexPositionColor(_topRightBack, color),
			    //new VertexPositionNormalTexture(_btmRightBack , normal, map.BotLeft),
			    //new VertexPositionNormalTexture(_topRightFront, normal, map.TopRight),
		    }, new short[]
		    {
			    0, 1, 2,
			    3, 1, 0
		    });
	    }

	    private (VertexPositionColor[] vertices, short[] indexes) GetFrontVertex(Color color)
	    {
		    // Add the vertices for the RIGHT face. 
		    return (new VertexPositionColor[]
		    {
			    new VertexPositionColor(_topLeftFront, color),
			    new VertexPositionColor(_topRightFront, color),
			    new VertexPositionColor(_btmLeftFront, color),
			    //new VertexPositionNormalTexture(_btmLeftFront , color),
			    //new VertexPositionNormalTexture(_topRightFront, color),
			    new VertexPositionColor(_btmRightFront, color),
		    }, new short[]
		    {
			    0, 1, 2,
			    2, 1, 3
			    //0, 2, 1, 2, 3, 1
		    });
	    }

	    private (VertexPositionColor[] vertices, short[] indexes) GetBackVertex(Color color)
	    {
		    // Add the vertices for the RIGHT face. 
		    return (new VertexPositionColor[]
		    {
			    new VertexPositionColor(_topLeftBack, color),
			    new VertexPositionColor(_btmLeftBack, color),
			    new VertexPositionColor(_topRightBack, color),
			    //new VertexPositionNormalTexture(_btmLeftBack , color),
			    new VertexPositionColor(_btmRightBack, color),
			    //new VertexPositionNormalTexture(_topRightBack, color),
		    }, new short[]
		    {
			    0, 1, 2,
			    1, 3, 2
			    //0, 1, 2, 1, 3, 2
		    });
	    }

	    private (VertexPositionColor[] vertices, short[] indexes) GetTopVertex(Color color)
	    {
		    // Add the vertices for the RIGHT face. 
		    return (new VertexPositionColor[]
		    {
			    new VertexPositionColor(_topLeftFront, color),
			    new VertexPositionColor(_topLeftBack, color),
			    new VertexPositionColor(_topRightBack, color),
			    //new VertexPositionNormalTexture(_topLeftFront , color),
			    //	new VertexPositionNormalTexture(_topRightBack , color),
			    new VertexPositionColor(_topRightFront, color),
		    }, new short[]
		    {
			    0, 1, 2,
			    0, 2, 3
		    });
	    }

	    private (VertexPositionColor[] vertices, short[] indexes) GetBottomVertex(Color color)
	    {
		    // Add the vertices for the RIGHT face. 
		    return (new VertexPositionColor[]
		    {
			    new VertexPositionColor(_btmLeftFront, color),
			    new VertexPositionColor(_btmRightBack, color),
			    new VertexPositionColor(_btmLeftBack, color),
			    //new VertexPositionNormalTexture(_btmLeftFront , color),
			    new VertexPositionColor(_btmRightFront, color),
			    //new VertexPositionNormalTexture(_btmRightBack , color),
		    }, new short[]
		    {
			    0, 1, 2,
			    0, 3, 1
		    });
	    }
    }
}
