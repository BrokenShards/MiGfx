////////////////////////////////////////////////////////////////////////////////
// TextCaret.cs 
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
	///   A caret showing the current position in editable text.
	/// </summary>
	public class TextCaret : MiComponent, IEquatable<TextCaret>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public TextCaret()
		:	base()
		{
			m_curs    = new RectangleShape();
			m_timer   = new Clock();
			Thickness = 1.0f;
			BlinkRate = 1.0f;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="b">
		///   The object to copy.
		/// </param>
		public TextCaret( TextCaret b )
		:	base( b )
		{
			m_curs    = new RectangleShape( b.m_curs );
			m_timer   = new Clock();
			BlinkRate = b.BlinkRate;
		}
		/// <summary>
		///   Constructor setting the caret thickness and optional blink rate.
		/// </summary>
		/// <param name="thick">
		///   The caret thickness.
		/// </param>
		/// <param name="blink">
		///   The blink rate.
		/// </param>
		public TextCaret( float thick, float blink = 1.0f )
		:	base()
		{
			m_curs    = new RectangleShape();
			m_timer   = new Clock();
			Thickness = thick;
			BlinkRate = blink;
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( TextCaret ); }
		}

		/// <summary>
		///   The thickness of the caret.
		/// </summary>
		public float Thickness
		{
			get { return m_curs.OutlineThickness; }
			set { m_curs.OutlineThickness = value; }
		}
		/// <summary>
		///   The rate in seconds that the caret blinks (0 to not blink).
		/// </summary>
		public float BlinkRate
		{
			get { return m_blink; }
			set { m_blink = value < 0.0f ? 0.0f : value; }
		}

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( Transform ), nameof( Label ), nameof( TextListener ) };
		}

		/// <summary>
		///   Refreshes visual elements.
		/// </summary>
		public override void Refresh()
		{
			if( Parent == null )
				return;

			Label        lab = Parent.GetComponent<Label>();
			Transform    trn = Parent.GetComponent<Transform>();
			TextListener txt = Parent.GetComponent<TextListener>();

			float curheight;
			{
				Text text = new Text();
				lab.Text.Apply( ref text );
				text.Scale = trn.Scale;
				text.DisplayedString = "|";

				curheight = text.GetGlobalBounds().Height;
			}

			Vector2f curpos = lab.GetCharacterPosition( txt.CaretPosition );

			m_curs.Position = curpos;
			m_curs.Size     = new Vector2f( 0, curheight );
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Parent == null )
				return;

			Label lab = Parent.GetComponent<Label>();

			if( m_timer.ElapsedTime.AsSeconds() >= BlinkRate )
			{
				if( m_curs.OutlineColor.A == 0 )
					m_curs.OutlineColor = lab.Text.FillColor;
				else
					m_curs.OutlineColor = new Color( m_curs.OutlineColor.R, m_curs.OutlineColor.G, m_curs.OutlineColor.B, 0 );

				m_timer.Restart();
			}
		}
		/// <summary>
		///   Draws the sprite to the render target.
		/// </summary>
		/// <param name="target">
		///   The target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			if( Parent == null )
				return;

			m_curs.Draw( target, states );
		}
		/// <summary>
		///   Called when disposing of the object.
		/// </summary>
		protected override void OnDispose()
		{
			m_curs?.Dispose();
			m_timer?.Dispose();
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
				Thickness = sr.ReadSingle();
				BlinkRate = sr.ReadSingle();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading TextCaret: " + e.Message, false, LogType.Error );
			}

			m_timer.Restart();
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
				sw.Write( Thickness );
				sw.Write( BlinkRate );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving TextCaret: " + e.Message, false, LogType.Error );
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

			if( element.HasAttribute( nameof( Thickness ) ) )
			{
				if( !float.TryParse( element.GetAttribute( nameof( Thickness ) ), out float t ) )
					return Logger.LogReturn( "Failed parsing TextListeners' Thickness attribute.", false, LogType.Error );

				Thickness = t;
			}
			else
				Thickness = 1.0f;

			if( element.HasAttribute( nameof( BlinkRate ) ) )
			{
				if( !float.TryParse( element.GetAttribute( nameof( BlinkRate ) ), out float b ) )
					return Logger.LogReturn( "Failed parsing TextListeners' BlinkRate attribute.", false, LogType.Error );

				BlinkRate = b;
			}
			else
				BlinkRate = 1.0f;

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
			sb.Append( nameof( Thickness ) );
			sb.Append( "=\"" );
			sb.Append( Thickness );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( BlinkRate ) );
			sb.Append( "=\"" );
			sb.Append( BlinkRate );
			sb.AppendLine( "\"/>" );

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
		public bool Equals( TextCaret other )
		{
			return base.Equals( other ) && BlinkRate == other.BlinkRate;
		}
		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new TextCaret( this );
		}

		readonly RectangleShape m_curs;
		readonly Clock          m_timer;

		float m_blink;
	}
}
