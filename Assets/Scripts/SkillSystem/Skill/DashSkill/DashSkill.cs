/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.21
 *  LastUpdate: 2025.12.30
 * 
 *  功能简述：
 *  DashSkill 为冲刺技能的运行时实现类，
 *  负责处理技能触发后的位移、动画与状态控制逻辑。
 *
 *  主要功能：
 *  - 基于技能配置执行冲刺行为
 *  - 控制角色移动、输入锁定与模型动画
 *  - 在技能结束时恢复角色状态并触发回调
 *
 *  使用说明：
 *  - 由对应的 SkillConfig 创建实例
 *  - 需在 Init 中完成角色相关组件绑定
 *  - 由技能系统周期性调用 SkillUpdate 驱动逻辑
 * ------------------------------------------------------------ */

using System;
using DG.Tweening;
using PlayerControl;
using UnityEngine;

namespace SkillSystem.Skill.DashSkill
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
            mesh.localPosition = originalLocalPos + Vector3.up * _characterController.height / 2 - Vector3.forward * _characterController.height / 2;

            float duration = _config.dashDuration;

            // 清理残留 Tween
            mesh.DOKill();

            // 构建动画序列
            Sequence seq = DOTween.Sequence();
            
            mesh.localEulerAngles = new Vector3(0f, 90f, 90f);

            // 万向锁处理
            seq.Join(
                mesh.DOLocalRotate(
                    new Vector3(540f, 90f, 90f),
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