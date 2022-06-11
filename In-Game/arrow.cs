using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour
{
    bool isClick = false;
    private void OnMouseOver()
    {
        isClick = true;
        
    }

    private void OnMouseExit()
    {
        isClick = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isClick)
            {

                Destroy(gameObject);
            }
        }
    }
}
