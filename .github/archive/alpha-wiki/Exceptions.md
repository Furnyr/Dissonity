# 💥 Exceptions 💥

Exceptions thrown by Dissonity are always marked in the method's comment summary. There are only 2 custom exceptions:

### CommandException

- Thrown by Api.Commands methods when the Discord client sends an error response for a command. You can access `Message` and `Code`.

### OutsideDiscordException

- Thrown by the Api.Initialize method when the query parameters are not defined, or it's running inside the Unity editor without a mock.