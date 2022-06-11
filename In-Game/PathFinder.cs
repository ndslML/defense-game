using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy 에 넣음

public class PathFinder : MonoBehaviour
{
    int wayPointCount;
    Transform[] wayPoint;
    int currentIndex = 0;
    Movement2D movement2D;
    [SerializeField] int ID;

    public void Setup(Transform[] wayPoints)
    {
        currentIndex = 0;
        movement2D = GetComponent<Movement2D>();
        //적의 이동경로 웨이포인트를 세팅하고
        wayPointCount = wayPoints.Length;
        wayPoint = new Transform[wayPointCount];
        this.wayPoint = wayPoints;
        //적 위치를 첫 웨이포인트로 지정한다
        transform.position = wayPoints[currentIndex].position;
        //적군의 이동과 목표지점을 설정하는 코루틴 함수를 작동.
        StartCoroutine("OnMove");
    }

    IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            //transform.Rotate(Vector3.forward * 10);
            //현위치와 목표거리가 일정량 이하일때 다음 경로를 찾는다
            //거리에 별도로 속도를 곱하는 이유는 경로 이탈을 방지하는 목적.
            if (Vector3.Distance(transform.position, wayPoint[currentIndex].position) < 0.05f * movement2D.Movespeed)
            {
                NextMoveTo();
            }
            yield return null;
        }

    }

    void NextMoveTo()
    {
        if (currentIndex < wayPointCount - 1)
        {
            transform.position = wayPoint[currentIndex].position;

            currentIndex++;
            Vector3 direction = (wayPoint[currentIndex].position - transform.position).normalized;
            //movement2D.MoveTo(direction);
        }
        else
        {
            EnemyManager.instance.DestoryEnemy(this.gameObject, ID);
        }
    }
}
