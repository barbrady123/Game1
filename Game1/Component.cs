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
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1
{
	public abstract class Component : IActivatable
	{
		private readonly SpriteBatchData _spriteBatchData;
		private bool _isActive;
		protected ActivationManager _activator;
		private string _backgroundName;
		protected ImageTexture _background;
		private bool _hasBorder;
		protected ImageTexture _border;
		private Rectangle _bounds;
		private int _delayInputCycles;
		private bool _readyDisableOnEscape;
		private bool _fireMouseEvents;
		private bool _inactiveMouseEvents;
		private bool _killFurtherInput;
		private bool _drawIfDisabled;
		private bool _enabledTooltip;
		protected Tooltip _tooltip;
		private bool _enabledContextMenu;

		// TODO: add context?

		public virtual bool IsActive
		{
			get { return _isActive; }
			set {
				if (_isActive != value)
				{
					_isActive = value;
					IsActiveChange();
				}
			}
		}

		protected virtual void IsActiveChange() { }

		public int? Duration { get; set; }

		public Rectangle Bounds
		{ 
			get { return _bounds; }
			set
			{
				if (_bounds != value)
				{
					_bounds = value;
					bool bgLoaded = SetupBackground();
					bool borderLoaded = SetupBorder();
					RepositionObjects(bgLoaded || borderLoaded);
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
		public virtual string TooltipText => null;

		public Component(Rectangle? bounds = null,
						 bool readyDisableOnEscape = false,
						 string background = "black",
						 SpriteBatchData spriteBatchData = null,
						 bool hasBorder = false,
						 bool fireMouseEvents = true,
						 bool inactiveMouseEvents = false,
						 bool killFurtherInput = false,
						 bool drawIfDisabled = true,
						 bool enabledTooltip = false)
		{
			_bounds = bounds ?? Rectangle.Empty;
			_activator = new ActivationManager();
			// Auto register tooltip/context/etc...when available...
			_readyDisableOnEscape = readyDisableOnEscape;
			_spriteBatchData = spriteBatchData;
			_hasBorder = hasBorder;
			_delayInputCycles = 0;
			_mouseover = false;
			_backgroundName = background;
			_fireMouseEvents = fireMouseEvents;
			_inactiveMouseEvents = inactiveMouseEvents;
			_killFurtherInput = killFurtherInput;
			_drawIfDisabled = drawIfDisabled;
			if (_enabledTooltip = enabledTooltip)
				_activator.Register(_tooltip = new Tooltip(this, SpriteBatchManager.Get("tooltip")), false, "popup");

			SetupBackground();
			SetupBorder();
			RepositionObjects();
		}

		public virtual void LoadContent()
		{
			_background?.LoadContent();
			_border?.LoadContent();
			_tooltip?.LoadContent();
		}

		public virtual void UnloadContent()
		{
			_background?.UnloadContent();
			_border?.UnloadContent();
			_tooltip?.UnloadContent();
		}

		public virtual void Update(GameTime gameTime)
		{
			if (_inactiveMouseEvents && !this.IsActive)
				CheckMouseEvents();

			if (this.IsActive)
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
			_tooltip?.Update(gameTime);
			UpdateDelayInput(gameTime);
		}

		private void UpdateDelayInput(GameTime gameTime)
		{
			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			UpdateInput(gameTime);
		}

		public virtual void UpdateInput(GameTime gameTime)
		{
			_mouseover = CheckMouseEvents();

			if (_readyDisableOnEscape && InputManager.KeyPressed(Keys.Escape, true))
				ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.Escape, Value = "escape" });

			if (_killFurtherInput)
				InputManager.BlockAllInput();
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

		public void Draw(SpriteBatch spriteBatch)
		{
			if ((!_drawIfDisabled) && (!this.IsActive))
				return;

			if (_spriteBatchData != null)
				Util.WrappedDraw(DrawInternal, _spriteBatchData, _bounds);
			else
				DrawInternal(spriteBatch);
		}

		protected virtual void DrawInternal(SpriteBatch spriteBatch)
		{
			_background?.Draw(spriteBatch);
			_border?.Draw(spriteBatch);
			_tooltip?.Draw(spriteBatch);
		}

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		protected virtual void ReadyDisable(ComponentEventArgs e) => OnReadyDisable?.Invoke(this, e);

		protected virtual void MouseOver(ComponentEventArgs e) => OnMouseOver?.Invoke(this, e);

		protected virtual void MouseIn(ComponentEventArgs e) => OnMouseIn?.Invoke(this, e);

		protected virtual void MouseOut(ComponentEventArgs e) => OnMouseOut?.Invoke(this, e);

		protected virtual void RepositionObjects(bool loadContent = false)
		{
			if (loadContent)
			{
				if (_background?.Texture != null)
					_background?.UnloadContent();

				if (_border?.Texture != null)
					_border?.UnloadContent();
			}

			if (_background != null)
			{
				_background.Position = this.Bounds.CenterVector();
				_background.SourceRect = this.Bounds;
			}

			if (_border != null)
				_border.Position = this.Bounds.CenterVector();

			if (loadContent)
			{
				_background?.LoadContent();
				_border?.LoadContent();
			}
		}

		protected bool SetupBackground()
		{
			bool wasLoaded = _background?.Texture != null;

			_background?.UnloadContent();
			// TODO: This should add support for the Util.GenerateSolidBackgroundTexture method for solid colors...
			if ((!String.IsNullOrWhiteSpace(_backgroundName)) && (this.Bounds != Rectangle.Empty))
				_background = new ImageTexture($"{Game1.BackgroundRoot}/{_backgroundName}", true) { Alignment = ImageAlignment.Centered };

			return wasLoaded;
		}

		protected bool SetupBorder()
		{
			bool wasLoaded = _border?.Texture != null;

			if (_hasBorder && (this.Bounds != Rectangle.Empty))
			{
				_border?.UnloadContent();
				_border = Util.GenerateBorderTexture(this.Bounds.Width, this.Bounds.Height, this.BorderThickness, this.BorderColor, true);
				_border.Alignment = ImageAlignment.Centered;
			}

			return wasLoaded;
		}
	}
}
