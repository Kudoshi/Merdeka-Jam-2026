using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CustomButton))]
[CanEditMultipleObjects]
public class CustomButtonEditor : SelectableEditor
{
    SerializedProperty onClickProp;
    SerializedProperty selectOnPressProp;
    SerializedProperty overrideSelectSoundProp;
    SerializedProperty overrideSubmitSoundProp;
    SerializedProperty overrideRightClickSoundProp;
    SerializedProperty enableRightClick;
    SerializedProperty buttonAnimationType;

    protected override void OnEnable()
    {
        base.OnEnable();

        onClickProp = serializedObject.FindProperty("m_OnClick");
        selectOnPressProp = serializedObject.FindProperty("_uiObjectToSelectOnPress");
        overrideSelectSoundProp = serializedObject.FindProperty("_overrideDefaultSelectSound");
        overrideSubmitSoundProp = serializedObject.FindProperty("_overrideDefaultSubmitSound");
        overrideRightClickSoundProp = serializedObject.FindProperty("_overrideDefaultRightClickSound");
        enableRightClick = serializedObject.FindProperty("_enableRightClick");
        buttonAnimationType = serializedObject.FindProperty("_buttonAnimationType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw Selectable (Navigation, Colors, etc.)
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onClickProp);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(selectOnPressProp);
        EditorGUILayout.PropertyField(buttonAnimationType);
        EditorGUILayout.PropertyField(overrideSelectSoundProp);
        EditorGUILayout.PropertyField(overrideSubmitSoundProp);
        EditorGUILayout.PropertyField(overrideRightClickSoundProp);
        EditorGUILayout.PropertyField(enableRightClick);

        serializedObject.ApplyModifiedProperties();
    }
}
