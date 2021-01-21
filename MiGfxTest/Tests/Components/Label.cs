////////////////////////////////////////////////////////////////////////////////
// Label.cs 
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

using MiCore;
using MiInput;

namespace MiGfx.Test
{
	public class LabelTest : VisualTestModule
	{
		const string LabelPath = "label.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Label Tests..." );

			Label l1 = new Label( "Test" );

			if( !BinarySerializable.ToFile( l1, LabelPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Label to file.", false );

			Label l2 = BinarySerializable.FromFile<Label>( LabelPath );

			try
			{
				File.Delete( LabelPath );
			}
			catch
			{ }

			if( l2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Label from file.", false );
			if( !l2.Equals( l1 ) )
				return Logger.LogReturn( "Failed: Deserialized Label has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + l1.ToString();
			Label x = XmlLoadable.FromXml<Label>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Label from xml.", false );
			if( !x.Equals( l1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Label has incorrect values.", false );

			l1.Dispose();
			l2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running Label Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			using( MiEntity ent = new MiEntity( "tester", window ) )
			{
				if( !ent.Components.AddNew<Label>() )
					return Logger.LogReturn( "Failed: Unable to add Label to test entity.", false );

				ent.Components.Get<Label>().String = "Test";
				ent.Components.Get<Label>().Text.FillColor = Color.White;
				ent.Components.Get<Transform>().Center = window.GetView().Center;

				Logger.Log( "Is label text displayed on window? (y/n)" );
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
					return Logger.LogReturn( "Failed: Label did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
