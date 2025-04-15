using UnityEngine;
using Dissonity.Models.Mock;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dissonity
{
    /// <summary>
    /// <b> Warning: </b> This script is self-destroyed outside of the Unity Editor
    /// </summary>
    public class JavascriptMock : MonoBehaviour
    {
        #nullable enable annotations

        // Local Storage
        public List<MockStorageItem> _localStorage = new();

        // hiRPC App Hash
        public string _hiRpcAppHash = "4pp4cc3ssh4sh";

        // hiRPC Send to App
        public string _hiRpcSendJsonToUnity = "";
        public string _hiRpcChannel = "";

        // hiRPC Send to JS
        public bool _hiRpclogJsMessages = true;

        // Singleton
        public static JavascriptMock Singleton { get; private set; }

        //# RUNTIME - - - - -
        void Awake()
        {
            if (!Api.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            if (Singleton != null && Singleton != this) 
            {
                Destroy(gameObject); 
            }
            else 
            {
                Singleton = this; 
            }

            DontDestroyOnLoad(gameObject);
        }

        public void DispatchMessage()
        {
            var data = JsonConvert.DeserializeObject(_hiRpcSendJsonToUnity);

            Api.bridge!.MockHiRpcDispatch(_hiRpcChannel, data);
        }
    }
}