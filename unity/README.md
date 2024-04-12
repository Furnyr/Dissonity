<div align="center">
	<br />
	<p>
		<a><img src="https://i.imgur.com/5elvldR.png" width="500"/></a>
	</p>
	<br />
</div>

# About

Dissonity allows you to easily create Discord activities inside Unity. The npm and Unity packages share data to provide functionality.

# Installation

1. Go to `Window` > `Package Manager` > `Add package from git URL`
2. Install the package from `https://github.com/Furnyr/Dissonity.git?path=/unity`

# Usage

> First of all, you will need a configured Node.js app using the [Dissonity npm package].

1. Right click in the hierarchy, `Dissonity` > `Discord Bridge` to create a Discord Bridge. You need this object in your scene to interact with the Embedded App SDK.
2. Use the methods from the static class `Dissonity.Api`
3. When your project is ready to be tested, make a WebGL build and put it inside your Node.js client
4. If you're implementing this package by yourself, you will need to follow the "Project configuration" guide below.

## Example script
```cs
using UnityEngine;
using static Dissonity.Api;

public class MyScript : MonoBehaviour
{
    async void Start()
    {
        string userId = await GetUserId();
        Debug.Log($"The user's id is {userId}");

        SubActivityInstanceParticipantsUpdate((data) => {
            Debug.Log("Received a participants update!");
        });
    }
}
```

# Links
- [GitHub](https://github.com/Furnyr/Dissonity)
- [NPM Package](https://www.npmjs.com/package/dissonity)
- [Unity Package Documentation](https://github.com/Furnyr/Dissonity/blob/main/unity/Documentation~/Dissonity.md)
- [Project configuration](https://github.com/Furnyr/Dissonity/blob/main/examples/projectConfiguration.md)

[Dissonity npm package]: https://www.npmjs.com/package/dissonity