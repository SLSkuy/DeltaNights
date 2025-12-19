using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace InputProcess
{
    /// <summary>
    /// 玩家输入捕获处理类
    /// </summary>
    public class GameInput : MonoBehaviour, IInputAxisOwner, ILocomotionInputSource ,ILookInputSource, IAttackSkillInputSource
    {
        public static GameInput Instance { get; private set; }

        [Header("移动输入配置")]
        [Tooltip("X轴移动 范围(-1,1) 控制左右移动")]
        public InputAxis moveX = InputAxis.DefaultMomentary;
        [Tooltip("Y轴移动 范围(-1,1) 控制前后移动")]
        public InputAxis moveZ = InputAxis.DefaultMomentary;
        [Tooltip("跳跃 值为0或1 控制垂直移动")]
        public InputAxis jump = InputAxis.DefaultMomentary;
        
        [Header("瞄准瞄准配置")]
        [Tooltip("水平旋转，角度单位，0为居中")]
        public InputAxis horizontalLook = new () { Range = new Vector2(-180, 180), Wrap = true, Recentering = InputAxis.RecenteringSettings.Default };
        [Tooltip("垂直旋转，角度单位，0为居中")]
        public InputAxis verticalLook = new () { Range = new Vector2(-70, 70), Recentering = InputAxis.RecenteringSettings.Default };
        [Tooltip("肩射 值为0或1 控制肩射状态")]
        public InputAxis shoulderAim = InputAxis.DefaultMomentary;
        [Tooltip("瞄准 值为0或1 控制开镜瞄准状态")]
        public InputAxis aim = InputAxis.DefaultMomentary;
        
        [Header("攻击技能输入配置")]
        [Tooltip("攻击输入")]
        public InputAxis attack = InputAxis.DefaultMomentary;
        [Tooltip("主动技能输入")]
        public InputAxis activeSkill = InputAxis.DefaultMomentary;
        [Tooltip("终极技能输入")]
        public InputAxis ultimateSkill = InputAxis.DefaultMomentary;
        
        /// <summary>
        /// 暴露属性，使用接口实现便于网络传输使用
        /// </summary>
        public ref InputAxis HorizontalLook => ref horizontalLook;
        public ref InputAxis VerticalLook => ref verticalLook;
        public float MoveX => moveX.Value;
        public float MoveZ => moveZ.Value;
        public float Jump => jump.Value;
        public float ShoulderAim => shoulderAim.Value;
        public float Aim => aim.Value;
        public float Attack => attack.Value;
        public float ActiveSkill => activeSkill.Value;
        public float UltimateSkill => ultimateSkill.Value;

        #region cinemachine输入控制
        
        /// <summary>
        /// 实现IInputAxisOwner接口
        /// 用于Cinemachine Input Axis Controller读取相应信息以获取Input System中的输入信息
        /// </summary>
        /// <param name="axes"></param>
        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new () { DrivenAxis = () => ref moveX, Name = "Move X", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new () { DrivenAxis = () => ref moveZ, Name = "Move Z", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new () { DrivenAxis = () => ref jump, Name = "Jump" });
            axes.Add(new () { DrivenAxis = () => ref shoulderAim, Name = "ShoulderAim" });
            axes.Add(new () { DrivenAxis = () => ref aim, Name = "Aim" });
            axes.Add(new () { DrivenAxis = () => ref horizontalLook, Name = "Horizontal Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new () { DrivenAxis = () => ref verticalLook, Name = "Vertical Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new () {DrivenAxis = () => ref attack, Name = "Attack"});
            axes.Add(new () {DrivenAxis = () => ref activeSkill, Name = "ActiveSkill"});
            axes.Add(new () {DrivenAxis = () => ref ultimateSkill, Name = "UltimateSkill"});
        }
        
        /// <summary>
        /// 编辑器更新时限定填入值在规定范围内
        /// </summary>
        private void OnValidate()
        {
            moveX.Validate();
            moveZ.Validate();
            jump.Validate();
            shoulderAim.Validate();
            aim.Validate();
            
            verticalLook.Range.x = Mathf.Clamp(verticalLook.Range.x, -90, 90);
            verticalLook.Range.y = Mathf.Clamp(verticalLook.Range.y, -90, 90);
            horizontalLook.Validate();
            verticalLook.Validate();
            
            attack.Validate();
            activeSkill.Validate();
            ultimateSkill.Validate();
        }
        
        #endregion
        
        #region 周期函数

        private void Awake()
        {
            Instance = this;
        }
        
        #endregion
    }
}
