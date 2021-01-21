////////////////////////////////////////////////////////////////////////////////
// Assets.cs
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
using MiCore;

namespace MiGfx
{
	/// <summary>
	///   Singleton asset manager.
	/// </summary>
	public class Assets : IDisposable
	{
		private Assets()
		{
			Font     = new FontManager();
			Sound    = new SoundManager();
			Texture  = new TextureManager();
			Disposed = false;
		}

		/// <summary>
		///   The singleton manager instance.
		/// </summary>
		public static Assets Manager
		{
			get
			{
				if( _instance == null )
				{
					lock( _syncRoot )
					{
						if( _instance == null )
						{
							_instance = new Assets();
						}
					}
				}

				return _instance;
			}
		}

		/// <summary>
		///   Font manager.
		/// </summary>
		public FontManager Font { get; private set; }
		/// <summary>
		///   Sound manager.
		/// </summary>
		public SoundManager Sound { get; private set; }
		/// <summary>
		///   Texture manager.
		/// </summary>
		public TextureManager Texture { get; private set; }

		/// <summary>
		///   If the asset managers have been disposed.
		/// </summary>
		public bool Disposed
		{
			get; private set;
		}

		/// <summary>
		///   If an asset has been loaded from the path.
		/// </summary>
		/// <param name="path">
		///   The path of the asset.
		/// </param>
		/// <returns>
		///   True if an asset has already been loaded from the path and false otherwise.
		/// </returns>
		public bool IsLoaded<T>( string path ) where T : class, IDisposable
		{
			if( typeof( T ) == typeof( SFML.Graphics.Font ) )
				return Font.IsLoaded( path );
			else if( typeof( T ) == typeof( SFML.Audio.SoundBuffer ) )
				return Sound.IsLoaded( path );
			else if( typeof( T ) == typeof( SFML.Graphics.Texture ) )
				return Texture.IsLoaded( path );

			return Logger.LogReturn( "Invalid type given to Assets.IsLoaded(string)", false, LogType.Error );
		}

		/// <summary>
		///   Gets the asset loaded from the given path, attempting to load a new one if needed.
		/// </summary>
		/// <remarks>
		///   Please note the given path should be relative to the executable
		///   as it will be appended to the executable path.
		/// </remarks>
		/// <typeparam name="T">
		///   The asset type to get.
		/// </typeparam>
		/// <param name="path">
		///   The path of the asset.
		/// </param>
		/// <param name="reload">
		///   If an already loaded asset ahould be reloaded.
		/// </param>
		/// <returns>
		///   The asset loaded from the given path or null if either T is not 
		///   a valid asset type or on failure.
		/// </returns>
		public T Get<T>( string path, bool reload = false ) where T : class, IDisposable
		{
			if( typeof( T ) == typeof( SFML.Graphics.Font ) )
				return Font.Get( path, reload ) as T;
			else if( typeof( T ) == typeof( SFML.Audio.SoundBuffer ) )
				return Sound.Get( path, reload ) as T;
			else if( typeof( T ) == typeof( SFML.Graphics.Texture ) )
				return Texture.Get( path, reload ) as T;

			return Logger.LogReturn<T>( "Invalid type given to Assets.Get(string,(bool))", null, LogType.Error );
		}

		/// <summary>
		///   Unloads all assets of the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The asset type to unload.
		/// </typeparam>
		public void Unload<T>() where T : class, IDisposable
		{
			if( typeof( T ) == typeof( SFML.Graphics.Font ) )
				Font.Clear();
			else if( typeof( T ) == typeof( SFML.Audio.SoundBuffer ) )
				Sound.Clear();
			else if( typeof( T ) == typeof( SFML.Graphics.Texture ) )
				Texture.Clear();
		}
		/// <summary>
		///   Unloads all assets of all types.
		/// </summary>
		public void Clear()
		{
			Font?.Clear();
			Sound?.Clear();
			Texture?.Clear();
		}

		/// <summary>
		///   Recreates the asset managers after being disposed so they can be used again.
		/// </summary>
		public void Recreate()
		{
			if( Disposed )
			{
				Font = new FontManager();
				Sound = new SoundManager();
				Texture = new TextureManager();
				Disposed = false;
			}
		}
		/// <summary>
		///   Disposes of asset managers.
		/// </summary>
		public void Dispose()
		{
			if( Font != null )
			{
				Font.Dispose();
				Font = null;
			}
			if( Sound != null )
			{
				Sound.Dispose();
				Sound = null;
			}
			if( Texture != null )
			{
				Texture.Dispose();
				Texture = null;
			}

			Disposed = true;
		}

		private static volatile Assets _instance;
		private static readonly object _syncRoot = new object();
	}
}
