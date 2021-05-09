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
	public static class TestDB
	{
		public static bool CreateAnimations()
		{
			AnimationDB db = DatabaseManager.Instance.Get<AnimationDB, Animation>();

			if( db is null )
				return Logger.LogReturn( "Unable to get AnimationDB.", false );

			// Create and add animations.
			for( int i = 0; i < 10; i++ )
			{
				Animation anim = new( "as" + i.ToString() );

				for( int f = 0; f < 3; f++ )
					anim.Add( new Frame( new FloatRect( f * 512, 0, 512, 512 ), Time.FromSeconds( 1.0f ) ) );

				// Add animation to set.
				if( !db.Add( anim.ID, anim, true ) )
					return Logger.LogReturn( "Failed adding test animations to AnimationDB.", false );
			}

			return true;
		}
	}

	public class AnimationSetTest : TestModule
	{
		const string AnimationSetPath = "animation_set.bin";

		protected override bool OnTest()
		{
			if( !TestDB.CreateAnimations() )
				return Logger.LogReturn( "Failed creating test animations, skipping AnimationSet Tests...", false );

			Logger.Log( "Running AnimationSet Tests..." );

			AnimationDB db = DatabaseManager.Instance.Get<AnimationDB, Animation>();

			// Create animation set.
			AnimationSet a1 = new();

			// Create and add animations.
			foreach( var v in db )
			{
				// Add animation to set.
				if( !a1.Add( v.Key ) )
					return Logger.LogReturn( "Failed: Unable to add Animation ID to AnimationSet.", false );
			}

			// Ensure all animations were actually added.
			if( a1.Count is not 10 )
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
			if( a2 is null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimationSet from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized AnimationSet has incorrect values.", false );

			// Create an xml file string from ToString().
			string xml = $"{ Xml.Header }\r\n{ a1 }";
			// Load object from xml string.
			AnimationSet x = XmlLoadable.FromXml<AnimationSet>( xml );

			// Ensure object loaded and is the same.
			if( x is null )
				return Logger.LogReturn( "Failed: Unable to load AnimationSet from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded AnimationSet has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
