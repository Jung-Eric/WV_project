using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_control : MonoBehaviour
{
    public GameObject player;

    private Vector3 distance;


    // 0 : 맵 에딧 중. 1 : 플레이 중, 2 : 대화 중(연출)
    public int play_version = 0;

    //기본 이동, 점프, 튕기기의 값을 가진다.
    public Vector3 position;

    //카메라 이동속도
    public float base_speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Camera>().orthographicSize = 2f;
        distance = transform.position - player.transform.position;
        position = transform.position;
        
    }

    // Update is called once per frame
    /*
    void Update()
    {
        
    }
    */

    private void FixedUpdate()
    {
        if (play_version == 0)
        {
            Dir4_Move();
        }
    }

    private void LateUpdate()
    {
        //이동처리
        if (play_version == 1)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position + distance, 3.0f * Time.deltaTime);
        }
    }


    private void Dir4_Move()
    {
        if (play_version == 0)
        {
            position.x += base_speed * Input.GetAxisRaw("Horizontal");
            position.y += base_speed * Input.GetAxisRaw("Vertical");

            transform.position = position;
        }
    }

    public void Change_play()
    {

        if(play_version == 0)
        {
            //버전 전환과 위치 초기화
            play_version = 1;
            
        }
        else if(play_version == 1)
        {
            play_version = 0;
        }
    }

}
