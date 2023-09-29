using UnityEngine;

public class TankMovement : MonoBehaviour
{
    /*
     * 关于变量的几点说明：
     *    ① “m_”开头的变量表示成员变量
     *    ② public修饰的变量会显示在Unity的Inspector面板中，而private修饰的变量不会
     */

    public int m_PlayerNumber = 1;        // 玩家编号（玩家1、玩家2）      
    public float m_Speed = 12f;           // 坦克行驶的速度      
    public float m_TurnSpeed = 180f;      // 坦克每次转过的角度
    public AudioSource m_MovementAudio;   // 坦克行驶时的音频组件，在Unity中将Tank对象的第一个Audio Source赋给它
    public AudioClip m_EngineIdling;      // 音源1，在Unity编辑器中赋值
    public AudioClip m_EngineDriving;     // 音源2，在Unity编辑器中赋值
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     // 行驶方向轴的名称（Vertical1、Vertical2）
    private string m_TurnAxisName;         // 转弯方向轴的名称（Horizontal1、Horizontal2）
    private Rigidbody m_Rigidbody;         // 坦克对象的引用
    private float m_MovementInputValue;    // 行驶轴（Vertical）的输入大小
    private float m_TurnInputValue;        // 转弯轴（Horizontal）的输入大小
    private float m_OriginalPitch;

    // 脚本加载的时候执行的代码，不管脚本有没有被激活，都会执行
    private void Awake()
    {
        // 获取到坦克对象的引用
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // 当物体被激活的时候执行的代码，在Update()函数之前被执行
    private void OnEnable ()
    {
        // isKinematic是使其不受任何力，即初始化坦克的受力为0
        // 即不受物理引擎的影响，接受物理作用，但没有物理效果
        m_Rigidbody.isKinematic = false;
        // 输入重置为0
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    // 当物体被取消激活的时候执行的代码
    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // 根据玩家编号当前初始化输入轴的名称，便于后续处理输入
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        // Tank第一个Audio Source组件的pitch
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    // 存储玩家的输入，并且确保引擎的声音被播放
    private void Update()
    {
        // 随后会自动调用FixedUpdate()函数根据用户输入实现坦克的移动
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
        // 播放音频
        EngineAudio();
    }

    // 基于坦克是否正在移动，以及坦克当前播放的音源播放音频
    private void EngineAudio()
    {
        // 如果坦克没有移动，播放EngineIdling音源
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // 如果当前播放的就是EngineDriving，改成EngineIdling
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);

                // 更改音源后应该重新播放(Audio Source的使用，改变任何一个属性后都应该重新调用Play进行重新播放)
                m_MovementAudio.Play();
            }
        }
        // 如果坦克正在移动，播放EngineDriving音源
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);

                // 更改音源后应该重新播放(Audio Source的使用，改变任何一个属性后都应该重新调用Play进行重新播放)
                m_MovementAudio.Play();
            }
        }
    }

    // 这个函数会在每个固定的物理时间片被调用一次
    // 这是放置游戏基本物理行为代码的地方。UPDATE之后调用。
    private void FixedUpdate()
    {
        // 坦克的移动
        Move();
        Turn();
    }

    // 基于用户的输入，计算Tank该前进多远，调整Tank的Position
    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    // 基于用户的输入，调整Tank的Rotation
    private void Turn()
    {
        // 计算角度Quaternion（transform组件的Rotation属性）
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        // 将Vector3转换为Quaternion角度，沿Y轴旋转
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}