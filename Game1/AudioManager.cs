using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Game1.Effect;
using Game1.Enum;
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;

namespace Game1
{
	// Obviously this is just temp to start something...
	public static class AudioManager
	{
		private static ContentManager _content;
		private static Song _song;

		static AudioManager()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
		}

		public static void LoadContent()
		{
			_song = _content.Load<Song>(Path.Combine(Game1.AudioRoot, "overworld"));
		}

		public static void UnloadContent()
		{
			_content.Unload();
		}
	
		public static void Start()
		{
			MediaPlayer.Volume = 0.2f;
			MediaPlayer.Play(_song);
		}
	}
}
