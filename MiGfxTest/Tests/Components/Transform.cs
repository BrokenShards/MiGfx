////////////////////////////////////////////////////////////////////////////////
// Transform.cs 
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
	public class TransformTest : TestModule
	{
		const string TransformPath = "transform.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Transform Tests..." );

			Transform t1 = new Transform();

			if( t1.Position != default( Vector2f ) )
				return Logger.LogReturn( "Failed: Transform initial position is not zero.", false );
			if( t1.Size != new Vector2f( 1.0f, 1.0f ) )
				return Logger.LogReturn( "Failed: Transform initial size is not one.", false );
			if( t1.Scale != new Vector2f( 1.0f, 1.0f ) )
				return Logger.LogReturn( "Failed: Transform initial scale is not one.", false );

			t1.Scale = new Vector2f( 4.0f, 4.0f );

			if( t1.GlobalSize != new Vector2f( 1.0f * 4.0f, 1.0f * 4.0f ) )
				return Logger.LogReturn( "Failed: Transform scaled position is incorrect.", false );

			if( !BinarySerializable.ToFile( t1, TransformPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Transform to file.", false );

			Transform t2 = BinarySerializable.FromFile<Transform>( TransformPath );

			try
			{
				File.Delete( TransformPath );
			}
			catch
			{ }

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Transform from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized TextStyle has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + t1.ToString();
			Transform x = XmlLoadable.FromXml<Transform>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Transform from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Transform has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
