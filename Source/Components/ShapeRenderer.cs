////////////////////////////////////////////////////////////////////////////////
// ShapeRenderer.cs 
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
using SFML.Window;

using MiCore;
using MiGfx;

namespace MiGfx
{
	/// <summary>
	///   A component that renders a box with an optional outline and texture.
	/// </summary>
	public class ShapeRenderer : MiComponent, IEquatable<ShapeRenderer>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public ShapeRenderer()
		:	base()
		{
			m_circ = new CircleShape();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="br">
		///   The object to copy.
		/// </param>
		public ShapeRenderer( ShapeRenderer br )
		:	base( br )
		{
			m_circ = new CircleShape();
		}

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( ShapeRenderer ); }
		}

		/// <summary>
		///   Texture path (null or empty to not use a texture).
		/// </summary>
		public string Texture 
		{
			get { return m_tex; }
			set
			{
				m_tex = value;
				m_circ.Texture = Assets.Manager.Get<Texture>( m_tex );
			}
		}
		/// <summary>
		///   The rectangle of the texture to display.
		/// </summary>
		public IntRect TextureRect
		{
			get { return m_circ.TextureRect; }
			set { m_circ.TextureRect = value; }
		}
		/// <summary>
		///   Fill color.
		/// </summary>
		public Color FillColor
		{
			get { return m_circ.FillColor; }
			set { m_circ.FillColor = value; }
		}
		/// <summary>
		///   Outline color.
		/// </summary>
		public Color OutlineColor
		{
			get { return m_circ.OutlineColor; }
			set { m_circ.OutlineColor = value; }
		}
		/// <summary>
		///   Outline thickness.
		/// </summary>
		public float OutlineThickness
		{
			get { return m_circ.OutlineThickness; }
			set { m_circ.OutlineThickness = value; }
		}

		/// <summary>
		///   The radius of the shape.
		/// </summary>
		public float Radius
		{
			get { return m_circ.Radius; }
			set { m_circ.Radius = value; }
		}
		/// <summary>
		///   The amount of points/vertices that make up the shape.
		/// </summary>
		public uint PointCount
		{
			get { return m_circ.GetPointCount(); }
			set { m_circ.SetPointCount( Math.Max( value, 3 ) ); }
		}

		/// <summary>
		///   Gets the local position of the point at the given index.
		/// </summary>
		/// <param name="index">
		///   The point index.
		/// </param>
		/// <returns>
		///   The local vertex position of the point at the given index.
		/// </returns>
		public Vector2f GetPoint( uint index )
		{
			if( index >= PointCount )
				throw new ArgumentOutOfRangeException();

			return m_circ.GetPoint( index );
		}

		/// <summary>
		///   Refreshes visual elements.
		/// </summary>
		public override void Refresh()
		{
			if( Parent == null )
				return;

			Transform t = Parent.GetComponent<Transform>();
			t.Size = new Vector2f( Radius * 2, Radius * 2 );

			FloatRect bounds = t.GlobalBounds;

			m_circ.Scale    = t.Scale;
			m_circ.Position = new Vector2f( bounds.Left, bounds.Top );
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

			try
			{
				Texture          = sr.ReadString();
				TextureRect      = new IntRect( sr.ReadInt32(), sr.ReadInt32(), sr.ReadInt32(), sr.ReadInt32() );
				FillColor        = new Color( sr.ReadByte(), sr.ReadByte(), sr.ReadByte(), sr.ReadByte() );
				OutlineColor     = new Color( sr.ReadByte(), sr.ReadByte(), sr.ReadByte(), sr.ReadByte() );
				OutlineThickness = sr.ReadSingle();
				Radius           = sr.ReadSingle();
				PointCount       = sr.ReadUInt32();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading ShapeRenderer from stream: " + e.Message, false, LogType.Error );
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

			try
			{
				sw.Write( Texture );
				
				sw.Write( TextureRect.Left );  sw.Write( TextureRect.Top );
				sw.Write( TextureRect.Width ); sw.Write( TextureRect.Height );

				sw.Write( FillColor.R ); sw.Write( FillColor.G );
				sw.Write( FillColor.B ); sw.Write( FillColor.A );

				sw.Write( OutlineColor.R ); sw.Write( OutlineColor.G );
				sw.Write( OutlineColor.B ); sw.Write( OutlineColor.A );

				sw.Write( OutlineThickness );
				sw.Write( Radius ); 
				sw.Write( PointCount );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving ShapeRenderer to stream: " + e.Message, false, LogType.Error );
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

			XmlElement rect = element[ nameof( TextureRect ) ],
			           fill = element[ nameof( FillColor ) ],
					   outl = element[ nameof( OutlineColor ) ];

			if( rect != null )
			{
				IntRect? ir = Xml.ToIRect( rect );

				if( !ir.HasValue )
					return Logger.LogReturn( "Failed loading ShapeRenderer: Parsing TextureRect failed.", false, LogType.Error );

				TextureRect = ir.Value;
			}
			if( fill != null )
			{
				Color? c = Xml.ToColor( fill );

				if( !c.HasValue )
					return Logger.LogReturn( "Failed loading ShapeRenderer: Parsing FillColor failed.", false, LogType.Error );

				FillColor = c.Value;
			}
			if( outl != null )
			{
				Color? c = Xml.ToColor( outl );

				if( !c.HasValue )
					return Logger.LogReturn( "Failed loading ShapeRenderer: Parsing OutlineColor failed.", false, LogType.Error );

				OutlineColor = c.Value;
			}

			if( element.HasAttribute( nameof( Texture ) ) )
				Texture = element.GetAttribute( nameof( Texture ) );

			if( element.HasAttribute( nameof( OutlineThickness ) ) )
			{
				if( !float.TryParse( element.GetAttribute( nameof( OutlineThickness ) ), out float t ) )
					return Logger.LogReturn( "Failed loading ShapeRenderer: Parsing OutlineThickness failed.", false, LogType.Error );

				OutlineThickness = t;
			}

			if( !element.HasAttribute( nameof( Radius ) ) )
				return Logger.LogReturn( "Failed loading ShapeRenderer: No Radius attribute.", false, LogType.Error );
			
			if( !float.TryParse( element.GetAttribute( nameof( Radius ) ), out float r ) )
				return Logger.LogReturn( "Failed loading ShapeRenderer: Parsing Radius failed.", false, LogType.Error );

			Radius = r;

			if( element.HasAttribute( nameof( PointCount ) ) )
			{
				if( !uint.TryParse( element.GetAttribute( nameof( PointCount ) ), out uint p ) )
					return Logger.LogReturn( "Failed loading ShapeRenderer: Parsing PointCount failed.", false, LogType.Error );

				PointCount = p;
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

			sb.Append( "               " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "               " );
			sb.Append( nameof( Radius ) );
			sb.Append( "=\"" );
			sb.Append( Radius );
			sb.AppendLine( "\"" );

			sb.Append( "               " );
			sb.Append( nameof( PointCount ) );
			sb.Append( "=\"" );
			sb.Append( PointCount );
			sb.AppendLine( "\"" );

			sb.Append( "               " );
			sb.Append( nameof( Texture ) );
			sb.Append( "=\"" );
			sb.Append( Texture );
			sb.AppendLine( "\"" );

			sb.Append( "               " );
			sb.Append( nameof( OutlineThickness ) );
			sb.Append( "=\"" );
			sb.Append( OutlineThickness );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( TextureRect,  nameof( TextureRect ),  1 ) );
			sb.AppendLine( Xml.ToString( FillColor,    nameof( FillColor ),    1 ) );
			sb.AppendLine( Xml.ToString( OutlineColor, nameof( OutlineColor ), 1 ) );

			sb.Append( "</" );
			sb.Append( TypeName );
			sb.AppendLine( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   Get the type names of the components required for this component type.
		/// </summary>
		/// <returns>
		///   The type names of the components required for this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Get the type names of the components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of the components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Sprite ), nameof( SpriteArray ), nameof( BoxRenderer ) };
		}

		/// <summary>
		///   Draws the component to the render target.
		/// </summary>
		/// <param name="target">
		///   The render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			m_circ.Draw( target, states );
		}

		/// <summary>
		///   Disposes of the component.
		/// </summary>
		protected override void OnDispose()
		{
			m_circ?.Dispose();
		}

		/// <summary>
		///   Returns a clone of this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new ShapeRenderer( this );
		}

		/// <summary>
		///   Checks if another object is concidered equal to this.
		/// </summary>
		/// <param name="other">
		///   The object to check.
		/// </param>
		/// <returns>
		///   True if other is considered equal to this object, otherwise false.
		/// </returns>
		public bool Equals( ShapeRenderer other )
		{
			return base.Equals( other ) &&
				   Texture.Equals( other.Texture ) &&
				   TextureRect.Equals( other.TextureRect ) &&
				   FillColor.Equals( other.FillColor ) &&
				   OutlineColor.Equals( other.OutlineColor ) &&
				   OutlineThickness == other.OutlineThickness &&
				   Radius           == other.Radius &&
				   PointCount       == other.PointCount;
		}

		string m_tex;
		readonly CircleShape m_circ;
	}
}
