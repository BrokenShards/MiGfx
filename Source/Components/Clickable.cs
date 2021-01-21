////////////////////////////////////////////////////////////////////////////////
// Clickable.cs 
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

using System.IO;
using System.Xml;
using SFML.System;
using SFML.Window;

using MiCore;
using MiInput;

namespace MiGfx
{
	/// <summary>
	///   Possible clickable state.
	/// </summary>
	public enum ClickableState
	{
		/// <summary>
		///   If clickable is not being interacted with.
		/// </summary>
		Idle,
		/// <summary>
		///   If the mouse is hovering over the clickable.
		/// </summary>
		Hover,
		/// <summary>
		///   If the clickable has been clicked.
		/// </summary>
		Click
	}

	/// <summary>
	///   A component that makes entities clickable.
	/// </summary>
	public class Clickable : MiComponent
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Clickable()
		:	base()
		{
			ClickState = ClickableState.Idle;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="c">
		///   The object to copy.
		/// </param>
		public Clickable( Clickable c )
		:	base( c )
		{
			ClickState = c.ClickState;
		}

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Clickable ); }
		}

		/// <summary>
		///   Current clickable state.
		/// </summary>
		public ClickableState ClickState
		{
			get; private set;
		}

		/// <summary>
		///   If the mouse is hovering over the entity.
		/// </summary>
		public bool Hovering
		{
			get
			{
				if( !Enabled || Parent?.Window == null )
					return false;

				Vector2f pos = Parent.Window.MapPixelToCoords( Input.Manager.Mouse.GetPosition( Parent.Window ) );
				return Parent.Components.Get<Transform>().GlobalBounds.Contains( pos.X, pos.Y );
			}
		}
		/// <summary>
		///   If the entity is being clicked.
		/// </summary>
		public bool Clicked
		{
			get
			{
				return( Hovering && Input.Manager.Mouse.JustPressed( Mouse.Button.Left ) ) ||
					  ( Parent.Components.Get<Selectable>().Selected && Input.Manager.Keyboard.JustPressed( Keyboard.Key.Return ) );
			}
		}

		/// <summary>
		///   Updates the component logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			ClickState = Clicked ? ClickableState.Click : ( Hovering ? ClickableState.Hover : ClickableState.Idle );
		}

		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader sr )
		{
			ClickState = ClickableState.Idle;
			return true;
		}
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter sw )
		{
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
			ClickState = ClickableState.Idle;
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
			return "<" + TypeName + "/>";
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
		public bool Equals( Clickable other )
		{
			return base.Equals( other ) &&
				   ClickState == other.ClickState;
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new Clickable( this );
		}
	}
}
