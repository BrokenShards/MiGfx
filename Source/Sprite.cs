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
	public class Sprite : BinarySerializable, IXmlLoadable, Drawable, ITransformable, IDisposable, IEquatable<Sprite>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Sprite()
		{
			Image     = new ImageInfo();
			Transform = new Transform();
			m_verts   = new VertexArray( PrimitiveType.Quads, 4 );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s"></param>
		public Sprite( Sprite s )
		{
			if( s == null )
				throw new ArgumentNullException();

			Image     = new ImageInfo( s.Image );
			Transform = new Transform( s.Transform );
			m_verts   = new VertexArray( PrimitiveType.Quads, 4 );

			for( uint i = 0; i < 4; i++ )
				m_verts[ i ] = s.m_verts[ i ];
		}
		/// <summary>
		///   Constructs the sprite with the given image info.
		/// </summary>
		/// <param name="i">
		///   Image info.
		/// </param>
		public Sprite( ImageInfo i )
		{
			Image     = i ?? new ImageInfo();
			Transform = new Transform();
			m_verts   = new VertexArray( PrimitiveType.Quads, 4 );
		}

		/// <summary>
		///   Image info.
		/// </summary>
		public ImageInfo Image
		{
			get; set;
		}
		/// <summary>
		///   Transform.
		/// </summary>
		public Transform Transform
		{
			get; set;
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
		public virtual void Update( float dt )
		{
			if( Image == null )
				throw new ArgumentNullException( nameof( Image ), "Trying to update a sprite with a null image." );

			for( uint i = 0; i < m_verts.VertexCount; i++ )
				m_verts[ i ] = Image.GetVertex( i, Transform );
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
		public void Draw( RenderTarget target, RenderStates states )
		{
			if( Image == null )
				throw new ArgumentNullException( nameof( Image ), "Trying to draw a sprite with a null image." );

			Texture tex = Assets.Manager.Texture.Get( Image.Path );

			if( tex != null )
			{
				states.Texture = tex;

				FloatRect rect = Image.Rect;

				if( rect.Width == 0 )
					rect.Width = states.Texture.Size.X;
				if( rect.Height == 0 )
					rect.Height = states.Texture.Size.Y;

				Image.Rect = rect;
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
			if( br == null )
				return Logger.LogReturn( "Unable to load Sprite from null stream.", false, LogType.Error );

			if( !Image.LoadFromStream( br ) )
				return Logger.LogReturn( "Unable to load Sprite from stream: Failed loading Image.", false, LogType.Error );
			if( !Transform.LoadFromStream( br ) )
				return Logger.LogReturn( "Unable to load Sprite from stream: Failed loading Transform.", false, LogType.Error );

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
			if( bw == null )
				return Logger.LogReturn( "Unable to save Sprite to null stream.", false, LogType.Error );

			if( Image == null )
				Image = new ImageInfo();
			if( Transform == null )
				Transform = new Transform();

			if( !Image.SaveToStream( bw ) )
				return Logger.LogReturn( "Unable to save sprite image to stream.", false, LogType.Error );
			if( !Transform.SaveToStream( bw ) )
				return Logger.LogReturn( "Unable to save sprite transform to stream.", false, LogType.Error );

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
				return Logger.LogReturn( "Cannot load Sprite from a null XmlElement.", false, LogType.Error );

			Image     = new ImageInfo();
			Transform = new Transform();

			XmlElement info = element[ nameof( ImageInfo ) ],
					   tran = element[ nameof( MiGfx.Transform ) ];

			if( info == null )
				return Logger.LogReturn( "Failed loading Sprite: No " + nameof( ImageInfo ) + " xml element.", false, LogType.Error );
			if( tran == null )
				return Logger.LogReturn( "Failed loading Sprite: No " + nameof( Transform ) + " xml element.", false, LogType.Error );
			if( !Image.LoadFromXml( info ) )
				return Logger.LogReturn( "Failed loading Sprite: Loading ImageInfo failed.", false, LogType.Error );
			if( !Transform.LoadFromXml( tran ) )
				return Logger.LogReturn( "Failed loading Sprite: Loading Transform failed.", false, LogType.Error );

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
			sb.Append( nameof( Sprite ) );
			sb.AppendLine( ">" );

			sb.AppendLine( XmlLoadable.ToString( Image, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( Sprite ) );
			sb.AppendLine( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   Disposes of managed resources.
		/// </summary>
		public void Dispose()
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
			       Image.Equals( other.Image ) &&
				   Transform.Equals( other.Transform );
		}

		private VertexArray m_verts;
	}
}
