using TMPro;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public static class CustomButtonMenuCommand
{
    [MenuItem("GameObject/UI/CustomButton", false, 2030)]
    private static void CreateCustomButton(MenuCommand menuCommand)
    {
        // Create Button root
        GameObject go = new GameObject("[Btn] Custom Button");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        RectTransform btnRect = go.AddComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160f, 30f); // Unity default-ish button size

        var image = go.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.white;

        go.AddComponent<CustomButton>();

        // ---- TEXT ----
        GameObject text = new GameObject("[Lbl] Button Label");
        text.transform.SetParent(go.transform, false);

        RectTransform textRect = text.AddComponent<RectTransform>();

        // Stretch to parent
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI lbl = text.AddComponent<TextMeshProUGUI>();
        lbl.text = "Button";
        lbl.color = Color.black;
        lbl.alignment = TextAlignmentOptions.Center;

        // Optional: make it feel more like Unity Button
        lbl.fontSize = 24;
        lbl.raycastTarget = false;

        // ---- Undo + Select ----
        Undo.RegisterCreatedObjectUndo(go, "Create Custom Button");
        Selection.activeGameObject = go;

    }
}
