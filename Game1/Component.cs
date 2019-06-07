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
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1
{
	// Do we want to make screens components?  
	public class Component
	{
		private int _delayInputCycles;
		private ImageTexture _background;
		private ImageTexture _border;
		private bool _mouseover;
		private bool _readyDisableOnEscape;
		private Rectangle _bounds;
		private bool _hasBorder;

		public ComponentState State { get; set; }
		public int? Duration { get; set; }

		public Rectangle Bounds
		{ 
			get { return _bounds; }
			set
			{
				if (_bounds != value)
				{
					_bounds = value;
					SetupBorder();
					RepositionObjects();
				}
			}
		}

		public event EventHandler<ComponentEventArgs> OnReadyDisable;
		public event EventHandler<MouseEventArgs> OnMouseOver;
		public event EventHandler<MouseEventArgs> OnMouseIn;
		public event EventHandler<MouseEventArgs> OnMouseOut;

		protected virtual Size ContentMargin => new Size(20, 20);
		protected virtual int BorderThickness => 2;
		protected virtual Color BorderColor => Color.White;
		
		public Component(Rectangle bounds, bool readyDisableOnEscape = false, string background = "black", bool hasBorder = false)
		{
			_bounds = bounds;
			_readyDisableOnEscape = readyDisableOnEscape;
			_hasBorder = hasBorder;
			_delayInputCycles = 0;
			_mouseover = false;

			// TODO: This should add support for the Util.GenerateSolidBackgroundTexture method for solid colors...
			if (!String.IsNullOrWhiteSpace(background))
				_background = new ImageTexture($"{Game1.BackgroundRoot}/{background}", true) { Alignment = ImageAlignment.Centered };

			SetupBorder();
			RepositionObjects();
		}

		public virtual void LoadContent()
		{
			_background?.LoadContent();
			_border?.LoadContent();
		}

		public virtual void UnloadContent()
		{
			_background?.UnloadContent();
			_border?.UnloadContent();
		}

		public virtual void Update(GameTime gameTime)
		{
			if (this.State.HasFlag(ComponentState.Active))
				UpdateDuration(gameTime);
		}

		private void UpdateDuration(GameTime gameTime)
		{
			if (this.Duration != null)
			{
				if (this.Duration <= 0)
				{
					ReadyDisable(new ComponentEventArgs("timer", this.GetType().Name, null));
					// Should we auto-disable here?
					this.Duration = null;
				}
				else
				{
					this.Duration--;
				}
			}

			UpdateActive(gameTime);
		}

		public virtual void UpdateActive(GameTime gameTime)
		{
			_background?.Update(gameTime);
			_border?.Update(gameTime);
			UpdateDelayInput(gameTime);
		}

		private void UpdateDelayInput(GameTime gameTime)
		{
			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			if (this.State.HasFlag(ComponentState.DetectingMousePosition))
				UpdateMousePosition(gameTime);
			if (this.State.HasFlag(ComponentState.TakingInput))
				UpdateInput(gameTime);
		}

		public virtual void UpdateMousePosition(GameTime gameTime)
		{
			var mouseover = InputManager.MouseOver(this.Bounds);
			if (mouseover)
			{
				if (!_mouseover)
					MouseIn(new MouseEventArgs());
				MouseOver(new MouseEventArgs());
			}
			else if (_mouseover)
			{
				MouseOut(new MouseEventArgs());
			}

			_mouseover = mouseover;
		}

		public virtual void UpdateInput(GameTime gameTime)
		{
			if (!_readyDisableOnEscape)
				return;

			if (InputManager.KeyPressed(Keys.Escape, true))
				ReadyDisable(new ComponentEventArgs("escape", this.GetType().Name, null));
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			// Do we still need to wrap this call to DrawInternal?  Or, better to make this "dumb" to the concept and use the manager to get the spritebatches for these calls..?
			if (this.State.HasFlag(ComponentState.Visible))
				DrawVisible(spriteBatch);
		}

		public virtual void DrawVisible(SpriteBatch spriteBatch)
		{
			_background?.Draw(spriteBatch);
			_border?.Draw(spriteBatch);
		}

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		protected virtual void ReadyDisable(ComponentEventArgs e) => OnReadyDisable?.Invoke(this, e);

		protected virtual void MouseOver(MouseEventArgs e)  => OnMouseOver?.Invoke(this, e);

		protected virtual void MouseIn(MouseEventArgs e) => OnMouseIn?.Invoke(this, e);

		protected virtual void MouseOut(MouseEventArgs e) => OnMouseOut?.Invoke(this, e);

		protected virtual void RepositionObjects()
		{
			if (_background != null)
			{
				_background.Position = this.Bounds.CenterVector();
				_background.SourceRect = this.Bounds;
			}

			if (_border != null)
				_border.Position = this.Bounds.CenterVector();
		}

		private void SetupBorder()
		{
			if (_hasBorder && (this.Bounds != Rectangle.Empty))
			{
				_border?.UnloadContent();
				_border = Util.GenerateBorderTexture(this.Bounds.Width, this.Bounds.Height, this.BorderThickness, this.BorderColor, true);
				_border.Alignment = ImageAlignment.Centered;
				_border.LoadContent();
			}
		}
	}
}
