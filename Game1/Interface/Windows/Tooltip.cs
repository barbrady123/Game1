using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Items;
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface.Windows
{
	public class Tooltip : Component
	{
		private const int TooltipTimer = 15;

		private int _timer;
		private ImageText _text;
		private Component _owner;
		private Component _host;

		public Component Owner
		{
			get { return _owner; }
			set
			{
				if (_owner != value)
				{
					if (_owner != null)
					{
						_owner.OnMouseOut -= _owner_OnMouseOut;
						_owner.OnMouseOver -= _owner_OnMouseOver;
					}

					_owner = value;

					if (_owner != null)
					{
						_owner.OnMouseOut += _owner_OnMouseOut;
						_owner.OnMouseOver += _owner_OnMouseOver;
						Show(_owner.TooltipText);
					}
					else
					{
						Hide();
					}
				}
			}
		}

		public int TextPadding => 5;

		// Eventually we'll want prettier tooltips with more than just a line of text...
		public Tooltip(Component host, SpriteBatchData spriteBatchData = null) : base(Rectangle.Empty, background: "black", spriteBatchData: spriteBatchData, drawIfDisabled: false)
		{			
			_timer = -1;
			_text = new ImageText(null, true);
			_text.Alignment = ImageAlignment.Centered;
			_text.Scale = new Vector2(0.9f, 0.9f);
			_host = host;
			_host.OnMouseOver += _host_OnMouseOver;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_text.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_text.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			if (_timer == 0)
			{
				_timer = -1;
				this.IsActive = true;
			}
			else if (_timer > 0)
			{
				_timer--;
			}

			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_text.Update(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_text.Draw(spriteBatch);
		}

		public void Show(string text)
		{
			if (String.IsNullOrWhiteSpace(text))
				return;

			_timer = Tooltip.TooltipTimer;
			UnloadContent();
			var position = InputManager.MousePosition.Offset(10, 10);
			_text.UpdateText(text);
			var textSize = _text.Size;
			this.Bounds = new Rectangle(position.X, position.Y, (int)textSize.X + this.TextPadding * 2, (int)textSize.Y + this.TextPadding * 2);
			LoadContent();
		}

		private void Refresh()
		{
			if (_owner == null)
				Hide();

			var position = InputManager.MousePosition.Offset(10, 10);
			_text.UpdateText(_owner.TooltipText);
			var textSize = _text.Size;
			this.Bounds = new Rectangle(position.X, position.Y, (int)textSize.X + this.TextPadding * 2, (int)textSize.Y + this.TextPadding * 2);
		}

		protected override void RepositionObjects(bool loadContent = false)
		{
			base.RepositionObjects(loadContent);
			if (_text != null)
				_text.Position = this.Bounds.CenterVector();
		}

		private void Hide()
		{
			_timer = -1;
			this.IsActive = false;
		}

		private void _owner_OnMouseOut(object sender, ComponentEventArgs e)
		{
			this.Owner = null;
		}

		private void _owner_OnMouseOver(object sender, ComponentEventArgs e)
		{
			Refresh();
		}

		private void _host_OnMouseOver(object sender, ComponentEventArgs e)
		{
			this.Owner = e.Meta as Component;
		}
	}
}
