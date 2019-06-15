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
		private int _timer;
		private ImageText _text;

		public object Owner { get; private set; }	// This should be a Component
		public int TextPadding => 5;

		// Eventually we'll want prettier tooltips with more than just a line of text...
		public Tooltip(SpriteBatchData spriteBatchData = null) : base(Rectangle.Empty, background: "black", spriteBatchData: spriteBatchData)
		{			
			_timer = -1;
			_text = new ImageText(null, true);
			_text.Alignment = ImageAlignment.Centered;
			_text.Scale = new Vector2(0.9f, 0.9f);
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

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_text.Update(gameTime);

			if (_timer == 0)
			{
				_timer = -1;
				this.State |= ComponentState.Visible;
			}
			else if (_timer > 0)
			{
				_timer--;
			}
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_text.Draw(spriteBatch);
		}

		public void Show(string text, Point position, int timer, object sender)
		{			
			// If this request is coming from a different object, reset the timer...otherwise let it continue where it is...
			if (sender != this.Owner)
			{
				_timer = Math.Max(0, timer);
				UnloadContent();
			}

			this.Owner = sender;
			_text.UpdateText(text);
			var textSize = _text.Size;
			this.Bounds = new Rectangle(position.X, position.Y, (int)textSize.X + this.TextPadding * 2, (int)textSize.Y + this.TextPadding * 2);

			LoadContent();
		}

		protected override void RepositionObjects()
		{
			base.RepositionObjects();
			if (_text != null)
				_text.Position = this.Bounds.CenterVector();
		}

		public void Reset(object sender = null)
		{
			if ((sender != null) && (sender != this.Owner))
				return;

			_timer = -1;
			this.Owner = null;
			this.State &= ~ComponentState.Visible;
		}
	}
}
