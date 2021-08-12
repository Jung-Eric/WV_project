using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    static public DatabaseManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }


    public List<MyEvent> m_events = new List<MyEvent>();
    public List<string> m_colors = new List<string>();

    public void AddEvent(string name)
    {
        m_events.Add(new MyEvent(name));
    }

    public void SetEventClear(string name)
    {
        for (int i = 0; i < m_events.Count; i++)
        {
            if (m_events[i].script_name == name)
            {
                m_events[i].clear = true;
            }
        }
    }
    
    public bool GetEventClear(string name)
    {
        for (int i = 0; i < m_events.Count; i++)
        {
            if (m_events[i].script_name == name)
            {
                return m_events[i].clear;
            }
        }
        return false;
    }

    public void AddColor(string name)
    {
        m_colors.Add(name);
    }
    public bool GetColor(string name)
    {
        return m_colors.Contains(name);
    }


    [SerializeField]
    public class MyEvent
    {
        public string script_name;
        public bool clear;

        public MyEvent(string name)
        {
            script_name = name;
            clear = false;
        }
    }
}