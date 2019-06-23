using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LuteBot.IO.Files
{
	static class SteamUtils
	{
		private const string libfile = "libraryfolders.vdf";

		/// <summary>
		/// Returns the steam installation path
		/// </summary>
		/// <returns>The steam install path</returns>
		public static string findSteamInstallPath()
		{
			string path = string.Empty;

			if (System.Environment.Is64BitOperatingSystem)
				path = "SOFTWARE\\Wow6432Node\\Valve\\Steam";
			else
				path = "SOFTWARE\\Valve\\Steam";



			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
			{
				if (key != null)
				{
					Object o = key.GetValue("InstallPath");
					if (o != null)
					{
						return (o as string);
					}
					else
						throw new Exception("Key of register InstallPath is null");
				}
				else
					throw new Exception("Can't open regpath");
			}
			
		}

		/// <summary>
		/// Find the Nth occurence of a T char in a S string
		/// </summary>
		/// <param name="s">Haystack</param>
		/// <param name="t">needle</param>
		/// <param name="n">Nth needle to find</param>
		/// <returns>The position of the needle</returns>
		private static int GetNthIndex(string s, char t, int n)
		{
			int count = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == t)
				{
					count++;
					if (count == n)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns all the steamApps folders where steam games are installed
		/// </summary>
		/// <param name="libraryfolders_path">Path where libraryfolders.vdf is</param>
		/// <returns>List of paths</returns>
		public static List<string> getSteamAppsPaths(string libraryfolders_path)
		{
			string[] lines = File.ReadAllLines(libraryfolders_path + libfile);
			List<string> pathList = new List<string>(lines.Length - 4); // 5 junk lines in the file
			pathList.Add(libraryfolders_path + @"common\");
			int i = 4;
			while (i < lines.Length-1)
			{
				int start = GetNthIndex(lines[i], '\"', 3);
				string path = lines[i].Substring(start + 1, lines[i].Length - start - 2);
				path = path.Replace(@"\\", @"\");
				path += "\\steamapps\\common\\";
				pathList.Add(path);
				i++;
			}

			return (pathList);
		}




	}
}
