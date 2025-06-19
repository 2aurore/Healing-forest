using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class NPCBase : CharacterBase, IDamage
    {
        public string NpcID => npcId;

        [Header("NPC Base Settings")]
        [SerializeField] private string npcId;
        [SerializeField] private Transform visualRoot;

        public event System.Action<CharacterBase> OnDamaged;

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

            // TODO: 상호작용이 종료된 다음 애니메이션 레이어를 초기화
            player.ResetAnimatorLayer();
        }

        public void Damage(CharacterBase attacker)
        {
            OnDamaged?.Invoke(attacker);
        }

        // <summary> NPC가 상호작용을 시작할 때 호출되는 메서드 </summary>
        // public void SetSmileActionTrigger()
        // {
        //     base.animator.SetTrigger("Smile");
        // }
    }
}
