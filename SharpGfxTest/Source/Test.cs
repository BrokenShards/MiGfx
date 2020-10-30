////////////////////////////////////////////////////////////////////////////////
// Test.cs 
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
using SharpLogger;

namespace SharpGfxTest
{
	partial class Test
	{
		static void Main( string[] args )
		{
			Logger.LogToConsole = true;
			Logger.LogToFile    = false;

			Logger.Log( "Running SharpGfx Tests..." );

			bool result = true;

			if( !AssetTests() )
				result = false;
			if( !GraphicsTests() )
				result = false;
			if( !UITests() )
				result = false;

			Logger.Log( result ? "All tests ran successfully." : "One or more tests failed." );

			Logger.Log( "Press enter to exit." );
			Console.ReadLine();
		}
	}
}
