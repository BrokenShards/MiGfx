////////////////////////////////////////////////////////////////////////////////
// UISprite.cs 
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

namespace MiGfx
{
	/// <summary>
	///   A graphical sprite.
	/// </summary>
	public class UISprite : Sprite, IEquatable<UISprite>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public UISprite()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s"></param>
		public UISprite( UISprite s )
		:	base( s )
		{ }
		/// <summary>
		///   Constructs the sprite with the given image info.
		/// </summary>
		/// <param name="i">
		///   Image info.
		/// </param>
		public UISprite( ImageInfo i )
		:	base( i )
		{ }

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UISprite ); }
		}

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( UITransform ) };
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Sprite ) };
		}

		/// <summary>
		///   Updates the sprite geometry. Call this before drawing.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			UITransform t = Parent?.GetComponent<UITransform>();

			if( Image != null && t != null )
				for( uint i = 0; i < m_verts.VertexCount; i++ )
					m_verts[ i ] = Image.GetVertex( i, t.GlobalPixelBounds );
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
		public bool Equals( UISprite other )
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
			return new UISprite( this );
		}
	}
}
