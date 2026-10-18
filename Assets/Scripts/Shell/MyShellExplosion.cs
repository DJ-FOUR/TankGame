using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class MyShellExplosion : MonoBehaviour
{
    public LayerMask TankMask;                  //图层
    public float MaxDamage = 100;
    public float ExplosionForce = 1000f;        //炮弹爆炸推力
    public float ExplosionRadius = 5f;          //炮弹爆炸半径
    public float MaxLifeTime = 2f;              //炮弹生存时间
    public ParticleSystem ExplosionParticles;
    public AudioSource ExplosionSound;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,MaxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //炮弹范围命中坦克
        //获取方圆半径为r的球里的TankMask图层里的物体（炮弹爆炸范围内存在坦克），存入targetCollider数组中
        Collider[] targetCollider = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);
        for (int i = 0; i < targetCollider.Length; i++)
        {
            //获取爆炸范围内的坦克的刚体组件
            Rigidbody target = targetCollider[i].GetComponent<Rigidbody>();
            if (target == null)
            {
                continue;
            }
            target.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

            //坦克扣血
            float damage = CalculateDamage(targetCollider[i].ClosestPoint(transform.position));     //返回碰撞体表面离给定点最近的点
            TankHealth targetTankHealth= targetCollider[i].GetComponent<TankHealth>();
            targetTankHealth.TakeDamage(damage);
        }

        //炮弹的效果
        ExplosionParticles.transform.parent = null;             //爆炸特效
        ExplosionParticles.Play();
        ExplosionSound.transform.parent = null;                 //爆炸音效
        ExplosionSound.Play();
        //销毁爆炸音效残留
        Destroy(ExplosionSound.gameObject, ExplosionSound.clip.length);
        //销毁爆炸特效残留
        Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);
        //销毁炮弹
        Destroy(gameObject);
    }
    //计算伤害
    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 v3 = targetPosition - transform.position;
        float distance = v3.magnitude;
        float damage = (ExplosionRadius - distance) / ExplosionRadius * MaxDamage;
        damage = Mathf.Max(0, damage);
        return damage;
    }
}
