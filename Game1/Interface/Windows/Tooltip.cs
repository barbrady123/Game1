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
	public class Tooltip : Window
	{		
		private int _timer;

		public object Owner { get; private set; }
		public override string SpriteBatchName => "tooltip";
		public override int TitleOffset => 5;

		// Eventually we'll want prettier tooltips with more than just a line of text...
		public Tooltip() : base(Rectangle.Empty, "black", "", null, false)
		{
			_timer = -1;
			_titleImage.Alignment = ImageAlignment.CenteredTop;
			_titleImage.Scale = new Vector2(0.9f, 0.9f);
		}

		public override void Update(GameTime gameTime, bool processInput)
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

			base.Update(gameTime, processInput);
		}

		public void Show(string text, Point position, int timer, object sender)
		{			
			if (sender != this.Owner)
				_timer = Math.Max(0, timer);

			this.Owner = sender;
			this.Title = text;
			this.Bounds = new Rectangle(position.X, position.Y, (int)this.TitleSize.X + this.TitleOffset * 2, (int)this.TitleSize.Y + this.TitleOffset * 2);
		}

		public void Reset(object sender)
		{
			if (sender != this.Owner)
				return;

			_timer = -1;
			this.Owner = null;
			this.IsActive = false;
		}
	}
}
