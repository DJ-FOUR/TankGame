using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          //初始血量

    //UI控件
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    

    public GameObject m_ExplosionPrefab;            //爆炸预制体


    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;    //爆炸的例子特效
    private float m_CurrentHealth;                  //坦克当前血量
    private bool m_Dead;


    private void Awake()
    {
        //从爆炸预制体中获取粒子特效和声音播放组件
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        //把爆炸粒子特效置为不可用
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        //当前血量设置为初始血量
        m_CurrentHealth = m_StartingHealth;
        //坦克是否死亡
        m_Dead = false;

        SetHealthUI();
    }


    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;
        Debug.Log(m_CurrentHealth);
        if (m_CurrentHealth <= 0 && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;
        //播放爆炸粒子特效
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        //播放爆炸音效
        m_ExplosionAudio.Play();
        //坦克不激活
        gameObject.SetActive(false);
    }
}