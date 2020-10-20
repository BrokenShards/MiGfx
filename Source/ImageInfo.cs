////////////////////////////////////////////////////////////////////////////////
// ImageInfo.cs 
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

using SharpAsset;
using SharpLogger;
using SharpSerial;

namespace SharpGfx
{
	/// <summary>
	///   Image display information.
	/// </summary>
	[Serializable]
	public class ImageInfo : BinarySerializable
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public ImageInfo()
		{
			Path  = string.Empty;
			Rect  = new FloatRect();
			Color = new Color( 255, 255, 255, 255 );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="i">
		///   The image info to copy from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <paramref name="i"/> is null.
		/// </exception>
		public ImageInfo( ImageInfo i )
		{
			if( i == null )
				throw new ArgumentNullException();

			Path  = new string( i.Path.ToCharArray() );
			Rect  = i.Rect;
			Color = i.Color;
		}
		/// <summary>
		///   Constructor that assigns texture path along with optional rect 
		///   and color.
		/// </summary>
		/// <param name="path">
		///   Texture path.
		/// </param>
		/// <param name="rect">
		///   Texture display rect.
		/// </param>
		/// <param name="col">
		///   Texture color modifier.
		/// </param>
		public ImageInfo( string path, FloatRect? rect = null, Color? col = null )
		{
			Path  = path ?? string.Empty;
			Rect  = rect ?? new FloatRect();
			Color = col  ?? new Color( 255, 255, 255, 255 );
		}

		/// <summary>
		///   If a valid texture exists at <see cref="Path"/>.
		/// </summary>
		public bool IsTextureValid
		{
			get
			{
				return Assets.Manager.Texture.Get( Path ) != null;
			}
		}

		/// <summary>
		///   Texture path.
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		///   Texture display rect.
		/// </summary>
		public FloatRect Rect { get; set; }
		/// <summary>
		///   Texture color modifier.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		///   Sets the texture display rect to the full texture.
		/// </summary>
		public void SetFullRect()
		{
			Texture tex = Assets.Manager.Texture.Get( Path );

			if( tex == null )
				Rect = Logger.LogReturn( "SetFullRect failed because of invalid texture path; Rect has been reset.", new FloatRect(), LogType.Warning );
			else
				Rect = new FloatRect( 0.0f, 0.0f, tex.Size.X, tex.Size.Y );
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
				Path  = br.ReadString();
				Rect  = new FloatRect( br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() );
				Color = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
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
				bw.Write( Path );
				bw.Write( Rect.Left ); bw.Write( Rect.Top ); bw.Write( Rect.Width ); bw.Write( Rect.Height );
				bw.Write( Color.R ); bw.Write( Color.G ); bw.Write( Color.B ); bw.Write( Color.A );
			}
			catch
			{
				return false;
			}

			return true;
		}
	}
}
