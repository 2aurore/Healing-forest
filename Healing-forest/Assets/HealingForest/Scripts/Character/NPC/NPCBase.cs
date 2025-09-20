using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HF
{
    public class NPCBase : CharacterBase, IDamage
    {
        void OnDrawGizmos()
        {
            if (headTransform)
            {
                GizmoUtility.DrawArrowHandle(headTransform.position, headTransform.up, 1f, Color.green);
            }
        }

        public string NpcID => npcId;

        [Header("NPC Base Settings")]
        [SerializeField] private string npcId;
        [SerializeField] private Transform visualRoot;

        public event System.Action<CharacterBase> OnDamaged;

        private NPCVisualCharacter visualCharacter;
        private NPCSensor npcSensor;
        private Transform detectedPlayerTransform;
        private Transform headTransform;

        protected override void Awake()
        {
            // VisualPrefab을 가지고와서 Visual Transform의 자식으로 생성
            var npcDataSO = GameDataModel.Singleton.GetNPCData(this.NpcID);
            var newVisual = Instantiate(npcDataSO.VisualPrefab);
            newVisual.transform.SetParent(visualRoot);
            newVisual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // VisualCharacter 컴포넌트 초기화
            visualCharacter = newVisual.GetComponent<NPCVisualCharacter>();
            base.animator = visualCharacter.animator;
            // animator = GetComponentInChildren<Animator>();

            headTransform = base.animator.GetBoneTransform(HumanBodyBones.Head);

            npcSensor = GetComponentInChildren<NPCSensor>();
            npcSensor.OnDetectedPlayer += OnDetectedPlayer;
            npcSensor.OnLostPlayer += OnLostPlayer;
        }

        protected override void Update()
        {
            base.Update();

            if (detectedPlayerTransform != null)
            {
                // NPC가 플레이어를 감지하고 있을 때, 머리 방향을 플레이어에게 향하도록 설정
                visualCharacter.headAimingIK.HeadAimingPoint = detectedPlayerTransform.position;
            }
        }

        private void OnDetectedPlayer(Transform target)
        {
            visualCharacter.headAimingIK.isActiveHeadAiming = true;
            detectedPlayerTransform = target;
        }

        private void OnLostPlayer()
        {
            visualCharacter.headAimingIK.isActiveHeadAiming = false;
            detectedPlayerTransform = null;
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
