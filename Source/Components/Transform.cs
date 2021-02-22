////////////////////////////////////////////////////////////////////////////////
// Transform.cs 
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using SFML.Graphics;
using SFML.System;
using MiCore;

namespace MiGfx
{
	/// <summary>
	///   Interface for objects with a transform.
	/// </summary>
	public interface ITransformable
	{
		/// <summary>
		///   The transform.
		/// </summary>
		Transform Transform { get; }
	}

	/// <summary>
	///   A 2D transformation.
	/// </summary>
	public class Transform : MiComponent, IEquatable<Transform>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Transform()
		:	base()
		{
			Origin   = Allignment.TopLeft;
			Position = new Vector2f();
			Size     = new Vector2f( 1.0f, 1.0f );
			Scale    = new Vector2f( 1.0f, 1.0f );
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The transform to copy from.
		/// </param>
		public Transform( Transform t )
		:	base( t )
		{
			Origin   = t.Origin;
			Position = t.Position;
			Size     = t.Size;
			Scale    = t.Scale;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The transform to copy from.
		/// </param>
		public Transform( UITransform t )
		:	base( t )
		{
			Origin   = t.Origin;
			Position = t.PixelPosition;
			Size     = t.PixelSize;
			Scale    = t.Scale;
		}
		/// <summary>
		///   Constructor assigning values.
		/// </summary>
		/// <param name="pos">
		///   Transform position.
		/// </param>
		/// <param name="size">
		///   Transform size.
		/// </param>
		/// <param name="scl">
		///   Transform scale.
		/// </param>
		/// <param name="org">
		///   Origin point.
		/// </param>
		public Transform( Vector2f pos, Vector2f? size = null, Vector2f? scl = null, Allignment org = 0 )
		:	base()
		{
			Origin   = org;
			Position = pos;
			Size     = size ?? new Vector2f( 1.0f, 1.0f );
			Scale    = scl  ?? new Vector2f( 1.0f, 1.0f );
		}

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( Transform ); }
		}

		/// <summary>
		///   The origin point of the transform.
		/// </summary>
		public Allignment Origin
		{
			get; set;
		}

		/// <summary>
		///   Position.
		/// </summary>
		public Vector2f Position 
		{
			get; set;
		}
		/// <summary>
		///   Local size.
		/// </summary>
		public Vector2f Size
		{
			get { return m_size; }
			set
			{
				if( value.X <= 0.0f )
				{
					float x = Math.Abs( value.X );
					value.X = x > 0.0f ? x : 1.0f;
				}
				if( value.Y <= 0.0f )
				{
					float y = Math.Abs( value.Y );
					value.Y = y > 0.0f ? y : 1.0f;
				}

				m_size = value;
			}
		}
		/// <summary>
		///   Scale.
		/// </summary>
		public Vector2f Scale
		{
			get { return m_scale; }
			set
			{
				if( value.X <= 0.0f || value.Y <= 0.0f )
					m_scale = new Vector2f( 1.0f, 1.0f );
				else
					m_scale = value;
			}
		}

		/// <summary>
		///   Scaled size.
		/// </summary>
		public Vector2f GlobalSize
		{
			get { return new Vector2f( m_size.X * m_scale.X, m_size.Y * m_scale.Y ); }
		}
		/// <summary>
		///   Scaled bounds of the transform.
		/// </summary>
		public FloatRect GlobalBounds
		{
			get
			{
				Vector2f pos  = Position,
				         size = GlobalSize;

				switch( Origin )
				{
					case Allignment.Top:
						pos.X -= size.X / 2.0f;
						break;
					case Allignment.TopRight:
						pos.X -= size.X;
						break;

					case Allignment.Left:
						pos.Y -= size.Y / 2.0f;
						break;
					case Allignment.Middle:
						pos -= size / 2.0f;
						break;
					case Allignment.Right:
						pos.X -= size.X;
						pos.Y -= size.Y / 2.0f;
						break;

					case Allignment.BottomLeft:
						pos.Y -= size.Y;
						break;
					case Allignment.Bottom:
						pos.X -= size.X / 2.0f;
						pos.Y -= size.Y;
						break;
					case Allignment.BottomRight:
						pos -= size;
						break;
				}

				return new FloatRect( pos, size ); 
			}
		}

		/// <summary>
		///   Gets the type names of components incompatible with this component type.
		/// </summary>
		/// <returns>
		///   The type names of components incompatible with this component type.
		/// </returns>
		protected override string[] GetIncompatibleComponents()
		{
			return new string[] { nameof( Button ), nameof( CheckBox ), nameof( FillBar ),
			                      nameof( TextBox ), nameof( UIClickable ), nameof( UILabel ),
								  nameof( UISprite ), nameof( UISpriteAnimator ), nameof( UISpriteArray ),
								  nameof( UITransform ) };
		}

		/// <summary>
		///   Sets the origin point while maintaining position.
		/// </summary>
		/// <param name="org">
		///   The new origin point.
		/// </param>
		public void SetOrigin( Allignment org )
		{
			if( org < 0 || (int)org >= Enum.GetNames( typeof( Allignment ) ).Length || org == Origin )
				return;

			FloatRect bounds = GlobalBounds;
			Vector2f  pos    = new Vector2f( bounds.Left,  bounds.Top ),
			          size   = new Vector2f( bounds.Width, bounds.Height );

			switch( org )
			{
				case Allignment.Top:
					pos.X -= size.X / 2.0f;
					break;
				case Allignment.TopRight:
					pos.X -= size.X;
					break;

				case Allignment.Left:
					pos.Y -= size.Y / 2.0f;
					break;
				case Allignment.Middle:
					pos -= size / 2.0f;
					break;
				case Allignment.Right:
					pos.X -= size.X;
					pos.Y -= size.Y / 2.0f;
					break;

				case Allignment.BottomLeft:
					pos.Y -= size.Y;
					break;
				case Allignment.Bottom:
					pos.X -= size.X / 2.0f;
					pos.Y -= size.Y;
					break;
				case Allignment.BottomRight:
					pos -= size;
					break;
			}

			Origin   = org;
			Position = pos;
		}

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
			if( !base.LoadFromStream( br ) )
				return false;

			try
			{
				Origin   = (Allignment)br.ReadInt32();
				Position = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Size     = new Vector2f( br.ReadSingle(), br.ReadSingle() );
				Scale    = new Vector2f( br.ReadSingle(), br.ReadSingle() );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed loading Transform: " + e.Message, false, LogType.Error );
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
			if( !base.SaveToStream( bw ) )
				return false;

			try
			{
				bw.Write( (int)Origin );
				bw.Write( Position.X ); bw.Write( Position.Y );
				bw.Write( Size.X );     bw.Write( Size.Y );
				bw.Write( Scale.X );    bw.Write( Scale.Y );
			}
			catch( Exception e )
			{
				return Logger.LogReturn( "Failed saving Transform: " + e.Message, false, LogType.Error );
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

			Origin     = 0;
			Scale      = new Vector2f( 1.0f, 1.0f );

			XmlElement position = element[ nameof( Position ) ],
			           size     = element[ nameof( Size ) ],
			           scale    = element[ nameof( Scale ) ];

			Vector2f? pos = Xml.ToVec2f( position ),
					  siz = Xml.ToVec2f( size );

			if( position == null )
				return Logger.LogReturn( "Failed loading Transform: No Position element.", false, LogType.Error );
			if( size == null )
				return Logger.LogReturn( "Failed loading Transform: No Size element.", false, LogType.Error );

			if( !pos.HasValue )
				return Logger.LogReturn( "Failed loading Transform: Unable to parse position.", false, LogType.Error );
			if( !siz.HasValue )
				return Logger.LogReturn( "Failed loading Transform: Unable to parse size.", false, LogType.Error );

			Position = pos.Value;
			Size     = siz.Value;
			
			if( element.HasAttribute( nameof( Origin ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Origin ) ), true, out Allignment a ) )
					return Logger.LogReturn( "Failed loading Transform: Unable to parse Anchor attribute.", false, LogType.Error );

				Origin = a;
			}

			if( scale != null )
			{
				Vector2f? scl = scale != null ? Xml.ToVec2f( scale ) : null;

				if( !scl.HasValue )
					return Logger.LogReturn( "Failed loading Transform: Unable to parse scale.", false, LogType.Error );

				Scale = scl.Value;
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

			sb.Append( "           " );
			sb.Append( nameof( Visible ) );
			sb.Append( "=\"" );
			sb.Append( Visible );
			sb.AppendLine( "\"" );

			sb.Append( "           " );
			sb.Append( nameof( Origin ) );
			sb.Append( "=\"" );
			sb.Append( Origin );
			sb.AppendLine( "\">" );

			sb.AppendLine( Xml.ToString( Position, nameof( Position ), 1 ) );
			sb.AppendLine( Xml.ToString( Size,     nameof( Size ), 1 ) );

			if( Scale.X != 1.0f || Scale.Y != 1.0f )
				sb.AppendLine( Xml.ToString( Scale, nameof( Scale ), 1 ) );

			sb.Append( "</" ); sb.Append( TypeName ); sb.AppendLine( ">" );

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
		public bool Equals( Transform other )
		{
			return Origin     == other.Origin &&
			       Position.Equals( other.Position ) &&
				   Size.Equals( other.Size ) &&
				   Scale.Equals( other.Scale );
		}
		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new Transform( this );
		}

		/// <summary>
		///   Gets immediate children of the root entity that contain a Transform and are within the
		///   given area.
		/// </summary>
		/// <param name="root">
		///   The root entity containing the entities to check.
		/// </param>
		/// <param name="area">
		///   The area in world pixel coordinates to check.
		/// </param>
		/// <returns>
		///   All immediate children of the root entity that contain a Transform and are within the
		///   given area.
		/// </returns>
		public static MiEntity[] GetEntitiesInArea( MiEntity root, FloatRect area )
		{
			List<MiEntity> ents = new List<MiEntity>();

			MiEntity[] children = root.GetChildrenWithComponent<Transform>();

			foreach( MiEntity c in children )
			{
				FloatRect b = c.GetComponent<Transform>().GlobalBounds;

				if( b.Left > area.Left + area.Width ||
					area.Left > b.Left + b.Width ||
					b.Top > area.Top + area.Height ||
					area.Top > b.Top + b.Height )
					continue;

				ents.Add( c );
			}

			return ents.ToArray();
		}
		/// <summary>
		///   Recursively gets all children of the root entity that contain a Transform and are
		///   within the given area.
		/// </summary>
		/// <param name="root">
		///   The root entity containing the entities to check.
		/// </param>
		/// <param name="area">
		///   The area in world pixel coordinates to check.
		/// </param>
		/// <returns>
		///   All children of the root entity that contain a Transform and are within the given area.
		/// </returns>
		public static MiEntity[] GetAllEntitiesInArea( MiEntity root, FloatRect area )
		{
			List<MiEntity> ents = new List<MiEntity>();

			MiEntity[] children = root.GetAllChildrenWithComponent<Transform>();

			foreach( MiEntity c in children )
			{
				FloatRect b = c.GetComponent<Transform>().GlobalBounds;

				if( b.Left > area.Left + area.Width ||
					area.Left > b.Left + b.Width ||
					b.Top > area.Top + area.Height ||
					area.Top > b.Top + b.Height )
					continue;

				ents.Add( c );
			}

			return ents.ToArray();
		}

		private Vector2f m_size, m_scale;
	}
}
