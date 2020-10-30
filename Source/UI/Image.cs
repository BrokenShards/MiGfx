////////////////////////////////////////////////////////////////////////////////
// Image.cs 
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
using SFML.Graphics;
using SharpLogger;

namespace SharpGfx.UI
{
	/// <summary>
	///   Image UI element.
	/// </summary>
	public class Image : UIElement, IEquatable<Image>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Image()
		:	base()
		{
			m_sprite = new Sprite();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="e">
		///   The image to copy from.
		/// </param>
		public Image( Image e )
		:	base( e )
		{
			m_sprite = new Sprite( e.m_sprite );
		}
		/// <summary>
		///   Constructs the image with an ID.
		/// </summary>
		/// <param name="id">
		///   The image ID.
		/// </param>
		public Image( string id )
		:	base( id )
		{
			m_sprite = new Sprite();
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UI.Image ); }
		}

		/// <summary>
		///   Info on the image to display.
		/// </summary>
		public ImageInfo DisplayImage
		{
			get { return m_sprite.Image; }
			set { m_sprite.Image = value; }
		}

		/// <summary>
		///   Called when disposing of the object.
		/// </summary>
		protected override void OnDispose()
		{
			m_sprite.Dispose();
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			m_sprite.Transform = Transform;
			m_sprite.Update( dt );
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
			m_sprite.Draw( target, states );
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

			if( !m_sprite.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load UIImages' Image from stream", false, LogType.Error );

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

			if( !m_sprite.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save UIImages' Image to stream", false, LogType.Error );

			return true;
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
		public bool Equals( Image other )
		{
			return base.Equals( other ) &&
				   m_sprite.Image.Equals( other.m_sprite.Image );
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

		private Sprite m_sprite;
	}
}
