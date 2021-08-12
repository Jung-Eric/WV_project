using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf_control : MonoBehaviour
{
    // Start is called before the first frame update
    //기본 이동, 점프, 튕기기의 값을 가진다.
    public Vector3 position;
    //public Vector3 scales;
    public float base_speed = 0.1f;
    public float jump_power = 15f;
    public float jump_power_hor;

    //속도 측정 목적
    public float velocity_x;
    public float velocity_y;

    //public float jump_power_hor = 0.001f;

    public Vector2 flight_velocity_L = new Vector2(-0.02f,0f);
    public Vector2 flight_velocity_R = new Vector2(0.02f, 0f);

    //public float bounce_power = 1f;
    Vector2 bounceVelocity;

    Rigidbody2D rigid;
    Vector2 rigid_imp;
    //Vector3 pos;
    Transform pos;

    //현재 움직일 수 있는 상태를 지정_게임 내 조건
    public bool is_move = true;
    //게임 외 조건
    //0은 에딧, 1은 플레이, 2은 대화(연출)
    public int is_move_mode = 0;


    //바닥에 닿아있는 상태를 지정
    public bool is_landing = false;

    //현재 점프 상태를 지정
    bool is_jumping = false;

    //현재 채굴 상태를 지정
    //채굴 시에는 움직일 수 없고 캔슬 하거나 당해야된다.
    public bool is_digging = false; //현재 채굴 상태
    public bool digging_avail = true; //채굴 가능한 물건이 있는지 표시
    public int digging_type; //현재 채굴 타입 -1 하단,0 중단,1 상단
    public float is_digging_time = 0.45f;   //총 채굴 시간
    public float is_crashing_time = 0.35f;  //채굴 완료 시간

    public float is_digging_time_up = 0.55f;   //총 채굴 시간
    public float is_crashing_time_up = 0.33f;  //채굴 완료 시간

    //slope에 붙어있을 경우 추가 상태지정
    public bool is_slope_L = false;
    public bool is_slope_R = false;

    public float is_slope_up = 0.2f;
    public float is_slope_down = 0.5f;

    //가만히 있는지 확인한다.
    public bool static_state = true;
  

    //점프 횟수 지정
    //더블 점프 추가 예정
    int jumping_count = 1;


    //아랫점프 상태를 지정
    public bool is_downjump = false;

    //발판을 통과 가능한 상태
    public bool is_underthrouh = false;

    //현재 바라보는 방향
    //true이 좌측, false이 우측
    public bool way_change = false;
    Vector3 VectorR = new Vector3(1, 1, 1);
    Vector3 VectorL = new Vector3(-1, 1, 1);

 

    //발판 용 인수
    public bool is_step = false;
    public float limited_pos = -1.0f;

    //사다리 메달리기 인수
    public bool ladder_avail_left = false; //상태 지정
    public bool ladder_avail_right = false;
    public bool ladder_on_left = false; //메달리기
    public bool ladder_on_right = false;
    public float ladder_pos_left = 0.0f; //각 위치
    public float ladder_pos_right = 0.0f;

    public int ladder_move = 0; // -1 내려가기, 0정지, 1 올라가기


    //게임 조건 완수
    bool goal_clear = false;


    //애니메이터 받아오기
    public Animator anime_wolf;

   


    void Start()
    {
        is_landing = false;
        is_jumping = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        pos = gameObject.GetComponent<Transform>();
        way_change = false;
        is_underthrouh = false;
        //bounceVelocity = new Vector2(0, bounce_power);
        //position = transform.position;

        //방향 전환용 인수
        VectorR = new Vector3(1,1,1);
        VectorL = new Vector3(-1, 1, 1);

    }

    // Update is called once per frame
    void Update()
    {

        // 매 순간 속도의 상태 측정
        // 사다리는 제외한다.
        if(ladder_on_left == true || ladder_on_right == true)
        {
            is_underthrouh = true;
        }
        else
        {
            if (rigid.velocity.y > 0)
            {
                is_underthrouh = true;
            }
            else
            {
                is_underthrouh = false;
            }
        }

        //---------------------------------------------------------
        //다양한 애니메이션 상시 규칙
        //발이 붙어있나 마나에 따라 상태 지정
        if (is_landing == true)
        {
            anime_wolf.SetBool("is_landing", true);
        }
        else if(is_landing == false)
        {
            anime_wolf.SetBool("is_landing", false);
            anime_wolf.SetBool("is_walking", false);
        }
            
        


        //---------------------------------------------------------
        //다양한 입력 규정
        /*
        if (Input.GetKey(KeyCode.Space))
        {
            if (ladder_on_left == true || ladder_on_right == true)
            {
                set_normal();
                is_jumping = true;
            }
            else if (is_landing == true)
            {
                is_jumping = true;
            }
            else
            {
                //is_jumping = true;
            }
        }
    */
        //d키 채굴 입력
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (is_landing == true)
            {
                is_jumping = true;
            }
        }


        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (ladder_on_left == true || ladder_on_right == true)
            {

            }
            else
            {
                is_downjump = true;
            }
        }

        //올라가는걸 새롭게 지정
        //쭉 받는걸로 만든다.
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (ladder_avail_left == true || ladder_avail_right == true)
            {
                if (way_change == true && ladder_avail_left == true)
                {
                    set_ladder(true);
                }
                else if (way_change == false && ladder_avail_right == true)
                {
                    set_ladder(false);
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (ladder_on_right)
            {
                if (is_landing == true)
                {
                    set_normal();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (ladder_on_left)
            {
                if (is_landing == true)
                {
                    set_normal();
                }
            }
        }


        

        //---------------------------------------------------------


        //---------------------------------------------------------
        //발판용도 함수
        //위치에 있게 받쳐준다.
        /*
        if (is_landing == true && is_downjump == false && is_underthrouh == false)
        {
            if (pos.position.y < limited_pos)
            {
                Vector3 vec = new Vector3(pos.position.x, limited_pos, pos.position.z);
                pos.position = vec;
                rigid.velocity = Vector2.zero;
            }
        }
        */
        if (is_step == true)
        {
            if (pos.position.y < limited_pos)
            {
                Vector3 vec = new Vector3(pos.position.x, limited_pos, pos.position.z);
                pos.position = vec;
                rigid.velocity = Vector2.zero;
            }
        }

        //---------------------------------------------------------
        //사다리 용 함수
        if (ladder_on_left == true)
        {
            Vector3 vec = new Vector3(pos.position.x, pos.position.y, pos.position.z);
            pos.position = vec;
        }

        //---------------------------------------------------------
        /*
        {
            if (is_downjump == false)
            {
                if (is_underthrouh == false)
                {              
                    if (pos.position.y < limited_pos)
                    {
                        Vector3 vec = new Vector3(pos.position.x, limited_pos, pos.position.z);
                        pos.position = vec;
                        rigid.velocity = Vector2.zero;
                    }
                }
            }
        }
        */


        /*
        if (right_col.GetComponent<wolf_right_col>().col_right == 1)
        {
            rigid.velocity = Vector2.zero;
            bounceVelocity = new Vector2(0, bounce_power*(-1));
            rigid.AddForce(bounceVelocity, ForceMode2D.Impulse);

            right_col.GetComponent<wolf_right_col>().col_right = 0;
        }

        if (left_col.GetComponent<wolf_left_col>().col_left == 1)
        {
            rigid.velocity = Vector2.zero;
            bounceVelocity = new Vector2(0, bounce_power);
            rigid.AddForce(bounceVelocity, ForceMode2D.Impulse);

            left_col.GetComponent<wolf_left_col>().col_left = 0;
        }
        */


        //리스폰
        if (transform.position.y < -50)
        {
            Vector3 pos_new = new Vector3(40, 5, 0);
            transform.position = pos_new;
        }

        //속도 조절
        if (rigid.velocity.y < -30)
        {
            float inst = rigid.velocity.x;
            rigid.velocity = new Vector2(inst,-30);
        }

        //마찰력을 만들어준다.
        if(is_landing == true)
        {
            if(rigid.velocity.x != 0)
            {
                rigid.velocity = new Vector2(0,0);
            }
        }


        //속도 측정 목적
        velocity_x = rigid.velocity.x;
        velocity_y = rigid.velocity.y;

    }

    //---------------------------------------------------------
    //본격적인 수행
    private void FixedUpdate()
    {

        Horizontal_Move();
        //Horizontal_Move2(); //이건 아직 안씀
        Up_Jump();

        //flight_Move();
        Vertical_Move();



    }
    //---------------------------------------------------------

    //방향 전환도 여기서 포함한다.
    //내려갈 시 바닥에 닿게 만든다.
    private void Horizontal_Move()
    {
        if (ladder_on_left == false && ladder_on_right == false )
        {

            if (is_move == true && is_move_mode == 1)
            {

                position = transform.position;

                if (is_landing == true)
                {
                    position.x += base_speed * Input.GetAxisRaw("Horizontal");
                    if(Input.GetAxisRaw("Horizontal") != 0)
                    {
                        anime_wolf.SetBool("is_walking",true);
                        static_state = false;
                    }
                    else if(Input.GetAxisRaw("Horizontal") == 0)
                    {
                        anime_wolf.SetBool("is_walking", false);
                        static_state = true;
                    }

                }

                //우측 이동
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    //단순 방향 전환
                    if (way_change == true)
                    {
                        //이제는 transform 전환으로 해결
                        //gameObject.GetComponent<SpriteRenderer>().flipX = false;
                        transform.localScale = VectorR;
                        way_change = false;
                    }

                    //우하향 이동
                    //내려갈 때 별도 수치 적용
                    if (is_slope_L == true)
                    {
                        position.y -= base_speed * Input.GetAxisRaw("Horizontal") * (is_slope_down);
                    }
                    //우상향 이동
                    else if(is_slope_R == true)
                    {
                        position.y += base_speed * Input.GetAxisRaw("Horizontal") * (is_slope_up);
                    }
                    
                


                }
                //좌측 이동
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    //단순 방향 전환
                    if (way_change == false)
                    {
                        //이제는 transform 전환으로 해결
                        //gameObject.GetComponent<SpriteRenderer>().flipX = true;
                        transform.localScale = VectorL;
                        way_change = true;
                    }

                    //좌하향 이동
                    //내려갈 때 별도 수치 적용
                    if (is_slope_L == true)
                    {
                        position.y -= base_speed * Input.GetAxisRaw("Horizontal") * (is_slope_down) * (-1);
                    }
                    //우상향 이동
                    else if (is_slope_R == true)
                    {
                        position.y += base_speed * Input.GetAxisRaw("Horizontal") * (is_slope_up) * (-1);
                    }
                    
                }

                
                //떠있는 동안 약간의 힘 적용 가능
                
                if (is_landing == false)
                {
                    if (Input.GetAxisRaw("Horizontal") > 0 && is_slope_L == false)
                    {
                        rigid.AddForce(flight_velocity_R, ForceMode2D.Impulse);
                    }
                    
                    else if (Input.GetAxisRaw("Horizontal") < 0 && is_slope_R == false)
                    {
                        rigid.AddForce(flight_velocity_L, ForceMode2D.Impulse);
                    } 
                    
                }
                

                transform.position = position;
                //position.y += base_speed * Time.deltaTime * Input.GetAxisRaw("Vertical");


            }

            //rigid.velocity = Vector2.zero;
            /*
            if(rigid.velocity.y < 0)
            {
                rigid.velocity = Vector2.zero;
            }
            */
        }
        else
        {
            //일단은 거짓으로 책정...
            static_state = false;
        }
        


    }

    //좌우 이동 새로운 버전
    //이건 아직 안씀
    /*
    private void Horizontal_Move2()
    {
        

            if (ladder_on_left == true || ladder_on_right == true)
            {
                rigid_imp = rigid.velocity;

                rigid_imp.x = rigid_imp.x + (1.0f * Input.GetAxisRaw("Horizontal"));

                if (rigid_imp.x >= 10)
                {
                    rigid_imp.x = 10;
                }
                else if (rigid_imp.x <= -10)
                {
                    rigid_imp.x = -10;
                }

                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    if (way_change == true)
                    {
                        gameObject.GetComponent<SpriteRenderer>().flipX = false;
                        way_change = false;
                    }

                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    if (way_change == false)
                    {
                        gameObject.GetComponent<SpriteRenderer>().flipX = true;
                        way_change = true;
                    }
                }

                rigid.velocity = rigid_imp;
            }
        
    }
    */

    //기본 점프와 좌 우 점프
    private void Up_Jump()
    {
        if (!is_jumping)
        {
            return;
        }

        /*
        if (is_landing)
        {

            rigid.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jump_power);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        }
        */

        //방향 점프도 확인한다.
        float jump_hor = 0f;


        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            jump_hor = jump_power_hor;
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            //jump_hor = -3;
            //jump_hor = jump_power_hor;
            jump_hor = (-1)*(jump_power_hor);
        }
        else
        {
            jump_hor = 0;

        }

        rigid.velocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(jump_hor, jump_power);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

 
        is_jumping = false;

    }

    //공중에서 추가 힘받기



    //사다리 전용이다.
    private void Vertical_Move()
    {
        
        if (ladder_on_left == true || ladder_on_right == true)
        {
            //메달린 상태에서 우주로 가지 않게 함
            if(ladder_avail_left == true || ladder_avail_right == true)
            {

                position = transform.position;
                position.y += base_speed * Input.GetAxisRaw("Vertical");
                transform.position = position;

            }
            else
            {
                set_normal();
            }

        }
        
    }

    //채굴 애니메이션 출력 시
    private void Digging()
    {
        

    }



    //일반 상태로 되돌리기
    private void set_normal()
    {
        rigid.velocity = Vector2.zero;
        ladder_on_right = false;
        ladder_on_left = false;
        is_move = true;
        //is_jumping = true;
        rigid.gravityScale = 3;
    }

    //사다리 상태로 만들기
    //좌측 true 우측 false
    private void set_ladder(bool direction)
    {
        rigid.velocity = Vector2.zero;
        Vector3 vec = new Vector3(0, 0, 0);
        if (direction == false)
        {
            vec = new Vector3(ladder_pos_right, pos.position.y, pos.position.z);
            ladder_on_right = true;
            ladder_on_left = false;
        }
        else if(direction == true)
        {
            vec = new Vector3(ladder_pos_left, pos.position.y, pos.position.z);
            ladder_on_right = false;
            ladder_on_left = true;
        }
        pos.position = vec;
        is_move = false;
        rigid.gravityScale = 0;
        is_underthrouh = false;
    }

    //벽돌 부수기
    private void crash_block()
    {

    }
    /*
    private void Down_Jump()
    {
        StartCoroutine("JumpDown");
    }
    */

    /*
    IEnumerator JumpDown()
    {
        is_downjump = true;
        yield return new WaitForSeconds(0.2f);
        is_downjump = false;
        yield return null;
    }
    */
    

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ladder_left")
        {
            ladder_pos_left = col.gameObject.GetComponent<Transform>().position.x - 0.9f;
        }
        else if (col.gameObject.tag == "ladder_right")
        {
            ladder_pos_right = col.gameObject.GetComponent<Transform>().position.x + 0.9f;
        }

        if(col.gameObject.tag == "goal")
        {
            Vector3 pos_new = new Vector3(40,5,0);
            transform.position = pos_new;
        }

        if(col.gameObject.tag == "Missile")
        {
            //anime_wolf.SetTrigger("cancle_trigger");
            anime_wolf.SetTrigger("destroyed_trigg");

            Time.timeScale = 0.2f;

            anime_wolf.speed = 0.2f;

            is_move = false;

        }

    }


        //자체 충돌 지정
        private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "ladder_left")
        {
            ladder_avail_left = true;

        }
        else if (col.gameObject.tag == "ladder_right")
        {
            ladder_avail_right = true;

        }
    }

    //자체 충돌 지정
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "ladder_left")
        {
            ladder_avail_left = false;

        }
        else if (col.gameObject.tag == "ladder_right")
        {
            ladder_avail_right = false;

        }


    }

    //위치 초기화(Play button 클릭 시)
    //0은 에딧, 1은 플레이, 2은 대화(연출)
    public void mode_change()
    {
        //움직이지 못하게
        if(is_move_mode == 0)
        {
            is_move_mode = 1;
        }
        else
        {
            is_move_mode = 0;
        }
    }
    
    //정지 상태를 출력
    public bool ret_static()
    {
        if (static_state == true && velocity_y == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //---------------------------------------------------------------------------------------------
    //코루틴 생성

    //파괴 시 애니메이션과 동작
    IEnumerator co_diggigng()
    {
        yield return new WaitForSeconds(is_crashing_time);
        //일정 시간 후 파괴 처리
        //anime_wolf.SetInteger()
        //anime_wolf.SetBool("is_walking", true);


    }

}
