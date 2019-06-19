using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
using Game1.Enum;
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;

namespace Game1
{
	public class NewItemNotificationViewer : Component
	{
		public const int MaxVisibleNotifications = 10;
		public const int NotificationDuration = 600;
		public const int NotificationPadding = 10;

		private FixedQueue<NewItemNotification> _notifications;

		public NewItemNotificationViewer(Rectangle bounds) : base(bounds, background: null, fireMouseEvents: false)
		{ 
			_notifications = new FixedQueue<NewItemNotification>(MaxVisibleNotifications);
		}

		public void AddNotification(InventoryItem item)
		{
			if (item == null)
				return;

			var notification = new NewItemNotification(item, NotificationDuration);
			notification.LoadContent();
			notification.OnReadyDisable += Notification_OnReadyDisable;
			var oldItem = _notifications.Enqueue(notification);
			if (oldItem != null)
			{
				oldItem.UnloadContent();
				oldItem.IsActive = false;
			}
		}

		private void Notification_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			((NewItemNotification)sender).IsActive = false;
		}

		public override void Update(GameTime gameTime)
		{
			_notifications = new FixedQueue<NewItemNotification>(_notifications.Where(x => x.IsActive), MaxVisibleNotifications);

			int startPosX = this.Bounds.Left + this.ContentMargin.Width;
			int startPosY = this.Bounds.Bottom - this.ContentMargin.Height - NewItemNotification.Size.Height;

			foreach (var notification in _notifications.Reverse())
			{
				notification.UpdatePosition(new Point(startPosX, startPosY));
				startPosY -= (NotificationPadding + NewItemNotification.Size.Height);
				notification.Update(gameTime);
			}
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			foreach (var notification in _notifications)
				notification.Draw(spriteBatch);
		}
	}
}
