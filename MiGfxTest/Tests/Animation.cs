////////////////////////////////////////////////////////////////////////////////
// Animation.cs 
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
	public class AnimationTest : TestModule
	{
		const string AnimationPath = "animation.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Animation Tests..." );

			// Create a new animation with an ID.
			Animation a1 = new( "test" );

			// [SANITY CHECK] Ensure ID set correctly.
			if( a1.ID.ToLower() is not "test" )
				return Logger.LogReturn( "Failed: Animation ID did not get set correctly with constructor.", false );

			// Add frames to animation.
			for( int i = 0; i < 10; i++ )
				a1.Add( new Frame( new FloatRect( 0, 0, 24, 24 ), Time.FromSeconds( 1.0f ) ) );

			// [SANITY CHECK] Ensure frames added correctly.
			if( a1.Count is not 10 )
				return Logger.LogReturn( "Failed: Frame did not get added to the animation correctly.", false );
			if( a1.Length != Time.FromSeconds( 1.0f * 10 ) )
				return Logger.LogReturn( "Failed: Animation length is incorrect.", false );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( a1, AnimationPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Animation to file.", false );

			// Read the object from a binary file.
			Animation a2 = BinarySerializable.FromFile<Animation>( AnimationPath );

			try
			{
				// Delete temp file.
				File.Delete( AnimationPath );
			}
			catch
			{ }

			// Ensure object loaded successfully.
			if( a2 is null )
				return Logger.LogReturn( "Failed: Unable to deserialize Animation from file.", false );
			// [SANITY CHECK] Ensure loaded object is the same.
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized Animation has incorrect values.", false );

			// Create an xml file string from ToString().
			string xml = $"{ Xml.Header }\r\n{ a1 }";
			// Load object from xml string.
			Animation x = XmlLoadable.FromXml<Animation>( xml );

			// Ensure object loaded successfully.
			if( x is null )
				return Logger.LogReturn( "Failed: Unable to load Animation from xml.", false );
			// [SANITY CHECK] Ensure loaded object is the same.
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Animation has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
