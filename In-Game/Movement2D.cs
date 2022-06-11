using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̵��� ����Ѵ�

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
    ���� ���� : �̵� ���� ������Ʈ���ٰ� �ִ´�
    �Լ�
    MoveTo() �ܺο��� ������ ��������
    */
}
