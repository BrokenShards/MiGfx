////////////////////////////////////////////////////////////////////////////////
// SpriteAnimator.cs 
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
using MiInput;
using System.IO;
using SFML.Graphics;
using SFML.System;

namespace MiGfx.Test
{
	public class SpriteAnimatorTest : VisualTestModule
	{
		const string AnimatorPath = "animator.bin";

		protected override bool OnTest()
		{
			Logger.Log( "Running SpriteAnimator Tests..." );

			SpriteAnimator a1 = new SpriteAnimator();

			if( !a1.Animations.Add( new Animation( "test", new Frame( new FloatRect( 0, 0, 30, 30 ), Time.FromSeconds( 1.0f ) ) ) ) )
				return Logger.LogReturn( "Failed: Unable to add Animation to AnimationSet.", false );

			if( !BinarySerializable.ToFile( a1, AnimatorPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize SpriteAnimator to file.", false );

			SpriteAnimator a2 = BinarySerializable.FromFile<SpriteAnimator>( AnimatorPath );

			try
			{
				File.Delete( AnimatorPath );
			}
			catch
			{ }

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize SpriteAnimator from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized SpriteAnimator has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + a1.ToString();
			SpriteAnimator x = XmlLoadable.FromXml<SpriteAnimator>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load SpriteAnimator from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded SpriteAnimator has incorrect values.", false );

			a1?.Dispose();
			a2?.Dispose();
			x?.Dispose();

			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running SpriteAnimator Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			using( MiEntity ent = new MiEntity( "tester", window ) )
			{
				if( !ent.Components.AddNew<SpriteAnimator>() )
					return Logger.LogReturn( "Failed: Unable to add SpriteAnimator to test entity.", false );

				Transform      trn  = ent.Components.Get<Transform>();
				Sprite         spr  = ent.Components.Get<Sprite>();
				SpriteAnimator anim = ent.Components.Get<SpriteAnimator>();

				spr.Image = new ImageInfo( Test.AnimationTexturePath );

				Texture tex = Assets.Manager.Texture.Get( spr.Image.Path );

				if( tex == null )
					return Logger.LogReturn( "Failed: Unable to load texture.", false );

				Vector2u texsize = tex.Size;

				trn.Size   = new Vector2f( 200, 200 );
				trn.Center = window.GetView().Center;

				{
					Animation a = new Animation( "test",
						new Frame( new FloatRect( 0,                       0, texsize.X / 3.0f, texsize.Y ), Time.FromSeconds( 0.5f ) ),
						new Frame( new FloatRect( texsize.X / 3.0f,        0, texsize.X / 3.0f, texsize.Y ), Time.FromSeconds( 0.5f ) ),
						new Frame( new FloatRect( texsize.X / 3.0f * 2.0f, 0, texsize.X / 3.0f, texsize.Y ), Time.FromSeconds( 0.5f ) ) );

					if( !anim.Animations.Add( a ) )
						return Logger.LogReturn( "Failed: Unable to add animation.", false );
				}

				anim.Loop = true;
				anim.Play( "test" );

				Logger.Log( "Is colour changing square displayed on window? (y/n)" );
				bool? inp = null;

				while( window.IsOpen && inp == null )
				{
					window.DispatchEvents();

					Input.Manager.Update();
					ent.Update( 1.0f );

					if( Input.Manager.Keyboard.JustPressed( "Y" ) )
						inp = true;
					else if( Input.Manager.Keyboard.JustPressed( "N" ) )
						inp = false;

					window.Clear();
					window.Draw( ent );
					window.Display();
				}

				if( inp == null || !inp.Value )
					return Logger.LogReturn( "Failed: SpriteAnimator did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
