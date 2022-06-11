using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적군을 담당한다. 매니저가 존재한다.
public class Enemy : MonoBehaviour
{
    //pathFinder 변수
    int wayPointCount;
    Transform[] wayPoint;
    int currentIndex = 0;
    Movement2D movement2D;
    [SerializeField] int ID;
    //pathFinder 끝

    [SerializeField] float baseMaxHitPoint = 100;
    [SerializeField] float maxHitPoint = 100;//최대체력
    [SerializeField] int hitPointPlusRound = 15;
    [SerializeField] float HPRoundMulti = 0.05f;
    float hitPoint = 100;//현재체력
    float baseCost = 100;
    [SerializeField] int cost = 100;//무찌르면 줄 금액
    [SerializeField] int costMinusRound = 50;
    [SerializeField] float costRoundMulti = 0.02f;
    [SerializeField]float baseSpeed = 0;
    [SerializeField] float speed = 0;//속도
    [SerializeField] int speedPlusRound = 30;
    [SerializeField] float speedRoundMulti = 0.05f;

    [SerializeField] GameObject effect;

    SpriteRenderer rend;

    private void Awake()
    {
        baseMaxHitPoint = maxHitPoint;
        baseCost = cost;
        baseSpeed = speed;

        rend = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (IsEnemyDown())
        {//죽으면
            TowerManager.instance.AddEffect(effect, transform);
            SoundManager.instance.ShotBoomSoundLow(5);
            GameManager.instance.costPlus(cost);
            EnemyManager.instance.DestoryEnemy(this.gameObject, ID);
        }
    }

    public bool IsEnemyDown()
    {//죽었는지 확인하는 용도.
        
        return hitPoint <= 0;
    }
    public void MinusHitPoint(float damage)
    {
        //hitPoint = Mathf.Max(hitPoint, 0);둘 중 큰값을 되돌린다
        hitPoint -= damage;

    }

    public float GetMaxHP()
    {
        return maxHitPoint;
    }
    public float GetCurrentHP()
    {
        return hitPoint;
    }

    //PathFinder 함수
    public void Setup(Transform[] wayPoints)
    {
        currentIndex = 0;

        var HitPointPercent = (Mathf.Max(WaveSystem.instance.currentWave - hitPointPlusRound, 0) * HPRoundMulti + 1);
        
        var costPercent = (Mathf.Max(1 - (Mathf.Max(WaveSystem.instance.currentWave - costMinusRound, 0) * costRoundMulti), 0.5f));

        var speedPercent = (Mathf.Max(WaveSystem.instance.currentWave - speedPlusRound, 0) * speedRoundMulti + 1);

        maxHitPoint = baseMaxHitPoint * HitPointPercent;
        cost = (int)(baseCost * costPercent);
        speed = baseSpeed * speedPercent;
        hitPoint = maxHitPoint;

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
            if (direction.x == -1)
            {
                rend.flipX = true;
            }
            else
            {
                rend.flipX = false;
            }

            movement2D.MoveTo(speed, direction);
        }
        else
        {
            SoundManager.instance.ShotBoomSound(4);
            EnemyManager.instance.PathFinishEnemyDistory(hitPoint);
            UIManager.instance.DestoryBar(gameObject);
            EnemyManager.instance.DestoryEnemy(this.gameObject, ID);
        }
    }
    //pahtFinder 종료
}
