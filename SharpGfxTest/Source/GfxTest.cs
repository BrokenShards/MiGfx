////////////////////////////////////////////////////////////////////////////////
// GfxTest.cs 
////////////////////////////////////////////////////////////////////////////////
//
// SharpGfx - A basic graphics library for use with SFML.Net.
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

using SharpGfx;
using SharpLogger;
using SharpSerial;
using SharpTest;

using Sprite    = SharpGfx.Sprite;
using Transform = SharpGfx.Transform;

namespace SharpGfxTest
{
	public class FrameTest : TestModule
	{
		const string FramePath = "frame.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Frame Tests..." );

			Frame f1 = new Frame( new FloatRect( 0, 0, 24, 24 ), Time.FromSeconds( 1.5f ) );

			if( f1.Rect.Width != 24 || f1.Rect.Height != 24 )
				return Logger.LogReturn( "Failed: Frame rect did not set correctly.", false );
			if( f1.Length != Time.FromSeconds( 1.5f ) )
				return Logger.LogReturn( "Failed: Frame length did not set correctly.", false );

			if( !BinarySerializable.ToFile( f1, FramePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Frame to file.", false );

			Frame f2 = BinarySerializable.FromFile<Frame>( FramePath );

			try
			{
				File.Delete( FramePath );
			}
			catch
			{ }

			if( f2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Frame from file.", false );
			if( !f2.Equals( f1 ) )
				return Logger.LogReturn( "Failed: Deserialized Frame has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + f1.ToString();
			Frame x = XmlLoadable.FromXml<Frame>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Frame from xml.", false );
			if( !x.Equals( f1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Frame has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimationTest : TestModule
	{
		const string AnimationPath = "animation.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Animation Tests..." );

			Animation a1 = new Animation( "test" );

			if( a1.ID.ToLower() != "test" )
				return Logger.LogReturn( "Failed: Animation ID did not get set correctly with constructor.", false );

			for( int i = 0; i < 10; i++ )
				a1.Add( new Frame( new FloatRect( 0, 0, 24, 24 ), Time.FromSeconds( 1.0f ) ) );

			if( a1.Count != 10 )
				return Logger.LogReturn( "Failed: Frame did not get added to the animation correctly.", false );
			if( a1.Length != Time.FromSeconds( 1.0f * 10 ) )
				return Logger.LogReturn( "Failed: Animation length is incorrect.", false );

			if( !BinarySerializable.ToFile( a1, AnimationPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Animation to file.", false );

			Animation a2 = BinarySerializable.FromFile<Animation>( AnimationPath );

			try
			{
				File.Delete( AnimationPath );
			}
			catch
			{ }

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Animation from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized Animation has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + a1.ToString();
			Animation x = XmlLoadable.FromXml<Animation>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Animation from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Animation has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimationSetTest : TestModule
	{
		const string AnimationSetPath = "animation_set.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running AnimationSet Tests..." );

			AnimationSet a1 = new AnimationSet();

			for( int i = 0; i < 10; i++ )
			{
				Animation anim = new Animation( "as" + i.ToString() );

				for( int f = 0; f < 10; f++ )
					anim.Add( new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) );

				if( !a1.Add( anim ) )
					return Logger.LogReturn( "Failed: Unable to add Animation to AnimationSet.", false );
			}

			if( a1.Count != 10 )
				return Logger.LogReturn( "Failed: Not all Animations were added to AnimationSet.", false );

			if( !BinarySerializable.ToFile( a1, AnimationSetPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize AnimationSet to file.", false );

			AnimationSet a2 = BinarySerializable.FromFile<AnimationSet>( AnimationSetPath );

			try
			{
				File.Delete( AnimationSetPath );
			}
			catch
			{ }

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimationSet from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized AnimationSet has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + a1.ToString();
			AnimationSet x = XmlLoadable.FromXml<AnimationSet>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load AnimationSet from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded AnimationSet has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimatorTest : TestModule
	{
		const string AnimatorPath = "animator.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Animator Tests..." );

			Animator a1 = new Animator();

			if( !a1.Animations.Add( new Animation( "test", new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) ) ) )
				return Logger.LogReturn( "Failed: Unable to add Animation to Animators' AnimationSet.", false );

			if( !BinarySerializable.ToFile( a1, AnimatorPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Animator to file.", false );

			Animator a2 = BinarySerializable.FromFile<Animator>( AnimatorPath );

			try
			{
				File.Delete( AnimatorPath );
			}
			catch
			{ }

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Animator from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized Animator has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + a1.ToString();
			Animator x = XmlLoadable.FromXml<Animator>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Animator from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Animator has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class ImageInfoTest : TestModule
	{
		const string ImageInfoPath = "image_info.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running ImageInfo Tests..." );

			ImageInfo i1 = new ImageInfo( "image.png", new FloatRect( 0, 0, 40, 40 ), null, Color.Blue );

			if( !BinarySerializable.ToFile( i1, ImageInfoPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize ImageInfo to file.", false );

			ImageInfo i2 = BinarySerializable.FromFile<ImageInfo>( ImageInfoPath );

			try
			{
				File.Delete( ImageInfoPath );
			}
			catch
			{ }

			if( i2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize ImageInfo from file.", false );
			if( !i2.Equals( i1 ) )
				return Logger.LogReturn( "Failed: Deserialized ImageInfo has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + i1.ToString();
			ImageInfo x = XmlLoadable.FromXml<ImageInfo>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load ImageInfo from xml.", false );
			if( !x.Equals( i1 ) )
				return Logger.LogReturn( "Failed: Xml loaded ImageInfo has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

	public class SpriteTest : TestModule
	{
		const string SpritePath = "sprite.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Sprite Tests..." );

			Sprite s1 = new Sprite( new ImageInfo( "test.png", new FloatRect( 0, 0, 30, 30 ), null, Color.Red ) );

			s1.Transform.Position = new Vector2f( 100, 100 );
			s1.Transform.Size = new Vector2f( 100, 100 );

			if( !BinarySerializable.ToFile( s1, SpritePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Sprite to file.", false );

			Sprite s2 = BinarySerializable.FromFile<Sprite>( SpritePath );

			try
			{
				File.Delete( SpritePath );
			}
			catch
			{ }

			if( s2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Sprite from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized Sprite has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + s1.ToString();
			Sprite x = XmlLoadable.FromXml<Sprite>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Sprite from xml.", false );
			if( !x.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Sprite has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimatedSpriteTest : TestModule
	{
		const string AnimatedSpritePath = "animated_sprite.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running AnimatedSprite Tests..." );

			AnimatedSprite s1 = new AnimatedSprite( new ImageInfo( "test.png", new FloatRect( 0, 0, 30, 30 ), null, Color.Red ) );

			s1.Transform.Position = new Vector2f( 100, 100 );
			s1.Transform.Size = new Vector2f( 100, 100 );

			if( !s1.Animator.Animations.Add( new Animation( "test", new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) ) ) )
				return Logger.LogReturn( "Failed: Unable to add Animation to AnimatedSprites' Animator.", false );

			if( !BinarySerializable.ToFile( s1, AnimatedSpritePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize AnimatedSprite to file.", false );

			AnimatedSprite s2 = BinarySerializable.FromFile<AnimatedSprite>( AnimatedSpritePath );

			try
			{
				File.Delete( AnimatedSpritePath );
			}
			catch
			{ }

			if( s2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimatedSprite from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized AnimatedSprite has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + s1.ToString();
			AnimatedSprite x = XmlLoadable.FromXml<AnimatedSprite>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load AnimatedSprite from xml.", false );
			if( !x.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Xml loaded AnimatedSprite has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

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

			string xml = XmlLoadable.XmlHeader + "\r\n" + t1.ToString();
			TextStyle x = XmlLoadable.FromXml<TextStyle>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load TextStyle from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded TextStyle has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class TilesetTest : TestModule
	{
		const string TilesetPath = "tileset.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Tileset Tests..." );

			Tileset t1 = new Tileset( "test", null, new Vector2u( 64, 64 ), new Vector2u( 4, 4 ), new Vector2u( 4, 4 ) )
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

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Tileset from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized Tileset has incorrect values.", false );

			string xml = XmlLoadable.XmlHeader + "\r\n" + t1.ToString();
			Tileset x = XmlLoadable.FromXml<Tileset>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Tileset from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Tileset has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
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

			string xml = XmlLoadable.XmlHeader + "\r\n" + t1.ToString();
			Transform x = XmlLoadable.FromXml<Transform>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load Transform from xml.", false );
			if( !x.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Xml loaded Transform has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

	partial class Test
	{
		static bool GraphicsTests( RenderWindow window )
		{
			bool result = true;

			if( !new FrameTest().RunTest( window ) )
				result = false;
			if( !new AnimationTest().RunTest( window ) )
				result = false;
			if( !new AnimationSetTest().RunTest( window ) )
				result = false;
			if( !new AnimatorTest().RunTest( window ) )
				result = false;
			if( !new ImageInfoTest().RunTest( window ) )
				result = false;
			if( !new SpriteTest().RunTest( window ) )
				result = false;
			if( !new AnimatedSpriteTest().RunTest( window ) )
				result = false;
			if( !new TextStyleTest().RunTest( window ) )
				result = false;
			if( !new TilesetTest().RunTest( window ) )
				result = false;
			if( !new TransformTest().RunTest( window ) )
				result = false;

			return result;
		}
	}
}
