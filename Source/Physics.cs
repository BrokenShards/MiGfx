﻿////////////////////////////////////////////////////////////////////////////////
// Physics.cs 
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

using SFML.Graphics;
using SFML.System;

namespace MiGfx
{
	/// <summary>
	///   Contains physics and geometric collision functionality.
	/// </summary>
	public static class Physics
	{
		/// <summary>
		///   The force and direction of gravity.
		/// </summary>
		public static Vector2f Gravity
		{
			get; set;
		} = new Vector2f( 0, -9.807f );

		/// <summary>
		///   Checks if a line intersects another line.
		/// </summary>
		/// <param name="l1p1">
		///   Line one first point.
		/// </param>
		/// <param name="l1p2">
		///   Line one second point.
		/// </param>
		/// <param name="l2p1">
		///   Line two first point.
		/// </param>
		/// <param name="l2p2">
		///   Line two second point.
		/// </param>
		/// <returns>
		///   True if the lines intersect.
		/// </returns>
		public static bool LineIntersectsLine( Vector2f l1p1, Vector2f l1p2, Vector2f l2p1, Vector2f l2p2 )
		{
			float q = ( l1p1.Y - l2p1.Y ) * ( l2p2.X - l2p1.X ) - ( l1p1.X - l2p1.X ) * ( l2p2.Y - l2p1.Y );
			float d = ( l1p2.X - l1p1.X ) * ( l2p2.Y - l2p1.Y ) - ( l1p2.Y - l1p1.Y ) * ( l2p2.X - l2p1.X );

			if( d == 0 )
			{
				return false;
			}

			float r = q / d;

			q = ( l1p1.Y - l2p1.Y ) * ( l1p2.X - l1p1.X ) - ( l1p1.X - l2p1.X ) * ( l1p2.Y - l1p1.Y );
			float s = q / d;

			if( r < 0 || r > 1 || s < 0 || s > 1 )
			{
				return false;
			}

			return true;
		}
		/// <summary>
		///   Checks if a line intersects a rectangle.
		/// </summary>
		/// <param name="p1">
		///   The first point of the line.
		/// </param>
		/// <param name="p2">
		///   The second point of the line.
		/// </param>
		/// <param name="r">
		///   The rectangle.
		/// </param>
		/// <returns>
		///   True if the line intersects the rectangle and false otherwise.
		/// </returns>
		public static bool LineIntersectsRect( Vector2f p1, Vector2f p2, FloatRect r )
		{
			return LineIntersectsLine( p1, p2, new Vector2f( r.Left, r.Top ), new Vector2f( r.Left + r.Width, r.Top ) ) ||
				   LineIntersectsLine( p1, p2, new Vector2f( r.Left + r.Width, r.Top ), new Vector2f( r.Left + r.Width, r.Top + r.Height ) ) ||
				   LineIntersectsLine( p1, p2, new Vector2f( r.Left + r.Width, r.Top + r.Height ), new Vector2f( r.Left, r.Top + r.Height ) ) ||
				   LineIntersectsLine( p1, p2, new Vector2f( r.Left, r.Top + r.Height ), new Vector2f( r.Left, r.Top ) ) ||
				   ( r.Contains( p1.X, p1.Y ) && r.Contains( p2.X, p2.Y ) );
		}
		/// <summary>
		///   Checks if the given line intersects the given circle.
		/// </summary>
		/// <param name="cpos">
		///   Circle center position.
		/// </param>
		/// <param name="radius">
		///   Circle radius.
		/// </param>
		/// <param name="l1">
		///   The first point of the line.
		/// </param>
		/// <param name="l2">
		///   The second point of the line.
		/// </param>
		/// <returns>
		///   True if the line intersects the circle and false otherwise.
		/// </returns>
		public static bool LineIntersectsCircle( Vector2f cpos, float radius, Vector2f l1, Vector2f l2 )
		{
			return LineCircleIntersections( cpos, radius, l1, l2, out _, out _ ) > 0;
		}
		/// <summary>
		///   Gets the amount of points the line intersects the circle and outs the point(s) of contact.
		/// </summary>
		/// <param name="cpos">
		///   Circle center position.
		/// </param>
		/// <param name="radius">
		///   Circle radius.
		/// </param>
		/// <param name="l1">
		///   First point of the line.
		/// </param>
		/// <param name="l2">
		///   Second point of the line.
		/// </param>
		/// <param name="int1">
		///   The first intersection point or (float.NaN, float.NaN) if the line does not intersect
		///   the circle.
		/// </param>
		/// <param name="int2">
		///   The second intersection point or (float.NaN, float.NaN) if the line does not intersect
		///   the circle twice.
		/// </param>
		/// <returns>
		///   The amount of intersection points between the circle and line.
		/// </returns>
		public static int LineCircleIntersections( Vector2f cpos, float radius, Vector2f l1, Vector2f l2,
			out Vector2f int1, out Vector2f int2 )
		{
			float dx, dy, A, B, C, det, t;

			dx = l2.X - l1.X;
			dy = l2.Y - l1.Y;

			A = dx * dx + dy * dy;
			B = 2 * ( dx * ( l1.X - cpos.X ) + dy * ( l1.Y - cpos.Y ) );
			C = ( l1.X - cpos.X ) * ( l1.X - cpos.X ) +
				( l1.Y - cpos.Y ) * ( l1.Y - cpos.Y ) -
				radius * radius;

			det = B * B - 4 * A * C;
			if( ( A <= 0.0000001 ) || ( det < 0 ) )
			{
				// No real solutions.
				int1 = new Vector2f( float.NaN, float.NaN );
				int2 = new Vector2f( float.NaN, float.NaN );
				return 0;
			}
			else if( det == 0 )
			{
				// One solution.
				t = -B / ( 2 * A );
				int1 =
					new Vector2f( l1.X + t * dx, l1.Y + t * dy );
				int2 = new Vector2f( float.NaN, float.NaN );
				return 1;
			}
			else
			{
				// Two solutions.
				t = (float)( ( -B + Math.Sqrt( det ) ) / ( 2 * A ) );
				int1 =
					new Vector2f( l1.X + t * dx, l1.Y + t * dy );
				t = (float)( ( -B - Math.Sqrt( det ) ) / ( 2 * A ) );
				int2 =
					new Vector2f( l1.X + t * dx, l1.Y + t * dy );
				return 2;
			}
		}

		/// <summary>
		///   Checks if a rectangle intersects a circle.
		/// </summary>
		/// <param name="rect">
		///   The rectangle.
		/// </param>
		/// <param name="center">
		///   The center of the circle.
		/// </param>
		/// <param name="radius">
		///   The radius of the circle.
		/// </param>
		/// <returns></returns>
		public static bool RectIntersectsCircle( FloatRect rect, Vector2f center, float radius )
		{
			return rect.Contains( center.X, center.Y ) ||
				   LineIntersectsCircle( center, radius, new Vector2f( rect.Left, rect.Top ),
								         new Vector2f( rect.Left + rect.Width, rect.Top ) ) ||
				   LineIntersectsCircle( center, radius, new Vector2f( rect.Left + rect.Width, rect.Top ),
								         new Vector2f( rect.Left + rect.Width, rect.Top + rect.Height ) ) ||
				   LineIntersectsCircle( center, radius, new Vector2f( rect.Left + rect.Width, rect.Top + rect.Height ),
								         new Vector2f( rect.Left, rect.Top + rect.Height ) ) ||
				   LineIntersectsCircle( center, radius, new Vector2f( rect.Left, rect.Top + rect.Height ),
								         new Vector2f( rect.Left, rect.Top ) );
		}
		/// <summary>
		///   Checks if a given point is within a circle.
		/// </summary>
		/// <param name="center">
		///   The center of the circle.
		/// </param>
		/// <param name="radius">
		///   The radius of the circle.
		/// </param>
		/// <param name="point">
		///   The point to check.
		/// </param>
		/// <returns>
		///   True if the point is inside the circle and false otherwise.
		/// </returns>
		public static bool CircleContainsPoint( Vector2f center, float radius, Vector2f point )
		{
			Vector2f diff = point - center;

			double len = Math.Sqrt( Math.Pow( diff.X, 2 ) + Math.Pow( diff.Y, 2 ) );
			return len <= radius;
		}
	}
}
