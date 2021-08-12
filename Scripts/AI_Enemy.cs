using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Enemy : MonoBehaviour
{
    //탐색 대상인 대상들의 배열을 받는다.
    int serch_count; //대상 개수



    //위치 좌표와 보조 좌표
    Transform pos;
    Vector3 enemy_pos = new Vector3(0,0,0);  //위치 지정 목적
    Vector3 position;
    public float base_speed = 0.1f;
    public float jump_power = 15f;


    int patrol_x, patrol_y; // 패트롤 시작 점

    //int base_x, base_y; //항상 지정되는 기본 위치(자료형 때문에) 없어질 가능성 높음
    
    int now_x, now_y; //이동을 위한 추가 좌표를 계속 갱신해준다. 이는 int 형으로 관리해서 명령을 수행하도록 만든다.

    //기본 정보
    int patrol_len;     //기본 정찰 거리
    int patrol_index;   //정찰 시작 좌표 인덱스
    int patrol_cir;   //순환여부
    int direction;     //유닛 방향 (0 / 1)

    //비순환이면 각 끝에 적용되면 기존 경로의 시간
    float reach_1;
    float reach_2;


    Vector2 base_pos;  //기본 좌표(나중에 귀환 시 돌아가는 좌표가 된다), 고정 값에 해당한다.


    //경로 방향 정보(0~7)
    public List <int> patrol_path = new List<int>();   //경로 리스트, len에 의거해 관리

    //편한 작업을 위해 경로룰 좌표로 저장
    List<int> patrol_posX = new List<int>();
    List<int> patrol_posY = new List<int>();


    //이동 제한 영역 범위, 영역에 속해있는지도 지정
    bool area_limit; //0이면 자유롭게 이동
    int area_num;   //area를 가지게 된다.
    int area_x1, area_y1, area_x2, area_y2;


    //경계 레벨
    int warning_level = 0;

    //이동 수준 - 0 기본 패트롤 / 1 탐색 목적 이동 / 2 시야 내 추적 
    int moving_mode = 0;

    //전환 시간
    float warn1_maintainT; //경로 시작
    float warn2_maintainT; //경로 끝(반환점)

    //착지 상태
    public bool is_landing = false;

    //유닛 위치와 방향(-1,1)
    //Vector2 enemy_pos;
    //public int direction;

    //적 탐지 좌표, 이게 우선이 된다.
    //만약 텔포로 사라지면 혼란 상태가 된다.
    Vector2 detect_pos; //
    bool detect_on;

    //탐지 대상
    int layerMask; //일반 대상 마스크
    int layerMask_enemy; // 대상 마스크

    List<Vector2> targets_posR; //탐지 필요 좌표 우측
    List<Vector2> targets_posL; //탐지 필요 좌표 좌측
    int targets_len;

    int layer_player, layer_stranger, layer_obstacle, layer_destroyed, layer_world;
    int layer_enemy;
    

    //명령 목표 좌표
    Vector2 target_pos;
    bool target_on;

    //4단계 시 무기 종류

    // Start is called before the first frame update
    void Start()
    {
        //시작 세팅
        layer_player = 1 << LayerMask.NameToLayer("Player");
        layer_stranger = 1 << LayerMask.NameToLayer("Stranger");
        layer_obstacle = 1 << LayerMask.NameToLayer("Obstacle");
        layer_destroyed = 1 << LayerMask.NameToLayer("Player_Destroyed");
        layer_world = 1 << LayerMask.NameToLayer("World");
        layer_enemy = 1 << LayerMask.NameToLayer("Enemy");
        //레이어 마스크
        layerMask = layer_player + layer_stranger + layer_obstacle + layer_destroyed + layer_world;
        layerMask = layer_world + layer_enemy;

        //경계, 행동
        warning_level = 0;
        moving_mode = 1;

        //위치 받아오기
        pos = gameObject.GetComponent<Transform>();
        //enemy_pos = new Vector2(0, 0);
        enemy_pos = transform.position;
        //enemytransform.position;


        //레이들을 최초 증록한다.
        RayManager_base();
        //listRay2D.Add(new Ray2D());

        detect_pos = new Vector2(0, 0);
        detect_on = false;
        target_pos = new Vector2(0, 0);
        target_on = false;


        //패트롤을 좌표 설정
        patrol_pos_setting();
    }

    // Update is called once per frame
    // 경계 별 행동 방식을 변화시킨다.
    void FixedUpdate()
    {
        //위치 지속적으로 수정,이 위치를 이동좌표 처리
        now_pos_setter();

        //탐지는 지속적으로 수행 (시야와 소리)
        detecting_sight();
        detecting_sound();

        //경비 또는 추적을 한다.
        enemy_move();

        //경계 수준 조정
        

    }

    //인수 세팅---------------------------------------------------------------

    public void set_base(int a, int b, int c)
    {
        patrol_len = a;
        patrol_cir = b;
        direction = c;
        //patrol_index = c;
    }


    public void set_pos(int a,int b,int c)
    {
        //enemy_pos = transform.position;
        patrol_x = a;
        patrol_y = b;
        enemy_pos.x = (float)a;
        enemy_pos.y = (float)b;
        patrol_index = c;
        pos.position = enemy_pos;

    }

    public void set_area(int k, int a, int b, int c, int d)
    {
        area_num = k;
        if(k == 0)
        {
            area_limit = false;
        }
        else
        {
            area_limit = true;
        }

        area_x1 = a;
        area_y1 = b;
        area_x2 = c;
        area_y2 = d;
    }

 


    //경로 추가해서 이동하게 만든다.
    public void Add_patrol(int a)
    {
        patrol_path.Add(a);
    }

    //좌측 우측 도착 시 관찰 시간
    public void seeker_time(int a, int b)
    {
        reach_1 = (float)(a * 100);
        reach_2 = (float)(b * 100);
    }


    //패트롤 좌표를 정리(정찰 정리 용)
    public void patrol_pos_setting()
    {
        int imp_x = patrol_x;
        int imp_y = patrol_y;

        //기본 시작좌표 추가
        patrol_posX.Add(imp_x);
        patrol_posX.Add(imp_y);


        for (int i = 0; i<patrol_len; i++)
        {
            switch (patrol_path[i])
            {
                case 0:
                    imp_x = imp_x - 1;
                    imp_y = imp_y + 1;
                    break;

                case 1:
                    imp_y = imp_y + 1;
                    break;

                case 2:
                    imp_x = imp_x + 1;
                    imp_y = imp_y + 1;
                    break;

                case 3:
                    imp_x = imp_x + 1;
                    break;

                case 4:
                    imp_x = imp_x + 1;
                    imp_y = imp_y - 1;
                    break;

                case 5:
                    imp_y = imp_y - 1;
                    break;

                case 6:
                    imp_x = imp_x - 1;
                    imp_y = imp_y - 1;
                    break;

                case 7:
                    imp_x = imp_x - 1;
                    break;
            }
            patrol_posX.Add(imp_x);
            patrol_posX.Add(imp_y);
        }

    }

    //-------------------------------------------------------------------------

    // now_x, now_y를 조정해준다.
    void now_pos_setter()
    {
        position = transform.position;
        now_x = Mathf.RoundToInt(position.x);
        now_y = Mathf.RoundToInt(position.y);
    }

    // 현재 패트롤 좌표에 있는지 확인한다.
    // 목표한 것보다 먼저 도달하면 그냥 넘어가기도 해준다.
    // 벗어났으면 -1을 반영한다.

    //이동용 좌표 측정으로 정밀 측정
    //now하고 정밀하게 유사한지 측정한다.
    bool precise_pos_setter()
    {
        position = transform.position;
        float imp_x, imp_y;

        imp_x = position.x - now_x;
        imp_y = position.y - now_y;

        if(-0.1<imp_x && imp_x < 0.1)
        {
            if(-0.1 < imp_y && imp_x < 0.1)
            {
                return true;
            }
        }

        return false;
    }

    // 자신이 경로에 있는지를 확인한다.
    bool now_on_patrol()
    {
        int len_x = patrol_posX.Count;
        int len_y = patrol_posY.Count;

        for(int i=0; i < len_x; i++)
        {
            if(now_x == patrol_posX[i])
            {
                if(now_y == patrol_posY[i])
                {
                    return true;
                }
            }
        }

        return false;
    }

    //레이 매니져를 통해 관리해준다.--------------------------------------
    //시작 레이 매니져, 유저의 정보를 더해준다.
    //추가 대상은 Enemy_Head에서 더해준다.
    void RayManager_base()
    {
        float[] imp_val = new float[8] {10,10,(float)9.5, (float)9.5,9,9,(float)8.5,8};

        for(int i=0; i<=7; i++)
        {
            add_vector(i,imp_val[i]);
        }
        add_vector((float)7.5, (float)7.5);

        targets_len = targets_posL.Count;

    }


    void add_vector(float imp_x, float imp_y)
    {
        Vector2 imp_1 = new Vector2(imp_x, imp_y);
        Vector2 imp_2 = new Vector2(imp_x, (-1)*(imp_y));

        targets_posR.Add(imp_1);
        targets_posR.Add(imp_2);

        imp_1 = new Vector2(imp_y, imp_x);
        imp_2 = new Vector2((-1) * (imp_y), imp_x);

        if (imp_x != imp_y)
        {
            targets_posR.Add(imp_1);
            targets_posR.Add(imp_2);
        }


        imp_1 = new Vector2((-1) * imp_x, imp_y);
        imp_2 = new Vector2((-1) * imp_x, (-1) * (imp_y));

        targets_posL.Add(imp_1);
        targets_posL.Add(imp_2);

        imp_1 = new Vector2(imp_y, (-1) * imp_x);
        imp_2 = new Vector2((-1) * (imp_y),(-1) * imp_x);

        if (imp_x != imp_y)
        {
            targets_posL.Add(imp_1);
            targets_posL.Add(imp_2);
        }
    }

    // 시야 관련, 레이캐스트 사용---------------------------------------------------------------
    // 방향(0,1)에 따라 바뀜
    // 발견한 물건에 따라 다른 경계 레벨을 취한다.
    void detecting_sight()
    {
        RaycastHit2D ray;
        Vector2 imp_vec = new Vector2(0,0);

        imp_vec.x = pos.position.x;
        imp_vec.y = pos.position.y + (float)0.5;


        //왼쪽, 오른쪽에 따라 레이의 방향을 조정
        //ray를 분석해서 위험 정도를 조정한다.
        if (direction == 0)
        {
            for(int i=0; i < targets_len; i++)
            {              
                ray = Physics2D.Raycast(imp_vec,targets_posL[i],10,layerMask);
                sight_rays(ray);
            }
        }
        else if(direction == 1)
        {
            for (int i = 0; i < targets_len; i++)
            {
                ray = Physics2D.Raycast(imp_vec, targets_posR[i], 10, layerMask);
                sight_rays(ray);
            }
        }

        //만약 레이캐스트로 발견했을 경우
        if (true)
        {
            //해당 좌표를 받는다.
            //
            detect_on = true;
            target_on = true;

            warning_level = 3;
        }
    }
    //
    void sight_rays(RaycastHit2D ray)
    {
        int imp;
        float imp_dist; //대상과의 거리
        if (!ray)
        {
            imp = ray.collider.gameObject.layer;
            imp_dist = ray.distance;
            if(imp == layer_player)
            {
                warn3();
            }
            else if(imp == layer_stranger)
            {
                warn3();
            }
            else if (imp == layer_obstacle)
            {
                warn2();
            }
            else if(imp == layer_destroyed)
            {
                warn3();
            }
            else
            {
                
            }


        }
    }


    // 소리 관련---------------------------------------------------------------
    void detecting_sound()
    {

    }


    //기본적인 경비와 이동(경우에 
    public void enemy_move()
    {
        
        //현재 위치
        now_pos_setter();

        //현재 패트롤 true false
        //정확한 위치까지 왔을 경우
        if (precise_pos_setter())
        {

        }

        position = transform.position;
        
        //position.x += base_speed * Input.GetAxisRaw("Horizontal");

        //우선 자신이 정찰 범위 안에 있는지 확인한다.
        if(!(moving_mode == 1 || moving_mode == 2))
        {





        }



        //정찰
        //만약 밖에 있다면 귀환을 사용한다.
        if (moving_mode == 0)
        {


        }
        //탐색(물건에 대한 탐색)
        else if(moving_mode == 1)
        {

        }
        //추적(3단계 이상)
        else if(moving_mode == 2)
        {

        }
        //귀환(특정 지점까지 이동)
        //탐색 알고리즘과 거의 동일
        else if(moving_mode == 3)
        {

        }




    }

    //경계 별 고유 함수 0~4
    void warn0()
    {
        //patrol();
    }


    void warn1()
    {
        //조건 부로 작동한다.
        if(warning_level < 1)
        {

        }


    }

    void warn2()
    {
        //조건 부로 작동한다.
        if (warning_level < 2)
        {

        }
    }

    void warn3()
    {
        //조건 부로 작동한다.
        if (warning_level < 3)
        {

        }
    }

    void warn4()
    {
        //조건 부로 작동한다.
        if (warning_level < 4)
        {

        }
    }

    //공격모드로 변경
    void attack_mode()
    {

    }

    // 경계수준 증감
    void warn_up()
    {

    }
  

    

    //
    
    string enemy_toString()
    {
        char pad = '0';

        string imp = "";
        string inp = "";
        int imp_int = 0;


        //---------------------------------------
        imp_int += (patrol_len << 2);
        imp_int += (patrol_cir << 1);
        imp_int += direction;

        inp = imp_int.ToString("X2");

        //기존 스트링에 연결
        imp += inp;

        //---------------------------------------
        //위치 정보
        imp_int = patrol_x;
        inp = imp_int.ToString("X3");
        imp += inp;

        imp_int = patrol_y;
        inp = imp_int.ToString("X3");
        imp += inp;

        imp_int = patrol_index;
        inp = imp_int.ToString("X2");
        imp += inp;

        //---------------------------------------
        //영역 정보, 영역 정보는 나중에 EnemyHead를 통해 받아온다.
        imp_int = area_num;
        inp = imp_int.ToString("X2");
        imp += inp;

        //---------------------------------------
        //순찰 리스트와 대기 시간(조건부)
        for (int p = 0; p<patrol_len; p++)
        {
            imp_int = patrol_path[p];
            inp = imp_int.ToString("X1");
            imp += inp;
        }


        if (patrol_cir == 0)
        {
            imp_int = (int)(reach_1 / 100);
            inp = imp_int.ToString("X2");
            imp += inp;

            imp_int = (int)(reach_2 / 100);
            inp = imp_int.ToString("X2");
            imp += inp;
        }

        return imp;
    }

}
