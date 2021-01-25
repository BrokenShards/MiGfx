////////////////////////////////////////////////////////////////////////////////
// SpriteArray.cs 
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using SFML.Graphics;
using SFML.System;

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   Image display information.
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
			if( i == null )
				throw new ArgumentNullException();
			
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
		/// <param name="t">
		///   Transform of the display image.
		/// </param>
		/// <returns>
		///   The vertex at the given index to display the image.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   If index is out of range (greater than 3).
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   If transform is null.
		/// </exception>
		public Vertex GetVertex( uint index, Transform t )
		{
			if( t == null )
				throw new ArgumentNullException();
			if( index > 3 )
				throw new ArgumentOutOfRangeException( nameof( index ), "Vertex index is out of range." );

			FloatRect bounds = new FloatRect( t.Position + new Vector2f( Offset.X * t.Scale.X, Offset.Y * t.Scale.Y ), 
			                                  new Vector2f( Size.X * t.Scale.X, Size.Y * t.Scale.Y ) ),
			          rect   = Rect;

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
				return Logger.LogReturn( "Failed loading SpriteInfo from stream: " + e.Message, false, LogType.Error );
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
				return Logger.LogReturn( "Failed saving SpriteInfo to stream: " + e.Message, false, LogType.Error );
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
				return Logger.LogReturn( "Cannot load SpriteInfo from a null XmlElement.", false, LogType.Error );

			XmlElement offset = element[ nameof( Offset ) ],
			           size   = element[ nameof( Size ) ],
					   rect   = element[ nameof( Rect ) ],
					   color  = element[ nameof( Color ) ];

			if( size == null )
				return Logger.LogReturn( "Failed loading SpriteInfo: No Size element.", false, LogType.Error );
			if( rect == null )
				return Logger.LogReturn( "Failed loading SpriteInfo: No Rect element.", false, LogType.Error );

			Vector2f?  off = offset != null ? Xml.ToVec2f( offset ) : null;
			Vector2f?  siz = Xml.ToVec2f( size );
			FloatRect? rec = Xml.ToFRect( rect );
			Color?     col = color != null ? Xml.ToColor( color ) : null;

			if( offset != null && !off.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Offset element.", false, LogType.Error );
			else if( offset == null )
				off = new Vector2f();

			if( !siz.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Size element.", false, LogType.Error );
			if( !rec.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Rect element.", false, LogType.Error );

			if( color != null && !col.HasValue )
				return Logger.LogReturn( "Failed loading SpriteInfo: Unable to parse Color element.", false, LogType.Error );
			else if( color == null )
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
			StringBuilder sb = new StringBuilder();

			sb.Append( "<" );
			sb.Append( nameof( SpriteInfo ) );

			sb.Append( " " );
			sb.Append( nameof( Orientation ) );
			sb.Append( "=\"" );
			sb.Append( Orientation );
			sb.AppendLine( "\"" );

			sb.Append( "            " );
			sb.Append( nameof( FlipHorizontal ) );
			sb.Append( "=\"" );
			sb.Append( FlipHorizontal );
			sb.AppendLine( "\"" );

			sb.Append( "            " );
			sb.Append( nameof( FlipVertical ) );
			sb.Append( "=\"" );
			sb.Append( FlipVertical );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Offset, nameof( Offset ), 1 ) );
			sb.AppendLine( Xml.ToString( Size,   nameof( Size ),   1 ) );
			sb.AppendLine( Xml.ToString( Rect,   nameof( Rect ),   1 ) );
			sb.AppendLine( Xml.ToString( Color,  nameof( Color ),  1 ) );

			sb.Append( "</" );
			sb.Append( nameof( SpriteInfo ) );
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

		Vector2f m_size;
	}

	/// <summary>
	///   A component that manages and draws multiple sprites that use the same texture.
	/// </summary>
	public class SpriteArray : MiComponent, IEquatable<SpriteArray>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public SpriteArray()
		:	base()
		{
			Sprites     = new List<SpriteInfo>();
			TexturePath = null;

			UpdateVerts();

			RequiredComponents     = new string[] { nameof( Transform ) };
			IncompatibleComponents = new string[] { nameof( Sprite ) };
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s"></param>
		public SpriteArray( SpriteArray s )
		:	base( s )
		{
			if( s == null )
				throw new ArgumentNullException();

			Sprites     = new List<SpriteInfo>();
			TexturePath = s.TexturePath == null ? null : new string( s.TexturePath.ToCharArray() );

			foreach( SpriteInfo sp in s.Sprites )
				if( sp != null )
					Sprites.Add( new SpriteInfo( sp ) );

			m_verts = new VertexArray( PrimitiveType.Quads, 4 );

			for( uint i = 0; i < 4; i++ )
				m_verts[ i ] = s.m_verts[ i ];

			RequiredComponents     = new string[] { nameof( Transform ) };
			IncompatibleComponents = new string[] { nameof( Sprite ) };
		}
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="path">
		///   The texture path.
		/// </param>
		public SpriteArray( string path )
		:	base()
		{
			Sprites     = new List<SpriteInfo>();
			TexturePath = path;

			UpdateVerts();

			RequiredComponents     = new string[] { nameof( Transform ) };
			IncompatibleComponents = new string[] { nameof( Sprite ) };
		}

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( SpriteArray ); }
		}

		/// <summary>
		///   The texture path.
		/// </summary>
		public string TexturePath
		{
			get; set;
		}

		/// <summary>
		///   List of sprites.
		/// </summary>
		public List<SpriteInfo> Sprites
		{
			get; private set; 
		}

		/// <summary>
		///   Updates the sprite geometry. Call this before drawing.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <see cref="Image"/> is null.
		/// </exception>
		protected override void OnUpdate( float dt )
		{
			if( Stack == null )
				return;

			Texture tex = Assets.Manager.Texture.Get( TexturePath );

			if( tex != null )
			{
				foreach( SpriteInfo si in Sprites )
				{
					FloatRect rect = si.Rect;

					if( rect.Width == 0 )
						rect.Width = tex.Size.X - rect.Left;
					if( rect.Height == 0 )
						rect.Height = tex.Size.Y - rect.Top;

					si.Rect = rect;
				}
			}

			UpdateVerts( Stack.Get<Transform>() );
		}
		/// <summary>
		///   Draws the sprite to the render target.
		/// </summary>
		/// <param name="target">
		///   The target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			if( Sprites.Count == 0 )
				return;

			states.Texture = Assets.Manager.Texture.Get( TexturePath );
			m_verts.Draw( target, states );
		}

		private void UpdateVerts( Transform t = null )
		{
			if( Sprites.Count > 0 )
			{
				m_verts = new VertexArray( PrimitiveType.Quads, (uint)( 4 * Sprites.Count ) );

				if( t != null )
					for( int i = 0; i < Sprites.Count; i++ )
						for( uint v = 0; v < 4; v++ )
							m_verts[ ( (uint)i * 4u ) + v ] = Sprites[ i ].GetVertex( v, t );
			}
			else
				m_verts = new VertexArray();
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader
		/// </param>
		/// <returns>
		///   True if the sprite was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( !base.LoadFromStream( br ) )
				return false;

			Sprites = new List<SpriteInfo>();

			try
			{
				TexturePath = br.ReadString();

				int count = br.ReadInt32();

				for( int i = 0; i < count; i++ )
				{
					SpriteInfo info = new SpriteInfo();

					if( !info.LoadFromStream( br ) )
						return Logger.LogReturn( "Failed loading SpriteArray's SpriteInfo from stream.", false, LogType.Error );

					Sprites.Add( info );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading SpriteArray from stream: " + e.Message, false, LogType.Error );
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
		///   True if the sprite was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( !base.SaveToStream( bw ) )
				return false;

			try
			{
				bw.Write( TexturePath ?? string.Empty );
				bw.Write( Sprites.Count );

				for( int i = 0; i < Sprites.Count; i++ )
					if( !Sprites[ i ].SaveToStream( bw ) )
						return Logger.LogReturn( "Failed saving SpriteArray's SpriteInfo to stream.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving SpriteArray to stream: " + e.Message, false, LogType.Error );
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
			if( !element.HasAttribute( nameof( TexturePath ) ) )
				return Logger.LogReturn( "Failed loading SpriteArray: No TexturePath xml attribute.", false, LogType.Error );

			TexturePath = element.GetAttribute( nameof( TexturePath ) );
			Sprites = new List<SpriteInfo>();

			XmlNodeList list = element.SelectNodes( nameof( SpriteInfo ) );

			foreach( XmlNode n in list )
			{
				SpriteInfo info = new SpriteInfo();

				if( !info.LoadFromXml( (XmlElement)n ) )
					return Logger.LogReturn( "Failed loading SpriteArray: Failed loading SpriteInfo.", false, LogType.Error );

				Sprites.Add( info );
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
			sb.Append( TypeName );

			sb.Append( " " );
			sb.Append( nameof( Enabled ) );
			sb.Append( "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "        " );
			sb.Append( nameof( TexturePath ) );
			sb.Append( "=\"" );
			sb.Append( TexturePath );
			sb.AppendLine( "\">" );

			foreach( SpriteInfo si in Sprites )
				sb.AppendLine( XmlLoadable.ToString( si, 1 ) );

			sb.Append( "</" );
			sb.Append( TypeName );
			sb.AppendLine( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   Disposes of managed resources.
		/// </summary>
		protected override void OnDispose()
		{
			( (IDisposable)m_verts ).Dispose();
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
		public bool Equals( SpriteArray other )
		{
			if( !base.Equals( other ) || Sprites.Count != other.Sprites.Count ||
				!( TexturePath?.Equals( other.TexturePath ) ?? TexturePath == other.TexturePath ) )
				return false;

			for( int i = 0; i < Sprites.Count; i++ )
				if( !Sprites[ i ].Equals( other.Sprites[ i ] ) )
					return false;

			return true;
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new SpriteArray( this );
		}

		private VertexArray m_verts;
	}
}
