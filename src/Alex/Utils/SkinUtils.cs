﻿using System;
using System.Drawing;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework.Graphics;
using MiNET.Utils.Skins;
using Newtonsoft.Json;

namespace Alex.Utils
{
	public static class SkinUtils
	{
		public static bool TryGetSkin(string json, GraphicsDevice graphics, out Texture2D texture, out bool isSlim)
		{
			isSlim = false;
			try
			{
				TexturesResponse r = JsonConvert.DeserializeObject<TexturesResponse>(json);
				if (r != null)
				{
					string url = r.textures?.SKIN?.url;
					if (url != null)
					{
						byte[] data;
						using (WebClient wc = new WebClient())
						{
							data = wc.DownloadData(url);
						}

						using (MemoryStream ms = new MemoryStream(data))
						{
							texture = Texture2D.FromStream(graphics, ms);
						}

						isSlim = (r.textures.SKIN.metadata?.model == "slim");

						return true;
					}
				}
			}
			catch
			{
				
			}

			texture = null;
			return false;
		}
		
		public static bool TryGetSkin(Uri skinUri, GraphicsDevice graphics, out Texture2D texture)
		{
			try
			{
				byte[] data;
				using (WebClient wc = new WebClient())
				{
					data = wc.DownloadData(skinUri);
				}

				using (MemoryStream ms = new MemoryStream(data))
				{
					texture = Texture2D.FromStream(graphics, ms);
				}

				return true;
			}
			catch
			{

			}

			texture = null;
			return false;
		}

		public static bool TryGetBitmap(this Skin skin, out Bitmap result)
		{
			try
			{
				var bytes = skin.Data;

				int width = 64;
				var height = bytes.Length == 64 * 32 * 4 ? 32 : 64;

				Bitmap bitmap = new Bitmap(width, height);

				int i = 0;
				for (int y = 0; y < bitmap.Height; y++)
				{
					for (int x = 0; x < bitmap.Width; x++)
					{
						byte r = bytes[i++];
						byte g = bytes[i++];
						byte b = bytes[i++];
						byte a = bytes[i++];

						Color color = Color.FromArgb(a, r, g, b);
						bitmap.SetPixel(x, y, color);
					}
				}

				result = bitmap;
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		public class SkinMetadata
		{
			public string model { get; set; }
		}

		public class SKIN
		{
			public string url { get; set; }
			public SkinMetadata metadata { get; set; } = null;
		}

		public class Textures
		{
			public SKIN SKIN { get; set; }
			public SKIN CAPE { get; set; }
		}

		public class TexturesResponse
		{
			public long timestamp { get; set; }
			public string profileId { get; set; }
			public string profileName { get; set; }
			public Textures textures { get; set; }
		}
	}
}
