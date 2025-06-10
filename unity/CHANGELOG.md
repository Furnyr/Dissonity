# Changelog

All notable changes to this package will be documented in this file.

## [2.1.0] - 2025-06-10

## Added

- LazyHiRpcLoad option in the Advanced Configuration
- Api.Commands.GetRelationships (requires Discord approval)
- Api.Subscribe.RelationshipUpdate (requires Discord approval)
- Models.Relationship
- Relationships section in the Discord Mock

## Changed

- Minor performance improvements related to the hiRPC Interface

## Fixed

- The game doesn't turn into a black screen when loading the index.html

## [2.0.1] - 2025-05-30

### Added

- Added link.xml to the Dissonity folder. This fixes stripping issues in some Unity versions.

### Changed

- Updated the mock return data of `Api.Commands.ShareLink`.

### Fixed

- `Api.GuildId` is now nullable, since it has a null value in DMs.

## [2.0.0] - 2025-04-15

Compared to `v2.0.0-beta.2`:

### BREAKING CHANGES

- Removed referrerId from ShareLink command.
- Added HTTP DELETE body to Proxy.
- Renamed `Timeframe` to `ActivityTimestamps`.

### Added

- Error propagation from JS to C# (ExternalException).

### Changed

- hiRPC mock messages need initialization.
- Improved absolute url check.
- Updated ShareLink parameters and response data.
- Updated GetChannel response data.