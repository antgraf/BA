﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACommon;
using System.IO;

namespace ExecutionActors
{
	public abstract class PluginBase : IActorObserver
	{
		private const string pSettingsFileExtension = ".xml";

		protected Settings pSettings = null;
		protected string pPluginName = null;
		protected string pAbsoluteSettingsFolder = null;
		protected string pRelativeSettingsFolder = null;
		protected Actor pActor = null;
		protected IPluginObserver pObserver = null;

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

		public void Init(IPluginObserver observer)
		{
			pObserver = observer;
		}

		public virtual void Run()
		{
			pActor.Run();
		}

		public virtual void Pause()
		{
			pActor.Pause();
		}

		public virtual void Continue()
		{
			pActor.Continue();
		}

		public virtual void Exit()
		{
			pActor.Exit();
		}

		public virtual void GoToFinalState()
		{
			pActor.GoToFinalState();
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

		#region IActorObserver Members

		public void Notify(Actor actor, string stage, int percentage, string message)
		{
			if(pObserver != null)
			{
				pObserver.Notify(this, stage, percentage, message);
			}
		}

		public void Notify(Actor actor, string stage, int percentage)
		{
			if(pObserver != null)
			{
				pObserver.Notify(this, stage, percentage);
			}
		}

		public void Notify(Actor actor, string message)
		{
			if(pObserver != null)
			{
				pObserver.Notify(this, message);
			}
		}

		#endregion

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
