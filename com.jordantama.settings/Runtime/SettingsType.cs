using UnityEngine;

namespace JordanTama.Settings
{
    public abstract class SettingsType : ScriptableObject
    {
        public abstract string DataPath();
        public abstract string InstanceDirectory();
        protected abstract string InstanceFileName();
    
        public string InstancePath => InstanceDirectory() + InstanceFileName();
    }
}