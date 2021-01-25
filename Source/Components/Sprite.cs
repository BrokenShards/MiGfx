////////////////////////////////////////////////////////////////////////////////
// Sprite.cs 
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

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   A graphical sprite.
	/// </summary>
	public class Sprite : MiComponent, IEquatable<Sprite>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Sprite()
		:	base()
		{
			Image     = new ImageInfo();
			m_verts   = new VertexArray( PrimitiveType.Quads, 4 );

			RequiredComponents = new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s"></param>
		public Sprite( Sprite s )
		:	base( s )
		{
			if( s == null )
				throw new ArgumentNullException();

			Image     = new ImageInfo( s.Image );
			m_verts   = new VertexArray( PrimitiveType.Quads, 4 );

			for( uint i = 0; i < 4; i++ )
				m_verts[ i ] = s.m_verts[ i ];

			RequiredComponents = new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Constructs the sprite with the given image info.
		/// </summary>
		/// <param name="i">
		///   Image info.
		/// </param>
		public Sprite( ImageInfo i )
		:	base()
		{
			Image     = i ?? new ImageInfo();
			m_verts   = new VertexArray( PrimitiveType.Quads, 4 );

			RequiredComponents = new string[] { nameof( Transform ) };
		}

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Sprite ); }
		}

		/// <summary>
		///   Image info.
		/// </summary>
		public ImageInfo Image
		{
			get; set;
		}

		/// <summary>
		///   Updates the sprite geometry. Call this before drawing.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			Transform t = Stack?.Get<Transform>();

			if( Image != null && t != null )
				for( uint i = 0; i < m_verts.VertexCount; i++ )
					m_verts[ i ] = Image.GetVertex( i, t );
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
			if( Image != null )
			{
				Texture tex = Assets.Manager.Texture.Get( Image.Path );

				if( tex != null )
				{
					states.Texture = tex;

					FloatRect rect = Image.Rect;

					if( rect.Width == 0 )
						rect.Width = states.Texture.Size.X - rect.Left;
					if( rect.Height == 0 )
						rect.Height = states.Texture.Size.Y - rect.Top;

					Image.Rect = rect;
				}
			}

			m_verts.Draw( target, states );
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

			if( !Image.LoadFromStream( br ) )
				return Logger.LogReturn( "Failed loading Sprite's ImageInfo from stream.", false, LogType.Error );

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

			if( Image == null )
				Image = new ImageInfo();

			if( !Image.SaveToStream( bw ) )
				return Logger.LogReturn( "Failed saving Sprite's ImageInfo to stream.", false, LogType.Error );

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

			Image = new ImageInfo();
			XmlElement info = element[ nameof( ImageInfo ) ];

			if( info == null )
				return Logger.LogReturn( "Failed loading Sprite: No ImageInfo element.", false, LogType.Error );
			if( !Image.LoadFromXml( info ) )
				return Logger.LogReturn( "Failed loading Sprite: Parsing ImageInfo failed.", false, LogType.Error );

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
			sb.AppendLine( "\">" );

			sb.AppendLine( XmlLoadable.ToString( Image, 1 ) );

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
		public bool Equals( Sprite other )
		{
			return other != null && 
			       Image.Equals( other.Image );
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new Sprite( this );
		}

		private VertexArray m_verts;
	}
}
