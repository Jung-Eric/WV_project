using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

public class WorldMaker_demo : MonoBehaviour
{

    //���� ����Ʈ----------------------------------------------------------
    public int start_x;
    public int start_y;

    //������ ���� ũ��
    public int world_sizeX;
    public int world_sizeY;

    //cell�� �迭 ����
    public cells[,] map_cell = new cells[512, 128];
 
    GameObject imp1;
    GameObject imp2;

    GameObject imp_child1;
    GameObject imp_child2;

    //�̹��� ��ȯ�� sprite
    Sprite[] real_sprites;

    //�ӽ� ���� ������Ʈ
    GameObject Object_normal_all;
    GameObject impObject;
    GameObject impObject_base;
    Transform impObject_base_child; // xy��ǥ�� 0~3������ ������ �޾ƿ´�.

    //��������Ʈ ���� �ε���
    //���� �������� �ۼ��ؼ� ������ ���ϵ��� �����.
    public int[] block_RealS_index;

    //������ ���� �ε���
    //���� �������� �ۼ��ؼ� ������ ���ϵ��� �����.
    public int[] block_RealP_index;
    //�ǹ� ������ ����Ʈ, �� �ε����� ����� �޾ƿ´�.
    GameObject[] blockP;
    public GameObject base_Blocks; //�⺻���� ���, �� ��� �ȿ� �����յ��� �����Ѵ�.

    //���� ��ġ ������ ��ġ��
    Transform Parent_normal_all;

    //���� ����� �μ�
    StreamWriter sw;
    FileStream fileS;

    public string[] map_string;
    public int map_num;


    //�� ������ �뵵
    // editor type 0�� �⺻ ���� �κ�, ���� ������ ���� �ÿ��� ����ȴ�.
    // type 1�� ���+���(blocks_back + blocks_real)
    // type 2�� ��游(blocks_back)
    // type 3�� ��Ϻ�����(blocks_sub) -> �߰��� 1���� 
    // type 4�� Ư����ϸ�(blocks_special) -> �߰��� 1����

    public cells cell_imp = new cells(); //�ӽÿ� ��
    bool editor_on; //�÷��� �ÿ��� ������ �Ǵϱ�...
    int editor_type; //���� type ����
    int touch_x;
    int touch_y;
    //int blocks_real;
    //int blocks_back;
    //int blocks_trans;
    public Camera Camera_editor;

    int pointerID;

    //���� ����
    //0�̸� editor
    //1�̸� play
    public int play_version = 0;

    //ĳ���͸� �޾Ƽ� grid�� �޾ƿ´�.
    public GameObject game_character;

    //grid ������ �޾ƿ´�.
    public GameObject grid_sel_all;
    public GameObject[] grid_selection;

    Transform grid_pos;
    Transform player_pos;
    //grid�� ��ǥ
    float grid_x = 0;
    float grid_y = 0;

    float grid_x_old = -99;
    float grid_y_old = -99;

    public float grid_acc = -0.48f;

    //���� ���� ���� �ޱ��
    wolf_control imp_wolf_control;

    public bool imp_control = true;
    //grid�� Ű�� �� �� �ְ� �����.
    public bool imp_grid_on = false;

    //���� ���� �ڽ� ����
    public GameObject normals;

    //���� ���
    int dest_x;
    int dest_y;

    //���� �� �켱 ���� �̸����� �޾ƿ´�.
    /*
    private void Awake()
    {

        sw = new StreamWriter(map_string[map_num]);


    }
    */

    // Start is called before the first frame update
    void Start()
    {
        //�Է� ������ ���� �ʿ�
#if UNITY_EDITOR
        pointerID = -1; //PC�� ����Ƽ �󿡼��� -1
#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        pointerID = 0;  // �޴����̳� �̿ܿ��� ��ġ �󿡼��� 0 
#endif

        //���� �迭 �ʱ�ȭ
        //System.Array.Clear(map_cell,0,map_cell.Length);


        //sprite �޾ƿ���
        real_sprites = (Sprite[])Resources.LoadAll<Sprite>("Sprites/Blocks_Real");
        //object �޾ƿ���
        blockP = (GameObject[])Resources.LoadAll<GameObject>("Prefabs/Blocks_Real");

        //��ġ ����
        Object_normal_all = GameObject.Find("normal_all");
        Parent_normal_all = Object_normal_all.GetComponent<Transform>();

        cell_imp.clear_all();
        //cell ���� �о�� ���� ���� ����
        for (int cell_y=0; cell_y<world_sizeY; cell_y++)
        {
            for (int cell_x = 0; cell_x < world_sizeX; cell_x++)
            {
                //�о�� ������ cell�� �ִ´�. info, xy ��ǥ
                //�׽�Ʈ��129 ����
                //���� �۾��ϴ°� ������ ����� �����ؼ� ���̴� ������ ��ü
                cells cell_imp = new cells();

                //16384+128 16512     0/1/1
                //32768+128 32896     0/1/2
                //cell_imp.set_cell(16384*0 + 128*1, cell_x + start_x, cell_y + start_y);
                cell_imp.set_cell(0, cell_x + start_x, cell_y + start_y);
                map_cell[cell_x, cell_y] = cell_imp;
                //map_cell[cell_x, cell_y].set_cell(129, cell_x + start_x, cell_y + start_y);

            }
        }

        //cell ������ world �����-----------------------------------------------------------------
        //���� ���� ��ǥ�� ���� sprite �̹����� �ٲ��ش�.
        for (int cell_y = 0; cell_y < world_sizeY; cell_y++)
        {
            for (int cell_x = 0; cell_x < world_sizeX; cell_x++)
            {

                //�о�� ������ cell�� �ִ´�.
                cell_creation(map_cell[cell_x, cell_y],0);



            }
        }
        //--------------------------------------------------------------------------------------------
        //�ϴ� �ڵ�����
        //�� ���� �ݱ�
        sw = new StreamWriter("Assets/MapNote/" + map_string[map_num]);
        map_write();
        sw.Flush();
        sw.Close();

        //cell_imp �ʱ�ȭ
        cell_imp.clear_all();

        //grid ��ǥ�� �޾ƿ���
        //�߰��� ���� ��ǥ �޾ƿ���
        grid_pos = grid_sel_all.GetComponent<Transform>();
        player_pos = game_character.GetComponent<Transform>();

        //�ܺ� ��ũ��Ʈ ���� �ޱ�
        imp_wolf_control = game_character.GetComponent<wolf_control>();


    }

    // Update is called once per frame
    //���⼭ ��ġ�� �޾Ƽ� prefab�� �����Ѵ�.
    void Update()
    {
        //��ġ�� �޾Ƽ� �����Ѵ�.
        if (Input.GetMouseButtonDown(0) && (EventSystem.current.IsPointerOverGameObject(pointerID) == false))
        {
            Vector2 M_point;
            M_point = Input.mousePosition;

            //ī�޶� ��ġ �ޱ�
            float camera_z = Camera_editor.transform.position.z * (-1);

            Vector2 wp = Camera_editor.ScreenToWorldPoint(new Vector3(M_point.x, M_point.y, camera_z));
            //Vector3 wp = Camera_editor.main.ScreenToWorldPoint(Input.mousePosition);

           //ī�޶� ��� ��ǥ �ޱ�
            int imp_x = Mathf.RoundToInt(wp.x);
            int imp_y = Mathf.RoundToInt(wp.y);


            
            if((imp_x>=start_x) && (imp_x<start_x+world_sizeX) && (imp_y >= start_y) && (imp_y < start_y + world_sizeY))
            {
                //���� �� �Լ�
                if (play_version == 0)
                {
                    //�̰� ���� �Լ��̰� ���ο� �Լ��� ��ü
                    //Instantiate(blockP[7], new Vector3(imp_x, imp_y, 0), Quaternion.identity);
                    cell_imp.set_cell_point(imp_x, imp_y);
                    //cell_imp.set_cell_block(0,1,0);

                    //���� ���⿡ ������Ʈ�� �̹� �ִٸ� �������� �ʴ´�.
                    //�ش� ��ǥ�� �̹� ������ �ִٸ� �������� �ʴ´�.
                    int imp_x2 = imp_x - start_x;
                    int imp_y2 = imp_y - start_y;

                    if (map_cell[imp_x2, imp_y2].get_real() == 0)
                    {
                        cell_creation(cell_imp, 1);
                    }
                }
                //�ı� �� �Լ�
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

        //���� ��ġ�� ���� AvailGrid�� �̵���Ų��.
        //grid_sel_x = Mathf.RoundToInt(grid_x);
        //grid_sel_y = Mathf.RoundToInt(grid_y);
        grid_x = Mathf.Round(player_pos.position.x);
        //grid_y = Mathf.Round(player_pos.position.y-0.48f+1);
        //grid_y = Mathf.Round(player_pos.position.y + 1);
        grid_y = Mathf.Round(player_pos.position.y + grid_acc);

        grid_pos.position = new Vector3(grid_x,grid_y);

        //�߰������� map_cell�� �޾ƿͼ� ������ ������ �͵鸸 ǥ���Ѵ�.
        //���ŵǴ� Ÿ�̹��� üũ�Ѵ�.
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
                //��Ȳ ����
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



    //�߰��� ��������Ʈ�� �����Ǿ ���� ���� �ε����� �Ҿ�´�.
    int call_indexS(int num)
    {
        int imp = 0;

        for(int i=0; i<num; i++)
        {
            imp = imp + block_RealS_index[i];
        }

        return imp;
    }
    //�߰��� �������� �����Ǿ ���� ���� �ε����� �Ҿ�´�.
    int call_indexP(int num)
    {
        int imp = 0;

        for (int i = 0; i < num; i++)
        {
            imp = imp + block_RealP_index[i];
        }

        return imp;
    }


    //���� ����
    //cell������ ������� ����� �����Ѵ�.----------------------------------------------------
    // type 0�� �⺻ ���� �κ�, ���� ������ ���� �ÿ��� ����ȴ�.
    // type 1�� ���+���(blocks_back + blocks_real)
    // type 2�� ��游(blocks_back)
    // type 3�� ��Ϻ�����(blocks_sub) -> �߰��� 1���� 
    // type 4�� Ư����ϸ�(blocks_special) -> �߰��� 1����
    void cell_creation(cells imp,int type)
    {
        //��ǥ �����
        int int_x;
        int int_y;
        int_x = imp.get_x();
        int_y = imp.get_y();

        //�ӽ� ��� ����
        int int_imp1 = 0;
        int int_imp2 = 0;
        int int_inst = 0;

        //��������Ʈ�� Ȱ��
        int sprite_num1 = 0;
        int sprite_num2 = 0;

        //��ǥ�� ���� �μ�(������ �����)
        int sprite_x = 0;
        int sprite_y = 0;

        //�켱 ���� ������ ��� base_blocks�� �����Ѵ�.---------
        if(type == 0)
        {
            impObject_base = Instantiate(base_Blocks, new Vector3(int_x, int_y, 0), Quaternion.identity);
            impObject_base.transform.parent = Parent_normal_all;
        }
        //------------------------------------------------------


        //��� ���� �ޱ�
        //�̰� ���� �۾��� ���� �ʾҴ�.
        int_imp1 = imp.get_back();


        //���� ��� ���� �ޱ�---------------------------------------
        int_imp1 = imp.get_real();


        //�ǹ��� ���� Ÿ�Ե� �޶�����.
        //type ������ �ܴ�.
        if (int_imp1 != 0 && (type==0 || type ==1))
        {

            //��� ���� Ÿ��
            int_imp2 = imp.get_transType();

            //���ǹ��� ���� ���� ����� �����Ѵ�.
            //�ε����� ���� ����� ������ �����ϰ� ��ǥ�� �޴´�.
            int_inst = call_indexP(int_imp1)+int_imp2;


            map_cell[int_x - start_x, int_y - start_y].set_cell_block(0,int_imp1, int_imp2);
            //Debug.Log("x��ǥ : " + (int_x - start_x) + " / y��ǥ : " + (int_y - start_y));
            //Debug.Log("real�� : " + int_imp1 + " / trans�� : " + int_imp2);
            //Debug.Log("real�� : " + map_cell[int_x - start_x, int_y - start_y].get_real() + " / trans�� : " + map_cell[int_x - start_x, int_y - start_y].get_transType());
            impObject = Instantiate(blockP[int_inst], new Vector3(int_x, int_y, 0), Quaternion.identity);

            //���� ��ġ�� �̸� �ٲ���´�.
            //���� ������ ���
            if(type==0)
            {
                impObject.transform.parent = impObject_base.transform.GetChild(1);
            }
            //�ΰ� ������ ���
            else if (type == 1)
            {
                call_created_cell(int_x-start_x, int_y-start_y, 1);
                impObject.transform.parent = impObject_base_child;
            }



            //prefab�� ���� �� ������Ʈ�� �̹����� �ٲ��ش�.
            //�̴� ��� ���� �������� �ַ� ����ȴ�.
            //int_x int_y���� ��ǥ������ �����Ѵ�.


            //���, �ڰ��� ���
            if (int_imp1 == 1 || int_imp1 == 2)
            {
                int indexS = call_indexS(int_imp1);

                //mod�� ���� ��ġ ����, ������ �����
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


                //���� Ÿ�Կ� ���� ��������Ʈ ����
                //�⺻ ����
                if(int_imp2 == 0)
                {
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    imp_child2 = impObject.transform.GetChild(1).gameObject;
                    
                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22);
                    sprite_num2 = sprite_x + ((4 - sprite_y) * 22)+11;

                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1+ indexS];
                    imp_child2.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num2+ indexS];
                    
                }
                //���� �޳���
                else if(int_imp2 == 1)
                {
                    
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 110 + 11;
                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1 + indexS];
                    

                }
                //���� �޳���
                else if (int_imp2 == 2)
                {
                    
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    imp_child2 = impObject.transform.GetChild(1).gameObject;

                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 110;
                    sprite_num2 = sprite_x + ((4 - sprite_y) * 22) + 11;

                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1+ indexS];
                    imp_child2.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num2+ indexS];
                    
                }
                //���� ��������
                else if (int_imp2 == 3)
                {
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    imp_child2 = impObject.transform.GetChild(1).gameObject;

                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 220;
                    sprite_num2 = sprite_x + ((4 - sprite_y) * 22) + 11;

                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1 + indexS];
                    imp_child2.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num2 + indexS];


                }
                //���� ��������
                else if (int_imp2 == 4)
                {
                    imp_child1 = impObject.transform.GetChild(0).gameObject;
                    sprite_num1 = sprite_x + ((4 - sprite_y) * 22) + 220 + 11;
                    imp_child1.GetComponent<SpriteRenderer>().sprite = real_sprites[sprite_num1 + indexS];

                }
            }

        }
        //����� ���� Ư�� ����� ���
        else
        {
            int_imp1 = imp.get_special_count();
            
            //���� ��ŭ ������� �����Ѵ�.
            for(int i=0; i<int_imp1; i++)
            {
                //������� �����ϴϱ� ������ ���� �ʴ´�.
                int_imp2 = imp.get_special(i);

                //Instantiate...

            }


        }

    } 
    //version ��ȯ
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

    //��� �ı�
    public void block_destory()
    {
        int x3 = dest_x;
        int y3 = dest_y;

        Destroy(normals.transform.GetChild(x3 + y3 * world_sizeX).transform.GetChild(1).transform.GetChild(0).gameObject);

        map_cell[x3, y3].clear_all();

    }

    //-------------------------------------------------------------------------------------
    //�ش� ��ǥ�� type(0~3)�� ���� child�� �ҷ��´�
    void call_created_cell(int x, int y, int type)
    {
        //GameObject impObject_created;
        impObject_base_child = Object_normal_all.transform.GetChild(y * world_sizeX + x).gameObject.transform.GetChild(type);
        //impObject_base_child
    }

    
    //�� �о�ͼ� �����ϱ�
    void map_creation()
    {
        string map_path = "MapNote/�̸�";

        if(false == File.Exists(map_path))
        {
            var file = File.CreateText(map_path+".txt");
            file.Close();
        };
         
    }

    //���� �ʿ� ���� �� ����
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

    //��ġ �ȿ� �ִ��� Ȯ��
    public bool in_cells(float val_x, float val_y)
    {
        //int imp_cell_x = (int)val_x;
        //int imp_cell_y = (int)val_y;

        if (( val_x >= start_x && val_x < start_x + world_sizeX) && (val_y >= start_y && val_y < start_y + world_sizeY))
        {
            //Debug.Log("���ο� ���� Ž��");
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

    //���� ��ġ�� ���⿡ ���������Ʈ Ȱ��ȭ
    
    //���� ��� ����
    public class cells
    {
        
        //��ǥ��
        private int cells_x = 0, cells_y = 0;

        //���
        private int blocks_back = 0;

        //�⺻���
        private int blocks_real = 0;
        //�⺻��� ���� Ÿ��(�⺻����� 0�� �ƴϾ�ߵȴ�)
        private int real_trans_type = 0;

        //��Ϻ��� ����
        //�⺻������ block_real�� 0�� �ƴϾ�� �ȴ�.
        //��Ǯ, ȭ���� ���� ���
        private List<int> blocks_sub = new List<int>();


        //Ư����� (�߰� ���Ž� �� clear �ϵ��� ����)
        //�⺻������ block_real�� 0�̾�ߵȴ�. (���ߵȴ�)
        //���ڿ� ���� ���
        private List<int> blocks_special = new List<int>();

        //������
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

        //����� �뵵�ε� ���ȴ�
        public void clear_all()
        {
            blocks_back = 0;
            blocks_real = 0;
            real_trans_type = 0;

            blocks_sub.Clear();
            blocks_special.Clear();
        }

        //�� �����ϱ�
        //���� ���� parsing �� ������ �����´�.
        public int get_cell()
        {
            int cell_int = 0;

            int cell_index = 0;

            //��� �߰�
            cell_int = cell_int + blocks_back;
            cell_index = cell_index + 7;

            //��� �߰�
            cell_int = cell_int + (blocks_real<<cell_index);
            cell_index = cell_index + 7;

            //�ǹ��� ���� ���Ǻ�
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

        //cell�� ��ġ�� info���� ���� cell���� ����
        public void set_cell(int info, int x, int y)
        {
            cells_x = x;
            cells_y = y;

            int parsing_int = info;
            int parsed_int = 0;
            int parsing_index = 0;

            int sub_count = 0;
            int special_count = 0;

            //��� ��������
            parsed_int = int_parser(parsing_int, parsing_index, 7);
            parsing_index = parsing_index + 7;
            blocks_back = parsed_int;

            //�ǹ� ��������
            parsed_int = int_parser(parsing_int, parsing_index, 7);
            parsing_index = parsing_index + 7;
            blocks_real = parsed_int;

            //�ǹ��� 0�ΰ��� ���ؼ� �޶�����.
            //0�� �ƴϸ� �Ϲ���, 0�̸� ������Ʈ���̴�.

            //�Ϲ����� ���
            if(blocks_real != 0)
            {
                //�ǹ� ���� ����
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
            //Ư������ ���
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

        //��� ������ ���
        //���� ���ڸ� �־ �����
        //��Ͽ� ���� ����
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

            //�ٸ� ��ϵ� �̷��� ������ �ȴ�.
           
            blocks_real = real;

            real_trans_type = type;
        }

        //�̰� ���߿�...(special�� �ش�)
        public void set_cell_object()
        {

        }


        //���� �Ľ� �Լ�
        //par�� ���ڸ� start_pos(0�� ����)��ġ���� length��ŭ �о�´�.

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
