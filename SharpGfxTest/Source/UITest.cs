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
using SFInput;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using SharpGfx;
using SharpGfx.UI;
using SharpLogger;
using SharpSerial;

using Image = SharpGfx.UI.Image;

namespace SharpGfxTest
{
	public class ImageTest : VisualTestModule
	{
		const string ImagePath = "image.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Image Tests..." );

			Image i1 = new Image( "img_test" );
			i1.DisplayImage = new ImageInfo( Test.TexturePath );
			i1.Transform.LocalSize = new Vector2f( 200, 200 );

			if( !BinarySerializable.ToFile( i1, ImagePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Image to file.", false );

			Image i2 = BinarySerializable.FromFile<Image>( ImagePath );

			try
			{
				File.Delete( ImagePath );
			}
			catch
			{ }

			if( i2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Image from file.", false );
			if( !i2.Equals( i1 ) )
				return Logger.LogReturn( "Failed: Deserialized Image has incorrect values.", false );

			i1.Dispose();
			i2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running Image Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			Image img = new Image( "img_test" );
			img.DisplayImage = new ImageInfo( Test.TexturePath );
			img.Transform.Center = window.GetView().Center;
			img.Transform.LocalSize = new Vector2f( 200, 200 );

			if( !Test.Manager.Add( img ) )
				return Logger.LogReturn( "Failed: Unable to add Image to UIManager.", false );

			Logger.Log( "Is yellow square displayed on window? (y/n)" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();

				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: Image did not display correctly (user input).", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class AnimatedImageTest : VisualTestModule
	{
		const string AnimatedImagePath = "animated_image.bin";

		static readonly string AnimationTexturePath = FolderPaths.Textures + "test_anim.png";

		protected override bool OnTest()
		{
			Logger.Log( "Running AnimatedImage Tests..." );

			AnimatedImage a1 = new AnimatedImage( "animated_test" );
			a1.Sprite.Image = new ImageInfo( AnimationTexturePath );

			{
				Animation anim = new Animation( "test_anim" );

				for( int i = 0; i < 3; i++ )
					anim.Add( new Frame( new FloatRect( i * 512, 0, 512, 512 ) ) );

				a1.Sprite.Animator.AnimationSet.Add( anim );
			}

			if( !BinarySerializable.ToFile( a1, AnimatedImagePath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize AnimatedImage to file.", false );

			AnimatedImage a2 = BinarySerializable.FromFile<AnimatedImage>( AnimatedImagePath );

			try
			{
				File.Delete( AnimatedImagePath );
			}
			catch
			{ }

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize AnimatedImage from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized AnimatedImage has incorrect values.", false );

			a1.Dispose();
			a2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running AnimatedImage Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			AnimatedImage animg = new AnimatedImage( "animated_test" );
			animg.Sprite.Image = new ImageInfo( AnimationTexturePath );

			{
				Animation anim = new Animation( "test_anim" );

				for( int i = 0; i < 3; i++ )
					anim.Add( new Frame( new FloatRect( i * 512, 0, 512, 512 ) ) );

				animg.Sprite.Animator.AnimationSet.Add( anim );
			}

			animg.Transform.Center = window.GetView().Center;
			animg.Transform.LocalSize = new Vector2f( 200, 200 );

			animg.Sprite.Animator.Loop = true;
			animg.Sprite.Animator.Play( "test_anim" );

			if( !Test.Manager.Add( animg ) )
				return Logger.LogReturn( "Failed: Unable to add AnimatedImage to UIManager.", false );

			Logger.Log( "Is colour changing square displayed on window? (y/n)" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();
				
				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: AnimatedImage did not display correctly (user input).", false );
		
			return Logger.LogReturn( "Success!", true );
		}
	}
	public class LabelTest : VisualTestModule
	{
		const string LabelPath = "label.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Label Tests..." );

			Label l1 = Label.Default( "label_test", "Test" );

			if( !BinarySerializable.ToFile( l1, LabelPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Label to file.", false );

			Label l2 = BinarySerializable.FromFile<Label>( LabelPath );

			try
			{
				File.Delete( LabelPath );
			}
			catch
			{ }

			if( l2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Label from file.", false );
			if( !l2.Equals( l1 ) )
				return Logger.LogReturn( "Failed: Deserialized Label has incorrect values.", false );

			l1.Dispose();
			l2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running Label Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			Label lab = Label.Default( "label_test", "Test" );
			lab.Transform.Center = window.GetView().Center;

			if( !Test.Manager.Add( lab ) )
				return Logger.LogReturn( "Failed: Unable to add Label to UIManager.", false );

			Logger.Log( "Is label text displayed on window? (y/n)" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();

				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: Label did not display correctly (user input).", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class ButtonTest : VisualTestModule
	{
		const string ButtonPath = "button.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Button Tests..." );

			Button b1 = Button.Default( "button_test", "test" );

			if( !BinarySerializable.ToFile( b1, ButtonPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Button to file.", false );

			Button b2 = BinarySerializable.FromFile<Button>( ButtonPath );

			try
			{
				File.Delete( ButtonPath );
			}
			catch
			{ }

			if( b2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Button from file.", false );
			if( !b2.Equals( b1 ) )
				return Logger.LogReturn( "Failed: Deserialized Button has incorrect values.", false );

			b1.Dispose();
			b2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running Button Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			Button lab = Button.Default( "label_test", "Test" );
			lab.Transform.Center = window.GetView().Center;

			if( !Test.Manager.Add( lab ) )
				return Logger.LogReturn( "Failed: Unable to add Button to UIManager.", false );

			Logger.Log( "Is button displayed on window? (y/n)" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();

				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: Button did not display correctly (user input).", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class CheckBoxTest : VisualTestModule
	{
		const string CheckBoxPath = "checkbox.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running CheckBox Tests..." );

			CheckBox c1 = CheckBox.Default( "check_test", false );

			if( c1.Checked )
				return Logger.LogReturn( "Failed: Default constructed object with incorrect checked value.", false );
			if( !BinarySerializable.ToFile( c1, CheckBoxPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize CheckBox to file.", false );

			CheckBox c2 = BinarySerializable.FromFile<CheckBox>( CheckBoxPath );

			try
			{
				File.Delete( CheckBoxPath );
			}
			catch
			{ }

			if( c2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize CheckBox from file.", false );
			if( !c2.Equals( c1 ) )
				return Logger.LogReturn( "Failed: Deserialized CheckBox has incorrect values.", false );

			c1.Dispose();
			c2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running CheckBox Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			CheckBox check = CheckBox.Default( "check_test", false );
			check.Transform.Center = window.GetView().Center;

			if( !Test.Manager.Add( check ) )
				return Logger.LogReturn( "Failed: Unable to add CheckBox to UIManager.", false );

			Logger.Log( "Is checkbox displayed on window? (y/n)" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();

				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: CheckBox did not display correctly (user input).", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

	public class TextBoxTest : VisualTestModule
	{
		const string TextBoxPath = "text_box.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running TextBox Tests..." );

			TextBox t1 = TextBox.Default( "textbox_test", true, true );

			if( !BinarySerializable.ToFile( t1, TextBoxPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize TextBox to file.", false );

			TextBox t2 = BinarySerializable.FromFile<TextBox>( TextBoxPath );

			try
			{
				File.Delete( TextBoxPath );
			}
			catch
			{ }

			if( t2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize TextBox from file.", false );
			if( !t2.Equals( t1 ) )
				return Logger.LogReturn( "Failed: Deserialized TextBox has incorrect values.", false );

			t1.Dispose();
			t2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running TextBox Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			TextBox box = TextBox.Default( "textbox_test", true, true );
			box.String = "Testing text box.";
			box.Transform.Position = new Vector2f( 0, 0 );

			if( !Test.Manager.Add( box ) )
				return Logger.LogReturn( "Failed: Unable to add TextBox to UIManager.", false );

			Logger.Log( "Is textbox displayed on window? (y/n)" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();

				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: TextBox did not display correctly (user input).", false );

			return Logger.LogReturn( "Success!", true );
		}
	}
	public class SliderTest : VisualTestModule
	{
		const string SliderPath = "slider.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running Slider Tests..." );

			// TODO: Change to Slider.Default when implemented.
			Slider s1 = new Slider( "slider_test" );

			if( !BinarySerializable.ToFile( s1, SliderPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize Slider to file.", false );

			Slider s2 = BinarySerializable.FromFile<Slider>( SliderPath );

			try
			{
				File.Delete( SliderPath );
			}
			catch
			{ }

			if( s2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize Slider from file.", false );
			if( !s2.Equals( s1 ) )
				return Logger.LogReturn( "Failed: Deserialized Slider has incorrect values.", false );

			s1.Dispose();
			s2.Dispose();
			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running Slider Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );
			if( Test.Manager == null )
				return Logger.LogReturn( "Failed: Test UIManager is null.", false );

			// TODO: Change to Slider.Default when implemented.
			Slider img = new Slider( "slider_test" );
			img.Transform.Center = window.GetView().Center;

			if( !Test.Manager.Add( img ) )
				return Logger.LogReturn( "Failed: Unable to add Slider to UIManager.", false );

			Logger.Log( "Is slider displayed on window? (y/n) [Not implemented, will display nothing!]" );
			bool? inp = null;

			while( window.IsOpen && inp == null )
			{
				window.DispatchEvents();

				Input.Manager.Update();
				Test.Manager.Update( 1.0f );

				if( Input.Manager.Keyboard.JustPressed( "Y" ) )
					inp = true;
				else if( Input.Manager.Keyboard.JustPressed( "N" ) )
					inp = false;

				window.Clear();
				window.Draw( Test.Manager );
				window.Display();
			}

			if( inp == null || !inp.Value )
				return Logger.LogReturn( "Failed: Slider did not display correctly (user input).", false );

			return Logger.LogReturn( "Success!", true );
		}
	}

	partial class Test
	{
		public static UIManager Manager
		{
			get; private set;
		}

		static bool UITests( RenderWindow window )
		{
			bool result = true;
			window.TextEntered += OnTextEntered;
			
			using( Manager = new UIManager( window ) )
			{
				if( !new ImageTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();

				if( !new AnimatedImageTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();

				if( !new LabelTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();

				if( !new ButtonTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();

				if( !new CheckBoxTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();

				if( !new SliderTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();

				if( !new TextBoxTest().RunTest( window ) )
					result = false;
				Manager.RemoveAll();
			}

			window.TextEntered -= OnTextEntered;
			return result;
		}

		private static void OnTextEntered( object sender, TextEventArgs e )
		{
			if( Manager != null )
				Manager.TextEntered( e );
		}
	}
}
