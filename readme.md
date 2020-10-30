# SharpGfx
A basic C# graphics library for use with SFML.Net.

## Dependencies
- SFML.Net `https://github.com/graphnode/SFML.Net.git`
- SFInput `https://github.com/BrokenShards/SFInput.git`
- SharpID `https://github.com/BrokenShards/SharpID.git`
- SharpLogger `https://github.com/BrokenShards/SharpLogger.git`
- SharpSerial `https://github.com/BrokenShards/SharpSerial.git`
- XInputDotNetPure `https://github.com/speps/XInputDotNet.git`

## NOTE
Please note `UI.Slider` is incomplete and not in a usable state, it is only included in the current release
as an artifact from merging SharpUI into SharpGfx.

## TODO
- Finish `UI.Slider`.
- Implement UI containers/layouting.
- Implement visual tests with user input for graphical classes.

## Changelog

### Version 0.5.0
- SharpUI has now been merged into SharpGfx and thus now depends on SFInput and XInputDotNetPure.
- The UI code is now all documented, implements `IEquatable<T>`, and has tests.

### Version 0.4.1
- Added previously missed `AnimatedSprite` test.
- Fixed an issue where `AnimatedSprite.Equals` was returning incorrect values.

### Version 0.4.0
- Added a sanity test project.
- Fixed issue where Animations were not serializing and deserializing correctly.
- Now all classes inherit from `System.IEquatable<T>` for easy equality comparison for the test project.
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
