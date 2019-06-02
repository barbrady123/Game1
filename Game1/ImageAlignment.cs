using Game1.Enum;

namespace Game1
{
	public struct ImageAlignment
	{
		public HorizontalAlignment Horizatal { get; set; }

		public VerticalAlignment Vertical { get; set; }

		public static ImageAlignment Centered => new ImageAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);

		public static ImageAlignment CenteredTop => new ImageAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);

		public static ImageAlignment LeftTop => new ImageAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);

		public static ImageAlignment LeftBottom => new ImageAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);

		public static ImageAlignment LeftCentered => new ImageAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);

		public static ImageAlignment RightCentered => new ImageAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);

		public static ImageAlignment RightBottom => new ImageAlignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);

		public ImageAlignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			this.Horizatal = horizontal;
			this.Vertical = vertical;
		}
	}
}
