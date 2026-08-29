using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public static class WorldCanvasCreator
{
    [MenuItem("GameObject/UI/World Canvas", false, 10)]
    private static void CreateWorldCanvas(MenuCommand menuCommand)
    {
        GameObject canvasObj = new GameObject("World Canvas", typeof(Canvas), typeof(CanvasScaler));
        GameObjectUtility.SetParentAndAlign(canvasObj, menuCommand.context as GameObject);

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        canvasObj.transform.localScale = Vector3.one * 0.01f; 

        GameObject imageObj = new GameObject("Image", typeof(Image));
        imageObj.transform.SetParent(canvasObj.transform);

        RectTransform imageRt = imageObj.GetComponent<RectTransform>();
        imageRt.anchorMin = Vector2.zero;
        imageRt.anchorMax = Vector2.one;
        imageRt.offsetMin = Vector2.zero;
        imageRt.offsetMax = Vector2.zero;
        imageRt.localScale = Vector3.one;
        imageRt.localPosition = Vector3.zero;

        Image image = imageObj.GetComponent<Image>();
        image.color = Color.white;

        Undo.RegisterCreatedObjectUndo(canvasObj, "Create World Canvas");
        Selection.activeObject = canvasObj;
    }
}
