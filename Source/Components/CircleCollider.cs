////////////////////////////////////////////////////////////////////////////////
// CircleCollider.cs 
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
	///   A circular collider.
	/// </summary>
	public class CircleCollider : Collider, IEquatable<CircleCollider>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public CircleCollider()
		:	base()
		{
			Offset           = default;
			RadiusMultiplier = 1.0f;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="c">
		///   The object to copy.
		/// </param>
		public CircleCollider( CircleCollider c )
		:	base( c )
		{
			Offset           = c.Offset;
			RadiusMultiplier = c.RadiusMultiplier;
		}
		/// <summary>
		///   Constructor setting collider radius and offset.
		/// </summary>
		/// <param name="radius">
		///   The radius of the circle collider.
		/// </param>
		/// <param name="off">
		///   Collider offset.
		/// </param>
		public CircleCollider( float radius, Vector2f off = default )
		:	base()
		{
			RadiusMultiplier = radius;
			Offset           = off;
		}

		/// <summary>
		///   The type name of the component.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( CircleCollider ); }
		}

		/// <summary>
		///   Collider offset from the entity transform (as a ratio of the transforms' size).
		/// </summary>
		public Vector2f Offset
		{
			get; set;
		}
		/// <summary>
		///   Radius of the circle collider.
		/// </summary>
		public float RadiusMultiplier
		{
			get { return m_rad; }
			set { m_rad = value <= 0 ? 1.0f : value; }
		}

		/// <summary>
		///   The radius of the circle before the multiplier (always half of the smallest side of
		///   the Transform).
		/// </summary>
		/// <remarks>
		///   Returns <see cref="float.NaN"/> if the parent does not have a Transform.
		/// </remarks>
		public float Radius
		{
			get
			{
				Transform t = Parent?.GetComponent<Transform>();

				if( t == null )
					return float.NaN;

				FloatRect gb = t.GlobalBounds;

				return Math.Min( gb.Width, gb.Height ) / 2.0f;
			}
		}

		/// <summary>
		///   Gets the rectangular bounds of the collider.
		/// </summary>
		public override FloatRect Bounds
		{
			get
			{
				Transform t = Parent?.GetComponent<Transform>();

				Vector2f size = new Vector2f( RadiusMultiplier, RadiusMultiplier );

				if( t == null )
					return new FloatRect( Offset, size );

				FloatRect gb = t.GlobalBounds;
				float radius = Radius * RadiusMultiplier;
				Vector2f center = new Vector2f( gb.Left + ( gb.Width / 2.0f ), gb.Top + ( gb.Height / 2.0f ) ) +
					new Vector2f( Offset.X * ( radius * 2 ), Offset.Y * ( radius * 2 ) );

				gb.Left   = center.X - radius;
				gb.Top    = center.Y - radius;
				gb.Width  = radius * 2;
				gb.Height = radius * 2;

				return gb;
			}
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

			if( t == null )
				return false;

			FloatRect b = Bounds;
			float radius = Radius * RadiusMultiplier;

			Vector2f center = new Vector2f( b.Left + ( b.Width / 2.0f ), b.Top + ( b.Height / 2.0f ) );
			center += new Vector2f( Offset.X * ( radius * 2 ), Offset.Y * ( radius * 2 ) );

			return Physics.CircleContainsPoint( center, RadiusMultiplier * Math.Min( t.Scale.X, t.Scale.Y ), point );
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
			Transform t = Parent?.GetComponent<Transform>();

			if( t == null )
				return false;
			
			FloatRect b = Bounds;
			float radius = Radius * RadiusMultiplier;

			Vector2f center = new Vector2f( b.Left + ( b.Width / 2.0f ), b.Top + ( b.Height / 2.0f ) );
			center += new Vector2f( Offset.X * ( radius * 2 ), Offset.Y * ( radius * 2 ) );

			return Physics.LineIntersectsCircle( center, radius, l1, l2 );
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

			if( t == null )
				return false;

			FloatRect b = Bounds;
			float radius = Radius * RadiusMultiplier;

			Vector2f center = new Vector2f( b.Left + ( b.Width / 2.0f ), b.Top + ( b.Height / 2.0f ) );
			center += new Vector2f( Offset.X * ( radius * 2 ), Offset.Y * ( radius * 2 ) );

			return Physics.RectIntersectsCircle( rect, center, RadiusMultiplier * Math.Min( t.Scale.X, t.Scale.Y ) );
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
				nameof( BoxCollider )
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
				RadiusMultiplier = sr.ReadSingle();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading CircleCollider from stream: " + e.Message, false, LogType.Error );
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
				sw.Write( RadiusMultiplier );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving CircleCollider to stream: " + e.Message, false, LogType.Error );
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
			RadiusMultiplier = 100;

			XmlElement off = element[ nameof( Offset ) ];

			if( off != null )
			{
				Vector2f? o = Xml.ToVec2f( off );

				if( !o.HasValue )
					return Logger.LogReturn( "Failed loading CircleCollider: Parsing Offset failed.", false, LogType.Error );

				Offset = o.Value;
			}

			if( !element.HasAttribute( nameof( RadiusMultiplier ) ) )
				return Logger.LogReturn( "Failed loading CircleCollider: No Radius attribute.", false, LogType.Error );
	
			if( !float.TryParse( element.GetAttribute( nameof( RadiusMultiplier ) ), out float r ) )
				return Logger.LogReturn( "Failed loading CircleCollider: Parsing Radius failed.", false, LogType.Error );

			RadiusMultiplier = r;
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
			sb.Append( nameof( RadiusMultiplier ) );
			sb.Append( "=\"" );
			sb.Append( RadiusMultiplier );
			sb.AppendLine( "\"" );

			sb.Append( "             " );
			sb.Append( nameof( StaticCollider ) );
			sb.Append( "=\"" );
			sb.Append( StaticCollider );
			sb.AppendLine( "\">" );

			if( !Offset.Equals( default ) )
				sb.AppendLine( Xml.ToString( Offset, nameof( Offset ), 1 ) );

			sb.Append( "</" );
			sb.Append( TypeName );
			sb.AppendLine( ">" );

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
		public bool Equals( CircleCollider other )
		{
			return base.Equals( other ) &&
				   Offset.Equals( other.Offset ) &&
				   RadiusMultiplier == other.RadiusMultiplier;
		}
		/// <summary>
		///   Deep coppies this object.
		/// </summary>
		/// <returns>
		///   A deep copy of this object.
		/// </returns>
		public override object Clone()
		{
			return new CircleCollider( this );
		}

		private float m_rad;
	}
}
