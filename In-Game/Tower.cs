 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//타워 자체적으로 사용하는 코드. 매니저가 존재함
enum IsThis50Alpha
{
    Alpha50,//로드아웃용
    AttackTower//실제공격용
}

enum TowerKind
{
    Normal,
    Explosive
}

public class Tower : MonoBehaviour
{
    float time = 0;//타워가 배치되고 흐른 시간. 탄환 연사 확인용
    Vector3 towerPlacement;

    [SerializeField] IsThis50Alpha isThis50Alpha;
    [SerializeField] TowerKind towerKind;

    [SerializeField] int thisTowerID = 0;//이 탑의 프리펩 ID. 풀에 넣어야 할때(팔때) 쓰임

    public GameObject[] bullet;//사격할 탄환 종류
    [SerializeField] SpriteRenderer lookingRangeRadius;

    BoxCollider2D towerCollider;

    //GameObject closeEmemy = null;

    //List<GameObject> collisionEnemy = new List<GameObject>();

    RaycastHit2D[] enemy;
    [Header("Tower Basic stat")]
    [SerializeField] float towerSize = 2;//타워크기
    public float GetTowerSize => towerSize;
    //public float GetTowerSize { get => towerSize; }

    [SerializeField] float range = 3;//사거리
    float BaseRange;//사거리
    [SerializeField] float rapidShot = 1;//연사력
    float baseRapidShot;
    float plusShotSpeed = 0;//추가탄환속도
    float plusDamage = 0;//추가피해량
    int plusPierce = 0;//추가관통력
    float plusExplosive = 0;//추가폭발력

    [Header("Pierce")]
    [SerializeField] int PierceLevel = 1;//관통력 레벨
    [SerializeField] float PierceLevelPerUp = 0.5f;//레벨당 오르는 관통력수
    [SerializeField] int PierceLevelPerUpCost = 300;//레벨당 오르는 관통력 비용
    [SerializeField] int PierceCost = 300;//관통력 비용
    public int GetPierceLevel => PierceLevel;
    public int GetPierceCost => PierceCost;
    public void LevelUPPierceLevel()
    {
        PierceLevel += 1;
    }

    [Header("Range")]
    [SerializeField] int RangeLevel = 1;//사거리 레벨
    [SerializeField] float RangeLevelPerUp = 0.3f;//레벨당 오르는 사거리
    [SerializeField] int RangeLevelPerUpCost = 500;//레벨당 오르는 사거리 비용
    [SerializeField] int RangeCost = 300;//관통력 비용
    public int GetRangeLevel => RangeLevel;
    public int GetRangeCost => RangeCost;
    public void LevelUPRangeLevel()
    {
        RangeLevel += 1;
    }

    [Header("Damage")]
    [SerializeField] int damageLevel = 1;//피해량 레벨
    [SerializeField] float damageLevelPerUp = 3f;//레벨당 오르는 피해량
    [SerializeField] int damageLevelPerUpCost = 600;//레벨당 오르는 피해량 비용
    [SerializeField] int damageCost = 300;//관통력 비용
    public int GetdamageLevel => damageLevel;
    public int GetdamageCost => damageCost;
    public void LevelUPDamageLevel()
    {
        damageLevel += 1;
    }

    [Header("Special")]
    [SerializeField] int SPLevel = 1;//특별한 레벨
    [SerializeField] float SPLevelPerUp = 0.8f;//레벨당 오르는 특별계수
    [SerializeField] int SPLevelPerUpCost = 1000;//레벨당 오르는 특별계수 비용
    [SerializeField] int SPCost = 300;//관통력 비용
    public int GetSPLevel => SPLevel;
    public int GetSPCost => SPCost;
    public void LevelUPSPLevel()
    {
        SPLevel += 1;
    }

    [SerializeField] int towerCost = 100;//비용
    int totalTowerCost = 100;//사용금
    public int GetTotalTowerCost => totalTowerCost;
    public void totalTowerCostPlus(int cost)
    {
        totalTowerCost+=cost;
    }

    SpriteRenderer rend;
    Animator anim;
    AudioSource audio;

    private void Start()
    {
        towerCollider = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        //타워의 크기. 큰 타워는 그만큼 페널티를 가진다.
        towerCollider.size = new Vector2(towerSize, towerSize);
        BaseRange = range;
        baseRapidShot = rapidShot;
        totalTowerCost = towerCost;
    }

    private void OnEnable()
    {
        towerPlacement = transform.position;
        towerPlacement.z = 0;
        //배치된 타워는 재이동을 하지 않으므로 고정된다.

        enemy = Physics2D.CircleCastAll(towerPlacement, range, Vector2.right, 0, 1 << LayerMask.NameToLayer("Enemy"));
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(towerPlacement, range);
    //}
    bool needResearch = false;
    float minDistance = Mathf.Infinity;
    int targetI = 0;
    private void FixedUpdate()
    {
        if (isThis50Alpha == IsThis50Alpha.AttackTower)
        {
            if (enemy.Length == 0 || needResearch == true)//접촉 적군이 없으면 적을 탐색한다
            {
                time = 0;

                enemy = Physics2D.CircleCastAll(towerPlacement, range, Vector2.right, 0, 1 << LayerMask.NameToLayer("Enemy"));
                
                for(int i = 0; i < enemy.Length; i++)
                {
                    float distance = Vector2.Distance(transform.position, enemy[i].transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetI = i;
                    }

                }
                //현재 들어간 모든 적군의 거리를 확인해서 제일 가까운 적을 타겟으로 한다.
                needResearch = false;//재탐색이 필요한가?
            }
            else if (enemy.Length != 0 && needResearch == false)//적을 탐색했다면 탐색을 중단하고 그 적만 죽어라 공격한다.
            {
                Enemy target = enemy[0].collider.GetComponent<Enemy>();//처음 걸린 적을 타겟으로 삼고

                Vector3 direction = (enemy[0].transform.position - transform.position).normalized;
                if (direction.x < 0)
                {
                    rend.flipX = true;
                }
                else
                {
                    rend.flipX = false;
                }

                //Debug.DrawLine(transform.position, enemy[0].transform.position);
                time += Time.deltaTime;

                if (time > rapidShot)//연사시간이 되면 탄환을 만들어서 보낸다
                {
                    time = 0;
                    var aBullet = TowerManager.instance.AddBullet(bullet[0], transform);//탄환 풀에서 소속탄환을 꺼냄

                    anim.SetTrigger("shot");
                    audio.Play();

                    var bulletCheck = aBullet.GetComponent<NormalBullet>();//탄환에 스크립트가 있으면
                    if (bulletCheck)
                    {
                        var dir = (target.transform.position - towerPlacement).normalized;//방향을 지정한다
                        bulletCheck.targetPosition = dir;//탄환 돌리기
                        bulletCheck.SetDamage(bulletCheck.GetDamage(), plusDamage);
                        bulletCheck.SetPierce(bulletCheck.GetPierce(), plusPierce);
                        bulletCheck.SetBlast(bulletCheck.GetBlast(), plusExplosive);
                        //탄환의 피해량과 관통력은 타워의 보너스 피해량과 보너스 관통력을 탄환에 더해 최종산출한다.

                        var bulletRigid = bulletCheck.GetComponent<Rigidbody2D>();
                        if (bulletRigid)
                        {
                            bulletRigid.velocity = dir * (bulletCheck.GetShotSpeed() + plusShotSpeed);//속도와 방향을 기준으로 탄환이 나간다

                        }
                    }
                }
                
                if (Vector2.Distance(transform.position, enemy[0].transform.position) > range || !target.isActiveAndEnabled)
                {//적이 공격범위를 넘어서면 다시 범위를 탐지한다. 없으면 NULL 이 된다.
                    enemy = Physics2D.CircleCastAll(towerPlacement, range, Vector2.right, 0, 1 << LayerMask.NameToLayer("Enemy"));
                    //needResearch = true;
                    minDistance = Mathf.Infinity;
                }
            }
        }
    }
    

    public void TowerSell()
    {
        UIManager.instance.RangeOff();
        UIManager.instance.UpgradeOff();
        GameManager.instance.costPlus(totalTowerCost * 0.7f);//판매금은 구매금의 70%
        TowerManager.instance.DestoryTower(this.gameObject, thisTowerID);

        PierceLevel = 1;
        RangeLevel = 1;
        damageLevel = 1;
        SPLevel = 1;
        totalTowerCost = towerCost;
        UpgradeMath();
    }
    public void UpgradeMath()
    {
        SoundManager.instance.ButtonSound(0);

        plusPierce = (int)((PierceLevel - 1) * PierceLevelPerUp);
        range = BaseRange + ((RangeLevel - 1) * RangeLevelPerUp);
        plusDamage = (damageLevel - 1) * damageLevelPerUp;

        switch (towerKind)
        {
            case TowerKind.Normal:
                rapidShot = baseRapidShot * Mathf.Pow(SPLevelPerUp, (SPLevel - 1));
                anim.SetFloat("animSpeed", 1 + (0.1f * (SPLevel - 1)));
                break;
            case TowerKind.Explosive:
                plusExplosive = (SPLevel - 1) * SPLevelPerUp;
                break;
        }
        PierceCost = PierceLevel * PierceLevelPerUpCost;
        RangeCost = RangeLevel * RangeLevelPerUpCost;
        damageCost = damageLevel * damageLevelPerUpCost;
        SPCost = SPLevel * SPLevelPerUpCost;
    }

    public float GetTowerRange()
    {
        return range;
    }
    public float GetTowerRapid()
    {
        return rapidShot;
    }
    public float GetTowerShotSpeed()
    {
        return plusShotSpeed;
    }
    public int GetTowerCost()
    {
        return towerCost;
    }
}
