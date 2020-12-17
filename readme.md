# SharpGfx
A basic C# graphics library for use with SFML.Net.

## Dependencies
- SFML.Net `https://github.com/graphnode/SFML.Net.git`
- SFInput `https://github.com/BrokenShards/SFInput.git`
- SharpID `https://github.com/BrokenShards/SharpID.git`
- SharpLogger `https://github.com/BrokenShards/SharpLogger.git`
- SharpSerial `https://github.com/BrokenShards/SharpSerial.git`
- SharpTest `https://github.com/BrokenShards/SharpTest.git`
- XInputDotNetPure `https://github.com/speps/XInputDotNet.git`

## NOTE
Please note `UI.Slider` is incomplete and not in a usable state, it is only included in the current 
release as an artifact from merging SharpUI into SharpGfx.
Also SharpTest is only required for the test project, SharpGfx itself does not depend on it.

## TODO
- Implement UI containers/grouping/layouting.

## Changelog

### Version 0.7.0
- All `IXmlLoadable` classes now have tests, properly check for existing attributes and return 
  errors when suitable and now use `nameof( Var )` for attribute and element names. Unimportant xml
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
