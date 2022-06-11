using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void AnimStart()
    {
        anim.SetBool("loading", true);
    }

    public void AnimStop()
    {
        anim.speed = 0;
    }
}
