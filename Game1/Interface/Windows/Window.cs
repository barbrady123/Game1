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
		public virtual Size ContentMargin => new Size(20, this.Title == null ? 20 : 60);
		protected ImageText _titleImage;
		private string _title;
		private bool _isActive;
		private int _delayInputCycles;
		private bool _readyDisableOnEscape;
		private bool _resetMouseOnInactive;

		public virtual string SpriteBatchName => "modal";
		public virtual int TitleOffset => 30;

		public int? Duration { get; set; }

		public bool IsActive
		{
			get { return _isActive; }
			set {
				if (_isActive != value)
				{
					_isActive = value;
					if (_isActive)
					{
						DelayInput(1);
						OnBecomeActive();
					}
					else
					{
						this.Duration = null;
						if (_resetMouseOnInactive)
							InputManager.ResetMouseCursor();
					}
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

		public override void RepositionScreenObjects()
		{
			base.RepositionScreenObjects();
			if (_titleImage != null)
				_titleImage.Position = this.Bounds.TopCenterVector(yOffset: this.TitleOffset);
		}

		public Vector2 TitleSize => _titleImage?.Size ?? Vector2.Zero;

		public event EventHandler OnButtonClick;
		public event EventHandler OnReadyDisable;

		public Window(Rectangle bounds,
					  string backgroundName,
					  string title,
					  int? duration = null,
					  bool readyDisableOnEscape = true,
					  bool resetMouseOnInactive = false) : base(bounds, backgroundName)
		{
			_title = title ?? "";
			this.IsActive = false;
			this.Duration = duration;
			_titleImage = new ImageText(_title, true);
			_readyDisableOnEscape = readyDisableOnEscape;
			_resetMouseOnInactive = resetMouseOnInactive;
			_delayInputCycles = 0;
			RepositionScreenObjects();
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

		/// <summary>
		/// Main update method...
		/// </summary>
		public override void Update(GameTime gameTime, bool processInput)
		{
			if (!this.IsActive)
				return;

			if (this.Duration != null)
			{
				if (this.Duration <= 0)
				{
					BeforeReadyDisable(new ScreenEventArgs("timer", this.GetType().Name, null));
					this.Duration = null;
				}
				else
				{
					this.Duration--;
				}
			}

			UpdateActive(gameTime, processInput);
		}

		/// <summary>
		/// Called if IsActive and Duration timer passed...
		/// </summary>
		public virtual void UpdateActive(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_titleImage.Update(gameTime);
			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			if (!processInput)
				return;

			UpdateReady(gameTime);
		}

		/// <summary>
		/// Called after input delay expired, if processInput is true...
		/// </summary>
		public virtual void UpdateReady(GameTime gameTime)
		{
			if (_readyDisableOnEscape)
			{
				if (InputManager.KeyPressed(Keys.Escape, true))
					BeforeReadyDisable(new ScreenEventArgs("escape", this.GetType().Name, null));
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!this.IsActive)
				return;

			var modalBatch = Util.WrappedDraw(base.Draw, this.SpriteBatchName, this.Bounds);
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

		protected virtual void BeforeReadyDisable(ScreenEventArgs args)
		{
			OnReadyDisable?.Invoke(this, args);
		}

		protected virtual void OnBecomeActive() { }
	}
}
