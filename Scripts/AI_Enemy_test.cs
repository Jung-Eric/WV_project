using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Enemy_test : MonoBehaviour
{
    // 관련 함수를 
    // 

    //캐릭터 기본 정보
    Vector3 position; //좌표 처리용 벡터
    //public float base_speed_cross = 0.01f; //대각선 이동속도
    public float patrol_speed = 0.014f; //일반 이동속도
    public bool way_change;     //유닛 방향 (0 / 1)

    // 패트롤 시작 점(최초 위치 지점)
    public int patrol_x, patrol_y; // 패트롤 시작 점, 최초 시작점인 것은 절대로 아니다.
    //끝에 도달했을 경우 대기 시간
    float patrol_delay1, patrol_delay2;

    // 현재 좌표
    public int now_x, now_y; //이동을 위한 추가 좌표를 계속 갱신해준다. 이는 int 형으로 관리해서 명령을 수행하도록 만든다.
    //public int now_len; //탈선 문제 때문에 벗어나면 기록했다가 이동에 반영해준다.

    //순찰 정보
    public int patrol_blocks;     //기본 정찰 거리(순환 여부에 구애 받지 않는다)
    public int patrol_len;   //총 이동 거리, patrol_posXY 에 사용된다. 2n-2 비순환, n은 순환에 적용된다.
    //public int patrol_index;   //정찰 시작 좌표 인덱스
    public bool patrol_cir = false;   //순환여부 순환 시 true(n 적용), 비순환 시 false(2n-2 적용)

    public bool patrolBack = false; //귀환하는 경우 true로 바뀐다.
    public int patrol_last = -1;
    

    //경로 방향 정보(0~7)
    public List<int> patrol_path = new List<int>();   //경로 리스트, blocks에 의거해 관리

    //편한 작업을 위해 경로룰 좌표로 저장
    List<int> patrol_posX = new List<int>();
    List<int> patrol_posY = new List<int>();

    //착지 상태
    public bool is_landing = false;

    //경계 레벨
    int warning_level = 0;

    void Start()
    {
        patrol_pos_setting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        //위치 지속적으로 수정,이 위치를 이동좌표 처리
        now_pos_setter();

        //현재 패트롤 위에 있는지를 확인한다.
        //현재 위치한 패트롤 번호를 출력하고 만약 바깥에 있으면 -1 출력, 복귀 알고리즘 시행
        now_len_onPatrol();

        detecting_sight();
        enemy_move();
    }

    //-------------------------------------------------------------------------------

    /*
    private void now_warn_work()
    {
        switch(warning_level)
        {
            

        }
    }
    */

    //테스트용 경로 넣기 인수
    public void patrol_path_setter()
    {

    }

    // now_x, now_y를 조정해준다.
    void now_pos_setter()
    {
        position = transform.position;
        now_x = Mathf.RoundToInt(position.x);
        now_y = Mathf.RoundToInt(position.y);

    }

    // 현재 몇 번째 패트롤 위에 있는지 확인한다.
    int now_len_onPatrol()
    {
        //순환하는 경우
        if(patrol_cir == true)
        {
            for (int i = 0; i < patrol_len; i++)
            {
                if (patrol_posX[i] == now_x && patrol_posY[i] == now_y)
                {
                    return i;
                }
            }
        }
        //순환하지 않는 경우 patrol_cir == false
        else
        {
            //반환점에 도달하기 전 까지는
            if (patrolBack == false)
            {
                for (int i = 0; i < patrol_blocks-1; i++)
                {
                    if (patrol_posX[i] == now_x && patrol_posY[i] == now_y)
                    {
                        patrol_last = i;
                        return i;
                    }
                }
                if (patrol_posX[patrol_blocks - 1] == now_x && patrol_posY[patrol_blocks - 1] == now_y)
                {
                    patrol_last = (patrol_blocks - 1);
                    patrolBack = true;
                    return (patrol_blocks - 1);
                }

            }
            //시작점에 도달하기 전 까지는
            else if (patrolBack == true)
            {
                for (int i = patrol_blocks; i < patrol_len; i++)
                {
                    if (patrol_posX[i] == now_x && patrol_posY[i] == now_y)
                    {
                        patrol_last = i;
                        return i;
                    }
                }
                if (patrol_posX[0] == now_x && patrol_posY[0] == now_y)
                {
                    patrol_last = 0;
                    patrolBack = false;
                    return (0);
                }

            }

        }

        //모든게 아니면 -1 반환
        return -1;
    }

    //패트롤 좌표를 정리(정찰 정리 용)
    //이외에도 다양한 정찰 관련 정보를 정리한다. 순환에 따라 달라진다.
    public void patrol_pos_setting()
    {
        int imp_x = patrol_x;
        int imp_y = patrol_y;

        //patrolBack = false;

        if(patrol_cir == true)
        {
            patrol_blocks = patrol_path.Count;
            patrol_len = patrol_blocks;
        }
        else if(patrol_cir == false)
        {
            //비순환 왕복의 경우
            patrol_blocks = patrol_path.Count + 1;
            patrol_len = 2 * patrol_path.Count;
        }
        else
        {
            patrol_blocks = patrol_path.Count + 1;
            patrol_len = 2 * patrol_path.Count;
        }
        
        //기본 위치 넣기
        patrol_posX.Add(imp_x);
        patrol_posY.Add(imp_y);

        //이동을 위한 좌표 추가
        for (int i = 0; i < patrol_blocks-1; i++)
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
            patrol_posY.Add(imp_y);
        }
        //비순환은 돌아오는 것도 따로 추가해야 된다.
        if (patrol_cir == false)
        {
            for (int i = 1; i < patrol_blocks - 1; i++)
            {
                patrol_posX.Add(patrol_posX[patrol_blocks - 1 - i]);
                patrol_posY.Add(patrol_posY[patrol_blocks - 1 - i]);
            }
        }

    }

    public void enemy_move()
    {
        //경계 상태에 따라 달라진다.
        int imp;

        //기본적인 위치 가져오기
        position = transform.position;

        //경계가 0일 경우 다음 칸을 찾아가거나 경로 밖에 있으면 복귀한다.
        if (warning_level == 0)
        {
            imp = now_len_onPatrol();
            //경로 안에 있을 경우
            if(imp != -1)
            {
                //일반적인 이동의 경우
                if(imp != (patrol_len - 1))
                {
                    position.x += patrol_speed * (patrol_posX[imp + 1] - patrol_posX[imp]);
                    position.y += patrol_speed * (patrol_posY[imp + 1] - patrol_posY[imp]);
                }
                //만약 맨끝에 도달했을 경우 0으로 이동한다.
                else
                {
                    position.x += patrol_speed * (patrol_posX[0] - patrol_posX[patrol_len - 1]);
                    position.y += patrol_speed * (patrol_posY[0] - patrol_posY[patrol_len - 1]);
                }
            }
            //단순 벗어날 수도 있다.
            //또는 멀리 벗어났을 경우 복귀의 경우도 포함한다.
            
            else if(imp == -1)
            {
                //우선은 patrol_last를 이용해 귀환하는 형식을 사용한다.
                if(patrol_last != -1)
                {
                    if(patrol_last+1 != patrol_len)
                    {
                        position.x += patrol_speed * (patrol_posX[patrol_last + 1] - patrol_posX[patrol_last]);
                        position.y += patrol_speed * (patrol_posY[patrol_last + 1] - patrol_posY[patrol_last]);
                    }
                    else
                    {
                        position.x += patrol_speed * (patrol_posX[0] - patrol_posX[patrol_len-1]);
                        position.y += patrol_speed * (patrol_posY[0] - patrol_posY[patrol_len-1]);
                    }
                   
                }
               
            }
            
        }

        //최종 적용
        transform.position = position;

    }

    void detecting_sight()
    {

    }



}
