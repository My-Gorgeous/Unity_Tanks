using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // 镜头移动的时间间隔
    public float m_DampTime = 0.2f;
    public float m_ScreenEdgeBuffer = 4f;
    // 最小的镜头大小，避免一直放大
    public float m_MinSize = 6.5f;      
    // HideInInspector表示不在Inspector面板中显示，即不可在Unity编辑器中指定
    [HideInInspector] public Transform[] m_Targets;    // 场景中所有坦克的transform组件数组


    private Camera m_Camera;             // 镜头对象的引用
    private float m_ZoomSpeed;           //          
    private Vector3 m_MoveVelocity;      //           
    private Vector3 m_DesiredPosition;   //           


    private void Awake()
    {
        // GetComponentInChildren获取到的是满足条件的第一个子对象
        m_Camera = GetComponentInChildren<Camera>();
    }

    // 因为Tank的移动是在FixedUpdate()函数中实现的，而镜头需要跟随移动，所以镜头也在该函数中移动
    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        // 1.找到场景中存活坦克间的平均距离
        FindAveragePosition();
        // 平滑移动到指定位置
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    // 找到所有被激活坦克中间的位置，为m_DesiredPosition赋值
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            // 如果坦克没有被激活，就不计入计算，跳过
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // 将所有被激活Tank的position求和
            averagePos += m_Targets[i].position;
            // 被激活坦克的数量加1
            numTargets++;
        }

        // 如果被激活坦克的数量大于0，求position的平均值
        if (numTargets > 0)
            averagePos /= numTargets;

        // 确保镜头不会上下偏移，因为Tank的y轴Position是固定的，因此镜头的y轴也不需要改变，每次保持和前面一致就好了
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        // 计算需要的orthographic镜头的大小
        float requiredSize = FindRequiredSize();
        // 设置orthographic镜头大小为计算得到的大小
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        // 根据镜头的相对位置计算CameraRig的Global坐标
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        // 找到所有坦克距离中心的距离，取最大值
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);
        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}