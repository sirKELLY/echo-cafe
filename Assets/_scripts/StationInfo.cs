using UnityEngine;

namespace _scripts
{
    [CreateAssetMenu(fileName = "StationInfo", menuName = "Scriptable Objects/StationInfo")]
    public class StationInfo : ScriptableObject
    {
        public float craftTime = 10f;
        public ItemInfo output;   // the item this station produces
        // other?
    }
}