using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                  
    public ParticleSystem m_ExplosionParticles;   // 子弹爆炸的特效    
    public AudioSource m_ExplosionAudio;          // 子弹爆炸的音效    
    public float m_MaxDamage = 100f;              // 造成的最大伤害   
    public float m_ExplosionForce = 1000f;        // 最大的爆炸力    
    public float m_MaxLifeTime = 2f;              // 子弹最长的生命周期   
    public float m_ExplosionRadius = 5f;          // 爆炸半径   


    private void Start()
    {
        // 在m_MaxLifeTime时间后销毁Shell对象
        Destroy(gameObject, m_MaxLifeTime);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        // 找到所有在shell对象Trigger范围内的Tank，对它们造成伤害
        // Trigger范围的圆心为transform.position，半径为m_ExplosionRadius，并且只捕获Players层的对象，即Tank对象
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        
        // 遍历该层的所有对象
        for (int i = 0; i < colliders.Length; ++i)
        {
            // 查看碰撞器中是否找得到Rigidbody
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            // 如果找不到，说明触发到的不是Tank，继续下次循环
            if (!targetRigidbody)
            {
                continue;
            }
            // 如果是Rigidbody，说明targetRigidbody是Tank对象，为Tank对象添加爆炸力
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            TankHealth targetTankHealth = targetRigidbody.GetComponent<TankHealth>();
            
            if (!targetTankHealth)
            {
                continue;
            }

            float damage = CalculateDamage(targetRigidbody.position);
            targetTankHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;
        // 播放特效和音效
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
        // 在子弹的生命周期后销毁粒子对象
        ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

        Destroy(gameObject);
    }

    // 基于Tank的位置计算应该受到的伤害
    private float CalculateDamage(Vector3 targetPosition)
    {
        // 计算坦克和子弹之间的矢量距离
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        // 根据距离比例计算伤害值
        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}