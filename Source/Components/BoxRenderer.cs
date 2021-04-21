////////////////////////////////////////////////////////////////////////////////
// BoxRenderer.cs 
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
	public class BoxRenderer : MiComponent, IEquatable<BoxRenderer>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public BoxRenderer()
		:	base()
		{
			m_rect = new RectangleShape();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="br">
		///   The object to copy.
		/// </param>
		public BoxRenderer( BoxRenderer br )
		:	base( br )
		{
			m_rect = new RectangleShape();
		}

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( BoxRenderer ); }
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
				m_rect.Texture = Assets.Manager.Get<Texture>( m_tex );
			}
		}
		/// <summary>
		///   The rectangle of the texture to display.
		/// </summary>
		public IntRect TextureRect
		{
			get { return m_rect.TextureRect; }
			set { m_rect.TextureRect = value; }
		}
		/// <summary>
		///   Fill color.
		/// </summary>
		public Color FillColor
		{
			get { return m_rect.FillColor; }
			set { m_rect.FillColor = value; }
		}
		/// <summary>
		///   Outline color.
		/// </summary>
		public Color OutlineColor
		{
			get { return m_rect.OutlineColor; }
			set { m_rect.OutlineColor = value; }
		}
		/// <summary>
		///   Outline thickness.
		/// </summary>
		public float OutlineThickness
		{
			get { return m_rect.OutlineThickness; }
			set { m_rect.OutlineThickness = value; }
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
			if( index >= m_rect.GetPointCount() )
				throw new ArgumentOutOfRangeException();

			return m_rect.GetPoint( index );
		}

		/// <summary>
		///   Refreshes visual elements.
		/// </summary>
		public override void Refresh()
		{
			if( Parent == null )
				return;

			Transform t = Parent.GetComponent<Transform>();

			FloatRect bounds = t.GlobalBounds;

			m_rect.Position = new SFML.System.Vector2f( bounds.Left, bounds.Top );
			m_rect.Size     = t.Size;
			m_rect.Scale    = t.Scale;
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
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading BoxRenderer from stream: " + e.Message, false, LogType.Error );
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
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving BoxRenderer to stream: " + e.Message, false, LogType.Error );
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
				IntRect? r = Xml.ToIRect( rect );

				if( !r.HasValue )
					return Logger.LogReturn( "Failed loading BoxRenderer: Parsing TextureRect failed.", false, LogType.Error );

				TextureRect = r.Value;
			}
			if( fill != null )
			{
				Color? c = Xml.ToColor( fill );

				if( !c.HasValue )
					return Logger.LogReturn( "Failed loading BoxRenderer: Parsing FillColor failed.", false, LogType.Error );

				FillColor = c.Value;
			}
			if( outl != null )
			{
				Color? c = Xml.ToColor( outl );

				if( !c.HasValue )
					return Logger.LogReturn( "Failed loading BoxRenderer: Parsing OutlineColor failed.", false, LogType.Error );

				OutlineColor = c.Value;
			}

			if( element.HasAttribute( nameof( Texture ) ) )
				Texture = element.GetAttribute( nameof( Texture ) );

			if( element.HasAttribute( nameof( OutlineThickness ) ) )
			{
				if( !float.TryParse( element.GetAttribute( nameof( OutlineThickness ) ), out float t ) )
					return Logger.LogReturn( "Failed loading BoxRenderer: Parsing OutlineThickness failed.", false, LogType.Error );

				OutlineThickness = t;
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

			sb.Append( "             " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "             " );
			sb.Append( nameof( Texture ) );
			sb.Append( "=\"" );
			sb.Append( Texture );
			sb.AppendLine( "\"" );

			sb.Append( "             " );
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
			return new string[] { nameof( Sprite ), nameof( SpriteArray ), nameof( ShapeRenderer ) };
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
			m_rect.Draw( target, states );
		}

		/// <summary>
		///   Disposes of the component.
		/// </summary>
		protected override void OnDispose()
		{
			m_rect?.Dispose();
		}

		/// <summary>
		///   Returns a clone of this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new BoxRenderer( this );
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
		public bool Equals( BoxRenderer other )
		{
			return base.Equals( other ) &&
				   Texture.Equals( other.Texture ) &&
				   TextureRect.Equals( other.TextureRect ) &&
				   FillColor.Equals( other.FillColor ) &&
				   OutlineColor.Equals( other.OutlineColor ) &&
				   OutlineThickness == m_rect.OutlineThickness;
		}

		string m_tex;
		readonly RectangleShape m_rect;
	}
}
