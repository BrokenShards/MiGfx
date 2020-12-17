////////////////////////////////////////////////////////////////////////////////
// CheckBox.cs 
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
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using SFInput;
using System.IO;
using SharpLogger;
using System.Xml;
using SharpSerial;
using System.Text;

namespace SharpGfx
{
	public static partial class FilePaths
	{
		/// <summary>
		///   Default check box texture.
		/// </summary>
		public static readonly string CheckBoxTexture = FolderPaths.UI + "CheckBox.png";
	}
}

namespace SharpGfx.UI
{ 
	/// <summary>
	///   Possible state of check box.
	/// </summary>
	public enum CheckBoxState
	{
		/// <summary>
		///   If not selected and not checked.
		/// </summary>
		Unchecked,
		/// <summary>
		///   If not selected and checked.
		/// </summary>
		Checked,
		/// <summary>
		///   If selected and unchecked.
		/// </summary>
		SelectedUnchecked,
		/// <summary>
		///   If selected and checked.
		/// </summary>
		SelectedChecked
	}

	/// <summary>
	///   Toggling check box.
	/// </summary>
	public class CheckBox : UIElement, IEquatable<CheckBox>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public CheckBox()
		:	base()
		{
			Images = new ImageInfo[ Enum.GetNames( typeof( CheckBoxState ) ).Length ];

			for( int i = 0; i < Images.Length; i++ )
				Images[ i ] = new ImageInfo();

			Checked = false;
			m_image = new Image( EmbeddedID );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="b">
		///   The object to copy.
		/// </param>
		public CheckBox( CheckBox b )
		:	base( b )
		{
			Images = new ImageInfo[ Enum.GetNames( typeof( CheckBoxState ) ).Length ];

			for( int i = 0; i < Images.Length; i++ )
				Images[ i ] = new ImageInfo( b.Images[ i ] );

			Checked = b.Checked;
			m_image = new Image( b.m_image );
		}
		/// <summary>
		///   Constructor setting the initial name.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		public CheckBox( string id )
		:	base( id )
		{
			Images = new ImageInfo[ Enum.GetNames( typeof( CheckBoxState ) ).Length ];

			for( int i = 0; i < Images.Length; i++ )
				Images[ i ] = new ImageInfo();

			Checked = false;
			m_image = new Image( EmbeddedID );
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( CheckBox ); }
		}

		/// <summary>
		///   If check boxes can be selected.
		/// </summary>
		public override bool IsSelectable
		{
			get { return true; }
		}

		/// <summary>
		///   Current state of the object.
		/// </summary>
		public CheckBoxState State
		{
			get
			{
				return Selected ? ( Checked ? CheckBoxState.SelectedChecked : CheckBoxState.SelectedUnchecked ) :
				( Checked ? CheckBoxState.Checked : CheckBoxState.Unchecked );
			}
		}

		/// <summary>
		///   Array of images for each checkbox state.
		/// </summary>
		public ImageInfo[] Images { get; private set; }
		/// <summary>
		///   If the box is checked.
		/// </summary>
		public bool Checked { get; set; }

		/// <summary>
		///   If the mouse is hovering over.
		/// </summary>
		public bool Hovering
		{
			get
			{
				if( !Enabled )
					return false;

				Vector2f pos = Manager.Window.MapPixelToCoords( Input.Manager.Mouse.GetPosition( Manager.Window ) );
				return Transform.GlobalBounds.Contains( pos.X, pos.Y );
			}
		}
		/// <summary>
		///   If the object was clicked.
		/// </summary>
		public bool Clicked
		{
			get
			{
				return ( Hovering && Input.Manager.Mouse.JustPressed( Mouse.Button.Left ) ) ||
					   ( Selected && Input.Manager.Keyboard.JustPressed( Keyboard.Key.Return ) );
			}
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			m_image.Transform = Transform;

			bool hover = Hovering,
				 click = Clicked;

			if( Manager != null )
			{
				if( Manager.LastInteraction == Interaction.Mouse )
				{
					if( hover )
						Manager.Select( ID );
					else if( Selected )
						Manager.Select( null );
				}
			}
			else
				Selected = hover;

			if( click )
				Checked = !Checked;
			
			m_image.DisplayImage = Images[ (int)State ];
			m_image.Update( dt );
		}
		/// <summary>
		///   Draws the element.
		/// </summary>
		/// <param name="target">
		///   The render target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			m_image.Draw( target, states );
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
			if( !base.LoadFromStream( sr ) )
				return false;

			m_image = new Image( EmbeddedID );

			for( CheckBoxState s = 0; (int)s < Enum.GetNames( typeof( CheckBoxState ) ).Length; s++ )
				if( !Images[ (int)s ].LoadFromStream( sr ) )
					return Logger.LogReturn( "Unable to load UICheckbox data image from stream.", false, LogType.Error );

			try
			{
				Checked = sr.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load UICheckbox: " + e.Message + ".", false, LogType.Error );
			}

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
			if( !base.SaveToStream( sw ) )
				return false;

			foreach( ImageInfo i in Images )
				if( !i.SaveToStream( sw ) )
					return Logger.LogReturn( "Unable to save UICheckbox data image to stream.", false, LogType.Error );

			try
			{
				sw.Write( Checked );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save UICheckbox: " + e.Message + ".", false, LogType.Error );
			}

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

			Checked = false;
			XmlNodeList data = element.SelectNodes( nameof( ImageInfo ) );

			if( data.Count != Images.Length )
				return Logger.LogReturn( "Failed loading CheckBox: Incorrect amount of ImageInfo xml elements.", false, LogType.Error );

			for( int i = 0; i < Images.Length; i++ )
			{
				Images[ i ] = new ImageInfo();

				if( !Images[ i ].LoadFromXml( (XmlElement)data[ i ] ) )
					return Logger.LogReturn( "Failed loading CheckBox: Loading ImageInfo failed.", false, LogType.Error );
			}

			string check = element.GetAttribute( nameof( Checked ) );

			if( !string.IsNullOrWhiteSpace( check ) )
			{
				try
				{
					Checked = bool.Parse( check );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed loading CheckBox: " + e.Message, false, LogType.Error );
				}
			}

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
			sb.Append( nameof( CheckBox ) );
			sb.Append( "          " + nameof( ID ) + "=\"" );
			sb.Append( ID );
			sb.AppendLine( "\"" );

			sb.Append( "          " + nameof( Enabled ) + "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "          " + nameof( Visible ) + "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "          " + nameof( Checked ) + "=\"" );
			sb.Append( Checked );
			sb.AppendLine( "\">" );

			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );

			for( int i = 0; i < Images.Length; i++ )
				sb.AppendLine( XmlLoadable.ToString( Images[ i ], 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( CheckBox ) );
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
		public bool Equals( CheckBox other )
		{
			if( !base.Equals( other ) || !m_image.Equals( other.m_image ) || Checked != other.Checked )
				return false;

			for( CheckBoxState s = 0; (int)s < Enum.GetNames( typeof( CheckBoxState ) ).Length; s++ )
				if( !Images[ (int)s ].Equals( other.Images[ (int)s ] ) )
					return false;

			return true;
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

		/// <summary>
		///   Constructs a checkbox with default values.
		/// </summary>
		/// <param name="name">
		///   The object name.
		/// </param>
		/// <param name="check">
		///   If the checkbox is checked.
		/// </param>
		/// <returns>
		///   A new checkbox set up with default values and the given name and checked state.
		/// </returns>
		public static CheckBox Default( string name, bool check = false )
		{
			CheckBox box = new CheckBox( name );

			Texture tex = Assets.Manager.Texture.Get( FilePaths.CheckBoxTexture );

			if( tex == null )
				return null;

			Vector2u size = tex.Size;

			box.Transform.Size = (Vector2f)size / 2.0f;

			box.Images[ 0 ] = new ImageInfo( FilePaths.CheckBoxTexture, new FloatRect( 0,          0,          size.X / 2, size.Y / 2 ) );
			box.Images[ 1 ] = new ImageInfo( FilePaths.CheckBoxTexture, new FloatRect( size.X / 2, 0,          size.X / 2, size.Y / 2 ) );
			box.Images[ 2 ] = new ImageInfo( FilePaths.CheckBoxTexture, new FloatRect( 0,          size.Y / 2, size.X / 2, size.Y / 2 ) );
			box.Images[ 3 ] = new ImageInfo( FilePaths.CheckBoxTexture, new FloatRect( size.X / 2, size.Y / 2, size.X / 2, size.Y / 2 ) );

			box.Checked = check;
			return box;
		}

		private Image m_image;
	}
}
