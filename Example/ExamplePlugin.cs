using ExecutionActors;
using System.Windows.Forms;

namespace Example
{
	public class ExamplePlugin : PluginBase
	{
		public ExamplePlugin()
		{
			pPluginName = "Example BA plug-in";
			pActor = ActorsMan.NewActor(typeof(ExampleActor), this);
		}

		public override void ShowUI()
		{
			if(MessageBox.Show("Run automation?", "Example BA plug-in", MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Run();
			}
		}
	}
}
