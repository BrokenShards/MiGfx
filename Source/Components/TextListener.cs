////////////////////////////////////////////////////////////////////////////////
// Typeable.cs 
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
using System.Text;

using SFML.Window;
using MiCore;
using System.IO;
using System.Xml;

namespace MiGfx
{
	/// <summary>
	///   A component that listens for text input events.
	/// </summary>
	public class TextListener : MiComponent
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextListener()
		:	base()
		{
			AllowLetters     = true;
			AllowNumbers     = true;
			AllowSymbols     = true;
			AllowPunctuation = true;
			AllowSpace       = true;
			AllowNewline     = false;
			Listen           = true;
			EnteredText      = string.Empty;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The object to copy.
		/// </param>
		public TextListener( TextListener t )
		:	base( t )
		{
			AllowLetters     = t.AllowLetters;
			AllowNumbers     = t.AllowNumbers;
			AllowSymbols     = t.AllowSymbols;
			AllowPunctuation = t.AllowPunctuation;
			AllowSpace       = t.AllowSpace;
			AllowNewline     = t.AllowNewline;
			Listen           = t.Listen;
			EnteredText      = new string( t.EnteredText.ToCharArray() );
		}
		/// <summary>
		///   Constructor setting initial entered text.
		/// </summary>
		/// <param name="text">
		///   Initial entered text.
		/// </param>
		public TextListener( string text )
		:	base()
		{
			AllowLetters     = true;
			AllowNumbers     = true;
			AllowSymbols     = true;
			AllowPunctuation = true;
			AllowSpace       = true;
			AllowNewline     = text.Contains( "\n" );
			Listen           = true;
			EnteredText      = text ?? string.Empty;
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( TextListener ); }
		}

		/// <summary>
		///   Entered text.
		/// </summary>
		public string EnteredText
		{
			get { return m_text; }
			set
			{
				if( value == null )
					m_text = string.Empty;
				else
					m_text = value.Contains( "\n" ) && !AllowNewline ?
					         value.Replace( "\r\n", " " ).Replace( "\n", " " ) :
							 value;
			}
		}
		/// <summary>
		///   If the component should listen for text entered events.
		/// </summary>
		public bool Listen
		{
			get; set;
		}

		/// <summary>
		///   Maximum size of entered text.
		/// </summary>
		public uint MaxCharacters
		{
			get { return m_max; }
			set
			{
				m_max = value;

				if( m_max > 0 && EnteredText.Length > m_max )
					EnteredText = EnteredText.Substring( 0, (int)m_max );
			}
		}

		/// <summary>
		///   If letters should be added to the entered text.
		/// </summary>
		public bool AllowLetters { get; set; }
		/// <summary>
		///   If numbers should be added to the entered text (allows one '.').
		/// </summary>
		public bool AllowNumbers { get; set; }
		/// <summary>
		///   If symbols should be added to the entered text.
		/// </summary>
		public bool AllowSymbols { get; set; }
		/// <summary>
		///   If punctuation should be added to the entered text.
		/// </summary>
		public bool AllowPunctuation { get; set; }
		/// <summary>
		///   If the space character should be added to the entered text.
		/// </summary>
		public bool AllowSpace { get; set; }
		/// <summary>
		///   If entered text can contain newlines.
		/// </summary>
		public bool AllowNewline
		{
			get { return m_multi; }
			set
			{
				m_multi = value;

				if( EnteredText != null && EnteredText.Contains( "\n" ) && !AllowNewline )
					EnteredText = EnteredText.Replace( "\r\n", " " ).Replace( "\n", " " );
			}
		}
		
		/// <summary>
		///   Text entered event.
		/// </summary>
		/// <param name="e">
		///   Event arguments.
		/// </param>
		protected override void OnTextEntered( TextEventArgs e )
		{
			if( !Listen )
				return;

			int len = EnteredText.Length;

			Logger.Log( "Entered: " + EnteredText );

			// Backspace
			if( e.Unicode == "\u0008" || e.Unicode == "\u0232" )
			{
				if( len > 0 )
					EnteredText = EnteredText.Substring( 0, len - 1 );

				return;
			}

			if( MaxCharacters == 0 || len < MaxCharacters )
			{
				// Carriage Return, Newline, or both (all treated as newline)
				if( e.Unicode == "\r\n" || e.Unicode == "\u000A" || e.Unicode == "\u000D" )
				{
					if( len > 0 && AllowNewline )
						EnteredText += "\r\n";
				}
				else if( e.Unicode.Trim().Length > 0 || ( len > 0 && e.Unicode == " " ) )
				{
					if( char.IsLetter( e.Unicode, 0 ) && !AllowLetters )
						return;
					if( e.Unicode[ 0 ] >= '0' && e.Unicode[ 0 ] <= '9' && !AllowNumbers )
						return;
					if( e.Unicode == "." && AllowNumbers && !AllowPunctuation && EnteredText.Contains( "." ) )
						return;

					if( char.IsSymbol( e.Unicode, 0 ) && !AllowSymbols )
						return;
					if( char.IsPunctuation( e.Unicode, 0 ) && !AllowPunctuation )
						return;
					if( e.Unicode == " " && !AllowSpace )
						return;

					EnteredText += e.Unicode;
				}
			}
		}
		/// <summary>
		///   Attempts to deserialize the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   Stream reader.
		/// </param>
		/// <returns>
		///   True if deserialization succeeded and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader sr )
		{
			if( !base.LoadFromStream( sr ) )
				return false;

			try
			{
				EnteredText      = sr.ReadString();
				MaxCharacters    = sr.ReadUInt32();
				AllowLetters     = sr.ReadBoolean();
				AllowNumbers     = sr.ReadBoolean();
				AllowSymbols     = sr.ReadBoolean();
				AllowPunctuation = sr.ReadBoolean();
				AllowSpace       = sr.ReadBoolean();
				AllowNewline     = sr.ReadBoolean();
				Listen           = sr.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextListener: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Attempts to serialize the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   Stream writer.
		/// </param>
		/// <returns>
		///   True if serialization succeeded and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter sw )
		{
			if( !base.SaveToStream( sw ) )
				return false;

			try
			{
				sw.Write( EnteredText );
				sw.Write( MaxCharacters );
				sw.Write( AllowLetters );
				sw.Write( AllowNumbers );
				sw.Write( AllowSymbols );
				sw.Write( AllowPunctuation );
				sw.Write( AllowSpace );
				sw.Write( AllowNewline );
				sw.Write( Listen );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving TextListener: " + e.Message, false, LogType.Error );
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

			EnteredText      = string.Empty;
			MaxCharacters    = 0;
			AllowLetters     = true;
			AllowNumbers     = true;
			AllowSymbols     = true;
			AllowPunctuation = true;
			AllowSpace       = true;
			AllowNewline     = false;
			Listen           = true;

			XmlNode txt = element[ nameof( EnteredText ) ];

			if( txt != null )
				EnteredText = txt.Value;

			try
			{
				if( element.HasAttribute( nameof( MaxCharacters ) ) )
					MaxCharacters = uint.Parse( element.GetAttribute( nameof( MaxCharacters ) ) );
				if( element.HasAttribute( nameof( AllowLetters ) ) )
					AllowLetters = bool.Parse( element.GetAttribute( nameof( AllowLetters ) ) );
				if( element.HasAttribute( nameof( AllowNumbers ) ) )
					AllowNumbers = bool.Parse( element.GetAttribute( nameof( AllowNumbers ) ) );
				if( element.HasAttribute( nameof( AllowSymbols ) ) )
					AllowSymbols = bool.Parse( element.GetAttribute( nameof( AllowSymbols ) ) );
				if( element.HasAttribute( nameof( AllowPunctuation ) ) )
					AllowPunctuation = bool.Parse( element.GetAttribute( nameof( AllowPunctuation ) ) );
				if( element.HasAttribute( nameof( AllowSpace ) ) )
					AllowSpace = bool.Parse( element.GetAttribute( nameof( AllowSpace ) ) );
				if( element.HasAttribute( nameof( AllowNewline ) ) )
					AllowNewline = bool.Parse( element.GetAttribute( nameof( AllowNewline ) ) );
				if( element.HasAttribute( nameof( Listen ) ) )
					Listen = bool.Parse( element.GetAttribute( nameof( Listen ) ) );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextListener: " + e.Message, false, LogType.Error );
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

			sb.Append( "              " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( MaxCharacters ) );
			sb.Append( "=\"" );
			sb.Append( MaxCharacters );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( AllowLetters ) );
			sb.Append( "=\"" );
			sb.Append( AllowLetters );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( AllowNumbers ) );
			sb.Append( "=\"" );
			sb.Append( AllowNumbers );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( AllowSymbols ) );
			sb.Append( "=\"" );
			sb.Append( AllowSymbols );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( AllowPunctuation ) );
			sb.Append( "=\"" );
			sb.Append( AllowPunctuation );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( AllowSpace ) );
			sb.Append( "=\"" );
			sb.Append( AllowSpace );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( AllowNewline ) );
			sb.Append( "=\"" );
			sb.Append( AllowNewline );
			sb.AppendLine( "\"" );

			sb.Append( "              " );
			sb.Append( nameof( Listen ) );
			sb.Append( "=\"" );
			sb.Append( Listen );
			sb.AppendLine( "\">" );

			sb.Append( "\t<" );
			sb.Append( nameof( EnteredText ) );
			sb.Append( ">" );
			sb.Append( EnteredText );
			sb.Append( "</" );
			sb.Append( nameof( EnteredText ) );
			sb.Append( ">" );

			sb.Append( "</" );
			sb.Append( TypeName );
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
		public bool Equals( TextListener other )
		{
			return base.Equals( other ) && EnteredText.Equals( other.EnteredText ) &&
				   MaxCharacters    == other.MaxCharacters &&
			       AllowLetters     == other.AllowLetters &&
				   AllowNumbers     == other.AllowNumbers &&
				   AllowSymbols     == other.AllowSymbols &&
				   AllowPunctuation == other.AllowPunctuation &&
				   AllowSpace       == other.AllowSpace &&
				   AllowNewline     == other.AllowNewline && 
				   Listen           == other.Listen;
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new TextListener( this );
		}

		string m_text;
		bool   m_multi;
		uint   m_max;
	}
}
