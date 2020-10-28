# SharpGfx
A basic C# graphics library for use with SFML.Net.

## Dependencies
- SFML.Net `https://github.com/graphnode/SFML.Net.git`
- SharpID `https://github.com/BrokenShards/SharpID.git`
- SharpLogger `https://github.com/BrokenShards/SharpLogger.git`
- SharpSerial `https://github.com/BrokenShards/SharpSerial.git`

## Changelog

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
