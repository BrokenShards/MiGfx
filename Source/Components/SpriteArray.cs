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
		}

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( SpriteArray ); }
		}

		/// <summary>
		///   If this is not null, this texture will be used instead of using the texture manager.
		/// </summary>
		public Texture OverrideTexture
		{
			get; set;
		}
		/// <summary>
		///   The texture path.
		/// </summary>
		public string TexturePath
		{
			get; set;
		}

		/// <summary>
		///   Gets the texture.
		/// </summary>
		public Texture Texture
		{
			get { return OverrideTexture ?? Assets.Manager.Texture.Get( TexturePath ); }
		}

		/// <summary>
		///   List of sprites.
		/// </summary>
		public List<SpriteInfo> Sprites
		{
			get; private set; 
		}

		/// <summary>
		///   Gets a bounding rectangle containing all sprites.
		/// </summary>
		public FloatRect SpriteBounds
		{
			get
			{
				if( Sprites.Count == 0 )
					return default;
				
				float left  = 0, top    = 0,
					  right = 0, bottom = 0;

				for( uint i = 0; i < m_verts.VertexCount; i++ )
				{
					if( i == 0 )
					{
						left   = m_verts[ i ].Position.X;
						right  = m_verts[ i ].Position.X;
						top    = m_verts[ i ].Position.Y;
						bottom = m_verts[ i ].Position.Y;
					}
					else
					{
						if( m_verts[ i ].Position.X < left )
							left = m_verts[ i ].Position.X;
						if( m_verts[ i ].Position.X > right )
							right = m_verts[ i ].Position.X;

						if( m_verts[ i ].Position.Y < top )
							top = m_verts[ i ].Position.Y;
						if( m_verts[ i ].Position.Y > bottom )
							bottom = m_verts[ i ].Position.Y;
					}
				}

				return new FloatRect( left, top, right - left, bottom - top );
			}
		}

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Sprite ), nameof( SpriteAnimator ) };
		}

		/// <summary>
		///   Refreshes components' visual elements.
		/// </summary>
		public override void Refresh()
		{
			if( Parent == null )
				return;

			Texture tex = Texture;

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

			UpdateVerts( Parent.GetComponent<Transform>() );
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

			states.Texture = Texture;
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
							m_verts[ ( (uint)i * 4u ) + v ] = Sprites[ i ].GetVertex( v, t.GlobalBounds, t.Scale );
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

			if( element.HasAttribute( nameof( TexturePath ) ) )
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
			m_verts?.Dispose();
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
			if( !base.Equals( other ) || OverrideTexture != other.OverrideTexture ||
				Sprites.Count != other.Sprites.Count ||
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

		/// <summary>
		///   Vertices.
		/// </summary>
		protected VertexArray m_verts;
	}
}
