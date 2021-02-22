////////////////////////////////////////////////////////////////////////////////
// UISpriteAnimator.cs 
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
	public class UISpriteAnimatorTest : VisualTestModule
	{
		const string UIAnimatorPath = "uianimator.bin";

		protected override bool OnTest()
		{
			if( !TestDB.CreateAnimations() )
				return Logger.LogReturn( "Failed creating test animations, skipping UISpriteAnimator Tests...", false );

			Logger.Log( "Running UISpriteAnimator Tests..." );

			AnimationDB db = DatabaseManager.Instance.Get<AnimationDB, Animation>();

			UISpriteAnimator a1 = new UISpriteAnimator();

			foreach( var v in db )
				if( !a1.Animations.Add( v.Key ) )
					return Logger.LogReturn( "Failed: Unable to add Animation ID to AnimationSet.", false );

			if( !BinarySerializable.ToFile( a1, UIAnimatorPath, true ) )
				return Logger.LogReturn( "Failed: Unable to serialize UISpriteAnimator to file.", false );

			UISpriteAnimator a2 = BinarySerializable.FromFile<UISpriteAnimator>( UIAnimatorPath );

			try
			{
				File.Delete( UIAnimatorPath );
			}
			catch
			{ }

			if( a2 == null )
				return Logger.LogReturn( "Failed: Unable to deserialize UISpriteAnimator from file.", false );
			if( !a2.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Deserialized UISpriteAnimator has incorrect values.", false );

			string xml = Xml.Header + "\r\n" + a1.ToString();
			UISpriteAnimator x = XmlLoadable.FromXml<UISpriteAnimator>( xml );

			if( x == null )
				return Logger.LogReturn( "Failed: Unable to load UISpriteAnimator from xml.", false );
			if( !x.Equals( a1 ) )
				return Logger.LogReturn( "Failed: Xml loaded UISpriteAnimator has incorrect values.", false );

			a1?.Dispose();
			a2?.Dispose();
			x?.Dispose();

			return Logger.LogReturn( "Success!", true );
		}
		protected override bool OnVisualTest( RenderWindow window )
		{
			Logger.Log( "Running UISpriteAnimator Visual Tests..." );

			if( window == null || !window.IsOpen )
				return Logger.LogReturn( "Failed: Test window is null or closed.", false );

			using( MiEntity ent = new MiEntity( "tester", window ) )
			{
				if( !ent.AddNewComponent<UISpriteAnimator>() )
					return Logger.LogReturn( "Failed: Unable to add UISpriteAnimator to test entity.", false );

				AnimationDB      db   = DatabaseManager.Instance.Get<AnimationDB, Animation>();
				UITransform      trn  = ent.GetComponent<UITransform>();
				UISprite         spr  = ent.GetComponent<UISprite>();
				UISpriteAnimator anim = ent.GetComponent<UISpriteAnimator>();

				spr.Image = new ImageInfo( Test.AnimationTexturePath );

				Texture tex = Assets.Manager.Texture.Get( spr.Image.Path );

				if( tex == null )
					return Logger.LogReturn( "Failed: Unable to load texture.", false );

				Vector2u texsize = tex.Size;

				trn.Origin   = Allignment.Middle;
				trn.Size     = new Vector2f( 0.4f, 0.4f );
				trn.Position = new Vector2f( 0.5f, 0.5f );

				foreach( var v in db )
					if( !anim.Animations.Add( v.Key ) )
						return Logger.LogReturn( "Failed: Unable to add Animation ID to AnimationSet.", false );

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
					return Logger.LogReturn( "Failed: UISpriteAnimator did not display correctly (user input).", false );
			}

			return Logger.LogReturn( "Success!", true );
		}
	}
}
