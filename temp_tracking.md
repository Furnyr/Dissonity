- RpcErrorCode
- Activity (+Builder, Mock) now has details_url and state_url (original 2.1.0)
- ActivityAssets (+Mock) now has details_url and state_url (original 2.1.0)
- API: GetUser (needs testing, might be privated again in favor of SyncedUser)
- Updated links to the new format of the Discord documentation
- Updated TS config in hirpc and hirpc-interface
- Improved hirpc domain checking
- Removed hash checking for official methods
- Removed checks in the hirpc kit (needs testing)
- Updated workflow versions
- Now using pnpm v11 and Node.js v26
- Upgraded hirpc and local-automation to ESM (needs testing)
- Bumped versions
- Updated kit types
- Added notice regarding relationships.read
- Removed old #utils implementation from hiRPC
- Removed old versions folder from hiRPC
- Upgraded to ESNext instead of ES2022
- Removed everything related to the .proxy prefix
- hiRPC imports need file extensions to use NodeNext in the tests

There's some functionality that I cannot maintain if there's no official documentation
Changelogs are TODOes