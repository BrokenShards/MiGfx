////////////////////////////////////////////////////////////////////////////////
// UILabel.cs 
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
	public class UILabelTest : VisualTestModule
	{
		const string UILabelPath = "uilabel.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running UILabel Tests..." );

			UILabel l1 = new UILabel( "Test" );

			if( !BinarySerializable.ToFile( l1, UILabelPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UILabel to file.", false );

			UILabel l2 = BinarySerializable.FromFile<UILabel>( UILabelPath );

			try
			{
				File.Delete( UILabelPath );
			}
			catch
			{ }

			if( l2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UILabel from file.", false );
			if( !l2.Equals( l1 ) )
				return Logger.LogReturn( "Failed: Deserialized UILabel has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + l1.ToString();
			UILabel x = XmlLoadable.FromXml<UILabel>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load UILabel from xml.", false );
			if( !x.Equals( l1 ) )
				return Logger.LogReturn( "Failed: Xml loaded UILabel has incorrect values.", false );

			l1.Dispose();
			l2.Dispose();
			x.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running UILabel Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			using( MiEntity ent = new MiEntity( "tester", window ) )
			{
				if( !ent.AddNewComponent<UILabel>() )
					return Logger.LogReturn( "Failed: Unable to add UILabel to test entity.", false );

				UILabel label = ent.GetComponent<UILabel>();

				label.String = "Test";
				label.Text.FillColor = Color.White;
				label.Allign = Allignment.Middle;

				FloatRect   textbounds = label.GetTextBounds();
				UITransform tran = ent.GetComponent<UITransform>();

				tran.Origin   = Allignment.Middle;
				tran.Size     = new Vector2f( 0.5f, 0.5f );
				tran.Position = new Vector2f( 0.5f, 0.5f );

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
					return Logger.LogReturn( "Failed: UILabel did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
