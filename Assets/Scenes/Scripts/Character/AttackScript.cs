using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject sheatedSword;
    public GameObject unsheatedSword;
    public bool switchBool;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SheatheInAndOut();
        }
    }


    public void LightAttack(Animator animator)
    {


    }

    public void HeavyAttack(Animator animator)
    {


    }

    public void SheatheInAndOut()
    {
        if(switchBool)
        {
            sheatedSword.SetActive(false);
            unsheatedSword.SetActive(true);
            switchBool = false;
        }else if(!switchBool)
         {

            sheatedSword.SetActive(true);
            unsheatedSword.SetActive(false);
            switchBool = true;

         }
        Debug.Log("Method was called");


    }


}
