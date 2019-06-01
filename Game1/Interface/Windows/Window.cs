using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface.Windows
{
	public class Window : Screen, IActivatable
	{
		public Size ContentMargin => new Size(20, String.IsNullOrWhiteSpace(this.Title) ? 20 : 60);
		private ImageText _titleImage;
		private string _title;
		private bool _isActive;
		private int _delayInputCycles;
		private bool _readyDisableOnEscape;

		public int? Duration { get; set; }

		public bool IsActive
		{
			get { return _isActive; }
			set {
				if (_isActive != value)
				{
					_isActive = value;
					if (_isActive)
						DelayInput(1);
					else
						this.Duration = null;
				}
			}
		}

		public string Title
		{ 
			get { return _title; }
			set {
				string val = value ?? "";
				if (_title != val)
				{
					_title = val;
					_titleImage?.UpdateText(_title);					
				}
			}
		}

		public event EventHandler OnButtonClick;
		public event EventHandler OnReadyDisable;

		public Window(Rectangle bounds, string backgroundName, string title, int? duration = null, bool readyDisableOnEscape = true) : base(bounds, backgroundName)
		{
			_title = title ?? "";
			this.IsActive = false;
			this.Bounds = bounds;
			this.Duration = duration;
			_titleImage = new ImageText(_title, true) { Position = this.Bounds.CenterVector(yOffset: - this.Bounds.Height / 2 + 30) };
			_readyDisableOnEscape = readyDisableOnEscape;
			_delayInputCycles = 0;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_titleImage.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleImage.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			if (!this.IsActive)
				return;

			if (this.Duration != null)
			{
				if (this.Duration <= 0)
				{
					OnReadyDisable?.Invoke(this, new ScreenEventArgs("timer", this.GetType().Name, null));
					this.Duration = null;
				}
				else
				{
					this.Duration--;
				}
			}

			UpdateActive(gameTime, processInput);
		}

		public virtual void UpdateActive(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_titleImage.Update(gameTime);
			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			UpdateReady(gameTime, processInput);
		}

		public virtual void UpdateReady(GameTime gameTime, bool processInput)
		{
			if (_readyDisableOnEscape)
			{
				if (InputManager.KeyPressed(Keys.Escape, true))
					OnReadyDisable?.Invoke(this, new ScreenEventArgs("escape", this.GetType().Name, null));
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!this.IsActive)
				return;

			var modalBatch = Util.WrappedDraw(base.Draw, "modal", this.Bounds);
			DrawInternal(modalBatch);
		}

		public virtual void DrawInternal(SpriteBatch spriteBatch) 
		{
			_titleImage.Draw(spriteBatch);
		}

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		protected void ButtonClick(MenuEventArgs menuArgs)
		{
			OnButtonClick?.Invoke(this, new ScreenEventArgs(menuArgs.Type, this.GetType().Name, menuArgs.Item));
		}
	}
}
