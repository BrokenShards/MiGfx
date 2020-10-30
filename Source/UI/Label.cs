////////////////////////////////////////////////////////////////////////////////
// Label.cs 
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

namespace SharpGfx
{
	public static partial class FilePaths
	{
		/// <summary>
		///   Default font path.
		/// </summary>
		public static string DefaultFont = FolderPaths.Fonts + "FallingSky.otf";
	}
}
namespace SharpGfx.UI
{ 
	/// <summary>
	///   A text label UI element.
	/// </summary>
	public class Label : UIElement, IEquatable<Label>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Label()
		:	base()
		{
			Text   = new TextStyle();
			m_text = new Text();
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
			m_text = new Text();
		}
		/// <summary>
		///   Constructs the label and sets its ID.
		/// </summary>
		/// <param name="id">
		///   The label ID.
		/// </param>
		public Label( string id )
		:	base( id )
		{
			Text   = new TextStyle();
			m_text = new Text();
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
		public Label( TextStyle text, string str = null )
		:	base()
		{
			Text   = text ?? new TextStyle();
			m_text = new Text();

			if( !string.IsNullOrWhiteSpace( str ) )
				String = str;
		}
		/// <summary>
		///   Constructs the label with an ID, text style and optional string.
		/// </summary>
		/// <param name="id">
		///   The label ID.
		/// </param>
		/// <param name="text">
		///   Text style.
		/// </param>
		/// <param name="str">
		///   Display string. If null or whitespace it will be replaced with an
		///   empty string.
		/// </param>
		public Label( string id, TextStyle text, string str = null )
		:	base( id )
		{
			Text   = text ?? new TextStyle();
			m_text = new Text();

			if( !string.IsNullOrWhiteSpace( str ) )
				String = str;
		}

		/// <summary>
		///   Type string unique to each UI element type.
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
		///   Gets the global bounds from the internal text object. (Use when 
		///   <see cref="UIElement.Update(float)"/> has not yet been called).
		/// </summary>
		/// <remarks>
		///   Only needed before the first call to <see cref="UIElement.Update(float)"/>.
		///   The use <see cref="Transform.GlobalBounds"/>.
		/// </remarks>
		/// <returns></returns>
		public FloatRect GetTextBounds()
		{
			return m_text.GetGlobalBounds();
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Text == null )
				Text = new TextStyle();

			Text.Apply( ref m_text );

			m_text.Position = Transform.Position;
			FloatRect bounds = m_text.GetGlobalBounds();
			if( bounds.Width <= 0.0f )
				bounds.Width = 1.0f;
			if( bounds.Height <= 0.0f )
				bounds.Height = 1.0f;

			Transform.LocalSize = new Vector2f( bounds.Width, bounds.Height );
		}
		/// <summary>
		///   Draws the element.
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
				return Logger.LogReturn( "Unable to load UILabels' TextStyle from stream.", false, LogType.Error );

			try
			{
				String = sr.ReadString();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load UILabel from stream: " + e.Message + ".", false, LogType.Error );
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
				return Logger.LogReturn( "Unable to save UIImages' Image to stream", false, LogType.Error );

			try
			{
				sw.Write( String );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save UILabel to stream: " + e.Message + ".", false, LogType.Error );
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
		public bool Equals( Label other )
		{
			return base.Equals( other ) &&
				   Text.Equals( other.Text ) &&
				   String == other.String;
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

			using( AnimatedImage a = new AnimatedImage() )
				name = a.TypeName;

			return name;
		}

		/// <summary>
		///   Creates a default label with the given ID and optional string.
		/// </summary>
		/// <param name="id">
		///   The label ID.
		/// </param>
		/// <param name="str">
		///   The display string.
		/// </param>
		/// <returns>
		///   A new default label with the given ID and optional string.
		/// </returns>
		public static Label Default( string id, string str = null )
		{
			return new Label( id, new TextStyle( FilePaths.DefaultFont ), str );
		}

		private Text m_text;
	}
}
