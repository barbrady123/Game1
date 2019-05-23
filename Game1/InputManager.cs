using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
	public class InputManager
	{
		private KeyboardState _currentKeyState;
		private KeyboardState _prevKeyState;

		private static readonly InputManager _instance = new InputManager();

		private InputManager() { }

		public static InputManager Instance => _instance;

		public void Update()
		{
			_prevKeyState = _currentKeyState;
			_currentKeyState = Keyboard.GetState();
		}

		public bool KeyPressed(params Keys[] keys) => keys.Any(k => _currentKeyState.IsKeyDown(k) && _prevKeyState.IsKeyUp(k));

		public bool KeyReleased(params Keys[] keys) => keys.Any(k => _currentKeyState.IsKeyUp(k) && _prevKeyState.IsKeyDown(k));

		public bool KeyDown(params Keys[] keys) => keys.Any(k => _currentKeyState.IsKeyDown(k));	
	}
}
