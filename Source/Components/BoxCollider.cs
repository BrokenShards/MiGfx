////////////////////////////////////////////////////////////////////////////////
// BoxCollider.cs 
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
using System.Collections.Generic;

namespace MiGfx
{
	/// <summary>
	///   An axis-aligned rectangular collider.
	/// </summary>
	public class BoxCollider : Collider, IEquatable<BoxCollider>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public BoxCollider()
		:	base()
		{
			Offset = default;
			Size   = new Vector2f( 1.0f, 1.0f );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="box">
		///   The object to copy.
		/// </param>
		public BoxCollider( BoxCollider box )
		:	base( box )
		{
			Offset = box.Offset;
			Size   = box.Size;
		}
		/// <summary>
		///   Constructor setting collider offset and size.
		/// </summary>
		/// <param name="off">
		///   The collider offset.
		/// </param>
		/// <param name="size">
		///   The collider size.
		/// </param>
		public BoxCollider( Vector2f off, Vector2f size )
		:	base()
		{
			Offset = off;
			Size   = size;
		}
		
		/// <summary>
		///   Collider offset from the entity transform (as a ratio of the transforms' size).
		/// </summary>
		public Vector2f Offset
		{
			get; set;
		}
		/// <summary>
		///   Collider size (as a ratio of the transforms' size).
		/// </summary>
		public Vector2f Size
		{
			get; set;
		}

		/// <summary>
		///   Gets the rectangular bounds of the collider.
		/// </summary>
		public override FloatRect Bounds
		{
			get
			{
				Transform t = Parent?.GetComponent<Transform>();

				if( t is null )
					return new FloatRect( Offset, Size );

				FloatRect gb = t.GlobalBounds;
				gb.Left   += gb.Width * Offset.X;
				gb.Top    += gb.Top   * Offset.Y;
				gb.Width  *= Size.X;
				gb.Height *= Size.Y;
				
				return gb;
			}
		}

		/// <summary>
		///   The type name of the component.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( BoxCollider ); }
		}

		/// <summary>
		///   Checks if the given point is inside the collider.
		/// </summary>
		/// <param name="point">
		///   The point to check.
		/// </param>
		/// <returns>
		///   True if the point is within the collider and false otherwise.
		/// </returns>
		public override bool ContainsPoint( Vector2f point )
		{
			Transform t = Parent?.GetComponent<Transform>();

			if( t is null )
				return false;

			return Bounds.Contains( point.X, point.Y );
		}
		/// <summary>
		///   Checks if the line made up from the given coordinates intersects the collider.
		/// </summary>
		/// <param name="l1">
		///   The first point of the line.
		/// </param>
		/// <param name="l2">
		///   The second point of the line.
		/// </param>
		/// <returns>
		///   True if the line intersects the collider, otherwise false.
		/// </returns>
		public override bool LineIntersects( Vector2f l1, Vector2f l2 )
		{
			return Physics.LineIntersectsRect( l1, l2, Bounds );
		}
		/// <summary>
		///   Checks if the given rect intersects the collider.
		/// </summary>
		/// <param name="rect">
		///   The point to check.
		/// </param>
		/// <returns>
		///   True if the point is within the collider and false otherwise.
		/// </returns>
		public override bool Intersects( FloatRect rect )
		{
			Transform t = Parent?.GetComponent<Transform>();

			if( t is null )
				return false;

			return Bounds.Intersects( rect );
		}

		/// <summary>
		///   Get the type names of the components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of the components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new List<string>( base.GetIncompatibleComponents() )
			{
				nameof( CircleCollider )
			}.ToArray();
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
				Offset = new Vector2f( sr.ReadSingle(), sr.ReadSingle() );
				Size   = new Vector2f( sr.ReadSingle(), sr.ReadSingle() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed loading BoxCollider from stream: { e.Message }", false, LogType.Error );
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
				sw.Write( Offset.X ); sw.Write( Offset.Y );
				sw.Write( Size.X );   sw.Write( Size.Y );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed saving BoxCollider to stream: { e.Message }", false, LogType.Error );
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

			Offset = default;
			Size   = default;

			XmlElement off = element[ nameof( Offset ) ],
			           siz = element[ nameof( Size ) ];

			if( off is not null )
			{
				Vector2f? o = Xml.ToVec2f( off );

				if( !o.HasValue )
					return Logger.LogReturn( "Failed loading BoxCollider: Parsing Offset failed.", false, LogType.Error );

				Offset = o.Value;
			}
			if( siz is not null )
			{
				Vector2f? s = Xml.ToVec2f( siz );

				if( !s.HasValue )
					return Logger.LogReturn( "Failed loading BoxCollider: Parsing Size failed.", false, LogType.Error );

				Size = s.Value;
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
			StringBuilder sb = new();

			sb.Append( '<' ).Append( TypeName ).Append( ' ' )
				.Append( nameof( Enabled ) ).Append( "=\"" ).Append( Enabled ).AppendLine( "\"" )
				.Append( "             " )
				.Append( nameof( Visible ) ).Append( "=\"" ).Append( Visible ).AppendLine( "\"" )
				.Append( "             " )
				.Append( nameof( StaticCollider ) ).Append( "=\"" ).Append( StaticCollider ).AppendLine( "\">" );

			if( !Offset.Equals( default ) )
				sb.AppendLine( Xml.ToString( Offset, nameof( Offset ), 1 ) );
			
			sb.AppendLine( Xml.ToString( Size, nameof( Size ), 1 ) )
				.Append( "</" ).Append( TypeName ).AppendLine( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   If this object is considered equal to another.
		/// </summary>
		/// <param name="other">
		///   The object to check against.
		/// </param>
		/// <returns>
		///   True if the other object is considered equal to this, otherwise false.
		/// </returns>
		public bool Equals( BoxCollider other )
		{
			return base.Equals( other ) &&
				   Offset.Equals( other.Offset ) &&
				   Size.Equals( other.Size );
		}
		/// <summary>
		///   If this object has the same values of the other object.
		/// </summary>
		/// <param name="obj">
		///   The other object to check against.
		/// </param>
		/// <returns>
		///   True if both objects are concidered equal and false if they are not.
		/// </returns>
		public override bool Equals( object obj )
		{
			return Equals( obj as BoxCollider );
		}

		/// <summary>
		///   Serves as the default hash function.
		/// </summary>
		/// <returns>
		///   A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return HashCode.Combine( base.GetHashCode(), Offset, Size );
		}
		/// <summary>
		///   Deep coppies this object.
		/// </summary>
		/// <returns>
		///   A deep copy of this object.
		/// </returns>
		public override object Clone()
		{
			return new BoxCollider( this );
		}
	}
}