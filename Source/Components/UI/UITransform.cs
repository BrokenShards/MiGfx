////////////////////////////////////////////////////////////////////////////////
// UITransform.cs 
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
using System.Xml;
using MiCore;
using SFML.Graphics;
using SFML.System;

namespace MiGfx
{
	/// <summary>
	///   A 2D UI transformation.
	/// </summary>
	public class UITransform : Transform, IEquatable<UITransform>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public UITransform()
		:	base()
		{ }
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="t">
		///   The transform to copy from.
		/// </param>
		public UITransform( UITransform t )
		:	base( t )
		{ }
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
		/// <param name="anc">
		///   Anchor point.
		/// </param>
		public UITransform( Vector2f pos, Vector2f? size = null, Vector2f? scl = null, Allignment anc = 0 )
		:	base( pos, size, scl, anc )
		{ }

		/// <summary>
		///   Component type name.
		/// </summary>
		public override string TypeName
		{
			get { return nameof( UITransform ); }
		}

		/// <summary>
		///   Position in pixels.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   If parent or parent's window is invalid.
		/// </exception>
		public Vector2f PixelPosition
		{
			get
			{
				View view = Parent?.Window?.GetView();

				if( view == null )
					throw new InvalidOperationException();

				return view.Center - ( view.Size / 2.0f ) + new Vector2f( Position.X * view.Size.X, Position.Y * view.Size.Y );
			}
			set
			{
				View view = Parent?.Window?.GetView();

				if( view == null )
					throw new InvalidOperationException();

				Vector2f topleft = view.Center - ( view.Size / 2.0f );
				value -= topleft;

				if( value.X != 0.0f )
					value.X /= view.Size.X;
				if( value.Y != 0.0f )
					value.Y /= view.Size.Y;

				Position = value;
			}
		}
		/// <summary>
		///   Size in pixels.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   If parent or parent's window is invalid.
		/// </exception>
		public Vector2f PixelSize
		{
			get
			{
				View view = Parent?.Window?.GetView();

				if( view == null )
					throw new InvalidOperationException();

				return new Vector2f( Size.X * view.Size.X, Size.Y * view.Size.Y );
			}
			set
			{
				View view = Parent?.Window?.GetView();

				if( view == null )
					throw new InvalidOperationException();

				if( value.X != 0.0f )
					value.X /= view.Size.X;
				if( value.Y != 0.0f )
					value.Y /= view.Size.Y;

				Size = value;
			}
		}
		/// <summary>
		///   Scaled size in pixels.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   If parent or parent's window is invalid.
		/// </exception>
		public Vector2f GlobalPixelSize
		{
			get
			{
				View view = Parent?.Window?.GetView();

				if( view == null )
					throw new InvalidOperationException();

				return new Vector2f( Size.X * view.Size.X * Scale.X, Size.Y * view.Size.Y * Scale.Y );
			}
		}
		/// <summary>
		///   Global bounds in pixels.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   If parent or parent's window is invalid.
		/// </exception>
		public FloatRect GlobalPixelBounds
		{
			get
			{
				View view = Parent?.Window?.GetView();

				if( view == null )
					throw new InvalidOperationException();

				Vector2f topleft = view.Center - ( view.Size / 2.0f );
				FloatRect bounds = GlobalBounds;

				bounds.Left    = topleft.X + ( bounds.Left * view.Size.X );
				bounds.Top     = topleft.Y + ( bounds.Top  * view.Size.Y );
				bounds.Width  *= view.Size.X;
				bounds.Height *= view.Size.Y;

				return bounds;
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
			return new string[] { nameof( Clickable ), nameof( Label ), nameof( Sprite ), 
			                      nameof( SpriteAnimator ), nameof( SpriteArray ), nameof( Transform ) };
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

			if( position == null )
				return Logger.LogReturn( "Failed loading UITransform: No Position element.", false, LogType.Error );
			if( size == null )
				return Logger.LogReturn( "Failed loading UITransform: No Size element.", false, LogType.Error );

			{
				if( !position.HasAttribute( nameof( Vector2f.X ) ) ||
					!position.HasAttribute( nameof( Vector2f.Y ) ) )
					return Logger.LogReturn( "Failed loading UITransform: Position element invalid.", false, LogType.Error );

				string x = position.GetAttribute( nameof( Vector2f.X ) ).Trim().ToLower(),
				       y = position.GetAttribute( nameof( Vector2f.Y ) ).Trim().ToLower();

				bool xpix = x.Length > 2 && x.Substring( x.Length - 2 ) == "px",
				     ypix = y.Length > 2 && y.Substring( y.Length - 2 ) == "px";

				if( xpix )
					x = x.Substring( 0, x.Length - 2 ).Trim();
				if( ypix )
					y = y.Substring( 0, y.Length - 2 ).Trim();

				if( !float.TryParse( x, out float xval ) )
					return Logger.LogReturn( "Failed loading UITransform: Unable to parse Position.X attribute.", false, LogType.Error );
				if( !float.TryParse( y, out float yval ) )
					return Logger.LogReturn( "Failed loading UITransform: Unable to parse Position.Y attribute.", false, LogType.Error );

				Position = new Vector2f( xval, yval );

				if( xpix || ypix )
				{
					Vector2f pixpos = PixelPosition;

					if( xpix )
						pixpos.X = xval;
					if( ypix )
						pixpos.Y = yval;

					PixelPosition = pixpos;
				}
			}
			{
				if( !size.HasAttribute( nameof( Vector2f.X ) ) ||
					!size.HasAttribute( nameof( Vector2f.Y ) ) )
					return Logger.LogReturn( "Failed loading UITransform: Size element invalid.", false, LogType.Error );

				string x = size.GetAttribute( nameof( Vector2f.X ) ).Trim().ToLower(),
					   y = size.GetAttribute( nameof( Vector2f.Y ) ).Trim().ToLower();

				bool xpix = x.Length > 2 && x.Substring( x.Length - 2 ) == "px",
					 ypix = y.Length > 2 && y.Substring( y.Length - 2 ) == "px";

				if( xpix )
					x = x.Substring( 0, x.Length - 2 ).Trim();
				if( ypix )
					y = y.Substring( 0, y.Length - 2 ).Trim();

				if( !float.TryParse( x, out float xval ) )
					return Logger.LogReturn( "Failed loading UITransform: Unable to parse Size.X attribute.", false, LogType.Error );
				if( !float.TryParse( y, out float yval ) )
					return Logger.LogReturn( "Failed loading UITransform: Unable to parse Size.Y attribute.", false, LogType.Error );

				Size = new Vector2f( xval, yval );

				if( xpix || ypix )
				{
					Vector2f pixsize = PixelSize;

					if( xpix )
						pixsize.X = xval;
					if( ypix )
						pixsize.Y = yval;

					PixelSize = pixsize;
				}
			}
			
			if( element.HasAttribute( nameof( Origin ) ) )
			{
				if( !Enum.TryParse( element.GetAttribute( nameof( Origin ) ), true, out Allignment a ) )
					return Logger.LogReturn( "Failed loading UITransform: Unable to parse Anchor attribute.", false, LogType.Error );

				Origin = a;
			}

			if( scale != null )
			{
				Vector2f? scl = scale != null ? Xml.ToVec2f( scale ) : null;

				if( !scl.HasValue )
					return Logger.LogReturn( "Failed loading UITransform: Unable to parse scale.", false, LogType.Error );

				Scale = scl.Value;
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
		public bool Equals( UITransform other )
		{
			return base.Equals( other );
		}

		/// <summary>
		///   Clones this object.
		/// </summary>
		/// <returns>
		///   A clone of this object.
		/// </returns>
		public override object Clone()
		{
			return new UITransform( this );
		}
	}
}
