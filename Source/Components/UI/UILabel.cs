////////////////////////////////////////////////////////////////////////////////
// UILabel.cs 
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
using SFML.System;

namespace MiGfx
{
	/// <summary>
	///   A text label component.
	/// </summary>
	public class UILabel : Label, IEquatable<UILabel>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public UILabel()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="l">
		///   The label to copy from.
		/// </param>
		public UILabel( UILabel l )
		:	base( l )
		{ }
		/// <summary>
		///   Constructs the label and sets its display string.
		/// </summary>
		/// <param name="str">
		///   Display string. If null or whitespace it will be replaced with an empty string.
		/// </param>
		/// <param name="allign">
		///   Text allignment.
		/// </param>
		public UILabel( string str, Allignment allign = default )
		:	base( str, allign )
		{ }
		/// <summary>
		///   Constructs the label with a text style and an optional string.
		/// </summary>
		/// <param name="text">
		///   Text style.
		/// </param>
		/// <param name="str">
		///   Display string. If null or whitespace it will be replaced with an
		///   empty string.
		/// </param>
		/// <param name="allign">
		///   Text allignment.
		/// </param>
		public UILabel( TextStyle text, string str = null, Allignment allign = default )
		:	base( text, str, allign )
		{ }

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UILabel ); }
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
			return new string[] { nameof( Label ) };
		}

		/// <summary>
		///   Updates the component logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Text == null )
				Text = new TextStyle();

			Text.Apply( ref m_text );

			UITransform trn = Parent?.GetComponent<UITransform>();

			if( trn == null || Parent?.Window == null )
				return;

			m_text.Scale = trn.Scale;

			FloatRect bounds = GetTextBounds();
			trn.Size = new Vector2f( Math.Max( trn.Size.X, bounds.Width ), Math.Max( trn.Size.Y, bounds.Height ) );

			FloatRect pb = trn.GlobalPixelBounds;

			Vector2f pos  = new Vector2f( pb.Left, pb.Top ),
					 size = new Vector2f( pb.Width, pb.Height );

			FloatRect lb = m_text.GetLocalBounds();

			switch( Allign )
			{
				case Allignment.Top:
					m_text.Origin = new Vector2f( lb.Width / 2.0f, 0.0f );
					pos += new Vector2f( size.X / 2.0f, 0.0f );
					break;
				case Allignment.TopRight:
					m_text.Origin = new Vector2f( lb.Width, 0.0f );
					pos += new Vector2f( size.X, 0.0f );
					break;

				case Allignment.Left:
					m_text.Origin = new Vector2f( 0.0f, lb.Height / 2.0f );
					pos += new Vector2f( 0.0f, size.Y / 2.0f );
					break;
				case Allignment.Middle:
					m_text.Origin = new Vector2f( lb.Width, lb.Height ) / 2.0f;
					pos += size / 2.0f;
					break;
				case Allignment.Right:
					m_text.Origin = new Vector2f( lb.Width, lb.Height / 2.0f );
					pos += new Vector2f( size.X, size.Y / 2.0f );
					break;

				case Allignment.BottomLeft:
					m_text.Origin = new Vector2f( 0.0f, lb.Height );
					pos += new Vector2f( 0.0f, size.Y );
					break;
				case Allignment.Bottom:
					m_text.Origin = new Vector2f( lb.Width / 2.0f, lb.Height );
					pos += new Vector2f( size.X / 2.0f, size.Y );
					break;
				case Allignment.BottomRight:
					m_text.Origin = new Vector2f( lb.Width, lb.Height );
					pos += size;
					break;
			}

			m_text.Position = pos + new Vector2f( Offset.X * trn.Scale.X, Offset.Y * trn.Scale.Y );
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
		public bool Equals( UILabel other )
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
			return new UILabel( this );
		}
	}
}
