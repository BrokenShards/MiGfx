////////////////////////////////////////////////////////////////////////////////
// CheckBox.cs 
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
	public class CheckBoxTest : VisualTestModule
	{
		const string CheckBoxPath = "checkbox.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running CheckBox Tests..." );

			CheckBox c1 = new CheckBox();

			if( c1.Checked )
				return Logger.LogReturn( "Failed: Default constructed object with incorrect checked value.", false );
			if( !BinarySerializable.ToFile( c1, CheckBoxPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize CheckBox to file.", false );

			CheckBox c2 = BinarySerializable.FromFile<CheckBox>( CheckBoxPath );

			try
			{
				File.Delete( CheckBoxPath );
			}
			catch
			{ }

			if( c2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize CheckBox from file.", false );
			if( !c2.Equals( c1 ) )
				return Logger.LogReturn( "Failed: Deserialized CheckBox has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + c1.ToString();
			CheckBox x = XmlLoadable.FromXml<CheckBox>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load CheckBox from xml.", false );
			if( !x.Equals( c1 ) )
				return Logger.LogReturn( "Failed: Xml loaded CheckBox has incorrect values.", false );

			c1.Dispose();
			c2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running CheckBox Visual Tests..." );

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

				MiEntity chk = CheckBox.Create( "tester", window );

				if( chk == null )
					return Logger.LogReturn( "Failed: Unable to create CheckBox.", false );

				UITransform tran = chk.GetComponent<UITransform>();

				tran.Origin   = Allignment.Middle;
				tran.Position = new Vector2f( 0.5f, 0.5f );

				ent.AddChild( chk );
				ent.GetComponent<Selector>().Select( 0 );

				Logger.Log( "Is checkbox displayed on window? (y/n)" );
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
					return Logger.LogReturn( "Failed: CheckBox did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
