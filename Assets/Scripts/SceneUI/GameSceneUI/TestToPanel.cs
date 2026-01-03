using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 添加这个命名空间

namespace SceneUI.GameSceneUI
{
    /// <summary>
    /// UI功能测试脚本
    /// </summary>
    public class UITestController : MonoBehaviour
    {
        private GameSceneUIManager uiManager;
        private Keyboard keyboard;
        
        void Start()
        {
            uiManager = FindAnyObjectByType<GameSceneUIManager>();
            keyboard = Keyboard.current; // 获取当前键盘设备
        }
        
        void Update()
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.current;
                return;
            }
            
            // 按H键受伤（减少10点血）
            if (keyboard.hKey.wasPressedThisFrame)
            {
                if (uiManager != null)
                {
                    uiManager.TestPlayerTakeDamage(10f);
                }
            }
            
            // 按F键射击（减少1发子弹）
            if (keyboard.fKey.wasPressedThisFrame)
            {
                if (uiManager != null)
                {
                    uiManager.TestPlayerFire();
                }
            }
            
            // 按R键换弹（重新装满子弹）
            if (keyboard.rKey.wasPressedThisFrame)
            {
                if (uiManager != null)
                {
                    uiManager.TestPlayerReload();
                }
            }
            
            // 按数字键1测试直接发送事件
            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                // 模拟受伤事件
                Signals.Get<GameSceneUIManager.PlayerHealthChangedSignal>().Dispatch(50f, 100f);
            }
            
            // 按数字键2测试直接发送事件
            if (keyboard.digit2Key.wasPressedThisFrame)
            {
                // 模拟弹药变化事件
                Signals.Get<GameSceneUIManager.PlayerAmmoChangedSignal>().Dispatch(10, 30);
            }
        }
    }
}