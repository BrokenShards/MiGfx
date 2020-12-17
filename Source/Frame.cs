////////////////////////////////////////////////////////////////////////////////
// Frame.cs 
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
using System.Linq;
using System.Text;
using System.Xml;
using SFML.Graphics;
using SFML.System;
using SharpLogger;
using SharpSerial;

namespace SharpGfx
{
	/// <summary>
	///   A sprite-based animation frame.
	/// </summary>
	[Serializable]
	public class Frame : BinarySerializable, IXmlLoadable, IEquatable<Frame>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Frame()
		{
			Rect   = new FloatRect();
			Length = Time.Zero;
		}
		/// <summary>
		///   Constructor assigning the texture rect and frame length.
		/// </summary>
		/// <param name="rect">
		///   The texture rect to display on the frame.
		/// </param>
		/// <param name="len">
		///   The length of time the frame lasts; defaults to 1.0 if null.
		/// </param>
		public Frame( FloatRect rect, Time? len = null )
		{
			Rect   = rect;
			Length = len ?? Time.FromSeconds( 1.0f );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="f">
		///   The frame to copy from.
		/// </param>
		public Frame( Frame f )
		{
			if( f == null )
				throw new ArgumentNullException();

			Rect   = f.Rect;
			Length = f.Length;
		}

		/// <summary>
		///   The texture rect to display on the frame.
		/// </summary>
		public FloatRect Rect { get; set; }
		/// <summary>
		///   The length of time the frame is displayed.
		/// </summary>
		public Time Length { get; set; }

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
			if( br == null )
				return Logger.LogReturn( "Unable to load Frame from null stream.", false, LogType.Error );

			try
			{
				Rect   = new FloatRect( br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() );
				Length = Time.FromMicroseconds( br.ReadInt64() ); 
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load Frame from stream: " + e.Message, false, LogType.Error );
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
			if( bw == null )
				return Logger.LogReturn( "Unable to save Frame to null stream.", false, LogType.Error );

			try
			{
				bw.Write( Rect.Left ); bw.Write( Rect.Top ); bw.Write( Rect.Width ); bw.Write( Rect.Height );
				bw.Write( Length.AsMicroseconds() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save Frame to stream: " + e.Message, false, LogType.Error );
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
			if( element == null )
				return Logger.LogReturn( "Cannot load Frame from a null XmlElement.", false, LogType.Error );

			XmlElement rect = element[ nameof( Rect ) ];
			string l = element.GetAttribute( nameof( Length ) );

			if( rect == null )
				return Logger.LogReturn( "Failed loading Frame: No Rect element.", false, LogType.Error );
			if( string.IsNullOrWhiteSpace( l ) )
				return Logger.LogReturn( "Failed loading Frame: missing Length attribute.", false, LogType.Error );

			FloatRect? r = Xml.ToFRect( rect );

			if( !r.HasValue )
				return Logger.LogReturn( "Failed loading Frame: Unable to parse Rect element.", false, LogType.Error );

			Rect = r.Value;

			try
			{
				Length = Time.FromSeconds( float.Parse( l ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Frame: " + e.Message, false, LogType.Error );
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
			sb.Append( nameof( Frame ) );
			sb.Append( " " + nameof( Length ) + "=\"" );
			sb.Append( Length.AsSeconds() );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Rect, nameof( Rect ), 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( Frame ) );
			sb.AppendLine( ">" );

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
		public bool Equals( Frame other )
		{
			return other  != null &&
			       Rect   == other.Rect &&
			       Length == other.Length;
		}
	}
}
