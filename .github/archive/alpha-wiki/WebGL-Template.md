# 🗺️ WebGL Template 🗺️

Unity doesn't allow packages to include custom WebGL templates natively. Hence, Dissonity uses a custom implementation to install the WebGL template in the developer's project.

---

1. The code runs in a **OnPostprocessAllAssets** method from a child of the **AssetPostprocessor** class

2. Checks if the folders to `Assets/WebGLTemplates/Dissonity/Files/Bridge` exist.

3. Checks if the version.json file of the template matches the package.json version.

4. If every folder existed and the version matches, the template is considered generated and sets a static boolean to not check for the template again in this session.

3. If any folder didn't exist, or the template version is outdated, the template is loaded from the `Resources` folder and added to the project

After adding the WebGL template, a log will be sent to the console so the developer knows that Dissonity added something to their project.