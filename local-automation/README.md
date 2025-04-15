<div align="center">

  <h1><code>@dissonity/local-automation</code></h1>

  <strong>Utilities for local development.</strong>
</div>

## About

Scripts used for file management while developing Dissonity locally.

<ul>
    <li><b>move</b>: Moving files within the repository folder.</li>
    <li><b>pull</b>: Moving files from the repository to your local Unity project.</li>
    <li><b>sync</b>: Moving files from your local Unity project to the repository.</li>
</ul>

## Developing

You may need to install [pnpm](https://pnpm.io).

### 0. 🎮 Prepare a Unity project

### Unity version

While Dissonity supports Unity 2021.3 and later, please use the latest Unity version to develop new features. If you need to use older versions, be careful with `.meta` files and make sure you don't accidentally introduce obsolete APIs into the package.

### New project

Create a new Unity project, then, create a folder named `LocalDissonity` in your `Assets`.

You can additionally right-click your assets and select `Create > Dissonity > Development > Dev Dialogs Asset` to access C# local development utilities.

### 1. ⚒️ Install script dependencies

Run `pnpm i` to install dependencies.

### 2. 📋 Set the project path

Rename `.example.env` to `.env` and copy the path to your Unity project in it.


## Working on the Unity package

Run this command to copy the Unity package to your `LocalDissonity` folder:

```
pnpm pull
```

### Dependencies

Once the command runs, your console might have *a lot* of errors.

Open `LocalDissonity/package.json` and install the corresponding dependencies manually using `Install package by name` in the Package Manager.

### Saving your changes

Once you are done, use this command to bring back the changes from your local project to the repository:

```
pnpm sync
```

You may need to close Unity if the files are inaccessible.

## Working on TypeScript modules

Update the modules as needed (e.g., `hirpc`). Then, to move the files to your local Unity project you can run:

```
pnpm build_all_and_pull
```

To build everything before moving the files to your project.

> [!IMPORTANT]
> Remember using `pnpm sync` first if you have changes in your Unity project!

You can also manage each module individually:

```
# In the module folder
pnpm build
pnpm move

# In the local folder (here)
pnpm pull
```

After moving the JavaScript files to the Unity project, use `DevDialogs.asset` to regenerate the WebGL Template.