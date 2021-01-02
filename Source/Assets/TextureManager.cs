////////////////////////////////////////////////////////////////////////////////
// TextureManager.cs
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

using SFML.Graphics;

namespace MiGfx
{
	/// <summary>
	///   Manages textures.
	/// </summary>
	public class TextureManager : AssetManager<Texture>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextureManager()
		:	base()
		{ }

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
				Texture tex = new Texture( path );
				m_assets.Add( path, tex );
			}
			catch
			{
				return false;
			}

			return true;
		}
	}
}
