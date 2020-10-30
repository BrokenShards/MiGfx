////////////////////////////////////////////////////////////////////////////////
// SoundManager.cs
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
using SFML.Audio;

namespace SharpGfx
{
	/// <summary>
	///   Manages sounds.
	/// </summary>
	public class SoundManager : AssetManager<SoundBuffer>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public SoundManager()
		:	base()
		{ }

		/// <summary>
		///   Current music track;
		/// </summary>
		public Music MusicTrack
		{
			get; set;
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
		///   True if the asset was/has been loaded successfully and false
		///   otherwise.
		/// </returns>
		protected override bool Load( string path, bool reload )
		{
			if( IsLoaded( path ) )
			{
				if( !reload )
					return true;

				Unload( path );
			}
			try
			{
				SoundBuffer snd = new SoundBuffer( path );
				m_assets.Add( path, snd );
			}
			catch
			{
				return false;
			}

			return true;
		}
	}
}
