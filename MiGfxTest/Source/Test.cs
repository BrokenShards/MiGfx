////////////////////////////////////////////////////////////////////////////////
// Test.cs 
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
using SFML.Graphics;
using SFML.Window;
using MiCore;

namespace MiGfxTest
{
	partial class Test
	{
		public static readonly string TexturePath = MiGfx.FolderPaths.Textures + "test.png";

		static void Main( string[] args )
		{
			Logger.LogToConsole = true;
			Logger.LogToFile    = false;

			Logger.Log( "Running MiGfx Tests..." );

			bool result = true;

			if( !AssetTests() )
				result = false;

			using( RenderWindow window = new RenderWindow( new VideoMode( 800, 600, 32 ), "MiGfx Test", Styles.Close ) )
			{
				if( !GraphicsTests( window ) )
					result = false;
				if( !UITests( window ) )
					result = false;

				window.Close();
			}

			Logger.Log( result ? "All tests ran successfully." : "One or more tests failed." );
			Logger.Log( "Press enter to exit." );
			Console.ReadLine();
		}
	}
}
