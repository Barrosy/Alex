﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Alex.API.Graphics;
using Alex.API.Graphics.Textures;
using Alex.API.Gui.Graphics;
using Alex.API.Gui.Layout;
using Microsoft.Xna.Framework;

namespace Alex.API.Gui.Elements
{
    public partial class GuiElement
    {
        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set => _rotation = MathHelper.ToRadians(value);
        }

        public virtual Vector2 RotationOrigin { get; set; } = Vector2.Zero;

        public GuiTexture2D Background;
        public GuiTexture2D BackgroundOverlay;


        protected virtual void OnDraw(GuiSpriteBatch graphics, GameTime gameTime)
        {
            graphics.FillRectangle(RenderBounds, Background);
            
            graphics.FillRectangle(RenderBounds, BackgroundOverlay);
        }
    }
}
