////////////////////////////////////////////////////////////////////////////////
// Selectable.cs 
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

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   A component making an entity selectable.
	/// </summary>
	public class Selectable : MiComponent
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Selectable()
		:	base()
		{
			Selector = null;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s">
		///   The object to copy.
		/// </param>
		public Selectable( Selectable s )
		:	base( s )
		{
			Selector = null;
		}

		/// <summary>
		///   The selector object managing this selectable.
		/// </summary>
		public Selector Selector 
		{ 
			get; set; 
		}

		/// <summary>
		///   If currently selected.
		/// </summary>
		/// <remarks>
		///   When assigning to true, there is no guarantee the object will be successfully
		///   selected; Check the value after assignment to ensure the object has been selected.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///   If trying to assign with no parent or the selector is invalid or has no parent.
		/// </exception>
		public bool Selected
		{
			get
			{
				if( Parent == null || Selector?.Parent == null )
					return false;

				return Selector.Selected == Parent;
			}
			set
			{
				if( Parent == null || Selector?.Parent == null )
					throw new InvalidOperationException();

				if( value )
					Selector.Select( Parent );
				else if( Selector.Selected == Parent )
					Selector.Select( null );
			}
		}

		/// <summary>
		///   The component type name
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Selectable ); }
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   The stream reader
		/// </param>
		/// <returns>
		///   True if the sprite was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader sr )
		{
			if( !base.LoadFromStream( sr ) )
				return false;

			return true;
		}
		/// <summary>
		///   Writes the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the sprite was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter sw )
		{
			if( !base.SaveToStream( sw ) )
				return false;

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

			sb.Append( "            " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"/>" );

			return sb.ToString();
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new Selectable( this );
		}
	}
}
