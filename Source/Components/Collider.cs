////////////////////////////////////////////////////////////////////////////////
// Collider.cs 
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

using SFML.Graphics;
using SFML.System;

using MiCore;
using System.IO;
using System.Xml;

namespace MiGfx
{
	/// <summary>
	///   Base class for collision shapes.
	/// </summary>
	public abstract class Collider : MiComponent, IEquatable<Collider>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Collider()
		:	base()
		{
			StaticCollider = false;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="col">
		///   The object to copy.
		/// </param>
		public Collider( Collider col )
		:	base( col )
		{
			StaticCollider = col.StaticCollider;
		}

		/// <summary>
		///   If the collider is considered static and is not moved.
		/// </summary>
		public bool StaticCollider
		{
			get; set;
		}

		/// <summary>
		///   Gets the rectangular bounds of the collider.
		/// </summary>
		public abstract FloatRect Bounds
		{
			get;
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
			return new string[] { nameof( UITransform ) };
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
		public abstract bool ContainsPoint( Vector2f point );
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
		public abstract bool LineIntersects( Vector2f l1, Vector2f l2 );
		/// <summary>
		///   Checks if the given rect intersects the collider.
		/// </summary>
		/// <param name="rect">
		///   The point to check.
		/// </param>
		/// <returns>
		///   True if the point is within the collider and false otherwise.
		/// </returns>
		public abstract bool Intersects( FloatRect rect );

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
				StaticCollider = sr.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Collider from stream: " + e.Message, false, LogType.Error );
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
				sw.Write( StaticCollider );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Collider to stream: " + e.Message, false, LogType.Error );
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

			StaticCollider = false;

			if( element.HasAttribute( nameof( StaticCollider ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( StaticCollider ) ), out bool s ) )
					return Logger.LogReturn( "Failed loading Collider: Unable to parse StaticCollider attribute.", false, LogType.Error );

				StaticCollider = s;
			}

			return true;
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
		public bool Equals( Collider other )
		{
			return base.Equals( other ) && StaticCollider == other.StaticCollider;
		}
	}
}
