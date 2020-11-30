////////////////////////////////////////////////////////////////////////////////
// Sprite.cs 
////////////////////////////////////////////////////////////////////////////////
//
// SharpGfx - A basic graphics library for use with SFML.Net.
// Copyright (C) 2020 Michael Furlong <michaeljfurlong@outlook.com>
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

using SharpLogger;
using SharpSerial;

namespace SharpGfx
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

			XmlElement img = element[ "image_info" ],
					   trn = element[ "color" ];

			if( img == null )
				return Logger.LogReturn( "Failed loading Sprite: No image_info element.", false, LogType.Error );
			if( trn == null )
				return Logger.LogReturn( "Failed loading Sprite: No transform element.", false, LogType.Error );

			Image     = new ImageInfo();
			Transform = new Transform();

			if( !Image.LoadFromXml( img ) )
				return Logger.LogReturn( "Failed loading Sprite: Loading ImageInfo failed.", false, LogType.Error );
			if( !Transform.LoadFromXml( trn ) )
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

			sb.AppendLine( "<sprite>" );
			sb.AppendLine( XmlLoadable.ToString( Image, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );
			sb.Append( "</sprite>" );

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

	/// <summary>
	///   An animated sprite.
	/// </summary>
	public class AnimatedSprite : Sprite, IEquatable<AnimatedSprite>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public AnimatedSprite()
		:	base()
		{
			Animator = new Animator();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="a">
		///   The sprite to copy from.
		/// </param>
		public AnimatedSprite( AnimatedSprite a )
		:	base( a )
		{
			Animator = a.Animator == null ? null : new Animator( a.Animator );
		}
		/// <summary>
		///   Constructs the sprite with the given image info and optional animator.
		/// </summary>
		/// <param name="i">
		///   Image info.
		/// </param>
		/// /// <param name="a">
		///   Animator for sprite animation.
		/// </param>
		public AnimatedSprite( ImageInfo i, Animator a = null )
		:	base( i )
		{
			Animator = a ?? new Animator();
		}

		/// <summary>
		///   The animator that controls the sprite's animations.
		/// </summary>
		public Animator Animator
		{
			get; set;
		}

		/// <summary>
		///   Updates the animated sprite. Call before drawing.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		public override void Update( float dt )
		{
			if( Animator != null )
			{
				Animator.Update( dt );

				if( Animator.CurrentFrame != null )
					Image.Rect = Animator.CurrentFrame.Rect;
			}

			base.Update( dt );
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
			if( Animator == null )
				Animator = new Animator();

			if( !base.LoadFromStream( br ) )
				return false;
			if( !Animator.LoadFromStream( br ) )
				return Logger.LogReturn( "Unable to load AnimatedSprite's Animator from stream.", false, LogType.Error );

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
			if( Animator == null )
				Animator = new Animator();

			if( !base.SaveToStream( bw ) )
				return false;
			if( !Animator.SaveToStream( bw ) )
				return Logger.LogReturn( "Unable to save AnimatedSprite's Animator to stream.", false, LogType.Error );

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

			XmlElement ani = element[ "animator" ];

			if( ani == null )
				return Logger.LogReturn( "Failed loading AnimatedSprite: No animator element.", false, LogType.Error );

			if( Animator == null )
				Animator = new Animator();

			if( !Animator.LoadFromXml( ani ) )
				return Logger.LogReturn( "Failed loading AnimatedSprite: Loading Animator failed.", false, LogType.Error );

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

			sb.AppendLine( "<animated_sprite>" );
			sb.AppendLine( XmlLoadable.ToString( Image, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Animator, 1 ) );
			sb.Append( "</animated_sprite>" );

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
		public bool Equals( AnimatedSprite other )
		{
			return other != null &&
				   base.Equals( other ) &&
				   ( ( Animator == null && other.Animator == null ) ||
				   Animator.Equals( other.Animator ) );
		}
	}
}
