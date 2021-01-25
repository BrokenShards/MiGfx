////////////////////////////////////////////////////////////////////////////////
// Frame.cs 
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
	///   A sprite-based animation frame.
	/// </summary>
	public class Frame : BinarySerializable, IXmlLoadable, IEquatable<Frame>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Frame()
		{
			Rect   = new FloatRect();
			Length = Time.Zero;
			Color  = Color.White;

			Orientation    = Direction.Up;
			FlipHorizontal = false;
			FlipVertical   = false;
		}
		/// <summary>
		///   Constructor assigning the texture rect and frame length.
		/// </summary>
		/// <param name="rect">
		///   The texture rect to display on the frame.
		/// </param>
		/// <param name="len">
		///   The length of time the frame lasts; defaults to 1.0 if null.
		/// </param>
		/// <param name="col">
		///   The texture modifier color.
		/// </param>
		/// <param name="dir">
		///   Orientation direction.
		/// </param>
		/// <param name="hflip">
		///   If texture rect should be flipped horizontally.
		/// </param>
		/// <param name="vflip">
		///   If texture rect should be flipped vertically.
		/// </param>
		public Frame( FloatRect rect, Time? len = null, Color? col = null, Direction dir = 0, bool hflip = false, bool vflip = false )
		{
			Rect   = rect;
			Length = len ?? Time.FromSeconds( 1.0f );
			Color  = col ?? Color.White;
			
			Orientation    = dir;
			FlipHorizontal = hflip;
			FlipVertical   = vflip;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="f">
		///   The frame to copy from.
		/// </param>
		public Frame( Frame f )
		{
			if( f == null )
				throw new ArgumentNullException();

			Rect   = f.Rect;
			Length = f.Length;
			Color  = f.Color;
			
			Orientation    = f.Orientation;
			FlipHorizontal = f.FlipHorizontal;
			FlipVertical   = f.FlipVertical;
		}

		/// <summary>
		///   The texture rect to display on the frame.
		/// </summary>
		public FloatRect Rect { get; set; }
		/// <summary>
		///   The length of time the frame is displayed.
		/// </summary>
		public Time Length { get; set; }
		/// <summary>
		///   The texture color modifier.
		/// </summary>
		public Color Color { get; set; }
		/// <summary>
		///   Texture orientation modifier.
		/// </summary>
		public Direction Orientation { get; set; }
		/// <summary>
		///   If texture rect should be flipped horizontally.
		/// </summary>
		public bool FlipHorizontal { get; set; }
		/// <summary>
		///   If texture rect should be flipped vertically.
		/// </summary>
		public bool FlipVertical { get; set; }

		/// <summary>
		///   Applies the frames' properties to an image info.
		/// </summary>
		/// <param name="i">
		///   The image info to apply values to.
		/// </param>
		public void Apply( ref ImageInfo i )
		{
			if( i == null )
				return;

			i.Rect           = Rect;
			i.Color          = Color;
			i.Orientation    = Orientation;
			i.FlipHorizontal = FlipHorizontal;
			i.FlipVertical   = FlipVertical;
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return Logger.LogReturn( "Cannot load Frame from null stream.", false, LogType.Error );

			try
			{
				Rect   = new FloatRect( br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() );
				Length = Time.FromMicroseconds( br.ReadInt64() );
				Color  = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
				Orientation    = (Direction)br.ReadInt32();
				FlipHorizontal = br.ReadBoolean();
				FlipVertical   = br.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Frame from stream: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Writes the object to the stream.
		/// </summary>
		/// <param name="bw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the object was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( bw == null )
				return Logger.LogReturn( "Cannot save Frame to null stream.", false, LogType.Error );

			try
			{
				bw.Write( Rect.Left ); bw.Write( Rect.Top ); bw.Write( Rect.Width ); bw.Write( Rect.Height );
				bw.Write( Length.AsMicroseconds() );
				bw.Write( Color.R ); bw.Write( Color.G ); bw.Write( Color.B ); bw.Write( Color.A );
				bw.Write( (int)Orientation );
				bw.Write( FlipHorizontal );
				bw.Write( FlipVertical );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Frame to stream: " + e.Message, false, LogType.Error );
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
			if( element == null )
				return Logger.LogReturn( "Cannot load Frame from a null XmlElement.", false, LogType.Error );
			if( !element.HasAttribute( nameof( Length ) ) )
				return Logger.LogReturn( "Failed loading Frame: Missing Length attribute.", false, LogType.Error );

			Orientation    = Direction.Up;
			FlipHorizontal = false;
			FlipVertical   = false;

			XmlElement rect = element[ nameof( Rect ) ],
			           col  = element[ nameof( Color ) ];

			if( rect == null )
				return Logger.LogReturn( "Failed loading Frame: No Rect element.", false, LogType.Error );

			FloatRect? r = Xml.ToFRect( rect );

			if( !r.HasValue )
				return Logger.LogReturn( "Failed loading Frame: Unable to parse Rect element.", false, LogType.Error );

			if( element.HasAttribute( nameof( Orientation ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Orientation ) ), out Direction o ) )
					return Logger.LogReturn( "Failed loading Frame: Unable to parse Orientation attribute.", false, LogType.Error );

				Orientation = o;
			}
			if( element.HasAttribute( nameof( FlipHorizontal ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipHorizontal ) ), out bool h ) )
					return Logger.LogReturn( "Failed loading Frame: Unable to parse FlipHorizontal attribute.", false, LogType.Error );

				FlipHorizontal = h;
			}
			if( element.HasAttribute( nameof( FlipVertical ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipVertical ) ), out bool v ) )
					return Logger.LogReturn( "Failed loading Frame: Unable to parse FlipVertical attribute.", false, LogType.Error );

				FlipVertical = v;
			}

			Rect = r.Value;

			if( col != null )
			{
				Color? c = Xml.ToColor( col );

				if( !c.HasValue )
					return Logger.LogReturn( "Failed loading Frame: Unable to parse Color element.", false, LogType.Error );

				Color = c.Value;
			}

			try
			{
				Length = Time.FromSeconds( float.Parse( element.GetAttribute( nameof( Length ) ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Frame: " + e.Message, false, LogType.Error );
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
			sb.Append( nameof( Frame ) );
			sb.Append( " " );
			sb.Append( nameof( Length ) );
			sb.Append( "=\"" );
			sb.Append( Length.AsSeconds() );
			sb.AppendLine( "\"" );

			sb.Append( "       " );
			sb.Append( nameof( Orientation ) );
			sb.Append( "=\"" );
			sb.Append( Orientation.ToString() );
			sb.AppendLine( "\"" );

			sb.Append( "       " );
			sb.Append( nameof( FlipHorizontal ) );
			sb.Append( "=\"" );
			sb.Append( FlipHorizontal );
			sb.AppendLine( "\"" );

			sb.Append( "       " );
			sb.Append( nameof( FlipVertical ) );
			sb.Append( "=\"" );
			sb.Append( FlipVertical );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Rect,  nameof( Rect ),  1 ) );
			sb.AppendLine( Xml.ToString( Color, nameof( Color ), 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( Frame ) );
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
		public bool Equals( Frame other )
		{
			return other  != null &&
			       Rect.Equals( other.Rect ) &&
			       Length.Equals( other.Length ) &&
				   Color.Equals( other.Color ) &&
				   Orientation    == other.Orientation &&
				   FlipHorizontal == other.FlipHorizontal &&
				   FlipVertical   == other.FlipVertical;
		}
	}
}
