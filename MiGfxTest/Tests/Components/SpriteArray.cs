////////////////////////////////////////////////////////////////////////////////
// SpriteArray.cs 
////////////////////////////////////////////////////////////////////////////////
//
// MiGfx - A basic graphics library for use with SFML.Net.
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

using System.IO;

using SFML.Graphics;
using SFML.System;

using MiCore;
using MiInput;

namespace MiGfx.Test
{
	public class SpriteArrayTest : VisualTestModule
	{
		const string SpriteArrayPath = "spritearray.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running SpriteArray Tests..." );

			Texture tex = Assets.Manager.Texture.Get( Test.AnimationTexturePath );

			if( tex is null )
				return Logger.LogReturn( "Failed: Unable to load test texture.", false );

			Vector2u texsize = tex.Size;

			// Create a new Sprite with an ImageInfo that tells it which texture to draw.
			SpriteArray s1 = new() { TexturePath = Test.AnimationTexturePath };

			s1.Sprites.Add( new SpriteInfo( new FloatRect( 0, 0, texsize.X / 3.0f, texsize.Y ) ) );
			s1.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f, 0, texsize.X / 3.0f, texsize.Y ) ) );
			s1.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f * 2.0f, 0, texsize.X / 3.0f, texsize.Y ) ) );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( s1, SpriteArrayPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize SpriteArray to file.", false );

			// Read the object from a binary file.
			SpriteArray s2 = BinarySerializable.FromFile<SpriteArray>( SpriteArrayPath );

			try
			{
				// Delete temp file.
				File.Delete( SpriteArrayPath );
			}
			catch
			{ }

			// Ensure object loaded and is the same.
			if( s2 is null )
				return Logger.LogReturn( "Failed: Unable to deserialize SpriteArray from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized SpriteArray has incorrect values.", false );

			string xml = $"{ Xml.Header }\r\n{ s1 }";
			SpriteArray x = XmlLoadable.FromXml<SpriteArray>( xml );

			if( x is null )
				return Logger.LogReturn( "Failed: Unable to load SpriteArray from xml.", false );
			if( !x.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Xml loaded SpriteArray has incorrect values.", false );

			s1.Dispose();
			s2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}

		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running SpriteArray Visual Tests..." );

			Texture tex = Assets.Manager.Texture.Get( Test.AnimationTexturePath );

			if( tex is null )
				return Logger.LogReturn( "Failed: Unable to load test texture.", false );

			Vector2u texsize = tex.Size;

			using( MiEntity ent = new( "tester", window ) )
			{
				if( !ent.AddNewComponent<SpriteArray>() )
					return Logger.LogReturn( "Failed: Unable to add SpriteArray to entity.", false ); ;

				SpriteArray sa = ent.GetComponent<SpriteArray>();
				sa.TexturePath = Test.AnimationTexturePath;

				sa.Sprites.Add( new SpriteInfo( new FloatRect( 0, 0, texsize.X / 3.0f, texsize.Y ), null,
				                                null, new Vector2f( 100, 100 ) ) );
				sa.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f, 0, texsize.X / 3.0f, texsize.Y ), null,
												new Vector2f( 150, 150 ), new Vector2f( 100, 100 ) ) );
				sa.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f * 2.0f, 0, texsize.X / 3.0f, texsize.Y ), null,
												new Vector2f( 300, 300 ), new Vector2f( 100, 100 ) ) );

				Transform trn = ent.GetComponent<Transform>();

				trn.Position = new Vector2f();
				trn.Size     = new Vector2f( 400, 400 );

				Logger.Log( "Is SpriteArray displayed on window? (y/n)" );
				bool? inp = null;

				while( window.IsOpen && inp is null )
				{
					window.DispatchEvents();

					Input.Manager.Update();
					ent.Update( 1.0f );

					if( Input.Manager.Keyboard.JustPressed( "Y" ) )
						inp = true;
					else if( Input.Manager.Keyboard.JustPressed( "N" ) )
						inp = false;

					window.Clear();
					window.Draw( ent );
					window.Display();
				}

				if( inp is null || !inp.Value )
					return Logger.LogReturn( "Failed: SpriteArray did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
