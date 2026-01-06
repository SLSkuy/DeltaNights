using UnityEngine;
using UnityEngine.EventSystems;
using WeaponSystem.Weapon;

public class Bullet : MonoBehaviour
{
    private Ray _centerRay;
    private float _moveSpeed = 100f;
    public Rifle _rifle;
    void Start()
    {
        // 初始化时确定方向（只执行一次）
        _centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // 让子弹朝向移动方向
        transform.rotation = Quaternion.LookRotation(_centerRay.direction);
        Destroy(gameObject,4);//4秒后销毁，防止向上射无碰撞
    }

    void Update()
    {
        
        transform.position += _centerRay.direction *_moveSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Destroy(this.gameObject);
    }
    public void setRifle(Rifle rifle)
    {
        _rifle = rifle;
    }
}
