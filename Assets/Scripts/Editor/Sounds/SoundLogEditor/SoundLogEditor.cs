using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundLogEditor : EditorWindow
{
    // Configs
    private int _maxLogs = 200;

    private List<SoundLog> _logs = new List<SoundLog>();
    private Vector2 _scroll;
    private bool _autoScroll = true;

    // Config for showing the table
    private float _indexWidth = 30f;
    private float _timeWidth = 50f;
    private float _groupWidth = 80f;
    private float _padding = 5f;

    // Tooltip
    private int _hoveredIndex = -1;

    [MenuItem("Settings/Sounds/Sound Log")]
    public static void ShowWindow()
    {
        SoundLogEditor window = GetWindow<SoundLogEditor>("Sound Log");
        window.minSize = new Vector2(300, 300);
        window.Show();
    }

    // =========================
    // SUBSCRIBE / UNSUBSCRIBE
    // =========================
    private void OnEnable()
    {
        SoundManager.OnSoundSfxPlayed += HandleSoundPlayed;
        SoundManager.OnDialoguePlayed += HandleSoundPlayed;
        SoundManager.OnBGMPlayed += HandleSoundPlayed;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        SoundManager.OnSoundSfxPlayed -= HandleSoundPlayed;
        SoundManager.OnDialoguePlayed -= HandleSoundPlayed;
        SoundManager.OnBGMPlayed -= HandleSoundPlayed;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            _logs.Add(new SoundLog(null, 0f)); // null = session divider
            Repaint();
        }
    }

    // =========================
    // EVENT CALLBACK
    // =========================
    private void HandleSoundPlayed(Sound sound)
    {
        _logs.Add(new SoundLog(sound, Time.time));
        Repaint();

        if (_logs.Count > _maxLogs)
            _logs.RemoveAt(0);
    }

    // =========================
    // UI
    // =========================
    private void OnGUI()
    {
        DrawToolbar();

        // Reserve scroll area
        Rect scrollAreaRect = new Rect(0, EditorStyles.toolbar.fixedHeight + 2, position.width, position.height - EditorStyles.toolbar.fixedHeight - 2);
        _scroll = GUI.BeginScrollView(scrollAreaRect, _scroll, GetContentRect());

        float y = 0f;
        float rowHeight = 20f;

        for (int i = 0; i < _logs.Count; i++)
        {

            SoundLog log = _logs[i];
            Rect rowRect = new Rect(0, y, position.width - 16f, rowHeight);

            // Session divider
            if (log.Sound == null)
            {
                DrawSessionDivider(rowRect);
                y += rowHeight;
                continue;
            }

            // Hover highlight
            if (rowRect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(rowRect, new Color(1f, 1f, 1f, 0.07f));
            }

            // Detect hover via MouseMove
            if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.Repaint)
            {
                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (_hoveredIndex != i)
                    {
                        _hoveredIndex = i;
                        Repaint();
                    }
                }
                else if (_hoveredIndex == i)
                {
                    _hoveredIndex = -1;
                    Repaint();
                }
            }

            // ── Row content ──
            float x = rowRect.x + 4f;

            Rect indexRect = new Rect(x, rowRect.y, _indexWidth, rowRect.height);
            x += _indexWidth + _padding;

            Rect nameRect = new Rect(x, rowRect.y, rowRect.width - (_indexWidth + _timeWidth + _groupWidth + _padding * 3 + 4f), rowRect.height);
            Rect timeRect = new Rect(rowRect.xMax - (_timeWidth + _groupWidth + _padding), rowRect.y, _timeWidth, rowRect.height);
            Rect groupRect = new Rect(rowRect.xMax - _groupWidth, rowRect.y, _groupWidth, rowRect.height);

            EditorGUI.LabelField(indexRect, i.ToString(), EditorStyles.boldLabel);
            EditorGUI.LabelField(nameRect, log.Sound.SoundName);

            string time = TimeSpan.FromSeconds(log.Time).ToString(@"mm\:ss");
            EditorGUI.LabelField(timeRect, time);

            DrawSoundGroup(groupRect, log.Sound.SoundGroup);

            y += rowHeight;
        }

        // Draw tooltip AFTER the loop so it renders on top
        for (int i = 0; i < _logs.Count; i++)
        {
            SoundLog log = _logs[i];
            if (log.Sound == null) continue;

            Rect rowRect = new Rect(0, i * rowHeight, position.width - 16f, rowHeight);

            if (rowRect.Contains(Event.current.mousePosition))
            {
                DrawTooltip(log, Event.current.mousePosition);
                break;
            }
        }

        GUI.EndScrollView();

        if (_autoScroll)
            _scroll.y = float.MaxValue;
    }

    private float GetRowScreenY(int index, float rowHeight)
    {
        float y = 0f;
        for (int i = 0; i < index; i++)
            y += rowHeight;
        return y;
    }

    private Rect GetContentRect()
    {
        float totalHeight = _logs.Count * 20f;
        return new Rect(0, 0, position.width - 16f, Mathf.Max(totalHeight, position.height));
    }

    // =========================
    // DRAW HELPERS
    // =========================
    private void DrawSessionDivider(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.2f, 0.6f, 1f, 0.15f));

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 11,
            normal = { textColor = new Color(0.4f, 0.8f, 1f) }
        };

        EditorGUI.LabelField(rect, "▶  New Play Session", style);
    }

    private void DrawTooltip(SoundLog log, Vector2 mousePos)
    {
        Sound s = log.Sound;
        SoundSetting ss = s.SoundSetting;

        float lineHeight = 17f;
        float tooltipWidth = 250f;
        float tooltipHeight = lineHeight * 16 + 16f;

        float tx = mousePos.x + 14f;
        float ty = mousePos.y + 10f;

        // Flip if overflow
        if (tx + tooltipWidth > position.width) tx = mousePos.x - tooltipWidth - 4f;
        if (ty + tooltipHeight > position.height) ty = mousePos.y - tooltipHeight - 4f;

        Rect tooltipRect = new Rect(tx, ty, tooltipWidth, tooltipHeight);

        // Shadow + Background + Border
        EditorGUI.DrawRect(new Rect(tooltipRect.x + 3, tooltipRect.y + 3, tooltipRect.width, tooltipRect.height), new Color(0, 0, 0, 0.4f));
        EditorGUI.DrawRect(tooltipRect, new Color(0.18f, 0.18f, 0.18f, 0.97f));
        DrawRectBorder(tooltipRect, new Color(0.4f, 0.4f, 0.4f, 1f));

        float y = tooltipRect.y + 8f;
        float lx = tooltipRect.x + 10f;
        float lw = tooltipWidth - 20f;

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
        EditorGUI.LabelField(new Rect(lx, y, lw, lineHeight), s.SoundName, headerStyle);
        y += lineHeight + 2f;

        DrawTooltipDivider(new Rect(lx, y, lw, 1f)); y += 5f;
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Group", s.SoundGroup.ToString());

        DrawTooltipDivider(new Rect(lx, y, lw, 1f)); y += 5f;
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Volume", ss.GeneralAudioSettings.Volume.ToString("F2"));
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Pitch", ss.GeneralAudioSettings.Pitch.ToString("F2"));
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Loop", ss.GeneralAudioSettings.Loop.ToString());
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Priority", ss.GeneralAudioSettings.Priority.ToString());

        DrawTooltipDivider(new Rect(lx, y, lw, 1f)); y += 5f;
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Spatial Blend", ss.StereoSettings.SpatialBlend.ToString("F2"));
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Pan Stereo", ss.StereoSettings.PanStereo.ToString("F2"));
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Reverb Mix", ss.StereoSettings.ReverbZoneMix.ToString("F2"));

        DrawTooltipDivider(new Rect(lx, y, lw, 1f)); y += 5f;
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Rolloff", ss.RolloffSettings.RolloffMode.ToString());
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Min Dist", ss.RolloffSettings.MinDistance.ToString("F1"));
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Max Dist", ss.RolloffSettings.MaxDistance.ToString("F1"));
        DrawTooltipRow(ref y, lx, lw, lineHeight, "Doppler", ss.RolloffSettings.DopplerLevel.ToString("F2"));

        Repaint();
    }

    private void DrawTooltipRow(ref float y, float x, float width, float lineHeight, string label, string value)
    {
        float labelWidth = width * 0.45f;
        float valueWidth = width * 0.55f;

        EditorGUI.LabelField(new Rect(x, y, labelWidth, lineHeight), label, EditorStyles.miniLabel);
        EditorGUI.LabelField(new Rect(x + labelWidth, y, valueWidth, lineHeight), value, EditorStyles.label);
        y += lineHeight;
    }

    private void DrawTooltipDivider(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f, 0.5f));
    }

    private void DrawRectBorder(Rect rect, Color color)
    {
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), color);
        EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), color);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), color);
        EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y, 1, rect.height), color);
    }

    private void DrawSoundGroup(Rect rect, SoundGroup soundGroup)
    {
        GUIStyle tagStyle = new GUIStyle(EditorStyles.miniButton)
        {
            padding = new RectOffset(6, 6, 2, 2)
        };

        Color prevColor = GUI.backgroundColor;
        GUI.backgroundColor = SoundRepositoryEditor.GetSoundGroupColor(soundGroup);
        EditorGUI.LabelField(rect, soundGroup.ToString(), tagStyle);
        GUI.backgroundColor = prevColor;
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            _logs.Clear();

        _autoScroll = GUILayout.Toggle(_autoScroll, "Auto Scroll", EditorStyles.toolbarButton);

        GUILayout.FlexibleSpace();

        if (!Application.isPlaying)
        {
            GUIStyle notPlayingStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = new Color(1f, 0.6f, 0.2f) },
                fontStyle = FontStyle.Italic
            };
            GUILayout.Label("Not in Play Mode — showing previous logs", notPlayingStyle);
        }

        EditorGUILayout.EndHorizontal();
    }
}

public class SoundLog
{
    public Sound Sound; // null = session divider
    public float Time;

    public SoundLog(Sound sound, float time)
    {
        Sound = sound;
        Time = time;
    }
}