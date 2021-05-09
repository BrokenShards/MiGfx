////////////////////////////////////////////////////////////////////////////////
// FontManager.cs 
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

using MiCore;
using SFML.Graphics;

namespace MiGfx.Test
{
	public class FontManagerTest : TestModule
	{
		// The path of the Font to load.
		static readonly string FontPath = $"{ FolderPaths.Fonts }FallingSky.otf";

		protected override bool OnTest()
		{
			Logger.Log( "Running FontManager Tests..." );

			// It is always good practise to check if the asset manager has been disposed of before
			// using it with Disposed. If it has, we can recreate it with Recreate.
			if( Assets.Manager.Disposed )
				Assets.Manager.Recreate();

			// Here we use the asset manager to get a Font. Get checks if an asset has already been
			// loaded from the file path, if not, an attempt is made to load it from file before
			// returning it.
			Font fnt = Assets.Manager.Get<Font>( FontPath );

			// Above is the same as:
			Font fnt2 = Assets.Manager.Font.Get( FontPath );

			// If the asset is null, either the file does not exist or loading from the file failed.
			if( fnt is null )
				return Logger.LogReturn( "Failed: Unable to load Font from path.", false );

			//
			// ...Code using asset...
			//

			// When a specific asset is no longer needed, it can be explicitly unloaded to save
			// memory, this can be done with Unload:
			Assets.Manager.Font.Unload( FontPath );

			// If you would like to unload all assets of a specific type, use:
			Assets.Manager.Unload<Font>();
			// Or call it from the specific asset manager directly:
			Assets.Manager.Font.Clear();

			// At the end of your program, you should always unload all assets by disposing of the
			// main asset manager, this will in turn dispose of the internal asset managers.
			Assets.Manager.Dispose();

			// Recreating asset manager so next test can run.
			if( Assets.Manager.Disposed )
				Assets.Manager.Recreate();

			return Logger.LogReturn( "Success!", true );
		}
	}
}
