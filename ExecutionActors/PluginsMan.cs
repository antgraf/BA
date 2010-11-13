using System;
using System.Collections.Generic;
using System.Linq;
using BACommon;
using System.IO;
using System.Reflection;

namespace ExecutionActors
{
	public static class PluginsMan
	{
		private const string pluginsMask = "*.bap";
		private const string pluginsPathRelative = "Plugins";

		private static readonly List<PluginBase> pPlugins = new List<PluginBase>();

		public static PluginBase GetPluginByName(string name)
		{
			return pPlugins.FirstOrDefault(plugin => plugin.Name == name);
		}

		public static PluginBase CreateMainClass(Assembly plugin)
		{
			Type[] types = plugin.GetTypes();

			return (from t in types where t.BaseType == typeof (PluginBase) select (PluginBase) plugin.CreateInstance(t.ToString())).FirstOrDefault();
		}

		public static int ReadAllPlugins()
		{
			string path = FileUtils.Relative2AbsolutePath(pluginsPathRelative);
			return ReadAllPlugins(path);
		}

		public static int ReadAllPlugins(string path)
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			FileInfo[] files = dir.GetFiles(pluginsMask);
			pPlugins.Clear();

			foreach(FileInfo file in files)
			{
				PluginBase mainclass = null;
				try
				{
					Assembly plugin = Assembly.LoadFrom(file.FullName);
					mainclass = CreateMainClass(plugin);
				}
// ReSharper disable EmptyGeneralCatchClause
				catch
				{
					// ignore
				}
				if(mainclass != null)
				{
					pPlugins.Add(mainclass);
				}
			}

			return pPlugins.Count;
		}

		public static PluginBase[] Plugins
		{
			get { return pPlugins.ToArray(); }
		}
	}
}
