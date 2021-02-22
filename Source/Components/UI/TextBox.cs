////////////////////////////////////////////////////////////////////////////////
// TextBox.cs 
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
using System.Text;
using System.Xml;

using SFML.Graphics;
using SFML.System;

using MiCore;

namespace MiGfx
{
	public static partial class FilePaths
	{
		/// <summary>
		///   Default path to text box texture.
		/// </summary>
		public static readonly string TextBoxTexture = FolderPaths.UI + "TextBox.png";
	}

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
			Color      = Color.White;
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
			Color      = t.Color;
			Text       = new TextStyle( t.Text );
			TextOffset = t.TextOffset;
		}
		/// <summary>
		///   Constructor setting image data, text style and text offset.
		/// </summary>
		/// <param name="col">
		///   The texture color modifier.
		/// </param>
		/// <param name="txt">
		///   The text style.
		/// </param>
		/// <param name="off">
		///   The text offset.
		/// </param>
		public TextBoxData( Color col, TextStyle txt = null, Vector2f? off = null )
		{
			Color      = col;
			Text       = txt ?? new TextStyle();
			TextOffset = off ?? new Vector2f();
		}

		/// <summary>
		///   Texture color modifier.
		/// </summary>
		public Color Color { get; set; }
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
		/// <param name="br">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return Logger.LogReturn( "Cannot load TextBoxData from null stream.", false, LogType.Error );

			if( Text == null )
				Text = new TextStyle();
			if( !Text.LoadFromStream( br ) )
				return Logger.LogReturn( "Failed loading TextBoxData's Text from stream.", false, LogType.Error );

			try
			{
				Color      = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
				TextOffset = new Vector2f( br.ReadSingle(), br.ReadSingle() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextBoxData from stream: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="bw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( bw == null )
				return Logger.LogReturn( "Cannot save TextBoxData to null stream.", false, LogType.Error );

			if( Text == null )
				Text = new TextStyle();
			if( !Text.SaveToStream( bw ) )
				return Logger.LogReturn( "Failed saving TextBoxData's Text to stream.", false, LogType.Error );

			try
			{
				bw.Write( Color.R ); bw.Write( Color.G ); bw.Write( Color.B ); bw.Write( Color.A );
				bw.Write( TextOffset.X ); bw.Write( TextOffset.Y );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving TextBoxData to stream: " + e.Message, false, LogType.Error );
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
				return Logger.LogReturn( "Cannot load TextBoxData from a null xml element.", false, LogType.Error );

			XmlElement color = element[ nameof( Color ) ],
					   txt   = element[ nameof( TextStyle ) ],
					   off   = element[ nameof( TextOffset ) ];

			if( txt == null )
				return Logger.LogReturn( "Failed loading TextBoxData: No TextStyle xml element.", false, LogType.Error );
			if( off == null )
				return Logger.LogReturn( "Failed loading TextBoxData: No TextOffset xml element.", false, LogType.Error );

			Color? col = color != null ? Xml.ToColor( color ) : null;

			if( color != null && !col.HasValue )
				return Logger.LogReturn( "Failed loading TextBoxData: Unable to parse Color element.", false, LogType.Error );
			else if( color == null )
				col = Color.White;

			Color = col.Value;
			Text  = new TextStyle();

			Vector2f? o = Xml.ToVec2f( off );

			if( !Text.LoadFromXml( txt ) )
				return Logger.LogReturn( "Failed loading TextBoxData: Loading TextStyle failed.", false, LogType.Error );
			if( !o.HasValue )
				return Logger.LogReturn( "Failed loading TextBoxData: Unable to parse TextOffset xml element.", false, LogType.Error );

			TextOffset = o.Value;

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
			sb.Append( nameof( TextBoxData ) );
			sb.AppendLine( ">" );

			sb.AppendLine( Xml.ToString( Color, nameof( Color ), 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Text, 1 ) );
			sb.AppendLine( Xml.ToString( TextOffset, nameof( TextOffset ), 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( TextBoxData ) );
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
		public bool Equals( TextBoxData other )
		{
			return other != null &&
				   Color.Equals( other.Color ) &&
				   Text.Equals( other.Text ) &&
				   TextOffset.Equals( other.TextOffset );
		}
	}
	/// <summary>
	///   A UI text box.
	/// </summary>
	public class TextBox : MiComponent, IEquatable<TextBox>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextBox()
		:	base()
		{
			SelectedData   = new TextBoxData();
			DeselectedData = new TextBoxData();
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
		}
		/// <summary>
		///   Constructs a text box with selected and/or deselected data.
		/// </summary>
		/// <param name="s">
		///   Selected data.
		/// </param>
		/// <param name="d">
		///   Deselected data, will use selected data if null.
		/// </param>
		public TextBox( TextBoxData s, TextBoxData d = null )
		{
			SelectedData   = s ?? new TextBoxData();
			DeselectedData = d ?? new TextBoxData( SelectedData );
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( TextBox ); }
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
		///   Sets the displayed string keeping both the Label and TextListener components updated.
		/// </summary>
		/// <param name="str">
		///   The string to set.
		/// </param>
		public void SetString( string str )
		{
			if( Parent == null )
				return;

			Parent.GetComponent<UILabel>().String           = new string( str.ToCharArray() );
			Parent.GetComponent<TextListener>().EnteredText = new string( str.ToCharArray() );
		}

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( UITransform ), nameof( Selectable ), nameof( UIClickable ),
			                      nameof( TextListener ), nameof( UISprite ), nameof( UILabel ) };
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Button ), nameof( UISpriteAnimator ), nameof( CheckBox ),
			                      nameof( FillBar ), nameof( UISpriteArray ), };
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			CorrectData();

			if( Parent == null )
				return;

			bool selected = Parent.GetComponent<Selectable>().Selected;

			UISprite    spr  = Parent.GetComponent<UISprite>();
			UILabel     lab  = Parent.GetComponent<UILabel>();
			TextBoxData data = selected ? SelectedData : DeselectedData;

			spr.Image.Color = data.Color;
			lab.Text        = data.Text;
			lab.Offset      = data.TextOffset;
			lab.String      = Parent.GetComponent<TextListener>().EnteredText;

			Texture tex = Assets.Manager.Texture.Get( spr.Image.Path );

			if( tex != null )
			{
				Vector2u size = tex.Size;
				bool newline = Parent.GetComponent<TextListener>().AllowNewline;

				FloatRect idle = newline ? new FloatRect( 0, size.Y / 3u,     size.X, size.Y / 3u )
				                         : new FloatRect( 0, 0,               size.X, size.Y / 6u ),
						  sele = newline ? new FloatRect( 0, size.Y / 3u * 2, size.X, size.Y / 3u )
						                 : new FloatRect( 0, size.Y / 6u,     size.X, size.Y / 6u );

				spr.Image.Rect = selected ? sele : idle;
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

			SelectedData   = new TextBoxData();
			DeselectedData = new TextBoxData();

			if( !SelectedData.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading TextBox's SelectedData from stream.", false, LogType.Error );
			if( !DeselectedData.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading TextBox's DeselectedData from stream.", false, LogType.Error );

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
				return Logger.LogReturn( "Failed saving TextBox's SelectedData to stream.", false, LogType.Error );
			if( !DeselectedData.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving TextBox's DeselectedData to stream.", false, LogType.Error );

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
			
			XmlNodeList data = element.SelectNodes( nameof( TextBoxData ) );

			if( data.Count < 1 || data.Count > 2 )
				return Logger.LogReturn( "Failed loading TextBox: Incorrect amount of TextBoxData elements.", false, LogType.Error );

			SelectedData   = new TextBoxData();
			DeselectedData = new TextBoxData();

			if( !SelectedData.LoadFromXml( (XmlElement)data[ 0 ] ) )
				return Logger.LogReturn( "Failed loading TextBox: Loading SelectedData failed.", false, LogType.Error );
			if( data.Count == 1 )
			{
				if( !DeselectedData.LoadFromXml( (XmlElement)data[ 0 ] ) )
					return Logger.LogReturn( "Failed loading TextBox: Loading SelectedData as DeselectedData failed.", false, LogType.Error );
			}
			else if( !DeselectedData.LoadFromXml( (XmlElement)data[ 1 ] ) )
				return Logger.LogReturn( "Failed loading TextBox: Loading DeselectedData failed.", false, LogType.Error );

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
			sb.Append( TypeName );

			sb.Append( " " );
			sb.Append( nameof( Enabled ) );
			sb.Append( "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "         " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\">" );

			sb.AppendLine( XmlLoadable.ToString( SelectedData, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( DeselectedData, 1 ) );

			sb.Append( "</" );
			sb.Append( TypeName );
			sb.Append( ">" );

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
			return base.Equals( other ) &&
			       SelectedData.Equals( other.SelectedData ) &&
			       DeselectedData.Equals( other.DeselectedData );
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new TextBox( this );
		}

		private void CorrectData()
		{
			if( SelectedData == null )
				SelectedData = new TextBoxData();
			else
			{
				if( SelectedData.Text == null )
					SelectedData.Text = new TextStyle();
			}
			if( DeselectedData == null )
				DeselectedData = new TextBoxData( SelectedData );
			else
			{
				if( DeselectedData.Text == null )
					DeselectedData.Text = new TextStyle( SelectedData.Text );
			}
		}

		/// <summary>
		///   Constructs a textbox with default values.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		/// <param name="window">
		///   The render window.
		/// </param>
		/// <param name="allign">
		///   Text allignment.
		/// </param>
		/// <param name="multi">
		///   If text can be multiline.
		/// </param>
		/// <returns></returns>
		public static MiEntity Create( string id, RenderWindow window = null, Allignment allign = default, bool multi = false )
		{
			MiEntity ent = new MiEntity( id, window );
			
			if( !ent.AddNewComponent<TextBox>() )
			{
				ent.Dispose();
				return Logger.LogReturn<MiEntity>( "Failed creating TextBox entity: Adding TextBox failed.", null, LogType.Error );
			}

			UISprite spr = ent.GetComponent<UISprite>();
			spr.Image = new ImageInfo( FilePaths.TextBoxTexture );

			if( !spr.Image.IsTextureValid )
			{
				ent.Dispose();
				return Logger.LogReturn<MiEntity>( "Failed creating TextBox entity: Loading Texture failed.", null, LogType.Error );
			}

			ent.GetComponent<UILabel>().Allign = allign;
			ent.GetComponent<TextListener>().AllowNewline = multi;

			Vector2u size = spr.Image.TextureSize;
			View view = window.GetView();

			ent.GetComponent<UITransform>().Size = new Vector2f( size.X / view.Size.X, ( multi ? size.Y / 3u : size.Y / 6u ) / view.Size.Y );

			Vector2f off = new Vector2f();

			switch( allign )
			{
				case Allignment.TopLeft:
					off = new Vector2f(  12,  8 );
					break;
				case Allignment.Top:
					off = new Vector2f(   0,  8 );
					break;
				case Allignment.TopRight:
					off = new Vector2f( -12,  8 );
					break;

				case Allignment.Left:
					off = new Vector2f(  12,  0 );
					break;
				case Allignment.Middle:
					off = new Vector2f();
					break;
				case Allignment.Right:
					off = new Vector2f( -12,  0 );
					break;

				case Allignment.BottomLeft:
					off = new Vector2f(  12, -8 );
					break;
				case Allignment.Bottom:
					off = new Vector2f(   0, -8 );
					break;
				case Allignment.BottomRight:
					off = new Vector2f( -12, -8 );
					break;
			}

			TextBox box = ent.GetComponent<TextBox>();

			box.SelectedData   = new TextBoxData( Color.White, new TextStyle( FilePaths.DefaultFont, 36, 0, Color.White ), off );
			box.DeselectedData = new TextBoxData( Color.White, new TextStyle( FilePaths.DefaultFont, 36, 0, Color.White ), off );

			return ent;
		}
	}
}
