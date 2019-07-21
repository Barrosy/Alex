﻿using Alex.API.Gui.Elements;
using Alex.API.Gui.Elements.Controls;
using Alex.API.Utils;
using Alex.GameStates;
using Alex.GameStates.Gui.Common;
using Alex.Gui.Screens;
using RocketUI;

namespace Alex.GameStates
{
    public class DisconnectedScreen : GuiMenuStateBase
    {
	    public string Reason { get; set; } = "disconnect.lost";
	    public GuiTextElement DisconnectedTextElement { get; private set; }
		public DisconnectedScreen()
		{
			TitleTranslationKey = "multiplayer.disconnect.generic";

			Body.ChildAnchor = Alignment.MiddleCenter;
			Body.AddChild(DisconnectedTextElement = new GuiTextElement()
			{
				Text = Reason,
				TextColor = TextColor.Red,
				Anchor = Alignment.MiddleCenter
			});

			Footer.AddChild(new GuiButton(MenuButtonClicked)
			{
				TranslationKey = "gui.toTitle",
				Anchor = Alignment.MiddleCenter,
				Modern = false
			});
		}

		private void MenuButtonClicked()
		{
			Alex.GameStateManager.SetActiveState<TitleScreen>("title");
		}

	    protected override void OnShow()
	    {
			
		    base.OnShow();
	    }
    }
}
