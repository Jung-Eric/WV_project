using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//오른쪽으로 튕겨내는 스크립트
public class wolf_left_col : MonoBehaviour
{
    public int col_left;

    // Start is called before the first frame update
    void Start()
    {
        col_left = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "World")
        {
            col_left = 1;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "World")
        {
            col_left = 1;
        }
    }

}
