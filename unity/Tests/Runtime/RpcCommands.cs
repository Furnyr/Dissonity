using System.Collections;
using Dissonity.Models;
using Dissonity.Models.Builders;
using UnityEngine;
using UnityEngine.TestTools;
using static Dissonity.Api;

public class RpcCommands
{
    [UnityTest]
    public IEnumerator RpcCommandsTest()
    {
        var captureLogTask = Commands.CaptureLog(ConsoleLevel.Debug, "");
        yield return new WaitUntil(() => captureLogTask.IsCompleted);

        var hwaTask = Commands.EncourageHardwareAcceleration();
        yield return new WaitUntil(() => hwaTask.IsCompleted); 

        var getChannelTask = Commands.GetChannel(0);
        yield return new WaitUntil(() => getChannelTask.IsCompleted); 

        var getChannelPermsTask = Commands.GetChannelPermissions();
        yield return new WaitUntil(() => getChannelPermsTask.IsCompleted); 

        var getEntTask = Commands.GetEntitlements();
        yield return new WaitUntil(() => getEntTask.IsCompleted); 

        var getParticipantsTask = Commands.GetInstanceConnectedParticipants();
        yield return new WaitUntil(() => getParticipantsTask.IsCompleted); 

        var getPlatformBhTask = Commands.GetPlatformBehaviors();
        yield return new WaitUntil(() => getPlatformBhTask.IsCompleted); 

        var getSkusTask = Commands.GetSkus();
        yield return new WaitUntil(() => getSkusTask.IsCompleted); 

        var initImgUpTask = Commands.InitiateImageUpload();
        yield return new WaitUntil(() => initImgUpTask.IsCompleted); 

        var openLinkTask = Commands.OpenExternalLink("");
        yield return new WaitUntil(() => openLinkTask.IsCompleted); 

        var openInviteTask = Commands.OpenInviteDialog();
        yield return new WaitUntil(() => openInviteTask.IsCompleted); 

        var openShareMomentTask = Commands.OpenShareMomentDialog("");
        yield return new WaitUntil(() => openShareMomentTask.IsCompleted); 

        var setActTask = Commands.SetActivity(new ActivityBuilder());
        yield return new WaitUntil(() => setActTask.IsCompleted); 

        var setConfigTask = Commands.SetConfig(true);
        yield return new WaitUntil(() => setConfigTask.IsCompleted); 

        var setOriLockStateTask = Commands.SetOrientationLockState(OrientationLockStateType.Landscape);
        yield return new WaitUntil(() => setOriLockStateTask.IsCompleted); 

        var shareLinkTask = Commands.ShareLink("");
        yield return new WaitUntil(() => shareLinkTask.IsCompleted); 

        var startPurchaseTask = Commands.StartPurchase(0);
        yield return new WaitUntil(() => startPurchaseTask.IsCompleted); 

        var getLocaleTask = Commands.UserSettingsGetLocale();
        yield return new WaitUntil(() => getLocaleTask.IsCompleted); 
    }
}
