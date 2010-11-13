using ExecutionActors;
using WindowEntity;

namespace Example
{
	public class ExampleActor : Actor
	{
		protected override void Worker()
		{
			pObserver.Notify(this, "Start", 100, "Example actor started");
			pObserver.Notify(this, "Run", 0, "Open Run form");
			Desktop.Primary.WinKeyDown();
			Desktop.Primary.KeySendAndWait("r");
			Desktop.Primary.WinKeyUp();
			pObserver.Notify(this, "Run", 33, "Run calculator application");
			Desktop.Primary.KeySendAndWait("calc~");
			pObserver.Notify(this, "Run", 67, "Waiting for window");
			Window.WaitGlobal(500);
			Window calc = WindowsMan.AttachTo("calc");
			pObserver.Notify(this, "Run", 100, "Calculator's window found");
			pObserver.Notify(this, "Calculation", 0, "Let's calculate 2*2");
			calc.KeySendAndWait("2*2~");
			pObserver.Notify(this, "Calculation", 100, "Ready");
			Window.WaitGlobal(2000);
			pObserver.Notify(this, "Finish", 0, "Closing window");
			calc.Close();
			pObserver.Notify(this, "Finish", 100, "Example actor finished");
		}

		protected override void Init(object data)
		{
			pObserver.Notify(this, "Initialization", 100, "Example actor initialized");
		}
	}
}
