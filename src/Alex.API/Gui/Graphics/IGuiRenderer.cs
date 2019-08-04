﻿using Alex.API.Graphics.Textures;
using Alex.API.Graphics.Typography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alex.API.Gui.Graphics
{
    public interface IGuiRenderer
    {
        GuiScaledResolution ScaledResolution { get; set; }
        void Init(GraphicsDevice graphics);

        IFont Font { get; set; }
        
        TextureSlice2D GetTexture(string textureName);
        Texture2D GetTexture2D(string textureName);

        string GetTranslation(string key);

        Vector2 Project(Vector2 point);
        Vector2 Unproject(Vector2 screen);
    }
}
