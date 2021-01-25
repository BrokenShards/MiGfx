////////////////////////////////////////////////////////////////////////////////
// Selector.cs 
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
	public class Selector : MiComponent
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Selector()
		:	base()
		{
			SelectedChild = -1;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s">
		///   The object to copy.
		/// </param>
		public Selector( Selector s )
		:	base( s )
		{
			SelectedChild = s.SelectedChild;
		}

		/// <summary>
		///   The index of the selected child.
		/// </summary>
		public int SelectedChild
		{
			get; private set;
		}

		/// <summary>
		///   The component type name
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Selector ); }
		}

		/// <summary>
		///   Selects the child at the given index.
		/// </summary>
		/// <param name="index">
		///   The index of the child to select or a negative value to deselect everything.
		/// </param>
		/// <returns>
		///   True if the parent entity is valid, has a child with a Selectable component at the 
		///   given index (if index is positive) and was selected (or deselected) successfully.
		/// </returns>
		public bool Select( int index )
		{
			if( Parent == null || !Parent.HasChildren || index >= Parent.ChildCount )
				return false;

			if( SelectedChild < -1 )
				SelectedChild = -1;

			if( index >= 0 )
			{
				MiEntity e = Parent.GetChild( index );

				if( !e.Components.Contains<Selectable>() )
					return false;

				e.Components.Get<Selectable>().Selected = true;
			}
			for( int i = 0; i < Parent.ChildCount; i++ )
			{
				if( i == index )
					continue;

				MiEntity e = Parent.GetChild( index );

				if( !e.Components.Contains<Selectable>() )
					continue;

				e.Components.Get<Selectable>().Selected = false;
			}

			SelectedChild = index;
			return true;
		}
		/// <summary>
		///   Selects the next selctable child entity.
		/// </summary>
		/// <returns>
		///   True on success or false if no child to select.
		/// </returns>
		public bool SelectNext()
		{
			if( Parent == null || !Parent.HasChildren )
				return false;

			if( SelectedChild < -1 )
				SelectedChild = -1;

			int initial = SelectedChild + 1;

			for( int i = initial; i < Parent.ChildCount; i++ )
				if( Select( i ) )
					return true;

			if( initial > 0 )
				for( int i = 0; i < initial; i++ )
					if( Select( i ) )
						return true;

			return false;
		}
		/// <summary>
		///   Selects the previous selctable child entity.
		/// </summary>
		/// <returns>
		///   True on success or false if no child to select.
		/// </returns>
		public bool SelectPrevious()
		{
			if( Parent == null || !Parent.HasChildren )
				return false;

			int initial = SelectedChild - 1;

			if( initial <= 0 )
				initial = Parent.ChildCount - 1;

			for( int i = initial; i >= 0; i-- )
				if( Select( i ) )
					return true;

			if( initial + 1 < Parent.ChildCount )
				for( int i = Parent.ChildCount - 1; i > initial; i-- )
					if( Select( i ) )
						return true;

			return false;
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

			try
			{
				SelectedChild = sr.ReadInt32();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Selector from stream: " + e.Message, false, LogType.Error );
			}
			
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

			try
			{
				sw.Write( SelectedChild );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Selector to stream: " + e.Message, false, LogType.Error );
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

			if( element.HasAttribute( nameof( SelectedChild ) ) )
			{
				if( !int.TryParse( element.GetAttribute( nameof( SelectedChild ) ), out int s ) )
					return Logger.LogReturn( "Failed loading Selector: Failed parsing SelectedChild xml attribute.", false, LogType.Error );

				SelectedChild = s;
			}

			return true;
		}

		/// <summary>
		///   Converts the object to an xml string.
		/// </summary>
		/// <returns>
		///   Returns the object as an xml string.
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

			sb.Append( "          " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "          " );
			sb.Append( nameof( SelectedChild ) );
			sb.Append( "=\"" );
			sb.Append( SelectedChild );
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
			return new Selector( this );
		}
	}
}
