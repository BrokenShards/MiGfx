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
	///   Image display information.
	/// </summary>
	public class FillBarInfo : BinarySerializable, IXmlLoadable, IEquatable<FillBarInfo>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public FillBarInfo()
		{
			Color = Color.White;
			Orientation = Direction.Up;
			FlipHorizontal = false;
			FlipVertical = false;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="i">
		///   The fill bar info to copy from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If <paramref name="i"/> is null.
		/// </exception>
		public FillBarInfo( FillBarInfo i )
		{
			if( i == null )
				throw new ArgumentNullException();

			Color = i.Color;
			Orientation = i.Orientation;
			FlipHorizontal = i.FlipHorizontal;
			FlipVertical = i.FlipVertical;
		}
		/// <summary>
		///   Constructor that assigns values.
		/// </summary>
		/// <param name="col">
		///   Texture color modifier.
		/// </param>
		/// <param name="dir">
		///   Orientation direction.
		/// </param>
		/// <param name="hflip">
		///   Horizontal flip.
		/// </param>
		/// <param name="vflip">
		///   Vertical flip..
		/// </param>
		public FillBarInfo( Color col, Direction dir = 0, bool hflip = false, bool vflip = false )
		{
			Color = col;
			Orientation = dir;
			FlipHorizontal = hflip;
			FlipVertical = vflip;
		}

		/// <summary>
		///   Texture color modifier.
		/// </summary>
		public Color Color { get; set; }
		/// <summary>
		///   Texture orientation modifier.
		/// </summary>
		public Direction Orientation
		{
			get; set;
		}
		/// <summary>
		///   If image should be flipped horizontally on the x-axis.
		/// </summary>
		public bool FlipHorizontal { get; set; }
		/// <summary>
		///   If image should be flipped vertically on the y-axis.
		/// </summary>
		public bool FlipVertical { get; set; }

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
				return Logger.LogReturn( "Unable to load FillBarInfo from null stream.", false, LogType.Error );

			try
			{
				Color = new Color( br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte() );
				Orientation = (Direction)br.ReadInt32();
				FlipHorizontal = br.ReadBoolean();
				FlipVertical = br.ReadBoolean();
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load FillBarInfo from stream: " + e.Message, false, LogType.Error );
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
				return Logger.LogReturn( "Unable to save FillBarInfo to null stream.", false, LogType.Error );

			try
			{
				bw.Write( Color.R ); bw.Write( Color.G ); bw.Write( Color.B ); bw.Write( Color.A );
				bw.Write( (int)Orientation );
				bw.Write( FlipHorizontal ); bw.Write( FlipVertical );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save FillBarInfo to stream: " + e.Message, false, LogType.Error );
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
				return Logger.LogReturn( "Cannot load FillBarInfo from a null XmlElement.", false, LogType.Error );

			XmlElement color = element[ nameof( Color ) ];
			Color? col = color != null ? Xml.ToColor( color ) : null;

			if( color != null && !col.HasValue )
				return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse Color element.", false, LogType.Error );
			else if( color == null )
				col = Color.White;

			Color = col.Value;
			Orientation = Direction.Up;
			FlipHorizontal = false;
			FlipVertical = false;

			if( element.HasAttribute( nameof( Orientation ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Orientation ) ), out Direction o ) )
					return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse Orientation attribute.", false, LogType.Error );

				Orientation = o;
			}
			if( element.HasAttribute( nameof( FlipHorizontal ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipHorizontal ) ), out bool f ) )
					return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse FlipHorizontal attribute.", false, LogType.Error );

				FlipHorizontal = f;
			}
			if( element.HasAttribute( nameof( FlipVertical ) ) )
			{
				if( !bool.TryParse( element.GetAttribute( nameof( FlipVertical ) ), out bool f ) )
					return Logger.LogReturn( "Failed loading ImageInfo: Unable to parse FlipVertical attribute.", false, LogType.Error );

				FlipVertical = f;
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
			sb.Append( nameof( ImageInfo ) );

			sb.Append( " " );
			sb.Append( nameof( Orientation ) );
			sb.Append( "=\"" );
			sb.Append( Orientation );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( FlipHorizontal ) );
			sb.Append( "=\"" );
			sb.Append( FlipHorizontal );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( FlipVertical ) );
			sb.Append( "=\"" );
			sb.Append( FlipVertical );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Color, nameof( Color ), 1 ) );

			sb.Append( "</" );
			sb.Append( nameof( ImageInfo ) );
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
		public bool Equals( FillBarInfo other )
		{
			return other != null &&
				   Color.Equals( other.Color ) &&
				   Orientation == other.Orientation &&
				   FlipHorizontal == other.FlipHorizontal &&
				   FlipVertical == other.FlipVertical;
		}
	}

	/// <summary>
	///   A bar that fills up with progress.
	/// </summary>
	public class FillBar : MiComponent, IEquatable<FillBar>
	{
		/// <summary>
		///   Possible label configurations.
		/// </summary>
		public enum LabelType
		{
			/// <summary>
			///   Label displays nothing.
			/// </summary>
			None,
			/// <summary>
			///   Label displays value as a percentage.
			/// </summary>
			Percentage,
			/// <summary>
			///   Label displays value as a ratio between 0 and 1.
			/// </summary>
			Decimal,
			/// <summary>
			///   Label displays value.
			/// </summary>
			Value,
			/// <summary>
			///   Label displays value and max value.
			/// </summary>
			ValueMax
		}

		/// <summary>
		///   Constructor.
		/// </summary>
		public FillBar()
		:	base()
		{
			m_min    =   0;
			Max      = 100;
			Value    =   0;
			Labeling =   0;

			Background  = new FillBarInfo();
			Fill        = new FillBarInfo();
			FillPadding = new Vector2f();

			RequiredComponents     = new string[] { nameof( Transform ), nameof( SpriteArray ) };
			IncompatibleComponents = new string[] { nameof( Button ), nameof( CheckBox ), 
			                                        nameof( Sprite ), nameof( TextBox ),
													nameof( SpriteAnimator ) };
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
			m_min    = s.Min;
			Max      = s.Max;
			Value    = s.Value;
			Labeling = s.Labeling;

			Background  = s.Background == null ? new FillBarInfo() : new FillBarInfo( s.Background );
			Fill        = s.Fill       == null ? new FillBarInfo() : new FillBarInfo( s.Fill );
			FillPadding = s.FillPadding;

			RequiredComponents     = new string[] { nameof( Transform ), nameof( SpriteArray ) };
			IncompatibleComponents = new string[] { nameof( Button ), nameof( CheckBox ),
													nameof( Sprite ), nameof( TextBox ),
													nameof( SpriteAnimator ) };
		}
		/// <summary>
		///   Constructor setting min-max values.
		/// </summary>
		/// <param name="val">
		///   Initial value.
		/// </param>
		/// <param name="min">
		///   Minimum value.
		/// </param>
		/// <param name="max">
		///   Maximum value.
		/// </param>
		/// <param name="lt">
		///   The labeling type.
		/// </param>
		public FillBar( long val, long min = 0, long max = 100, LabelType lt = 0 )
		:	base()
		{
			if( min > max )
				min = max;

			m_min    = min;
			m_max    = max;
			Value    = val;
			Labeling = lt;

			Background  = new FillBarInfo();
			Fill        = new FillBarInfo();
			FillPadding = new Vector2f();

			RequiredComponents     = new string[] { nameof( Transform ), nameof( SpriteArray ) };
			IncompatibleComponents = new string[] { nameof( Button ), nameof( CheckBox ),
													nameof( Sprite ), nameof( TextBox ),
													nameof( SpriteAnimator ) };
		}

		/// <summary>
		///   Minimum value.
		/// </summary>
		public long Min
		{
			get { return m_min; }
			set
			{
				m_min = value > Max ? Max : value;

				if( Value < Min )
					Value = Min;
			}
		}
		/// <summary>
		///   Maximum value.
		/// </summary>
		public long Max
		{
			get { return m_max; }
			set
			{
				m_max = value < Min ? Min : value;

				if( Value > Max )
					Value = Max;
			}
		}
		/// <summary>
		///   Current value.
		/// </summary>
		public long Value
		{
			get { return m_value; }
			set { m_value = value < Min ? Min : ( value > Max ? Max : value ); }
		}

		/// <summary>
		///   Bar progress (between 0 and 1).
		/// </summary>
		public float Progress
		{
			get
			{
				if( Min == Max )
					return 1.0f;

				return (float)( Value - Min ) / ( Max - Min );
			}
		}
		/// <summary>
		///   How the label should display the value.
		/// </summary>
		public LabelType Labeling
		{
			get { return m_label; }
			set
			{
				if( value < 0 || (int)value >= Enum.GetNames( typeof( LabelType ) ).Length )
					m_label = 0;
				else
					m_label = value;
			}
		}

		/// <summary>
		///   The background image.
		/// </summary>
		public FillBarInfo Background
		{
			get; set;
		}
		/// <summary>
		///   The fill image.
		/// </summary>
		public FillBarInfo Fill
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
				Background = new FillBarInfo();
			if( Fill == null )
				Fill = new FillBarInfo();

			Transform   tr = Stack.Get<Transform>();
			SpriteArray sa = Stack.Get<SpriteArray>();

			Texture tex = Assets.Manager.Get<Texture>( sa.TexturePath );

			if( tex != null )
			{
				Vector2u size = tex.Size;

				SpriteInfo bginfo = new SpriteInfo( new FloatRect( 0, 0, size.X, size.Y / 2 ),
									Background.Color, null, tr.Size, Background.Orientation,
									Background.FlipHorizontal, Background.FlipVertical ),
				           flinfo = new SpriteInfo( 
							        new FloatRect( FillPadding.X, FillPadding.Y + ( size.Y / 2 ),
										         ( size.X - ( FillPadding.X * 2 ) ) * Progress,
												 ( size.Y / 2 ) - ( FillPadding.Y * 2 ) ),
									Fill.Color, FillPadding, tr.Size - ( FillPadding * 2 ), Fill.Orientation,
									Fill.FlipHorizontal, Fill.FlipVertical );

				if( Progress == 0.0f )
					flinfo.Color = new Color( 255, 255, 255, 0 );
				else
					flinfo.Size = new Vector2f( flinfo.Size.X * Progress, flinfo.Size.Y );

				if( sa.Sprites.Count != 2 )
				{
					sa.Sprites.Clear();
					sa.Sprites.Add( bginfo );
					sa.Sprites.Add( flinfo );
				}
				else
				{
					sa.Sprites[ 0 ] = bginfo;
					sa.Sprites[ 1 ] = flinfo;
				}
			}

			// Label
			if( Stack.Contains<Label>() )
			{
				if( Labeling == LabelType.Percentage )
					Stack.Get<Label>().String = string.Format( "{0:0.#}", (double)( Progress * 100.0 ) ) + "%";
				else if( Labeling == LabelType.Decimal )
					Stack.Get<Label>().String = string.Format( "{0:0.#}", (double)Progress );
				else if( Labeling == LabelType.Value )
					Stack.Get<Label>().String = Value.ToString();
				else if( Labeling == LabelType.ValueMax )
					Stack.Get<Label>().String = Value.ToString() + "/" + Max.ToString();
				else
					Stack.Get<Label>().String = string.Empty;
			}
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

			Background = new FillBarInfo();
			Fill       = new FillBarInfo();

			if( !Background.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading FillBar's Background info from stream.", false, LogType.Error );
			if( !Fill.LoadFromStream( sr ) )
				return Logger.LogReturn( "Failed loading FillBar's Fill info from stream.", false, LogType.Error );
			
			try
			{
				m_min = sr.ReadInt64();
				m_max = sr.ReadInt64();
				Value = sr.ReadInt64();

				Labeling = (LabelType)sr.ReadInt32();

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
				Background = new FillBarInfo();
			if( Fill == null )
				Fill = new FillBarInfo();

			if( !Background.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving FillBar's Background info to stream.", false, LogType.Error );
			if( !Fill.SaveToStream( sw ) )
				return Logger.LogReturn( "Failed saving FillBar's Fill info to stream.", false, LogType.Error );

			try
			{
				sw.Write( Min ); sw.Write( Max ); sw.Write( Value );
				sw.Write( (int)Labeling );
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

			Labeling   = LabelType.Percentage;
			Background  = new FillBarInfo();
			Fill        = new FillBarInfo();
			FillPadding = new Vector2f();

			XmlElement bg = element[ nameof( Background ) ]?[ nameof( ImageInfo ) ],
			           fl = element[ nameof( Fill ) ]?[ nameof( ImageInfo ) ],
					   pd = element[ nameof( Fill ) ]?[ nameof( FillPadding ) ];

			if( !element.HasAttribute( nameof( Min ) ) )
				return Logger.LogReturn( "Failed loading FillBar: No Min xml attribute.", false, LogType.Error );
			if( !element.HasAttribute( nameof( Max ) ) )
				return Logger.LogReturn( "Failed loading FillBar: No Max xml attribute.", false, LogType.Error );
			if( !element.HasAttribute( nameof( Value ) ) )
				return Logger.LogReturn( "Failed loading FillBar: No Value xml attribute.", false, LogType.Error );

			if( bg == null )
				return Logger.LogReturn( "Failed loading FillBar: No Background xml element.", false, LogType.Error );
			if( fl == null )
				return Logger.LogReturn( "Failed loading FillBar: No Fill xml element.", false, LogType.Error );

			if( !Background.LoadFromXml( bg ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to load Background from element.", false, LogType.Error );
			if( !Fill.LoadFromXml( fl ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to load Fill from element.", false, LogType.Error );

			if( !long.TryParse( element.GetAttribute( nameof( Min ) ), out long min ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to parse Min xml attribute.", false, LogType.Error );
			if( !long.TryParse( element.GetAttribute( nameof( Max ) ), out long max ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to parse Max xml attribute.", false, LogType.Error );
			if( !long.TryParse( element.GetAttribute( nameof( Value ) ), out long val ) )
				return Logger.LogReturn( "Failed loading FillBar: Unable to parse Value xml attribute.", false, LogType.Error );

			if( element.HasAttribute( nameof( Labeling ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Labeling ) ), out LabelType lab ) )
					return Logger.LogReturn( "Failed loading FillBar: Unable to parse Labelling xml attribute.", false, LogType.Error );

				Labeling = lab;
			}

			m_min = min;
			Max   = max;
			Value = val;

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

			sb.Append( "         " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "         " );
			sb.Append( nameof( Min ) );
			sb.Append( "=\"" );
			sb.Append( Min );
			sb.AppendLine( "\"" );

			sb.Append( "         " );
			sb.Append( nameof( Max ) );
			sb.Append( "=\"" );
			sb.Append( Max );
			sb.AppendLine( "\"" );

			sb.Append( "         " );
			sb.Append( nameof( Value ) );
			sb.Append( "=\"" );
			sb.Append( Value );
			sb.AppendLine( "\"" );

			sb.Append( "         " );
			sb.Append( nameof( Labeling ) );
			sb.Append( "=\"" );
			sb.Append( Labeling.ToString() );
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
			return base.Equals( other ) && Min == other.Min && Max == other.Max && Value == other.Value &&
				   ( Background?.Equals( other.Background ) ?? false ) && Labeling == other.Labeling &&
				   ( Fill?.Equals( other.Fill ) ?? false ) &&
				   FillPadding.Equals( other.FillPadding );
		}

		/// <summary>
		///   Creates an entity and sets it up with a default fillbar.
		/// </summary>
		/// <param name="id">
		///   Entity ID.
		/// </param>
		/// <param name="window">
		///   Render window.
		/// </param>
		/// <param name="val">
		///   Current fill value.
		/// </param>
		/// <param name="min">
		///   Minimum fill value.
		/// </param>
		/// <param name="max">
		///   Maximum fill value.
		/// </param>
		/// <param name="col">
		///   Fill image color modifier.
		/// </param>
		/// <param name="lt">
		///   Labeling type.
		/// </param>
		/// <returns>
		///   A valid entity containing the fillbar on success or null on failure.
		/// </returns>
		public static MiEntity Create( string id = null, RenderWindow window = null, long val = 0, long min = 0, long max = 100, Color? col = null, LabelType lt = 0 )
		{
			MiEntity ent = new MiEntity( id, window );

			if( !ent.Components.Add( new FillBar( val, min, max ) ) )
			{
				ent.Dispose();
				return Logger.LogReturn<MiEntity>( "Failed creating FillBar entity: Adding FillBar failed.", null, LogType.Error );
			}
			if( !ent.Components.AddNew<Label>() )
			{
				ent.Dispose();
				return Logger.LogReturn<MiEntity>( "Failed creating FillBar entity: Adding Label failed.", null, LogType.Error );
			}

			ent.Components.Get<SpriteArray>().TexturePath = FilePaths.FillBarTexture;

			FillBar fb = ent.Components.Get<FillBar>();
			fb.Labeling = lt;
			fb.Fill.Color = col ?? Color.White;

			Label lab = ent.Components.Get<Label>();
			lab.Text.FillColor = Color.White;
			lab.Allign = Allignment.Middle;
			lab.Offset = new Vector2f( 0, -8.0f );

			Transform trn = ent.Components.Get<Transform>();

			trn.Size = new Vector2f( 640, 64 );
			trn.LockSize = true;

			return ent;
		}

		long m_min, 
		     m_max,
		     m_value;

		LabelType m_label;
	}
}
