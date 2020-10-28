////////////////////////////////////////////////////////////////////////////////
// TextInfo.cs 
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

using SharpLogger;
using SharpSerial;

namespace SharpGfx
{
	/// <summary>
	///   Text style information.
	/// </summary>
	[Serializable]
	public class TextStyle : BinarySerializable, IEquatable<TextStyle>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextStyle()
		:	base()
		{
			FontPath     = string.Empty;
			Size         = 24;
			Style        = 0;
			Outline      = 0.0f;
			FillColor    = new Color( 255, 255, 255, 255 );
			OutlineColor = new Color( 255, 255, 255, 255 );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="i">
		///   The style to copy from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <paramref name="i"/> is null.
		/// </exception>
		public TextStyle( TextStyle i )
		:	this()
		{
			if( i == null )
				throw new ArgumentNullException();

			FontPath     = new string( i.FontPath.ToCharArray() );
			Size         = i.Size;
			Style        = i.Style;
			Outline      = i.Outline;
			FillColor    = i.FillColor;
			OutlineColor = i.OutlineColor;
		}
		/// <summary>
		///   Constructor assigning the font path and optionally other
		///   variables.
		/// </summary>
		/// <param name="font">
		///   Font path.
		/// </param>
		/// <param name="size">
		///   Character size.
		/// </param>
		/// <param name="style">
		///   Font style.
		/// </param>
		/// <param name="fill">
		///   Fill color.
		/// </param>
		/// <param name="line">
		///   Outline thickness.
		/// </param>
		/// <param name="outline">
		///   Outline color.
		/// </param>
		public TextStyle( string font, uint size = 24, uint style = 0, Color? fill = null, float line = 0.0f, Color? outline = null )
		:	base()
		{
			FontPath     = font ?? string.Empty;
			Size         = size == 0 ? 24 : size;
			Style        = style;
			Outline      = line < 0.0f ? 0.0f : line;
			FillColor    = fill ?? new Color( 255, 255, 255, 255 );
			OutlineColor = outline ?? new Color( 255, 255, 255, 255 );
		}

		/// <summary>
		///   If a valid font exists at <see cref="FontPath"/>.
		/// </summary>
		public bool IsFontValid
		{
			get
			{
				return Assets.Manager.Font.Get( FontPath ) != null;
			}
		}

		/// <summary>
		///   Font path.
		/// </summary>
		public string FontPath { get; set; }
		/// <summary>
		///   Character size.
		/// </summary>
		/// <remarks>
		///   Character size will only be set if the given value is greater
		///   than zero.
		/// </remarks>
		public uint Size
		{
			get { return m_size; }
			set
			{
				if( value > 0 )
					m_size = value;
			}
		}
		/// <summary>
		///   Text style.
		/// </summary>
		public uint Style { get; set; }
		/// <summary>
		///   Outline thickness.
		/// </summary>
		/// <remarks>
		///   Outline thickness will only be set if the given value is greater
		///   than or equal to zero.
		/// </remarks>
		public float Outline
		{
			get { return m_outline; }
			set
			{
				if( value >= 0.0f )
					m_outline = value;
			}
		}
		/// <summary>
		///   Text fill color.
		/// </summary>
		public Color FillColor { get; set; }
		/// <summary>
		///   Text outline color.
		/// </summary>
		public Color OutlineColor { get; set; }

		/// <summary>
		///   Sets both the fill and outline colors.
		/// </summary>
		/// <param name="col">
		///   The color to assign.
		/// </param>
		public void SetColor( Color col )
		{
			FillColor    = col;
			OutlineColor = col;
		}
		
		/// <summary>
		///   Applies the text style to the given text object if it is not null.
		/// </summary>
		/// <param name="t">
		///   The text to assign the style to.
		/// </param>
		public void Apply( ref Text t )
		{
			if( t == null )
				return;

			t.Font             = Assets.Manager.Font.Get( FontPath );
			t.CharacterSize    = Size;
			t.Style            = (Text.Styles)Style;
			t.OutlineThickness = Outline;
			t.FillColor        = FillColor;
			t.OutlineColor     = OutlineColor;
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="br">
		///   The stream reader.
		/// </param>
		/// <returns>
		///   True if the animator was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader br )
		{
			if( br == null )
				return false;

			try
			{
				FontPath     = br.ReadString();
				Size         = br.ReadUInt32();
				Style        = br.ReadUInt32();
				Outline      = br.ReadSingle();
				FillColor    = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
				OutlineColor = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
			}
			catch
			{
				return false;
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
		///   True if the animator was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter bw )
		{
			if( bw == null )
				return false;

			try
			{
				bw.Write( FontPath );
				bw.Write( Size );
				bw.Write( Style );
				bw.Write( Outline );
				bw.Write( FillColor.R );    bw.Write( FillColor.G );
				bw.Write( FillColor.B );    bw.Write( FillColor.A );
				bw.Write( OutlineColor.R ); bw.Write( OutlineColor.G );
				bw.Write( OutlineColor.B ); bw.Write( OutlineColor.A );
			}
			catch
			{
				return false;
			}

			return true;
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
		public bool Equals( TextStyle other )
		{
			return FontPath     == other.FontPath  &&
				   Size         == other.Size      &&
				   Style        == other.Style     &&
				   Outline      == other.Outline   &&
				   FillColor    == other.FillColor &&
				   OutlineColor == other.OutlineColor;
		}

		private uint  m_size;
		private float m_outline;
	}
}
