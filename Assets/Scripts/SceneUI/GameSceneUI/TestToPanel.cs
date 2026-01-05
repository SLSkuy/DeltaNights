/* ------------------------------------------------------------
 *  Author:  2023051604020 QuZhiYao
 *  Date:  2025.12.27
 *  LastUpdate:  2025.1.4
 * 
 *  功能简述：
 *  UI功能测试脚本
 * 
 * ------------------------------------------------------------ */


using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
                    uiManager.PlayerTakeDamage(10f);
                }
            }
            
            // 按F键射击（减少1发子弹）
            if (keyboard.fKey.wasPressedThisFrame)
            {
                if (uiManager != null)
                {
                    uiManager.PlayerFire();
                }
            }
            
            // 按R键换弹（重新装满子弹）
            if (keyboard.rKey.wasPressedThisFrame)
            {
                if (uiManager != null)
                {
                    uiManager.PlayerReload();
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
                Signals.Get<GameSceneUIManager.PlayerAmmoChangedSignal>().Dispatch(30, 60);
            }
        }
    }
}