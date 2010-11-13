using System.Windows.Forms;
using System.Drawing;

namespace WindowEntity
{
	public class Desktop : StaticWindow
	{
		private static readonly Desktop pInstance = new Desktop() { X = 0, Y = 0 };

		private Desktop()
		{}

		private static void Update()
		{
			Rectangle screen = Screen.PrimaryScreen.Bounds;
			pInstance.Width = screen.Width;
			pInstance.Height = screen.Height;
		}

		public static Desktop Primary
		{
			get
			{
				Update();
				return pInstance;
			}
		}
	}
}
