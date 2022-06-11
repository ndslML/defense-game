using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이동을 담당한다

public class Movement2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.0f;
    [SerializeField] Vector3 moveDirection = Vector3.zero;

    public float Movespeed => moveSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void MoveTo(float speed, Vector3 direction)
    {
        moveDirection = direction;
        moveSpeed = speed;
    }

    /*
    파일 설명 : 이동 가능 오브젝트에다가 넣는다
    함수
    MoveTo() 외부에서 방향을 지정해줌
    */
}
