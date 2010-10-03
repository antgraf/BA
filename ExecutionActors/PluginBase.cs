using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACommon;
using System.IO;

namespace ExecutionActors
{
	public abstract class PluginBase
	{
		private const string pSettingsFileExtension = ".xml";

		protected Settings pSettings = null;
		protected string pPluginName = null;
		protected string pAbsoluteSettingsFolder = null;
		protected string pRelativeSettingsFolder = null;
		protected Actor pActor = null;

		private string GetDefaultSettingsName()
		{
			string filename = this.GetType().ToString() + pSettingsFileExtension;
			return FileUtils.MakeValidFileName(filename);
		}

		private string GetSettingsFileName()
		{
			string filename = null;
			if(pPluginName != null)
			{
				filename = pPluginName + pSettingsFileExtension;
			} 
			else
			{
				filename = GetDefaultSettingsName();
			}
			filename = FileUtils.MakeValidFileName(filename);
			if(pRelativeSettingsFolder != null)
			{
				filename = FileUtils.CombineWinPath(pRelativeSettingsFolder, filename);
			}
			if(pAbsoluteSettingsFolder != null)
			{
				filename = FileUtils.CombineWinPath(pAbsoluteSettingsFolder, filename);
			}
			else
			{
				filename = FileUtils.Relative2AbsolutePath(filename);
			}
			return filename;
		}

		public void Run()
		{
			pActor.Run();
		}

		public void SaveSettings()
		{
			string xml = pSettings.XmlSerialize();
			FileUtils.WriteFileContentUtf8(GetSettingsFileName(), xml);
		}

		public void LoadSettings()
		{
			pSettings = null;
			string xml = FileUtils.ReadFileContentUtf8(GetSettingsFileName());
			if(!StringUtils.IsEmpty(xml))
			{
				pSettings = Settings.XmlDeserialize(xml);
			}
			if(pSettings == null)
			{
				pSettings = new Settings();
			}
		}

		public abstract void ShowUI();

		public string Name
		{
			get { return pPluginName; }
		}

		public Settings Settings
		{
			get { return pSettings; }
		}
	}
}
