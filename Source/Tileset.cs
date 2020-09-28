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
using SharpSerial;

namespace SharpGfx
{
	[Serializable]
	public class Tileset : BinarySerializable, IIdentifiable<string>
	{
		public const uint DefaultCellSize = 64;

		public Tileset()
		{
			ID       = Identifiable.NewStringID( "Tileset" );
			Texture  = string.Empty;
			CellSize = new Vector2u( DefaultCellSize, DefaultCellSize );
			Offset   = new Vector2u();
			Padding  = new Vector2u();
		}
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
		public Tileset( string id, string path = null, Vector2u? size = null, Vector2u? off = null, Vector2u? pad = null )
		{
			ID = id;
			
			if( path != null )
			{
				if( !LoadTexture( path, size, off, pad ) )
					throw new ArgumentException();
			}
			else
			{
				Texture  = string.Empty;
				CellSize = size ?? new Vector2u( DefaultCellSize, DefaultCellSize );
				Offset   = off ?? new Vector2u();
				Padding  = pad ?? new Vector2u();
			}
		}

		public bool TextureValid
		{
			get
			{
				Texture tex = null;

				try
				{
					tex = new Texture( Texture );
				}
				catch
				{
					tex = null;
				}

				bool result = tex != null;
				tex?.Dispose();

				return result;
			}
		}

		public string Texture { get; set; }
		public Vector2u CellSize
		{
			get { return m_cellsize; }
			set
			{
				if( value.X == 0 || value.Y == 0 )
					throw new ArgumentException( "Tilesets' cell size must be greater than zero." );

				m_cellsize = value;
			}
		}
		public Vector2u? Size
		{
			get
			{
				if( !TextureValid )
					return null;

				Vector2u count = new Vector2u();
				Vector2u texsize = new Vector2u();
				Vector2u size = Offset + CellSize + Padding;

				using( Texture tex = new Texture( Texture ) )
					texsize = tex.Size;

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
		public Vector2u Offset { get; set; }
		public Vector2u Padding { get; set; }

		public string ID
		{
			get { return m_id; }
			set
			{
				m_id = Identifiable.AsValid( value );
			}
		}

		public bool LoadTexture( string path, Vector2u? size = null, Vector2u? off = null, Vector2u? pad = null )
		{
			if( string.IsNullOrWhiteSpace( path ) )
				return false;

			Texture tex = null;

			try
			{
				tex = new Texture( path );
			}
			catch
			{
				tex = null;
			}

			if( tex == null )
				return false;

			tex.Dispose();
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
		public FloatRect GetTileRect( uint index )
		{
			uint ccount = CellCount;

			if( ccount == 0 || index >= ccount )
				throw new ArgumentOutOfRangeException();

			Vector2u size = Offset + CellSize + Padding;

			uint rows = Size.Value.Y;

			uint col = index % rows;
			uint row = index / rows;

			return new FloatRect( ( col * size.X ) + Offset.X, 
			                      ( row * size.Y ) + Offset.Y,
								  CellSize.X, CellSize.Y );
		}

		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return false;

			try
			{
				ID     = br.ReadString();
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
