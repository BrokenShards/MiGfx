﻿////////////////////////////////////////////////////////////////////////////////
// Transform.cs 
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
using SFML.System;

using SharpLogger;
using SharpSerial;

namespace SharpGfx
{
	/// <summary>
	///   Interface for objects with a transform.
	/// </summary>
	public interface ITransformable
	{
		Transform Transform { get; }
	}

	/// <summary>
	///   A 2D transformation.
	/// </summary>
	[Serializable]
	public class Transform : BinarySerializable
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Transform()
		{
			Position  = new Vector2f();
			LocalSize = new Vector2f( 1.0f, 1.0f );
			Scale     = new Vector2f( 1.0f, 1.0f );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The transform to copy from.
		/// </param>
		public Transform( Transform t )
		{
			if( t == null )
				throw new ArgumentNullException();

			Position  = t.Position;
			LocalSize = t.LocalSize;
			Scale     = t.Scale;
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
		{
			Position  = pos;
			LocalSize = size  ?? new Vector2f( 1.0f, 1.0f );
			Scale     = scale ?? new Vector2f( 1.0f, 1.0f );
		}

		/// <summary>
		///   Position.
		/// </summary>
		public Vector2f Position { get; set; }
		/// <summary>
		///   Size without scaling.
		/// </summary>
		public Vector2f LocalSize
		{
			get { return m_size; }
			set
			{
				if( value.X <= 0.0f )
				{
					Console.WriteLine( "Transforms' width must be greater than zero and has been adjusted." );
					Console.ReadLine();
					value.X = 1.0f;
				}
				if( value.Y <= 0.0f )
				{
					Console.WriteLine( "Transforms' height must be greater than zero and has been adjusted." );
					Console.ReadLine();
					value.Y = 1.0f;
				}

				m_size = value;
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
				if( value.X <= 0.0f || value.Y <= 0.0f )
				{
					Console.WriteLine( "Transforms' scale must be greater than zero, it has been reset." );
					Console.ReadLine();
					m_scale = new Vector2f( 1.0f, 1.0f );
				}
				else
					m_scale = value;
			}
		}

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
					Scale     = new Vector2f( 1.0f, 1.0f );
					Position  = new Vector2f( value.Left,  value.Top );
					LocalSize = new Vector2f( value.Width, value.Height );
				}
				catch
				{
					throw;
				}
			}
		}

		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return false;

			try
			{
				Position  = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				LocalSize = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Scale     = new Vector2f( br.ReadSingle(), br.ReadSingle() );
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

			try
			{
				bw.Write( Position.X );  bw.Write( Position.Y );
				bw.Write( LocalSize.X ); bw.Write( LocalSize.Y );
				bw.Write( Scale.X );     bw.Write( Scale.Y );
			}
			catch
			{
				return false;
			}

			return true;
		}

		private Vector2f m_size, m_scale;
	}
}