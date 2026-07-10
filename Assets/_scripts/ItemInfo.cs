using System;
using UnityEngine;

namespace _scripts
{
    [CreateAssetMenu(fileName = "ItemInfo", menuName = "Scriptable Objects/ItemInfo")]
    [Serializable] 
    public class ItemInfo : ScriptableObject
    {
        public string displayName;
        public Sprite sprite;
        public int price = 5;   // used by the customer / economy layer
        // other?
    }
}