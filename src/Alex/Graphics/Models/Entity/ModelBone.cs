﻿using System;
using System.Collections.Generic;
using System.Linq;
using Alex.API.Entities;
using Alex.API.Graphics;
using Alex.API.Utils;
using Alex.ResourcePackLib.Json.Models.Entities;
using Alex.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alex.Graphics.Models.Entity
{
	public partial class EntityModelRenderer
	{
		public class ModelBone : IDisposable
		{
			private IndexBuffer Buffer { get; set; }
			public ModelBoneCube[] Parts { get; }

			private Vector3 _rotation = Vector3.Zero;
			public Vector3 Rotation
			{
				get { return _rotation; }
				set { _rotation = value; }
			}

			private Vector3 _position = Vector3.Zero;
			public Vector3 Position
			{
				get { return _position; }
				set { _position = value; }
			}
			
			private List<IAttachable> Attachables { get; } = new List<IAttachable>();

			private EntityModelBone OriginalBone { get; }
			public string Parent => OriginalBone.Parent;
			public ModelBone(ModelBoneCube[] parts, EntityModelBone originalBone)
			{
				Parts = parts;
				OriginalBone = originalBone;
			}

			private bool _isDirty = true;

			public Matrix RotationMatrix = Matrix.Identity;
			public bool UpdateRotationMatrix = true;
			private Matrix CharacterMatrix { get; set; }
			public void Render(IRenderArgs args, PlayerLocation position, Matrix characterMatrix)
			{
				if (Buffer == null)
					return;
				
				args.GraphicsDevice.Indices = Buffer;

				int idx = 0;
				for (var index = 0; index < Parts.Length; index++)
				{
					var part = Parts[index];

					AlphaTestEffect effect = part.Effect;
					if (effect == null) continue;
					
					effect.World = RotationMatrix;
					effect.View = args.Camera.ViewMatrix;
					effect.Projection = args.Camera.ProjectionMatrix;

					foreach (var pass in effect.CurrentTechnique.Passes)
					{
						pass.Apply();
					}
					
					args.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, idx, part.Indexes.Length / 3);
					idx += part.Indexes.Length;
				}

				foreach (var attach in Attachables.ToArray())
				{
					attach.Render(args);
				}
			}

			public void Update(IUpdateArgs args, Matrix characterMatrix, Vector3 diffuseColor, PlayerLocation position)
			{
				
				CharacterMatrix = characterMatrix;
				foreach (var part in Parts)
				{
					var headYaw = part.ApplyHeadYaw ? MathUtils.ToRadians(-(position.HeadYaw - position.Yaw)) : 0f;
					var pitch = part.ApplyPitch ? MathUtils.ToRadians(position.Pitch) : 0f;

					var rot = _rotation + part.Rotation;

					/*Matrix rotMatrix = Matrix.CreateTranslation(-part.Pivot)
									   * Matrix.CreateFromYawPitchRoll(
																	   MathUtils.ToRadians(rot.Y),
																	   MathUtils.ToRadians(rot.X),
																	   MathUtils.ToRadians(rot.Z)
																	  );
					rotMatrix *= Matrix.CreateTranslation(part.Pivot);
					
					if (part.ApplyYaw)
						rotMatrix *= Matrix.CreateRotationY(yaw);*/

					Matrix rotMatrix = Matrix.CreateTranslation(-part.Pivot) 
					                   * Matrix.CreateFromYawPitchRoll(
						                   MathUtils.ToRadians(rot.Y), 
						                   MathUtils.ToRadians(rot.X), 
						                   MathUtils.ToRadians(rot.Z)
					                   )  
					                   * Matrix.CreateTranslation(part.Pivot);


					var rotMatrix2 = Matrix.CreateTranslation(-part.Pivot) *
					                 Matrix.CreateFromYawPitchRoll(headYaw, pitch, 0f) *
					                 Matrix.CreateTranslation(part.Pivot);
					
					var rotateMatrix = Matrix.CreateTranslation(part.Origin) * (rotMatrix2 *
					                                                            rotMatrix);
					
					RotationMatrix = rotateMatrix * characterMatrix;

					if (part.Effect != null)
					{
						part.Effect.DiffuseColor = diffuseColor;
					}

					if (!part.IsDirty)
						continue;

					_isDirty = true;
					part.Update(args);
				}

				foreach (var attachable in Attachables.ToArray())
				{
					attachable.Update(RotationMatrix);
				}

				if (_isDirty)
				{
					UpdateVertexBuffer(args.GraphicsDevice);
				}
			}

			private void UpdateVertexBuffer(GraphicsDevice device)
			{
				var indices = Parts.SelectMany(x => x.Indexes).ToArray();

				IndexBuffer currentBuffer = Buffer;

				if (indices.Length > 0 && (Buffer == null || currentBuffer.IndexCount != indices.Length))
				{
					IndexBuffer buffer = GpuResourceManager.GetIndexBuffer(this, device, IndexElementSize.SixteenBits,
						indices.Length, BufferUsage.None);

					buffer.SetData(indices);
					Buffer = buffer;
					
					currentBuffer?.Dispose();
				}
			}

			public void Attach(IAttachable attachable)
			{
				if (!Attachables.Contains(attachable))
					Attachables.Add(attachable);
			}

			public void Detach(IAttachable attachable)
			{
				if (Attachables.Contains(attachable))
					Attachables.Remove(attachable);
			}

			public void DetachAll()
			{
				Attachables.Clear();
			}
			
			public void Dispose()
			{
				Buffer?.Dispose();
			}
		}
	}
}
