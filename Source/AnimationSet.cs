////////////////////////////////////////////////////////////////////////////////
// AnimationSet.cs 
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   A set of string indexed animations.
	/// </summary>
	public class AnimationSet : BinarySerializable, IXmlLoadable, IEnumerable<string>, IEquatable<AnimationSet>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public AnimationSet()
		{
			m_anims = new List<string>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="a">
		///   The set to copy from.
		/// </param>
		public AnimationSet( AnimationSet a )
		{
			if( a is null )
				throw new ArgumentNullException( nameof( a ) );

			m_anims = new List<string>( a.Count );

			foreach( var v in m_anims )
				Add( new string( v.ToCharArray() ) );
		}

		/// <summary>
		///   Access the animation ID at the given index.
		/// </summary>
		/// <param name="index">
		///   The index of the animation to access.
		/// </param>
		/// <returns>
		///   The ID of the animation at the given index.
		/// </returns>
		public string this[ int index ]
		{
			get { return Get( index ); }
			set { if( !Set( index, value ) ) throw new ArgumentException( "Failed setting animation." ); }
		}

		/// <summary>
		///   If the set contains no animations.
		/// </summary>
		public bool Empty
		{
			get { return Count is 0; }
		}
		/// <summary>
		///   The amount of animations the set contains.
		/// </summary>
		public int Count
		{
			get { return m_anims.Count; }
		}

		/// <summary>
		///   If the set contains an animation with the given ID.
		/// </summary>
		/// <param name="id">
		///   The animation ID.
		/// </param>
		/// <returns>
		///   True if the set contains an animation with the given ID, otherwise false.
		/// </returns>
		public bool Contains( string id )
		{
			if( string.IsNullOrWhiteSpace( id ) )
				return false;

			return m_anims.Contains( id.ToLower() );
		}

		/// <summary>
		///   Retrieves the animation from the database with the ID at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Animation GetAnimation( int index )
		{
			if( !DatabaseManager.Instance.Load<AnimationDB, Animation>() ||
				index < 0 || index >= Count )
				return null;

			return DatabaseManager.Instance.Get<AnimationDB, Animation>().Get( m_anims[ index ] );
		}

		/// <summary>
		///   Gets the animation ID at the given index.
		/// </summary>
		/// <param name="index">
		///   The animation index.
		/// </param>
		/// <returns>
		///   The animation ID at the given index or null if the index is out of range.
		/// </returns>
		public string Get( int index )
		{
			if( index < 0 || index >= Count )
				return null;

			return m_anims[ index ];
		}
		/// <summary>
		///   Replaces an existing animation ID.
		/// </summary>
		/// <remarks>
		///   If setting an animation ID that already exists at a different index, the old ID will
		///   be removed after setting.
		/// </remarks>
		/// <param name="index">
		///   The animation index.
		/// </param>
		/// <param name="anim">
		///   The new animation ID.
		/// </param>
		/// <returns>
		///   True if index is within range and the animation ID was set, otherwise false.
		/// </returns>
		public bool Set( int index, string anim )
		{
			if( index < 0 || index >= Count || !Identifiable.IsValid( anim ) )
				return false;

			int i = IndexOf( anim );
			m_anims[ index ] = anim.ToLower();

			if( i >= 0 && i != index )
				Remove( index );

			return true;
		}
		/// <summary>
		///   Adds an animation by ID to the set.
		/// </summary>
		/// <param name="anim">
		///   The animation ID to add.
		/// </param>
		/// <returns>
		///   True if the animation ID was either already in the set or it was successfully added,
		///   otherwise false.
		/// </returns>
		public bool Add( string anim )
		{
			if( !Identifiable.IsValid( anim ) )
				return false;

			if( !Contains( anim ) )
				m_anims.Add( anim.ToLower() );			
			
			return true;
		}
		/// <summary>
		///   Removes the animation ID from the given index.
		/// </summary>
		/// <param name="index">
		///   The index of the animation ID.
		/// </param>
		/// <returns>
		///   True if index is within range and the animation ID was removed, otherwise false.
		/// </returns>
		public bool Remove( int index )
		{
			if( index < 0 || index >= Count )
				return false;

			m_anims.RemoveAt( index );
			return true;
		}
		/// <summary>
		///   Removes the animation with the given ID from the set.
		/// </summary>
		/// <param name="name">
		///   The name of the animation to remove.
		/// </param>
		/// <returns>
		///   True if an animation existed with the given ID and was removed successfully, otherwise
		///   false.
		/// </returns>
		public bool Remove( string name )
		{
			if( string.IsNullOrWhiteSpace( name ) )
				return false;

			return m_anims.Remove( name.ToLower() );
		}
		/// <summary>
		///   Removes all animations from the set.
		/// </summary>
		public void RemoveAll()
		{
			m_anims.Clear();
		}

		/// <summary>
		///   Gets the index of the animation ID.
		/// </summary>
		/// <param name="anim">
		///   The animation ID.
		/// </param>
		/// <returns>
		///   The index of the animation ID or -1 if 
		/// </returns>
		public int IndexOf( string anim )
		{
			if( Identifiable.IsValid( anim ) )
				for( int i = 0; i < Count; i++ )
					if( m_anims[ i ].ToLower().Equals( anim.ToLower() ) )
						return i;

			return -1;
		}

		/// <summary>
		///   Gets an enumerator that can enumerate over the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can enumerate over the collection.
		/// </returns>
		public IEnumerator<string> GetEnumerator()
		{
			return ( (IEnumerable<string>)m_anims ).GetEnumerator();
		}
		/// <summary>
		///   Gets an enumerator that can enumerate over the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can enumerate over the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable<string>)m_anims ).GetEnumerator();
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( br is null )
				return Logger.LogReturn( "Cannot load AnimationSet from null stream.", false, LogType.Error );

			try
			{
				uint len = br.ReadUInt32();

				for( int i = 0; i < len; i++ )
					if( !Add( br.ReadString() ) )
						return Logger.LogReturn( "Failed adding Animation ID to AnimationSet loaded from stream.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed loading AnimationSet from stream: { e.Message }", false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Writes the object to the stream.
		/// </summary>
		/// <param name="bw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the object was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( bw is null )
				return Logger.LogReturn( "Cannot save AnimationSet to null stream.", false, LogType.Error );

			RemoveAll();

			try
			{
				bw.Write( (uint)Count );

				foreach( var v in m_anims )
					bw.Write( v );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed saving AnimationSet to stream: { e.Message }", false, LogType.Error );
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
		public bool LoadFromXml( XmlElement element )
		{
			if( element is null )
				return Logger.LogReturn( "Cannot load AnimationSet from a null XmlElement.", false, LogType.Error );

			RemoveAll();

			try
			{
				XmlNodeList anims = element.SelectNodes( nameof( Animation ) );

				foreach( XmlNode f in anims )
					if( !Add( f.InnerText.Trim() ) )
						return Logger.LogReturn( "Failed adding Animation ID to AnimationSet loaded from xml.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed loading AnimationSet: { e.Message }", false, LogType.Error );
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

			sb.Append( '<' ).Append( nameof( AnimationSet ) ).AppendLine( ">" );

			foreach( var a in m_anims )
			{
				sb.Append( "\t<" ).Append( nameof( Animation ) ).Append( '>' )
					.Append( a )
					.Append( "</" ).Append( nameof( Animation ) ).AppendLine( ">" );
			}

			sb.Append( "</" ).Append( nameof( AnimationSet ) ).Append( '>' );
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
		public bool Equals( AnimationSet other )
		{
			if( other is null || Count != other.Count )
				return false;

			for( int i = 0; i < Count; i++ )
				if( !m_anims[ i ].Equals( other.m_anims[ i ] ) )
					return false;

			return true;
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
			return Equals( obj as AnimationSet );
		}

		/// <summary>
		///   Serves as the default hash function.
		/// </summary>
		/// <returns>
		///   A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return m_anims.GetHashCode();
		}

		private readonly List<string> m_anims;
	}
}
