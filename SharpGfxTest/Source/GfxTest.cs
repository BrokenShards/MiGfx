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

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimationTest : TestModule
	{
		const string AnimationPath = "animation.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Animation Tests..." );

			Animation anim = new Animation( "test" );

			if( anim.ID.ToLower() != "test" )
				return Logger.LogReturn( "Failed: Animation ID did not get set correctly with constructor.", false );

			for( int i = 0; i < 10; i++ )
				anim.Add( new Frame( new FloatRect( 0, 0, 24, 24 ), Time.FromSeconds( 1.0f ) ) );

			if( anim.Count != 10 )
				return Logger.LogReturn( "Failed: Frame did not get added to the animation correctly.", false );
			if( anim.Length != Time.FromSeconds( 1.0f * 10 ) )
				return Logger.LogReturn( "Failed: Animation length is incorrect.", false );

			if( !BinarySerializable.ToFile( anim, AnimationPath, true ) )
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
			if( !a2.Equals( anim ) )
				return Logger.LogReturn( "Failed: Deserialized Animation has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimationSetTest : TestModule
	{
		const string AnimationSetPath = "animation_set.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running AnimationSet Tests..." );

			AnimationSet set = new AnimationSet();

			for( int i = 0; i < 10; i++ )
			{
				Animation anim = new Animation( "as" + i.ToString() );

				for( int f = 0; f < 10; f++ )
					anim.Add( new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) );

				if( !set.Add( anim ) )
					return Logger.LogReturn( "Failed: Unable to add Animation to AnimationSet.", false );
			}

			if( set.Count != 10 )
				return Logger.LogReturn( "Failed: Not all Animations were added to AnimationSet.", false );

			if( !BinarySerializable.ToFile( set, AnimationSetPath, true ) )
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
			if( !a2.Equals( set ) )
				return Logger.LogReturn( "Failed: Deserialized AnimationSet has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimatorTest : TestModule
	{
		const string AnimatorPath = "animator.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Animator Tests..." );

			Animator anim = new Animator();

			if( !anim.AnimationSet.Add( new Animation( "test", new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) ) ) )
				return Logger.LogReturn( "Failed: Unable to add Animation to Animators' AnimationSet.", false );

			if( !BinarySerializable.ToFile( anim, AnimatorPath, true ) )
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
			if( !a2.Equals( anim ) )
				return Logger.LogReturn( "Failed: Deserialized Animator has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class ImageInfoTest : TestModule
	{
		const string ImageInfoPath = "image_info.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running ImageInfo Tests..." );

			ImageInfo img = new ImageInfo( "image.png", new FloatRect( 0, 0, 40, 40 ), null, Color.Blue );

			if( !BinarySerializable.ToFile( img, ImageInfoPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize ImageInfo to file.", false );

			ImageInfo img2 = BinarySerializable.FromFile<ImageInfo>( ImageInfoPath );

			try
			{
				File.Delete( ImageInfoPath );
			}
			catch
			{ }

			if( img2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize ImageInfo from file.", false );
			if( !img2.Equals( img ) )
				return Logger.LogReturn( "Failed: Deserialized ImageInfo has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

	public class SpriteTest : TestModule
	{
		const string SpritePath = "sprite.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Sprite Tests..." );

			Sprite spr = new Sprite( new ImageInfo( "test.png", new FloatRect( 0, 0, 30, 30 ), null, Color.Red ) );

			spr.Transform.Position = new Vector2f( 100, 100 );
			spr.Transform.LocalSize = new Vector2f( 100, 100 );

			if( !BinarySerializable.ToFile( spr, SpritePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Sprite to file.", false );

			Sprite spr2 = BinarySerializable.FromFile<Sprite>( SpritePath );

			try
			{
				File.Delete( SpritePath );
			}
			catch
			{ }

			if( spr2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Sprite from file.", false );
			if( !spr2.Equals( spr ) )
				return Logger.LogReturn( "Failed: Deserialized Sprite has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimatedSpriteTest : TestModule
	{
		const string AnimatedSpritePath = "animated_sprite.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running AnimatedSprite Tests..." );

			AnimatedSprite spr = new AnimatedSprite( new ImageInfo( "test.png", new FloatRect( 0, 0, 30, 30 ), null, Color.Red ) );

			spr.Transform.Position = new Vector2f( 100, 100 );
			spr.Transform.LocalSize = new Vector2f( 100, 100 );

			if( !spr.Animator.AnimationSet.Add( new Animation( "test", new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) ) ) )
				return Logger.LogReturn( "Failed: Unable to add Animation to AnimatedSprites' Animator.", false );

			if( !BinarySerializable.ToFile( spr, AnimatedSpritePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize AnimatedSprite to file.", false );

			AnimatedSprite spr2 = BinarySerializable.FromFile<AnimatedSprite>( AnimatedSpritePath );

			try
			{
				File.Delete( AnimatedSpritePath );
			}
			catch
			{ }

			if( spr2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimatedSprite from file.", false );
			if( !spr2.Equals( spr ) )
				return Logger.LogReturn( "Failed: Deserialized AnimatedSprite has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

	public class TextStyleTest : TestModule
	{
		const string TextStylePath = "text_style.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running TextStyle Tests..." );

			TextStyle txt = new TextStyle( "image.png", 33, 0, Color.Red, 1, Color.Blue );

			if( !BinarySerializable.ToFile( txt, TextStylePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize TextStyle to file.", false );

			TextStyle txt2 = BinarySerializable.FromFile<TextStyle>( TextStylePath );

			try
			{
				File.Delete( TextStylePath );
			}
			catch
			{ }

			if( txt2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize TextStyle from file.", false );
			if( !txt2.Equals( txt ) )
				return Logger.LogReturn( "Failed: Deserialized TextStyle has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class TilesetTest : TestModule
	{
		const string TilesetPath = "tileset.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Tileset Tests..." );

			Tileset tile = new Tileset( "test", null, new Vector2u( 64, 64 ), new Vector2u( 4, 4 ), new Vector2u( 4, 4 ) );

			if( !BinarySerializable.ToFile( tile, TilesetPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Tileset to file.", false );

			Tileset tile2 = BinarySerializable.FromFile<Tileset>( TilesetPath );

			try
			{
				File.Delete( TilesetPath );
			}
			catch
			{ }

			if( tile2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Tileset from file.", false );
			if( !tile2.Equals( tile ) )
				return Logger.LogReturn( "Failed: Deserialized Tileset has incorrect values.", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class TransformTest : TestModule
	{
		const string TransformPath = "transform.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Transform Tests..." );

			Transform tran = new Transform();

			if( tran.Position != default( Vector2f ) )
				return Logger.LogReturn( "Failed: Transform initial position is not zero.", false );
			if( tran.LocalSize != new Vector2f( 1.0f, 1.0f ) )
				return Logger.LogReturn( "Failed: Transform initial size is not one.", false );
			if( tran.Scale != new Vector2f( 1.0f, 1.0f ) )
				return Logger.LogReturn( "Failed: Transform initial scale is not one.", false );

			tran.Scale = new Vector2f( 4.0f, 4.0f );

			if( tran.GlobalSize != new Vector2f( 1.0f * 4.0f, 1.0f * 4.0f ) )
				return Logger.LogReturn( "Failed: Transform scaled position is incorrect.", false );

			if( !BinarySerializable.ToFile( tran, TransformPath, true ) )
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
			if( !t2.Equals( tran ) )
				return Logger.LogReturn( "Failed: Deserialized TextStyle has incorrect values.", false );

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
