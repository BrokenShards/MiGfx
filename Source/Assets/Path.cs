////////////////////////////////////////////////////////////////////////////////
// Paths.cs
////////////////////////////////////////////////////////////////////////////////
//
// MiGfx - A basic graphics library for use with SFML.Net.
// Copyright (C) 2021 Michael Furlong <michaeljfurlong@outlook.com>
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

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   Contains folder paths.
	/// </summary>
	public static class FolderPaths
	{
		/// <summary>
		///   Directory separator character.
		/// </summary>
		public static readonly char Separator = Path.DirectorySeparatorChar;

		/// <summary>
		///   The path to the folder containing the binary executable.
		/// </summary>
		public static readonly string Executable = Path.GetDirectoryName( FilePaths.Executable ) + Separator;

		/// <summary>
		///   Base assets folder.
		/// </summary>
		public static readonly string Assets = $"{ Executable }Assets{ Separator }";

		/// <summary>
		///   Textures folder.
		/// </summary>
		public static readonly string Textures = $"{ Assets }Textures{ Separator }";
		/// <summary>
		///   Fonts folder.
		/// </summary>
		public static readonly string Fonts = $"{ Assets }Fonts{ Separator }";
		/// <summary>
		///   Sprites folder.
		/// </summary>
		public static readonly string Sprites = $"{ Assets }Sprites{ Separator }";
		/// <summary>
		///   Tilesets folder.
		/// </summary>
		public static readonly string Tilesets = $"{ Assets }Tilesets{ Separator }";

		/// <summary>
		///   Sounds folder.
		/// </summary>
		public static readonly string Sounds = $"{ Assets }Sounds{ Separator }";
		/// <summary>
		///   Music folder.
		/// </summary>
		public static readonly string Music = $"{ Assets }Music{ Separator }";

		/// <summary>
		///   GUI assets folder.
		/// </summary>
		public static readonly string UI = $"{ Assets }UI{ Separator }";

		/// <summary>
		///   Settings folder.
		/// </summary>
		public static readonly string Settings = $"{ Executable }Settings{ Separator }";

		/// <summary>
		///   Game data folder.
		/// </summary>
		public static readonly string Data = $"{ Executable }Data{ Separator }";
		/// <summary>
		///   Database folder.
		/// </summary>
		public static readonly string Databases = $"{ Data }Databases{ Separator }";
		/// <summary>
		///   Tilemaps folder.
		/// </summary>
		public static readonly string Tilemaps = $"{ Data }Tilemaps{ Separator }";

		/// <summary>
		///   Creates the entire folder structure.
		/// </summary>
		/// <returns></returns>
		public static bool CreateFolderStructure()
		{
			try
			{
				Directory.CreateDirectory( Assets );
				Directory.CreateDirectory( Textures );
				Directory.CreateDirectory( Fonts );
				Directory.CreateDirectory( Sprites );
				Directory.CreateDirectory( Tilesets );
				Directory.CreateDirectory( Sounds );
				Directory.CreateDirectory( Music );				
				Directory.CreateDirectory( UI );

				Directory.CreateDirectory( Settings );

				Directory.CreateDirectory( Data );
				Directory.CreateDirectory( Databases );
				Directory.CreateDirectory( Tilemaps );
			}
			catch( UnauthorizedAccessException )
			{
				return Logger.LogReturn( "Do not have permission to create folder structure. Try running again as admin?", false, LogType.Error );
			}
			catch( PathTooLongException )
			{
				return Logger.LogReturn( "Folder structure path too long. Try relocating program closer to root?", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Unable to create folder structure: { e.Message }", false, LogType.Error );
			}

			return true;
		}
	}

	/// <summary>
	///   Contains file paths.
	/// </summary>
	public static partial class FilePaths
	{
		/// <summary>
		///   The binary executable path.
		/// </summary>
		public static string Executable
		{
			get
			{
				string path = Assembly.GetExecutingAssembly().Location;

				if( path.StartsWith( "file:///" ) || path.StartsWith( "file:\\\\\\" ) )
					path = path[ 8.. ];
				else if( path.StartsWith( "file://" ) || path.StartsWith( "file:\\\\" ) )
					path = path[ 7.. ];

				return path;
			}
		}
	}
}
