using System;
using PlayerControl;
using UnityEngine;

namespace SkillSystem.Skill
{
    /// <summary>
    /// 示例技能：蓄力冲刺（非瞬发）
    /// </summary>
    public class DashSkill : ISkill
    {
        public SkillType Type => SkillType.ActiveSkill;
        public bool IsInstant => false;
        public float Cooldown => 5f;
        public bool LimitedUseSkill => false;
        public int MaxCharges => 1;

        public event Action OnFinished;

        private GameObject _player;
        private PlayerController _playerController;

        private bool _isDashing;    // 是否在冲刺

        private float _dashDuration = 0.2f;
        private float _dashTimer;

        private float _dashForce = 50f;

        public void Init(GameObject player)
        {
            _player = player;
            _playerController = _player.GetComponent<PlayerController>();
        }

        /// <summary>
        /// 技能准备（技能键按下）
        /// </summary>
        public void SkillArmed()
        {
            Debug.Log("Skill Armed");
        }

        /// <summary>
        /// 技能取消（再次按技能键）
        /// </summary>
        public void SkillUnarmed()
        {
            Debug.Log("Skill Unarmed");
        }

        /// <summary>
        /// 技能释放（攻击键按下）
        /// </summary>
        public void SkillUsed()
        {
            _playerController.PauseControl();
            
            _isDashing = true;
            _dashTimer = _dashDuration;

            Debug.Log("【技能】冲刺释放");
        }

        /// <summary>
        /// 技能逻辑更新
        /// </summary>
        public void SkillUpdate(float deltaTime)
        {
            if (!_isDashing)
                return;
            
            Vector3 forward = _player.transform.forward;
            _player.transform.Translate(forward * (_dashForce * deltaTime), Space.World);
            
            _dashTimer -= deltaTime;
            if (_dashTimer <= 0f)
            {
                FinishSkill();
            }
        }

        private void FinishSkill()
        {
            _isDashing = false;
            Debug.Log("【技能】冲刺结束");
            
            _playerController.ResumeControl();

            OnFinished?.Invoke();
        }
    }
}
