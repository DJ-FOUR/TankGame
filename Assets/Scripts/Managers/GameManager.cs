using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;           //场景中的坦克数组


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        //生成坦克并初始化
        SpawnAllTanks();
        //把坦克的坐标位置提供给相机
        SetCameraTargets();

        StartCoroutine(GameLoop()); //协程
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //在point点上生成坦克预制体
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation);
            m_Tanks[i].m_PlayerNumber = i + 1;      //给坦克编号
            ////初始化坦克的玩家编号、玩家文本、坦克颜色
            m_Tanks[i].Setup();
        }
    }
    private void SetCameraTargets()
    {
        //targets数组里面存放了每个坦克的transform组件
        Transform[] targets = new Transform[m_Tanks.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }
        //把坦克坐标的集合给相机
        m_CameraControl.m_Targets = targets;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());   //回合开始
        yield return StartCoroutine(RoundPlaying());    //对战中
        yield return StartCoroutine(RoundEnding());     //回合结束

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());     //实现游戏循环
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();                              // 所有坦克回出生点、状态复位
        DisableTankControl();                         // 禁止玩家操作
        m_CameraControl.SetStartPositionAndSize();     // 相机瞬移到初始取景
        m_RoundNumber++;                              // 回合数 +1
        m_MessageText.text = "ROUND_" + m_RoundNumber;  // 显示 "ROUND_n" 

        yield return m_StartWait;   //协程返回
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();                 // 允许玩家操作
        m_MessageText.text = "";             // 清空提示文字
        while (!OneTankLeft())               // 只要还有 ≥2 辆存活
        {
            yield return null;               // 每帧检查一次
        }
        yield return null;                   // 多等一帧再结算
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();                        // 禁止玩家操作
        m_RoundWinner = null;                        // 先清空（默认平局）
        m_RoundWinner = GetRoundWinner();            // 找唯一存活的坦克
        if (m_RoundWinner != null)                   // 有人活下来（非平局）
        {
            m_RoundWinner.m_Wins++;                  // 赢家胜场 +1
        }
        m_GameWinner = GetGameWinner();              // 检查是否有人先到 5 胜
        string message = EndMessage();               // 拼装结算文字
        m_MessageText.text = message;                // 显示到屏幕

        yield return m_EndWait;                      // 暂停 3 秒展示结算
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}