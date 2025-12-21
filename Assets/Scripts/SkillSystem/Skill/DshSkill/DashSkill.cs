using System;
using DG.Tweening;
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
        private CharacterController _characterController;
        private PlayerMeshController _playerMeshController;
        
        private Vector3 _originalPosition;

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
            _characterController = player.GetComponent<CharacterController>();
            _playerMeshController = player.GetComponentInChildren<PlayerMeshController>();
            
            _originalPosition = _playerMeshController.transform.localPosition;
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

            Transform mesh = _playerMeshController.transform;

            // 记录初始状态
            Vector3 originalLocalPos = _originalPosition;
            Quaternion originalLocalRot = mesh.localRotation;

            // 抬高模型，避免穿地
            mesh.localPosition = originalLocalPos + Vector3.up * _characterController.height / 2;

            float duration = _config.dashDuration;

            // 清理残留 Tween（非常重要）
            mesh.DOKill();

            // 构建动画序列
            Sequence seq = DOTween.Sequence();

            // 前倾 90 度（绕 X 轴）
            mesh.localEulerAngles = new Vector3(0f, 90f, 90f);

            // 同时旋转两圈（绕 Z 轴，翻滚感）
            seq.Join(
                mesh.DOLocalRotate(
                    new Vector3(360f, 90f, 90f),
                    duration,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear)
            );

            // 结束复原姿态
            seq.Append(
                mesh.DOLocalRotate(
                    originalLocalRot.eulerAngles,
                    0
                ).SetEase(Ease.OutQuad)
            );

            seq.OnComplete(() =>
            {
                // 位置复原
                mesh.localPosition = originalLocalPos;
            });

            Debug.Log("【技能】冲刺释放");
        }


        public void SkillUpdate(float deltaTime)
        {
            if (!_isDashing) return;
            
            _characterController.Move(_player.transform.forward * (deltaTime * _config.dashForce));

            _dashTimer -= deltaTime;
            if (_dashTimer <= 0f)
            {
                FinishSkill();
            }
        }

        private void FinishSkill()
        {
            _playerMeshController.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            
            _isDashing = false;
            _playerController.ResumeControl();
            OnFinished?.Invoke();
        }
    }
}