////////////////////////////////////////////////////////////////////////////////
// UISpriteAnimator.cs 
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
using SFML.System;

namespace MiGfx
{
	/// <summary>
	///   Runs and manages animations and animation sets on sprites.
	/// </summary>
	public class UISpriteAnimator : SpriteAnimator, IEquatable<UISpriteAnimator>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public UISpriteAnimator()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="a"></param>
		public UISpriteAnimator( UISpriteAnimator a )
		:	base( a )
		{ }
		/// <summary>
		///   Constructor setting the initial animation set.
		/// </summary>
		/// <param name="set">
		///   Initial animation set.
		/// </param>
		/// <param name="selected">
		///   The initially selected animation.
		/// </param>
		public UISpriteAnimator( AnimationSet set, int selected = 0 )
		:	base( set, selected )
		{ }

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UISpriteAnimator ); }
		}

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( UISprite ) };
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[ 0 ];
		}

		/// <summary>
		///   Updates the animator.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Animations.Empty )
				return;

			if( Playing && Multiplier > 0.0f )
			{
				Animation anim = CurrentAnimation;

				if( anim.Count > 1 )
				{
					Time len = anim.Get( m_frame ).Length * Multiplier;

					if( m_timer.ElapsedTime >= len )
					{
						m_frame++;
						m_timer.Restart();
					}

					if( m_frame >= anim.Count )
					{
						m_frame   = 0;
						m_playing = Loop;
					}
				}
				else
					m_frame = 0;
			}

			Frame current = CurrentFrame;

			if( Parent != null && current != null )
			{
				UISprite spr = Parent.GetComponent<UISprite>();
				ImageInfo i = spr.Image;
				current.Apply( ref i );
				spr.Image = i;
			}
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
		public bool Equals( UISpriteAnimator other )
		{
			return base.Equals( other );
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new UISpriteAnimator( this );
		}
	}
}
