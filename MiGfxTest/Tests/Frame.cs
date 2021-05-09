////////////////////////////////////////////////////////////////////////////////
// Frame.cs 
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
using SFML.System;

namespace MiGfx.Test
{
	public class FrameTest : TestModule
	{
		const string FramePath = "frame.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Frame Tests..." );

			// Construct a new frame with a set texture rect to be shown and a length in time.
			Frame f1 = new( new FloatRect( 0, 0, 24, 24 ), Time.FromSeconds( 1.5f ) );

			// Ensuring values were set correctly.
			if( f1.Rect.Width is not 24 || f1.Rect.Height is not 24 )
				return Logger.LogReturn( "Failed: Frame rect did not set correctly.", false );
			if( f1.Length != Time.FromSeconds( 1.5f ) )
				return Logger.LogReturn( "Failed: Frame length did not set correctly.", false );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( f1, FramePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Frame to file.", false );

			// Read the object back in from binary file.
			Frame f2 = BinarySerializable.FromFile<Frame>( FramePath );

			try
			{
				File.Delete( FramePath );
			}
			catch
			{ }

			// Ensure object loaded and is the same.
			if( f2 is null )
				return Logger.LogReturn( "Failed: Unable to deserialize Frame from file.", false );
			if( !f2.Equals( f1 ) )
				return Logger.LogReturn( "Failed: Deserialized Frame has incorrect values.", false );

			// Create an xml file string from ToString().
			string xml = $"{ Xml.Header }\r\n{ f1 }";
			// Load object from xml string.
			Frame x = XmlLoadable.FromXml<Frame>( xml );

			// Ensure object loaded and is the same.
			if( x is null )
				return Logger.LogReturn( "Failed: Unable to load Frame from xml.", false );
			if( !x.Equals( f1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Frame has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
