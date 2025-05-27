using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class NPCBase : CharacterBase
    {
        public string NpcID => npcId;

        [Header("NPC Base Settings")]
        [SerializeField] private string npcId;
        [SerializeField] private Transform visualRoot;

        protected override void Awake()
        {
            // VisualPrefab을 가지고와서 Visual Transform의 자식으로 생성
            var npcDataSO = GameDataModel.Singleton.GetNPCData(this.NpcID);
            var newVisual = Instantiate(npcDataSO.VisualPrefab);
            newVisual.transform.SetParent(visualRoot);
            newVisual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            animator = GetComponentInChildren<Animator>();
        }

        public void NotifyOnPlayerInteract(CharacterBase player)
        {
            // TODO: 플레이어와 상호작용을 시작하는 로직
            // 예: 대화 시작, 퀘스트 수락 등
            // SetSmileActionTrigger();
            Debug.Log($"{player.gameObject.name} interacts with NPC: {this.NpcID}");
        }

        // <summary> NPC가 상호작용을 시작할 때 호출되는 메서드 </summary>
        // public void SetSmileActionTrigger()
        // {
        //     base.animator.SetTrigger("Smile");
        // }
    }
}
