////////////////////////////////////////////////////////////////////////////////
// Button.cs 
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
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using SFInput;
using System.IO;
using SharpLogger;
using SharpSerial;

namespace SharpGfx
{
	public static partial class FilePaths
	{
		/// <summary>
		///   Default button texture.
		/// </summary>
		public static readonly string ButtonTexture = FolderPaths.UI + "Button.png";
	}
}

namespace SharpGfx.UI
{	
	/// <summary>
	///   Possible button state.
	/// </summary>
	public enum ButtonState
	{
		/// <summary>
		///   If button is not being interacted with.
		/// </summary>
		Idle,
		/// <summary>
		///   If the button is selected or the mouse is hovering over it.
		/// </summary>
		Hover,
		/// <summary>
		///   If the button is being clicked.
		/// </summary>
		Click
	}

	/// <summary>
	///   Contains visual button info.
	/// </summary>
	public class ButtonData : BinarySerializable, IEquatable<ButtonData>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public ButtonData()
		{
			Image      = new ImageInfo();
			Text       = new TextStyle();
			TextOffset = new Vector2f();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="b"></param>
		public ButtonData( ButtonData b )
		{
			if( b == null )
				throw new ArgumentNullException();

			Image      = new ImageInfo( b.Image );
			Text       = new TextStyle( b.Text );
			TextOffset = b.TextOffset;
		}

		/// <summary>
		///   The image info.
		/// </summary>
		public ImageInfo Image { get; set; }
		/// <summary>
		///   The text style.
		/// </summary>
		public TextStyle Text { get; set; }
		/// <summary>
		///   The text offset.
		/// </summary>
		public Vector2f TextOffset { get; set; }

		/// <summary>
		///   If this object has the same values of the other object.
		/// </summary>
		/// <param name="other">
		///   The other object to check against.
		/// </param>
		/// <returns>
		///   True if both objects are concidered equal and false if they are not.
		/// </returns>
		public bool Equals( ButtonData other )
		{
			return other != null && Image.Equals( other.Image ) &&
				   Text.Equals( other.Text ) && TextOffset == other.TextOffset;
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
			if( sr == null )
				return Logger.LogReturn( "Unable to load ButtonData from null stream.", false, LogType.Error );

			if( Image == null )
				Image = new ImageInfo();
			if( Text == null )
				Text = new TextStyle();

			if( !Image.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load ButtonData Image from stream.", false, LogType.Error );
			if( !Text.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load ButtonData Text from stream.", false, LogType.Error );

			try
			{
				TextOffset = new Vector2f( sr.ReadSingle(), sr.ReadSingle() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to load ButtonData from stream: " + e.Message + ".", false, LogType.Error );
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
			if( sw == null )
				return Logger.LogReturn( "Unable to save ButtonData to null stream.", false, LogType.Error );

			if( Image == null )
				Image = new ImageInfo();
			if( Text == null )
				Text = new TextStyle();

			if( !Image.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save ButtonData Image to stream.", false, LogType.Error );
			if( !Text.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save ButtonData Text to stream.", false, LogType.Error );

			try
			{
				sw.Write( TextOffset.X ); sw.Write( TextOffset.Y );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Unable to save ButtonData to stream: " + e.Message + ".", false, LogType.Error );
			}

			return true;
		}
	}

	/// <summary>
	///   UI Button.
	/// </summary>
	public class Button : UIElement, IEquatable<Button>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Button()
		:	base()
		{
			m_image = new Image();
			m_label = new Label();
			State   = ButtonState.Idle;
			Data    = new ButtonData[ Enum.GetNames( typeof( ButtonState ) ).Length ];

			for( int i = 0; i < Data.Length; i++ )
				Data[ i ] = new ButtonData();
		}
		/// <summary>
		///  Copy constructor.
		/// </summary>
		/// <param name="u">
		///   The object to copy.
		/// </param>
		public Button( Button u )
		:	base( u )
		{
			m_image = new Image( u.m_image );
			m_label = new Label( u.m_label );
			State   = u.State;
			Data    = new ButtonData[ Enum.GetNames( typeof( ButtonState ) ).Length ];

			for( int i = 0; i < Data.Length; i++ )
				Data[ i ] = new ButtonData( u.Data[ i ] );
		}
		/// <summary>
		///   Constructor setting object name and optionally display text.
		/// </summary>
		/// <param name="id">
		///   The object ID.
		/// </param>
		/// <param name="text">
		///   The display text.
		/// </param>
		public Button( string id, string text = null )
		:	base( id )
		{
			m_image = new Image();
			m_label = new Label();
			State   = ButtonState.Idle;
			Data    = new ButtonData[ Enum.GetNames( typeof( ButtonState ) ).Length ];
			String  = text ?? string.Empty;

			for( int i = 0; i < Data.Length; i++ )
				Data[ i ] = new ButtonData();
		}

		/// <summary>
		///   Type string unique to each UI element type.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Button ); }
		}

		/// <summary>
		///   If the object is selectable.
		/// </summary>
		public override bool IsSelectable
		{
			get { return true; }
		}

		/// <summary>
		///   Data for each state of the button.
		/// </summary>
		public ButtonData[] Data { get; private set; }
		/// <summary>
		///   Button display string.
		/// </summary>
		public string String
		{
			get { return m_label.String; }
			set { m_label.String = value; }
		}

		/// <summary>
		///   Current button state.
		/// </summary>
		public ButtonState State
		{
			get; private set;
		} 

		/// <summary>
		///   If the mouse is hovering over the button.
		/// </summary>
		public bool Hovering
		{
			get
			{
				if( !Enabled )
					return false;

				Vector2f pos = Manager.Window.MapPixelToCoords( Input.Manager.Mouse.GetPosition( Manager.Window ) );
				return Transform.GlobalBounds.Contains( pos.X, pos.Y );
			}
		}
		/// <summary>
		///   if the button was clicked.
		/// </summary>
		public bool Clicked
		{
			get
			{
				return( Hovering && Input.Manager.Mouse.JustPressed( Mouse.Button.Left ) )||
					  ( Selected && Input.Manager.Keyboard.JustPressed( Keyboard.Key.Return ) );
			}
		}

		/// <summary>
		///   Updates the elements' logic.
		/// </summary>
		/// <param name="dt">
		///   Delta time.
		/// </param>
		protected override void OnUpdate( float dt )
		{
			m_image.Transform = Transform;
			m_image.Update( dt );

			m_label.Transform.Position = Transform.Position;
			m_label.Transform.Scale    = Transform.Scale;

			bool click = Clicked;

			if( click && Manager != null )
				Manager.Select( ID );				

			State = Clicked ? ButtonState.Click : ( Hovering || Selected ? ButtonState.Hover : ButtonState.Idle );
			int s = (int)State;

			m_image.DisplayImage = Data[ s ].Image;
			m_label.Text         = Data[ s ].Text;

			m_label.Transform.Center = Transform.Center;
			m_label.Transform.Position += Data[ s ].TextOffset;
						
			m_label.Update( dt );
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
			m_image.Draw( target, states );
			m_label.Draw( target, states );
		}
		/// <summary>
		///   Called when disposing of the object.
		/// </summary>
		protected override void OnDispose()
		{
			m_image.Dispose();
			m_label.Dispose();
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

			if( !m_image.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load UIButton image from stream.", false, LogType.Error );
			if( !m_label.LoadFromStream( sr ) )
				return Logger.LogReturn( "Unable to load UIButton label from stream.", false, LogType.Error );

			foreach( ButtonData bd in Data )
				if( !bd.LoadFromStream( sr ) )
					return Logger.LogReturn( "Unable to load UIButton data from stream.", false, LogType.Error );

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
			if( !m_image.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save UIButton image to stream.", false, LogType.Error );
			if( !m_label.SaveToStream( sw ) )
				return Logger.LogReturn( "Unable to save UIButton label to stream.", false, LogType.Error );

			foreach( ButtonData bd in Data )
				if( !bd.SaveToStream( sw ) )
					return Logger.LogReturn( "Unable to save UIButton data to stream.", false, LogType.Error );

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
		public bool Equals( Button other )
		{
			if( !base.Equals( other ) ||
				!m_image.Equals( other.m_image ) ||
				!m_label.Equals( other.m_label ) )
				return false;

			for( ButtonState b = 0; (int)b < Enum.GetNames( typeof( ButtonState ) ).Length; b++ )
				if( !Data[ (int)b ].Equals( other.Data[ (int)b ] ) )
					return false;

			return true;
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
		///   Constructs a button with default values and the given name and display text.
		/// </summary>
		/// <param name="name">
		///   Object name.
		/// </param>
		/// <param name="text">
		///   Display text.
		/// </param>
		/// <returns>
		///   A button with default values and the given name and display text.
		/// </returns>
		public static Button Default( string name, string text )
		{
			Button but = new Button( name, text );

			Texture tex = Assets.Manager.Texture.Get( FilePaths.ButtonTexture );

			if( tex == null )
				return null;

			Vector2u size = tex.Size;

			but.Transform.LocalSize = new Vector2f( size.X, size.Y / 3u );

			but.Data[ 0 ].Image = new ImageInfo( FilePaths.ButtonTexture, new FloatRect( 0, 0,               size.X, size.Y / 3u ) );
			but.Data[ 0 ].Text  = new TextStyle( FilePaths.DefaultFont );
			but.Data[ 1 ].Image = new ImageInfo( FilePaths.ButtonTexture, new FloatRect( 0, size.Y / 3u,     size.X, size.Y / 3u ) );
			but.Data[ 1 ].Text  = new TextStyle( FilePaths.DefaultFont );
			but.Data[ 2 ].Image = new ImageInfo( FilePaths.ButtonTexture, new FloatRect( 0, size.Y / 3u * 2, size.X, size.Y / 3u ) );
			but.Data[ 2 ].Text  = new TextStyle( FilePaths.DefaultFont );

			return but;
		}
		
		private Image m_image;
		private Label m_label;
	}
}
