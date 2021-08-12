using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class WorldMaker : MonoBehaviour
{
    //읽어온 정보
    private string world_string_before;
    private string world_string_after;

    //읽기 관련 정보
    public int string_reader_index = 0;
    private string string_imp;
    private int int_imp;

    //참조 인덱싱
    List<int> num_index = new List<int>();
    int num_index_len = 0;

    //------(실제 수행)----------------------------------
    //가로와 세로 길이
    public int hor_val = 0;
    public int ver_val = 0;

    //월드 맵 종류
    public int world_map = 0;

    //게임 셀들 2차원 배열로 관리 (list로 관리하면 Add시 꼬일 수 있음)
    //행 별로 관리한다.0행 0열, 0행 1열...
    //hor 512, ver 128 한계를 만들고 나머지는 지속적으로 삭제한다.
    public cells[,] map_cell = new cells[512,128];

    //cells 파생 길찾기용 int, 통과가능여부만 확인
    //N, S, X의 접근 가능한지 확인한다.
    public int[,] path_info = new int[512, 128];

    //길을 탐색 가능한지의 bool값, 막힌 길은 모두 false / true인 좌표만 대상으로 연산한다.
    public bool[,] path_avail = new bool[512, 128];

    //길추천 시스템, 낮은 값들 위주로 우선 탐색 (2까지만 대상으로 한다)
    public int[,] path_val = new int[512, 128];



    //각 블록 별 인덱스, 일치여부와 
    public bool block_same = false;
    public int block_add = 0;

    //----------------------------------------------------


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //파일 입출력으로 string 읽어오기
    void read_map()
    {

    }


    //string 기반으로 decoding
    void get_map()
    {
        string_reader_index = 0;
        num_index_len = 0;
        num_index.Clear();

        //맵 크기 받아오기 X
        string_imp = world_string_before.Substring(string_reader_index, 3);
        string_reader_index += 3;
        hor_val = Convert.ToInt32(string_imp, 16);
        //맵 크기 받아오기 Y
        string_imp = world_string_before.Substring(string_reader_index, 3);
        string_reader_index += 3;
        ver_val = Convert.ToInt32(string_imp, 16);

        //길이만큼 수행
        //인덱스 만들기---------------------------------------------------
        //while (string_reader_index < world_string_before.Length)
        while (true)
        {
            //2칸 씩 읽어나간다.
            string_imp = world_string_before.Substring(string_reader_index, 2);
            string_reader_index += 2;

            //16진수 문자열을 int로
            //조기 종료 255등장 시 break
            int_imp = Convert.ToInt32(string_imp, 16);
            if (int_imp != 255)
            {
                num_index.Add(int_imp);
                //num_index_len++;
            }
            else if(int_imp == 255)
            {
                break;
            }
            
            //15개의 인덱스가 다 채워졌다면 break
            if(num_index.Count == 15)
            {
                break;
            }

        }


        //개수 측정
        num_index_len = num_index.Count;

        //본격적으로 읽어오기 수행, 가로, 세로별로 수행한다.
        for(int i = 0; i<ver_val; i++)
        {
            for(int j = 0; j<hor_val; j++)
            {
                //블록별 인덱스 받아오기------------------------------------------------
                //블록, 배경 일치여부를 측정한다.
                string_imp = world_string_before.Substring(string_reader_index, 1);
                string_reader_index += 1;
                int_imp = Convert.ToInt32(string_imp, 16);

                //초기화
                block_same = true;
                block_add = 0;

                if (int_imp < 8)
                {
                    block_same = false;
                    block_add = int_imp;
                }
                else if(int_imp >= 8)
                {
                    block_same = true;
                    block_add = int_imp - 8;
                }


                //일반 블록 읽기-----------------------------------------------
                string_imp = world_string_before.Substring(string_reader_index, 1);
                string_reader_index += 1;
                int_imp = Convert.ToInt32(string_imp, 16);
                //기본 인덱스 15 이외의 값이라면 인덱싱, 15라면 추가작업 수행
                if (int_imp != 15)
                {
                    map_cell[j, i].set_blockt1(num_index[int_imp]);
                }
                else
                {
                    string_imp = world_string_before.Substring(string_reader_index, 2);
                    string_reader_index += 2;
                    int_imp = Convert.ToInt32(string_imp, 16);
                    map_cell[j, i].set_blockt1(int_imp);
                }


                //배경 읽기-----------------------------------------------
                if(block_same == true)
                {
                    //블록 값을 가져와 넣는다.
                    map_cell[j, i].set_bg( map_cell[i, j].get_blockt1() );
                }
                else if (block_same == false)
                {

                    string_imp = world_string_before.Substring(string_reader_index, 1);
                    string_reader_index += 1;
                    int_imp = Convert.ToInt32(string_imp, 16);

                    if (int_imp != 15)
                    {
                        map_cell[j, i].set_bg(num_index[int_imp]);
                    }
                    else
                    {
                        string_imp = world_string_before.Substring(string_reader_index, 2);
                        string_reader_index += 2;
                        int_imp = Convert.ToInt32(string_imp, 16);
                        map_cell[j, i].set_bg(int_imp);
                    }

                }

                //특수 블록 읽기
                string_imp = world_string_before.Substring(string_reader_index, 1);
                for (int k = 0; k<block_add; k++)
                {
                    string_imp = world_string_before.Substring(string_reader_index, 1);
                    string_reader_index += 1;
                    int_imp = Convert.ToInt32(string_imp, 16);

                    if (int_imp != 15)
                    {
                        map_cell[j, i].set_add_blockt2(num_index[int_imp]);
                    }
                    else
                    {
                        string_imp = world_string_before.Substring(string_reader_index, 2);
                        string_reader_index += 2;
                        int_imp = Convert.ToInt32(string_imp, 16);
                        map_cell[j, i].set_add_blockt2(int_imp);
                    }

                }


            }
        }


          

    }

    //prefab을 생성한다.
    void draw_map()
    {



    }

    //string 기반으로 encoding
    // world_string_after을 생성하고 적는 방식을 이용한다.
    // 새롭게 변경이 이뤄진 맵을 수정하는데 사용된다.
    void write_map()
    {

    }

    //길찾기 맵 계산
    //N S X으로 구분한다.
    //여기서 avail도 연산해둔다.초기화도 진행
    void path_info_cal()
    {
        int imp;
        for (int i = 0; i < hor_val; i++)
        {
            for (int j = 0; j < ver_val; j++)
            {
                path_avail[i, j] = true;
                path_val[i, j] = -128; //그냥 일반적인 음수 값(-1,-2는 문제 발생 가능)
                imp = map_cell[i, j].cell_info();
                if(imp != 0)
                {
                    path_avail[i, j] = false;
                }
            }
        }

    }


    //길찾기 탐색용 계산
    //avail과 path를 계산한다.
    //4가지 방향에 대해서 탐색한다.
    void path_val_cal(int x, int y)
    {
        List<int> path = new List<int>();
        while (true)
        {



            if (true)
            {
                break;
            }
        }

    }

    

    //x,y좌표 전달 시 방향(0~7) 전달
    //같은 숫자이면 random하게 결정해줌.
    public int path_idicator(int x, int y)
    {
        int now_val = path_val[x, y]; //현재 값

        List<int> path_1 = new List<int>();
        List<int> path_2 = new List<int>();

        if(x>0 && y < ver_val - 1)
        {
            if (path_val[x - 1, y + 1] == now_val - 2)
            {
                path_2.Add(0);
            }
        }

        if(y < ver_val - 1)
        {
            if (path_val[x, y + 1] == now_val - 1)
            {
                path_1.Add(1);
            }
        }

        if (x < hor_val-1 && y < ver_val - 1)
        {
            if (path_val[x + 1, y + 1] == now_val - 2)
            {
                path_2.Add(2);
            }
        }

        if (x < hor_val - 1)
        {
            if (path_val[x+1, y] == now_val - 1)
            {
                path_1.Add(3);
            }
        }

        if (x < hor_val - 1 && y > 0)
        {
            if (path_val[x + 1, y - 1] == now_val - 2)
            {
                path_2.Add(4);
            }
        }

        if (y > 0)
        {
            if (path_val[x, y-1] == now_val - 1)
            {
                path_1.Add(5);
            }
        }


        if (x > 0 && y > 0)
        {
            if(path_val[x-1,y-1] == now_val-2)
            {
                path_2.Add(6);
            }
        }

        if (x > 0)
        {
            if (path_val[x-1, y] == now_val - 1)
            {
                path_1.Add(7);
            }
        }

        int path1_C = path_1.Count;
        int path2_C = path_2.Count;

        int rand;

        //본격적으로 값 추천
        if(path2_C > 0)
        {
            if(path2_C == 1)
            {
                return path_2[0];
            }
            else
            {
                rand = UnityEngine.Random.Range(0, path2_C);
                return path_2[rand];
            }
        }
        else if(path1_C > 0)
        {
            if (path1_C == 1)
            {
                return path_1[0];
            }
            else
            {
                rand = UnityEngine.Random.Range(0, path1_C);
                return path_1[rand];
            }

        }

        return -1;
    }
 
}

//
public class cells
{
    //배경
    private int background;
    //기본블록
    private int block_t1;
    //특수블록 (추가 갱신시 꼭 clear 하도록 하자)
    private List<int> block_t2 = new List<int>();


    public int get_bg()
    {
        return background;
    }

    public int get_blockt1()
    {
        return block_t1;
    }

    public int get_blockt2_size()
    {
        return block_t2.Count;
    }

    public int get_blockt2(int i)
    {
        return block_t2[i];
    }

    public void set_bg(int i)
    {
        background = i;
    }

    public void set_blockt1(int i)
    {
        block_t1 = i;
    }

    public void set_add_blockt2(int i)
    {
        block_t2.Add(i);
    }

    public void clear_all()
    {
        background = 0;
        block_t1 = 0;
        block_t2.Clear();
    }

    public int cell_info()
    {
        int return_int = 0;
        int imp = 0;    

        if(block_t1 == 0)
        {
            return_int += 1;
        }
        else if(block_t1 == 128)
        {
            return_int += 3;
            imp += (1 << 0); //상 접근가능
            //imp += (1 << 1); //우
            //imp += (1 << 2); //하
            imp += (1 << 3); //좌

            return_int += (imp << 2);
        }
        else if (block_t1 == 129)
        {
            return_int += 3;
            imp += (1 << 0); //상 접근가능
            imp += (1 << 1); //우
            //imp += (1 << 2); //하
            //imp += (1 << 3); //좌

            return_int += (imp << 2);
        }
        else
        {
            //이외에는 다 막힌걸로 처리
            return_int = 0;
        }
        return return_int;
    }

}
