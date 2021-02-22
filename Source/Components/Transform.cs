﻿////////////////////////////////////////////////////////////////////////////////
// Transform.cs 
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

namespace MiGfx
{
	/// <summary>
	///   Interface for objects with a transform.
	/// </summary>
	public interface ITransformable
	{
		/// <summary>
		///   The transform.
		/// </summary>
		Transform Transform { get; }
	}

	/// <summary>
	///   A 2D transformation.
	/// </summary>
	[Serializable]
	public class Transform : MiComponent, IEquatable<Transform>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Transform()
		:	base()
		{
			LockPosition = false;
			LockSize     = false;
			LockScale    = false;

			Position = new Vector2f();
			Size     = new Vector2f( 1.0f, 1.0f );
			Scale    = new Vector2f( 1.0f, 1.0f );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The transform to copy from.
		/// </param>
		public Transform( Transform t )
		:	base( t )
		{
			LockPosition = false;
			LockSize     = false;
			LockScale    = false;

			Position = t.Position;
			Size     = t.Size;
			Scale    = t.Scale;

			LockPosition = t.LockPosition;
			LockSize     = t.LockSize;
			LockScale    = t.LockScale;
		}
		/// <summary>
		///   Constructor assigning the position and optionally the size and
		///   scale.
		/// </summary>
		/// <param name="pos">
		///   Transform position.
		/// </param>
		/// <param name="size">
		///   Transform size.
		/// </param>
		/// <param name="scale">
		///   Transform scale.
		/// </param>
		public Transform( Vector2f pos, Vector2f? size = null, Vector2f? scale = null )
		:	base()
		{
			LockPosition = false;
			LockSize     = false;
			LockScale    = false;

			Position = pos;
			Size     = size  ?? new Vector2f( 1.0f, 1.0f );
			Scale    = scale ?? new Vector2f( 1.0f, 1.0f );
		}

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Transform ); }
		}

		/// <summary>
		///   Position.
		/// </summary>
		public Vector2f Position 
		{
			get { return m_pos; }
			set
			{
				if( !LockPosition )
					m_pos = value;
			}
		}
		/// <summary>
		///   Local size.
		/// </summary>
		public Vector2f Size
		{
			get { return m_size; }
			set
			{
				if( !LockSize )
				{
					if( value.X <= 0.0f )
					{
						float x = Math.Abs( value.X );
						value.X = x > 0.0f ? x : 1.0f;
					}
					if( value.Y <= 0.0f )
					{
						float y = Math.Abs( value.Y );
						value.Y = y > 0.0f ? y : 1.0f;
					}

					m_size = value;
				}
			}
		}
		/// <summary>
		///   Scale.
		/// </summary>
		public Vector2f Scale
		{
			get { return m_scale; }
			set
			{
				if( !LockScale )
				{
					if( value.X <= 0.0f || value.Y <= 0.0f )
						m_scale = new Vector2f( 1.0f, 1.0f );
					else
						m_scale = value;
				}
			}
		}

		/// <summary>
		///   Prevents position from being modified if true.
		/// </summary>
		public bool LockPosition { get; set; }
		/// <summary>
		///   Prevents size from being modified if true.
		/// </summary>
		public bool LockSize { get; set; }
		/// <summary>
		///   Prevents scale from being modified if true.
		/// </summary>
		public bool LockScale { get; set; }

		/// <summary>
		///   Center position.
		/// </summary>
		public Vector2f Center
		{
			get { return Position + ( GlobalSize / 2.0f ); }
			set { Position = value - ( GlobalSize / 2.0f ); }
		}
		/// <summary>
		///   Scaled size.
		/// </summary>
		public Vector2f GlobalSize
		{
			get { return new Vector2f( m_size.X * m_scale.X, m_size.Y * m_scale.Y ); }
		}

		/// <summary>
		///   Scaled bounds of the transform.
		/// </summary>
		public FloatRect GlobalBounds
		{
			get { return new FloatRect( Position, GlobalSize ); }
			set
			{
				try
				{
					Scale    = new Vector2f( 1.0f, 1.0f );
					Position = new Vector2f( value.Left,  value.Top );
					Size     = new Vector2f( value.Width, value.Height );
				}
				catch
				{
					throw;
				}
			}
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
			if( !base.LoadFromStream( br ) )
				return false;

			LockPosition = false;
			LockSize     = false;
			LockScale    = false;

			try
			{
				Position     = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Size         = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Scale        = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				LockPosition = br.ReadBoolean();
				LockSize     = br.ReadBoolean();
				LockScale    = br.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Transform: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Writes the object to a stream.
		/// </summary>
		/// <param name="bw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the object was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( !base.SaveToStream( bw ) )
				return false;

			try
			{
				bw.Write( Position.X ); bw.Write( Position.Y );
				bw.Write( Size.X );     bw.Write( Size.Y );
				bw.Write( Scale.X );    bw.Write( Scale.Y );
				bw.Write( LockPosition );
				bw.Write( LockSize );
				bw.Write( LockScale );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Transform: " + e.Message, false, LogType.Error );
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

			LockPosition = false;
			LockSize     = false;
			LockScale    = false;

			XmlElement position = element[ nameof( Position ) ],
			           size     = element[ nameof( Size ) ],
			           scale    = element[ nameof( Scale ) ];

			if( position == null )
				return Logger.LogReturn( "Failed loading Transform: No Position element.", false, LogType.Error );
			if( size == null )
				return Logger.LogReturn( "Failed loading Transform: No Size element.", false, LogType.Error );

			Vector2f? pos = Xml.ToVec2f( position ),
					  siz = Xml.ToVec2f( size ),
					  scl = scale != null ? Xml.ToVec2f( scale ) : null;

			if( !pos.HasValue )
				return Logger.LogReturn( "Failed loading Transform: Unable to parse position.", false, LogType.Error );
			if( !siz.HasValue )
				return Logger.LogReturn( "Failed loading Transform: Unable to parse size.", false, LogType.Error );
			if( scale != null && !scl.HasValue )
				return Logger.LogReturn( "Failed loading Transform: Unable to parse scale.", false, LogType.Error );
			else if( scale == null )
				scl = new Vector2f( 1.0f, 1.0f );

			Position = pos.Value;
			Size     = siz.Value;
			Scale    = scl.Value;

			if( element.HasAttribute( nameof( LockPosition ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( LockPosition ) ), out bool lp ) )
					return Logger.LogReturn( "Failed loading Transform: Unable to parse position lock.", false, LogType.Error );

				LockPosition = lp;
			}
			if( element.HasAttribute( nameof( LockSize ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( LockSize ) ), out bool ls ) )
					return Logger.LogReturn( "Failed loading Transform: Unable to parse size lock.", false, LogType.Error );

				LockSize = ls;
			}
			if( element.HasAttribute( nameof( LockScale ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( LockScale ) ), out bool ls ) )
					return Logger.LogReturn( "Failed loading Transform: Unable to parse scale lock.", false, LogType.Error );

				LockScale = ls;
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
			StringBuilder sb = new StringBuilder();

			sb.Append( "<" ); 
			sb.Append( TypeName );

			sb.Append( " " );
			sb.Append( nameof( Enabled ) );
			sb.Append( "=\"" );
			sb.Append( Enabled ); 
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( LockPosition ) );
			sb.Append( "=\"" );
			sb.Append( LockPosition );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( LockSize ) );
			sb.Append( "=\"" );
			sb.Append( LockSize );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( LockScale ) );
			sb.Append( "=\"" );
			sb.Append( LockScale );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Position, nameof( Position ), 1 ) );
			sb.AppendLine( Xml.ToString( Size,     nameof( Size ), 1 ) );

			if( Scale.X != 1.0f || Scale.Y != 1.0f )
				sb.AppendLine( Xml.ToString( Scale, nameof( Scale ), 1 ) );

			sb.Append( "</" ); sb.Append( TypeName ); sb.AppendLine( ">" );

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
		public bool Equals( Transform other )
		{
			return Position  == other.Position  &&
				   Size == other.Size &&
				   Scale     == other.Scale;
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new Transform( this );
		}

		private Vector2f m_pos, m_size, m_scale;
	}
}