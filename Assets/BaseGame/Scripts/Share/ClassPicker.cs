using System.Collections;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public class ClassPicker : PropertyAttribute { }

[CustomPropertyDrawer(typeof(ClassPicker))]
public class SubclassPickerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property);
    }

    IEnumerable GetClasses(Type baseType)
    {
        return Assembly.GetAssembly(baseType).GetTypes().Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Type t = fieldInfo.FieldType;
        string typeName = property.managedReferenceValue?.GetType().Name ?? "Not set";
        Rect rect = position;
        rect.x += EditorGUIUtility.labelWidth + 2;
        rect.width -= EditorGUIUtility.labelWidth + 2;
        rect.height = EditorGUIUtility.singleLineHeight;
        if (EditorGUI.DropdownButton(rect, new(typeName), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            foreach (Type type in GetClasses(t))
            {
                menu.AddItem(new(type.Name), typeName == type.Name, () =>
                {
                    property.managedReferenceValue = type.GetConstructor(Type.EmptyTypes)?.Invoke(null);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height), property, label, true);
    }
}