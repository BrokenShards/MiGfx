////////////////////////////////////////////////////////////////////////////////
// TextBox.cs 
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
	public class TextBoxTest : VisualTestModule
	{
		const string TextBoxPath = "text_box.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running TextBox Tests..." );

			TextBox t1 = new TextBox( new TextBoxData( new ImageInfo( Test.SpriteTexturePath ) ) );

			if( !BinarySerializable.ToFile( t1, TextBoxPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize TextBox to file.", false );

			TextBox t2 = BinarySerializable.FromFile<TextBox>( TextBoxPath );

			try
			{
				File.Delete( TextBoxPath );
			}
			catch
			{ }

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize TextBox from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized TextBox has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + t1.ToString();
			TextBox x = XmlLoadable.FromXml<TextBox>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load CheckBox from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded CheckBox has incorrect values.", false );

			t1.Dispose();
			t2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running TextBox Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			MiEntity ent = null;

			void OnTextEntered( object sender, SFML.Window.TextEventArgs e )
			{
				ent.TextEntered( e );
			}

			window.TextEntered += OnTextEntered;

			using( ent = TextBox.Create( "tester", window, default, true ) )
			{
				ent.Components.Get<Transform>().Size = window.GetView().Size;
				ent.Components.Get<Transform>().Position = new Vector2f( 0, 0 );
				ent.Components.Get<TextBox>().SetString( "Testing text box." );

				Logger.Log( "Is textbox displayed on window? (y/n)" );
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
					return Logger.LogReturn( "Failed: TextBox did not display correctly (user input).", false );
			}

			window.TextEntered -= OnTextEntered;

			return Logger.LogReturn( "Success!", true );
		}
	}
}
