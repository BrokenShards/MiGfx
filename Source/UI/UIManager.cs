////////////////////////////////////////////////////////////////////////////////
// UIManager.cs 
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
using System.Collections;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

using SFInput;

namespace SharpGfx.UI
{
	/// <summary>
	///   Possible UI user interactions.
	/// </summary>
	public enum Interaction
	{
		/// <summary>
		///   Interaction using the mouse.
		/// </summary>
		Mouse,
		/// <summary>
		///   Interaction using the keyboard or joystick.
		/// </summary>
		Control
	}

	/// <summary>
	///   Used for managing UI elements.
	/// </summary>
	public class UIManager : Drawable, IDisposable, IEnumerable<KeyValuePair<string, UIElement>>
	{
		/// <summary>
		///   Constructor that sets the target render window.
		/// </summary>
		/// <param name="window">
		///   The target render window.
		/// </param>
		public UIManager( RenderWindow window )
		{
			m_elements      = new Dictionary<string, UIElement>();
			Selected        = null;
			Window          = window ?? throw new ArgumentNullException();
			LastInteraction = Interaction.Mouse;
		}

		/// <summary>
		///   If the manager contains no elements.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of elements contained by the manager.
		/// </summary>
		public uint Count
		{
			get { return (uint)m_elements.Count; }
		}
		/// <summary>
		///   If the manager contains at least one selectable element.
		/// </summary>
		public bool HasSelectable
		{
			get
			{
				foreach( var v in m_elements )
					if( v.Value.IsSelectable )
						return true;

				return false;
			}
		}

		/// <summary>
		///   Target render window.
		/// </summary>
		public RenderWindow Window { get; private set; }
		/// <summary>
		///   Currently selected element ID.
		/// </summary>
		public string Selected { get; private set; }
		/// <summary>
		///   The most recent input interaction type.
		/// </summary>
		public Interaction LastInteraction { get; private set; }

		/// <summary>
		///   Checks if the manager contains an element with the given ID.
		/// </summary>
		/// <param name="id">
		///   The element ID.
		/// </param>
		/// <returns>
		///   True if the manager contains an element with the given ID and false otherwise.
		/// </returns>
		public bool Contains( string id )
		{
			if( string.IsNullOrWhiteSpace( id ) )
				return false;

			return m_elements.ContainsKey( id.ToLower() );
		}
		/// <summary>
		///   Checks if the manager contains the given element.
		/// </summary>
		/// <param name="e">
		///   The element.
		/// </param>
		/// <returns>
		///   True if the element is valid and is contained by the manager, otherwise false.
		/// </returns>
		public bool Contains( UIElement e )
		{
			if( e == null )
				return false;

			return m_elements.ContainsValue( e );
		}
		/// <summary>
		///   Gets the element with the given type and ID.
		/// </summary>
		/// <typeparam name="T">
		///   The type of the element.
		/// </typeparam>
		/// <param name="id">
		///   The element ID.
		/// </param>
		/// <returns>
		///   The element with the given ID if it exists within the manager otherwise null.
		/// </returns>
		public T Get<T>( string id ) where T : UIElement
		{
			if( !Contains( id ) )
				return null;

			T t = null;

			try
			{
				t = m_elements[ id.ToLower() ] as T;
			}
			catch
			{
				return null;
			}

			return t;
		}
		/// <summary>
		///   Adds a new element to the manager.
		/// </summary>
		/// <param name="e">
		///   The element to add.
		/// </param>
		/// <param name="replace">
		///   If an element already exists with the same ID, should it be replaced?
		/// </param>
		/// <returns>
		///   True if the element was successfully added to the manager and false otherwise.
		/// </returns>
		public bool Add( UIElement e, bool replace = false )
		{
			if( e == null )
				return false;

			string name = e.ID.ToLower();

			if( Contains( name ) )
			{
				if( !replace )
					return false;
				
				m_elements[ name ].Dispose();
				m_elements.Remove( name );
			}

			e.Manager = this;
			m_elements.Add( name, e );

			if( !Contains( Selected ) )
				Selected = new string( name.ToCharArray() );

			return true;
		}
		/// <summary>
		///   Adds multiple elements to the manager.
		/// </summary>
		/// <param name="ele">
		///   The elements to add.
		/// </param>
		/// <returns>
		///   True if all elements were added successfull, otherwise false.
		/// </returns>
		public bool Add( params UIElement[] ele )
		{
			foreach( UIElement e in ele )
				if( !Add( e ) )
					return false;

			return true;
		}
		/// <summary>
		///   Removes the element with the given ID from the manager.
		/// </summary>
		/// <param name="id">
		///   The element ID.
		/// </param>
		/// <returns>
		///   True if an element existed with the given ID and was removed successfully, otherwise false.
		/// </returns>
		public bool Remove( string id )
		{
			if( !Contains( id ) )
				return false;

			string n = id.ToLower();

			m_elements[ n ].Manager = null;
			m_elements[ n ].Dispose();
			m_elements.Remove( n );

			if( Selected == n )
			{
				var e = m_elements.GetEnumerator();

				if( !e.MoveNext() )
					Selected = null;
				else
					Selected = new string( e.Current.Key.ToCharArray() );
			}

			return true;
		}
		/// <summary>
		///   Removes multiple elements by ID from the manager.
		/// </summary>
		/// <param name="ids">
		///   The element IDs to remove.
		/// </param>
		/// <returns>
		///   The amount of elements that were successfully removed.
		/// </returns>
		public int Remove( params string[] ids )
		{
			int count = 0;

			foreach( string s in ids )
				if( Remove( s ) )
					count++;

			return count;
		}
		/// <summary>
		///   Removes all elements from the manager.
		/// </summary>
		public void RemoveAll()
		{
			foreach( var v in m_elements )
			{
				v.Value.Manager = null;
				v.Value.Dispose();
			}

			m_elements.Clear();
			Selected = null;
		}
		/// <summary>
		///   Selects the element with the given ID and deselects all others. If the ID is null or 
		///   empty, all elements will just be deselected.
		/// </summary>
		/// <param name="id">
		///   The element ID.
		/// </param>
		/// <returns>
		///   True if an element exists with the given ID and was selected.
		/// </returns>
		public bool Select( string id )
		{
			if( !string.IsNullOrWhiteSpace( id ) )
			{
				string s = id.ToLower();

				if( Contains( s ) && m_elements[ s ].IsSelectable )
				{
					if( !m_elements[ s ].Enabled )
						return false;

					Selected = s;
				}
				else
					return false;
			}
			else
				Selected = string.Empty;

			foreach( var v in m_elements )
				v.Value.Selected = v.Key == Selected;

			return true;
		}
		/// <summary>
		///   Selects the next element in the manager.
		/// </summary>
		/// <returns>
		///   True on success or false on failure.
		/// </returns>
		public bool SelectNext()
		{
			if( !HasSelectable )
				return false;

			if( Contains( Selected ) )
			{
				bool next = false;

				foreach( var v in m_elements )
				{
					if( !v.Value.IsSelectable || !v.Value.Enabled )
						continue;

					if( !next )
					{
						if( v.Key == Selected )
							next = true;
					}
					else
						return Select( v.Key );
				}
			}

			var e = m_elements.GetEnumerator();
			e.MoveNext();

			while( !e.Current.Value.IsSelectable )
				e.MoveNext();

			return Select( e.Current.Key );
		}
		/// <summary>
		///   Selects the previous element in the manager.
		/// </summary>
		/// <returns>
		///   True on success or false on failure.
		/// </returns>
		public bool SelectPrevious()
		{
			if( !HasSelectable )
				return false;

			if( Contains( Selected ) )
			{
				string last = string.Empty;

				foreach( var v in m_elements )
				{
					if( !v.Value.IsSelectable || !v.Value.Enabled )
						continue;

					if( v.Key == Selected )
					{
						if( !Contains( last ) )
							break;

						return Select( last );
					}
					else
						last = v.Key;
				}
			}

			var e = m_elements.GetEnumerator();
			e.MoveNext();

			while( !e.Current.Value.IsSelectable )
				e.MoveNext();

			return Select( e.Current.Key );
		}

		/// <summary>
		///   Text entered event.
		/// </summary>
		/// <param name="e">
		///   Event arguments.
		/// </param>
		public void TextEntered( TextEventArgs e )
		{
			if( Contains( Selected ) )
				m_elements[ Selected ].TextEntered( e );
		}
		/// <summary>
		///   Draws visible elements in the manager.
		/// </summary>
		/// <param name="target">
		///   Render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		public void Draw( RenderTarget target, RenderStates states )
		{
			foreach( var v in m_elements )
				v.Value.Draw( target, states );
		}
		/// <summary>
		///   Updates enabled elements in the manager.
		/// </summary>
		/// <param name="dt"></param>
		public void Update( float dt )
		{
			if( Input.Manager.Mouse.AnyJustMoved() || Input.Manager.Mouse.AnyJustPressed() || Input.Manager.Mouse.AnyJustReleased() )
				LastInteraction = Interaction.Mouse;
			else
			{
				JoystickManager joy = Input.Manager.Joystick[ Input.Manager.FirstJoystick ];

				if( Input.Manager.Keyboard.AnyJustPressed() || Input.Manager.Keyboard.AnyJustReleased() ||
					( joy != null && ( joy.AnyJustPressed() || joy.AnyJustReleased() || joy.AnyJustMoved() ) ) )
					LastInteraction = Interaction.Control;
			}

			foreach( var v in m_elements )
				v.Value.Update( dt );
		}
		/// <summary>
		///   Disposes of all elements in the manager.
		/// </summary>
		public void Dispose()
		{
			RemoveAll();
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, UIElement>> GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, UIElement>>)m_elements ).GetEnumerator();
		}
		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, UIElement>>)m_elements ).GetEnumerator();
		}

		private Dictionary<string, UIElement> m_elements;
	}
}
