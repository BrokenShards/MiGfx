////////////////////////////////////////////////////////////////////////////////
// AnimatedImage.cs 
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

namespace MiGfx.UI
{
	/// <summary>
	///   An animated UI image.
	/// </summary>
	[Serializable]
	public class AnimatedImage : UIElement, IEquatable<AnimatedImage>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public AnimatedImage()
		:	base()
		{
			Sprite = new AnimatedSprite();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="e">
		///   The object to copy.
		/// </param>
		public AnimatedImage( AnimatedImage e )
		:	base( e )
		{
			Sprite = new AnimatedSprite( e.Sprite );
		}
		/// <summary>
		///   Constructs the object with the given ID.
		/// </summary>
		/// <param name="id"></param>
		public AnimatedImage( string id )
		:	base( id )
		{
			Sprite = new AnimatedSprite();
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName 
		{
			get { return nameof( AnimatedImage ); }
		}

		/// <summary>
		///   The animated sprite.
		/// </summary>
		public AnimatedSprite Sprite
		{
			get; private set;
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			Sprite.Transform = Transform;
			Sprite.Update( dt );
		}
		/// <summary>
		///   Draws the element.
		/// </summary>
		/// <param name="target">
		///   The render target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			Sprite.Draw( target, states );
		}

		/// <summary>
		///   Called when disposing of the object.
		/// </summary>
		protected override void OnDispose()
		{
			Sprite.Dispose();
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
			if( !Sprite.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load UIAnimatedImage from stream.", false, LogType.Error );

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
			if( !Sprite.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save UIAnimatedImage to stream.", false, LogType.Error );

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

			Sprite = new AnimatedSprite();

			XmlElement aspr = element[ nameof( AnimatedSprite ) ];

			if( aspr == null )
				return Logger.LogReturn( "Failed loading AnimatedImage: No AnimatedSprite xml element.", false, LogType.Error );
			if( !Sprite.LoadFromXml( aspr ) )
				return Logger.LogReturn( "Failed loading AnimatedImage: Loading AnimatedSprite failed.", false, LogType.Error );

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
			sb.Append( nameof( AnimatedImage ) );
			sb.Append( " " + nameof( ID ) + "=\"" );
			sb.Append( ID );
			sb.AppendLine( "\"" );

			sb.Append( "               " + nameof( Enabled ) + "=\"" );
			sb.Append( Enabled );
			sb.AppendLine( "\"" );

			sb.Append( "               " + nameof( Visible ) + "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\">" );

			sb.AppendLine( XmlLoadable.ToString( Transform, 1 ) );
			sb.AppendLine( XmlLoadable.ToString( Sprite, 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( AnimatedImage ) );
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
		public bool Equals( AnimatedImage other )
		{
			return base.Equals( other ) && Sprite.Equals( other.Sprite );
		}

		/// <summary>
		///   Gets the type name.
		/// </summary>
		/// <returns>
		///   The type name.
		/// </returns>
		public static string GetTypeName()
		{
			string name = string.Empty;

			using( AnimatedImage a = new AnimatedImage() )
				name = a.TypeName;

			return name;
		}
	}
}
