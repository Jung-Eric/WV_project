using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_Enemy_Head : MonoBehaviour
{
    //분리된 남은 string
    public string EnemyAll;          //전체 정보
    //string EenemyArea_string; //영역 지정
    //string EnemyCount_string; //기본 지정
    string EnemyDetail;       //세부 이동정보

    //명령 목표 좌표---------------------------
    Vector2 target_pos;
    bool target_on;

    // 적 종류(프리팹)--------------------------
    public GameObject[] enemy_prefabs_type01;
    public GameObject[] enemy_prefabs_type02;
    public GameObject[] enemy_prefabs_type03;


    /*
    public GameObject enemy_type01; //기본 적
    public GameObject enemy_type02; //대형 적
    public GameObject enemy_type03; //경화 적

    public GameObject rader_type01; //레이더 적

    public GameObject detector_type01; //탐지 수리 적
    */

    //특정 수 만큼 병사들을 소환한다.----------
    //public GameObject obj; //임시 오브젝트
    int enemy_count = 0;


    //영역 관리--------------------------------
    int area_count = 0;
    List<int> area_x1 = new List<int>();
    List<int> area_y1 = new List<int>();
    List<int> area_x2 = new List<int>();
    List<int> area_y2 = new List<int>();

    //적 대략적 정보---------------------------
    List<int> enemy_manager_type = new List<int>();
    List<int> enemy_manager_num = new List<int>();
    // 하나를 두 개로 만든다.

    //기존의 것을 2차원 배열로 개선
    public int[,] type_manager = new int[15, 15];

    //int[] type_0 = new int[15]; //각자의 개수
    //int[] type_1 = new int[15];
    //int[] type_2 = new int[15]; //나머지는 나중에 추가

    //캐릭터들 실제 관리
    public List<GameObject> Enemy_1_1 = new List<GameObject>();
    public List<GameObject> Enemy_1_2 = new List<GameObject>();
    public List<GameObject> Enemy_1_3 = new List<GameObject>();


    public cells[,] map_cell = new cells[512, 128];

    // Start is called before the first frame update
    void Start()
    {
        //우선 string 기반으로 정보를 넣고
        string_slice();
        //정보를 기반으로 적을 소환한다.
        summon_enemy();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    // string 읽기, 
   void string_slice()
    {
        //초기화 인수
        int string_reader_index = 0;

        //임시 인수
        string string_imp;
        int int_imp;
        int int_imp2;
        int int_imp3;

        //최초 영역 지정
        string_imp = EnemyAll.Substring(string_reader_index,1);
        string_reader_index += 1;
        area_count = Convert.ToInt32(string_imp, 16);

        //영역 개수만큼 영역 지정
        for(int i = 0; i< area_count; i++)
        {
            //영역 좌표 지정
            string_imp = EnemyAll.Substring(string_reader_index, 3);
            string_reader_index += 3;
            int_imp = Convert.ToInt32(string_imp, 16);
            area_x1.Add(int_imp);

            string_imp = EnemyAll.Substring(string_reader_index, 3);
            string_reader_index += 3;
            int_imp = Convert.ToInt32(string_imp, 16);
            area_y1.Add(int_imp);

            string_imp = EnemyAll.Substring(string_reader_index, 3);
            string_reader_index += 3;
            int_imp = Convert.ToInt32(string_imp, 16);
            area_x2.Add(int_imp);

            string_imp = EnemyAll.Substring(string_reader_index, 3);
            string_reader_index += 3;
            int_imp = Convert.ToInt32(string_imp, 16);
            area_y2.Add(int_imp);

        }

        //돼지 대략적 정보
        while (true)
        {
            string_imp = EnemyAll.Substring(string_reader_index, 1);
            string_reader_index += 1;
            int_imp = Convert.ToInt32(string_imp, 16);

            if(int_imp != 15)
            {
                break;
            }
            else
            {
                //우선15가 아닐 경우 종류로 처리
                enemy_manager_type.Add(int_imp);

                string_imp = EnemyAll.Substring(string_reader_index, 1);
                string_reader_index += 1;
                int_imp2 = Convert.ToInt32(string_imp, 16);
                enemy_manager_num.Add(int_imp2);

                
                //종류별 대수 받기
                string_imp = EnemyAll.Substring(string_reader_index, 2);
                string_reader_index += 2;
                int_imp3 = Convert.ToInt32(string_imp, 16);

                type_manager[int_imp, int_imp2] = int_imp3;

            }

        }

        //이 문자열을 기반으로 세부 정보를 처리한다.
        EnemyDetail = EnemyAll.Substring(string_reader_index);

    }


 

    // 유닛 소환
    void summon_enemy()
    {
        //초기화 인수
        int string_reader_index = 0;

        //기본 인수
        int count_imp = enemy_manager_type.Count;
        int int_imp;
        int int_imp1;

        int int_inp1;
        int int_inp2;
        int int_inp3;
        int int_inp4;
        int int_inplen;
        int int_inpcir;
        string string_imp;

        //인수에 따라 적 생성
        for(int i=0; i<count_imp; i++)
        {
            int_imp = enemy_manager_type[i];
            int_imp1 = enemy_manager_num[i];

            /*
            if (int_imp == 0)
            {


                GameObject Instance = Instantiate(enemy_prefabs_type01[int_imp1]);
            }
            */

            
            switch (int_imp)
            {
                //1번 타입 생성
                case 0:
                    for(int k=0; k<type_manager[int_imp,int_imp1]; i++)
                    {

                        GameObject Instance = Instantiate(enemy_prefabs_type01[int_imp1]);

                        //기본 정보-------------------------------------------------------------------
                        string_imp = EnemyDetail.Substring(string_reader_index, 2);
                        string_reader_index += 2;
                        int_inp1 = Convert.ToInt32(string_imp, 16);

                        int_inp2 = int_inp1 >> 2; //이동 총거리
                        int_inplen = int_inp2; //길이 임시 저장

                        int_inp3 = int_inp1 & 2; //순환정보
                        int_inpcir = int_inp3;
                        int_inp4 = int_inp1 & 1; //시작 방향

                        Instance.GetComponent<AI_Enemy>().set_base(int_inp2, int_inp3, int_inp4);


                        //기본 좌표와 영역 적용---------------------------------------------------------
                        string_imp = EnemyDetail.Substring(string_reader_index, 3);
                        string_reader_index += 3;
                        int_inp1 = Convert.ToInt32(string_imp, 16);

                        string_imp = EnemyDetail.Substring(string_reader_index, 3);
                        string_reader_index += 3;
                        int_inp2 = Convert.ToInt32(string_imp, 16);

                        string_imp = EnemyDetail.Substring(string_reader_index, 2);
                        string_reader_index += 2;
                        int_inp3 = Convert.ToInt32(string_imp, 16);

                        Instance.GetComponent<AI_Enemy>().set_pos(int_inp1, int_inp2, int_inp3);

                        //영역 설정-----------------------------------------------------------------
                        string_imp = EnemyDetail.Substring(string_reader_index, 1);
                        string_reader_index += 1;
                        int_imp = Convert.ToInt32(string_imp, 16);

                        if(int_imp == 0)
                        {
                            Instance.GetComponent<AI_Enemy>().set_area(int_imp, 0, 0, 0, 0);
                        }
                        else{
                            Instance.GetComponent<AI_Enemy>().set_area(int_imp, area_x1[int_imp], area_y1[int_imp], area_x2[int_imp], area_y2[int_imp]);
                        }


                        //패트롤 정보-----------------------------------------------------------------
                        for(int p=0; p < int_inplen; p++)
                        {
                            string_imp = EnemyDetail.Substring(string_reader_index, 1);
                            string_reader_index += 1;
                            int_inp1 = Convert.ToInt32(string_imp, 16);
                            Instance.GetComponent<AI_Enemy>().Add_patrol(int_inp1);
                        }

                        //순환 여부에 따라서
                        //패트롤 대기 시간 받아오기 수행한다.
                        if (int_inpcir == 0)
                        {
                            string_imp = EnemyDetail.Substring(string_reader_index, 2);
                            string_reader_index += 2;
                            int_inp1 = Convert.ToInt32(string_imp, 16);

                            string_imp = EnemyDetail.Substring(string_reader_index, 2);
                            string_reader_index += 2;
                            int_inp2 = Convert.ToInt32(string_imp, 16);

                            Instance.GetComponent<AI_Enemy>().seeker_time(int_inp1, int_inp2);
                        }


                        //마지막으로 관리를 용이하게 만들기 위해 배열 list로 처리한다.
                        

                    }
                    break;

                case 1:
                    
                    break;

                case 2:

                    break;

                default:
                    break;
            }
            

            //string_imp = EnemyDetail.Substring(string_reader_index, 3);


        }
        
    }

    // 문자열 읽기
    /*
    int string_reader(string imp, int count)
    {
        string string_imp = imp.Substring(count, 2);
        count = 
        return 1;
    }
    */

    void add_GameObject(GameObject imp, int a, int b)
    {
        switch (a)
        {
            case 0:
                switch (b)
                {
                    case 0:
                        Enemy_1_1.Add(imp);
                        break;
                    case 1:
                        Enemy_1_2.Add(imp);
                        break;
                    case 2:
                        Enemy_1_2.Add(imp);
                        break;
                    //나머지는 추후 추가
                }
                break;

            case 1:

                break;

            case 2:

                break;

        }
    }

    void enemy_toString()
    {

    }

}
