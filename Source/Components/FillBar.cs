////////////////////////////////////////////////////////////////////////////////
// FillBar.cs 
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
		///   Default FillBar texture.
		/// </summary>
		public static readonly string FillBarTexture = FolderPaths.UI + "FillBar.png";
		/// <summary>
		///   Fill image padding for default FillBar texture.
		/// </summary>
		public static readonly Vector2f FillBarPadding = new Vector2f( 4, 4 );
	}

	/// <summary>
	///   A bar that fills up with progress.
	/// </summary>
	public class FillBar : MiComponent, IEquatable<FillBar>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public FillBar()
		:	base()
		{
			Progress    = 0.0f;
			Background  = new ImageInfo();
			Fill        = new ImageInfo();
			FillPadding = new Vector2f();

			m_bgverts = new VertexArray( PrimitiveType.Quads, 4 );
			m_flverts = new VertexArray( PrimitiveType.Quads, 4 );

			Texture tex = Assets.Manager.Get<Texture>( FilePaths.FillBarTexture );

			if( tex != null )
				SetTexture( FilePaths.FillBarTexture, FilePaths.FillBarPadding );

			RequiredComponents     = new string[] { nameof( Transform ) };
			IncompatibleComponents = new string[] { nameof( Button ), nameof( CheckBox ), 
			                                        nameof( Sprite ), nameof( TextBox ) };
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="s">
		///   The object to copy.
		/// </param>
		public FillBar( FillBar s )
		:	base( s )
		{
			Progress    = s.Progress;
			Background  = s.Background == null ? new ImageInfo() : new ImageInfo( s.Background );
			Fill        = s.Fill       == null ? new ImageInfo() : new ImageInfo( s.Fill );
			FillPadding = s.FillPadding;

			m_bgverts = new VertexArray( PrimitiveType.Quads, 4 );
			m_flverts = new VertexArray( PrimitiveType.Quads, 4 );

			RequiredComponents     = new string[] { nameof( Transform ) };
			IncompatibleComponents = new string[] { nameof( Button ), nameof( CheckBox ), 
			                                        nameof( Sprite ), nameof( TextBox ) };
		}
		/// <summary>
		///   Constructor setting direction.
		/// </summary>
		/// <param name="progress">
		///   Initial progress.
		/// </param>
		public FillBar( float progress )
		:	base()
		{
			Progress    = progress;
			Background  = new ImageInfo();
			Fill        = new ImageInfo();
			FillPadding = new Vector2f();

			m_bgverts = new VertexArray( PrimitiveType.Quads, 4 );
			m_flverts = new VertexArray( PrimitiveType.Quads, 4 );

			Texture tex = Assets.Manager.Get<Texture>( FilePaths.FillBarTexture );

			if( tex != null )
				SetTexture( FilePaths.FillBarTexture, FilePaths.FillBarPadding );

			RequiredComponents     = new string[] { nameof( Transform ) };
			IncompatibleComponents = new string[] { nameof( Button ), nameof( CheckBox ), 
			                                        nameof( Sprite ), nameof( TextBox ) };
		}

		/// <summary>
		///   Bar progress (between 0 and 1).
		/// </summary>
		public float Progress
		{
			get { return m_progress; }
			set { m_progress = value < 0.0f ? 0.0f : ( value > 1.0f ? 1.0f : value ); }
		}

		/// <summary>
		///   The background image.
		/// </summary>
		public ImageInfo Background
		{
			get; set;
		}
		/// <summary>
		///   The fill image.
		/// </summary>
		public ImageInfo Fill
		{
			get; set;
		}

		/// <summary>
		///   Padding around fill image to keep it alligned with the background.
		/// </summary>
		public Vector2f FillPadding
		{
			get; set;
		}

		/// <summary>
		///   The component type name
		/// </summary>
		public override string TypeName
		{
			get { return nameof( FillBar ); }
		}

		/// <summary>
		///   Sets up the progress bar with the given texture and fill padding.
		/// </summary>
		/// <param name="path">
		///   The path of the texture.
		/// </param>
		/// <param name="padding">
		///   The padding around the fill section of the texture.
		/// </param>
		/// <returns>
		///   True if the texture loaded successfully and all values were set, otherwise false.
		/// </returns>
		public bool SetTexture( string path, Vector2f padding = default )
		{
			Texture tex = Assets.Manager.Get<Texture>( path );

			if( tex == null )
				return false;

			Background  = new ImageInfo( path );
			Fill        = new ImageInfo( path );
			FillPadding = padding;
			return true;
		}

		/// <summary>
		///   Updates the progress bar.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Stack == null )
				return;
			if( Background == null )
				Background = new ImageInfo();
			if( Fill == null )
				Fill = new ImageInfo();

			if( Background.IsTextureValid )
			{
				Vector2u size   = Background.TextureSize;
				Background.Rect = new FloatRect( 0, 0, size.X, size.Y / 2 );
			}
			if( Fill.IsTextureValid )
			{
				Vector2u size = Fill.TextureSize;
				Fill.Rect = new FloatRect( FillPadding.X, FillPadding.Y + ( size.Y / 2 ),
										  ( size.X - ( FillPadding.X * 2 ) ) * Progress, ( size.Y / 2 ) - ( FillPadding.Y * 2 ) );
			}

			// Background image.
			Transform t = Stack.Get<Transform>();

			if( Background != null )
				for( uint i = 0; i < m_bgverts.VertexCount; i++ )
					m_bgverts[ i ] = Background.GetVertex( i, t );

			// Fill image.
			Transform f = new Transform( t );

			Vector2f pad = new Vector2f( FillPadding.X * f.Scale.X, FillPadding.Y * f.Scale.Y );

			f.Position += pad;
			f.Size -= FillPadding * 2;
			f.Size = new Vector2f( f.Size.X * Progress, f.Size.Y );

			if( Progress > 0.0f && f.Size.X >= 1.0f )
				for( uint i = 0; i < m_flverts.VertexCount; i++ )
					m_flverts[ i ] = Fill.GetVertex( i, f );

			// Label
			if( Stack.Contains<Label>() )
			{
				Stack.Get<Label>().String = string.Format( "{0:0.#}", (double)( Progress * 100.0 ) ) + "%";

				int pindex = Stack.IndexOf<FillBar>();

				// If label will be drawn under progress bar, move label on top of it.
				if( Stack.IndexOf<Label>() < pindex )
					Stack.Insert( pindex + 1, Stack.Release<Label>() );
			}
		}
		/// <summary>
		///   Draws the object to the render target.
		/// </summary>
		/// <param name="target">
		///   Render target.
		/// </param>
		/// <param name="states">
		///   Render states.
		/// </param>
		protected override void OnDraw( RenderTarget target, RenderStates states )
		{
			if( Background != null )
			{
				states.Texture = Assets.Manager.Get<Texture>( Background.Path );
				m_bgverts.Draw( target, states );
			}
			if( Progress > 0.0f )
			{
				states.Texture = Assets.Manager.Get<Texture>( Fill.Path );
				m_flverts.Draw( target, states );
			}
		}
		/// <summary>
		///   Disposes of the object.
		/// </summary>
		protected override void OnDispose()
		{
			m_bgverts.Dispose();
			m_flverts.Dispose();

			m_bgverts = null;
			m_flverts = null;
		}

		/// <summary>
		///   Loads the object from the stream.
		/// </summary>
		/// <param name="sr">
		///   The stream reader
		/// </param>
		/// <returns>
		///   True if the sprite was successfully loaded from the stream and false otherwise.
		/// </returns>
		public override bool LoadFromStream( BinaryReader sr )
		{
			if( !base.LoadFromStream( sr ) )
				return false;

			Background = new ImageInfo();
			Fill       = new ImageInfo();

			if( !Background.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to load Background from stream.", false, LogType.Error );
			if( !Fill.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to load Fill from stream.", false, LogType.Error );
			
			try
			{
				FillPadding = new Vector2f( sr.ReadSingle(), sr.ReadSingle() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading FillBar: " + e.Message, false, LogType.Error );
			}

			return true;
		}
		/// <summary>
		///   Writes the object to the stream.
		/// </summary>
		/// <param name="sw">
		///   The stream writer.
		/// </param>
		/// <returns>
		///   True if the sprite was successfully written to the stream and false otherwise.
		/// </returns>
		public override bool SaveToStream( BinaryWriter sw )
		{
			if( !base.SaveToStream( sw ) )
				return false;
			if( Background == null )
				Background = new ImageInfo();
			if( Fill == null )
				Fill = new ImageInfo();

			if( !Background.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving FillBar: Unable to save Background to stream.", false, LogType.Error );
			if( !Fill.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving FillBar: Unable to save Fill to stream.", false, LogType.Error );

			try
			{
				sw.Write( FillPadding.X ); sw.Write( FillPadding.Y );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving FillBar: " + e.Message, false, LogType.Error );
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

			Background  = new ImageInfo();
			Fill        = new ImageInfo();
			FillPadding = new Vector2f();

			XmlElement bg = element[ nameof( Background ) ]?[ nameof( ImageInfo ) ],
			           fl = element[ nameof( Fill ) ]?[ nameof( ImageInfo ) ],
					   pd = element[ nameof( Fill ) ]?[ nameof( FillPadding ) ];

			if( bg == null )
				return Logger.LogReturn( "Failed loading FillBar: No Background xml element.", false, LogType.Error );
			if( fl == null )
				return Logger.LogReturn( "Failed loading FillBar: No Fill xml element.", false, LogType.Error );

			if( !Background.LoadFromXml( bg ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to load Background from element.", false, LogType.Error );
			if( !Fill.LoadFromXml( fl ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to load Fill from element.", false, LogType.Error );

			if( pd != null )
			{
				Vector2f? padding = Xml.ToVec2f( pd );

				if( !padding.HasValue )
					return Logger.LogReturn( "Failed loading FillBar: Unable to parse FillPadding from element.", false, LogType.Error );

				FillPadding = padding.Value;
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

			sb.Append( "             " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\">" );

			sb.Append( "\t<" );
			sb.Append( nameof( Background ) );
			sb.Append( ">" );
			sb.AppendLine( XmlLoadable.ToString( Background, 2 ) );
			sb.Append( "\t</" );
			sb.Append( nameof( Background ) );
			sb.Append( ">" );

			sb.Append( "\t<" );
			sb.Append( nameof( Fill ) );
			sb.Append( ">" );
			sb.AppendLine( XmlLoadable.ToString( Fill, 2 ) );
			sb.AppendLine( Xml.ToString( FillPadding, nameof( FillPadding ), 2 ) );
			sb.Append( "\t</" );
			sb.Append( nameof( Fill ) );
			sb.Append( ">" );

			sb.Append( "</" );
			sb.Append( TypeName );
			sb.Append( ">" );

			return sb.ToString();
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new FillBar( this );
		}

		/// <summary>
		///   If this object is considered equal to the other object.
		/// </summary>
		/// <param name="other">
		///   The object to compare to.
		/// </param>
		/// <returns>
		///   True if this object is considered equal to the other object, otherwise false.
		/// </returns>
		public bool Equals( FillBar other )
		{
			return base.Equals( other ) && Progress == other.Progress &&
			       ( Background?.Equals( other.Background ) ?? false ) &&
				   ( Fill?.Equals( other.Fill ) ?? false ) &&
				   FillPadding.Equals( other.FillPadding );
		}

		float m_progress;

		VertexArray m_bgverts,
		            m_flverts;
	}
}
