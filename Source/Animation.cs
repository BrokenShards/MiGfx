////////////////////////////////////////////////////////////////////////////////
// Animation.cs 
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

using SFML.System;
using MiCore;

namespace MiGfx
{
	/// <summary>
	///   A sprite-based animation.
	/// </summary>
	public class Animation : BinarySerializable, IXmlLoadable, IIdentifiable<string>, IEquatable<Animation>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Animation()
		{
			ID = Identifiable.NewStringID( nameof( Animation ) );
			m_frames = new List<Frame>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="a">
		///   The animation to copy from.
		/// </param>
		public Animation( Animation a )
		{
			if( a is null )
				throw new ArgumentNullException( nameof( a ) );

			ID = $"{ a.ID }_Copy";
			m_frames = new List<Frame>( a.m_frames.Count );

			foreach( Frame f in a.m_frames )
				Add( new Frame( f ) );
		}
		/// <summary>
		///   Constructor that assigns the ID.
		/// </summary>
		/// <param name="id">
		///   The animation ID.
		/// </param>
		/// <exception cref="ArgumentException">
		///   If id is not a valid ID <see cref="ID"/>.
		/// </exception>
		public Animation( string id )
		{
			try
			{
				ID = id;
			}
			catch
			{
				throw;
			}

			m_frames = new List<Frame>();
		}
		/// <summary>
		///   Constructs the animation with the given frame(s).
		/// </summary>
		/// <param name="fs">
		///   The list of frames to construct the animation with.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If either the list of frames, or an individual frame is null.
		/// </exception>
		public Animation( params Frame[] fs )
		:	this()
		{
			try
			{
				Add( fs );
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		///   Constructs the animation with the given ID and frames.
		/// </summary>
		/// <param name="id">
		///   The animation ID.
		/// </param>
		/// <param name="fs">
		///   The list of frames to construct the animation with.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If either the list of frames, or an individual frame is null.
		/// </exception>
		public Animation( string id, params Frame[] fs )
		{
			ID = id;
			m_frames = new List<Frame>();

			try
			{
				Add( fs );
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		///   ID accessor.
		/// </summary>
		/// <exception cref="ArgumentException">
		///   If attempting to set to an invalid ID. <see cref="Identifiable.IsValid(string)"/>.
		/// </exception>
		public string ID 
		{
			get { return m_id; }
			set
			{
				if( !Identifiable.IsValid( value ) )
					throw new ArgumentException( "Trying to set invalid Animation ID." );

				m_id = value;
			}
		}

		/// <summary>
		///   Frame accessor.
		/// </summary>
		/// <param name="index">
		///   The index of the desired frame.
		/// </param>
		/// <returns>
		///   The frame at the given index.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   See <see cref="Get(uint)"/> and <see cref="Set(uint, Frame)"/>.
		/// </exception>
		public Frame this[ uint index ]
		{
			get { try { return Get( index ); } catch { throw; } }
			set { try { Set( index, value ); } catch { throw; } }
		}

		/// <summary>
		///   If the animation contains no frames.
		/// </summary>
		public bool Empty
		{
			get { return Count is 0; }
		}
		/// <summary>
		///   The amount of frames in the animation.
		/// </summary>
		public int Count
		{
			get { return m_frames.Count; }
		}
		/// <summary>
		///   The total length of the animation.
		/// </summary>
		public Time Length
		{
			get
			{
				Time time = Time.Zero;

				for( int i = 0; i < Count; i++ )
					time += m_frames[ i ].Length;

				return time;
			}
		}

		/// <summary>
		///   Gets the frame at the given index.
		/// </summary>
		/// <param name="index">
		///   The index of the desired frame.
		/// </param>
		/// <returns>
		///   The frame at the given index.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   If the given index is out of range.
		/// </exception>
		public Frame Get( uint index )
		{
			if( index < 0 || index >= Count )
				throw new ArgumentOutOfRangeException( nameof( index ) );

			return m_frames[ (int)index ];
		}
		/// <summary>
		///   Replaces an existing frame with a new one at the given index.
		/// </summary>
		/// <param name="index">
		///   The desired frame index.
		/// </param>
		/// <param name="f">
		///   The new frame.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		///   If the given index is out of range.
		/// </exception>
		public void Set( uint index, Frame f )
		{
			if( index < 0 || index >= Count )
				throw new ArgumentOutOfRangeException( nameof( index ) );

			m_frames[ (int)index ] = f;
		}

		/// <summary>
		///   Adds a frame to the end of the animation.
		/// </summary>
		/// <param name="f">
		///   The frame to add.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If the frame to add is null.
		/// </exception>
		public void Add( Frame f )
		{
			if( f is null )
				throw new ArgumentNullException( nameof( f ) );

			m_frames.Add( f );
		}
		/// <summary>
		///   Adds multiple frames to the end of the animation.
		/// </summary>
		/// <param name="fs">
		///   List of frames to add.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If either the list of frames, or an individual frame is null.
		/// </exception>
		public void Add( params Frame[] fs )
		{
			if( fs is null )
				throw new ArgumentNullException( nameof( fs ) );

			try
			{
				foreach( Frame f in fs )
					Add( f );
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		///   Removes the frame at the given index.
		/// </summary>
		/// <param name="index">
		///   The index of the frame to remove.
		/// </param>
		public void Remove( uint index )
		{
			if( index >= 0 && index < Count )
				m_frames.RemoveAt( (int)index );
		}
		/// <summary>
		///   Removes all frames from the animation.
		/// </summary>
		public void RemoveAll()
		{
			m_frames.Clear();
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
				return Logger.LogReturn( "Cannot load Animation from null stream.", false, LogType.Error );

			try
			{
				ID = br.ReadString();
				uint count = br.ReadUInt32();

				m_frames = count is 0 ? new List<Frame>() : new List<Frame>( (int)count );

				for( int i = 0; i < count; i++ )
				{
					Frame f = new();

					if( !f.LoadFromStream( br ) )
						return Logger.LogReturn( "Failed loading Animation's Frame from stream.", false, LogType.Error );

					m_frames.Add( f );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed loading Animation from stream: { e.Message }", false, LogType.Error );
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
				return Logger.LogReturn( "Cannot save Animation to null stream.", false, LogType.Error );

			try
			{
				bw.Write( ID );
				bw.Write( Count );

				for( int i = 0; i < Count; i++ )
					if( !m_frames[ i ].SaveToStream( bw ) )
						return Logger.LogReturn( "Failed saving Animation's Frame to stream.", false, LogType.Error );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed saving Animation to stream: { e.Message }", false, LogType.Error );
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
				return Logger.LogReturn( "Cannot load Animation from a null XmlElement.", false, LogType.Error );

			RemoveAll();

			try
			{
				string id = element.GetAttribute( nameof( ID ) );
				ID = string.IsNullOrWhiteSpace( id ) ? Identifiable.NewStringID( nameof( Animation ) ) : id;

				XmlNodeList frames = element.SelectNodes( nameof( Frame ) );

				foreach( XmlNode f in frames )
				{
					Frame frame = new();

					if( !frame.LoadFromXml( (XmlElement)f ) )
						return Logger.LogReturn( "Failed loading Animation's Frame from xml.", false, LogType.Error );

					Add( frame );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( $"Failed loading Animation: { e.Message }", false, LogType.Error );
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

			sb.Append( '<' ).Append( nameof( Animation ) ).Append( ' ' )
				.Append( nameof( ID ) ).Append( "=\"" ).Append( ID ).AppendLine( "\">" );

			for( int i = 0; i < Count; i++ )
				sb.AppendLine( XmlLoadable.ToString( m_frames[ i ], 1 ) );

			sb.Append( "</" ).Append( nameof( Animation ) ).Append( '>' );

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
		public bool Equals( Animation other )
		{
			if( other is null || ID != other.ID || Count != other.Count )
				return false;

			for( int i = 0; i < Count; i++ )
				if( !m_frames[ i ].Equals( other.m_frames[ i ] ) )
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
			return Equals( obj as Animation );
		}

		/// <summary>
		///   Serves as the default hash function.
		/// </summary>
		/// <returns>
		///   A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return HashCode.Combine( m_id, m_frames );
		}

		string m_id;
		List<Frame> m_frames;
	}

}
