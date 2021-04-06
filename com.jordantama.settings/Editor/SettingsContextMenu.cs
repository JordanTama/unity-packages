using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using Object = UnityEngine.Object;
using ContextMenu = EditorUtils.ContextMenu;

namespace Settings
{
    public abstract class SettingsContextMenu : ContextMenu
    {
        private const string ContextDirectory = "Assets/Create/Settings/";

        private const string BaseDirectory = "Packages/com.jordantama.settings/ScriptTemplates/";
        
        private const string SettingsPath = BaseDirectory + "SettingsTemplate.txt";
        private const string SettingsTypePath = BaseDirectory + "SettingsTypeTemplate.txt";
        private const string SettingsWindowPath = BaseDirectory + "SettingsWindowTemplate.txt";
        

        [MenuItem(ContextDirectory + "Settings")]
        private static void NewSettings()
        {
            CreateScriptFromTemplate<DoCreateSettingsScript>(SettingsPath,
                "New" + Path.GetFileNameWithoutExtension(SettingsPath) + ".cs");
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

            return CreateScriptAssetWithContent(pathName, content);
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

            string path = settingsPathName.Remove(settingsPathName.Length - Path.GetFileName(settingsPathName).Length) +
                          Path.GetFileName(settingsPathName).Replace(baseFile, scriptName);
            
            return CreateScriptAssetWithContent(path, content);
        }

        private static Object CreateSettingsWindowScript(string settingsPathName)
        {
            string content = File.ReadAllText(SettingsWindowPath);
            
            string baseFile = Path.GetFileNameWithoutExtension(settingsPathName);
            string baseFileNoSpaces = baseFile.Replace(" ", "");
            string scriptName = baseFileNoSpaces + "Window";
            
            content = content.Replace("#NOTRIM#", "");
            content = content.Replace("#SCRIPTNAME#", scriptName);
            content = content.Replace("#SETTINGSNAME#", baseFileNoSpaces);

            string directory = Path.GetDirectoryName(Path.GetFullPath(settingsPathName));
            Directory.CreateDirectory(directory + "/Editor");

            string path =
                settingsPathName.Remove(settingsPathName.Length - Path.GetFileName(settingsPathName).Length) +
                "Editor/" + Path.GetFileName(settingsPathName).Replace(baseFile, scriptName);
            
            return CreateScriptAssetWithContent(path, content);
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