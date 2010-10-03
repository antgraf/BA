using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACommon;
using System.IO;
using System.Reflection;

namespace ExecutionActors
{
	public static class PluginsMan
	{
		private const string PluginsMask = "*.bap";
		private const string PluginsPathRelative = "Plugins";

		private static List<PluginBase> pPlugins = new List<PluginBase>();

		public static PluginBase GetPluginByName(string name)
		{
			foreach(PluginBase plugin in pPlugins)
			{
				if(plugin.Name == name)
				{
					return plugin;
				}
			}
			return null;
		}

		public static PluginBase CreateMainClass(Assembly plugin)
		{
			Type[] types = plugin.GetTypes();

			foreach(Type t in types)
			{
				if(t.BaseType == typeof(PluginBase))
				{
					return (PluginBase)plugin.CreateInstance(t.ToString());
				}
			}

			return null;
		}

		public static int ReadAllPlugins()
		{
			string path = FileUtils.Relative2AbsolutePath(PluginsPathRelative);
			return ReadAllPlugins(path);
		}

		public static int ReadAllPlugins(string path)
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			FileInfo[] files = dir.GetFiles(PluginsMask);
			pPlugins.Clear();

			foreach(FileInfo file in files)
			{
				PluginBase mainclass = null;
				try
				{
					Assembly plugin = Assembly.LoadFrom(file.FullName);
					mainclass = CreateMainClass(plugin);
				}
				catch(Exception)
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
