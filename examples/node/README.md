# 🎮 Dissonity v2 —  Example backend 🎮

>  Node.js + Colyseus + hiRPC

## Project structure

### Source folder (src)

Where the main code of the project is.

- `src/client/`: Where the activity files are.
- - `src/client/Unity/`: Build your Unity game here.
- - `src/client/index.ts`: Use hiRPC to send messages to C# here.

<br>

- `src/server/`: Where the server code is.
- - `src/server/index.ts`: Handles Discord authentication and networking.
- - `src/server/utils/`: Where the Colyseus code is. Notice rooms are defined in `index.ts`.


### Build folder (build)

Folder that will be generated after compiling TypeScript files.

### package.json scripts
- `colyseus`Generate Colyseus C# classes for Unity inside `_unity_colyseus`.
- `build`: Compile TypeScript files to JavaScript.
- `build:clean`: Compile TypeScript files to JavaScript and clean the build folder.
- `execute`: Run the server without compiling TypeScript.
- `start`: Build the project, then run the server.

## Multiplayer networking

This base project has a basic multiplayer implementation using [Colyseus](https://colyseus.io). If you want to use Colyseus for your networking you'll need to install the [Colyseus Unity SDK](https://docs.colyseus.io/getting-started/unity-sdk/) in your Unity project. Otherwise, simply set the `COLYSEUS` env variable to false.


> [!IMPORTANT]  
> Don't forget to set your env variables inside a .env file!

# Relevant links

- [Colyseus GitHub](https://github.com/colyseus/colyseus)
- [Colyseus Documentation](https://docs.colyseus.io)