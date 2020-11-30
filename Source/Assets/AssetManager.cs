////////////////////////////////////////////////////////////////////////////////
// AssetManager.cs
////////////////////////////////////////////////////////////////////////////////
//
// SharpGfx - A basic graphics library for use with SFML.Net.
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
using System.Collections.Generic;

namespace SharpGfx
{
	/// <summary>
	///   Base class for asset managers.
	/// </summary>
	/// <typeparam name="T">
	///   The managed asset type.
	/// </typeparam>
	public abstract class AssetManager<T> : IDisposable where T : class, IDisposable
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public AssetManager()
		{
			m_assets = new Dictionary<string, T>();
		}

		/// <summary>
		///   If the manager contains no assets.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of assets the manager contains.
		/// </summary>
		public int Count
		{
			get { return m_assets.Count; }
		}

		/// <summary>
		///   If an asset been loaded from the path.
		/// </summary>
		/// <param name="path">
		///   The path of the asset.
		/// </param>
		/// <returns>
		///   True if an asset has already been loaded from the path and false otherwise.
		/// </returns>
		public bool IsLoaded( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return false;

			path = Paths.ToWindows( path );
			string exec = FolderPaths.Executable;

			int elen = exec.Length;

			if( path.Length <= elen || path.Substring( 0, elen ) != exec )
				path = Path.Combine( exec, path );

			return m_assets.ContainsKey( path );
		}
		/// <summary>
		///   Gets the asset loaded from the given path, attempting to load a new one if needed.
		/// </summary>
		/// <remarks>
		///   Please note the given path should be relative to the executable as it will be 
		///   appended to the executable path.
		/// </remarks>
		/// <param name="path">
		///   The path of the asset.
		/// </param>
		/// <param name="reload">
		///   If an already loaded asset ahould be reloaded.
		/// </param>
		/// <returns>
		///   The asset loaded from the given path or null on failure.
		/// </returns>
		public T Get( string path, bool reload = false )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return null;

			path = Paths.ToWindows( path );
			string exec = FolderPaths.Executable;

			int elen = exec.Length;

			if( path.Length <= elen || path.Substring( 0, elen ) != exec )
				path = Path.Combine( exec, path );

			if( !IsLoaded( path ) )
				if( !Load( path, reload ) )
					return null;

			return m_assets[ path ];
		}

		/// <summary>
		///   Loads an asset from the given path.
		/// </summary>
		/// <param name="path">
		///   The path to load the asset from.
		/// </param>
		/// <param name="reload">
		///   If an already existing asset should be reloaded.
		/// </param>
		/// <returns>
		///   True if the asset was/has been loaded successfully and false otherwise.
		/// </returns>
		protected abstract bool Load( string path, bool reload );
		/// <summary>
		///   Unloads the asset loaded from the given path.
		/// </summary>
		/// <remarks>
		///   Please note the given path should be relative to the executable as it will be 
		///   appended to the executable path.
		/// </remarks>
		/// <param name="path">
		///   The path of the asset.
		/// </param>
		/// <returns>
		///   True if the asset existed and was unloaded and removed successfully, otherwise false.
		/// </returns>
		public virtual bool Unload( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return false;

			path = Paths.ToWindows( path );
			string exec = FolderPaths.Executable;

			int elen = exec.Length;

			if( path.Length <= elen || path.Substring( 0, elen ) != exec )
				path = Path.Combine( exec, path );

			if( m_assets.ContainsKey( path ) )
				m_assets[ path ].Dispose();

			return m_assets.Remove( path );
		}
		/// <summary>
		///   Unloads all assets.
		/// </summary>
		public void UnloadAll()
		{
			var keys = m_assets.Keys;

			string[] list = new string[ keys.Count ];
			keys.CopyTo( list, 0 );

			foreach( string s in list )
				Unload( s );
		}

		/// <summary>
		///   Disposes of all assets.
		/// </summary>
		public virtual void Dispose()
		{
			UnloadAll();
		}

		/// <summary>
		///   Dictionary containing assets indexed by their file paths.
		/// </summary>
		protected Dictionary<string, T> m_assets;
	}
}
