using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    public abstract class ContextMenu
    {
        /// <summary>
        /// Creates a ScriptableObject of a given type.
        /// </summary>
        /// <typeparam name="T">Type of ScriptableObject.</typeparam>
        protected static void CreateScriptableObject<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New " + typeof(T).Name + ".asset";

            ProjectWindowUtil.CreateAsset(asset, path);
        }
    }
}