////////////////////////////////////////////////////////////////////////////////
// CheckBox.cs 
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
	///   Possible state of check box.
	/// </summary>
	public enum CheckBoxState
	{
		/// <summary>
		///   If not selected and not checked.
		/// </summary>
		Unchecked,
		/// <summary>
		///   If not selected and checked.
		/// </summary>
		Checked,
		/// <summary>
		///   If selected and unchecked.
		/// </summary>
		SelectedUnchecked,
		/// <summary>
		///   If selected and checked.
		/// </summary>
		SelectedChecked
	}

	/// <summary>
	///   A toggling check box component.
	/// </summary>
	public class CheckBox : MiComponent, IEquatable<CheckBox>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public CheckBox()
		:	base()
		{
			Colors = new Color[ Enum.GetNames( typeof( CheckBoxState ) ).Length ];

			for( int i = 0; i < Colors.Length; i++ )
				Colors[ i ] = Color.White;

			Checked = false;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="b">
		///   The object to copy.
		/// </param>
		public CheckBox( CheckBox b )
		:	base( b )
		{
			Colors = new Color[ Enum.GetNames( typeof( CheckBoxState ) ).Length ];

			for( int i = 0; i < Colors.Length; i++ )
				Colors[ i ] = new Color( b.Colors[ i ] );

			Checked = b.Checked;
		}
		/// <summary>
		///   Constructor setting the initial checked value.
		/// </summary>
		/// <param name="check">
		///   The initial check state of the checkbox.
		/// </param>
		public CheckBox( bool check )
		:	base()
		{
			Colors = new Color[ Enum.GetNames( typeof( CheckBoxState ) ).Length ];

			for( int i = 0; i < Colors.Length; i++ )
				Colors[ i ] = Color.White;

			Checked = check;
		}

		/// <summary>
		///   The component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( CheckBox ); }
		}

		/// <summary>
		///   Current state of the object.
		/// </summary>
		public CheckBoxState State
		{
			get;
			private set;
		}

		/// <summary>
		///   Array of texture modifier colors for each checkbox state.
		/// </summary>
		public Color[] Colors { get; private set; }
		/// <summary>
		///   If the box is checked.
		/// </summary>
		public bool Checked { get; set; }

		/// <summary>
		///   Gets the type names of components required by this component type.
		/// </summary>
		/// <returns>
		///   The type names of components required by this component type.
		/// </returns>
		protected override string[] GetRequiredComponents()
		{
			return new string[] { nameof( Transform ), nameof( Selectable ), nameof( Clickable ),
			                      nameof( Sprite ) };
		}
		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Button ),  nameof( FillBar ), nameof( TextBox ),
			                      nameof( SpriteAnimator ), nameof( SpriteArray ) };
		}

		/// <summary>
		///   Called when the component is added to an entity.
		/// </summary>
		public override void OnAdd()
		{
			Sprite spr = Parent.GetComponent<Sprite>();
			spr.Image = new ImageInfo( FolderPaths.UI + "CheckBox.png" );

			if( spr.Image.IsTextureValid )
			{
				Vector2u size = spr.Image.TextureSize / 2;
				Parent.GetComponent<Transform>().Size = new Vector2f( size.X, size.Y );
			}
		}
		/// <summary>
		///   Refreshes components' visual elements.
		/// </summary>
		public override void Refresh()
		{
			if( Parent == null )
				return;

			Sprite  spr = Parent.GetComponent<Sprite>();
			Texture tex = spr.Image.Texture;

			if( tex != null )
			{
				Vector2u size = tex.Size;
				spr.Image.Rect = new FloatRect( 0, 0, size.X / 2, size.Y / 2 );
			}
						
			spr.Image.Color = Colors[ 0 ];
		}
		/// <summary>
		///   Updates the component logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			if( Parent == null )
				return;

			Selectable sel = Parent.GetComponent<Selectable>();
			Clickable  clk = Parent.GetComponent<Clickable>();
			Sprite     spr = Parent.GetComponent<Sprite>();
			Texture    tex = spr.Image.Texture;

			if( clk.Clicked )
				Checked = !Checked;

			State = ( sel.Selected || clk.Hovering ) ?
					( Checked ? CheckBoxState.SelectedChecked : CheckBoxState.SelectedUnchecked ) :
					( Checked ? CheckBoxState.Checked : CheckBoxState.Unchecked );

			if( tex != null )
			{
				Vector2u size = tex.Size;

				if( State == CheckBoxState.Checked )
					spr.Image.Rect = new FloatRect( size.X / 2, 0, size.X / 2, size.Y / 2 );
				else if( State == CheckBoxState.SelectedUnchecked )
					spr.Image.Rect = new FloatRect( 0, size.Y / 2, size.X / 2, size.Y / 2 );
				else if( State == CheckBoxState.SelectedChecked )
					spr.Image.Rect = new FloatRect( size.X / 2, size.Y / 2, size.X / 2, size.Y / 2 );
			}
						
			spr.Image.Color = Colors[ (int)State ];
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
				Checked = sr.ReadBoolean();

				for( CheckBoxState s = 0; (int)s < Enum.GetNames( typeof( CheckBoxState ) ).Length; s++ )
					Colors[ (int)s ] = new Color( sr.ReadByte(), sr.ReadByte(), sr.ReadByte(), sr.ReadByte() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Checkbox: " + e.Message, false, LogType.Error );
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
				sw.Write( Checked );

				foreach( Color i in Colors )
				{
					sw.Write( i.R );
					sw.Write( i.G );
					sw.Write( i.B );
					sw.Write( i.A );
				}
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Checkbox: " + e.Message, false, LogType.Error );
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

			Checked = false;
			XmlNodeList data = element.SelectNodes( nameof( Color ) );

			if( data.Count != Colors.Length )
				return Logger.LogReturn( "Failed loading CheckBox: Incorrect amount of Color xml elements.", false, LogType.Error );

			for( int i = 0; i < Colors.Length; i++ )
			{
				Color? c = Xml.ToColor( (XmlElement)data[ i ] );
				
				if( !c.HasValue )
					return Logger.LogReturn( "Failed loading CheckBox's Color.", false, LogType.Error );

				Colors[ i ] = c.Value;
			}

			string check = element.GetAttribute( nameof( Checked ) );

			if( !string.IsNullOrWhiteSpace( check ) )
			{
				try
				{
					Checked = bool.Parse( check );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed loading CheckBox: " + e.Message, false, LogType.Error );
				}
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

			sb.Append( "          " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "          " );
			sb.Append( nameof( Checked ) );
			sb.Append( "=\"" );
			sb.Append( Checked );
			sb.AppendLine( "\">" );

			for( int i = 0; i < Colors.Length; i++ )
				sb.AppendLine( Xml.ToString( Colors[ i ], nameof( Color ), 1 ) );

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
		public bool Equals( CheckBox other )
		{
			if( !base.Equals( other ) || Checked != other.Checked )
				return false;

			for( CheckBoxState s = 0; (int)s < Enum.GetNames( typeof( CheckBoxState ) ).Length; s++ )
				if( !Colors[ (int)s ].Equals( other.Colors[ (int)s ] ) )
					return false;

			return true;
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new CheckBox( this );
		}

		/// <summary>
		///   Creates an entity and sets it up with a default checkbox.
		/// </summary>
		/// <param name="id">
		///   The entity id.
		/// </param>
		/// <param name="window">
		///   The render window.
		/// </param>
		/// <param name="check">
		///   If the box should be checked.
		/// </param>
		/// <returns>
		///   A valid entity containing the checkbox on success or null on failure.
		/// </returns>
		public static MiEntity Create( string id = null, RenderWindow window = null, bool check = false )
		{
			MiEntity ent = new MiEntity( id, window );

			if( !ent.AddComponent( new CheckBox( check ), true ) )
			{
				ent.Dispose();
				return Logger.LogReturn<MiEntity>( "Failed creating CheckBox entity: Adding CheckBox failed.", null, LogType.Error );
			}

			Sprite spr = ent.GetComponent<Sprite>();
			spr.Image = new ImageInfo( FolderPaths.UI + "CheckBox.png" );

			if( !spr.Image.IsTextureValid )
			{
				ent.Dispose();
				return Logger.LogReturn<MiEntity>( "Failed creating CheckBox entity: Loading Texture failed.", null, LogType.Error );
			}

			if( window != null )
			{
				Vector2u size = spr.Image.TextureSize / 2;
				ent.GetComponent<Transform>().Size = new Vector2f( size.X, size.Y );
			}

			return ent;
		}
	}
}
