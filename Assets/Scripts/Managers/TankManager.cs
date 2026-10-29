using System;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;                 //坦克颜色
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;         //坦克编号     
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;      //在point上生成的坦克实例    
    [HideInInspector] public int m_Wins;                     


    private MyTankMovement m_Movement;       
    private MyTankShooting m_Shooting;
    private GameObject m_CanvasGameObject;

    public void Setup()
    {
        //获取坦克实例的移动，射击，UI组件
        m_Movement = m_Instance.GetComponent<MyTankMovement>();
        m_Shooting = m_Instance.GetComponent<MyTankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        //设置该坦克的玩家编号
        m_Movement.PlayerNumber = m_PlayerNumber;
        m_Shooting.PlayerNumber = m_PlayerNumber;
        //生成带颜色的玩家名文本
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        //获取坦克所有子物体的网格渲染组件存入数组
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
        //把坦克的每个子物体涂成玩家色
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        //执行组件里的OnEnable，在里面初速化
        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
