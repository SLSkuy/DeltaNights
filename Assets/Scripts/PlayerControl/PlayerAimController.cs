using System.Collections.Generic;
using InputProcess;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 玩家视角控制器
    /// </summary>
    public class PlayerAimController : MonoBehaviour, IInputAxisOwner
    {
        #region 内部成员
        
        [Header("视角配置")]
        public float rotationDamping = 0.2f;
        
        [Header("输入轴配置")]
        [Tooltip("水平旋转，角度单位，0为居中")]
        public InputAxis horizontalLook = new () { Range = new Vector2(-180, 180), Wrap = true, Recentering = InputAxis.RecenteringSettings.Default };
        [Tooltip("垂直旋转，角度单位，0为居中")]
        public InputAxis verticalLook = new () { Range = new Vector2(-70, 70), Recentering = InputAxis.RecenteringSettings.Default };
        
        // 组件获取
        private PlayerController _controller;
        
        #endregion
        
        #region cinemachine输入控制
        
        /// <summary>
        /// 实现IInputAxisOwner接口
        /// 用于Cinemachine Input Axis Controller读取相应信息以获取Input System中的输入信息
        /// </summary>
        /// <param name="axes"></param>
        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new () { DrivenAxis = () => ref horizontalLook, Name = "Horizontal Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new () { DrivenAxis = () => ref verticalLook, Name = "Vertical Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
        }
        
        /// <summary>
        /// 编辑器更新时限定填入值在规定范围内
        /// </summary>
        void OnValidate()
        {
            horizontalLook.Validate();
            verticalLook.Range.x = Mathf.Clamp(verticalLook.Range.x, -90, 90);
            verticalLook.Range.y = Mathf.Clamp(verticalLook.Range.y, -90, 90);
            verticalLook.Validate();
        }
        
        #endregion

        #region 成员方法

        private void UpdatePlayerRotation()
        {
            // 旋转摄像机
            float h = horizontalLook.Value;
            float v = verticalLook.Value;
            transform.localRotation = Quaternion.Euler(v, h, 0);
            
            // 旋转玩家模型
            RecenterPlayer(rotationDamping);
            
            // 无输入时自动回正视角
            verticalLook.UpdateRecentering(Time.deltaTime, verticalLook.TrackValueChange());
            horizontalLook.UpdateRecentering(Time.deltaTime, horizontalLook.TrackValueChange());
        }

        /// <summary>
        /// 重新设置玩家当前朝向
        /// </summary>
        /// <param name="damping">重朝向需要的时间</param>
        private void RecenterPlayer(float damping = 0)
        {
            if (!_controller) return;

            if (!_controller.IsMoving()) return;
            
            // 获取玩家模型与当前朝向角度
            var rot = transform.localRotation.eulerAngles;
            rot.y = NormalizeAngle(rot.y);
            var delta = rot.y;
            delta = Damper.Damp(delta, damping, Time.deltaTime);
            
            // 更新玩家模型朝向到当前朝向
            _controller.transform.rotation = Quaternion.AngleAxis(
                delta, _controller.transform.up) *  _controller.transform.rotation;
            
            // 更新朝向角度，避免无限旋转
            horizontalLook.Value -= delta;
            rot.y -= delta;
            transform.localRotation = Quaternion.Euler(rot);
        }
        
        /// <summary>
        /// 限制角度在 -180 ~ 180 范围内
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }

        #endregion
        
        #region 周期函数
        
        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _controller = GetComponentInParent<PlayerController>();
            
            _controller.PreUpdate += UpdatePlayerRotation;
        }

        private void OnDisable()
        {
            _controller.PreUpdate -= UpdatePlayerRotation;
            
            _controller =  null;
        }
        
        #endregion
    }
}