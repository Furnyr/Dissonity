# 📏 Utils 📏

The word "utils" refers to useful extra functionality that Dissonity offers:

<br>

## Dissonity.Utils

- **DissonityLog**(object): Passes the object to Unity's Debug.Log, but adding "[Dissonity] " before. Its main use case is being able to filter by the word "Dissonity" in the console while testing the activity in Discord. There's also **DissonityLogWarning** and **DissonityLogError**.

```cs
void DissonityLog(object message)
```

- **GetMockSnowflake**: Returns a unique snowflake id for testing purposes.

```cs
long GetMockSnowflake()
```

<br>

## Api.Proxy

This static class offers methods to make HTTP(s) request through the Discord proxy. These methods are not available during [API mock mode](https://github.com/Furnyr/Dissonity/wiki/Mock#api-mock-mode).

```cs
// Example signature
Task<TJsonResponse> HttpsPostRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload)

// Example call
MyResponse response = await Proxy.HttpsPostRequest<MyRequest, MyResponse>("/api/example", new());
```