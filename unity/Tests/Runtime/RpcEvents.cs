using System.Collections;
using System.Threading.Tasks;
using Dissonity;
using UnityEngine;
using UnityEngine.TestTools;
using static Dissonity.Api;

public class RpcEvents
{
    [UnityTest]
    public IEnumerator RpcEventsTest()
    {
        DiscordMock mock = GameObject.FindAnyObjectByType<DiscordMock>();

        // ActivityInstanceParticipantsUpdate
        TaskCompletionSource<bool> ActivityInstanceParticipantsUpdateTask = new();
        _ = Subscribe.ActivityInstanceParticipantsUpdate((d) =>
        {
            ActivityInstanceParticipantsUpdateTask.SetResult(true);
        });
        mock.ActivityInstanceParticipantsUpdate();
        yield return new WaitUntil(() => ActivityInstanceParticipantsUpdateTask.Task.IsCompleted);


        // ActivityLayoutModeUpdate
        TaskCompletionSource<bool> ActivityLayoutModeUpdateTask = new();
        _ = Subscribe.ActivityLayoutModeUpdate((d) =>
        {
            ActivityLayoutModeUpdateTask.SetResult(true);
        });
        mock.ActivityLayoutModeUpdate();
        yield return new WaitUntil(() => ActivityLayoutModeUpdateTask.Task.IsCompleted);


        // CurrentGuildMemberUpdate
        TaskCompletionSource<bool> CurrentGuildMemberUpdateTask = new();
        _ = Subscribe.CurrentGuildMemberUpdate(0, (d) =>
        {
            CurrentGuildMemberUpdateTask.SetResult(true);
        });
        mock.CurrentGuildMemberUpdate();
        yield return new WaitUntil(() => CurrentGuildMemberUpdateTask.Task.IsCompleted);


        // CurrentUserUpdate
        TaskCompletionSource<bool> CurrentUserUpdateTask = new();
        _ = Subscribe.CurrentUserUpdate((d) =>
        {
            CurrentUserUpdateTask.SetResult(true);
        });
        mock.CurrentUserUpdate();
        yield return new WaitUntil(() => CurrentUserUpdateTask.Task.IsCompleted);


        // EntitlementCreate
        TaskCompletionSource<bool> EntitlementCreateTask = new();
        _ = Subscribe.EntitlementCreate((d) =>
        {
            EntitlementCreateTask.SetResult(true);
        });
        mock._entitlements.Add(new()
        {
            Id = 0
        });
        mock.EntitlementCreate(0);
        yield return new WaitUntil(() => EntitlementCreateTask.Task.IsCompleted);


        // OrientationUpdate
        TaskCompletionSource<bool> OrientationUpdateTask = new();
        _ = Subscribe.OrientationUpdate((d) =>
        {
            OrientationUpdateTask.SetResult(true);
        });
        mock.OrientationUpdate();
        yield return new WaitUntil(() => OrientationUpdateTask.Task.IsCompleted);


        // SpeakingStart
        TaskCompletionSource<bool> SpeakingStartTask = new();
        _ = Subscribe.SpeakingStart(0, (d) =>
        {
            SpeakingStartTask.SetResult(true);
        });
        mock.SpeakingStart();
        yield return new WaitUntil(() => SpeakingStartTask.Task.IsCompleted);


        // SpeakingStop
        TaskCompletionSource<bool> SpeakingStopTask = new();
        _ = Subscribe.SpeakingStop(0, (d) =>
        {
            SpeakingStopTask.SetResult(true);
        });
        mock.SpeakingStop();
        yield return new WaitUntil(() => SpeakingStopTask.Task.IsCompleted);


        // ThermalStateUpdate
        TaskCompletionSource<bool> ThermalStateUpdateTask = new();
        _ = Subscribe.ThermalStateUpdate((d) =>
        {
            ThermalStateUpdateTask.SetResult(true);
        });
        mock.ThermalStateUpdate();
        yield return new WaitUntil(() => ThermalStateUpdateTask.Task.IsCompleted);


        // VoiceStateUpdate
        TaskCompletionSource<bool> VoiceStateUpdateTask = new();
        _ = Subscribe.VoiceStateUpdate(0, (d) =>
        {
            VoiceStateUpdateTask.SetResult(true);
        });
        mock.VoiceStateUpdate();
        yield return new WaitUntil(() => VoiceStateUpdateTask.Task.IsCompleted);


        // RelationshipUpdate
        TaskCompletionSource<bool> RelationshipUpdateTask = new();
        _ = Subscribe.RelationshipUpdate((d) =>
        {
            RelationshipUpdateTask.SetResult(true);
        });
        mock.RelationshipUpdate();
        yield return new WaitUntil(() => RelationshipUpdateTask.Task.IsCompleted);
    }
}
