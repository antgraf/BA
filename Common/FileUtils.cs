using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// ReSharper disable CheckNamespace
namespace BACommon
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// Utility class for file I/O manipulations.
	/// </summary>
	public static class FileUtils
	{
		/// <summary>
		/// Forward slash symbol.
		/// </summary>
		public const char Slash = '/';

		/// <summary>
		/// Back slash symbol.
		/// </summary>
		public const char BackSlash = '\\';
		/// <summary>
		/// Extension delimiter character
		/// </summary>
		public const string ExtensionDelimiter = ".";
		/// <summary>
		/// String contains characters which cannot be part of path or filename.
		/// </summary>
		public const string InvalidCharacters = "\\/:*?\"<>|";
		/// <summary>
		/// Character used to replace invalid ones.
		/// </summary>
		public const string ReplaceCharacter = "_";

		/// <summary>
		/// Checks if specified file name is valid.
		/// </summary>
		/// <param name="filename">File name or path.</param>
		/// <returns>True if file name/path is valid. False otherwise.</returns>
		public static bool ValidateFileName(string filename)
		{
			if(StringUtils.IsEmpty(filename))
			{
				return false;
			}
			int index = filename.IndexOfAny(InvalidCharacters.ToCharArray());
			return index < 0;
		}

		/// <summary>
		/// Checks if specified file name is valid.
		/// </summary>
		/// <param name="filename">File name or path.</param>
		/// <returns>Valid file name.</returns>
		public static string MakeValidFileName(string filename)
		{
			return Regex.Replace(filename, string.Format("[{0}]", InvalidCharacters), ReplaceCharacter);
		}

		/// <summary>
		/// Returns size of directory with all subdirectories.
		/// </summary>
		/// <param name="dirinfo">Directory to count size of.</param>
		/// <returns></returns>
		public static long DirSize(DirectoryInfo dirinfo)
		{
			// Add file sizes.
			FileInfo[] fis = dirinfo.GetFiles();
			long size = fis.Sum(fi => fi.Length);
			// Add subdirectory sizes.
			DirectoryInfo[] dis = dirinfo.GetDirectories();
			size += dis.Sum(di => DirSize(di));
			return size;
		}

		/// <summary>
		/// Deletes all derictory's content.
		/// </summary>
		/// <param name="path">Path to directory to purge.</param>
		public static void PurgeDirectory(string path)
		{
			try
			{
				if(Directory.Exists(path))
				{
					Directory.Delete(path, true);
				}
				Directory.CreateDirectory(path);
			}
// ReSharper disable EmptyGeneralCatchClause
			catch
			{
				// ignore
			}
		}

		/// <summary>
		/// Copy file or folder from <paramref name="sourcePath"/> to <paramref name="destinationPath"/> recursively.
		/// </summary>
		/// <param name="sourcePath">Path to source file or folder.</param>
		/// <param name="destinationPath">Path to destination folder.</param>
		/// <param name="recursive">Copy folders recursively flag.</param>
		/// <param name="cleanDestination">Clean destination folder before copying flag.</param>
		/// <param name="ignoreErrors">Ignore errors flag.</param>
		public static void Copy(string sourcePath, string destinationPath, bool recursive = true, bool cleanDestination = true, bool ignoreErrors = true)
		{
			try
			{
				if(cleanDestination)
				{
					PurgeDirectory(destinationPath);
				}
				else if(!Directory.Exists(destinationPath))
				{
					Directory.CreateDirectory(destinationPath);
				}
				string[] files;
				bool isfile = false;
				FileInfo finfo = new FileInfo(sourcePath);
				if(finfo.Exists)
				{
					isfile = true;
					files = new[] { sourcePath };
				}
				else
				{
					files = Directory.GetFiles(sourcePath);
				}
				foreach(string file in files)
				{
					try
					{
						File.Copy(file, CombinePath(destinationPath, ExtractFileName(file)), true);
					}
					catch(Exception)
					{
						// TODO: add logging
						if(!ignoreErrors) throw;
					}
				}
				if(recursive && !isfile)
				{
					string[] dirs = Directory.GetDirectories(sourcePath);
					foreach(string dir in dirs)
					{
						string dest = CombinePath(destinationPath, ExtractFileName(dir));
						Copy(dir, dest, true, false, ignoreErrors);
					}
				}
			}
// ReSharper disable RedundantCatchClause
			catch(Exception)
			{
				// TODO: add logging
				throw;
			}
// ReSharper restore RedundantCatchClause
		}

		/// <summary>
		/// Adds file path branch to root. Adds additional slash if needed.
		/// </summary>
		/// <param name="root">First part of file path.</param>
		/// <param name="directory">Second part of file path.</param>
		/// <returns>Cobined path from first and second paths.</returns>
		public static string CombinePath(string root, string directory)
		{
			string result = root;
			if(!result.EndsWith(Slash.ToString()) && !result.EndsWith(BackSlash.ToString()))
			{
				result += Slash;
			}
			return result + directory.TrimStart(Slash, BackSlash);
		}

		/// <summary>
		/// Adds file path branch to root. Adds additional backslash if needed.
		/// </summary>
		/// <param name="root">First part of file path.</param>
		/// <param name="directory">Second part of file path.</param>
		/// <returns>Cobined path from first and second paths.</returns>
		public static string CombineWinPath(string root, string directory)
		{
			string result = root;
			if(!result.EndsWith(Slash.ToString()) && !result.EndsWith(BackSlash.ToString()))
			{
				result += BackSlash;
			}
			return result + directory.TrimStart(Slash, BackSlash);
		}

		/// <summary>
		/// Gets substring after last dot in the given path
		/// </summary>
		/// <param name="path">String to parse</param>
		/// <returns>Result substring</returns>
		public static string ExtractExtension(string path)
		{
			if(path == null)
			{
				return null;
			}
			string aRes = string.Empty;
			string aFileName = ExtractFileName(path);
			int anIndex = aFileName.LastIndexOf(ExtensionDelimiter);
			if(anIndex >= 0)
			{
				aRes = aFileName.Substring(anIndex + 1);
			}
			return aRes;
		}

		/// <summary>
		/// Gets part of file path after last slash. If there is no slash in path returns whole path.
		/// </summary>
		/// <param name="path">Path to extract part from.</param>
		/// <returns>Part of file path after last slash.</returns>
		public static string ExtractFileName(string path)
		{
			int index = path.LastIndexOf(Slash);
			int index2 = path.LastIndexOf(BackSlash);
			index = index > index2 ? index : index2;
			return index >= 0 ? path.Substring(index + 1) : path;
		}

		/// <summary>
		/// Gets part of file path before last slash. If there is no slash in path returns empty string.
		/// </summary>
		/// <param name="path">Path to extract part from.</param>
		/// <returns>Part of file path before last slash.</returns>
		public static string ExtractDirectory(string path)
		{
			string target;
			int index = path.LastIndexOf(Slash);
			int index2 = path.LastIndexOf(BackSlash);
			index = index > index2 ? index : index2;
			if(index > 0)
			{
				target = path.Substring(0, index);
			}
			else if(index == 0)
			{
				return Slash.ToString();
			}
			else
			{
				target = string.Empty;
			}
			return target;
		}

		/// <summary>
		/// Removes slashes from the end of path string.
		/// </summary>
		/// <param name="path">Path to trim.</param>
		/// <returns>Path string w/o trailing slashes (back and forward). Root path ("/" or "\\") remains unchanged.</returns>
		public static string TrimLastSlashes(string path)
		{
			string result = path.Trim().TrimEnd(Slash, BackSlash);
			if(StringUtils.IsEmpty(result))
			{
				return Slash.ToString();
			}
			return result;
		}

		/// <summary>
		/// Generates absolute path by path relative to startup path.
		/// </summary>
		/// <param name="relativePath">Path relative to startup path.</param>
		/// <returns>Absolute path.</returns>
		public static string Relative2AbsolutePath(string relativePath)
		{
			return CombinePath(LocalPath, relativePath);
		}

		/// <summary>
		/// Generates relative path string.
		/// </summary>
		/// <param name="root">Root path.</param>
		/// <param name="fullPath">Full path to be converted to relative.</param>
		/// <returns>Path relative to <i>root</i>.</returns>
		public static string GetRelativePath(string root, string fullPath)
		{
			if(root == null || fullPath == null)
			{
				return null;
			}
			root = root.Replace(BackSlash, Slash).TrimEnd(Slash).Replace("//", "/");
			fullPath = fullPath.Replace(BackSlash, Slash).TrimEnd(Slash).Replace("//", "/");
			return !fullPath.StartsWith(root) ? null : fullPath.Substring(root.Length, fullPath.Length - root.Length).TrimStart(Slash, BackSlash);
		}

		/// <summary>
		/// Reads whole file into a bytes array.
		/// </summary>
		/// <param name="path">Path to file to read.</param>
		/// <returns>File content.</returns>
		public static byte[] ReadFile(string path)
		{
			byte[] content = null;
			if(File.Exists(path))
			{
				try
				{
					using(FileStream file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						content = new byte[file.Length];
						file.Read(content, 0, (int)file.Length);
					}
				}
				catch(Exception)
				{
					// ignore
				}
			}
			return content;
		}

		/// <summary>
		/// Reads whole file into a string.
		/// </summary>
		/// <param name="path">Path to file to read.</param>
		/// <returns>File content.</returns>
		public static string ReadFileContent(string path)
		{
			string content = null;
			if(File.Exists(path))
			{
				using(StreamReader file = new StreamReader(path, Encoding.Default))
				{
					content = file.ReadToEnd();
				}
			}
			return content;
		}

		/// <summary>
		/// Reads whole file into a string (UTF-8 encoded).
		/// </summary>
		/// <param name="path">Path to file to read.</param>
		/// <returns>File content.</returns>
		public static string ReadFileContentUtf8(string path)
		{
			string content = null;
			if(File.Exists(path))
			{
				using(StreamReader file = new StreamReader(path, Encoding.UTF8))
				{
					content = file.ReadToEnd();
				}
			}
			return content;
		}

		/// <summary>
		/// Creates a file from bytes array.
		/// </summary>
		/// <param name="path">Path to file to write.</param>
		/// <param name="bytes">Bytes array to write to file.</param>
		public static void WriteFile(string path, byte[] bytes)
		{
			using(FileStream file = File.Create(path))
			{
				if(bytes != null)
				{
					file.Write(bytes, 0, bytes.Length);
				}
			}
		}

		/// <summary>
		/// Creates a UTF-8 file with text content provided.
		/// </summary>
		/// <param name="path">Path to file to write.</param>
		/// <param name="content">Text content to write to file.</param>
		public static void WriteFileContentUtf8(string path, string content)
		{
			using(StreamWriter file = File.CreateText(path))
			{
				if(content != null)
				{
					file.Write(content);
				}
			}
		}

		/// <summary>
		/// Application startup path.
		/// </summary>
		public static string LocalPath
		{
			get
			{
				return Application.StartupPath;
			}
		}
	}
}
