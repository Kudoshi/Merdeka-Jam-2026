using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

/// <summary>
/// This attribute can only be applied to fields because its
/// associated PropertyDrawer only operates on fields (either
/// public or tagged with the [SerializeField] attribute) in
/// the target MonoBehaviour.
/// 
/// Need to be attached to a field
/// 
/// Example Usage:
/// [Button(nameof(Btn_VisualizeCollider), "Toggle Visualize Collider")]
//  [SerializeField] private float _;
/// 
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
public class ButtonAttribute : PropertyAttribute
{
    public static float kDefaultButtonWidth = 200;
    public readonly string text;
    public readonly string MethodName;

    private float _buttonWidth = kDefaultButtonWidth;
    public float ButtonWidth
    {
        get { return _buttonWidth; }
        set { _buttonWidth = value; }
    }

    public ButtonAttribute(string MethodName, string text)
    {
        this.MethodName = MethodName;
        this.text = text;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonPropertyDrawer : PropertyDrawer
{
    private MethodInfo _eventMethodInfo = null;

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        ButtonAttribute inspectorButtonAttribute = (ButtonAttribute)attribute;
        Rect buttonRect = new Rect(position.x + (position.width - inspectorButtonAttribute.ButtonWidth) * 0.5f, position.y, inspectorButtonAttribute.ButtonWidth, position.height);
        if (GUI.Button(buttonRect, inspectorButtonAttribute.text))
        {
            System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
            string eventName = inspectorButtonAttribute.MethodName;

            if (_eventMethodInfo == null)
                _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (_eventMethodInfo != null)
                _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
            else
                Debug.LogWarning(string.Format("Button: Unable to find method {0} in {1}", eventName, eventOwnerType));
        }
    }
}
#endif