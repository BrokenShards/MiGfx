////////////////////////////////////////////////////////////////////////////////
// Slider.cs 
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
using System.Security.Policy;
using SFML.Graphics;
using SFML.System;

namespace SharpGfx.UI
{
	/// <summary>
	///   Possible directional axis.
	/// </summary>
	public enum DirectionAxis
	{
		/// <summary>
		///   Horizizontal axis.
		/// </summary>
		Horizontal,
		/// <summary>
		///   Vertical axis.
		/// </summary>
		Vertical
	}

	/// <summary>
	///   UI slider.
	/// </summary>
	public class Slider : UIElement, IEquatable<Slider>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Slider()
		:	base()
		{
			m_slider  = new Image();
			m_pointer = new Image();
			Direction = DirectionAxis.Horizontal;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="e">
		///   The object to copy.
		/// </param>
		public Slider( Slider e )
		:	base( e )
		{
			m_slider  = new Image( e.m_slider );
			m_pointer = new Image( e.m_pointer );
			Direction = e.Direction;
		}
		/// <summary>
		///   Constructor setting the object ID.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		public Slider( string id )
		:	base( id )
		{
			m_slider  = new Image();
			m_pointer = new Image();
			Direction = DirectionAxis.Horizontal;
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Slider ); }
		}

		/// <summary>
		///   If the object type is selectable.
		/// </summary>
		public override bool IsSelectable
		{
			get { return true; }
		}

		/// <summary>
		///   Slider direction.
		/// </summary>
		public DirectionAxis Direction 
		{
			get
			{
				if( Transform.LocalSize.X >= Transform.LocalSize.Y )
					return DirectionAxis.Horizontal;
				else
					return DirectionAxis.Vertical;
			}
			set
			{
				DirectionAxis dir = Direction;

				if( value == dir || (int)value >= Enum.GetNames( typeof( DirectionAxis ) ).Length )
					return;

				float x = Transform.LocalSize.X,
				      y = Transform.LocalSize.Y;

				Transform.LocalSize = new Vector2f( y, x );
			}
		}

		/// <summary>
		///   Image for the slider background.
		/// </summary>
		public ImageInfo SliderImage
		{
			get { return m_slider.DisplayImage; }
			set { m_slider.DisplayImage = value; }
		}
		/// <summary>
		///   Image for the slider background.
		/// </summary>
		public ImageInfo PointerImage
		{
			get { return m_pointer.DisplayImage; }
			set { m_pointer.DisplayImage = value; }
		}

		/// <summary>
		///   Override to update the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			float minsize = Math.Min( Transform.LocalSize.X, Transform.LocalSize.Y );

			m_slider.Transform  = new Transform( Transform );
			m_pointer.Transform = new Transform( Transform.Position, new Vector2f( minsize, minsize ), Transform.Scale );


			m_slider.Update( dt );
			m_pointer.Update( dt );
		}
		/// <summary>
		///   Override to draw the element.
		/// </summary>
		/// <param name="target">
		///   The render target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			m_slider.Draw( target, states );
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
		public bool Equals( Slider other )
		{
			return base.Equals( other )                &&
			       m_slider.Equals( other.m_slider )   &&
				   m_pointer.Equals( other.m_pointer ) &&
				   Direction == other.Direction;
		}

		/// <summary>
		///   Gets the type name.
		/// </summary>
		/// <returns>
		///   The type name.
		/// </returns>
		public static string GetTypeName()
		{
			string name = string.Empty;

			using( AnimatedImage a = new AnimatedImage() )
				name = a.TypeName;

			return name;
		}

		private Image m_slider, 
		              m_pointer;
	}
}
