////////////////////////////////////////////////////////////////////////////////
// UISpriteArray.cs 
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
	public class UISpriteArrayTest : VisualTestModule
	{
		const string UISpriteArrayPath = "uispritearray.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running UISpriteArray Tests..." );

			Texture tex = Assets.Manager.Texture.Get( Test.AnimationTexturePath );

			if( tex == null )
				return Logger.LogReturn( "Failed: Unable to load test texture.", false );

			Vector2u texsize = tex.Size;

			// Create a new Sprite with an ImageInfo that tells it which texture to draw.
			UISpriteArray s1 = new UISpriteArray() { TexturePath = Test.AnimationTexturePath };

			s1.Sprites.Add( new SpriteInfo( new FloatRect( 0, 0, texsize.X / 3.0f, texsize.Y ) ) );
			s1.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f, 0, texsize.X / 3.0f, texsize.Y ) ) );
			s1.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f * 2.0f, 0, texsize.X / 3.0f, texsize.Y ) ) );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( s1, UISpriteArrayPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UISpriteArray to file.", false );

			// Read the object from a binary file.
			UISpriteArray s2 = BinarySerializable.FromFile<UISpriteArray>( UISpriteArrayPath );

			try
			{
				// Delete temp file.
				File.Delete( UISpriteArrayPath );
			}
			catch
			{ }

			// Ensure object loaded and is the same.
			if( s2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UISpriteArray from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized UISpriteArray has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + s1.ToString();
			UISpriteArray x = XmlLoadable.FromXml<UISpriteArray>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load UISpriteArray from xml.", false );
			if( !x.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Xml loaded UISpriteArray has incorrect values.", false );

			s1.Dispose();
			s2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}

		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running UISpriteArray Visual Tests..." );

			Texture tex = Assets.Manager.Texture.Get( Test.AnimationTexturePath );

			if( tex == null )
				return Logger.LogReturn( "Failed: Unable to load test texture.", false );

			Vector2u texsize = tex.Size;

			using( MiEntity ent = new MiEntity( "tester", window ) )
			{
				if( !ent.AddNewComponent<UISpriteArray>() )
					return Logger.LogReturn( "Failed: Unable to add UISpriteArray to entity.", false ); ;

				UISpriteArray sa = ent.GetComponent<UISpriteArray>();
				sa.TexturePath = Test.AnimationTexturePath;

				sa.Sprites.Add( new SpriteInfo( new FloatRect( 0, 0, texsize.X / 3.0f, texsize.Y ), null,
				                                null, new Vector2f( 100, 100 ) ) );
				sa.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f, 0, texsize.X / 3.0f, texsize.Y ), null,
												new Vector2f( 150, 150 ), new Vector2f( 100, 100 ) ) );
				sa.Sprites.Add( new SpriteInfo( new FloatRect( texsize.X / 3.0f * 2.0f, 0, texsize.X / 3.0f, texsize.Y ), null,
												new Vector2f( 300, 300 ), new Vector2f( 100, 100 ) ) );

				UITransform trn = ent.GetComponent<UITransform>();

				trn.Position = new Vector2f();
				trn.Size     = new Vector2f( 0.4f, 0.4f );

				Logger.Log( "Is UISpriteArray displayed on window? (y/n)" );
				bool? inp = null;

				while( window.IsOpen && inp == null )
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

				if( inp == null || !inp.Value )
					return Logger.LogReturn( "Failed: UISpriteArray did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
