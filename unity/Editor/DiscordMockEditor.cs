using UnityEngine;
using UnityEditor;
using Dissonity.Models.Mock;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dissonity.Editor
{
    [CustomEditor(typeof(DiscordMock))]
    internal class DiscordMockEditor : UnityEditor.Editor
    {
        // Main foldouts
        private bool showActivity = false;
        private bool showGeneralEvents = false;
        private bool showCurrentPlayerEvents = false;
        private bool showOtherPlayers = false;
        private bool showChannels = false;

        // True when a general event that requires details is clicked
        private bool inspectingLayout = false;
        private bool inspectingOrientation = false;
        private bool inspectingThermalState = false;

        // Handles general events foldouts
        private bool showEventLayout = false;
        private bool showEventOrientation = false;
        private bool showEventThermalState = false;

        // True when the clear menus are open
        private bool clearingPlayers = false;
        private bool clearingChannels = false;

        // Handles the "other players" event foldouts
        private List<bool> showOtherPlayerEvents = new();


        public override void OnInspectorGUI()
        {
            DiscordMock mock = (DiscordMock) target;

            GUIStyle leftButtonStyle; 
            GUIStyle dangerLeftButtonStyle; 
            GUIStyle disabledLeftButtonStyle;

            SetButtonStyles(out leftButtonStyle, out dangerLeftButtonStyle, out disabledLeftButtonStyle);

            // Shorcut
            bool isPlaying = UnityEngine.Application.isPlaying;

            // Query
            var queryProperty = serializedObject.FindProperty(nameof(DiscordMock.query));
            EditorGUILayout.PropertyField(queryProperty, false);

            if (queryProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawChildrenRecursively(queryProperty);
                EditorGUI.indentLevel--;
            }

            showActivity = EditorGUILayout.Foldout(showActivity, "Activity");

            // Activity
            if (showActivity)
            {
                EditorGUI.indentLevel++;

                // Locale
                var localeProperty = serializedObject.FindProperty(nameof(DiscordMock.locale));
                EditorGUILayout.PropertyField(localeProperty, true);

                //# CURRENT PLAYER - - - - -      
                var currentPlayerProperty = serializedObject.FindProperty(nameof(DiscordMock.currentPlayer));
                EditorGUILayout.PropertyField(currentPlayerProperty, false);

                if (currentPlayerProperty.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    DrawChildrenRecursively(currentPlayerProperty);

                    showCurrentPlayerEvents = EditorGUILayout.Foldout(showCurrentPlayerEvents, "Dispatch Events");

                    if (showCurrentPlayerEvents)
                    {
                        StartSpace(40);
                        DrawDispatchButtons(leftButtonStyle, mock);
                        EndSpace();
                    }

                    EditorGUI.indentLevel--;
                }


                //# OTHER PLAYERS - - - - -
                var otherPlayersProperty = serializedObject.FindProperty(nameof(DiscordMock.otherPlayers));
                showOtherPlayers = EditorGUILayout.Foldout(showOtherPlayers, "Other Players");

                if (showOtherPlayers)
                {
                    EditorGUI.indentLevel++;

                    // Draw and add dispatch buttons to every player
                    for (int i = 0; i < mock.otherPlayers.Count; i++)
                    {
                        // Draw the array element
                        var otherPlayer = otherPlayersProperty.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(otherPlayer, new GUIContent ($"Player {i+2}"), false);

                        if (otherPlayer.isExpanded)
                        {
                            //? Handle difference in players and tracked players
                            if (showOtherPlayerEvents.Count != mock.otherPlayers.Count)
                            {
                                //? Mock has more
                                if (showOtherPlayerEvents.Count < mock.otherPlayers.Count)
                                {
                                    int newPlayers = mock.otherPlayers.Count - showOtherPlayerEvents.Count;

                                    for (int y = 0; y < newPlayers; y++)
                                    {
                                        showOtherPlayerEvents.Add(false);
                                    }
                                }

                                //? A player was deleted, regen
                                else
                                {
                                    showOtherPlayerEvents.Clear();

                                    foreach (var _ in mock.otherPlayers)
                                    {
                                        showOtherPlayerEvents.Add(false);
                                    }
                                }
                            }
                            
                            EditorGUI.indentLevel++;

                            DrawChildrenRecursively(otherPlayer);

                            showOtherPlayerEvents[i] = EditorGUILayout.Foldout(showOtherPlayerEvents[i], "Dispatch Events");

                            if (showOtherPlayerEvents[i])
                            {
                                StartSpace(60);
                                DrawDispatchButtons(leftButtonStyle, mock, i);
                                EndSpace();
                            }
                        
                            EditorGUI.indentLevel--;
                        }
                    }
                    
                    EditorGUI.indentLevel--;

                    // Draw add player button
                    StartSpace(20);

                    if (GUILayout.Button("Add player", leftButtonStyle))
                    {
                        var player = new MockPlayer();

                        // Unique id
                        string id = player.Participant.Id = Utils.GetMockSnowflake();
                        player.GuildMemberRpc.UserId = id;
                        player.UserVoiceState.User.Id = id;

                        // Unique username
                        string username = player.Participant.Username += $" {mock.otherPlayers.Count+2}";
                        player.UserVoiceState.User.Username = username;

                        // Unique global name
                        string globalName = player.Participant.GlobalName += $" {mock.otherPlayers.Count+2}";
                        player.UserVoiceState.User.GlobalName = globalName;

                        // Unique nickname
                        string nickname = player.Participant.Nickname += $" {mock.otherPlayers.Count+2}";
                        player.GuildMemberRpc.Nickname = nickname;
                        player.UserVoiceState.Nickname = nickname;

                        mock.otherPlayers.Add(player);
                    }

                    if (!clearingPlayers && GUILayout.Button("Clear", leftButtonStyle))
                    {
                        clearingPlayers = true;
                    }

                    if (clearingPlayers)
                    {
                        if (GUILayout.Button("Cancel clear", leftButtonStyle))
                        {
                            clearingPlayers = false;
                        }

                        if (GUILayout.Button("Confirm clear", dangerLeftButtonStyle))
                        {
                            clearingPlayers = false;
                            mock.otherPlayers.Clear();
                            showOtherPlayerEvents.Clear();
                        }
                    }

                    EndSpace();
                }

                else if (clearingPlayers) clearingPlayers = false;


                //# CHANNELS - - - - -
                var channelsProperty = serializedObject.FindProperty(nameof(DiscordMock.channels));
                showChannels = EditorGUILayout.Foldout(showChannels, "Channels");

                if (showChannels)
                {
                    EditorGUI.indentLevel++;

                    // Draw every channel
                    for (int i = 0; i < mock.channels.Count; i++)
                    {
                        // Draw the array element
                        var dictionary = channelsProperty.GetArrayElementAtIndex(i);
                        var channel = dictionary.FindPropertyRelative(nameof(MockDictionary<MockChannel>.Value));
                        var channelData = channel.FindPropertyRelative(nameof(MockDictionary<MockChannel>.Value.ChannelData));
                        string channelName = channelData.FindPropertyRelative(nameof(MockDictionary<MockChannel>.Value.ChannelData.Name)).stringValue;
                        
                        EditorGUILayout.PropertyField(dictionary, new GUIContent (channelName), false);

                        if (dictionary.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            DrawChildrenRecursively(dictionary, new string[] { nameof(MockGetChannelData.VoiceStates) });

                            EditorGUI.indentLevel--;
                        }
                    
                        // Don't ask me why, but Unity draws the array length box comically large the more indented the array is...
                        // This fixes that, but either way, the voice states are excluded because it was unreadable.
                        if (channel.isExpanded)
                        {
                            int indentLevel = EditorGUI.indentLevel;
                            EditorGUI.indentLevel = 0;

                            var voiceStates = channelData.FindPropertyRelative(nameof(MockGetChannelData.VoiceStates));

                            StartSpace(60);
                            EditorGUILayout.PropertyField(voiceStates, true);
                            EndSpace();

                            EditorGUI.indentLevel = indentLevel;
                        }
                    }
                    
                    EditorGUI.indentLevel--;

                    // Draw add channel button
                    StartSpace(20);


                    if (GUILayout.Button("Add channel", leftButtonStyle))
                    {
                        var channel = new MockChannel();

                        // Unique id
                        string id = channel.ChannelData.Id = Utils.GetMockSnowflake();

                        // Unique name
                        channel.ChannelData.Name = $"mock-channel-{mock.channels.Count + 1}";

                        mock.channels.Add(new MockDictionary<MockChannel> { Id = id, Value = channel });
                    }

                    if (!clearingChannels && GUILayout.Button("Clear", leftButtonStyle))
                    {
                        clearingChannels = true;
                    }

                    if (clearingChannels)
                    {
                        if (GUILayout.Button("Cancel clear", leftButtonStyle))
                        {
                            clearingChannels = false;
                        }

                        if (GUILayout.Button("Confirm clear", dangerLeftButtonStyle))
                        {
                            clearingChannels = false;
                            mock.channels.Clear();
                        }
                    }

                    EndSpace();
                }

                else if (clearingChannels) clearingChannels = false;

                EditorGUI.indentLevel--;
            }

            showGeneralEvents = EditorGUILayout.Foldout(showGeneralEvents, "General Events");

            // General events
            if (showGeneralEvents)
            {
                StartSpace(20);

                if (GUILayout.Button("Activity Instance Participants Update", leftButtonStyle))
                {
                    //? Not in runtime
                    if (!isPlaying)
                    {
                        Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                        return;
                    }

                    mock.ActivityInstanceParticipantsUpdate();

                    if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Activity Instance Participants Update");
                }

                if (!inspectingLayout && GUILayout.Button("Activity Layout Mode Update", leftButtonStyle))
                {
                    inspectingLayout = true;
                    showEventLayout = true;
                }
                else if (inspectingLayout)
                {
                    EditorGUI.indentLevel++;

                    // Layout
                    showEventLayout = EditorGUILayout.Foldout(showEventLayout, "Activity Layout Mode Update");

                    if (showEventLayout)
                    {
                        EditorGUI.indentLevel++;

                        // Property
                        var layoutProperty = serializedObject.FindProperty(nameof(DiscordMock.layoutMode));
                        EditorGUILayout.PropertyField(layoutProperty, true);

                        StartSpace(20);

                        // Dispatch
                        if (GUILayout.Button("Dispatch Event", leftButtonStyle))
                        {
                            //? Not in runtime
                            if (!isPlaying)
                            {
                                Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                return;
                            }

                            mock.ActivityLayoutModeUpdate();

                            if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Activity Layout Mode Update");
                        }

                        // Close
                        if (GUILayout.Button("Close", leftButtonStyle))
                        {
                            inspectingLayout = false;
                            showEventLayout = false;
                        }

                        EndSpace();

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }

                if (GUILayout.Button("Entitlement Create", disabledLeftButtonStyle))
                {
                    Debug.Log("[Dissonity Editor]: The Entitlement Create event is coming soon");
                }

                if (!inspectingOrientation && GUILayout.Button("Orientation Update", leftButtonStyle))
                {
                    inspectingOrientation = true;
                    showEventOrientation = true;
                }
                else if (inspectingOrientation)
                {
                    EditorGUI.indentLevel++;

                    // Layout
                    showEventOrientation = EditorGUILayout.Foldout(showEventOrientation, "Orientation Update");

                    if (showEventOrientation)
                    {
                        EditorGUI.indentLevel++;

                        // Property
                        var orientationProperty = serializedObject.FindProperty(nameof(DiscordMock.screenOrientation));
                        EditorGUILayout.PropertyField(orientationProperty, true);

                        StartSpace(20);

                        // Dispatch
                        if (GUILayout.Button("Dispatch Event", leftButtonStyle))
                        {
                            //? Not in runtime
                            if (!isPlaying)
                            {
                                Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                return;
                            }

                            mock.OrientationUpdate();

                            if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Orientation Update");
                        }

                        // Close
                        if (GUILayout.Button("Close", leftButtonStyle))
                        {
                            inspectingOrientation = false;
                            showEventOrientation = false;
                        }

                        EndSpace();

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }

                if (!inspectingThermalState && GUILayout.Button("Thermal State Update", leftButtonStyle))
                {
                    inspectingThermalState = true;
                    showEventThermalState = true;
                }
                else if (inspectingThermalState)
                {
                    EditorGUI.indentLevel++;

                    // Layout
                    showEventThermalState = EditorGUILayout.Foldout(showEventThermalState, "Thermal State Update");

                    if (showEventThermalState)
                    {
                        EditorGUI.indentLevel++;

                        // Property
                        var thermalStateProperty = serializedObject.FindProperty(nameof(DiscordMock.thermalState));
                        EditorGUILayout.PropertyField(thermalStateProperty, true);

                        StartSpace(20);

                        // Dispatch
                        if (GUILayout.Button("Dispatch Event", leftButtonStyle))
                        {
                            //? Not in runtime
                            if (!isPlaying)
                            {
                                Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                return;
                            }

                            mock.ThermalStateUpdate();

                            if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Thermal State Update");
                        }

                        // Close
                        if (GUILayout.Button("Close", leftButtonStyle))
                        {
                            inspectingThermalState = false;
                            showEventThermalState = false;
                        }

                        EndSpace();

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            
                EndSpace();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // Used to draw the children of a property. If Unity does it automatically, indentation breaks between versions.
        // "exclude" and "action" are used to draw arrays properties manually before other children properties.
        // When exclude[i] is found, action[actionCount] is run.
        private void DrawChildrenRecursively(SerializedProperty property, string[] exclude = null)
        {
            SerializedProperty endProperty = property.GetEndProperty();

            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, endProperty))
            {
                if (exclude == null || !exclude.Contains(property.name))
                {
                    EditorGUILayout.PropertyField(property, property.isArray);

                    if (property.hasVisibleChildren && property.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawChildrenRecursively(property, exclude);
                        EditorGUI.indentLevel--;

                        continue;
                    }
                }
                
                property.NextVisible(false);
            }
            
        }

        private void DrawDispatchButtons(GUIStyle style, DiscordMock mock, int playerIndex = -1)
        {
            // Shorcut
            bool isPlaying = UnityEngine.Application.isPlaying;

            //? Current player
            if (playerIndex == -1)
            {
                if (GUILayout.Button("Current Guild Member Update", style))
                {
                    //? Not in runtime
                    if (!isPlaying)
                    {
                        Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                        return;
                    }

                    mock.CurrentGuildMemberUpdate();

                    if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Current Guild Member Update");
                }

                if (GUILayout.Button("Current User Update", style))
                {
                    //? Not in runtime
                    if (!isPlaying)
                    {
                        Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                        return;
                    }

                    mock.CurrentUserUpdate();

                    if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Current User Update");
                }
            }

            if (GUILayout.Button("Voice State Update", style))
            {
                //? Not in runtime
                if (!isPlaying)
                {
                    Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                    return;
                }

                mock.VoiceStateUpdate(playerIndex);

                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Voice State Update");
            }

            if (GUILayout.Button("Speaking Start", style))
            {
                //? Not in runtime
                if (!isPlaying)
                {
                    Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                    return;
                }

                mock.SpeakingStart(playerIndex);

                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Speaking Start");
            }

            if (GUILayout.Button("Speaking Stop", style))
            {
                //? Not in runtime
                if (!isPlaying)
                {
                    Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                    return;
                }

                mock.SpeakingStop(playerIndex);

                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Speaking Stop");
            }
        }

        private void SetButtonStyles(out GUIStyle leftButtonStyle, out GUIStyle dangerLeftButtonStyle, out GUIStyle disabledLeftButtonStyle)
        {
            // Normal button style
            leftButtonStyle = new GUIStyle(GUI.skin.button);
            leftButtonStyle.alignment = TextAnchor.MiddleLeft;

            // Danger button style texture
            Texture2D redTexture = new Texture2D(1, 1);
            redTexture.SetPixels(new Color[] { new Color(0.59f, 0.34f, 0.34f) });
            redTexture.Apply();

            // Danger button style
            dangerLeftButtonStyle = new GUIStyle(GUI.skin.button);
            dangerLeftButtonStyle.alignment = TextAnchor.MiddleLeft;
            dangerLeftButtonStyle.normal.background = redTexture;

            // Disabled button style texture
            Texture2D disabledTexture = new Texture2D(1, 1);
            disabledTexture.SetPixels(new Color[] { new Color(0.25f, 0.25f, 0.25f) });
            disabledTexture.Apply();

            // Disabled button style
            disabledLeftButtonStyle = new GUIStyle(GUI.skin.button);
            disabledLeftButtonStyle.alignment = TextAnchor.MiddleLeft;
            disabledLeftButtonStyle.normal.background = disabledTexture;
        }
    
        private void StartSpace(int space)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.BeginVertical();
        }

        private void EndSpace()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}