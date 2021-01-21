////////////////////////////////////////////////////////////////////////////////
// TextStyle.cs 
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
using System.IO;
using SFML.Graphics;

namespace MiGfx.Test
{
	public class TextStyleTest : TestModule
	{
		const string TextStylePath = "text_style.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running TextStyle Tests..." );

			TextStyle t1 = new TextStyle( "font.ttf", 33, 0, Color.Red, 1, Color.Blue );

			if( !BinarySerializable.ToFile( t1, TextStylePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize TextStyle to file.", false );

			TextStyle t2 = BinarySerializable.FromFile<TextStyle>( TextStylePath );

			try
			{
				File.Delete( TextStylePath );
			}
			catch
			{ }

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize TextStyle from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized TextStyle has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + t1.ToString();
			TextStyle x = XmlLoadable.FromXml<TextStyle>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load TextStyle from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded TextStyle has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
