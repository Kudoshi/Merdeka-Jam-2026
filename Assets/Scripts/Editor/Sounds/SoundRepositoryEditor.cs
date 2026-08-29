using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class SoundRepositoryEditor : EditorWindow
{
    private static string SOUND_REPO_PATH = "Assets/ScriptableObjects/SO_SoundRepo.asset";

    private SO_SoundRepository soundRepo;
    private int selectedTab = 0;
    private string searchQuery = "";
    private int selectedIndex = -1;
    private HashSet<int> selectedIndices = new HashSet<int>();
    private Vector2 listScrollPosition;
    private Vector2 propertiesScrollPosition;

    private float panelSplitRatio = 0.35f;
    private bool isResizing = false;
    private Color borderColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [MenuItem("Settings/Sounds/Sound Repository")]
    public static void ShowWindow()
    {
        SoundRepositoryEditor window = GetWindow<SoundRepositoryEditor>("Sound Repository");
        window.minSize = new Vector2(900, 700);
    }

    private void OnGUI()
    {
        soundRepo = AssetDatabase.LoadAssetAtPath<SO_SoundRepository>(SOUND_REPO_PATH);
        if (soundRepo == null) return;

        EditorGUILayout.Space();
        selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Sounds", "BGM" });
        EditorGUILayout.Space();

        SerializedObject serializedRepo = new SerializedObject(soundRepo);
        SerializedProperty listProperty = GetSelectedList(serializedRepo);
        if (listProperty == null) return;

        float totalWidth = position.width;
        float leftPanelWidth = totalWidth * panelSplitRatio;

        EditorGUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(leftPanelWidth));
        DrawSearchBar();
        DrawList(listProperty);
        DrawAddButton(listProperty);
        GUILayout.EndVertical();

        DrawBorder(new Rect(leftPanelWidth, 75, 2, position.height));
        DrawResizeHandle();

        GUILayout.BeginVertical();
        DrawProperties(listProperty);
        GUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        serializedRepo.ApplyModifiedProperties();
    }

    private void DrawSearchBar()
    {
        searchQuery = EditorGUILayout.TextField("Search:", searchQuery);
        EditorGUILayout.Space();
    }

    private void DrawList(SerializedProperty listProperty)
    {
        listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition, GUILayout.Height(position.height - 120));

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = element.FindPropertyRelative("SoundName");
            SerializedProperty groupProp = element.FindPropertyRelative("SoundGroup");

            if (!string.IsNullOrEmpty(searchQuery) && !nameProp.stringValue.ToLower().Contains(searchQuery.ToLower()))
                continue;

            bool isSelected = selectedIndices.Contains(i);

            GUIStyle bgStyle = new GUIStyle();
            if (isSelected)
                bgStyle.normal.background = Texture2D.grayTexture;

            EditorGUILayout.BeginVertical(bgStyle);
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            if (GUI.Button(rowRect, GUIContent.none, GUIStyle.none))
            {
                // Ctrl/Cmd = toggle individual
                if (Event.current.control || Event.current.command)
                {
                    if (selectedIndices.Contains(i))
                        selectedIndices.Remove(i);
                    else
                        selectedIndices.Add(i);

                    // Primary selection = last clicked
                    selectedIndex = selectedIndices.Count > 0 ? selectedIndices.Last() : -1;
                }
                // Shift = range select
                else if (Event.current.shift && selectedIndex >= 0)
                {
                    int min = Mathf.Min(selectedIndex, i);
                    int max = Mathf.Max(selectedIndex, i);
                    for (int j = min; j <= max; j++)
                        selectedIndices.Add(j);
                }
                // Normal click = select only this
                else
                {
                    selectedIndices.Clear();
                    selectedIndices.Add(i);
                    selectedIndex = i;
                }
            }

            GUIStyle labelStyle = isSelected ? EditorStyles.boldLabel : EditorStyles.label;
            GUILayout.Label(nameProp.stringValue, labelStyle);

            if (selectedTab != 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                continue;
            }

            GUILayout.FlexibleSpace();

            GUIStyle tagStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(6, 6, 2, 2)
            };

            string rightText = groupProp != null ? groupProp.enumDisplayNames[groupProp.enumValueIndex] : "-";
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = GetSoundGroupColor(groupProp.enumValueIndex);
            Vector2 size = tagStyle.CalcSize(new GUIContent(rightText));
            GUILayout.Label(rightText, tagStyle, GUILayout.Width(size.x + 6));
            GUI.backgroundColor = prevColor;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawAddButton(SerializedProperty listProperty)
    {
        if (GUILayout.Button("Add New Sound"))
        {
            listProperty.arraySize++;
            selectedIndex = listProperty.arraySize - 1;
            selectedIndices.Clear();
            selectedIndices.Add(selectedIndex);
        }
    }

    private void DrawProperties(SerializedProperty listProperty)
    {
        if (selectedIndices.Count == 0 || selectedIndex < 0 || selectedIndex >= listProperty.arraySize) return;

        bool isMultiSelect = selectedIndices.Count > 1;

        SerializedProperty element = listProperty.GetArrayElementAtIndex(selectedIndex);
        SerializedProperty soundName = element.FindPropertyRelative("SoundName");
        SerializedProperty clip = element.FindPropertyRelative("Clip");
        SerializedProperty soundGroup = element.FindPropertyRelative("SoundGroup");
        SerializedProperty soundSetting = element.FindPropertyRelative("SoundSetting");

        SerializedProperty generalSettings = soundSetting.FindPropertyRelative("GeneralAudioSettings");
        SerializedProperty bypassSettings = soundSetting.FindPropertyRelative("BypassSettings");
        SerializedProperty stereoSettings = soundSetting.FindPropertyRelative("StereoSettings");
        SerializedProperty rolloffSettings = soundSetting.FindPropertyRelative("RolloffSettings");

        propertiesScrollPosition = EditorGUILayout.BeginScrollView(propertiesScrollPosition);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16 };
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };

        // Title — show multi select hint
        string title = isMultiSelect ? $"Sound Properties ({selectedIndices.Count} selected)" : "Sound Properties";
        EditorGUILayout.LabelField(title, titleStyle, GUILayout.Height(24));
        EditorGUILayout.Space(6);

        // ── Core ── (skip name/clip in multi — too risky to batch)
        EditorGUILayout.LabelField("Core", headerStyle, GUILayout.Height(20));
        EditorGUILayout.Space(2);
        EditorGUI.indentLevel++;
        if (!isMultiSelect)
        {
            EditorGUILayout.PropertyField(soundName);
            EditorGUILayout.PropertyField(clip);
        }
        else
        {
            EditorGUILayout.HelpBox("Name and Clip are not editable in multi-select.", MessageType.Info);
        }
        DrawMultiField(listProperty, soundGroup, "SoundGroup", null, isMultiSelect);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(8);

        // ── General ─────────────────────────────────────────────
        EditorGUILayout.LabelField("General", headerStyle, GUILayout.Height(20));
        EditorGUILayout.Space(2);
        EditorGUI.indentLevel++;
        DrawMultiField(listProperty, generalSettings.FindPropertyRelative("Volume"), "SoundSetting.GeneralAudioSettings.Volume", generalSettings, isMultiSelect);
        DrawMultiField(listProperty, generalSettings.FindPropertyRelative("Pitch"), "SoundSetting.GeneralAudioSettings.Pitch", generalSettings, isMultiSelect);
        DrawMultiField(listProperty, generalSettings.FindPropertyRelative("Loop"), "SoundSetting.GeneralAudioSettings.Loop", generalSettings, isMultiSelect);
        DrawMultiField(listProperty, generalSettings.FindPropertyRelative("Priority"), "SoundSetting.GeneralAudioSettings.Priority", generalSettings, isMultiSelect);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(8);

        // ── Stereo & Spatial ────────────────────────────────────
        EditorGUILayout.LabelField("Stereo & Spatial", headerStyle, GUILayout.Height(20));
        EditorGUILayout.Space(2);
        EditorGUI.indentLevel++;
        DrawMultiField(listProperty, stereoSettings.FindPropertyRelative("PanStereo"), "SoundSetting.StereoSettings.PanStereo", stereoSettings, isMultiSelect);
        DrawMultiField(listProperty, stereoSettings.FindPropertyRelative("SpatialBlend"), "SoundSetting.StereoSettings.SpatialBlend", stereoSettings, isMultiSelect);
        DrawMultiField(listProperty, stereoSettings.FindPropertyRelative("ReverbZoneMix"), "SoundSetting.StereoSettings.ReverbZoneMix", stereoSettings, isMultiSelect);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(8);

        // ── Rolloff & Distance ───────────────────────────────────
        EditorGUILayout.LabelField("Rolloff & Distance", headerStyle, GUILayout.Height(20));
        EditorGUILayout.Space(2);
        EditorGUI.indentLevel++;
        DrawMultiField(listProperty, rolloffSettings.FindPropertyRelative("RolloffMode"), "SoundSetting.RolloffSettings.RolloffMode", rolloffSettings, isMultiSelect);
        DrawMultiField(listProperty, rolloffSettings.FindPropertyRelative("MinDistance"), "SoundSetting.RolloffSettings.MinDistance", rolloffSettings, isMultiSelect);
        DrawMultiField(listProperty, rolloffSettings.FindPropertyRelative("MaxDistance"), "SoundSetting.RolloffSettings.MaxDistance", rolloffSettings, isMultiSelect);
        DrawMultiField(listProperty, rolloffSettings.FindPropertyRelative("DopplerLevel"), "SoundSetting.RolloffSettings.DopplerLevel", rolloffSettings, isMultiSelect);
        DrawMultiField(listProperty, rolloffSettings.FindPropertyRelative("Spread"), "SoundSetting.RolloffSettings.Spread", rolloffSettings, isMultiSelect);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(8);

        // ── Bypass ──────────────────────────────────────────────
        EditorGUILayout.LabelField("Bypass", headerStyle, GUILayout.Height(20));
        EditorGUILayout.Space(2);
        EditorGUI.indentLevel++;
        DrawMultiField(listProperty, bypassSettings.FindPropertyRelative("BypassEffects"), "SoundSetting.BypassSettings.BypassEffects", bypassSettings, isMultiSelect);
        DrawMultiField(listProperty, bypassSettings.FindPropertyRelative("BypassListenerEffects"), "SoundSetting.BypassSettings.BypassListenerEffects", bypassSettings, isMultiSelect);
        DrawMultiField(listProperty, bypassSettings.FindPropertyRelative("BypassReverbZones"), "SoundSetting.BypassSettings.BypassReverbZones", bypassSettings, isMultiSelect);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(12);

        // ── Remove ──────────────────────────────────────────────
        GUI.backgroundColor = Color.red;
        string removeLabel = isMultiSelect ? $"Remove {selectedIndices.Count} Sounds" : "Remove Sound";
        if (GUILayout.Button(removeLabel, GUILayout.Width(160)))
        {
            // Remove in reverse order so indices don't shift
            List<int> sorted = selectedIndices.OrderByDescending(x => x).ToList();
            foreach (int idx in sorted)
                listProperty.DeleteArrayElementAtIndex(idx);

            selectedIndices.Clear();
            selectedIndex = -1;
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Draws a property field. If multi-select, detects changes and applies to all selected.
    /// </summary>
    private void DrawMultiField(SerializedProperty listProperty, SerializedProperty prop, string relativePath, SerializedProperty parentProp, bool isMultiSelect)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(prop);
        if (EditorGUI.EndChangeCheck() && isMultiSelect)
        {
            ApplyToAllSelected(listProperty, prop, relativePath);
        }
    }

    /// <summary>
    /// Copies the value of a property to all selected indices.
    /// </summary>
    private void ApplyToAllSelected(SerializedProperty listProperty, SerializedProperty sourceProp, string relativePath)
    {
        foreach (int idx in selectedIndices)
        {
            if (idx == selectedIndex) continue;

            SerializedProperty targetElement = listProperty.GetArrayElementAtIndex(idx);
            SerializedProperty targetProp = targetElement.FindPropertyRelative(relativePath);
            if (targetProp == null) continue;

            CopyPropertyValue(sourceProp, targetProp);
        }
    }

    private void CopyPropertyValue(SerializedProperty source, SerializedProperty target)
    {
        switch (source.propertyType)
        {
            case SerializedPropertyType.Float: target.floatValue = source.floatValue; break;
            case SerializedPropertyType.Integer: target.intValue = source.intValue; break;
            case SerializedPropertyType.Boolean: target.boolValue = source.boolValue; break;
            case SerializedPropertyType.String: target.stringValue = source.stringValue; break;
            case SerializedPropertyType.Enum: target.enumValueIndex = source.enumValueIndex; break;
            case SerializedPropertyType.ObjectReference: target.objectReferenceValue = source.objectReferenceValue; break;
        }
    }

    private SerializedProperty GetSelectedList(SerializedObject serializedRepo)
    {
        switch (selectedTab)
        {
            case 0: return serializedRepo.FindProperty("SoundList");
            case 1: return serializedRepo.FindProperty("BGMList");
            default: return null;
        }
    }

    private void DrawResizeHandle()
    {
        Rect dragRect = new Rect(position.width * panelSplitRatio, 0, 5, position.height);
        EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.ResizeHorizontal);

        if (Event.current.type == EventType.MouseDown && dragRect.Contains(Event.current.mousePosition))
            isResizing = true;
        if (isResizing)
            panelSplitRatio = Mathf.Clamp(Event.current.mousePosition.x / position.width, 0.2f, 0.8f);
        if (Event.current.type == EventType.MouseUp)
            isResizing = false;

        GUILayout.Space(5);
    }

    private void DrawBorder(Rect rect)
    {
        EditorGUI.DrawRect(rect, borderColor);
    }

    public static Color GetSoundGroupColor(int index)
    {
        if (System.Enum.IsDefined(typeof(SoundGroup), index))
            return GetSoundGroupColor((SoundGroup)index);
        return Color.gray;
    }

    public static Color GetSoundGroupColor(SoundGroup soundGroup)
    {
        switch ((int)soundGroup)
        {
            case 0: return new Color(1f, 0.7f, 0.3f);
            case 1: return new Color(0.4f, 0.7f, 1f);
            case 2: return new Color(0.5f, 0.85f, 0.5f);
            case 3: return new Color(1f, 0.4f, 0.4f);
            case 4: return new Color(1f, 0.5f, 0.2f);
            case 5: return new Color(0.8f, 0.6f, 1f);
            case 6: return new Color(0.6f, 1f, 1f);
            case 7: return new Color(0.7f, 0.7f, 0.7f);
            case 8: return new Color(1f, 1f, 0.5f);
            case 9: return new Color(1f, 0.6f, 1f);
            case 10: return new Color(0.6f, 1f, 0.8f);
            case 11: return Color.green;
            case 12: return Color.blue;
            case 13: return Color.red;
            case 14: return Color.cyan;
            case 15: return Color.magenta;
            default: return Color.gray;
        }
    }
}