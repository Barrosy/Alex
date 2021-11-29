using System.Threading.Tasks;
using Alex.Common.Graphics;
using Alex.Common.Gui.Elements;
using Alex.Common.Services;
using Alex.Gamestates.Common;
using Alex.Gui;
using Alex.Gui.Elements;
using Alex.Utils;
using Microsoft.Xna.Framework;
using RocketUI;

namespace Alex.Gamestates.Multiplayer
{
	public class UserSelectionState : ListSelectionStateBase<UserSelectionItem>
	{
		public delegate void ProfileSelected(PlayerProfile selectedProfile);
		public delegate void Cancelled();
		public delegate void AddAccountButtonClicked();
		
		private AlexButton SelectButton { get; set; }
		private AlexButton AddButton { get; set; }
		private AlexButton RemoveButton { get; set; }

		private readonly ServerTypeImplementation _serverType;
		private GuiPanoramaSkyBox _skyBox;
		public bool AllowDelete { get; set; } = true;

		private readonly ProfileSelected _onProfileSelected;
		private readonly Cancelled _onCancel;
		public UserSelectionState(ServerTypeImplementation serverType, GuiPanoramaSkyBox skyBox, ProfileSelected onProfileSelected, Cancelled onCancel)
		{
			Title = $"{serverType.DisplayName} - Account Selection";
			_serverType = serverType;
			_skyBox = skyBox;
			_onProfileSelected = onProfileSelected;
			_onCancel = onCancel;
			Background = new GuiTexture2D(skyBox, TextureRepeatMode.Stretch);

			Footer.AddRow(
				row =>
				{
					row.AddChild(
						SelectButton = new AlexButton("Select Account", SelectAccountClicked) { Enabled = false });

					row.AddChild(AddButton = new AlexButton("Add Account", AddAccountClicked) { Enabled = true });
				});

			Footer.AddRow(
				row =>
				{
					row.AddChild(RemoveButton = new AlexButton("Remove", OnRemoveClicked) { Enabled = false });
					row.AddChild(new GuiBackButton() { TranslationKey = "gui.cancel" });
				});
		}

		public void ReloadData()
		{
			ClearItems();
			foreach (var profile in _serverType.ProfileProvider.Data)
			{
				AddItem(new UserSelectionItem(profile));
			}
		}

		private void SaveData()
		{
			//_serverType.ProfileProvider.Save();
		}
		
		private void OnRemoveClicked()
		{
			var item = SelectedItem;
			var selectedProfile = item?.Profile;

			if (selectedProfile == null)
				return;

			var profileProvider = _serverType.ProfileProvider;
			if (profileProvider != null)
			{
				RemoveItem(item);
				profileProvider.RemoveEntry(selectedProfile);//.RemoveProfile(selectedProfile);
			}
		}

		private void AddAccountClicked()
		{
			//Show login.
			_serverType.Authenticate(_skyBox, _onProfileSelected, null);
		}

		private void SelectAccountClicked()
		{
			SelectAccount(SelectedItem);
		}

		private void SelectAccount(UserSelectionItem item)
		{
			if (item?.Profile != null)
			{
				_onProfileSelected?.Invoke(item?.Profile);/*.ContinueWith(
					async x =>
					{
						var profile = await x;

						profile = await _serverType.VerifySession(profile);

						if (profile.Authenticated)
						{
							
						}
					});*/
			}
		}
		
		/// <inheritdoc />
		protected override void OnShow()
		{
			base.OnShow();
			ReloadData();
		}
		
		/// <inheritdoc />
		protected override void OnHide()
		{
			base.OnHide();
			SaveData();
			
			_onCancel?.Invoke();
		}

		/// <inheritdoc />
		protected override void OnSelectedItemChanged(UserSelectionItem newItem)
		{
			base.OnSelectedItemChanged(newItem);
			if (newItem == null)
			{
				SelectButton.Enabled = false;
				RemoveButton.Enabled = false;
				return;
			}
			
			SelectButton.Enabled = true;
			RemoveButton.Enabled = AllowDelete;
		}

		/// <inheritdoc />
		protected override void OnUpdate(GameTime gameTime)
		{
			_skyBox?.Update(gameTime);
			base.OnUpdate(gameTime);
		}

		/// <inheritdoc />
		protected override void OnDraw(IRenderArgs args)
		{
			var sb = _skyBox;

			if (sb != null)
			{
				if (!sb.Loaded)
				{
					sb.Load(Alex.GuiRenderer);
				}
				sb.Draw(args);
			}

			base.OnDraw(args);
		}
		
		protected override void OnItemDoubleClick(UserSelectionItem item)
		{
			base.OnItemDoubleClick(item);
		    
			if (SelectedItem != item)
				return;
		    
			SelectAccount(item);
		}
	}
}