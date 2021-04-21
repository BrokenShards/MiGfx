////////////////////////////////////////////////////////////////////////////////
// ImageInfo.cs 
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
	///   Possible directions.
	/// </summary>
	public enum Direction
	{
		/// <summary>
		///   Up direction.
		/// </summary>
		Up,
		/// <summary>
		///   Right direction.
		/// </summary>
		Right,
		/// <summary>
		///   Down direction.
		/// </summary>
		Down,
		/// <summary>
		///   Left direction.
		/// </summary>
		Left
	}

	/// <summary>
	///   Image display information.
	/// </summary>
	public class ImageInfo : BinarySerializable, IXmlLoadable, IEquatable<ImageInfo>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public ImageInfo()
		{
			OverrideTexture = null;
			Path            = string.Empty;
			Rect            = new FloatRect();
			Color           = Color.White;
			Orientation     = Direction.Up;
			FlipHorizontal  = false;
			FlipVertical    = false;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="i">
		///   The image info to copy from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <paramref name="i"/> is null.
		/// </exception>
		public ImageInfo( ImageInfo i )
		{
			if( i == null )
				throw new ArgumentNullException();
			
			OverrideTexture = i.OverrideTexture;
			Path            = new string( i.Path.ToCharArray() );
			Rect            = i.Rect;
			Color           = i.Color;
			Orientation     = i.Orientation;
			FlipHorizontal  = i.FlipHorizontal;
			FlipVertical    = i.FlipVertical;
		}
		/// <summary>
		///   Constructor that assigns texture path.
		/// </summary>
		/// <param name="path">
		///   Texture path.
		/// </param>
		/// <param name="rect">
		///   Texture display rect.
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
		public ImageInfo( string path, FloatRect rect = default, Color? col = null, 
		                  Direction dir = 0, bool hflip = false, bool vflip = false )
		{
			OverrideTexture = null;
			Path            = path ?? string.Empty;
			Rect            = rect;
			Color           = col  ?? new Color( 255, 255, 255, 255 );
			Orientation     = dir;
			FlipHorizontal  = hflip;
			FlipVertical    = vflip;
		}

		/// <summary>
		///   If this is not null, this texture will be used instead of using the texture manager.
		/// </summary>
		public Texture OverrideTexture
		{
			get; set;
		}
		/// <summary>
		///   Texture path.
		/// </summary>
		public string Path { get; set; }
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
		///   Gets the texture.
		/// </summary>
		public Texture Texture
		{
			get { return OverrideTexture ?? Assets.Manager.Texture.Get( Path ); }
		}

		/// <summary>
		///   If a valid texture exists at <see cref="Path"/>.
		/// </summary>
		public bool IsTextureValid
		{
			get { return Texture != null; }
		}
		/// <summary>
		///   Gets the size of the texture if valid.
		/// </summary>
		public Vector2u TextureSize
		{
			get { return Texture?.Size ?? default; }
		}

		/// <summary>
		///   Sets the texture display rect to the full texture.
		/// </summary>
		public void SetFullRect()
		{
			Texture tex = Texture;

			if( tex == null )
				Rect = Logger.LogReturn( "SetFullRect failed because of invalid texture path; Rect has been reset.", new FloatRect(), LogType.Warning );
			else
				Rect = new FloatRect( 0.0f, 0.0f, tex.Size.X, tex.Size.Y );
		}

		/// <summary>
		///   Calculates and returns the vertex at the given index from top-left to bottom-left.
		/// </summary>
		/// <param name="index">
		///   The index of the vertex.
		/// </param>
		/// <param name="bounds">
		///   Display image bounds.
		/// </param>
		/// <returns>
		///   The vertex at the given index to display the image.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   If index is out of range (greater than 3).
		/// </exception>
		public Vertex GetVertex( uint index, FloatRect bounds )
		{
			if( index > 3 )
				throw new ArgumentOutOfRangeException( nameof( index ), "Vertex index is out of range." );

			FloatRect rect = Rect;

			if( rect.Width <= 0.0f || rect.Height <= 0.0f )
			{
				if( IsTextureValid )
				{
					Vector2u ts = Texture.Size;
					rect = new FloatRect( 0, 0, ts.X, ts.Y );
				}
				else
				{
					rect = new FloatRect( 0, 0, bounds.Width, bounds.Height );
				}
			}

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
				Position = new Vector2f( index > 0 && index < 3 ? bounds.Left + bounds.Width : bounds.Left,
				                         index > 1 ? bounds.Top + bounds.Height : bounds.Top ),
				Color = Color,
				TexCoords = new Vector2f( texindex > 0 && texindex < 3 ? rect.Left + rect.Width : rect.Left,
										  texindex > 1 ? rect.Top + rect.Height : rect.Top )
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
			if( br == null )
				return Logger.LogReturn( "Cannot load ImageInfo from null stream.", false, LogType.Error );

			try
			{
				Path           = br.ReadString();
				Rect           = new FloatRect( br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() );
				Color          = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
				Orientation    = (Direction)br.ReadInt32();
				FlipHorizontal = br.ReadBoolean();
				FlipVertical   = br.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading ImageInfo from stream: " + e.Message, false, LogType.Error );
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
			if( bw == null )
				return Logger.LogReturn( "Cannot save ImageInfo to null stream.", false, LogType.Error );

			try
			{
				bw.Write( Path );
				bw.Write( Rect.Left ); bw.Write( Rect.Top ); bw.Write( Rect.Width ); bw.Write( Rect.Height );
				bw.Write( Color.R ); bw.Write( Color.G ); bw.Write( Color.B ); bw.Write( Color.A );
				bw.Write( (int)Orientation );
				bw.Write( FlipHorizontal ); bw.Write( FlipVertical );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving ImageInfo to stream: " + e.Message, false, LogType.Error );
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
				return Logger.LogReturn( "Cannot load ImageInfo from a null XmlElement.", false, LogType.Error );
			
			XmlElement rect   = element[ nameof( Rect ) ],
					   color  = element[ nameof( Color ) ];

			if( rect == null )
				return Logger.LogReturn( "Failed loading ImageInfo: No Rect element.", false, LogType.Error );

			FloatRect? rec = Xml.ToFRect( rect );
			Color?     col = color != null ? Xml.ToColor( color ) : null;

			if( !rec.HasValue )
				return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse Rect element.", false, LogType.Error );

			if( color != null && !col.HasValue )
				return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse Color element.", false, LogType.Error );
			else if( color == null )
				col = Color.White;

			Rect  = rec.Value;
			Color = col.Value;

			if( element.HasAttribute( nameof( Path ) ) )
				Path = element.GetAttribute( nameof( Path ) );

			Orientation    = Direction.Up;
			FlipHorizontal = false;
			FlipVertical   = false;

			if( element.HasAttribute( nameof( Orientation ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Orientation ) ), out Direction o ) )
					return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse Orientation attribute.", false, LogType.Error );

				Orientation = o;
			}
			if( element.HasAttribute( nameof( FlipHorizontal ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipHorizontal ) ), out bool f ) )
					return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse FlipHorizontal attribute.", false, LogType.Error );

				FlipHorizontal = f;
			}
			if( element.HasAttribute( nameof( FlipVertical ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipVertical ) ), out bool f ) )
					return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse FlipVertical attribute.", false, LogType.Error );

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
			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( nameof( ImageInfo ) );

			sb.Append( " " );
			sb.Append( nameof( Path ) );
			sb.Append( "=\"" );
			sb.Append( Path );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( Orientation ) );
			sb.Append( "=\"" );
			sb.Append( Orientation.ToString() );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( FlipHorizontal ) );
			sb.Append( "=\"" );
			sb.Append( FlipHorizontal );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( FlipVertical ) );
			sb.Append( "=\"" );
			sb.Append( FlipVertical );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Rect,  nameof( Rect ), 1 ) );
			sb.AppendLine( Xml.ToString( Color, nameof( Color ), 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( ImageInfo ) );
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
		public bool Equals( ImageInfo other )
		{
			return other != null &&
			       OverrideTexture == other.OverrideTexture &&
			       Path?.Trim()?.ToLower() == other.Path?.Trim()?.ToLower() &&
				   Rect.Equals( other.Rect ) &&
				   Color.Equals( other.Color ) &&
				   Orientation    == other.Orientation &&
				   FlipHorizontal == other.FlipHorizontal &&
				   FlipVertical   == other.FlipVertical;
		}
	}
}
