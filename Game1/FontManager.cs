using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;
using Game1.Enum;

namespace Game1
{
	// Eventually this will handle multiple fonts, etc...
	public static class FontManager
	{		
		public const string DefaultFontName = "Orbitron";

		/// <summary>
		/// At game start, we should load up the font and measure a sample string to get this value "for real"...
		/// </summary>
		public static int FontHeight = 24;

		private static ContentManager _content;
		private static readonly Dictionary<string, SpriteFont> _fonts;

		static FontManager()
		{
			_fonts = new Dictionary<string, SpriteFont>();
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
		}

		public static void LoadContent()
		{
			_fonts[FontManager.DefaultFontName] = _content.Load<SpriteFont>($"{Game1.FontsRoot}/{FontManager.DefaultFontName}");
		}

		public static void UnloadContent()
		{
			_content.Unload();
		}

		public static SpriteFont Get(string font = FontManager.DefaultFontName)
		{
			return _fonts[font];
		}

		public static Vector2 MeasureString(string text, float scale = 1.0f, string font = FontManager.DefaultFontName)
		{
			return _fonts[font].MeasureString(text) * scale;
		}
	}
}
