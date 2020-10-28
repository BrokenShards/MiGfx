////////////////////////////////////////////////////////////////////////////////
// Test.cs 
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

using System;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

using SharpGfx;
using SharpLogger;
using SharpSerial;
using Sprite = SharpGfx.Sprite;
using Transform = SharpGfx.Transform;

namespace SharpGfxTest
{
	class Test
	{
		static readonly string FontPath    = FolderPaths.Fonts    + "test.otf";
		static readonly string SoundPath   = FolderPaths.Sounds   + "test.wav";
		static readonly string TexturePath = FolderPaths.Textures + "test.png";

		const string FramePath        = "frame.bin";
		const string AnimationPath    = "animation.bin";
		const string AnimationSetPath = "animation_set.bin";
		const string AnimatorPath     = "animator.bin";
		const string ImageInfoPath    = "image_info.bin";
		const string SpritePath       = "sprite.bin";
		const string TextStylePath    = "text_style.bin";
		const string TilesetPath      = "tileset.bin";
		const string TransformPath    = "transform.bin";

		static void Main( string[] args )
		{
			Logger.LogToConsole = true;
			Logger.LogToFile    = false;

			Logger.Log( "Running SharpGfx Tests..." );

			bool result = true;

			if( !AssetTests() )
				result = false;
			if( !GraphicsTests() )
				result = false;

			Logger.Log( result ? "All tests ran successfully." : "One or more tests failed." );

			Logger.Log( "Press enter to exit." );
			Console.ReadLine();
		}

		static bool AssetTests()
		{
			bool result = true;

			if( !FontTest() )
				result = false;
			if( !SoundTest() )
				result = false;
			if( !TextureTest() )
				result = false;

			return result;
		}

		static bool FontTest()
		{
			Logger.Log( "Running FontManager Tests..." );

			Font font = Assets.Manager.Get<Font>( FontPath );

			if( font == null )
				return Logger.LogReturn( "Failed: Unable to load font from path.", false );

			Assets.Manager.Font.Unload( FontPath );

			return Logger.LogReturn( "Success!", true );
		}
		static bool SoundTest()
		{
			Logger.Log( "Running SoundManager Tests..." );

			SoundBuffer snd = Assets.Manager.Get<SoundBuffer>( SoundPath );

			if( snd == null )
				return Logger.LogReturn( "Failed: Unable to load sound from path.", false );

			Assets.Manager.Sound.Unload( SoundPath );

			return Logger.LogReturn( "Success!", true );
		}
		static bool TextureTest()
		{
			Logger.Log( "Running TextureManager Tests..." );

			Texture tex = Assets.Manager.Get<Texture>( TexturePath );

			if( tex == null )
				return Logger.LogReturn( "Failed: Unable to load texture from path.", false );

			Assets.Manager.Sound.Unload( TexturePath );

			return Logger.LogReturn( "Success!", true );
		}

		static bool GraphicsTests()
		{
			bool result = true;

			if( !FrameTest() )
				result = false;
			if( !AnimationTest() )
				result = false;
			if( !AnimationSetTest() )
				result = false;
			if( !AnimatorTest() )
				result = false;
			if( !ImageInfoTest() )
				result = false;
			if( !SpriteTest() )
				result = false;
			if( !TextStyleTest() )
				result = false;
			if( !TilesetTest() )
				result = false;
			if( !TransformTest() )
				result = false;

			return result;
		}

		static bool FrameTest()
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

			if( f2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Frame from file.", false );
			if( !f2.Equals( f1 ) )
				return Logger.LogReturn( "Failed: Deserialized Frame has incorrect values.", false );

			try
			{
				File.Delete( FramePath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool AnimationTest()
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

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Animation from file.", false );
			if( !a2.Equals( anim ) )
				return Logger.LogReturn( "Failed: Deserialized Animation has incorrect values.", false );

			try
			{
				File.Delete( AnimationPath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool AnimationSetTest()
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

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimationSet from file.", false );
			if( !a2.Equals( set ) )
				return Logger.LogReturn( "Failed: Deserialized AnimationSet has incorrect values.", false );

			try
			{
				File.Delete( AnimationSetPath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool AnimatorTest()
		{
			Logger.Log( "Running Animator Tests..." );

			Animator anim = new Animator();

			if( !anim.AnimationSet.Add( new Animation( "test", new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) ) ) )
				return Logger.LogReturn( "Failed: Unable to add Animation to Animators' AnimationSet.", false );

			if( !BinarySerializable.ToFile( anim, AnimatorPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Animator to file.", false );

			Animator a2 = BinarySerializable.FromFile<Animator>( AnimatorPath );

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Animator from file.", false );
			if( !a2.Equals( anim ) )
				return Logger.LogReturn( "Failed: Deserialized Animator has incorrect values.", false );

			try
			{
				File.Delete( AnimatorPath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool ImageInfoTest()
		{
			Logger.Log( "Running ImageInfo Tests..." );

			ImageInfo img = new ImageInfo( "image.png", new FloatRect( 0, 0, 40, 40 ), Color.Blue );

			if( !BinarySerializable.ToFile( img, ImageInfoPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize ImageInfo to file.", false );

			ImageInfo img2 = BinarySerializable.FromFile<ImageInfo>( ImageInfoPath );

			if( img2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize ImageInfo from file.", false );
			if( !img2.Equals( img ) )
				return Logger.LogReturn( "Failed: Deserialized ImageInfo has incorrect values.", false );

			try
			{
				File.Delete( ImageInfoPath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool SpriteTest()
		{
			Logger.Log( "Running Sprite Tests..." );

			Sprite spr = new Sprite( new ImageInfo( "test.png", new FloatRect( 0, 0, 30, 30 ), Color.Red ) );

			spr.Transform.Position  = new Vector2f( 100, 100 );
			spr.Transform.LocalSize = new Vector2f( 100, 100 );

			if( !BinarySerializable.ToFile( spr, SpritePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Sprite to file.", false );

			Sprite spr2 = BinarySerializable.FromFile<Sprite>( SpritePath );

			if( spr2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Sprite from file.", false );
			if( !spr2.Equals( spr ) )
				return Logger.LogReturn( "Failed: Deserialized Sprite has incorrect values.", false );

			try
			{
				File.Delete( SpritePath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool TextStyleTest()
		{
			Logger.Log( "Running TextStyle Tests..." );

			TextStyle txt = new TextStyle( "image.png", 33, 0, Color.Red, 1, Color.Blue );

			if( !BinarySerializable.ToFile( txt, TextStylePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize TextStyle to file.", false );

			TextStyle txt2 = BinarySerializable.FromFile<TextStyle>( TextStylePath );

			if( txt2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize TextStyle from file.", false );
			if( !txt2.Equals( txt ) )
				return Logger.LogReturn( "Failed: Deserialized TextStyle has incorrect values.", false );

			try
			{
				File.Delete( TextStylePath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool TilesetTest()
		{
			Logger.Log( "Running Tileset Tests..." );

			Tileset tile = new Tileset( "test", null, new Vector2u( 64, 64 ), new Vector2u( 4, 4 ), new Vector2u( 4, 4 ) );

			if( !BinarySerializable.ToFile( tile, TilesetPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Tileset to file.", false );

			Tileset tile2 = BinarySerializable.FromFile<Tileset>( TilesetPath );

			if( tile2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Tileset from file.", false );
			if( !tile2.Equals( tile ) )
				return Logger.LogReturn( "Failed: Deserialized Tileset has incorrect values.", false );

			try
			{
				File.Delete( TilesetPath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
		static bool TransformTest()
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

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Transform from file.", false );
			if( !t2.Equals( tran ) )
				return Logger.LogReturn( "Failed: Deserialized TextStyle has incorrect values.", false );

			try
			{
				File.Delete( TransformPath );
			}
			catch
			{ }

			return Logger.LogReturn( "Success!", true );
		}
	}
}
