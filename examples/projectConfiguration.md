# Project configuration

If you're not using the example Node.js project, you need to know how to configure the Unity Build to work with Dissonity and display correctly in the Discord client.

## Steps inside Unity
1. Set WebGL as the target build platform
2. Go to `Project Settings` > `Player` > `Resolution and Presentation`
3. Set the WebGL Template to Minimal
4. Build the game and place it in your Node.js project

## Steps inside Node.js

1. If you haven't already, make sure the iframe where your build will be has the id `dissonity-child`

2. Inside the build's index.html, in the canvas tag, set the style's width and height to `100vw` and `100vh` respectively:

```html
<canvas id="unity-canvas" style="width: 100vw; height: 100vh; ..."></canvas>
```

##

3. Inside the build's index.html, save the unity instance created by `createUnityInstance` inside a variable named `unityInstance`:

```js
var unityInstance;
createUnityInstance(document.querySelector("#unity-canvas"), {...})
.then(instance => {
    unityInstance = instance;
});
```

##

4. Now, in the parent index.html, set the `scrolling` attribute to "no" inside the child iframe tag:

```html
<iframe id="dissonity-child" src="nested/index.html" ... scrolling="no"></iframe>
```

##

5. In the parent index.html set the head's style tag to the following or something that achieves the same:

```css
<style>
  body {
    margin: 0px;
    background-color: #000000;
    padding: 0px;
  }
  iframe {
    display: block;
    border: 0px;
    height: 100vh;
    width: 100vw;
  }
</style>
```

# Other server configuration

This is not related to Dissonity but it's good that you're aware of it.

The server also needs to send the proper content headers for each of the build files. If you're using [Express](https://www.npmjs.com/package/express) you could modify the `setHeaders` field in your `<Express>.static` call:

```js
app.use(express.static(path.join(__dirname, "../client"), {
  setHeaders: (res, path) => {

    if (path.endsWith(".gz")) {
      res.appendHeader("Content-Encoding", "gzip");
    }

    if (path.endsWith(".br")) {
      res.appendHeader("Content-Encoding", "br");
    }

    if (path.endsWith(".wasm.gz") || path.endsWith(".wasm.br") || path.endsWith(".wasm")) {
      res.appendHeader("Content-Type", "application/wasm");
    }

    if (path.endsWith(".js.gz") || path.endsWith(".js.br") || path.endsWith(".js")) {
      res.appendHeader("Content-Type", "application/javascript");
    }

    if (path.endsWith(".data") || path.endsWith(".mem")) {
      res.appendHeader('Content-Type', 'application/octet-stream');
    }
  }
}));
```

##

Contratulations! Your build is now ready to use Dissonity and will display correctly in the Discord client.

> [!NOTE]  
> You only need to do this process once, except if you replace the build's index.html when testing a new build. Then you will need to reconfigure it. Unless you changed project settings that modify `index.html`, make sure you only replace the `Build` folder.

> [!TIP]
> If this process seems like a tedious task, you can start using the base Node.js project found in the examples folder. It handles most of it internally and allows you to just place your build files and run it.

