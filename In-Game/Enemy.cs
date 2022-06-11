using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ ����Ѵ�. �Ŵ����� �����Ѵ�.
public class Enemy : MonoBehaviour
{
    //pathFinder ����
    int wayPointCount;
    Transform[] wayPoint;
    int currentIndex = 0;
    Movement2D movement2D;
    [SerializeField] int ID;
    //pathFinder ��

    [SerializeField] float baseMaxHitPoint = 100;
    [SerializeField] float maxHitPoint = 100;//�ִ�ü��
    [SerializeField] int hitPointPlusRound = 15;
    [SerializeField] float HPRoundMulti = 0.05f;
    float hitPoint = 100;//����ü��
    float baseCost = 100;
    [SerializeField] int cost = 100;//����� �� �ݾ�
    [SerializeField] int costMinusRound = 50;
    [SerializeField] float costRoundMulti = 0.02f;
    [SerializeField]float baseSpeed = 0;
    [SerializeField] float speed = 0;//�ӵ�
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
        {//������
            TowerManager.instance.AddEffect(effect, transform);
            SoundManager.instance.ShotBoomSoundLow(5);
            GameManager.instance.costPlus(cost);
            EnemyManager.instance.DestoryEnemy(this.gameObject, ID);
        }
    }

    public bool IsEnemyDown()
    {//�׾����� Ȯ���ϴ� �뵵.
        
        return hitPoint <= 0;
    }
    public void MinusHitPoint(float damage)
    {
        //hitPoint = Mathf.Max(hitPoint, 0);�� �� ū���� �ǵ�����
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

    //PathFinder �Լ�
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
    //pahtFinder ����
}
