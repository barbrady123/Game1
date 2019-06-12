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
	public abstract class Component
	{
		private int _delayInputCycles;
		protected ImageTexture _background;
		private string _backgroundName;
		protected ImageTexture _border;
		private bool _readyDisableOnEscape;
		private Rectangle _bounds;
		private bool _hasBorder;
		private bool _fireMouseEvents;
		private bool _inactiveMouseEvents;
		private ComponentState _state;
		private readonly SpriteBatchData _spriteBatchData;

		public virtual ComponentState State
		{
			get { return _state; }
			set {
				if (_state != value)
				{
					_state = value;
					StateChange();
				}
			}
		}

		protected virtual void StateChange() { }

		public int? Duration { get; set; }

		public Rectangle Bounds
		{ 
			get { return _bounds; }
			set
			{
				if (_bounds != value)
				{
					_bounds = value;
					SetupBackground();
					SetupBorder();
					RepositionObjects();
				}
			}
		}

		public event EventHandler<ComponentEventArgs> OnReadyDisable;
		public event EventHandler<ComponentEventArgs> OnMouseOver;
		public event EventHandler<ComponentEventArgs> OnMouseIn;
		public event EventHandler<ComponentEventArgs> OnMouseOut;

		protected bool _mouseover;
		protected virtual Size ContentMargin => new Size(20, 20);
		protected virtual int BorderThickness => 2;
		protected virtual Color BorderColor => Color.White;

		/// <summary>
		/// For non-visual components....
		/// </summary>
		public Component(Rectangle? bounds = null,
						 bool readyDisableOnEscape = false,
						 string background = "black",
						 SpriteBatchData spriteBatchData = null,
						 bool hasBorder = false,
						 bool fireMouseEvents = true,
						 bool inactiveMouseEvents = false)
		{
			_bounds = bounds ?? Rectangle.Empty;
			_readyDisableOnEscape = readyDisableOnEscape;
			_spriteBatchData = spriteBatchData;
			_hasBorder = hasBorder;
			_delayInputCycles = 0;
			_mouseover = false;
			_backgroundName = background;
			_fireMouseEvents = fireMouseEvents;
			_inactiveMouseEvents = inactiveMouseEvents;

			SetupBackground();
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
			if (_inactiveMouseEvents && !this.State.HasFlag(ComponentState.Active))
				CheckMouseEvents();

			if (this.State.HasFlag(ComponentState.Active))
				UpdateDuration(gameTime);
		}

		private void UpdateDuration(GameTime gameTime)
		{
			if (this.Duration != null)
			{
				if (this.Duration <= 0)
				{
					ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.Timer });
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

			if (this.State.HasFlag(ComponentState.TakingInput))
				UpdateInput(gameTime);
		}

		public virtual void UpdateInput(GameTime gameTime)
		{
			_mouseover = CheckMouseEvents();

			if (_readyDisableOnEscape && InputManager.KeyPressed(Keys.Escape, true))
				ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.Escape, Value = "escape" });
		}

		public virtual bool CheckMouseEvents()
		{
			var mouseover = InputManager.MouseOver(this.Bounds);
			if (!_fireMouseEvents)
				return mouseover;

			if (mouseover)
			{
				if (!_mouseover)
					MouseIn(new ComponentEventArgs());
				MouseOver(new ComponentEventArgs());
			}
			else if (_mouseover)
			{
				MouseOut(new ComponentEventArgs());
			}

			return mouseover;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (this.State.HasFlag(ComponentState.Visible))
			{
				if (_spriteBatchData != null)
					Util.WrappedDraw(DrawVisible, _spriteBatchData, _bounds);
				else
					DrawVisible(spriteBatch);
			}
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

		protected virtual void MouseOver(ComponentEventArgs e) => OnMouseOver?.Invoke(this, e);

		protected virtual void MouseIn(ComponentEventArgs e) => OnMouseIn?.Invoke(this, e);

		protected virtual void MouseOut(ComponentEventArgs e) => OnMouseOut?.Invoke(this, e);

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

		protected void SetupBackground()
		{
			_background?.UnloadContent();
			// TODO: This should add support for the Util.GenerateSolidBackgroundTexture method for solid colors...
			if ((!String.IsNullOrWhiteSpace(_backgroundName)) && (this.Bounds != Rectangle.Empty))
				_background = new ImageTexture($"{Game1.BackgroundRoot}/{_backgroundName}", true) { Alignment = ImageAlignment.Centered };
		}

		protected void SetupBorder()
		{
			if (_hasBorder && (this.Bounds != Rectangle.Empty))
			{
				_border?.UnloadContent();
				_border = Util.GenerateBorderTexture(this.Bounds.Width, this.Bounds.Height, this.BorderThickness, this.BorderColor, true);
				_border.Alignment = ImageAlignment.Centered;
			}
		}
	}
}
