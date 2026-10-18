using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class MyTankShooting : MonoBehaviour
{
    public int PlayerNumber = 1; //坦克编号
    public GameObject FireTransform; //炮弹位置
    public Rigidbody Shell; //炮弹模型
    public Slider AimSlider; //蓄力条UI
    public AudioSource ShootingAudio;
    public AudioClip ChargingClip;
    public AudioClip FireClip;
    public float MinLaunchForce = 15;
    public float MaxLaunchForce = 30;
    public float ChargeTime = 1.5f;

    private float ChargeSpeed;
    private float CurrentLaunchForce;
    private string FireButton;
    private bool Fired; //短时间内是否发射过

    //每次启用时把蓄力条重置到最小力度
    void OnEnable()
    {
        CurrentLaunchForce = MinLaunchForce;
        AimSlider.value = MinLaunchForce;
    }

    // Start is called before the first frame update
    void Start()
    {
        FireButton = "Fire" + PlayerNumber;
        ChargeSpeed=(MaxLaunchForce-MinLaunchForce)/ChargeTime;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(FireButton)){
            Fired = false;
            CurrentLaunchForce = MinLaunchForce;
            AimSlider.value = MinLaunchForce;
            //播放蓄力音频
            ShootingAudio.clip = ChargingClip;
            ShootingAudio.Play();
        }
        else if (Input.GetButton(FireButton) && Fired == false)
        {

            CurrentLaunchForce += ChargeSpeed * Time.deltaTime;
            AimSlider.value = CurrentLaunchForce; //蓄力条跟随当前力度增长

            if (CurrentLaunchForce >= MaxLaunchForce)
            {
                CurrentLaunchForce = MaxLaunchForce;
                Fire(); 
            }
        }
        else if (Input.GetButtonUp(FireButton) && Fired == false)
        {
            Fire();
        }
    }
    void Fire()
    {
        Fired = true;
        AimSlider.value = MinLaunchForce;
        //发射音频
        Rigidbody gameObjectInstance = Instantiate(Shell, FireTransform.transform.position, FireTransform.transform.rotation) as Rigidbody;
        Vector3 fwd = FireTransform.transform.forward; //炮弹发射方向
        gameObjectInstance.velocity = fwd * CurrentLaunchForce;

        //播放发射音频
        ShootingAudio.clip = FireClip;
        ShootingAudio.Play();
    }
 
}
