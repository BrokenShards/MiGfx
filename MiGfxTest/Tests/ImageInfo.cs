////////////////////////////////////////////////////////////////////////////////
// ImageInfo.cs 
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
	public class ImageInfoTest : TestModule
	{
		const string ImageInfoPath = "image_info.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running ImageInfo Tests..." );

			// Create image info with a texture, rect, orientation and color.
			ImageInfo i1 = new( "image.png", new FloatRect( 0, 0, 40, 40 ), Color.Blue );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( i1, ImageInfoPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize ImageInfo to file.", false );

			// Read the object back in from binary file.
			ImageInfo i2 = BinarySerializable.FromFile<ImageInfo>( ImageInfoPath );

			try
			{
				File.Delete( ImageInfoPath );
			}
			catch
			{ }

			// Ensure object loaded and is the same.
			if( i2 is null )
				return Logger.LogReturn( "Failed: Unable to deserialize ImageInfo from file.", false );
			if( !i2.Equals( i1 ) )
				return Logger.LogReturn( "Failed: Deserialized ImageInfo has incorrect values.", false );

			// Create an xml file string from ToString().
			string xml = $"{ Xml.Header }\r\n{ i1 }";
			// Load object from xml string.
			ImageInfo x = XmlLoadable.FromXml<ImageInfo>( xml );

			// Ensure object loaded and is the same.
			if( x is null )
				return Logger.LogReturn( "Failed: Unable to load ImageInfo from xml.", false );
			if( !x.Equals( i1 ) )
				return Logger.LogReturn( "Failed: Xml loaded ImageInfo has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
