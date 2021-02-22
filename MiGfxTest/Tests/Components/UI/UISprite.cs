////////////////////////////////////////////////////////////////////////////////
// UISprite.cs 
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

using MiCore;
using MiInput;
using System.IO;
using SFML.Graphics;
using SFML.System;
using System;

namespace MiGfx.Test
{
	public class UISpriteTest : VisualTestModule
	{
		const string UISpritePath = "uisprite.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running UISprite Tests..." );

			// Sprite is a simple component that draws an image at the position and size of the
			// parent entities Transform.

			// Create a new Sprite with an ImageInfo that tells it which texture to draw.
			UISprite s1 = new UISprite( new ImageInfo( Test.SpriteTexturePath ) );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( s1, UISpritePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UISprite to file.", false );

			// Read the object from a binary file.
			UISprite s2 = BinarySerializable.FromFile<UISprite>( UISpritePath );

			try
			{
				// Delete temp file.
				File.Delete( UISpritePath );
			}
			catch
			{ }

			// Ensure object loaded and is the same.
			if( s2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UISprite from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized UISprite has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + s1.ToString();
			UISprite x = XmlLoadable.FromXml<UISprite>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load UISprite from xml.", false );
			if( !x.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Xml loaded UISprite has incorrect values.", false );

			s1.Dispose();
			s2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running UISprite Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			using( MiEntity ent = new MiEntity( "tester", window ) )
			{
				if( !ent.AddNewComponent<UISprite>() )
					return Logger.LogReturn( "Failed: Unable to add UISprite to test entity.", false );

				UISprite spr = ent.GetComponent<UISprite>();				
				spr.Image = new ImageInfo( Test.SpriteTexturePath );

				UITransform trn = ent.GetComponent<UITransform>();

				trn.Origin   = Allignment.Middle;
				trn.Size     = new Vector2f( 0.2f, 0.2f );
				trn.Position = new Vector2f( 0.5f, 0.5f );

				Logger.Log( "Is yellow quad displayed on window? (y/n)" );
				bool? inp = null;

				while( window.IsOpen && inp == null )
				{
					window.DispatchEvents();

					Input.Manager.Update();
					ent.Update( 1.0f );

					if( Input.Manager.Keyboard.JustPressed( "Right" ) )
					{
						int o = (int)spr.Image.Orientation;
						o++;

						if( o >= Enum.GetNames( typeof( Direction ) ).Length )
							o = 0;

						spr.Image.Orientation = (Direction)o;
						Logger.Log( "Sprite Orientation: " + spr.Image.Orientation.ToString() );
					}
					else if( Input.Manager.Keyboard.JustPressed( "Left" ) )
					{
						int o = (int)spr.Image.Orientation;
						o--;

						if( o < 0 )
							o = Enum.GetNames( typeof( Direction ) ).Length - 1;

						spr.Image.Orientation = (Direction)o;
						Logger.Log( "Sprite Orientation: " + spr.Image.Orientation.ToString() );
					}

					if( Input.Manager.Keyboard.JustPressed( "H" ) )
					{
						spr.Image.FlipHorizontal = !spr.Image.FlipHorizontal;
						Logger.Log( "Sprite FlipHorizontal: " + spr.Image.FlipHorizontal.ToString() );
					}
					else if( Input.Manager.Keyboard.JustPressed( "V" ) )
					{
						spr.Image.FlipVertical = !spr.Image.FlipVertical;
						Logger.Log( "Sprite FlipVertical: " + spr.Image.FlipVertical.ToString() );
					}

					if( Input.Manager.Keyboard.JustPressed( "Y" ) )
						inp = true;
					else if( Input.Manager.Keyboard.JustPressed( "N" ) )
						inp = false;

					window.Clear();
					window.Draw( ent );
					window.Display();
				}

				if( inp == null || !inp.Value )
					return Logger.LogReturn( "Failed: UISprite did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
