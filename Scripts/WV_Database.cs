using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WV_Database : MonoBehaviour
{
    /*
    static public WV_Database instance;

    // * 개인정보
    // ID, 이름, 코드명 등..
    public List<string> user = new List<string>();


    // * 아이템 (공용, 전용, 기타)
    // * 아이템 (파편)
    // * 아이템 (정수)
    public List<Item> items = new List<Item>();
    public List<Item> items2 = new List<Item>();
    public List<Item> items3 = new List<Item>();


    // * 맵 진행 정도외 정보를 포함
    // 0000 : 튜토리얼0번 진행 중 / 3076 : 3장 76번 맵
    public List<Game_Map> map = new List<Game_Map>();

    //처음 로드 시 DB를 받아오고 이를 유지한다.
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

    //유저 정보 더하기
    public void add_user(string user_s)
    {
        user.Add(user_s);
    }

    //아이템 추가
    public void add_item(Item item_i)
    {
        items.Add(item_i);
    }


    //맵 현황 더하기
    public void add_map(Game_Map maps)
    {
        map.Add(maps);
    }


    //아이템 추가 정도
    public int item_inc_amount(int code, int amount)
    {

        int num = items.Capacity;
        for(int i = 0; i<num; i++)
        {
            if(code == items[i].item_code)
            {
                //양 추가
                items[i].amount += amount;
                return 0;
            }
        }

        //탐색 실패
        return -1;
    }

    //아이템 감소 정도
    public int item_dec_amount(int code, int amount)
    {
        int num = items.Capacity;
        for (int i = 0; i < num; i++)
        {
            if (code == items[i].item_code)
            {
                //양 감소
                if (items[i].amount < amount)
                {
                    //현재 양 반영
                    return items[i].amount;
                }
                else
                {
                    items[i].amount -= amount;
                    return 0;
                }
                
            }
        }

        //탐색 실패
        return -1;
    }



    //기본 맵 진행 정보 클래스
    //미션 진행 정도(는 따로 나중에..)
    //세부 맵 정보는 따로 가진다.
    [SerializeField]
    public class Game_Map
    {
        //맵 이름
        //성공 여부
        public string map_name;
        public bool clear;
        
        public Game_Map(string name)
        {
            map_name = name;
            clear = false;
        }

    }

    //보유한 아이템 클래스
    [SerializeField]
    public class Item
    {
        //아이템 코드와 양
        public int item_code;
        public int amount;
        
    }

    */
 }
