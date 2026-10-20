using System.Collections.Generic;
using UnityEngine;
using Game.Combat;

namespace Game.Items
{
    [System.Serializable]
    public struct ElementResistanceDefinition
    {
        public ElementType element;
        [Range(-100f, 100f)] public float percent;
    }

    public interface IResistanceProvider
    {
        IReadOnlyList<ElementResistanceDefinition> ElementResistances { get; }
    }
}