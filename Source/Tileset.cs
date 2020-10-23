////////////////////////////////////////////////////////////////////////////////
// Tileset.cs 
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

using SharpID;
using SharpLogger;
using SharpSerial;

namespace SharpGfx
{
	/// <summary>
	///   Contains tileset information.
	/// </summary>
	[Serializable]
	public class Tileset : BinarySerializable, IIdentifiable<string>
	{
		/// <summary>
		///   The default tileset cell size.
		/// </summary>
		public const uint DefaultCellSize = 64;

		/// <summary>
		///   Constructor.
		/// </summary>
		public Tileset()
		{
			ID       = Identifiable.NewStringID( "Tileset" );
			Texture  = string.Empty;
			CellSize = new Vector2u( DefaultCellSize, DefaultCellSize );
			Offset   = new Vector2u();
			Padding  = new Vector2u();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The object to copy.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If `t` is null.
		/// </exception>
		public Tileset( Tileset t )
		{
			if( t == null )
				throw new ArgumentNullException();

			ID       = t.ID + "_Copy";
			Texture  = new string( t.Texture.ToCharArray() );
			CellSize = t.CellSize;
			Offset   = t.Offset;
			Padding  = t.Padding;
		}
		/// <summary>
		///   Constructs the object with the given parameters.
		/// </summary>
		/// <param name="id">
		///   Tileset ID.
		/// </param>
		/// <param name="path">
		///   Texture path.
		/// </param>
		/// <param name="size">
		///   Cell size.
		/// </param>
		/// <param name="off">
		///   Cell offset.
		/// </param>
		/// <param name="pad">
		///   Cell padding.
		/// </param>
		/// <exception cref="ArgumentException">
		///   If a texture path is provided but loading the texture fails.
		/// </exception>
		public Tileset( string id, string path = null, Vector2u? size = null, Vector2u? off = null, Vector2u? pad = null )
		{
			ID = id;
			
			if( path != null )
			{
				if( !LoadTexture( path, size, off, pad ) )
					throw Logger.LogReturn( "Tileset construction failed: Unable to load texture from \"" + path + "\".", new ArgumentException(), LogType.Error );
			}
			else
			{
				Texture  = string.Empty;
				CellSize = size ?? new Vector2u( DefaultCellSize, DefaultCellSize );
				Offset   = off ?? new Vector2u();
				Padding  = pad ?? new Vector2u();
			}
		}

		/// <summary>
		///   If <see cref="Texture"/> is valid and leads to a valid texture.
		/// </summary>
		public bool TextureValid
		{
			get
			{
				return Assets.Manager.Texture.Get( Texture ) != null;
			}
		}

		/// <summary>
		///   The path to the tileset texture.
		/// </summary>
		public string Texture { get; set; }

		/// <summary>
		///   The size of each individual cell.
		/// </summary>
		public Vector2u CellSize
		{
			get { return m_cellsize; }
			set
			{
				if( value.X == 0 )
				{
					Logger.Log( "Tileset cell width must be greater than zero and has been adjusted", LogType.Warning );
					m_cellsize.X = 1;
				}
				else
				{
					m_cellsize.X = value.X;
				}

				if( value.Y == 0 )
				{
					Logger.Log( "Tileset cell height must be greater than zero and has been adjusted", LogType.Warning );
					m_cellsize.Y = 1;
				}
				else
				{
					m_cellsize.Y = value.Y;
				}
			}
		}
		/// <summary>
		///   The amount of cells that fit in the texture.
		/// </summary>
		public Vector2u Size
		{
			get
			{
				if( !TextureValid )
					return default( Vector2u );

				Vector2u count = new Vector2u();
				Vector2u texsize = Assets.Manager.Texture.Get( Texture ).Size;
				Vector2u size = Offset + CellSize + Padding;

				if( size.X > texsize.X )
					count = new Vector2u( 1, 0 );
				else
					count = new Vector2u( texsize.X / size.X, 0 );

				if( size.Y > texsize.Y )
					count = new Vector2u( count.X, 1 );
				else
					count = new Vector2u( count.X, texsize.Y / size.Y );

				return count;
			}
		}
		/// <summary>
		///   The total amount of cells that fit in the texture.
		/// </summary>
		public uint CellCount
		{
			get
			{
				Vector2u? size = Size;

				if( size.HasValue )
					return 0;

				return size.Value.X * size.Value.Y;
			}
		}

		/// <summary>
		///   Offset before each cell.
		/// </summary>
		public Vector2u Offset { get; set; }
		/// <summary>
		///   Padding after each cell.
		/// </summary>
		public Vector2u Padding { get; set; }

		/// <summary>
		///   Tileset ID.
		/// </summary>
		public string ID
		{
			get { return m_id; }
			set
			{
				m_id = Identifiable.AsValid( value );
			}
		}

		/// <summary>
		///   Loads the tileset texture from the given path.
		/// </summary>
		/// <param name="path">
		///   The path to the tileset texture.
		/// </param>
		/// <param name="size">
		///   Optional cell size (null for default).
		/// </param>
		/// <param name="off"></param>
		///   Optional cell offset (null for default).
		/// <param name="pad">
		///   Optional cell padding (null for default).
		/// </param>
		/// <returns>
		///   True if the texture was loaded successfully, otherwise false.
		/// </returns>
		public bool LoadTexture( string path, Vector2u? size = null, Vector2u? off = null, Vector2u? pad = null )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return false;

			Texture tex = Assets.Manager.Texture.Get( path );

			if( tex == null )
				return false;

			Texture = path;

			if( off != null )
				Offset = (Vector2u)off;
			if( pad != null )
				Padding = (Vector2u)pad;

			try
			{
				if( size != null )
					CellSize = (Vector2u)size;
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		///   Gets the texture rect of the cell at the given index.
		/// </summary>
		/// <param name="index">
		///   The cell index.
		/// </param>
		/// <returns>
		///   Returns the texture rect of the cell at the given index.
		/// </returns>
		public FloatRect GetCellRect( uint index )
		{
			uint ccount = CellCount;

			if( ccount == 0 || index >= ccount )
				throw new ArgumentOutOfRangeException();

			Vector2u size = Offset + CellSize + Padding;

			uint rows = Size.Y;

			uint col = index % rows;
			uint row = index / rows;

			return new FloatRect( ( col * size.X ) + Offset.X, 
			                      ( row * size.Y ) + Offset.Y,
								  CellSize.X, CellSize.Y );
		}

		/// <summary>
		///   Loads the object from a stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader.
		/// </param>
		/// <returns>
		///   True if the object was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return false;

			try
			{
				ID       = br.ReadString();
				Texture  = br.ReadString();
				CellSize = new Vector2u( br.ReadUInt32(), br.ReadUInt32() );
				Offset   = new Vector2u( br.ReadUInt32(), br.ReadUInt32() );
				Padding  = new Vector2u( br.ReadUInt32(), br.ReadUInt32() );
			}
			catch
			{
				return false;
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
			if( bw == null )
				return false;

			try
			{
				bw.Write( ID );
				bw.Write( Texture );
				bw.Write( CellSize.X ); bw.Write( CellSize.Y );
				bw.Write( Offset.X );   bw.Write( Offset.Y );
				bw.Write( Padding.X );  bw.Write( Padding.Y );
			}
			catch
			{
				return false;
			}

			return true;
		}

		private string   m_id;
		private Vector2u m_cellsize;
	}
}
