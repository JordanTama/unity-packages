using UnityEditor;
using UnityEngine;

namespace Settings
{
public abstract class SettingsWindow : EditorWindow
{
    protected abstract string WindowTitle();
    
    protected static void DisplayWindow<T>() where T : SettingsWindow
    {
        T window = GetWindow(typeof(T)) as T;

        if (!window)
            throw new System.Exception("Window was not found.");
        
        window.titleContent = new GUIContent(window.WindowTitle());
        window.Show();
    }
}
}