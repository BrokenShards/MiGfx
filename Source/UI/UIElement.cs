////////////////////////////////////////////////////////////////////////////////
// UIElement.cs 
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
using System.Xml;
using SFML.Graphics;
using SFML.Window;

using MiCore;

namespace MiGfx.UI
{
	/// <summary>
	///   Base class for GUI elements.
	/// </summary>
	[Serializable]
	public abstract class UIElement : BinarySerializable, IXmlLoadable, Drawable, IIdentifiable<string>, IEquatable<UIElement>, IDisposable, ITransformable
	{
		/// <summary>
		///   ID given to elements embedded within elements.
		/// </summary>
		public const string EmbeddedID = "SharpGfxUIEmbeddedID";

		/// <summary>
		///   Constructor.
		/// </summary>
		public UIElement()
		{
			ID         = Identifiable.NewStringID( "UI" );
			Transform  = new Transform();
			Enabled    = true;
			Visible    = true;
			m_selected = false;
			Manager    = null;
			Disposed   = false;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="e">
		///   The element to copy from.
		/// </param>
		public UIElement( UIElement e )
		{
			if( e == null )
				throw new ArgumentNullException();

			ID         = e.ID + "_Copy";
			Transform  = new Transform( e.Transform );
			Enabled    = e.Enabled;
			Visible    = e.Visible;
			m_selected = false;
			Manager    = e.Manager;
			Disposed   = e.Disposed;
		}
		/// <summary>
		///   Constructs the element with the given ID.
		/// </summary>
		/// <param name="id">
		///   The desired element ID.
		/// </param>
		public UIElement( string id )
		{
			ID         = id;
			Transform  = new Transform();
			Enabled    = true;
			Visible    = true;
			m_selected = false;
			Manager    = null;
			Disposed   = false;
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public abstract string TypeName { get; }

		/// <summary>
		///   If the element has been disposed.
		/// </summary>
		public bool Disposed { get; private set; }

		/// <summary>
		///   If the element can be selected.
		/// </summary>
		public virtual bool IsSelectable
		{
			get { return false; }
		}
		/// <summary>
		///   If the element is a container.
		/// </summary>
		public virtual bool IsContainer
		{
			get { return false; }
		}

		/// <summary>
		///   The name of the element. Used to identify the object.
		/// </summary>
		/// <remarks>
		///   If set to an invalid name, it will be corrected.
		///   <see cref="Identifiable.IsValid(string)"/> and
		///   <see cref="Identifiable.AsValid(string)"/>.
		/// </remarks>
		public string ID
		{
			get { return m_id; }
			private set { m_id = Identifiable.AsValid( value ); }
		}
		/// <summary>
		///   The elements' transform.
		/// </summary>
		public Transform Transform { get; set; }
		/// <summary>
		///   If the element is visible and should be drawn.
		/// </summary>
		public bool Visible
		{
			get { return m_visible; }
			set
			{
				if( value == m_visible )
					return;

				m_visible = value;
				OnVisibilityChanged();
			}
		}
		/// <summary>
		///   If the element is enabled and should be updated.
		/// </summary>
		public bool Enabled
		{
			get { return m_enabled; }
			set
			{
				if( value == m_enabled )
					return;

				m_enabled = value;
				OnEnableChanged();
			}
		}
		/// <summary>
		///   If the element is currently selected.
		/// </summary>
		public bool Selected
		{
			get { return IsSelectable && m_selected; }
			set
			{
				if( IsSelectable )
				{
					if( value == m_selected )
						return;

					m_selected = value;
					OnSelectionChanged();
				}
			}
		}

		/// <summary>
		///   The object managing the element.
		/// </summary>
		public UIManager Manager { get; set; }

		/// <summary>
		///   Updates the element logic.
		/// </summary>
		/// <remarks>
		///   Calls <see cref="OnUpdate(float)"/> if the element is enabled.
		/// </remarks>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		public void Update( float dt )
		{
			if( Enabled )
				OnUpdate( dt );
		}
		/// <summary>
		///   Draws the element.
		/// </summary>
		/// <remarks>
		///   Calls <see cref="OnDraw(RenderTarget, RenderStates)"/> if the 
		///   element is visible.
		/// </remarks>
		/// <param name="target">
		///   The render target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		public void Draw( RenderTarget target, RenderStates states )
		{
			if( Visible )
				OnDraw( target, states );
		}

		/// <summary>
		///   Disposes of the element.
		/// </summary>
		/// <remarks>
		///   Makes sure to call <see cref="OnDispose"/> only once and updates
		///   the <see cref="Disposed"/> field.
		/// </remarks>
		public void Dispose()
		{
			if( !Disposed )
			{
				OnDispose();
				Disposed = true;
			}
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
			if( sr == null )
				return Logger.LogReturn( "Unable to load UIElement from null stream.", false, LogType.Error );

			try
			{
				ID      = sr.ReadString();
				Enabled = sr.ReadBoolean();
				Visible = sr.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load UIElement from stream: " + e.Message + ".", false, LogType.Error );
			}

			if( !Transform.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load UIElements' Transform from stream.", false, LogType.Error );

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
			if( sw == null )
				return Logger.LogReturn( "Unable to save UIElement to null stream.", false, LogType.Error );

			try
			{
				sw.Write( ID );
				sw.Write( Enabled );
				sw.Write( Visible );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save UIElement to stream: " + e.Message + ".", false, LogType.Error );
			}

			if( !Transform.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save UIElements' Transform to stream.", false, LogType.Error );

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
		public virtual bool LoadFromXml( XmlElement element )
		{
			if( element == null )
				return Logger.LogReturn( "Cannot load UIElement from a null XmlElement.", false, LogType.Error );

			Transform = new Transform();
			Enabled   = true;
			Visible   = true;

			XmlElement trn = element[ nameof( MiGfx.Transform ) ];

			if( trn == null )
				return Logger.LogReturn( "Failed loading UIElement: No Transform xml element.", false, LogType.Error );
			if(	!Transform.LoadFromXml( trn ) )
				return Logger.LogReturn( "Failed loading UIElement: Loading Transform failed.", false, LogType.Error );

			string id = element.GetAttribute( nameof( ID ) );
			ID = string.IsNullOrWhiteSpace( id ) ? Identifiable.NewStringID( "UI" ) : id;

			try
			{
				string en  = element.GetAttribute( nameof( Enabled ) ),
					   vis = element.GetAttribute( nameof( Visible ) );

				if( !string.IsNullOrWhiteSpace( en ) )
					Enabled = bool.Parse( en );
				if( !string.IsNullOrWhiteSpace( vis ) )
					Visible = bool.Parse( vis );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading UIElement: " + e.Message, false, LogType.Error );
			}

			return true;
		}

		/// <summary>
		///   Called when the game window recieves a TextEntered event for 
		///   enabled and selected elements.
		/// </summary>
		/// <remarks>
		///   Calls <see cref="OnTextEntered(TextEventArgs)"/> if the element is
		///   enabled and selected.
		/// </remarks>
		/// <param name="e">
		///   Event arguments.
		/// </param>
		public virtual void TextEntered( TextEventArgs e )
		{
			if( Enabled && Selected )
				OnTextEntered( e );
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
		public bool Equals( UIElement other )
		{
			return other        != null &&
				   IsSelectable == other.IsSelectable  &&
				   IsContainer  == other.IsContainer   &&
				   Enabled      == other.Enabled       && 
				   Visible      == other.Visible       && 
				   Transform.Equals( other.Transform );
		}

		/// <summary>
		///   Override to update the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected abstract void OnUpdate( float dt );
		/// <summary>
		///   Override to draw the element.
		/// </summary>
		/// <param name="target">
		///   The render target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected abstract void OnDraw( RenderTarget target, RenderStates states );
		/// <summary>
		///   Override to respond to TextEntered events.
		/// </summary>
		/// <param name="e">
		///   Event arguments.
		/// </param>
		protected virtual void OnTextEntered( TextEventArgs e )
		{ }
		/// <summary>
		///   Called when selected or deselected.
		/// </summary>
		protected virtual void OnSelectionChanged()
		{ }
		/// <summary>
		///   Called when enabled or disabled.
		/// </summary>
		protected virtual void OnEnableChanged()
		{ }
		/// <summary>
		///   Called when visibility is changed.
		/// </summary>
		protected virtual void OnVisibilityChanged()
		{ }
		/// <summary>
		///   Override to dispose of any resources if needed.
		/// </summary>
		protected virtual void OnDispose()
		{ }

		private string m_id;
		private bool   m_selected,
		               m_enabled,
		               m_visible;
	}
}
