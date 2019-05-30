using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
	public static class InputManager
	{
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

		public static void Update()
		{
			_prevKeyState = _currentKeyState;
			_currentKeyState = Keyboard.GetState();			
		}

		// TODO: Verify that we still need this...
		public static bool KeyPressed(Keys key, bool clearAfterMatch = false)
		{
			bool result = KeyPressed(new[] { key });
			if (clearAfterMatch)
				_currentKeyState = new KeyboardState(_currentKeyState.GetPressedKeys().Where(k => k != key).ToArray(), _currentKeyState.CapsLock, _currentKeyState.NumLock);

			return result;
		}

		public static bool KeyPressed(Keys[] keys) => keys.Any(k => _currentKeyState.IsKeyDown(k) && _prevKeyState.IsKeyUp(k));

		public static bool KeyReleased(Keys key) => KeyReleased(new[] { key });

		public static bool KeyReleased(Keys[] keys) => keys.Any(k => _currentKeyState.IsKeyUp(k) && _prevKeyState.IsKeyDown(k));

		public static bool KeyDown(Keys key) => KeyDown(new[] { key });

		public static bool KeyDown(Keys[] keys) => keys.Any(k => _currentKeyState.IsKeyDown(k));	

		public static bool CapsLock => _currentKeyState.CapsLock;

		public static List<Keys> GetPressedKeys()
		{
			List<Keys> keysPressed = new List<Keys>();
			foreach (var key in _currentKeyState.GetPressedKeys())
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
	}
}
