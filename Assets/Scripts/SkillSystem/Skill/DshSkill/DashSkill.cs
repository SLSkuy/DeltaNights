using System;
using PlayerControl;
using UnityEngine;

namespace SkillSystem.Skill.DshSkill
{
    /// <summary>
    /// 冲刺技能具体实现，在此处实现每个技能的逻辑
    /// </summary>
    public class DashSkill : ISkill
    {
        private readonly DashSkillConfig _config;

        public SkillType Type => _config.skillType;
        public bool IsInstant => _config.isInstant;
        public float Cooldown => _config.cooldown;
        public bool LimitedUseSkill => _config.limitedUseSkill;
        public int MaxCharges => _config.maxCharges;

        public event Action OnFinished;

        private GameObject _player;
        private PlayerController _playerController;

        private bool _isDashing;
        private float _dashTimer;

        public DashSkill(DashSkillConfig config)
        {
            _config = config;
        }

        public void Init(GameObject player)
        {
            _player = player;
            _playerController = player.GetComponent<PlayerController>();
        }

        public void SkillArmed()
        {
            Debug.Log("Skill Armed");
        }

        public void SkillUnarmed()
        {
            Debug.Log("Skill Unarmed");
        }

        public void SkillUsed()
        {
            _playerController.PauseControl();

            _isDashing = true;
            _dashTimer = _config.dashDuration;

            Debug.Log("【技能】冲刺释放");
        }

        public void SkillUpdate(float deltaTime)
        {
            if (!_isDashing) return;

            _player.transform.Translate(
                _player.transform.forward * (_config.dashForce * deltaTime),
                Space.World
            );

            _dashTimer -= deltaTime;
            if (_dashTimer <= 0f)
            {
                FinishSkill();
            }
        }

        private void FinishSkill()
        {
            _isDashing = false;
            _playerController.ResumeControl();
            OnFinished?.Invoke();
        }
    }
}