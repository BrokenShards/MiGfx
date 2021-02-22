////////////////////////////////////////////////////////////////////////////////
// UIClickable.cs 
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

using SFML.System;
using SFML.Window;

using MiInput;
using SFML.Graphics;

namespace MiGfx
{
	/// <summary>
	///   A component that makes entities clickable.
	/// </summary>
	public class UIClickable : Clickable
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public UIClickable()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="c">
		///   The object to copy.
		/// </param>
		public UIClickable( UIClickable c )
		:	base( c )
		{ }

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UIClickable ); }
		}

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( UITransform ), nameof( Selectable ) };
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Clickable ) };
		}

		/// <summary>
		///   Updates the component logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			UITransform t = Parent?.GetComponent<UITransform>();
			Selectable  s = Parent?.GetComponent<Selectable>();

			if( !Enabled || Parent?.Window == null || t == null || s == null )
			{
				ClickState = ClickableState.Idle;
				return;
			}

			if( Input.Manager.LastDevice == InputDevice.Mouse )
			{
				RenderWindow window = Parent.Window;				

				Vector2f mpos = window.MapPixelToCoords( Input.Manager.Mouse.GetPosition( window ) );
				Hovering = t.GlobalPixelBounds.Contains( mpos.X, mpos.Y );

				if( s.Selector?.Parent != null )
					s.Selected = Hovering;

				Clicked = Hovering && Input.Manager.Mouse.JustPressed( Mouse.Button.Left );
			}
			else
			{
				Hovering = s.Selected;

				Action submit = Input.Manager.Actions[ 0 ]?.Get( "Submit" );

				bool sub = submit != null ? submit.JustPressed :
						( Input.Manager.Keyboard.JustPressed( Keyboard.Key.Enter ) ||
						  Input.Manager.Joystick[ 0 ].JustPressed( "A" ) );

				Clicked = Hovering && sub;
			}

			ClickState = Clicked ? ClickableState.Click : ( s.Selected ? ClickableState.Hover : ClickableState.Idle );
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
		public bool Equals( UIClickable other )
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
			return new UIClickable( this );
		}
	}
}
