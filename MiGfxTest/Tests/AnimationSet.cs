////////////////////////////////////////////////////////////////////////////////
// AnimationSet.cs 
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
	public class AnimationSetTest : TestModule
	{
		const string AnimationSetPath = "animation_set.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running AnimationSet Tests..." );

			// Create animation set.
			AnimationSet a1 = new AnimationSet();

			// Create and add animations.
			for( int i = 0; i < 10; i++ )
			{
				Animation anim = new Animation( "as" + i.ToString() );

				for( int f = 0; f < 10; f++ )
					anim.Add( new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) );

				// Add animation to set.
				if( !a1.Add( anim ) )
					return Logger.LogReturn( "Failed: Unable to add Animation to AnimationSet.", false );
			}

			// Ensure all animations were actually added.
			if( a1.Count != 10 )
				return Logger.LogReturn( "Failed: Not all Animations were added to AnimationSet.", false );

			// Write object to a binary file.
			if( !BinarySerializable.ToFile( a1, AnimationSetPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize AnimationSet to file.", false );

			// Read the object from a binary file.
			AnimationSet a2 = BinarySerializable.FromFile<AnimationSet>( AnimationSetPath );

			try
			{
				File.Delete( AnimationSetPath );
			}
			catch
			{ }

			// Ensure object loaded and is the same.
			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimationSet from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized AnimationSet has incorrect values.", false );

			// Create an xml file string from ToString().
			string xml = Xml.Header + "\r\n" + a1.ToString();
			// Load object from xml string.
			AnimationSet x = XmlLoadable.FromXml<AnimationSet>( xml );

			// Ensure object loaded and is the same.
			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load AnimationSet from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded AnimationSet has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
