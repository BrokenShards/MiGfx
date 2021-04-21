# MiGfx
A basic C# graphics library for use with SFML.Net.

## Dependencies
- SFML.Net `https://github.com/SFML/SFML.Net.git`
- MiCore `https://github.com/BrokenShards/MiCore.git`
- MiInput `https://github.com/BrokenShards/MiInput.git`
- XInputDotNetPure `https://github.com/speps/XInputDotNet.git`

## TODO
- Implement UI containers/grouping/layouting.
- Create `Slider` component from `FillBar`.
- Create `Tilemap` component that uses a `SpriteArray` to draw tiles from a `Tileset`.
- Write tests for BoxRenderer and ShapeRenderer.

## Changelog

### Version 0.12.0
- Added new `TextCaret` component that provides a text cursor for text boxes `TextBox` now requires
  the `TextCaret` because of this.
- `TextListener` now contains the text cursor index and handles left, right, home and end movement
  keys along with the delete key.
- Added `BoxRenderer` and `ShapeRenderer` components for drawing boxes or regular shapes that can
  be coloured, textured and have an outline.
- Added new function `Physics.ShapeContainsPoint(Shape,Vector2f)` with variations for checking 
  if a point is inside a shape.
- Now modifying `Transform.Position` will also modify child entities that have a `Transform`. This
  behaviour was intended to start with as it allows children to follow their parent when moved.
- Added `SpriteArray.SpriteBounds` to easily get a rectangle containing all sprites.
- Now path strings in `FolderPaths` and `FilePaths` use the correct system directory separator.
- `FilePaths.Executable` and `FolderPaths.Executable` no longer modify directory separators and now
  properly handle and strip the `file:` URI.
- `FolderPaths.Executable` is now guaranteed to end with the system directory separator character.
- Updated MiCore to version 0.10.0.
- Updated MiInput to version 0.11.0.

### Version 0.11.0
- Added `BoxCollider` and `CircleCollider` components along with `Physics` class for managing
  geometry collision.
- Fixed issue where `Label` and `UILabel` allignment was not working.
- `AssetManager<T>` no longer requires the asset type to be disposable and no longer disposes of
  assets on removal. Use `DisposableAssetManager<T>` if you would like the old functionality that
  manages disposable assets.
- `Paths.ToWindows(string)` and `Paths.FromWindows(string)` have been moved to MiCore.
- Added `AnimationDB`, `TextStyleDB` and `TilesetDB` databases.
- Now `AnimationSet` just stores the IDs of its animations rather than entire objects and retrieves
  the actual animations from `AnimationDB`.
- Updated MiCore to version 0.9.0.
- Updated MiInput to version 0.10.0.

### Version 0.10.0
- `AssetManager<T>` now implements `IEnumerable<KeyValuePair<string, T>>` so loaded assets can be
  iterated through.
- Added `SpriteArray` component for drawing multiple sprites from the same texture.
- `FillBar` now contains a minimum value, maximum value and a current value rather than just one
  property for progress, because of this `FillBar.Progress` now uses these values to calculate the
  current progress.
- `FillBar` now uses the entities `SpriteArray` for drawing.
- Added `FillBar.LabelType` and `FillBar.Labeling` to more easily customise how the label displays
  the fill bar value.
- Added `FillBar.Create(string, RenderWindow, long, long, long, Color?, LabelType)` for easily
  constructing FillBar entities.
- Fixed issue where `ImageInfo` was flipping horizontal texture coordinates unintentionally.
- Added `ImageInfo.FlipHorizontal` and `ImageInfo.FlipVertical` that will flip the texture 
  coordinates of the vertex returned by `ImageInfo.GetVertex(uint, Transform)`.
- Added `Frame.Orientation`, `Frame.FlipHorizontal` and `Frame.FlipVertical` to allow animation of
  those properties.
- Error logs are now more consistant and concise.

### Version 0.9.0
- Changed SFML source to latest official SFML.Net repository.
- Removed `UIElement` and `UIManager` in favour of `MiComponent` and `MiEntity`. Now all `UIElement`
  classes inherit from `MiComponent` and are used in `MiEntity` objects.
- Added `FillBar` component that provides a bar that can be filled to show progress.
- Added `Clickable` component that responds to mouse hovering and mouse clicks within an entities
  `Transform`.
- Added `Selectable` component that allows entities to be selected along with the `Selector`
  component that allows an entity to manage which component is selected within its children.
- Added `TextListener` component that listens for text entered events and keeps track of them in
  `TextListener.EnteredText`.
- Replaced `Label.Center` with `Label.Allign` to give more text allignment options.
- `Button` and `CheckBox` now rely on `Sprite.Image` and calculate the texture rect for each state 
  from it every frame, because of this, they just hold a texure modifier color rather than an
  `ImageInfo` object.
- Added property `Assets.Disposed` to check if the internal asset managers have been disposed of and 
  `Assets.Recreate()` to recreate them if they have so they can be used agai.
- `AssetManager.RemoveAll()` and `Assets.RemoveAll()` have been renamed to `AssetManager.Clear()`
  and `Assets.Clear()` for consistency.
- Updated MiCore to version 0.5.0.
- Updated MiInput to version 0.9.0.

### Version 0.8.0
- Renamed to `MiGfx` and updated to use `MiCore` and `MiInput` libraries.

### Version 0.7.0
- All `IXmlLoadable` classes now have tests, properly check for existing attributes and return 
  errors when suitable and now use `nameof( Class )` for attribute and element names. Unimportant xml
  attributes and elements can now also be ommitted, for example `Transform.LoadFromXml(XmlElement)`
  no longer requires the `Scale` element to exist.
- Fixed issue where `Sprite.LoadFromXml(XmlElement)` was failing loading the `Transform`.
- Added `Xml` helper class for loading and saving common objects to and from xml strings.
- Removed `Slider` UI element for now.
- Updated SFInput to version 0.7.0.
- Updated SharpSerial to version 0.6.0.

### Version 0.6.0
- `TextBox` now implements the missing `LoadFromStream(BinaryReader)`, `SaveToStream(BinaryWriter)`
  and `Equals(TextBox)` methods.
- Now all graphical classes that implement `IBinarySerializable`/`BinarySerializable` also 
  implement `IXmlLoadable`. This enables modifiable objects or objects in development to be 
  implemented and easily changed in xml, then exported to a binary file for release.
- `SoundManager` now properly disposes of its music track on dispose.
- The test project now uses SharpTest for testing.
- Updated SFInput to version 0.6.0.
- Updated SharpSerial to version 0.5.0.

### Version 0.5.1
- Visual tests with user input have been implemented for graphical classes.
- Added a default texture for `UI.Slider`.
- Updated SFInput to version 0.5.2.
- Updated project to copy `XInputInterface.dll` to the build directory.

### Version 0.5.0
- SharpUI has now been merged into SharpGfx and thus now depends on SFInput and XInputDotNetPure.
- The UI code is now all documented, implements `IEquatable<T>`, and has tests.

### Version 0.4.1
- Added previously missed `AnimatedSprite` test.
- Fixed an issue where `AnimatedSprite.Equals` was returning incorrect values.

### Version 0.4.0
- Added a sanity test project.
- Fixed issue where Animations were not serializing and deserializing correctly.
- Now all classes inherit from `System.IEquatable<T>` for easy equality comparison for the test 
  project.
- Updated SharpID to version 0.3.0.
- Updated SharpLogger to version 0.3.1.
- Updated SharpSerial to version 0.3.0.

### Version 0.3.0
- SharpAsset has now been merged with SharpGfx.

### Version 0.2.1
- Now the xml documentation is built with the binary and is included in releases.
- Updated SharpAsset to version 0.2.1.
- Updated SharpID to version 0.2.1.
- Updated SharpLogger to version 0.1.1.
- Updated SharpSerial to version 0.2.1.

### Version 0.2.0
- Now SharpAsset is used for asset caching.

### Version 0.1.0
- Initial release.
