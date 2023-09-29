using System;
using UnityEngine;

[Serializable]   // 该修饰符的作用是使得其在被实例化时可以在Inspector面板中显示
public class TankManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;  // 玩家的对应字体的颜色
    [HideInInspector] public GameObject m_Instance;       // 存储实例化的Tank对象 
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       // TankMovement脚本引用
    private TankShooting m_Shooting;       // TankShooting脚本引用
    private GameObject m_CanvasGameObject;          // 用于显示UI界面，例如“ROUND1”……


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        // 从Canvas子对象中获取Text对象
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // 将Tank所有颜色都换为指定的颜色
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        // 使Tank对象上的TankMovement、TankShooting脚本失效
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        // 激活Tank对象上的TankMovement、TankShooting脚本
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
