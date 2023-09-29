using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          // 初始时的血量
    public Slider m_Slider;                        // 血条HealthSlider对象的引用，在Unity中指定
    public Image m_FillImage;                      // 填充图Fill对象的引用，在Unity中指定
    public Color m_FullHealthColor = Color.green;  // 满血时的颜色
    public Color m_ZeroHealthColor = Color.red;    // 没血时的颜色
    public GameObject m_ExplosionPrefab;           // 坦克爆炸的特效TankExplosion对象
    
    
    private AudioSource m_ExplosionAudio;          // 坦克爆炸音效的引用，因为TankExplosion对象已经指定了，因此该变量只需要从m_ExplosionPrefab引用中获取该AudioSource即可
    private ParticleSystem m_ExplosionParticles;   // 坦克爆炸特效，也已经指定了，直接获取即可，无需赋值
    private float m_CurrentHealth;                 // 当前血量
    private bool m_Dead;                           // 坦克是否已死亡（血量耗尽以否）


    private void Awake()
    {
        // 创建TankExplosion对象，并获取该对象的粒子系统
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // 创建后先设置为未激活状态，后续需要使用时只需要激活即可
        // 这样的好处是，只需要创建一次对象，因为对象是不变的，因此只需要在需要时将其激活即可，减少内存消耗，更加高效
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        // 当Tank被激活时，初始化血量为满血，死亡状态为“未死”
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // 更新血条显示
        SetHealthUI();
    }

    // Tank承受伤害的函数，该函数由Shell的脚本调用
    // 参数amount即为子弹造成的伤害
    // 调整Tank的当前血量，基于当前血量更新血条的UI显示，并判断Tank是否死亡
    public void TakeDamage(float amount)
    {
        // 更新当前血量
        m_CurrentHealth -= amount;
        // 基于当前血量，更新血条显示
        SetHealthUI();
        // 判断Tank血量是否需要死亡，如果小于0，则调用死亡函数
        if (m_CurrentHealth <= 0 && !m_Dead)
        {
            OnDeath();
        }
    }

    // 根据当前血量设置血条的大小和颜色
    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;
        // 根据满血和没血确定残血的颜色
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    // 播放Tank的爆炸特效，并且取消激活Tank对象
    private void OnDeath()
    {
        m_Dead = true;
        // 播放特效
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // CameraController中统计的便是Active的Tank的数量，因此这里使用取消激活
        gameObject.SetActive(false);
    }
}