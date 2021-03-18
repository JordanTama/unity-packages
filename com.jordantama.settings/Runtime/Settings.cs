using UnityEditor;
using UnityEngine;

namespace Settings
{
    public abstract class Settings<T> where T : SettingsType
    {
        private static T _instance;
     
        public static T Instance => GetInstance();

        private static T GetInstance()
        {
            if (_instance)
                return _instance;

            T instance = ScriptableObject.CreateInstance<T>();
            _instance = Resources.Load<T>(instance.InstancePath);
        
            if (_instance)
                return _instance;

#if UNITY_EDITOR

            System.IO.Directory.CreateDirectory(instance.DataPath() + "/Resources/" + instance.InstanceDirectory());
            AssetDatabase.CreateAsset(instance, "Assets/Resources/" + instance.InstancePath + ".asset");
        
#endif

            return _instance = instance;
        }
    }
}
