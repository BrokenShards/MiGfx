////////////////////////////////////////////////////////////////////////////////
// TextBox.cs 
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
using SharpSerial;
using System.IO;
using SharpLogger;
using System.Xml;
using System.Text;

namespace SharpGfx
{
	public static partial class FilePaths
	{
		/// <summary>
		///   Default path to text box texture.
		/// </summary>
		public static readonly string TextBoxTexture = FolderPaths.UI + "TextBox.png";
	}
}
namespace SharpGfx.UI
{ 
	/// <summary>
	///   Textbox state info.
	/// </summary>
	public class TextBoxData : BinarySerializable, IXmlLoadable, IEquatable<TextBoxData>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextBoxData()
		{
			Image      = new ImageInfo();
			Text       = new TextStyle();
			TextOffset = new Vector2f();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   Object to copy.
		/// </param>
		public TextBoxData( TextBoxData t )
		{
			Image      = new ImageInfo( t.Image);
			Text       = new TextStyle( t.Text );
			TextOffset = t.TextOffset;
		}
		/// <summary>
		///   Constructor setting image, text style and text offset.
		/// </summary>
		/// <param name="img">
		///   The image.
		/// </param>
		/// <param name="txt">
		///   The text style.
		/// </param>
		/// <param name="off">
		///   The text offset.
		/// </param>
		public TextBoxData( ImageInfo img, TextStyle txt = null, Vector2f? off = null )
		{
			Image      = img ?? new ImageInfo();
			Text       = txt ?? new TextStyle();
			TextOffset = off ?? new Vector2f();
		}

		/// <summary>
		///   Background image.
		/// </summary>
		public ImageInfo Image { get; set; }
		/// <summary>
		///   Text style.
		/// </summary>
		public TextStyle Text { get; set; }
		/// <summary>
		///   Text offset.
		/// </summary>
		public Vector2f TextOffset { get; set; }

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
				return Logger.LogReturn( "Unable to load ButtonData from null stream.", false, LogType.Error );

			if( Image == null )
				Image = new ImageInfo();
			if( Text == null )
				Text = new TextStyle();

			if( !Image.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load ButtonData Image from stream.", false, LogType.Error );
			if( !Text.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load ButtonData Text from stream.", false, LogType.Error );

			try
			{
				TextOffset = new Vector2f( sr.ReadSingle(), sr.ReadSingle() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load ButtonData from stream: " + e.Message + ".", false, LogType.Error );
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
			if( sw == null )
				return Logger.LogReturn( "Unable to save ButtonData to null stream.", false, LogType.Error );

			if( Image == null )
				Image = new ImageInfo();
			if( Text == null )
				Text = new TextStyle();

			if( !Image.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save ButtonData Image to stream.", false, LogType.Error );
			if( !Text.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save ButtonData Text to stream.", false, LogType.Error );

			try
			{
				sw.Write( TextOffset.X ); sw.Write( TextOffset.Y );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save ButtonData to stream: " + e.Message + ".", false, LogType.Error );
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
		public virtual bool LoadFromXml( XmlElement element )
		{
			if( element == null )
				return Logger.LogReturn( "Cannot load TextBoxData from a null XmlElement.", false, LogType.Error );

			XmlElement img = element[ "image_info" ],
					   txt = element[ "text_style" ],
					   off = element[ "offset" ];

			if( img == null )
				return Logger.LogReturn( "Failed loading TextBoxData: No image_info element.", false, LogType.Error );
			if( txt == null )
				return Logger.LogReturn( "Failed loading TextBoxData: No text_style element.", false, LogType.Error );
			if( off == null )
				return Logger.LogReturn( "Failed loading TextBoxData: No offset element.", false, LogType.Error );

			Image = new ImageInfo();
			Text  = new TextStyle();

			if( !Image.LoadFromXml( img ) )
				return Logger.LogReturn( "Failed loading TextBoxData: Loading ImageInfo failed.", false, LogType.Error );
			if( !Text.LoadFromXml( txt ) )
				return Logger.LogReturn( "Failed loading TextBoxData: Loading TextStyle failed.", false, LogType.Error );

			try
			{
				TextOffset = new Vector2f( float.Parse( off.GetAttribute( "x" ) ),
										   float.Parse( off.GetAttribute( "y" ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextBoxData: " + e.Message, false, LogType.Error );
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

			sb.AppendLine( "<textbox_data>" );
			sb.AppendLine( XmlLoadable.ToString( Image, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Text, 1 ) );

			sb.Append( "\t<offset x=\"" );
			sb.Append( TextOffset.X );
			sb.Append( "\" y=\"" );
			sb.Append( TextOffset.Y );
			sb.AppendLine( "\"/>" );

			sb.Append( "</textbox_data>" );

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
		public bool Equals( TextBoxData other )
		{
			return other != null &&
				   Image.Equals( other.Image ) &&
				   Text.Equals( other.Text ) &&
				   TextOffset == other.TextOffset;
		}
	}
	/// <summary>
	///   A UI text box.
	/// </summary>
	public class TextBox : UIElement, IEquatable<TextBox>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextBox()
		:	base()
		{
			SelectedData   = new TextBoxData();
			DeselectedData = new TextBoxData();
			CenterText     = false;
			Multiline      = false;

			m_label = new Label();
			m_box   = new Image();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="b">
		///   The object to copy.
		/// </param>
		public TextBox( TextBox b )
		:	base( b )
		{
			SelectedData   = new TextBoxData( b.SelectedData );
			DeselectedData = new TextBoxData( b.DeselectedData );
			CenterText     = b.CenterText;
			Multiline      = b.Multiline;

			m_label = new Label( b.m_label );
			m_box   = new Image( b.m_box );
		}
		/// <summary>
		///   Constructor setting 
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		public TextBox( string id )
		:	base( id )
		{
			SelectedData   = new TextBoxData();
			DeselectedData = new TextBoxData();
			CenterText     = false;
			Multiline      = false;

			m_label = new Label();
			m_box   = new Image();
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( TextBox ); }
		}

		/// <summary>
		///   If the element type is selectable.
		/// </summary>
		public override bool IsSelectable
		{
			get { return true; }
		}

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
		///   If the box has just been clicked.
		/// </summary>
		public bool Clicked
		{
			get { return Hovering && Input.Manager.Mouse.JustPressed( Mouse.Button.Left ); }
		}

		/// <summary>
		///   Visual data for when deselected.
		/// </summary>
		public TextBoxData DeselectedData { get; set; }
		/// <summary>
		///   Visual data for when selected.
		/// </summary>
		public TextBoxData SelectedData { get; set; }

		/// <summary>
		///   Content of the textbox.
		/// </summary>
		public string String
		{
			get { return m_label.String; }
			set { m_label.String = value; }
		}
		/// <summary>
		///   If text should be centered.
		/// </summary>
		public bool CenterText { get; set; }
		/// <summary>
		///   If textbox can span multiple lines.
		/// </summary>
		public bool Multiline { get; set; }

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			CorrectData();

			if( Clicked && Manager != null )
				Manager.Select( ID );

			TextBoxData data = Selected ? SelectedData : DeselectedData;

			m_box.Transform    = Transform;
			m_box.DisplayImage = data.Image;
						
			m_label.Transform.Scale = Transform.Scale;
			
			if( CenterText )
				m_label.Transform.Center = new Vector2f( Transform.Center.X, 
					Multiline ? Transform.Center.Y : Transform.Position.Y );
			else
				m_label.Transform.Position = Transform.Position;

			m_label.Transform.Position += data.TextOffset;
			m_label.Text = data.Text;
			
			m_box.Update( dt );
			m_label.Update( dt );
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
			m_box.Draw( target, states );
			m_label.Draw( target, states );
		}

		/// <summary>
		///   Called when disposing of the object.
		/// </summary>
		protected override void OnDispose()
		{
			m_box.Dispose();
			m_label.Dispose();
		}

		/// <summary>
		///   Text entered event.
		/// </summary>
		/// <param name="e">
		///   Event arguments.
		/// </param>
		protected override void OnTextEntered( TextEventArgs e )
		{
			if( String == null )
				String = string.Empty;

			int len = String.Length;

			// Backspace
			if( e.Unicode == "\u0008" || e.Unicode == "\u0232" )
			{
				if( len > 0 )
					String = String.Substring( 0, len - 1 );
			}
			// Carriage Return, Newline, or both (all treated as newline)
			else if( e.Unicode == "\r\n" || e.Unicode == "\u000A" || e.Unicode == "\u000D" )
			{
				if( len > 0 && Multiline )
					String += '\n';
			}
			else if( e.Unicode.Trim().Length > 0 || ( len > 0 && e.Unicode == " " ) )
			{
				String += e.Unicode;
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
			if( !base.LoadFromStream( sr ) )
				return false;

			if( !SelectedData.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading TextBox SelectedData from stream.", false, LogType.Error );
			if( !DeselectedData.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading TextBox DeselectedData from stream.", false, LogType.Error );

			m_box   = new Image();
			m_label = new Label();

			try
			{
				String = sr.ReadString();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextBox: " + e.Message, false, LogType.Error );
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

			if( !SelectedData.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving TextBox SelectedData to stream.", false, LogType.Error );
			if( !DeselectedData.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving TextBox DeselectedData to stream.", false, LogType.Error );

			try
			{
				sw.Write( String );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving TextBox to stream: " + e.Message, false, LogType.Error );
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

			XmlElement  str  = element[ "string" ];
			XmlNodeList data = element.SelectNodes( "textbox_data" );

			if( str == null )
				return Logger.LogReturn( "Failed loading TextBox: No string element.", false, LogType.Error );
			if( data.Count != 2 )
				return Logger.LogReturn( "Failed loading TextBox: Incorrect amount of textbox_data elements.", false, LogType.Error );

			SelectedData   = new TextBoxData();
			DeselectedData = new TextBoxData();

			if( !SelectedData.LoadFromXml( (XmlElement)data[ 0 ] ) )
				return Logger.LogReturn( "Failed loading TextBox: Loading SelectedData failed.", false, LogType.Error );
			if( !DeselectedData.LoadFromXml( (XmlElement)data[ 1 ] ) )
				return Logger.LogReturn( "Failed loading TextBox: Loading DeselectedData failed.", false, LogType.Error );

			m_box   = new Image();
			m_label = new Label();

			try
			{
				String = str.Value;
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextBox: " + e.Message, false, LogType.Error );
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

			sb.Append( "<textbox id=\"" );
			sb.Append( ID );
			sb.AppendLine( "\"" );

			sb.Append( "         enabled=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "         visible=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\">" );

			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( SelectedData, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( DeselectedData, 1 ) );

			sb.Append( "\t<string>" );
			sb.Append( String );
			sb.AppendLine( "</string>" );

			sb.Append( "</button>" );

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
		public bool Equals( TextBox other )
		{
			return base.Equals( other ) && SelectedData.Equals( other.SelectedData ) &&
			       DeselectedData.Equals( other.DeselectedData ) &&
				   String.Equals( other.String );
		}

		private void CorrectData()
		{
			if( SelectedData == null )
				SelectedData = new TextBoxData();
			else
			{
				if( SelectedData.Image == null )
					SelectedData.Image = new ImageInfo();
				if( SelectedData.Text == null )
					SelectedData.Text = new TextStyle();
			}
			if( DeselectedData == null )
				DeselectedData = new TextBoxData( SelectedData );
			else
			{
				if( DeselectedData.Image == null )
					DeselectedData.Image = new ImageInfo( SelectedData.Image );
				if( DeselectedData.Text == null )
					DeselectedData.Text = new TextStyle( SelectedData.Text );
			}
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
		///   Constructs a textbox with default values.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		/// <param name="center">
		///   If text should be centered.
		/// </param>
		/// <param name="multi">
		///   If text can be multiline.
		/// </param>
		/// <returns></returns>
		public static TextBox Default( string id, bool center = false, bool multi = false )
		{
			TextBox box = new TextBox( id )
			{
				CenterText = center,
				Multiline  = multi
			};			

			Texture tex = Assets.Manager.Texture.Get( FilePaths.TextBoxTexture );

			if( tex == null )
				return null;

			Vector2u size = tex.Size;

			box.Transform.LocalSize = new Vector2f( size.X, multi ? size.Y / 3u : size.Y / 6u );
			
			ImageInfo si = new ImageInfo( FilePaths.TextBoxTexture, new FloatRect( 0, 0,               size.X, size.Y / 6u ) );
			ImageInfo ss = new ImageInfo( FilePaths.TextBoxTexture, new FloatRect( 0, size.Y / 6u,     size.X, size.Y / 6u ) );
			ImageInfo mi = new ImageInfo( FilePaths.TextBoxTexture, new FloatRect( 0, size.Y / 3u,     size.X, size.Y / 3u ) );
			ImageInfo ms = new ImageInfo( FilePaths.TextBoxTexture, new FloatRect( 0, size.Y / 3u * 2, size.X, size.Y / 3u ) );

			TextStyle t = new TextStyle( FilePaths.DefaultFont, 36, 0, Color.White );

			Vector2f off = center ? new Vector2f( 0, -20.0f ) : new Vector2f( 18, -5 );

			box.SelectedData   = new TextBoxData( multi ? ms : ss, new TextStyle( t ), off );
			box.DeselectedData = new TextBoxData( multi ? mi : si, new TextStyle( t ), off );

			return box;
		}

		private Label m_label;
		private Image m_box;
	}
}
