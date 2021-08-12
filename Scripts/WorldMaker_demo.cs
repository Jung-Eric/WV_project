using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

public class WorldMaker_demo : MonoBehaviour
{

    //생성 포인트----------------------------------------------------------
    public int start_x;
    public int start_y;

    //생성할 맵의 크기
    public int world_sizeX;
    public int world_sizeY;

    //cell의 배열 생성
    public cells[,] map_cell = new cells[512, 128];
 
    GameObject imp1;
    GameObject imp2;

    GameObject imp_child1;
    GameObject imp_child2;

    //이미지 변환용 sprite
    Sprite[] real_sprites;

    //임시 게임 오브젝트
    GameObject Object_normal_all;
    GameObject impObject;
    GameObject impObject_base;
    Transform impObject_base_child; // xy좌표와 0~3까지의 값으로 받아온다.

    //스프라이트 참조 인덱스
    //누적 형식으로 작성해서 참조가 편하도록 만든다.
    public int[] block_RealS_index;

    //프리팹 참조 인덱스
    //누적 형식으로 작성해서 참조가 편하도록 만든다.
    public int[] block_RealP_index;
    //실물 프리팹 리스트, 위 인덱스에 기반해 받아온다.
    GameObject[] blockP;
    public GameObject base_Blocks; //기본적인 블록, 이 블록 안에 프리팹들을 생성한다.

    //파일 위치 정리용 위치값
    Transform Parent_normal_all;

    //파일 쓰기용 인수
    StreamWriter sw;
    FileStream fileS;

    public string[] map_string;
    public int map_num;


    //맵 에디터 용도
    // editor type 0은 기본 생성 부분, 월드 생성과 복사 시에만 적용된다.
    // type 1은 배경+블록(blocks_back + blocks_real)
    // type 2은 배경만(blocks_back)
    // type 3은 블록보조만(blocks_sub) -> 추가한 1개만 
    // type 4은 특수블록만(blocks_special) -> 추가한 1개만

    public cells cell_imp = new cells(); //임시용 셀
    bool editor_on; //플레이 시에는 꺼져야 되니까...
    int editor_type; //위의 type 참고
    int touch_x;
    int touch_y;
    //int blocks_real;
    //int blocks_back;
    //int blocks_trans;
    public Camera Camera_editor;

    int pointerID;

    //버전 구분
    //0이면 editor
    //1이면 play
    public int play_version = 0;

    //캐릭터를 받아서 grid를 받아온다.
    public GameObject game_character;

    //grid 묶음을 받아온다.
    public GameObject grid_sel_all;
    public GameObject[] grid_selection;

    Transform grid_pos;
    Transform player_pos;
    //grid의 좌표
    float grid_x = 0;
    float grid_y = 0;

    float grid_x_old = -99;
    float grid_y_old = -99;

    public float grid_acc = -0.48f;

    //늑대 상태 인자 받기용
    wolf_control imp_wolf_control;

    public bool imp_control = true;
    //grid를 키고 끌 수 있게 만든다.
    public bool imp_grid_on = false;

    //삭제 목적 자식 접근
    public GameObject normals;

    //삭제 대상
    int dest_x;
    int dest_y;

    //시작 시 우선 맵의 이름들을 받아온다.
    /*
    private void Awake()
    {

        sw = new StreamWriter(map_string[map_num]);


    }
    */

    // Start is called before the first frame update
    void Start()
    {
        //입력 때문에 지정 필요
#if UNITY_EDITOR
        pointerID = -1; //PC나 유니티 상에서는 -1
#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        pointerID = 0;  // 휴대폰이나 이외에서 터치 상에서는 0 
#endif

        //최초 배열 초기화
        //System.Array.Clear(map_cell,0,map_cell.Length);


        //sprite 받아오기
        real_sprites = (Sprite[])Resources.LoadAll<Sprite>("Sprites/Blocks_Real");
        //object 받아오기
        blockP = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/Blocks_Real");

        //위치 정립
        Object_normal_all = GameObject.Find("normal_all");
        Parent_normal_all = Object_normal_all.GetComponent<Transform>();

        cell_imp.clear_all();
        //cell 파일 읽어온 다음 정보 적용
        for (int cell_y=0; cell_y<world_sizeY; cell_y++)
        {
            for (int cell_x = 0; cell_x < world_sizeX; cell_x++)
            {
                //읽어온 정보를 cell에 넣는다. info, xy 좌표
                //테스트용129 투입
                //직접 작업하는게 오류가 생기니 복사해서 붙이는 것으로 대체
                cells cell_imp = new cells();

                //16384+128 16512     0/1/1
                //32768+128 32896     0/1/2
                //cell_imp.set_cell(16384*0 + 128*1, cell_x + start_x, cell_y + start_y);
                cell_imp.set_cell(0, cell_x + start_x, cell_y + start_y);
                map_cell[cell_x, cell_y] = cell_imp;
                //map_cell[cell_x, cell_y].set_cell(129, cell_x + start_x, cell_y + start_y);

            }
        }

        //cell 정보로 world 만들기-----------------------------------------------------------------
        //또한 월드 좌표에 따라 sprite 이미지를 바꿔준다.
        for (int cell_y = 0; cell_y < world_sizeY; cell_y++)
        {
            for (int cell_x = 0; cell_x < world_sizeX; cell_x++)
            {

                //읽어온 정보를 cell에 넣는다.
                cell_creation(map_cell[cell_x, cell_y],0);



            }
        }
        //--------------------------------------------------------------------------------------------
        //일단 자동저장
        //맵 쓰고 닫기
        sw = new StreamWriter("Assets/MapNote/" + map_string[map_num]);
        map_write();
        sw.Flush();
        sw.Close();

        //cell_imp 초기화
        cell_imp.clear_all();

        //grid 좌표를 받아오기
        //추가로 유저 좌표 받아오기
        grid_pos = grid_sel_all.GetComponent<Transform>();
        player_pos = game_character.GetComponent<Transform>();

        //외부 스크립트 인자 받기
        imp_wolf_control = game_character.GetComponent<wolf_control>();


    }

    // Update is called once per frame
    //여기서 터치를 받아서 prefab을 생성한다.
    void Update()
    {
        //터치를 받아서 진행한다.
        if (Input.GetMouseButtonDown(0) && (EventSystem.current.IsPointerOverGameObject(pointerID) == false))
        {
            Vector2 M_point;
            M_point = Input.mousePosition;

            //카메라 위치 받기
            float camera_z = Camera_editor.transform.position.z * (-1);

            Vector2 wp = Camera_editor.ScreenToWorldPoint(new Vector3(M_point.x, M_point.y, camera_z));
            //Vector3 wp = Camera_editor.main.ScreenToWorldPoint(Input.mousePosition);

           //카메라 기반 좌표 받기
            int imp_x = Mathf.RoundToInt(wp.x);
            int imp_y = Mathf.RoundToInt(wp.y);


            
            if((imp_x>=start_x) && (imp_x<start_x+world_sizeX) && (imp_y >= start_y) && (imp_y < start_y + world_sizeY))
            {
                //생성 용 함수
                if (play_version == 0)
                {
                    //이게 옛날 함수이고 새로운 함수로 대체
                    //Instantiate(blockP[7], new Vector3(imp_x, imp_y, 0), Quaternion.identity);
                    cell_imp.set_cell_point(imp_x, imp_y);
                    //cell_imp.set_cell_block(0,1,0);

                    //만약 여기에 오브젝트가 이미 있다면 수행하지 않는다.
                    //해당 좌표에 이미 뭔가가 있다면 수행하지 않는다.
                    int imp_x2 = imp_x - start_x;
                    int imp_y2 = imp_y - start_y;

                    if (map_cell[imp_x2, imp_y2].get_real() == 0)
                    {
                        cell_creation(cell_imp, 1);
                    }
                }
                //파괴 용 함수
                else if (play_version == 1)
                {
                    int imp_x2 = imp_x - start_x;
                    int imp_y2 = imp_y - start_y;

                    game_character.GetComponent<Animator>().SetTrigger("dig_trigg");
                    //map_cell[imp_x2, imp_y2]
                    //GameObject

                    dest_x = imp_x2;
                    dest_y = imp_y2;

                    Invoke("block_destory", 0.6f);

                    //Destroy(normals.transform.GetChild(imp_x2 + imp_y2 * world_sizeX).transform.GetChild(1).transform.GetChild(0));

                    //cell_reset();

                }


            }
        
        }

        //현재 위치에 따라서 AvailGrid를 이동시킨다.
        //grid_sel_x = Mathf.RoundToInt(grid_x);
        //grid_sel_y = Mathf.RoundToInt(grid_y);
        grid_x = Mathf.Round(player_pos.position.x);
        //grid_y = Mathf.Round(player_pos.position.y-0.48f+1);
        //grid_y = Mathf.Round(player_pos.position.y + 1);
        grid_y = Mathf.Round(player_pos.position.y + grid_acc);

        grid_pos.position = new Vector3(grid_x,grid_y);

        //추가적으로 map_cell을 받아와서 적용이 가능한 것들만 표시한다.
        //갱신되는 타이밍을 체크한다.
        //imp_wolf_control.ret_static()

        imp_control = imp_wolf_control.ret_static();
        
        if (imp_control && imp_grid_on == false)
        {
            for (int i = 0; i < 9; i++)
            {
                float imp_x = 0;
                float imp_y = 0;

                switch (i)
                {
                    case 0:
                        imp_x = grid_x - 1;
                        imp_y = grid_y + 1;
                        break;

                    case 1:
                        imp_x = grid_x;
                        imp_y = grid_y + 1;
                        break;

                    case 2:
                        imp_x = grid_x + 1;
                        imp_y = grid_y + 1;
                        break;

                    case 3:
                        imp_x = grid_x + 1;
                        imp_y = grid_y;
                        break;

                    case 4:
                        imp_x = grid_x + 1;
                        imp_y = grid_y - 1;
                        break;

                    case 5:
                        imp_x = grid_x;
                        imp_y = grid_y - 1;
                        break;

                    case 6:
                        imp_x = grid_x - 1;
                        imp_y = grid_y - 1;
                        break;

                    case 7:
                        imp_x = grid_x - 1;
                        imp_y = grid_y;
                        break;

                    case 8:
                        imp_x = grid_x;
                        imp_y = grid_y;
                        break;

                }
                //상황 변경
                grid_selection[i].GetComponent<selection_avail>().setAvail(in_cells(imp_x, imp_y));
            }
            imp_grid_on = true;
        }
        else if(imp_control == false && imp_grid_on == true)
        {
            for (int i = 0; i < 9; i++)
            {
                grid_selection[i].GetComponent<selection_avail>().setAvail(false);
            }
            imp_grid_on = false;
        }

        grid_x_old = grid_x;
        grid_y_old = grid_y;

        //if ((grid_x_old != grid_x) || (grid_y_old != grid_y))
    }



    //중간에 스프라이트가 누적되어도 문제 없이 인덱스를 불어온다.
    int call_indexS(int num)
    {
        int imp = 0;

        for(int i=0; i<num; i++)
        {
            imp = imp + block_RealS_index[i];
        }

        return imp;
    }
    //중간에 프리팹이 누적되어도 문제 없이 인덱스를 불어온다.
    int call_indexP(int num)
    {
        int imp = 0;

        for (int i = 0; i < num; i++)
        {
            imp = imp + block_RealP_index[i];
        }

        return imp;
    }


    //월드 생성
    //cell정보를 기반으로 블록을 생성한다.----------------------------------------------------
    // type 0은 기본 생성 부분, 월드 생성과 복사 시에만 적용된다.
    // type 1은 배경+블록(blocks_back + blocks_real)
    // type 2은 배경만(blocks_back)
    // type 3은 블록보조만(blocks_sub) -> 추가한 1개만 
    // type 4은 특수블록만(blocks_special) -> 추가한 1개만
    void cell_creation(cells imp,int type)
    {
        //좌표 저장용
        int int_x;
        int int_y;
        int_x = imp.get_x();
        int_y = imp.get_y();

        //임시 사용 목적
        int int_imp1 = 0;
        int int_imp2 = 0;
        int int_inst = 0;

        //스프라이트에 활용
        int sprite_num1 = 0;
        int sprite_num2 = 0;

        //좌표를 정리 인수(음수는 양수로)
        int sprite_x = 0;
        int sprite_y = 0;

        //우선 최초 생성일 경우 base_blocks를 생성한다.---------
        if(type == 0)
        {
            impObject_base = Instantiate(base_Blocks, new Vector3(int_x, int_y, 0), Quaternion.identity);
            impObject_base.transform.parent = Parent_normal_all;
        }
        //------------------------------------------------------


        //배경 정보 받기
        //이건 아직 작업을 하지 않았다.
        int_imp1 = imp.get_back();


        //실제 블록 정보 받기---------------------------------------
        int_imp1 = imp.get_real();


        //실물에 따라 타입도 달라진다.
        //type 조건을 단다.
        if (int_imp1 != 0 && (type==0 || type ==1))
        {

            //블록 변형 타입
            int_imp2 = imp.get_transType();

            //조건문에 따라 실제 블록을 생성한다.
            //인덱스에 따라 블록의 종류를 생성하고 좌표도 받는다.
            int_inst = call_indexP(int_imp1)+int_imp2;


            map_cell[int_x - start_x, int_y - start_y].set_cell_block(0,int_imp1, int_imp2);
            //Debug.Log("x좌표 : " + (int_x - start_x) + " / y좌표 : " + (int_y - start_y));
            //Debug.Log("real값 : " + int_imp1 + " / trans값 : " + int_imp2);
            //Debug.Log("real값 : " + map_cell[int_x - start_x, int_y - start_y].get_real() + " / trans값 : " + map_cell[int_x - start_x, int_y - start_y].get_transType());
            impObject = Instantiate(blockP[int_inst], new Vector3(int_x, int_y, 0), Quaternion.identity);

            //파일 배치는 미리 바꿔놓는다.
            //최초 생성의 경우
            if(type==0)
            {
                impObject.transform.parent = impObject_base.transform.GetChild(1);
            }
            //부가 생성의 경우
            else if (type == 1)
            {
                call_created_cell(int_x-start_x, int_y-start_y, 1);
                impObject.transform.parent = impObject_base_child;
            }



            //prefab에 따라 이 오브젝트의 이미지를 바꿔준다.
            //이는 토양 같은 지형물에 주로 적용된다.
            //int_x int_y같은 좌표값에만 의존한다.


            //토양, 자갈의 경우
            if (int_imp1 == 1 || int_imp1 == 2)
            {
                int indexS = call_indexS(int_imp1);

                //mod를 통해 위치 정립, 음수는 양수로
                sprite_x = (int_x % 11);
                sprite_y = (int_y % 5);
                if(sprite_x < 0)
                {
                    sprite_x = sprite_x + 11;
                }
                if(sprite_y < 0)
                {
                    sprite_y = sprite_y + 5;
                }


                //변형 타입에 따른 스프라이트 변경
                //기본 형태
                if(int_imp2 == 0)
                {
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    imp_child2 = impObject.transform.GetChild(1).gameObject;
                    
                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22);
                    sprite_num2 = sprite_x + ((4 - sprite_y) * 22)+11;

                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1+ indexS];
                    imp_child2.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num2+ indexS];
                    
                }
                //낮은 왼날개
                else if(int_imp2 == 1)
                {
                    
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 110 + 11;
                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1 + indexS];
                    

                }
                //높은 왼날개
                else if (int_imp2 == 2)
                {
                    
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    imp_child2 = impObject.transform.GetChild(1).gameObject;

                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 110;
                    sprite_num2 = sprite_x + ((4 - sprite_y) * 22) + 11;

                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1+ indexS];
                    imp_child2.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num2+ indexS];
                    
                }
                //높은 오른날개
                else if (int_imp2 == 3)
                {
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    imp_child2 = impObject.transform.GetChild(1).gameObject;

                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 220;
                    sprite_num2 = sprite_x + ((4 - sprite_y) * 22) + 11;

                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1 + indexS];
                    imp_child2.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num2 + indexS];


                }
                //낮은 오른날개
                else if (int_imp2 == 4)
                {
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 220 + 11;
                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1 + indexS];

                }
            }

        }
        //블록이 없는 특수 블록의 경우
        else
        {
            int_imp1 = imp.get_special_count();
            
            //개수 만큼 스페셜을 생성한다.
            for(int i=0; i<int_imp1; i++)
            {
                //스페셜은 고유하니까 문제가 되지 않는다.
                int_imp2 = imp.get_special(i);

                //Instantiate...

            }


        }

    } 
    //version 전환
    public void version_change()
    {
        if (play_version == 0)
        {
            play_version = 1;
        }
        else if(play_version == 1)
        {
            play_version = 0;
        }
    }

    //블록 파괴
    public void block_destory()
    {
        int x3 = dest_x;
        int y3 = dest_y;

        Destroy(normals.transform.GetChild(x3 + y3 * world_sizeX).transform.GetChild(1).transform.GetChild(0).gameObject);

        map_cell[x3, y3].clear_all();

    }

    //-------------------------------------------------------------------------------------
    //해당 좌표의 type(0~3)에 따라 child를 불러온다
    void call_created_cell(int x, int y, int type)
    {
        //GameObject impObject_created;
        impObject_base_child = Object_normal_all.transform.GetChild(y * world_sizeX + x).gameObject.transform.GetChild(type);
        //impObject_base_child
    }

    
    //맵 읽어와서 적용하기
    void map_creation()
    {
        string map_path = "MapNote/이름";

        if(false == File.Exists(map_path))
        {
            var file = File.CreateText(map_path+".txt");
            file.Close();
        };
         
    }

    //지정 맵에 현재 맵 쓰기
    void map_write()
    {
        int WorldSize = ((world_sizeY << 9) + (world_sizeX));

        sw.WriteLine(WorldSize);

        for(int imp_y=0; imp_y<world_sizeY; imp_y++)
        {
            for(int imp_x=0; imp_x<world_sizeX; imp_x++)
            {
                sw.WriteLine(map_cell[imp_x,imp_y].get_cell());
            }
        }

    }

    //위치 안에 있는지 확인
    public bool in_cells(float val_x, float val_y)
    {
        //int imp_cell_x = (int)val_x;
        //int imp_cell_y = (int)val_y;

        if (( val_x >= start_x && val_x < start_x + world_sizeX) && (val_y >= start_y && val_y < start_y + world_sizeY))
        {
            //Debug.Log("내부에 있음 탐색");
            int imp_cell_x = 0;
            int imp_cell_y = 0;
            imp_cell_x = (int)(val_x- start_x);
            imp_cell_y = (int)(val_y- start_y);

            if (map_cell[imp_cell_x,imp_cell_y].get_real() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    //현재 위치와 방향에 따라오브젝트 활성화
    
    //게임 블록 정보
    public class cells
    {
        
        //좌표값
        private int cells_x = 0, cells_y = 0;

        //배경
        private int blocks_back = 0;

        //기본블록
        private int blocks_real = 0;
        //기본블록 변형 타입(기본블록이 0이 아니어야된다)
        private int real_trans_type = 0;

        //블록보조 개수
        //기본적으러 block_real이 0이 아니어야 된다.
        //수풀, 화석과 같은 경우
        private List<int> blocks_sub = new List<int>();


        //특수블록 (추가 갱신시 꼭 clear 하도록 하자)
        //기본적으로 block_real이 0이어야된다. (비어야된다)
        //상자와 같은 경우
        private List<int> blocks_special = new List<int>();

        //생성자
        public cells()
        {
            cells_x = 0;
            cells_y = 0;
            blocks_back = 0;
            blocks_real = 0;
            real_trans_type = 0;

            blocks_sub.Clear();
            blocks_special.Clear();

        }

        //지우는 용도로도 사용된다
        public void clear_all()
        {
            blocks_back = 0;
            blocks_real = 0;
            real_trans_type = 0;

            blocks_sub.Clear();
            blocks_special.Clear();
        }

        //맵 제작하기
        //개별 셀의 parsing 전 정보를 가져온다.
        public int get_cell()
        {
            int cell_int = 0;

            int cell_index = 0;

            //배경 추가
            cell_int = cell_int + blocks_back;
            cell_index = cell_index + 7;

            //블록 추가
            cell_int = cell_int + (blocks_real<<cell_index);
            cell_index = cell_index + 7;

            //실물에 따라 조건부
            if (blocks_real != 0)
            {
                cell_int = cell_int + (real_trans_type << cell_index);
                cell_index = cell_index + 3;

                int imp_sub = blocks_sub.Count;
                cell_int = cell_int + (imp_sub << cell_index);
                cell_index = cell_index + 2;

                for (int i=0; i<imp_sub; i++)
                {
                    cell_int = cell_int + (blocks_sub[i] << cell_index);
                    cell_index = cell_index + 4;
                }

            }
            else
            {

                int imp_spc = blocks_special.Count;
                cell_int = cell_int + (imp_spc << cell_index);
                cell_index = cell_index + 3;

                for (int i = 0; i < imp_spc; i++)
                {
                    cell_int = cell_int + (blocks_special[i] << cell_index);
                    cell_index = cell_index + 7;
                }

            }

            return cell_int;
        }

        //cell의 위치와 info값을 통해 cell정보 정립
        public void set_cell(int info, int x, int y)
        {
            cells_x = x;
            cells_y = y;

            int parsing_int = info;
            int parsed_int = 0;
            int parsing_index = 0;

            int sub_count = 0;
            int special_count = 0;

            //배경 가져오기
            parsed_int = int_parser(parsing_int, parsing_index, 7);
            parsing_index = parsing_index + 7;
            blocks_back = parsed_int;

            //실물 가져오기
            parsed_int = int_parser(parsing_int, parsing_index, 7);
            parsing_index = parsing_index + 7;
            blocks_real = parsed_int;

            //실물이 0인가에 대해서 달라진다.
            //0이 아니면 일반형, 0이면 오브젝트형이다.

            //일반형의 경우
            if(blocks_real != 0)
            {
                //실물 변형 형태
                parsed_int = int_parser(parsing_int, parsing_index, 3);
                parsing_index = parsing_index + 3;
                real_trans_type = parsed_int;

                parsed_int = int_parser(parsing_int, parsing_index, 2);
                parsing_index = parsing_index + 2;
                sub_count = parsed_int;

                for(int i=0; i<sub_count; i++)
                {
                    parsed_int = int_parser(parsing_int, parsing_index, 4);
                    parsing_index = parsing_index + 4;
                    blocks_sub.Add(parsed_int);
                }

            }
            //특수형의 경우
            else
            {
                parsed_int = int_parser(parsing_int, parsing_index, 3);
                parsing_index = parsing_index + 3;
                special_count = parsed_int;

                for (int i = 0; i < special_count; i++)
                {
                    parsed_int = int_parser(parsing_int, parsing_index, 7);
                    parsing_index = parsing_index + 7;
                    blocks_sub.Add(parsed_int);
                }

            }

        }

        //블록 변경의 경우
        //직접 인자를 넣어서 만들기
        //블록에 대한 정보
        public void delete_real()
        {

        }

        public void set_cell_point(int int_x, int int_y)
        {
            cells_x = int_x;
            cells_y = int_y;
        }
        public void set_cell_block(int back, int real, int type)
        {
            blocks_back = back;

            //다른 블록도 이렇게 만들어야 된다.
           
            blocks_real = real;

            real_trans_type = type;
        }

        //이건 나중에...(special에 해당)
        public void set_cell_object()
        {

        }


        //숫자 파싱 함수
        //par의 숫자를 start_pos(0이 시작)위치에서 length만큼 읽어온다.

        int int_parser(int par, int start_pos, int length)
        {
            int bit_maker = 0;
            int imp_bit = 1;

            int tester = par;

            tester = tester >> start_pos;

            for (int i = 0; i < length; i++)
            {
                imp_bit = imp_bit * 2;
            }
            imp_bit = imp_bit - 1;

            bit_maker = imp_bit & tester;

            return bit_maker;
        }
        

        public int get_x()
        {
            return cells_x;
        }

        public int get_y()
        {
            return cells_y;
        }

        public int get_back()
        {
            return blocks_back;
        }

        public int get_real()
        {
            return blocks_real;
        }

        public int get_transType()
        {
            return real_trans_type;
        }

        public int get_sub_count()
        {
            return blocks_sub.Count;
        }

        public int get_special_count()
        {
            return blocks_special.Count;
        }

        public int get_sub(int index)
        {
            return blocks_sub[index];
        }

        public int get_special(int index)
        {
            return blocks_special[index];
        }



    }
}
