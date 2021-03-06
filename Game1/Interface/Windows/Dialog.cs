﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Screens;
using Game1.Menus;

namespace Game1.Interface.Windows
{
	// TODO: Support text wrap/multiline...
	public class Dialog : Component
	{
		private Menu _buttonMenu;
		private ImageText _textImage;
		private string _text;

		public DialogButton Buttons { get; set; }

		public string Text
		{ 
			get { return _text; }
			set {
				if (_text != value)
				{
					_text = value;
					_textImage?.UpdateText(_text);
				}
			}
		}

		public event EventHandler<ComponentEventArgs> OnItemSelect;
		
		public Dialog(string text, DialogButton buttons, Rectangle bounds, int? duration) : base(bounds, true, "black", spriteBatchData:SpriteBatchManager.Get("notification"), drawIfDisabled: false)
		{
			this.Buttons = buttons;
			this.Duration = duration;

			// TODO : Move this to a buttons component...
			switch (this.Buttons)
			{
				case DialogButton.Ok :	
					_buttonMenu = new OkMenu(Point.Zero) { IsActive = true };
					var size = _buttonMenu.CalculateMenuSize(null);
					_buttonMenu.UpdatePosition(new Point(bounds.Center.X - (size.Width / 2), bounds.Bottom - this.ContentMargin.Height - (size.Height / 2)));
					_buttonMenu.OnItemSelect += _buttonMenu_OnItemSelect;
					break;
			}

			_textImage = new ImageText(text, true) {
				Alignment = ImageAlignment.Centered,
				Position = this.Bounds.TopCenterVector(yOffset: 30)
			};
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_buttonMenu?.LoadContent();
		}

		public override void UnloadContent()
		{
			_buttonMenu?.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_buttonMenu?.Update(gameTime);
			_textImage.Update(gameTime);
			InputManager.BlockAllInput();
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_buttonMenu?.Draw(spriteBatch);
			_textImage.Draw(spriteBatch);
		}

		private void _buttonMenu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			OnItemSelect?.Invoke(this, e);
			if (e.Value == "ok")
				ReadyDisable(e);
		}
	}
}
