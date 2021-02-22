////////////////////////////////////////////////////////////////////////////////
// TextStyleDB.cs 
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

using MiCore;

namespace MiGfx
{
	/// <summary>
	///   Database containing text styles.
	/// </summary>
	public class TextStyleDB : BinaryDatabase<TextStyle>, IBinarySerializable
	{
		/// <summary>
		///  Constructor.
		/// </summary>
		public TextStyleDB()
		:	base()
		{ }

		/// <summary>
		///   File path used for serialization.
		/// </summary>
		public override string FilePath
		{
			get
			{
				return FolderPaths.Databases + "textstyle.db";
			}
		}
	}
}
