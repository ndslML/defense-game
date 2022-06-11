using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy �� ����

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
        //���� �̵���� ��������Ʈ�� �����ϰ�
        wayPointCount = wayPoints.Length;
        wayPoint = new Transform[wayPointCount];
        this.wayPoint = wayPoints;
        //�� ��ġ�� ù ��������Ʈ�� �����Ѵ�
        transform.position = wayPoints[currentIndex].position;
        //������ �̵��� ��ǥ������ �����ϴ� �ڷ�ƾ �Լ��� �۵�.
        StartCoroutine("OnMove");
    }

    IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            //transform.Rotate(Vector3.forward * 10);
            //����ġ�� ��ǥ�Ÿ��� ������ �����϶� ���� ��θ� ã�´�
            //�Ÿ��� ������ �ӵ��� ���ϴ� ������ ��� ��Ż�� �����ϴ� ����.
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
