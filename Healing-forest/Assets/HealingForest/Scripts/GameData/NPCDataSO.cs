using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "NPCData", menuName = "ScriptableObjects/NPCDataSO")]
    public class NPCDataSO : ScriptableObject
    {
        [SerializeField] private string npcId;
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private List<string> randomDialogue = new List<string>();

        public string NpcID => npcId;
        public GameObject VisualPrefab => visualPrefab;
        public List<string> RandomDialogue => randomDialogue;
    }
}
