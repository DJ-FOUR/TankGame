using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;

public class MyTankMovement : MonoBehaviour
{
    public int PlayerNumber = 1;
    public float Speed = 20;
    public float TurnSpeed = 180;
    public AudioSource MovementAudio;
    public AudioClip EngineIdling;
    public AudioClip EngineDriving;
    public float PitchRange = 0.2f;


    private Rigidbody rb;
    private string MovementAxisName;
    private string TurnAxisName;
    private float MovementInputValue;
    private float TurnInputValue;
    private float OriginalPitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        OriginalPitch = MovementAudio.pitch; 

    }
    // Start is called before the first frame update
    void Start()
    {
        MovementAxisName = "Vertical"+PlayerNumber;
        TurnAxisName = "Horizontal"+PlayerNumber;
    }

    // Update is called once per frame
    void Update()
    {
        //W S移动
        Move();
        //A D旋转
        Turn();
        //播放声音
        EngineAudio();
    }
    private void FixedUpdate()
    {
        
    }
    void EngineAudio()
    {
        if (Mathf.Abs(MovementInputValue) < 0.01 && Mathf.Abs(TurnInputValue) < 0.01) //坦克不动
        {
            if (MovementAudio.clip == EngineDriving)
            {
                MovementAudio.clip = EngineIdling;
                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange);
                MovementAudio.Play();
            }
        }
        else
        {
            if (MovementAudio.clip == EngineIdling)
            {
                MovementAudio.clip = EngineDriving;
                MovementAudio.Play();
            }
        }
    }
    void Move()
    {
        MovementInputValue = Input.GetAxis(MovementAxisName);
        Vector3 movementV3 = transform.forward * MovementInputValue * Speed * Time.deltaTime;
        rb.MovePosition(rb.position + movementV3);
    }
    void Turn()
    {
        TurnInputValue = Input.GetAxis(TurnAxisName);
        float turn = TurnInputValue * Time.deltaTime * TurnSpeed;
        Quaternion quaternion = Quaternion.Euler(0, turn, 0);
        rb.MoveRotation(rb.rotation * quaternion);
    }
}
