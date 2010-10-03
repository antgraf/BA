using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExecutionActors.Tests
{
	internal class TestPlugin : PluginBase
	{
		public TestPlugin()
		{
			pAbsoluteSettingsFolder = @"C:\Temp";
			pActor = new TestActor();
		}

		public override void ShowUI()
		{
			TestPluginForm form = new TestPluginForm();
			form.Show();
		}
	}

	[TestFixture]
	public class PluginBaseTest
	{
		[Test]
		public void Create()
		{
			PluginBase pb = new TestPlugin();
			Assert.NotNull(pb);
		}

		[Test]
		public void Settings()
		{
			PluginBase pb = new TestPlugin();
			Assert.NotNull(pb);
			pb.LoadSettings();
			pb.Settings.Clear();
			Assert.AreEqual(pb.Settings.Count, 0);
			pb.Settings.Add("test setting", 1111);
			Assert.AreEqual(pb.Settings.Count, 1);
			pb.SaveSettings();
			pb.Settings.Clear();
			Assert.AreEqual(pb.Settings.Count, 0);
			pb.LoadSettings();
			Assert.AreEqual(pb.Settings.Count, 1);
			Assert.AreEqual(pb.Settings["test setting"], 1111);
		}

		[Test]
		public void Run()
		{
			PluginBase pb = new TestPlugin();
			Assert.NotNull(pb);
			pb.Run();
		}
	}
}
