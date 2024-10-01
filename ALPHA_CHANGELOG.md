This file will document changes made during the alpha phase.

# 2024-10-01 - alpha progress 1

## Wiki

- Added documentation for the client [POST request](https://github.com/Furnyr/Dissonity/wiki/Design)
- Added [BridgeLib](https://github.com/Furnyr/Dissonity/wiki#-terms-) to terms
- Added [Testing new changes](https://github.com/Furnyr/Dissonity/wiki#testing-new-changes) documentation
- Added [AuthorizationException](https://github.com/Furnyr/Dissonity/wiki/Exceptions#authorizationexception) documentation
- Added [SdkVersion](https://github.com/Furnyr/Dissonity/wiki/Design#api-properties) documentation

## Example Node.js server

- Removed "<>" from example .env
- Added reference Node.js version to package.json
- Updated dependencies

## BridgeLib

- RpcBridge sets closed state when authorization is rejected

- Removed command state limitation

- Merged initialization listeners into InitialBridgeListener

## Dissonity (internal)

- AuthorizationException is now thrown when the RpcBridge is in a closed state

- Mock should log warnings when attempting to run commands with the wrong scopes (or platform)

- Minor fix when consuming a command

- Build post-processor adds sdk version to RpcBridge

## Dissonity (API)

- Added Api.SdkVersion and Api.MobileAppVersion (https://github.com/discord/embedded-app-sdk/pull/262)

- Added AuthorizationException

- Added MobileAppVersion to mock query

- Exposed Ready property

## Testing notes

Api.MobileAppVersion should be null in desktop and a string in the format "number.number" in the mobile client.

The mock logs should be warnings when the command isn't allowed and normal logs otherwise.

## Next

The next thing I'm working on is implementing the features that were recently released.