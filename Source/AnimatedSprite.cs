////////////////////////////////////////////////////////////////////////////////
// Sprite.cs 
////////////////////////////////////////////////////////////////////////////////
//
// MiGfx - A basic graphics library for use with SFML.Net.
// Copyright (C) 2021 Michael Furlong <michaeljfurlong@outlook.com>
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
using System.Text;
using System.Xml;

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   An animated sprite.
	/// </summary>
	public class AnimatedSprite : Sprite, IEquatable<AnimatedSprite>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public AnimatedSprite()
		: base()
		{
			Animator = new Animator();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="a">
		///   The sprite to copy from.
		/// </param>
		public AnimatedSprite( AnimatedSprite a )
		: base( a )
		{
			Animator = a.Animator == null ? null : new Animator( a.Animator );
		}
		/// <summary>
		///   Constructs the sprite with the given image info and optional animator.
		/// </summary>
		/// <param name="i">
		///   Image info.
		/// </param>
		/// /// <param name="a">
		///   Animator for sprite animation.
		/// </param>
		public AnimatedSprite( ImageInfo i, Animator a = null )
		: base( i )
		{
			Animator = a ?? new Animator();
		}

		/// <summary>
		///   The animator that controls the sprite's animations.
		/// </summary>
		public Animator Animator
		{
			get; set;
		}

		/// <summary>
		///   Updates the animated sprite. Call before drawing.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		public override void Update( float dt )
		{
			if( Animator != null )
			{
				Animator.Update( dt );

				if( Animator.CurrentFrame != null )
					Image.Rect = Animator.CurrentFrame.Rect;
			}

			base.Update( dt );
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader
		/// </param>
		/// <returns>
		///   True if the sprite was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( Animator == null )
				Animator = new Animator();

			if( !base.LoadFromStream( br ) )
				return false;
			if( !Animator.LoadFromStream( br ) )
				return Logger.LogReturn( "Unable to load AnimatedSprite's Animator from stream.", false, LogType.Error );

			return true;
		}
		/// <summary>
		///   Writes the object to the stream.
		/// </summary>
		/// <param name="bw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the sprite was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( Animator == null )
				Animator = new Animator();

			if( !base.SaveToStream( bw ) )
				return false;
			if( !Animator.SaveToStream( bw ) )
				return Logger.LogReturn( "Unable to save AnimatedSprite's Animator to stream.", false, LogType.Error );

			return true;
		}

		/// <summary>
		///   Attempts to load the object from the xml element.
		/// </summary>
		/// <param name="element">
		///   The xml element.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded, otherwise false.
		/// </returns>
		public override bool LoadFromXml( XmlElement element )
		{
			if( !base.LoadFromXml( element ) )
				return false;

			Animator = new Animator();

			XmlElement anim = element[ nameof( MiGfx.Animator ) ];

			if( anim == null )
				return Logger.LogReturn( "Failed loading AnimatedSprite: No Animator xml element.", false, LogType.Error );
			if( !Animator.LoadFromXml( anim ) )
				return Logger.LogReturn( "Failed loading AnimatedSprite: Loading Animator failed.", false, LogType.Error );

			return true;
		}

		/// <summary>
		///   Converts the object to an xml string.
		/// </summary>
		/// <returns>
		///   Returns the object to an xml string.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( nameof( AnimatedSprite ) );
			sb.AppendLine( ">" );

			sb.AppendLine( XmlLoadable.ToString( Image, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Animator, 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( AnimatedSprite ) );
			sb.AppendLine( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   If this object has the same values of the other object.
		/// </summary>
		/// <param name="other">
		///   The other object to check against.
		/// </param>
		/// <returns>
		///   True if both objects are concidered equal and false if they are not.
		/// </returns>
		public bool Equals( AnimatedSprite other )
		{
			return other != null &&
				   base.Equals( other ) &&
				   ( ( Animator == null && other.Animator == null ) ||
				   Animator.Equals( other.Animator ) );
		}
	}
}
