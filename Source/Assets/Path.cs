////////////////////////////////////////////////////////////////////////////////
// Paths.cs
////////////////////////////////////////////////////////////////////////////////
//
// SharpAsset - A basic asset manager library for use with SFML.Net.
// Copyright (C) 2020 Michael Furlong <michaeljfurlong@outlook.com>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <https://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Reflection;

using SharpLogger;

namespace SharpGfx
{
	/// <summary>
	///   Contains path related functionality.
	/// </summary>
	public static class Paths
	{
		/// <summary>
		///   Swaps path seperator character '/' with '\\' like in windows paths.
		/// </summary>
		/// <param name="path">
		///   The path.
		/// </param>
		/// <returns>
		///   The path with the seperator character '/' replaced with '\\'.
		/// </returns>
		public static string ToWindows( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return path;

			return path.Replace( '/', '\\' );
		}
		/// <summary>
		///   Swaps path seperator character '\\' with '/' like in non windows paths.
		/// </summary>
		/// <param name="path">
		///   The path.
		/// </param>
		/// <returns>
		///   The path with the seperator character '\\' replaced with '/'.
		/// </returns>
		public static string FromWindows( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return path;

			return path.Replace( '\\', '/' );
		}
	}

	/// <summary>
	///   Contains folder paths.
	/// </summary>
	public static class FolderPaths
	{
		/// <summary>
		///   The folder path containing the binary executable.
		/// </summary>
		public static string Executable
		{
			get
			{
				string path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase );

				try
				{
					string s5  = path.Substring( 0, 5 ).ToLower();
					string s6  = path.Substring( 0, 6 ).ToLower();
					string s8  = path.Substring( 0, 8 ).ToLower();
					string s10 = path.Substring( 0, 10 ).ToLower();

					if( s5 == "dir:/" || s5 == "dir:\\" )
						path = path.Substring( 5 );
					else if( s6 == "file:/" || s6 == "file:\\" || s6 == "path:/" || s6 == "path:\\" )
						path = path.Substring( 6 );
					else if( s8 == "folder:/" || s8 == "folder:\\" )
						path = path.Substring( 8 );
					else if( s10 == "directory:/" || s10 == "directory:\\" )
						path = path.Substring( 10 );
				}
				catch( Exception e )
				{
					Console.WriteLine( e.Message );
				}

				return Paths.ToWindows( path );
			}
		}

		/// <summary>
		///   Base assets folder.
		/// </summary>
		public static readonly string Assets    = Executable + "\\Assets\\";

		/// <summary>
		///   Sprites folder.
		/// </summary>
		public static readonly string Sprites   = Assets + "Sprites\\";
		/// <summary>
		///   Filesets folder.
		/// </summary>
		public static readonly string Tilesets  = Assets + "Tilesets\\";
		/// <summary>
		///   Sounds folder.
		/// </summary>
		public static readonly string Sounds    = Assets + "Sounds\\";
		/// <summary>
		///   Music folder.
		/// </summary>
		public static readonly string Music     = Assets + "Music\\";
		/// <summary>
		///   Fonts folder.
		/// </summary>
		public static readonly string Fonts     = Assets + "Fonts\\";
		/// <summary>
		///   GUI assets folder.
		/// </summary>
		public static readonly string UI        = Assets + "UI\\";

		/// <summary>
		///   Settings folder.
		/// </summary>
		public static readonly string Settings  = Executable + "\\Settings\\";

		/// <summary>
		///   Game data folder.
		/// </summary>
		public static readonly string Data      = Executable + "\\Data\\";
		/// <summary>
		///   Database folder.
		/// </summary>
		public static readonly string Databases = Data + "Databases\\";
		/// <summary>
		///   Tilemaps folder.
		/// </summary>
		public static readonly string Tilemaps  = Data + "Tilemaps\\";

		/// <summary>
		///   Creates the entire folder structure.
		/// </summary>
		/// <returns></returns>
		public static bool CreateFolderStructure()
		{
			try
			{
				Directory.CreateDirectory( Assets );
				Directory.CreateDirectory( Sprites );
				Directory.CreateDirectory( Tilesets );
				Directory.CreateDirectory( Sounds );
				Directory.CreateDirectory( Music );
				Directory.CreateDirectory( Fonts );
				Directory.CreateDirectory( UI );

				Directory.CreateDirectory( Settings );

				Directory.CreateDirectory( Data );
				Directory.CreateDirectory( Databases );
				Directory.CreateDirectory( Tilemaps );
			}
			catch( UnauthorizedAccessException )
			{
				Logger.Log( "Do not have permission to create folder structure. Try running again as admin?", LogType.Error );
				return false;
			}
			catch( PathTooLongException )
			{
				Logger.Log( "Folder structure path too long. Try relocating game closer to root?", LogType.Error );
				return false;
			}
			catch( Exception e )
			{
				Logger.Log( "Unable to create folder structure: " + e.Message, LogType.Error );
				return false;
			}

			return true;
		}
	}
}
