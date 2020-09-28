////////////////////////////////////////////////////////////////////////////////
// AnimationSet.cs 
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
using System.Collections;
using System.Collections.Generic;

using SharpSerial;

namespace SharpGfx
{
	/// <summary>
	///   A set of string indexed animations.
	/// </summary>
	[Serializable]
	public class AnimationSet : BinarySerializable, IEnumerable<KeyValuePair<string, Animation>>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public AnimationSet()
		{
			m_animap = new Dictionary<string, Animation>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="a">
		///   The set to copy from.
		/// </param>
		public AnimationSet( AnimationSet a )
		{
			if( a == null )
				throw new ArgumentNullException();

			m_animap = new Dictionary<string, Animation>( a.Count );

			foreach( var v in m_animap )
				Add( new Animation( v.Value ) );
		}

		/// <summary>
		///   Animation accessor.
		/// </summary>
		/// <param name="id">
		///   The ID of the animation to access.
		/// </param>
		/// <returns>
		///   The animation with the given ID
		/// </returns>
		public Animation this[ string id ]
		{
			get { return Get( id ); }
			set { if( !Set( value ) ) throw new ArgumentException( "Unable to set animation." ); }
		}

		/// <summary>
		///   If the set contains no animations.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of animations the set contains.
		/// </summary>
		public int Count
		{
			get { return m_animap.Count; }
		}

		/// <summary>
		///   If the set contains an animation with the given ID.
		/// </summary>
		/// <param name="id">
		///   The animation ID.
		/// </param>
		/// <returns>
		///   True if the set contains an animation with the given ID and false
		///   otherwise.
		/// </returns>
		public bool Contains( string id )
		{
			if( string.IsNullOrWhiteSpace( id ) )
				return false;

			return m_animap.ContainsKey( id.ToLower() );
		}

		/// <summary>
		///   Gets the animation from the set with the given ID.
		/// </summary>
		/// <param name="id">
		///   The animation ID.
		/// </param>
		/// <returns>
		///   The animation from the set with the given ID or null if it does
		///   not exist.
		/// </returns>
		public Animation Get( string id )
		{
			if( !Contains( id ) )
				return null;

			return m_animap[ id.ToLower() ];
		}
		/// <summary>
		///   Replaces an existing animation with the given ID with a new one.
		/// </summary>
		/// <param name="anim">
		///   The new animation.
		/// </param>
		/// <returns>
		///   True if an animation with the given ID exists in the set and was
		///   replaced successfully.
		/// </returns>
		public bool Set( Animation anim )
		{
			if( anim == null || !Contains( anim.ID ) )
				return false;

			m_animap[ anim.ID.ToLower() ] = anim;
			return true;
		}
		/// <summary>
		///   Adds an animation to the set, optionally replacing an existing
		///   animation with the same ID.
		/// </summary>
		/// <param name="anim">
		///   The animation to add.
		/// </param>
		/// <param name="replace">
		///   If an animation already exists with the same ID, should it be
		///   replaced?
		/// </param>
		/// <returns>
		///   True if the animation was successfully added to the set and
		///   false otherwise.
		/// </returns>
		public bool Add( Animation anim, bool replace = false )
		{
			if( anim == null )
				return false;

			if( Contains( anim.ID ) )
			{
				if( !replace )
					return false;

				return Set( anim );
			}
			
			m_animap.Add( anim.ID.ToLower(), anim );
			return true;
		}
		/// <summary>
		///   Removes the animation with the given ID from the set.
		/// </summary>
		/// <param name="name">
		///   The name of the animation to remove.
		/// </param>
		/// <returns>
		///   True if an animation existed with the given ID and was removed
		///   successfully, otherwise false.
		/// </returns>
		public bool Remove( string name )
		{
			if( string.IsNullOrWhiteSpace( name ) )
				return false;

			return m_animap.Remove( name.ToLower() );
		}
		/// <summary>
		///   Removes all animations from the set.
		/// </summary>
		public void RemoveAll()
		{
			m_animap.Clear();
		}

		public IEnumerator<KeyValuePair<string, Animation>> GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, Animation>>)m_animap ).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<string, Animation>>)m_animap ).GetEnumerator();
		}

		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return false;

			try
			{
				uint len = br.ReadUInt32();

				for( int i = 0; i < len; i++ )
				{
					Animation anim = new Animation();

					if( !anim.LoadFromStream( br ) || !Add( anim ) )
						return false;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( bw == null )
				return false;

			RemoveAll();

			try
			{
				bw.Write( (uint)Count );

				foreach( var v in m_animap )
					if( !v.Value.SaveToStream( bw ) )
						return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		private Dictionary<string, Animation> m_animap;
	}
}
