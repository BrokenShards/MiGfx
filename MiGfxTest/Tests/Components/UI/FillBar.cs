////////////////////////////////////////////////////////////////////////////////
// FillBar.cs 
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
	public class FillBarTest : VisualTestModule
	{
		const string FillBarPath = "fillbar.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running FillBar Tests..." );

			FillBar b1 = new FillBar();

			if( !BinarySerializable.ToFile( b1, FillBarPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize FillBar to file.", false );

			FillBar b2 = BinarySerializable.FromFile<FillBar>( FillBarPath );

			try
			{
				File.Delete( FillBarPath );
			}
			catch
			{ }

			if( b2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize FillBar from file.", false );
			if( !b2.Equals( b1 ) )
				return Logger.LogReturn( "Failed: Deserialized FillBar has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + b1.ToString();
			FillBar x = XmlLoadable.FromXml<FillBar>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load FillBar from xml.", false );

			if( !x.Equals( b1 ) )
				return Logger.LogReturn( "Failed: Xml loaded FillBar has incorrect values.", false );

			b1.Dispose();
			b2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}

		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running FillBar Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			MiEntity ent = FillBar.Create( "tester", window, 0, 0, 4000, Color.Blue, FillBar.LabelType.ValueMax );

			if( ent == null )
				return Logger.LogReturn( "Failed: Unable to create FillBar entity.", false );

			using( ent )
			{
				Transform tran = ent.GetComponent<Transform>();

				tran.Origin   = Allignment.Middle; 
				tran.Relative = true;
				tran.Position = new Vector2f( 400, 300 );

				Logger.Log( "Is FillBar displayed on window? (y/n)" );
				bool? inp = null;

				while( window.IsOpen && inp == null )
				{
					window.DispatchEvents();

					Input.Manager.Update();
					ent.Update( 1.0f );

					if( Input.Manager.Keyboard.JustPressed( "Right" ) )
						ent.GetComponent<FillBar>().Value += 250;
					else if( Input.Manager.Keyboard.JustPressed( "Left" ) )
						ent.GetComponent<FillBar>().Value -= 250;

					if( Input.Manager.Keyboard.JustPressed( "Y" ) )
						inp = true;
					else if( Input.Manager.Keyboard.JustPressed( "N" ) )
						inp = false;

					window.Clear();
					window.Draw( ent );
					window.Display();
				}

				if( inp == null || !inp.Value )
					return Logger.LogReturn( "Failed: FillBar did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
