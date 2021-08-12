using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf_foot_col : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update

    float imp_num = -1;

    
    /*
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    */
    private void OnTriggerExit2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "World" || col.gameObject.tag == "slope")
        {
            player.GetComponent<wolf_control>().is_landing = false;

        }
        else if (col.gameObject.tag == "stephold")
        {
            player.GetComponent<wolf_control>().is_landing = false;
            player.GetComponent<wolf_control>().is_step = false;

        }


    }

    //충돌 유지 시 떠있을 수 있게 만든다.
    private void OnTriggerEnter2D(Collider2D col)
    {

        /*
        if (col.gameObject.tag == "World")
        {


        }
        else if (col.gameObject.tag == "stephold")
        {


        }


        */
        if (col.gameObject.tag == "stephold")
        {
  
            imp_num = col.gameObject.GetComponent<Transform>().position.y + 4.0f;

            player.GetComponent<wolf_control>().limited_pos = imp_num;

            player.GetComponent<wolf_control>().is_landing = true;

            player.GetComponent<wolf_control>().is_downjump = false;

            player.GetComponent<wolf_control>().is_step = true;

        }
        else if (col.gameObject.tag == "World" || col.gameObject.tag == "slope")
        {

            imp_num = col.gameObject.GetComponent<Transform>().position.y + 4.0f;

            player.GetComponent<wolf_control>().limited_pos = imp_num;

            player.GetComponent<wolf_control>().is_landing = true;

            player.GetComponent<wolf_control>().is_downjump = false;

        }

    }
    

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "World" || col.gameObject.tag == "slope")
        {
            player.GetComponent<wolf_control>().is_landing = true;

        }
        else if (col.gameObject.tag == "stephold")
        {
            player.GetComponent<wolf_control>().is_landing = true;

            player.GetComponent<wolf_control>().is_step = true;

        }
    }

}
