using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JordanTama.Settings.Editor
{
    public abstract class SettingsContextMenu : JordanTama.Tools.Editor.ContextMenu
    {
        private const string ContextDirectory = "Assets/Create/";

        private const string BaseDirectory = "Packages/com.jordantama.settings/ScriptTemplates/";
        
        private const string SettingsPath = BaseDirectory + "SettingsTemplate.txt";
        private const string SettingsTypePath = BaseDirectory + "SettingsTypeTemplate.txt";
        private const string SettingsWindowPath = BaseDirectory + "SettingsWindowTemplate.txt";
        

        [MenuItem(ContextDirectory + "Settings")]
        private static void NewSettings()
        {
            CreateRuntimeScripts(SettingsPath,
                "New" + Path.GetFileNameWithoutExtension(SettingsPath) + ".cs");
        }
        

        private static void CreateRuntimeScripts(string templatePath, string defaultNewFileName)
        {
            // Exceptions
            if (templatePath == null)
                throw new ArgumentNullException(nameof(templatePath));
            
            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"The template file \"{templatePath}\" could not be found.", templatePath);

            if (string.IsNullOrEmpty(defaultNewFileName))
                defaultNewFileName = Path.GetFileName(templatePath);
            
            // Asset icon
            Texture2D icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            DoCreateSettingsScript onNameEditEnd = ScriptableObject.CreateInstance<DoCreateSettingsScript>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, onNameEditEnd, defaultNewFileName, icon, templatePath);
        }

        private static Object CreateSettingsScript(string pathName)
        {
            string content = File.ReadAllText(SettingsPath);

            string baseFile = Path.GetFileNameWithoutExtension(pathName);
            string baseFileNoSpaces = baseFile.Replace(" ", "");
            
            content = content.Replace("#NOTRIM#", "");
            content = content.Replace("#NAME#", baseFile);
            content = content.Replace("#SCRIPTNAME#", baseFileNoSpaces);
            content = content.Replace("#TYPE#", baseFileNoSpaces + "Type");

            return CreateScriptAssetWithContent(
                Path.GetDirectoryName(pathName) + "/" + baseFileNoSpaces + Path.GetExtension(pathName),
                content);
        }
        
        private static Object CreateSettingsTypeScript(string settingsPathName)
        {
            string content = File.ReadAllText(SettingsTypePath);
            
            string baseFile = Path.GetFileNameWithoutExtension(settingsPathName);
            string baseFileNoSpaces = baseFile.Replace(" ", "");
            string scriptName = baseFileNoSpaces + "Type";
            
            content = content.Replace("#NOTRIM#", "");
            content = content.Replace("#SCRIPTNAME#", scriptName);
            content = content.Replace("#SETTINGSNAME#", baseFileNoSpaces);

            return CreateScriptAssetWithContent(settingsPathName.Replace(baseFile, scriptName), content);
        }

        private static Object CreateSettingsWindowScript(string settingsPathName)
        {
            string content = File.ReadAllText(SettingsWindowPath);
            
            string baseFile = Path.GetFileNameWithoutExtension(settingsPathName);
            string baseFileNoSpaces = baseFile.Replace(" ", "");
            string scriptName = baseFileNoSpaces + "Window";
            
            content = content.Replace("#NOTRIM#", "");
            content = content.Replace("#SCRIPTNAME#", scriptName);
            content = content.Replace("#DISPLAYNAME#", baseFile);
            content = content.Replace("#SETTINGSNAME#", baseFileNoSpaces);

            string directory = Path.GetDirectoryName(Path.GetFullPath(settingsPathName));
            Directory.CreateDirectory(directory + "/Editor");
            
            return CreateScriptAssetWithContent(settingsPathName.Replace(baseFile, "Editor/" + scriptName), content);
        }

        
        private class DoCreateSettingsScript : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object settings = CreateSettingsScript(pathName);
                Object settingsType = CreateSettingsTypeScript(pathName);
                Object settingsWindow = CreateSettingsWindowScript(pathName);
                
                ProjectWindowUtil.ShowCreatedAsset(settings);
            }
        }
    }
}