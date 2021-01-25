////////////////////////////////////////////////////////////////////////////////
// Label.cs 
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
	public static partial class FilePaths
	{
		/// <summary>
		///   Default font path.
		/// </summary>
		public static string DefaultFont = FolderPaths.Fonts + "FallingSky.otf";
	}

	/// <summary>
	///   Possible transform allignments.
	/// </summary>
	public enum Allignment
	{
		/// <summary>
		///   Allign to the top left of the transform.
		/// </summary>
		TopLeft,
		/// <summary>
		///   Allign to the top middle of the transform.
		/// </summary>
		Top,
		/// <summary>
		///   Allign to the top right of the transform.
		/// </summary>
		TopRight,
		/// <summary>
		///   Allign to the middle left of the transform.
		/// </summary>
		MiddleLeft,
		/// <summary>
		///   Allign to the middle of the transform.
		/// </summary>
		Middle,
		/// <summary>
		///   Allign to the middle right of the transform.
		/// </summary>
		MiddleRight,
		/// <summary>
		///   Allign to the bottom left of the transform.
		/// </summary>
		BottomLeft,
		/// <summary>
		///   Allign to the bottom of the transform.
		/// </summary>
		Bottom,
		/// <summary>
		///   Allign to the bottom right of the transform.
		/// </summary>
		BottomRight,
	}

	/// <summary>
	///   A text label component.
	/// </summary>
	public class Label : MiComponent, IEquatable<Label>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Label()
		:	base()
		{
			Text   = new TextStyle( FilePaths.DefaultFont );
			Offset = new Vector2f();
			Allign = Allignment.TopLeft;
			m_text = new Text();

			RequiredComponents = new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="l">
		///   The label to copy from.
		/// </param>
		public Label( Label l )
		:	base( l )
		{
			Text   = new TextStyle( l.Text );
			Offset = l.Offset;
			Allign = l.Allign;
			m_text = new Text();

			RequiredComponents = new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Constructs the label and sets its display string.
		/// </summary>
		/// <param name="str">
		///   Display string. If null or whitespace it will be replaced with an empty string.
		/// </param>
		/// <param name="allign">
		///   Text allignment.
		/// </param>
		public Label( string str, Allignment allign = default )
		:	base()
		{
			Text   = new TextStyle( FilePaths.DefaultFont );
			Offset = new Vector2f();
			Allign = allign;
			m_text = new Text();
			String = !string.IsNullOrWhiteSpace( str ) ? str : string.Empty;

			RequiredComponents = new string[] { nameof( Transform ) };
		}
		/// <summary>
		///   Constructs the label with a text style and an optional string.
		/// </summary>
		/// <param name="text">
		///   Text style.
		/// </param>
		/// <param name="str">
		///   Display string. If null or whitespace it will be replaced with an
		///   empty string.
		/// </param>
		/// <param name="allign">
		///   Text allignment.
		/// </param>
		public Label( TextStyle text, string str = null, Allignment allign = default )
		:	base()
		{
			Text   = text ?? new TextStyle();
			Offset = new Vector2f();
			Allign = allign;
			m_text = new Text();
			String = !string.IsNullOrWhiteSpace( str ) ? str : string.Empty;

			RequiredComponents = new string[] { nameof( Transform ) };
		}

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Label ); }
		}

		/// <summary>
		///   Text style.
		/// </summary>
		public TextStyle Text
		{
			get;
			set;
		}
		/// <summary>
		///   Display string.
		/// </summary>
		public string String
		{
			get { return m_text.DisplayedString; }
			set { m_text.DisplayedString = value; }
		}
		/// <summary>
		///   Text offset.
		/// </summary>
		public Vector2f Offset
		{
			get; set;
		}
		/// <summary>
		///   Text allignment.
		/// </summary>
		public Allignment Allign
		{
			get; set;
		}

		/// <summary>
		///   Gets the global bounds from the internal text object. (For use before
		///   <see cref="MiObject.Update(float)"/> has is called).
		/// </summary>
		/// <remarks>
		///   Only needed before the first call to <see cref="MiObject.Update(float)"/>.
		///   The use <see cref="Transform.GlobalBounds"/>.
		/// </remarks>
		/// <returns></returns>
		public FloatRect GetTextBounds()
		{
			return m_text.GetGlobalBounds();
		}

		/// <summary>
		///   Updates the component logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Text == null )
				Text = new TextStyle();

			Text.Apply( ref m_text );

			Transform trn = Stack?.Get<Transform>();

			if( trn != null )
			{
				FloatRect bounds = GetTextBounds();
				Vector2f  pos;

				switch( Allign )
				{
					case Allignment.Top:
						pos = trn.Position + new Vector2f( ( trn.GlobalSize.X / 2.0f ) - ( bounds.Width / 2.0f ), 0.0f );
						break;
					case Allignment.TopRight:
						pos = trn.Position + new Vector2f( trn.GlobalSize.X - bounds.Width, 0.0f );
						break;

					case Allignment.MiddleLeft:
						pos = trn.Position + new Vector2f( 0.0f, ( trn.GlobalSize.Y / 2.0f ) - ( bounds.Height / 2.0f ) );
						break;
					case Allignment.Middle:
						pos = trn.Center - ( new Vector2f( bounds.Width, bounds.Height ) / 2.0f );
						break;
					case Allignment.MiddleRight:
						pos = trn.Position + new Vector2f( trn.GlobalSize.X - bounds.Width, ( trn.GlobalSize.Y / 2.0f ) - ( bounds.Height / 2.0f ) );
						break;

					case Allignment.BottomLeft:
						pos = trn.Position + new Vector2f( 0.0f, trn.GlobalSize.Y - bounds.Height );
						break;
					case Allignment.Bottom:
						pos = trn.Center + new Vector2f( 0.0f, ( trn.GlobalSize.Y / 2.0f ) - bounds.Height );
						break;
					case Allignment.BottomRight:
						pos = trn.Position + trn.GlobalSize - new Vector2f( bounds.Width, bounds.Height );
						break;

					default:
						pos = trn.Position;
						break;
				}


				m_text.Position = pos + Offset;
			}
		}
		/// <summary>
		///   Draws the component.
		/// </summary>
		/// <param name="target">
		///   The render target to draw to.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			m_text.Draw( target, states );
		}
		/// <summary>
		///   Called when disposing of the object.
		/// </summary>
		protected override void OnDispose()
		{
			m_text.Dispose();
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
			if( !Text.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loadding Label's TextStyle from stream.", false, LogType.Error );

			try
			{
				String = sr.ReadString();
				Offset = new Vector2f( sr.ReadSingle(), sr.ReadSingle() );
				Allign = (Allignment)sr.ReadInt32();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Label from stream: " + e.Message, false, LogType.Error );
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
			if( !Text.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving Label's TextStyle to stream", false, LogType.Error );

			try
			{
				sw.Write( String );
				sw.Write( Offset.X ); sw.Write( Offset.Y );
				sw.Write( (int)Allign );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Label to stream: " + e.Message, false, LogType.Error );
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

			Text   = new TextStyle();
			Offset = new Vector2f();
			String = element.GetAttribute( nameof( String ) );
			Allign = Allignment.TopLeft;

			XmlElement sty = element[ nameof( TextStyle ) ],
			           off = element[ nameof( Offset ) ];

			if( sty == null )
				return Logger.LogReturn( "Failed loading Label: No TextStyle xml element.", false, LogType.Error );
			if( !Text.LoadFromXml( sty ) )
				return Logger.LogReturn( "Failed loading Label: Loading TextStyle failed.", false, LogType.Error );

			if( element.HasAttribute( nameof( Allignment ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Allignment ) ), out Allignment a ) )
					return Logger.LogReturn( "Failed loading Label: Unable to parse Allignment xml element.", false, LogType.Error );

				Allign = a;
			}

			if( off != null )
			{
				Vector2f? o = Xml.ToVec2f( off );

				if( !o.HasValue )
					return Logger.LogReturn( "Failed loading Label: Parsing Offset failed.", false, LogType.Error );

				Offset = o.Value;
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

			sb.Append( "       " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "       " );
			sb.Append( nameof( String ) );
			sb.Append( "=\"" );
			sb.Append( String );
			sb.AppendLine( "\"" );

			sb.Append( "       " );
			sb.Append( nameof( Allignment ) );
			sb.Append( "=\"" );
			sb.Append( Allign.ToString() );
			sb.AppendLine( "\">" );

			sb.AppendLine( XmlLoadable.ToString( Text, 1 ) );

			if( !Offset.Equals( default ) )
				sb.AppendLine( Xml.ToString( Offset, nameof( Offset ), 1 ) );

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
		public bool Equals( Label other )
		{
			return base.Equals( other ) &&
				   Text.Equals( other.Text ) &&
				   Offset.Equals( other.Offset ) &&
				   String.Equals( other.String ) &&
				   Allign == other.Allign;
		}

		/// <summary>
		///   Gets the type name.
		/// </summary>
		/// <returns>
		///   The type name.
		/// </returns>
		public static string GetTypeName()
		{
			string name = string.Empty;

			using( Label a = new Label() )
				name = a.TypeName;

			return name;
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new Label( this );
		}

		private Text m_text;
	}
}
