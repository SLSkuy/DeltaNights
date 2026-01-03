using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{
    private Ray _centerRay;
    private float _moveSpeed = 100f;
    void Start()
    {
        // 初始化时确定方向（只执行一次）
        _centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //_moveDirection = centerRay.direction; // 已经是单位向量

        // 让子弹朝向移动方向
        transform.rotation = Quaternion.LookRotation(_centerRay.direction);
        Destroy(gameObject,4);
    }

    void Update()
    {
        //_centerRay = Camera.main.ViewportPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        //this.transform.Translate(_centerRay.direction.normalized * Time.deltaTime);
        transform.position += _centerRay.direction *_moveSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Destroy(this.gameObject);
    }
}
