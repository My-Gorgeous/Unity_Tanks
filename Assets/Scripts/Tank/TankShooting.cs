using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;         // 玩家编号
    public Rigidbody m_Shell;              // 子弹对象的引用
    public Transform m_FireTransform;      // FireTransform对象的引用
    public Slider m_AimSlider;             // 瞄准滑条的引用
    public AudioSource m_ShootingAudio;    // Tnak对象的第2个Audio Source组件，在Unity中指定
    public AudioClip m_ChargingClip;       // 蓄力的音频
    public AudioClip m_FireClip;           // 发射子弹的音频
    public float m_MinLaunchForce = 15f;   // 与AimSlider的最值相同
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;  // 蓄能时间

    
    private string m_FireButton;           // "Fire" + m_PlayerNumber
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        // 速度 = （最大值 - 最小值）/ 蓄能时间
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    // 追踪当前玩家开火键的状态（是否按下、长按），并根据当前的发射力做出决定
    private void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;

        // 如果蓄能到最大值，并且没有发射
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // 是不是第一次按下开火键？
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // 播放蓄能音频
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // 按住了开火键，但是还没有发射
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // 松开了开火键，但是还没有发射
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }

    // Instantiate and launch the shell.
    private void Fire()
    {
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}