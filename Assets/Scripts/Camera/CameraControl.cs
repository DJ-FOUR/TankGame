//using System.Diagnostics;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 //相机运动所需时间
    public float m_ScreenEdgeBuffer = 4f;           //游戏画面边缘缓冲
    public float m_MinSize = 6.5f;                  //游戏画面最低尺寸
    [HideInInspector] public Transform[] m_Targets;     //坦克位置的集合


    private Camera m_Camera;   
    //存放相机缩放和移动时的速度
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;         
    //记录所有坦克之间的平均中心点
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();     //相机平移
        Zoom();     //相机缩放
    }

    //相机平移
    private void Move()
    {
        FindAveragePosition();      //得到坦克之间的中心点坐标给m_DesiredPosition
        //SmoothDamp是弹簧阻尼模型，带加速度和惯性（当前位置，目标位置，上一帧的速度，到达目标所需时间）
        //SmoothDamp返回的是新一帧下的绝对坐标
        //ref传变量的引用
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        
    }
    //寻找坦克之间的平均中心点m_DesiredPosition
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();     //得到坦克之间中心点的坐标
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)    //跳过已经阵亡的坦克
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }

    //相机缩放
    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }
    //得出正交视野大小
    private float FindRequiredSize()
    {
        //InverseTransformPoint 把世界坐标转成“以 CameraRig 为原点”的本地坐标
        //将坦克之间的平均中心点坐标转换为在游戏画面坐标系下的坐标
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);       
        float size = 0f;
        //当含有3个及以上数量的坦克的时候坦克与中心点差值不一样，需要遍历所有坦克
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;
            //将坦克坐标转换为游戏画面坐标系下的坐标
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
            //将坦克与坦克间的中心点在游戏画面坐标系下做差
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            //取大的作为正交视野大小
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    //初始化相机位置，正交视野大小，坦克间的平均中心点
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}