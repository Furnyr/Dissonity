using System;
using System.Collections;
using System.Threading.Tasks;
using Dissonity.Models;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Dissonity.Api;
using static Dissonity.I_UnitApi;

public class DoubleInitialize
{
    [UnityTest, Order(1)]
    public IEnumerator DoubleInitializeTest()
    {
        RawOverrideConfiguration(new Dissonity.I_UserData {
            DisableDissonityInfoLogs = true,
            OauthScopes = new string[] { OauthScope.Identify, OauthScope.RpcActivitiesWrite, OauthScope.RpcVoiceRead, OauthScope.Guilds, OauthScope.GuildsMembersRead }
        });

        Task task = Initialize();

        yield return new WaitUntil(() => task.IsCompleted);

        try {
            Initialize();
        }
        catch(InvalidOperationException) {
            yield break;
        }

        Assert.AreEqual("Initialize throws exception", "Initialize didn't throw exception");
    }
}
