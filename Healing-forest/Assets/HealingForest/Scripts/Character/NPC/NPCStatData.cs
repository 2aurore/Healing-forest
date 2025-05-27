using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "NPCStatData", menuName = "HealingForest/NPC/NPCStatData", order = 1)]
    public class NPCStatData : ScriptableObject
    {
        public float maxHp;
        public float happiness;
        public float attackPower;
    }
}
