////////////////////////////////////////////////////////////////////////////////
// SpriteInfo.cs 
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
	/// <summary>
	///   Sprite display information.
	/// </summary>
	public class SpriteInfo : BinarySerializable, IXmlLoadable, IEquatable<SpriteInfo>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public SpriteInfo()
		{
			Offset         = new Vector2f();
			Size           = new Vector2f( 1.0f, 1.0f );
			Rect           = new FloatRect();
			Color          = Color.White;
			Orientation    = Direction.Up;
			FlipHorizontal = false;
			FlipVertical   = false;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="i">
		///   The sprite info to copy from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <paramref name="i"/> is null.
		/// </exception>
		public SpriteInfo( SpriteInfo i )
		{
			if( i is null )
				throw new ArgumentNullException( nameof( i ) );
			
			Offset         = i.Offset;
			Size           = i.Size;
			Rect           = i.Rect;
			Color          = i.Color;
			Orientation    = i.Orientation;
			FlipHorizontal = i.FlipHorizontal;
			FlipVertical   = i.FlipVertical;
		}
		/// <summary>
		///   Constructor that assigns texture rect, orientation and color.
		/// </summary>
		/// <param name="rect">
		///   Texture rect to be displayed.
		/// </param>
		/// <param name="off">
		///   Sprite offset.
		/// </param>
		/// <param name="size">
		///   Sprite size.
		/// </param>
		/// <param name="dir">
		///   Orientation direction.
		/// </param>
		/// <param name="col">
		///   Texture color modifier.
		/// </param>
		/// <param name="hflip">
		///   Horizontal flip.
		/// </param>
		/// <param name="vflip">
		///   Vertical flip..
		/// </param>
		public SpriteInfo( FloatRect rect, Color? col = null, Vector2f? off = null, Vector2f? size = null, Direction dir = 0, bool hflip = false, bool vflip = false )
		{
			Offset         = off  ?? new Vector2f();
			Size           = size ?? new Vector2f( rect.Width, rect.Height );
			Rect           = rect;
			Color          = col  ?? Color.White;
			Orientation    = dir;
			FlipHorizontal = hflip;
			FlipVertical   = vflip;
		}

		/// <summary>
		///  The sprite offset.
		/// </summary>
		public Vector2f Offset
		{
			get; set;
		}
		/// <summary>
		///  The sprite size.
		/// </summary>
		public Vector2f Size
		{
			get { return m_size; }
			set
			{
				m_size = value;

				if( m_size.X <= 0.0f )
					m_size.X = 1.0f;
				if( m_size.Y <= 0.0f )
					m_size.Y = 1.0f;
			}
		}
		/// <summary>
		///   Texture display rect.
		/// </summary>
		public FloatRect Rect { get; set; }
		/// <summary>
		///   Texture color modifier.
		/// </summary>
		public Color Color { get; set; }
		/// <summary>
		///   Texture orientation modifier.
		/// </summary>
		public Direction Orientation
		{
			get; set;
		}
		/// <summary>
		///   If image should be flipped horizontally on the x-axis.
		/// </summary>
		public bool FlipHorizontal { get; set; }
		/// <summary>
		///   If image should be flipped vertically on the y-axis.
		/// </summary>
		public bool FlipVertical { get; set; }

		/// <summary>
		///   Calculates and returns the vertex at the given index from top-left to bottom-left.
		/// </summary>
		/// <param name="index">
		///   The index of the vertex.
		/// </param>
		/// <param name="bounds">
		///   Display image bounds.
		/// </param>
		/// <param name="scl">
		///   Sprite scale.
		/// </param>
		/// <returns>
		///   The vertex at the given index to display the image.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   If index is out of range (greater than 3).
		/// </exception>
		public Vertex GetVertex( uint index, FloatRect bounds, Vector2f? scl = null )
		{
			if( index > 3 )
				throw new ArgumentOutOfRangeException( nameof( index ), "Vertex index is out of range." );

			Vector2f scale = scl ?? new Vector2f( 1.0f, 1.0f );

			bounds.Width  = Size.X * scale.X;
			bounds.Height = Size.Y * scale.Y;
			bounds.Left  += Offset.X * scale.X;
			bounds.Top   += Offset.Y * scale.Y;

			uint texindex = index;

			if( FlipHorizontal )
			{
				if( index == 0 )
					index = 1;
				else if( index == 1 )
					index = 0;
				else if( index == 2 )
					index = 3;
				else if( index == 3 )
					index = 2;
			}
			if( FlipVertical )
			{
				if( index == 0 )
					index = 3;
				else if( index == 1 )
					index = 2;
				else if( index == 2 )
					index = 1;
				else if( index == 3 )
					index = 0;
			}

			switch( Orientation )
			{
				case Direction.Left:
					texindex++;
					break;
				case Direction.Down:
					texindex += 2;
					break;
				case Direction.Right:
					texindex += 3;
					break;
			}

			if( texindex >= 4 )
				texindex %= 4;

			return new Vertex
			{
				Position  = new Vector2f( index > 0 && index < 3 ? bounds.Left + bounds.Width : bounds.Left,
				                          index > 1 ? bounds.Top + bounds.Height : bounds.Top ),
				Color     = Color,
				TexCoords = new Vector2f( texindex > 0 && texindex < 3 ? Rect.Left + Rect.Width : Rect.Left,
										  texindex > 1 ? Rect.Top + Rect.Height : Rect.Top )
			};
		}

		/// <summary>
		///   Loads the object from a stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( br is null )
				return Logger.LogReturn( "Failed loading SpriteInfo from null stream.", false, LogType.Error );

			try
			{
				Offset         = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Size           = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Rect           = new FloatRect( br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() );
				Color          = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
				Orientation    = (Direction)br.ReadInt32();
				FlipHorizontal = br.ReadBoolean();
				FlipVertical   = br.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed loading SpriteInfo from stream: { e.Message }", false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Writes the object to a stream.
		/// </summary>
		/// <param name="bw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the object was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( bw is null )
				return Logger.LogReturn( "Failed saving SpriteInfo to null stream.", false, LogType.Error );

			try
			{
				bw.Write( Offset.X ); bw.Write( Offset.Y );
				bw.Write( Size.X ); bw.Write( Size.Y );
				bw.Write( Rect.Left ); bw.Write( Rect.Top ); bw.Write( Rect.Width ); bw.Write( Rect.Height );
				bw.Write( Color.R ); bw.Write( Color.G ); bw.Write( Color.B ); bw.Write( Color.A );
				bw.Write( (int)Orientation );
				bw.Write( FlipHorizontal ); bw.Write( FlipVertical );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed saving SpriteInfo to stream: { e.Message }", false, LogType.Error );
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
		public bool LoadFromXml( XmlElement element )
		{
			if( element is null )
				return Logger.LogReturn( "Cannot load SpriteInfo from a null XmlElement.", false, LogType.Error );

			Offset = default;

			XmlElement offset = element[ nameof( Offset ) ],
			           size   = element[ nameof( Size ) ],
					   rect   = element[ nameof( Rect ) ],
					   color  = element[ nameof( Color ) ];

			if( size is null )
				return Logger.LogReturn( "Failed loading SpriteInfo: No Size element.", false, LogType.Error );
			if( rect is null )
				return Logger.LogReturn( "Failed loading SpriteInfo: No Rect element.", false, LogType.Error );

			Vector2f?  off = offset is not null ? Xml.ToVec2f( offset ) : null;
			Vector2f?  siz = Xml.ToVec2f( size );
			FloatRect? rec = Xml.ToFRect( rect );
			Color?     col = color is not null ? Xml.ToColor( color ) : null;

			if( offset is not null && !off.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Offset element.", false, LogType.Error );
			else if( offset is null )
				off = new Vector2f();

			if( !siz.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Size element.", false, LogType.Error );
			if( !rec.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Rect element.", false, LogType.Error );

			if( color is not null && !col.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Color element.", false, LogType.Error );
			else if( color is null )
				col = Color.White;

			Offset = off.Value;
			Size   = siz.Value;
			Rect   = rec.Value;
			Color  = col.Value;

			Orientation    = Direction.Up;
			FlipHorizontal = false;
			FlipVertical   = false;

			if( element.HasAttribute( nameof( Orientation ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Orientation ) ), out Direction o ) )
					return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Orientation attribute.", false, LogType.Error );

				Orientation = o;
			}
			if( element.HasAttribute( nameof( FlipHorizontal ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipHorizontal ) ), out bool f ) )
					return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse FlipHorizontal attribute.", false, LogType.Error );

				FlipHorizontal = f;
			}
			if( element.HasAttribute( nameof( FlipVertical ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipVertical ) ), out bool f ) )
					return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse FlipVertical attribute.", false, LogType.Error );

				FlipVertical = f;
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
			StringBuilder sb = new();

			sb.Append( '<' ).Append( nameof( SpriteInfo ) ).Append( ' ' )
				.Append( nameof( Orientation ) ).Append( "=\"" ).Append( Orientation ).AppendLine( "\"" )
				.Append( "            " )
				.Append( nameof( FlipHorizontal ) ).Append( "=\"" ).Append( FlipHorizontal ).AppendLine( "\"" )
				.Append( "            " )
				.Append( nameof( FlipVertical ) ).Append( "=\"" ).Append( FlipVertical ).AppendLine( "\">" )
				
				.AppendLine( Xml.ToString( Offset, nameof( Offset ), 1 ) )
				.AppendLine( Xml.ToString( Size,   nameof( Size ),   1 ) )
				.AppendLine( Xml.ToString( Rect,   nameof( Rect ),   1 ) )
				.AppendLine( Xml.ToString( Color,  nameof( Color ),  1 ) )
				
				.Append( "</" ).Append( nameof( SpriteInfo ) ).Append( '>' );

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
		public bool Equals( SpriteInfo other )
		{
			return other != null &&
			       Offset.Equals( other.Offset ) &&
				   Size.Equals( other.Size ) &&
				   Rect.Equals( other.Rect ) &&
				   Color.Equals( other.Color ) &&
				   Orientation    == other.Orientation &&
				   FlipHorizontal == other.FlipHorizontal &&
				   FlipVertical   == other.FlipVertical;
		}
		/// <summary>
		///   If this object has the same values of the other object.
		/// </summary>
		/// <param name="obj">
		///   The other object to check against.
		/// </param>
		/// <returns>
		///   True if both objects are concidered equal and false if they are not.
		/// </returns>
		public override bool Equals( object obj )
		{
			return Equals( obj as SpriteInfo );
		}

		/// <summary>
		///   Serves as the default hash function.
		/// </summary>
		/// <returns>
		///   A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return HashCode.Combine( Offset, Size, Rect, Color, Orientation, FlipHorizontal, FlipVertical );
		}

		Vector2f m_size;
	}
}
