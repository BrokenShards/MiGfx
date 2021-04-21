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

namespace MiGfx.Test
{
	partial class Test
	{
		public static readonly string SpriteTexturePath    = FolderPaths.Textures + "test.png";
		public static readonly string AnimationTexturePath = FolderPaths.Textures + "test_anim.png";

		static void Main( string[] args )
		{
			Logger.LogToConsole = true;
			Logger.LogToFile    = false;

			Logger.Log( "Running MiGfx Tests..." );

			bool result = true;
			
			if( Components.Register() )
			{
				using( RenderWindow window = new RenderWindow( new VideoMode( 800, 600, 32 ), "MiGfx Test", Styles.Close ) )
				{
					if( !new FontManagerTest().RunTest() )
						result = false;
					if( !new SoundManagerTest().RunTest() )
						result = false;
					if( !new TextureManagerTest().RunTest() )
						result = false;

					if( !new FrameTest().RunTest( window ) )
						result = false;
					if( !new AnimationTest().RunTest( window ) )
						result = false;
					if( !new AnimationSetTest().RunTest( window ) )
						result = false;

					if( !new ImageInfoTest().RunTest( window ) )
						result = false;
					if( !new TextStyleTest().RunTest( window ) )
						result = false;

					if( !new TilesetTest().RunTest( window ) )
						result = false;
					if( !new TransformTest().RunTest( window ) )
						result = false;

					if( !new SpriteTest().RunTest( window ) )
						result = false;
					if( !new SpriteArrayTest().RunTest( window ) )
						result = false;
					if( !new SpriteAnimatorTest().RunTest( window ) )
						result = false;
					if( !new LabelTest().RunTest( window ) )
						result = false;

					if( !new ButtonTest().RunTest( window ) )
						result = false;
					if( !new CheckBoxTest().RunTest( window ) )
						result = false;
					if( !new FillBarTest().RunTest( window ) )
						result = false;
					if( !new TextBoxTest().RunTest( window ) )
						result = false;

					window.Close();
				}
			}
			else
				Logger.Log( "Failed registering components." );

			Logger.Log( result ? "All tests ran successfully." : "One or more tests failed." );
			Logger.Log( "Press enter to exit." );
			Console.ReadLine();
		}
	}
}
