using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Interface;
using Game1.Items;

namespace Game1
{
	public static class InputManager
	{
		private static Texture2D _pointer;

		private readonly static Dictionary<Keys, (char, char)> KeyMap = new Dictionary<Keys, (char, char)>
		{
			{ Keys.A, ('a', 'A') },
			{ Keys.B, ('b', 'B') },
			{ Keys.C, ('c', 'C') },
			{ Keys.D, ('d', 'D') },
			{ Keys.E, ('e', 'E') },
			{ Keys.F, ('f', 'F') },
			{ Keys.G, ('g', 'G') },
			{ Keys.H, ('h', 'H') },
			{ Keys.I, ('i', 'I') },
			{ Keys.J, ('j', 'J') },
			{ Keys.K, ('k', 'K') },
			{ Keys.L, ('l', 'L') },
			{ Keys.M, ('m', 'M') },
			{ Keys.N, ('n', 'N') },
			{ Keys.O, ('o', 'O') },
			{ Keys.P, ('p', 'P') },
			{ Keys.Q, ('q', 'Q') },
			{ Keys.R, ('r', 'R') },
			{ Keys.S, ('s', 'S') },
			{ Keys.T, ('t', 'T') },
			{ Keys.U, ('u', 'U') },
			{ Keys.V, ('v', 'V') },
			{ Keys.W, ('w', 'W') },
			{ Keys.X, ('x', 'X') },
			{ Keys.Y, ('y', 'Y') },
			{ Keys.Z, ('z', 'Z') },
			{ Keys.D1, ('1', '1') },
			{ Keys.D2, ('2', '2') },
			{ Keys.D3, ('3', '3') },
			{ Keys.D4, ('4', '4') },
			{ Keys.D5, ('5', '5') },
			{ Keys.D6, ('6', '6') },
			{ Keys.D7, ('7', '7') },
			{ Keys.D8, ('8', '8') },
			{ Keys.D9, ('9', '9') },
			{ Keys.D0, ('0', '0') },
			{ Keys.NumPad1 , ('1', '1') },
			{ Keys.NumPad2, ('2', '2') },
			{ Keys.NumPad3, ('3', '3') },
			{ Keys.NumPad4, ('4', '4') },
			{ Keys.NumPad5, ('5', '5') },
			{ Keys.NumPad6, ('6', '6') },
			{ Keys.NumPad7, ('7', '7') },
			{ Keys.NumPad8, ('8', '8') },
			{ Keys.NumPad9, ('9', '9') },
			{ Keys.NumPad0, ('0', '0') },
			{ Keys.Space, (' ', ' ') },
			{ Keys.OemPeriod, ('.', '.') },
			{ Keys.OemComma, (',', ',') },
			{ Keys.OemMinus, ('-', '_') }
		};

		private static KeyboardState _currentKeyState;
		private static KeyboardState _prevKeyState;
		private static MouseState _currentMouseState;
		private static MouseState _prevMouseState;

		static InputManager()
		{
			_pointer = AssetManager.GetInterfaceElement("pointer");
		}

		public static void Update()
		{
			excludedKeys.Clear();
			_blockAllInput = false;
			_prevKeyState = _currentKeyState;
			_currentKeyState = Keyboard.GetState();
			_prevMouseState = _currentMouseState;
			_currentMouseState = Mouse.GetState();
		}

		private static List<Keys> excludedKeys = new List<Keys>();
		private static bool _blockAllInput;

		public static bool KeyPressed(Keys key, bool clearAfterMatch = false)
		{
			if (_blockAllInput)
				return false;

			if (excludedKeys.Contains(key))
				return false;

			bool result = KeyPressed(new[] { key });
			if (result && clearAfterMatch)
				BlockKey(key);

			return result;
		}

		public static void BlockKey(Keys key)
		{
			excludedKeys.Add(key);
		}

		public static bool KeyPressed(Keys[] keys) => (!_blockAllInput) &&  keys.Any(k => _currentKeyState.IsKeyDown(k) && _prevKeyState.IsKeyUp(k));

		public static bool KeyReleased(Keys key) => KeyReleased(new[] { key });

		public static bool KeyReleased(Keys[] keys) => (!_blockAllInput) && keys.Any(k => _currentKeyState.IsKeyUp(k) && _prevKeyState.IsKeyDown(k));

		public static bool KeyDown(Keys key) => KeyDown(new[] { key });

		public static bool KeyDown(Keys[] keys) => (!_blockAllInput) && keys.Any(k => _currentKeyState.IsKeyDown(k));	

		public static bool CapsLock => _currentKeyState.CapsLock;

		public static List<Keys> GetPressedKeys()
		{
			List<Keys> keysPressed = new List<Keys>();
			if (_blockAllInput)
				return keysPressed;

			foreach (var key in _currentKeyState.GetPressedKeys().Where(k => !excludedKeys.Contains(k)))
				if (_prevKeyState.IsKeyUp(key))
					keysPressed.Add(key);

			return keysPressed;			
		}

		public static char KeyToChar(Keys key, bool isUpper)
		{
			if (!InputManager.KeyMap.TryGetValue(key, out (char lower, char upper) values))
				return '\0';

			return (isUpper ? values.upper : values.lower);
		}

		public static Point MousePosition => _blockAllInput ? new Point(-1, -1) : _currentMouseState.Position;

		public static bool MouseOver(Rectangle bounds) => (!_blockAllInput) && bounds.Contains(_currentMouseState.Position);

		public static bool LeftMouseClick() => (!_blockAllInput) && (_currentMouseState.LeftButton == ButtonState.Pressed) && (_prevMouseState.LeftButton == ButtonState.Released);

		public static bool LeftMouseClick(Rectangle bounds) => MouseOver(bounds) && LeftMouseClick();

		public static bool RightMouseClick() => (!_blockAllInput) &&  (_currentMouseState.RightButton == ButtonState.Pressed) && (_prevMouseState.RightButton == ButtonState.Released);

		public static bool RightMouseClick(Rectangle bounds) => MouseOver(bounds) && RightMouseClick();

		public static int MouseScrollAmount => (_blockAllInput ? 0 : _currentMouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue);

		public static void HandleCursorChange(object sender, ComponentEventArgs e)
		{
			var heldItem = ((InventoryItem)e.Meta);
			if (heldItem != null)
				SetMouseCursor(heldItem.Icon.Texture, (heldItem.Quantity > 1) ? heldItem.Quantity : (int?)null);
			else
				SetMouseCursor(null);
		}

		public static void SetMouseCursor(Texture2D texture, int? quantity = null)
		{
			if (texture == null)
			{
				ResetMouseCursor();
				return;
			}

			Mouse.SetCursor(MouseCursor.FromTexture2D(GenerateMouseCursorTexture(texture, quantity), 0, 0));
		}

		private static Texture2D GenerateMouseCursorTexture(Texture2D texture, int? quantity = null)
		{
			return AddTextAndPointer(RBSwappedTexture(texture), quantity);
		}

		private static Texture2D AddTextAndPointer(Texture2D texture, int? quantity)
		{
			// For non-square textures, they get all willy-wonka when converted to mouse cursor, so instead we always use full size icon target 
			// and just offset the drawing of the icon itself if it's smaller than the standard size (64x64) 
			var renderTarget = new RenderTarget2D(Game1.Graphics, Game1.IconSize, Game1.IconSize);
			Game1.Graphics.SetRenderTarget(renderTarget);
			Game1.Graphics.Clear(Color.Transparent);
			var spriteBatch = new SpriteBatch(Game1.Graphics);
			spriteBatch.Begin();
			spriteBatch.Draw(texture, Vector2.Zero.Offset((Game1.IconSize - texture.Width) / 2, (Game1.IconSize - texture.Height) / 2), Color.White);
			spriteBatch.Draw(_pointer, Vector2.Zero, Color.White);
			if (quantity != null)
				spriteBatch.DrawString(FontManager.Get(), ((int)quantity).ToString(), new Vector2(Game1.IconSize - 25, Game1.IconSize - 20), Color.White);
			spriteBatch.End();
			var newTexture = renderTarget;
			Game1.Graphics.SetRenderTarget(null);
			return newTexture;
		}

		private static Texture2D RBSwappedTexture(Texture2D textureOriginal)
		{
			var data = new Color[textureOriginal.Width * textureOriginal.Height];
			textureOriginal.GetData(data);

			// Due to bug in Monogame, we have to swap the red and blue channels when creating a texture for use as a mouse cursor....
			for (int i = 0; i < data.Length; i++)
			{
				byte red = data[i].R;
				data[i].R = data[i].B;
				data[i].B = red;
			}

			var reversedTexture = new Texture2D(Game1.Graphics, textureOriginal.Width, textureOriginal.Height);
			reversedTexture.SetData(data);
			return reversedTexture;
		}

		private static void ResetMouseCursor()
		{
			Mouse.SetCursor(MouseCursor.Arrow);
		}

		public static void BlockAllInput()
		{
			_blockAllInput = true;
		}
	}
}
