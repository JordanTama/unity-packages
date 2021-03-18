using UnityEngine;

namespace Settings
{
    public abstract class SettingsType : ScriptableObject
    {
        public abstract string DataPath();
        public abstract string InstanceDirectory();
        protected abstract string InstanceFileName();
    
        public string InstancePath => InstanceDirectory() + InstanceFileName();
    }
}