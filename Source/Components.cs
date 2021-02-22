////////////////////////////////////////////////////////////////////////////////
// Components.cs 
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
	///   Handles component related functionality.
	/// </summary>
	public static class Components
	{
		/// <summary>
		///   Register MiGfx components with the component register.
		/// </summary>
		/// <returns>
		///   True if all components were registered successfully, otherwise false.
		/// </returns>
		public static bool Register()
		{
			return ComponentRegister.Manager.Register<BoxCollider>() &&
			       ComponentRegister.Manager.Register<CircleCollider>() &&

				   ComponentRegister.Manager.Register<Clickable>() &&
				   ComponentRegister.Manager.Register<Label>() &&
				   ComponentRegister.Manager.Register<Selectable>() &&
				   ComponentRegister.Manager.Register<Selector>() &&
				   ComponentRegister.Manager.Register<Sprite>() &&
				   ComponentRegister.Manager.Register<SpriteAnimator>() &&
				   ComponentRegister.Manager.Register<SpriteArray>() &&
				   ComponentRegister.Manager.Register<TextListener>() &&
				   ComponentRegister.Manager.Register<Transform>() &&

				   ComponentRegister.Manager.Register<Button>() &&
				   ComponentRegister.Manager.Register<CheckBox>() &&
				   ComponentRegister.Manager.Register<FillBar>() &&
				   ComponentRegister.Manager.Register<TextBox>() &&
				   ComponentRegister.Manager.Register<UIClickable>() &&
				   ComponentRegister.Manager.Register<UILabel>() &&
				   ComponentRegister.Manager.Register<UISprite>() &&
				   ComponentRegister.Manager.Register<UISpriteAnimator>() &&
				   ComponentRegister.Manager.Register<UISpriteArray>() &&
				   ComponentRegister.Manager.Register<UITransform>();
		}
	}
}
