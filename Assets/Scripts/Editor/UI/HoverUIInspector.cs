using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HoverUIInspector : EditorWindow
{
    private List<RaycastResult> _results = new List<RaycastResult>();

    private GameObject hoveredUI;

    [MenuItem("Tools/UI/Hover UI Inspector")]
    public static void ShowWindow()
    {
        GetWindow<HoverUIInspector>("UI Hover Inspector");
    }

    void OnGUI()
    {
        GUILayout.Label("UI Hover Inspector", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to use this tool.", MessageType.Info);
            return;
        }


        GUILayout.Space(10);

        // Hover Info
        if (hoveredUI != null)
        {
            GUILayout.Label("Hovering: " + hoveredUI.name);
            GUILayout.Space(2);
        }
        else
        {
            GUILayout.Label("Hover over UI in Game View...");
        }

        Transform tempParent = hoveredUI != null? hoveredUI.transform.parent : null;
        int index = 1;
        while (tempParent != null)
        {
            GUILayout.Label(index + " | Hovering: " + tempParent.name);
            index++;
            tempParent = tempParent.transform.parent;
        }

        GUILayout.Space(10);

        // Debug stack
        if (_results.Count > 0)
        {
            GUILayout.Label("Raycast Stack:", EditorStyles.boldLabel);

            for (int i = 0; i < _results.Count; i++)
            {
                GUILayout.Label(i + ": " + _results[i].gameObject.name);
            }
        }
    }

    void Update()
    {
        if (!Application.isPlaying || EventSystem.current == null)
        {
            _results.Clear();
            hoveredUI = null;
            return;
        }

        // =========================
        // Get hovered UI
        // =========================
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        _results.Clear();
        EventSystem.current.RaycastAll(pointerData, _results);

        if (_results.Count > 0)
        {
            hoveredUI = _results[0].gameObject;
        }
        else
        {
            hoveredUI = null;
        }

        // =========================
        // Runtime input (works in Game View)
        // =========================

        Repaint();
    }


    // =========================
    // Smart parent detection
    // =========================
    GameObject GetRelevantParent(GameObject obj)
    {
        Transform current = obj.transform;

        while (current != null)
        {
            // If it's a selectable UI (Button, Toggle, etc.)
            if (current.GetComponent<Selectable>() != null)
                return current.gameObject;

            // Stop at Canvas
            if (current.GetComponent<Canvas>() != null)
                return current.gameObject;

            current = current.parent;
        }

        return obj;
    }
}