using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;

[System.Serializable]
public class LoadJson : MonoBehaviour
{
    /*

    void Start()
    {
        LoadScript();
    }


    private string[] scriptName = { "뱃사공의 부탁", "동생 찾기", "새로운 시도", "보물상자", "오브젝트" };
    private string[] scriptIdx = { "name", "script", "chooseNum" };
    private string[] scriptOrder = { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth", "tenth" };

    private int[] scriptOrderIdx = { 4, 4, 4, 2, 2 };

    public static Dictionary<string, List<Script>> scriptDic = new Dictionary<string, List<Script>>();

    public void LoadScript()
    {
        for (int k = 0; k < scriptName.Length; k++)
        {
            TextAsset textasset = Resources.Load("Data/EventScript/" + scriptName[k]) as TextAsset;
            string loadstring = textasset.text;
            JObject loadScript = JObject.Parse(loadstring);

            List<Script> tmp_script = new List<Script>();

            for (int j = 0; j < scriptOrderIdx[k]; j++)
            {
                JArray loadArr = (JArray)loadScript[scriptOrder[j]];
                List<Script.innerScript> tmp_innerScript = new List<Script.innerScript>();
                for (int i = 0; i < loadArr.Count; i++)
                {
                    string n, s;
                    int c;

                    n = loadArr[i][scriptIdx[0]].ToString();
                    s = loadArr[i][scriptIdx[1]].ToString();
                    c = int.Parse(loadArr[i][scriptIdx[2]].ToString());
                    tmp_innerScript.Add(new Script.innerScript(n, s, c));
                }
                tmp_script.Add(new Script(scriptOrder[j], tmp_innerScript));
            }
            scriptDic.Add(scriptName[k], tmp_script);
        }
    }


    [SerializeField]
    public class Script
    {
        public string order;
        public List<innerScript> InnerScripts = new List<innerScript>();

        public Script(string order, List<innerScript> innerScripts)
        {
            this.order = order;
            InnerScripts = innerScripts;

        }
        [Serializable]
        public class innerScript
        {
            public string name;
            public string script;
            public int chooseNum;
            public bool finished;
            public bool accepted;

            public innerScript(string name, string script, int cnum)
            {
                this.name = name;
                this.script = script;
                this.chooseNum = cnum;
                finished = false;
                accepted = false;
            }
        }
    }
    */
}