using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WV_LoadScript : MonoBehaviour
{
    /*
    // Start is called before the first frame update
    void Start()
    {
        LoadScript();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //아이템 받아와서 처리
    private string[] scriptName = { "Item List" };

    private int[] scriptOrderIdx = { 4, 4, 4, 2, 2 };

    public static Dictionary<string, List<Script>> scriptDic = new Dictionary<string, List<Script>>();

    /*
    public void LoadScript()
    {
        for (int k = 0; k < scriptName.Length; k++)
        {
            TextAsset textasset = Resources.Load("Scripts/" + scriptName[k]) as TextAsset;
            string loadstring = textasset.text;
            JObject loadScript = JObject.Parse(loadstring);

        }

    }
    */
    /*
    public void LoadScript2()
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


    */
}
