////////////////////////////////////////////////////////////////////////////////
// UISpriteArray.cs 
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
using SFML.Graphics;

namespace MiGfx
{
	/// <summary>
	///   A component that manages and draws multiple sprites that use the same texture.
	/// </summary>
	public class UISpriteArray : SpriteArray, IEquatable<UISpriteArray>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public UISpriteArray()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s"></param>
		public UISpriteArray( UISpriteArray s )
		:	base( s )
		{ }
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="path">
		///   The texture path.
		/// </param>
		public UISpriteArray( string path )
		:	base( path )
		{ }

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UISpriteArray ); }
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
			return new string[] { nameof( UISprite ) };
		}

		/// <summary>
		///   Updates the sprite geometry. Call this before drawing.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <see cref="Image"/> is null.
		/// </exception>
		protected override void OnUpdate( float dt )
		{
			if( Parent == null )
				return;

			Texture tex = Assets.Manager.Texture.Get( TexturePath );

			if( tex != null )
			{
				foreach( SpriteInfo si in Sprites )
				{
					FloatRect rect = si.Rect;

					if( rect.Width == 0 )
						rect.Width = tex.Size.X - rect.Left;
					if( rect.Height == 0 )
						rect.Height = tex.Size.Y - rect.Top;

					si.Rect = rect;
				}
			}

			UpdateVerts( Parent.GetComponent<UITransform>() );
		}

		private void UpdateVerts( UITransform t = null )
		{
			if( Sprites.Count > 0 )
			{
				m_verts = new VertexArray( PrimitiveType.Quads, (uint)( 4 * Sprites.Count ) );

				if( t != null )
					for( int i = 0; i < Sprites.Count; i++ )
						for( uint v = 0; v < 4; v++ )
							m_verts[ ( (uint)i * 4u ) + v ] = Sprites[ i ].GetVertex( v, t.GlobalPixelBounds, t.Scale );
			}
			else
				m_verts = new VertexArray();
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
		public bool Equals( UISpriteArray other )
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
			return new SpriteArray( this );
		}
	}
}
