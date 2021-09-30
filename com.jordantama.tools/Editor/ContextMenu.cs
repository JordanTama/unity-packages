using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using Object = UnityEngine.Object;

namespace JordanTama.Tools.Editor
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
        
        /// <summary>
        /// Create a new script from a template.
        /// </summary>
        /// <param name="templatePath">The path of the template script file.</param>
        /// <param name="defaultName">The default name of the new script file.</param>
        /// <typeparam name="T">The type of <see cref="EndNameEditAction"/> used to conclude name editing.</typeparam>
        /// <exception cref="ArgumentNullException">The template path is null.</exception>
        /// <exception cref="FileNotFoundException">The template path is invalid.</exception>
        protected static void CreateScriptFromTemplate<T>(string templatePath, string defaultName = null)
            where T : EndNameEditAction
        {
            // Exceptions
            if (templatePath == null)
                throw new ArgumentNullException(nameof(templatePath));

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"The template file {templatePath} could not be found.", templatePath);

            if (string.IsNullOrEmpty(defaultName))
                defaultName = Path.GetFileName(templatePath);
            
            // Asset icon
            Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            T onNameEditEnd = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, onNameEditEnd, defaultName, icon, templatePath);
        }
        
        /// <summary>
        /// Create a script with content.
        /// </summary>
        /// <param name="pathName">The path to create the script at, relative to the project directory.</param>
        /// <param name="content">The content to populate the new script with.</param>
        /// <returns>The created script as a <see cref="UnityEngine.Object"/>.</returns>
        protected static Object CreateScriptAssetWithContent(string pathName, string content)
        {
            content = SetLineEndings(content, EditorSettings.lineEndingsForNewScripts);

            string fullPath = Path.GetFullPath(pathName);
            var encoding = new System.Text.UTF8Encoding(true);

            File.WriteAllText(fullPath, content, encoding);
            AssetDatabase.ImportAsset(pathName);

            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
        
        private static string SetLineEndings(string content, LineEndingsMode lineEndingsMode)
        {
            const string windowsLineEndings = "\r\n";
            const string unixLineEndings = "\n";

            string preferredLineEndings;

            switch (lineEndingsMode)
            {
                case LineEndingsMode.OSNative:
                    if (Application.platform == RuntimePlatform.WindowsEditor)
                        preferredLineEndings = windowsLineEndings;
                    else
                        preferredLineEndings = unixLineEndings;
                    break;
                case LineEndingsMode.Unix:
                    preferredLineEndings = unixLineEndings;
                    break;
                case LineEndingsMode.Windows:
                    preferredLineEndings = windowsLineEndings;
                    break;
                default:
                    preferredLineEndings = unixLineEndings;
                    break;
            }

            content = Regex.Replace(content, @"\r\n?|\n", preferredLineEndings);

            return content;
        }
    }
}