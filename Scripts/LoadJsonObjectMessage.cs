using System.Collections.Generic;
using UnityEngine;
using System;
//using Newtonsoft.Json.Linq;

[Serializable]
public class LoadJsonObjectMessage : MonoBehaviour
{
    /*
    private string[] messageIdx = { "name", "message" };
    private string[] location = { "Center", "Quest", "Color" };

    public static Dictionary<string, List<ObjectMessage>> messageDic { get; } = new Dictionary<string, List<ObjectMessage>>();


    void Start()
    {
        LoadMessage();
    }

    public void LoadMessage()
    {
        TextAsset textasset = Resources.Load("Data/ObjectMessage") as TextAsset;
        string loadstring = textasset.text;
        JObject loadData = JObject.Parse(loadstring);

        for (int j = 0; j < location.Length; ++j)
        {
            JArray loadArr = (JArray)loadData[location[j]];

            List<ObjectMessage> tmp_messages = new List<ObjectMessage>();
            for (int i = 0; i < loadArr.Count; i++)
            {
                string name, message;
                name = loadArr[i][messageIdx[0]].ToString();
                message = loadArr[i][messageIdx[1]].ToString();

                tmp_messages.Add(new ObjectMessage(name, message));
            }
            messageDic.Add(location[j], tmp_messages);
        }
    }


    [SerializeField]
    public class ObjectMessage
    {
        public string name;
        public string message;

        public ObjectMessage(string nm, string msg)
        {
            this.name = nm;
            this.message = msg;
        }
    }

    */
}