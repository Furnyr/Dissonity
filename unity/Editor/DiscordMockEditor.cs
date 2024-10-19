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
        private bool showIap = false;
        private bool showCurrentPlayerEvents = false;
        private bool showOtherPlayers = false;
        private bool showChannels = false;
        private bool showSkus = false;
        private bool showEntitlements = false;

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
        private bool clearingSkus = false;
        private bool clearingEntitlements = false;

        // Handles the "other players" event foldouts
        private List<bool> showOtherPlayerEvents = new();


        public override void OnInspectorGUI()
        {            
            DiscordMock mock = (DiscordMock) target;

            GUIStyle leftButtonStyle; 

            SetButtonStyles(out leftButtonStyle);

            // Shorcut
            bool isPlaying = Application.isPlaying;

            // Query
            var queryProperty = serializedObject.FindProperty(nameof(DiscordMock._query));
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
                var localeProperty = serializedObject.FindProperty(nameof(DiscordMock._locale));
                EditorGUILayout.PropertyField(localeProperty, true);

                //# CURRENT PLAYER - - - - -      
                var currentPlayerProperty = serializedObject.FindProperty(nameof(DiscordMock._currentPlayer));
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
                var otherPlayersProperty = serializedObject.FindProperty(nameof(DiscordMock._otherPlayers));
                showOtherPlayers = EditorGUILayout.Foldout(showOtherPlayers, "Other Players");

                if (showOtherPlayers)
                {
                    EditorGUI.indentLevel++;

                    // Draw and add dispatch buttons to every player
                    for (int i = 0; i < mock._otherPlayers.Count; i++)
                    {
                        // Draw the array element
                        var otherPlayer = otherPlayersProperty.GetArrayElementAtIndex(i);
                        var participant = otherPlayer.FindPropertyRelative(nameof(MockPlayer.Participant));
                        string name = participant.FindPropertyRelative(nameof(MockParticipant.GlobalName)).stringValue;
                        
                        EditorGUILayout.PropertyField(otherPlayer, new GUIContent (name), false);

                        if (otherPlayer.isExpanded)
                        {
                            //? Handle difference in players and tracked players
                            if (showOtherPlayerEvents.Count != mock._otherPlayers.Count)
                            {
                                //? Mock has more
                                if (showOtherPlayerEvents.Count < mock._otherPlayers.Count)
                                {
                                    int newPlayers = mock._otherPlayers.Count - showOtherPlayerEvents.Count;

                                    for (int y = 0; y < newPlayers; y++)
                                    {
                                        showOtherPlayerEvents.Add(false);
                                    }
                                }

                                //? A player was deleted, regen
                                else
                                {
                                    showOtherPlayerEvents.Clear();

                                    foreach (var _ in mock._otherPlayers)
                                    {
                                        showOtherPlayerEvents.Add(false);
                                    }
                                }
                            }
                            
                            EditorGUI.indentLevel++;

                            DrawChildrenRecursively(otherPlayer, new string[] { nameof(MockPlayer.GuildMemberRpc) });

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
                        long id = player.Participant.Id = Utils.GetMockSnowflake();
                        player.GuildMemberRpc.UserId = id;

                        // Unique username
                        player.Participant.Username += $" {mock._otherPlayers.Count+2}";

                        // Unique global name
                        player.Participant.GlobalName += $" {mock._otherPlayers.Count+2}";

                        // Unique nickname
                        string nickname = player.Participant.Nickname += $" {mock._otherPlayers.Count+2}";
                        player.GuildMemberRpc.Nickname = nickname;

                        mock._otherPlayers.Add(player);
                    }

                    TintButtonBlue();

                    if (!clearingPlayers && GUILayout.Button("Clear", leftButtonStyle))
                    {
                        clearingPlayers = true;
                    }

                    ResetButtonTint();

                    if (clearingPlayers)
                    {
                        TintButtonDark();

                        if (GUILayout.Button("Cancel clear", leftButtonStyle))
                        {
                            clearingPlayers = false;
                        }

                        ResetButtonTint();
                        TintButtonRed();

                        if (GUILayout.Button("Confirm clear", leftButtonStyle))
                        {
                            clearingPlayers = false;
                            otherPlayersProperty.ClearArray();
                            showOtherPlayerEvents.Clear();
                        }

                        ResetButtonTint();
                    }

                    EndSpace();
                }

                else if (clearingPlayers) clearingPlayers = false;


                //# CHANNELS - - - - -
                var channelsProperty = serializedObject.FindProperty(nameof(DiscordMock._channels));
                showChannels = EditorGUILayout.Foldout(showChannels, "Channels");

                if (showChannels)
                {
                    EditorGUI.indentLevel++;

                    // Draw every channel
                    for (int i = 0; i < mock._channels.Count; i++)
                    {
                        // Draw the array element
                        var channel = channelsProperty.GetArrayElementAtIndex(i);
                        string channelName = channel.FindPropertyRelative(nameof(MockChannel.Name)).stringValue;
                        
                        EditorGUILayout.PropertyField(channel, new GUIContent (channelName), false);

                        if (channel.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            DrawChildrenRecursively(channel, new string[] { nameof(MockChannel.VoiceStates) });

                            EditorGUI.indentLevel--;
                        }
                    }
                    
                    EditorGUI.indentLevel--;

                    // Draw add channel button
                    StartSpace(20);


                    if (GUILayout.Button("Add channel", leftButtonStyle))
                    {
                        var channel = new MockChannel
                        {
                            // Unique id
                            Id = Utils.GetMockSnowflake(),

                            // Unique name
                            Name = $"mock-channel-{mock._channels.Count + 1}"
                        };

                        mock._channels.Add(channel);
                    }

                    TintButtonBlue();

                    if (!clearingChannels && GUILayout.Button("Clear", leftButtonStyle))
                    {
                        clearingChannels = true;
                    }

                    ResetButtonTint();

                    if (clearingChannels)
                    {
                        TintButtonDark();

                        if (GUILayout.Button("Cancel clear", leftButtonStyle))
                        {
                            clearingChannels = false;
                        }

                        ResetButtonTint();
                        TintButtonRed();

                        if (GUILayout.Button("Confirm clear", leftButtonStyle))
                        {
                            clearingChannels = false;
                            channelsProperty.ClearArray();
                        }

                        ResetButtonTint();
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
                bool displayButton;

                StartSpace(10);

                if (GUILayout.Button("Activity Instance Participants Update", leftButtonStyle))
                {
                    //? Not in runtime
                    if (!isPlaying)
                    {
                        Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                        return;
                    }

                    if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Activity Instance Participants Update");
                    
                    mock.ActivityInstanceParticipantsUpdate();
                }

                if (!inspectingLayout)
                {
                    TintButtonBlue();
                    displayButton = GUILayout.Button("Activity Layout Mode Update", leftButtonStyle);
                    ResetButtonTint();

                    if (displayButton)
                    {
                        inspectingLayout = true;
                        showEventLayout = true;
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;

                    // Layout
                    showEventLayout = EditorGUILayout.Foldout(showEventLayout, "Activity Layout Mode Update");

                    if (showEventLayout)
                    {
                        EditorGUI.indentLevel++;

                        // Property
                        var layoutProperty = serializedObject.FindProperty(nameof(DiscordMock._layoutMode));
                        EditorGUILayout.PropertyField(layoutProperty, true);

                        StartSpace(30);

                        // Dispatch
                        if (GUILayout.Button("Dispatch Event", leftButtonStyle))
                        {
                            //? Not in runtime
                            if (!isPlaying)
                            {
                                Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                return;
                            }

                            if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Activity Layout Mode Update");

                            mock.ActivityLayoutModeUpdate();
                        }

                        TintButtonDark();

                        // Close
                        if (GUILayout.Button("Close", leftButtonStyle))
                        {
                            inspectingLayout = false;
                            showEventLayout = false;
                        }

                        ResetButtonTint();

                        EndSpace();

                        GUILayout.Space(10);

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }

                if (!inspectingOrientation)
                {
                    TintButtonBlue();
                    displayButton = GUILayout.Button("Orientation Update", leftButtonStyle);
                    ResetButtonTint();

                    if (displayButton)
                    {
                        inspectingOrientation = true;
                        showEventOrientation = true;
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;

                    // Layout
                    showEventOrientation = EditorGUILayout.Foldout(showEventOrientation, "Orientation Update");

                    if (showEventOrientation)
                    {
                        EditorGUI.indentLevel++;

                        // Property
                        var orientationProperty = serializedObject.FindProperty(nameof(DiscordMock._screenOrientation));
                        EditorGUILayout.PropertyField(orientationProperty, true);

                        StartSpace(30);

                        // Dispatch
                        if (GUILayout.Button("Dispatch Event", leftButtonStyle))
                        {
                            //? Not in runtime
                            if (!isPlaying)
                            {
                                Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                return;
                            }

                            if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Orientation Update");

                            mock.OrientationUpdate();
                        }

                        TintButtonDark();

                        // Close
                        if (GUILayout.Button("Close", leftButtonStyle))
                        {
                            inspectingOrientation = false;
                            showEventOrientation = false;
                        }

                        ResetButtonTint();

                        EndSpace();

                        GUILayout.Space(10);

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }

                if (!inspectingThermalState)
                {
                    TintButtonBlue();
                    displayButton = GUILayout.Button("Thermal State Update", leftButtonStyle);
                    ResetButtonTint();

                    if (displayButton)
                    {
                        inspectingThermalState = true;
                        showEventThermalState = true;
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;

                    // Layout
                    showEventThermalState = EditorGUILayout.Foldout(showEventThermalState, "Thermal State Update");

                    if (showEventThermalState)
                    {
                        EditorGUI.indentLevel++;

                        // Property
                        var thermalStateProperty = serializedObject.FindProperty(nameof(DiscordMock._thermalState));
                        EditorGUILayout.PropertyField(thermalStateProperty, true);

                        StartSpace(30);

                        // Dispatch
                        if (GUILayout.Button("Dispatch Event", leftButtonStyle))
                        {
                            //? Not in runtime
                            if (!isPlaying)
                            {
                                Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                return;
                            }
                            
                            if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Thermal State Update");

                            mock.ThermalStateUpdate();
                        }

                        TintButtonDark();

                        // Close
                        if (GUILayout.Button("Close", leftButtonStyle))
                        {
                            inspectingThermalState = false;
                            showEventThermalState = false;
                        }

                        ResetButtonTint();

                        EndSpace();

                        GUILayout.Space(10);

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            
                EndSpace();
            }

            showIap = EditorGUILayout.Foldout(showIap, "In-App Purchases");

            // IAP
            if (showIap)
            {
                EditorGUI.indentLevel++;

                //# SKUS - - - - -
                var skusProperty = serializedObject.FindProperty(nameof(DiscordMock._skus));
                showSkus = EditorGUILayout.Foldout(showSkus, "Skus");

                if (showSkus)
                {
                    EditorGUI.indentLevel++;

                    // Draw every sku
                    for (int i = 0; i < mock._skus.Count; i++)
                    {
                        // Draw the array element
                        var sku = skusProperty.GetArrayElementAtIndex(i);
                        string skuName = sku.FindPropertyRelative(nameof(MockSku.Name)).stringValue;
                        
                        EditorGUILayout.PropertyField(sku, new GUIContent (skuName), false);

                        if (sku.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            DrawChildrenRecursively(sku, null, new Dictionary<string, string>{
                                { nameof(MockSkuPrice.Amount), "Amount represents cents" },
                                { nameof(MockSkuPrice.Currency), "Currency is a string of Models.CurrencyCode" }
                            });

                            EditorGUI.indentLevel--;
                        }
                    }
                    
                    EditorGUI.indentLevel--;

                    // Draw add sku button
                    StartSpace(20);


                    if (GUILayout.Button("Add SKU", leftButtonStyle))
                    {
                        var sku = new MockSku
                        {
                            // Unique id
                            Id = Utils.GetMockSnowflake(),

                            // Unique name
                            Name = $"Mock SKU {mock._skus.Count + 1}"
                        };

                        mock._skus.Add(sku);
                    }

                    TintButtonBlue();

                    if (!clearingSkus && GUILayout.Button("Clear", leftButtonStyle))
                    {
                        clearingSkus = true;
                    }

                    ResetButtonTint();

                    if (clearingSkus)
                    {
                        TintButtonDark();

                        if (GUILayout.Button("Cancel clear", leftButtonStyle))
                        {
                            clearingSkus = false;
                        }

                        ResetButtonTint();
                        TintButtonRed();

                        if (GUILayout.Button("Confirm clear", leftButtonStyle))
                        {
                            clearingSkus = false;
                            skusProperty.ClearArray();
                        }

                        ResetButtonTint();
                    }

                    EndSpace();
                }

                else if (clearingSkus) clearingSkus = false;

                //# ENTITLEMENTS - - - - -
                var entitlementsProperty = serializedObject.FindProperty(nameof(DiscordMock._entitlements));
                GUIContent entitlementsContent = new("Entitlements", "A mock entitlement will be created after calling Api.Commands.StartPurchase(mockSkuId)");
                showEntitlements = EditorGUILayout.Foldout(showEntitlements, entitlementsContent);

                if (showEntitlements)
                {
                    EditorGUI.indentLevel++;

                    // Draw every entitlement
                    for (int i = 0; i < mock._entitlements.Count; i++)
                    {
                        // Draw the array element
                        var entitlement = entitlementsProperty.GetArrayElementAtIndex(i);
                        string name = entitlement.FindPropertyRelative(nameof(MockEntitlement.__mock__name)).stringValue;
                        long id = entitlement.FindPropertyRelative(nameof(MockEntitlement.Id)).longValue;
                        
                        EditorGUILayout.PropertyField(entitlement, new GUIContent (name), false);

                        if (entitlement.isExpanded)
                        {
                            EditorGUI.indentLevel++;

                            DrawChildrenRecursively(entitlement, null, new Dictionary<string, string>{{ nameof(MockEntitlement.__mock__name), "Entitlements don't have names, this only changes the visual name in the mock." }});

                            StartSpace(40);

                            if (GUILayout.Button("Dispatch Entitlement Create", leftButtonStyle))
                            {
                                //? Not in runtime
                                if (!isPlaying)
                                {
                                    Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                                    return;
                                }

                                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Entitlement Create");
                                
                                mock.EntitlementCreate(id);
                            }

                            EndSpace();

                            GUILayout.Space(10);

                            EditorGUI.indentLevel--;
                        }
                    }
                    
                    EditorGUI.indentLevel--;

                    // Draw add sku button
                    StartSpace(20);


                    if (GUILayout.Button("Add default entitlement", leftButtonStyle))
                    {
                        var ent = new MockEntitlement
                        {
                            // Unique id
                            Id = Utils.GetMockSnowflake(),

                            // Unique name
                            __mock__name = $"Mock Entitlement {mock._entitlements.Count + 1}"
                        };

                        mock._entitlements.Add(ent);
                    }

                    TintButtonBlue();

                    if (!clearingEntitlements && GUILayout.Button("Clear", leftButtonStyle))
                    {
                        clearingEntitlements = true;
                    }

                    ResetButtonTint();

                    if (clearingEntitlements)
                    {
                        TintButtonDark();

                        if (GUILayout.Button("Cancel clear", leftButtonStyle))
                        {
                            clearingEntitlements = false;
                        }

                        ResetButtonTint();
                        TintButtonRed();

                        if (GUILayout.Button("Confirm clear", leftButtonStyle))
                        {
                            clearingEntitlements = false;
                            entitlementsProperty.ClearArray();
                        }

                        ResetButtonTint();
                    }

                    EndSpace();
                }

                else if (clearingEntitlements) clearingEntitlements = false;

                EditorGUI.indentLevel--;
            }
            

            serializedObject.ApplyModifiedProperties();
        }

        // Used to draw the children of a property. If Unity does it automatically, indentation breaks between versions.
        // exclude is used to prevent properties from rendering
        // tooltipMap is used to draw tooltips for specific properties
        internal static void DrawChildrenRecursively(SerializedProperty property, string[] exclude = null, Dictionary<string, string> tooltipMap = null)
        {
            SerializedProperty endProperty = property.GetEndProperty();

            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, endProperty))
            {
                if (exclude == null || !exclude.Contains(property.name))
                {
                    //? Tooltip
                    if (tooltipMap != null && tooltipMap.ContainsKey(property.name))
                    {
                        GUIContent content = new (property.name, tooltipMap[property.name]);
                        EditorGUILayout.PropertyField(property, content, property.isArray);
                    }

                    else EditorGUILayout.PropertyField(property, property.isArray);

                    if (property.hasVisibleChildren && property.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawChildrenRecursively(property, exclude, tooltipMap);
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
            bool isPlaying = Application.isPlaying;

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

                    if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Current Guild Member Update");

                    mock.CurrentGuildMemberUpdate();
                }

                if (GUILayout.Button("Current User Update", style))
                {
                    //? Not in runtime
                    if (!isPlaying)
                    {
                        Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                        return;
                    }

                    if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Current User Update");

                    mock.CurrentUserUpdate();
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

                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Voice State Update");

                mock.VoiceStateUpdate(playerIndex);
            }

            if (GUILayout.Button("Speaking Start", style))
            {
                //? Not in runtime
                if (!isPlaying)
                {
                    Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                    return;
                }

                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Speaking Start");

                mock.SpeakingStart(playerIndex);
            }

            if (GUILayout.Button("Speaking Stop", style))
            {
                //? Not in runtime
                if (!isPlaying)
                {
                    Debug.Log("[Dissonity Editor]: You can only dispatch events during runtime!");
                    return;
                }

                if (!Api.Configuration.DisableDissonityInfoLogs) Utils.DissonityLog("Dispatching mock Speaking Stop");

                mock.SpeakingStop(playerIndex);
            }
        }

        private void SetButtonStyles(out GUIStyle leftButtonStyle)
        {
            // Normal button style
            leftButtonStyle = new GUIStyle(GUI.skin.button);
            leftButtonStyle.alignment = TextAnchor.MiddleLeft;
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
    
        // Danger
        private void TintButtonRed()
        {
            GUI.backgroundColor = new Color(0.78f, 0.29f, 0.23f);
        }

        // Warning (currently unused)
        private void TintButtonYellow()
        {
            GUI.backgroundColor = new Color(1f, 0.8f, 0f);
        }

        // Close
        private void TintButtonDark()
        {
            GUI.backgroundColor = new Color(0.78f, 0.78f, 0.78f);
        }

        // Open
        private void TintButtonBlue()
        {
            GUI.backgroundColor = new Color(0.6f, 1f, 0.96f);
        }

        // Unreleased
        private void TintButtonDisabled()
        {
            GUI.backgroundColor = new Color(0.46f, 0.45f, 0.447f);
        }

        private void ResetButtonTint()
        {
            GUI.backgroundColor = Color.white;
        }
    }
}