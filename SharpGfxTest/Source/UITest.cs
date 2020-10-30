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

using SharpGfx;
using SharpGfx.UI;
using SharpLogger;
using SharpSerial;

namespace SharpGfxTest
{
	partial class Test
	{
		const string AnimatedImagePath = "animated_image.bin";
		const string ButtonPath        = "button.bin";
		const string CheckBoxPath      = "checkbox.bin";
		const string ImagePath         = "image.bin";
		const string LabelPath         = "label.bin";
		const string SliderPath        = "slider.bin";
		const string TextBoxPath       = "text_box.bin";

		static bool UITests()
		{
			bool result = true;

			if( !ImageTest() )
				result = false;
			if( !LabelTest() )
				result = false;
			if( !ButtonTest() )
				result = false;
			if( !CheckBoxTest() )
				result = false;
			if( !TextBoxTest() )
				result = false;
			if( !AnimatedImageTest() )
				result = false;
			if( !SliderTest() )
				result = false;

			return result;
		}

		static bool AnimatedImageTest()
		{
			Logger.Log( "Running UIAnimatedImage Tests..." );

			AnimatedImage a1 = new AnimatedImage( "animated_test" );

			a1.Sprite.Image = new ImageInfo( TexturePath );
			a1.Sprite.Animator.AnimationSet.Add( new Animation( "test_animation", new Frame( new FloatRect( 0, 0, 50, 50 ) ) ) );
			
			if( !BinarySerializable.ToFile( a1, AnimatedImagePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UIAnimatedImage to file.", false );

			AnimatedImage a2 = BinarySerializable.FromFile<AnimatedImage>( AnimatedImagePath );

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UIAnimatedImage from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized UIAnimatedImage has incorrect values.", false );

			try
			{
				File.Delete( AnimatedImagePath );
			}
			catch
			{ }

			a1.Dispose();
			a2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		static bool ButtonTest()
		{
			Logger.Log( "Running UIButton Tests..." );

			Button b1 = Button.Default( "button_test", "test" );

			if( !BinarySerializable.ToFile( b1, ButtonPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UIButton to file.", false );

			Button b2 = BinarySerializable.FromFile<Button>( ButtonPath );

			if( b2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UIButton from file.", false );
			if( !b2.Equals( b1 ) )
				return Logger.LogReturn( "Failed: Deserialized UIButton has incorrect values.", false );

			try
			{
				File.Delete( ButtonPath );
			}
			catch
			{ }

			b1.Dispose();
			b2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		static bool CheckBoxTest()
		{
			Logger.Log( "Running UICheckBox Tests..." );

			CheckBox c1 = CheckBox.Default( "check_test", false );

			if( c1.Checked )
				return Logger.LogReturn( "Failed: Default constructed object with incorrect checked value.", false );
			if( !BinarySerializable.ToFile( c1, CheckBoxPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UICheckBox to file.", false );

			CheckBox c2 = BinarySerializable.FromFile<CheckBox>( CheckBoxPath );

			if( c2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UICheckBox from file.", false );
			if( !c2.Equals( c1 ) )
				return Logger.LogReturn( "Failed: Deserialized UICheckBox has incorrect values.", false );

			try
			{
				File.Delete( CheckBoxPath );
			}
			catch
			{ }

			c1.Dispose();
			c2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		
		static bool ImageTest()
		{
			Logger.Log( "Running UIImage Tests..." );

			SharpGfx.UI.Image i1 = new SharpGfx.UI.Image( "img_test" );

			if( !BinarySerializable.ToFile( i1, ImagePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UIImage to file.", false );

			SharpGfx.UI.Image i2 = BinarySerializable.FromFile<SharpGfx.UI.Image>( ImagePath );

			if( i2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UIImage from file.", false );
			if( !i2.Equals( i1 ) )
				return Logger.LogReturn( "Failed: Deserialized UIImage has incorrect values.", false );

			try
			{
				File.Delete( ImagePath );
			}
			catch
			{ }

			i1.Dispose();
			i2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		static bool LabelTest()
		{
			Logger.Log( "Running UILabel Tests..." );

			Label l1 = new Label( "label_test" );

			if( !BinarySerializable.ToFile( l1, LabelPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UILabel to file.", false );

			Label l2 = BinarySerializable.FromFile<Label>( LabelPath );

			if( l2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UILabel from file.", false );
			if( !l2.Equals( l1 ) )
				return Logger.LogReturn( "Failed: Deserialized UILabel has incorrect values.", false );

			try
			{
				File.Delete( LabelPath );
			}
			catch
			{ }

			l1.Dispose();
			l2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		static bool SliderTest()
		{
			Logger.Log( "Running UISlider Tests..." );

			Slider s1 = new Slider( "slider_test" );

			if( !BinarySerializable.ToFile( s1, SliderPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UISlider to file.", false );

			Slider s2 = BinarySerializable.FromFile<Slider>( SliderPath );

			if( s2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UISlider from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized UISlider has incorrect values.", false );

			try
			{
				File.Delete( SliderPath );
			}
			catch
			{ }

			s1.Dispose();
			s2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		static bool TextBoxTest()
		{
			Logger.Log( "Running UITextBox Tests..." );

			TextBox t1 = new TextBox( "textbox_test" );

			if( !BinarySerializable.ToFile( t1, TextBoxPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UITextBox to file.", false );

			TextBox t2 = BinarySerializable.FromFile<TextBox>( TextBoxPath );

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UITextBox from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized UITextBox has incorrect values.", false );

			try
			{
				File.Delete( TextBoxPath );
			}
			catch
			{ }

			t1.Dispose();
			t2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
	}
}
