using UnityEngine;

namespace GameSetting.GameCamera
{
    [CreateAssetMenu(fileName = "GameCameraSetting", menuName = "GameSetting/GameCamera")]
    public class CameraSetting : ScriptableObject
    {
        [Header("灵敏度设置")]
        public float rotationDamping = 0.2f;
        [Range(1f,10f)]public float horizontalSensitive = 1f;
        [Range(1f,10f)]public float verticalSensitive = 1f;
    }
}
