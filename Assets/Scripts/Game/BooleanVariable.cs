using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Create BooleanVariable", fileName = "BooleanVariable", order = 0)]
    public class BooleanVariable : ScriptableObject
    {
        public event Action OnValueChanged;
        private bool value;

        public bool Value
        {
            set { 
                OnValueChanged?.Invoke(); 
                this.value = value;
            }
            get => value;
        }
    }
}