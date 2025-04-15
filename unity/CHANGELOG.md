# Changelog

All notable changes to this package will be documented in this file.

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