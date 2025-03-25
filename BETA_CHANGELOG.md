This file will document changes made during the beta phase.

---

# 2025-03-26 - beta progress 2

This version includes bug fixes, mock improvements and utilities for local Dissonity development.

## API

### Added

- `Api.ApplicationAccessHash` property.
- `@JavascriptMock` object.
- `DevDialogs` asset, used for local Dissonity development.

### Changed

- hiRPC messages can be mocked.
- Local storage can be mocked.
- Relative HTTPS Proxy requests can be mocked if the URL mappings are defined in the configuration file.

### Fixed

- Some mock implementations didn't work when disabling Dissonity info logs.

## Internal

### Added

- Unit tests.

### Changed

- App hash is now stored in the API.
- Local development folder is now `LocalDissonity`.

## UI

### Added

- Javascript Mock interface.

## hiRPC

### Changed

- The instance is automatically set to `window.dso_hirpc` on instantiation.
- The instance is a frozen object.
- If an instance already exists on the window object, it is returned when attempting to create another instance.

## hiRPC Interface

## Changed

- Plugin methods now start with `Dso`.

## Fixed

- Loader path is now correctly set in production builds.
- Fixed check to detect whether the game is nested.

## hiRPC Kit

### Added

- Support for the new object freezing process.

## CI

### Added

- `local-automation` folder, used for file management during local development.

### Documentation

- Updated website structure.
- Improved API reference site.

# 2025-03-02 - beta progress 1

First beta version. The beta phase is expected to be more stable than alpha, but you should still watch out for breaking changes!

- hiRPC functionality is ready and will be documented soon
- Public documentation will be published soon to https://dissonity.dev
- Try the v2 example backend at [`examples/node`](examples/node/)

From now on, the changelog will be split into

- **API**: C# API Interface
- **Internal**: C# Internal functionality
- **UI**: Unity package interface
- **hiRPC**: Core bridge
- **hiRPC Interface**: hiRPC Unity implementation
- **hiRPC Kit**: hiRPC Utility package
- **Examples**: Sample code
- **Documentation**: References / website

And other sections such as **Notes** or **Next**.

## API

### Added

- `Api.AccessToken` property.
- `Api.LocalStorage` subclass.

### Changed

- `HiRpc` methods now take the hiRPC channel as the first argument.

## Internal

### Fixed

- Generated folders are now deleted with the uninstaller.
- The dialog file is now regenerated after deletion.
- `NoResponse` RPC commands now raise a `CommandException` properly.
- Null values are not sent when using `Commands.SetActivity`.
- Enums are properly serialized as ints.
- `Commands.SetConfig` uses its corresponding command string.

### Changed

- The Resources folder is no longer used internally to load assets.

## hiRPC

### Added

- `SessionStorage["dso_instance_id"]` is used to clean up past storage items.

### Changed

- A channel is now required to add an app listener.
- A hash can't be requested before calling `load`.

### Fixed

- BigInts are properly serialized.
- Requesting a hash doesn't crash the process.
- Be able to send messages to the parent's parent if necessary.

## hiRPC Interface

### Fixed

- Use the base url for file paths.
- Use the correct property to send data to JS

## hiRPC Kit

### Added

- `loadIframe` function.
- RPC storage cleanup.

## Examples (Node)

The example Node.js backend has been updated. It's still pending a user authentication implementation.