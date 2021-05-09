////////////////////////////////////////////////////////////////////////////////
// Tileset.cs 
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
using SFML.System;

namespace MiGfx.Test
{
	public class TilesetTest : TestModule
	{
		const string TilesetPath = "tileset.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Tileset Tests..." );

			Tileset t1 = new( "test", null, new Vector2u( 64, 64 ), new Vector2u( 4, 4 ), new Vector2u( 4, 4 ) )
			{
				Texture = "texture"
			};

			if( !BinarySerializable.ToFile( t1, TilesetPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Tileset to file.", false );

			Tileset t2 = BinarySerializable.FromFile<Tileset>( TilesetPath );

			try
			{
				File.Delete( TilesetPath );
			}
			catch
			{ }

			if( t2 is null )
				return Logger.LogReturn( "Failed: Unable to deserialize Tileset from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized Tileset has incorrect values.", false );

			string xml = $"{ Xml.Header }\r\n{ t1 }";
			Tileset x = XmlLoadable.FromXml<Tileset>( xml );

			if( x is null )
				return Logger.LogReturn( "Failed: Unable to load Tileset from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Tileset has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
