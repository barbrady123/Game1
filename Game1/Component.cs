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
using Game1.Menus;

namespace Game1
{
	public abstract class Component : IActivatable, ISupportsTooltip, ISupportsContextMenu
	{
		protected readonly object _lock = new object();
		private readonly SpriteBatchData _spriteBatchData;
		private bool _isActive;
		protected ActivationManager _activator;
		private readonly string _backgroundName;
		protected ImageTexture _background;
		private readonly bool _hasBorder;
		protected ImageTexture _border;
		private Rectangle _bounds;
		private int _delayInputCycles;
		private readonly bool _readyDisableOnEscape;
		private readonly bool _fireMouseEvents;
		private readonly bool _inactiveMouseEvents;
		private readonly bool _drawIfDisabled;
		private readonly bool _enabledTooltip;
		protected Tooltip _tooltip;
		private readonly bool _enabledContextMenu;
		protected Menu _contextMenu;
		protected Dialog _dialog;

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
					bool resized = (_bounds.Width != value.Width) || (_bounds.Height != value.Height);
					_bounds = value;
					BoundsChanged(resized);
				}
			}
		}

		protected virtual void BoundsChanged(bool resized)
		{
			if (this.Bounds == Rectangle.Empty)
				return;

			if (_background != null)
			{
				_background.Position = this.Bounds.CenterVector();
				_background.Scale = Util.ScaleUpVector(_background.Bounds, this.Bounds);
			}

			if (resized)
				SetupBorder();

			if (_border != null)
				_border.Position = this.Bounds.CenterVector();
		}

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
						 bool drawIfDisabled = true,
						 bool enabledTooltip = false,
						 bool enabledContextMenu = false)
		{
			_bounds = bounds ?? Rectangle.Empty;
			_activator = new ActivationManager();
			_readyDisableOnEscape = readyDisableOnEscape;
			_spriteBatchData = spriteBatchData;
			_hasBorder = hasBorder;
			_delayInputCycles = 0;
			_mouseover = false;
			_backgroundName = background;
			_fireMouseEvents = fireMouseEvents;
			_inactiveMouseEvents = inactiveMouseEvents;
			_drawIfDisabled = drawIfDisabled;
			if (_enabledTooltip = enabledTooltip)
				_activator.Register(_tooltip = new Tooltip(this, SpriteBatchManager.Get("tooltip")), false, "popup");
			if (_enabledContextMenu = enabledContextMenu)
			{
				_activator.Register(_contextMenu = new ContextMenu(this, SpriteBatchManager.Get("context")), false, "popup");
				_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
			}

			if (!String.IsNullOrWhiteSpace(_backgroundName))
				_background = AssetManager.GetBackground(_backgroundName);

			BoundsChanged(true);
		}

		public virtual void LoadContent()
		{
			_tooltip?.LoadContent();
			_contextMenu?.LoadContent();
		}

		public virtual void UnloadContent()
		{
			_border?.UnloadContent();
			_tooltip?.UnloadContent();
			_contextMenu?.UnloadContent();
			_dialog?.UnloadContent();
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
			_dialog?.Update(gameTime);
			_background?.Update(gameTime);
			_border?.Update(gameTime);
			_tooltip?.Update(gameTime);
			_contextMenu?.Update(gameTime);
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
				if (InputManager.LeftMouseClick())
					MouseLeftClick(new ComponentEventArgs { Button = MouseButton.Left });
				if (InputManager.RightMouseClick())
					MouseRightClick(new ComponentEventArgs { Button = MouseButton.Right });
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
			_contextMenu?.Draw(spriteBatch);
			_dialog?.Draw(spriteBatch);
		}

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		protected void SetupBorder()
		{
			if (_hasBorder && (this.Bounds != Rectangle.Empty))
			{
				_border?.UnloadContent();
				_border = Util.GenerateBorderTexture(this.Bounds.Width, this.Bounds.Height, this.BorderThickness, this.BorderColor, true);
			}
		}

		private void _contextMenu_OnItemSelect(object sender, ComponentEventArgs e) => ContextMenuSelect(e);

		protected virtual void ContextMenuSelect(ComponentEventArgs e) { }

		public virtual List<string> GetContextMenuOptions() => new List<string>();

		protected void ShowNotification(string text, Rectangle parentBounds, string group = null, int? duration = null)
		{
			if (_dialog == null)
			{
				_activator.Register(_dialog = new Dialog(text, DialogButton.Ok, parentBounds.CenteredRegion(400, 200), duration), true, group);
				_dialog.LoadContent();
				_dialog.OnItemSelect += _dialog_OnItemSelect;
				_dialog.OnReadyDisable += _dialog_OnReadyDisable;
				return;
			}

			// Need to test that this works....
			_dialog.Text = text;
			_dialog.Bounds = parentBounds.CenteredRegion(400, 200);
			_dialog.Duration = duration;
			_activator.SetState(_dialog, true);
		}

		protected virtual void _dialog_OnItemSelect(object sender, ComponentEventArgs e) { }

		protected virtual void _dialog_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_activator.SetState(_dialog, false);
		}

		#region Events
		protected virtual void ReadyDisable(ComponentEventArgs e) => _onReadyDisable?.Invoke(this, e);
		private event EventHandler<ComponentEventArgs> _onReadyDisable;
		public event EventHandler<ComponentEventArgs> OnReadyDisable
		{
			add		{ lock(_lock) { _onReadyDisable -= value; _onReadyDisable += value; } }
			remove	{ lock(_lock) { _onReadyDisable -= value; } }
		}

		protected virtual void MouseOver(ComponentEventArgs e) => _onMouseOver?.Invoke(this, e);
		private event EventHandler<ComponentEventArgs> _onMouseOver;
		public event EventHandler<ComponentEventArgs> OnMouseOver
		{
			add		{ lock(_lock) { _onMouseOver -= value; _onMouseOver += value; } }
			remove	{ lock(_lock) { _onMouseOver -= value; } }
		}


		protected virtual void MouseIn(ComponentEventArgs e) => _onMouseIn?.Invoke(this, e);
		private event EventHandler<ComponentEventArgs> _onMouseIn;
		public event EventHandler<ComponentEventArgs> OnMouseIn
		{
			add		{ lock(_lock) { _onMouseIn -= value; _onMouseIn += value; } }
			remove	{ lock(_lock) { _onMouseIn -= value; } }
		}


		protected virtual void MouseOut(ComponentEventArgs e) => _onMouseOut?.Invoke(this, e);
		private event EventHandler<ComponentEventArgs> _onMouseOut;
		public event EventHandler<ComponentEventArgs> OnMouseOut
		{
			add		{ lock(_lock) { _onMouseOut -= value; _onMouseOut += value; } }
			remove	{ lock(_lock) { _onMouseOut -= value; } }
		}


		protected virtual void MouseLeftClick(ComponentEventArgs e) => _onMouseLeftClick?.Invoke(this, e);
		private event EventHandler<ComponentEventArgs> _onMouseLeftClick;
		public event EventHandler<ComponentEventArgs> OnMouseLeftClick
		{
			add		{ lock(_lock) { _onMouseLeftClick -= value; _onMouseLeftClick += value; } }
			remove	{ lock(_lock) { _onMouseLeftClick -= value; } }
		}

		protected virtual void MouseRightClick(ComponentEventArgs e) => _onMouseRightClick?.Invoke(this, e);
		private event EventHandler<ComponentEventArgs> _onMouseRightClick;
		public event EventHandler<ComponentEventArgs> OnMouseRightClick
		{
			add		{ lock(_lock) { _onMouseRightClick -= value; _onMouseRightClick += value; } }
			remove	{ lock(_lock) { _onMouseRightClick -= value; } }
		}
		#endregion
	}
}
