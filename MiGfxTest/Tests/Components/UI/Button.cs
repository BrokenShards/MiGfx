////////////////////////////////////////////////////////////////////////////////
// Button.cs 
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
	public class ButtonTest : VisualTestModule
	{
		const string ButtonPath = "button.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Button Tests..." );

			Button b1 = new Button();

			if( !BinarySerializable.ToFile( b1, ButtonPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Button to file.", false );

			Button b2 = BinarySerializable.FromFile<Button>( ButtonPath );

			try
			{
				File.Delete( ButtonPath );
			}
			catch
			{ }

			if( b2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Button from file.", false );
			if( !b2.Equals( b1 ) )
				return Logger.LogReturn( "Failed: Deserialized Button has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + b1.ToString();
			Button x = XmlLoadable.FromXml<Button>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Button from xml.", false );

			if( !x.Equals( b1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Button has incorrect values.", false );

			b1.Dispose();
			b2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running Button Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			MiEntity ent = new MiEntity( "selector", window );

			using( ent )
			{
				if( !ent.AddNewComponent<Selector>() )
				{
					ent.Dispose();
					return Logger.LogReturn( "Failed: Unable to create Selector.", false );
				}

				MiEntity but = Button.Create( "tester", window, "Test Button" );

				if( but == null )
					return Logger.LogReturn( "Failed: Unable to create button.", false );

				UITransform tran = but.GetComponent<UITransform>();

				tran.Origin   = Allignment.Middle;
				tran.Size     = new Vector2f( 0.4f, 0.1f );
				tran.Position = new Vector2f( 0.5f, 0.5f );

				ent.AddChild( but );
				ent.GetComponent<Selector>().Select( 0 );

				Logger.Log( "Is button displayed on window? (y/n)" );
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
					return Logger.LogReturn( "Failed: Button did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
