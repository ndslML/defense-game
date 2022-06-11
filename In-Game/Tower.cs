 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ÿ�� ��ü������ ����ϴ� �ڵ�. �Ŵ����� ������
enum IsThis50Alpha
{
    Alpha50,//�ε�ƿ���
    AttackTower//�������ݿ�
}

enum TowerKind
{
    Normal,
    Explosive
}

public class Tower : MonoBehaviour
{
    float time = 0;//Ÿ���� ��ġ�ǰ� �帥 �ð�. źȯ ���� Ȯ�ο�
    Vector3 towerPlacement;

    [SerializeField] IsThis50Alpha isThis50Alpha;
    [SerializeField] TowerKind towerKind;

    [SerializeField] int thisTowerID = 0;//�� ž�� ������ ID. Ǯ�� �־�� �Ҷ�(�ȶ�) ����

    public GameObject[] bullet;//����� źȯ ����
    [SerializeField] SpriteRenderer lookingRangeRadius;

    BoxCollider2D towerCollider;

    //GameObject closeEmemy = null;

    //List<GameObject> collisionEnemy = new List<GameObject>();

    RaycastHit2D[] enemy;
    [Header("Tower Basic stat")]
    [SerializeField] float towerSize = 2;//Ÿ��ũ��
    public float GetTowerSize => towerSize;
    //public float GetTowerSize { get => towerSize; }

    [SerializeField] float range = 3;//��Ÿ�
    float BaseRange;//��Ÿ�
    [SerializeField] float rapidShot = 1;//�����
    float baseRapidShot;
    float plusShotSpeed = 0;//�߰�źȯ�ӵ�
    float plusDamage = 0;//�߰����ط�
    int plusPierce = 0;//�߰������
    float plusExplosive = 0;//�߰����߷�

    [Header("Pierce")]
    [SerializeField] int PierceLevel = 1;//����� ����
    [SerializeField] float PierceLevelPerUp = 0.5f;//������ ������ ����¼�
    [SerializeField] int PierceLevelPerUpCost = 300;//������ ������ ����� ���
    [SerializeField] int PierceCost = 300;//����� ���
    public int GetPierceLevel => PierceLevel;
    public int GetPierceCost => PierceCost;
    public void LevelUPPierceLevel()
    {
        PierceLevel += 1;
    }

    [Header("Range")]
    [SerializeField] int RangeLevel = 1;//��Ÿ� ����
    [SerializeField] float RangeLevelPerUp = 0.3f;//������ ������ ��Ÿ�
    [SerializeField] int RangeLevelPerUpCost = 500;//������ ������ ��Ÿ� ���
    [SerializeField] int RangeCost = 300;//����� ���
    public int GetRangeLevel => RangeLevel;
    public int GetRangeCost => RangeCost;
    public void LevelUPRangeLevel()
    {
        RangeLevel += 1;
    }

    [Header("Damage")]
    [SerializeField] int damageLevel = 1;//���ط� ����
    [SerializeField] float damageLevelPerUp = 3f;//������ ������ ���ط�
    [SerializeField] int damageLevelPerUpCost = 600;//������ ������ ���ط� ���
    [SerializeField] int damageCost = 300;//����� ���
    public int GetdamageLevel => damageLevel;
    public int GetdamageCost => damageCost;
    public void LevelUPDamageLevel()
    {
        damageLevel += 1;
    }

    [Header("Special")]
    [SerializeField] int SPLevel = 1;//Ư���� ����
    [SerializeField] float SPLevelPerUp = 0.8f;//������ ������ Ư�����
    [SerializeField] int SPLevelPerUpCost = 1000;//������ ������ Ư����� ���
    [SerializeField] int SPCost = 300;//����� ���
    public int GetSPLevel => SPLevel;
    public int GetSPCost => SPCost;
    public void LevelUPSPLevel()
    {
        SPLevel += 1;
    }

    [SerializeField] int towerCost = 100;//���
    int totalTowerCost = 100;//����
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
        //Ÿ���� ũ��. ū Ÿ���� �׸�ŭ ���Ƽ�� ������.
        towerCollider.size = new Vector2(towerSize, towerSize);
        BaseRange = range;
        baseRapidShot = rapidShot;
        totalTowerCost = towerCost;
    }

    private void OnEnable()
    {
        towerPlacement = transform.position;
        towerPlacement.z = 0;
        //��ġ�� Ÿ���� ���̵��� ���� �����Ƿ� �����ȴ�.

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
            if (enemy.Length == 0 || needResearch == true)//���� ������ ������ ���� Ž���Ѵ�
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
                //���� �� ��� ������ �Ÿ��� Ȯ���ؼ� ���� ����� ���� Ÿ������ �Ѵ�.
                needResearch = false;//��Ž���� �ʿ��Ѱ�?
            }
            else if (enemy.Length != 0 && needResearch == false)//���� Ž���ߴٸ� Ž���� �ߴ��ϰ� �� ���� �׾�� �����Ѵ�.
            {
                Enemy target = enemy[0].collider.GetComponent<Enemy>();//ó�� �ɸ� ���� Ÿ������ ���

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

                if (time > rapidShot)//����ð��� �Ǹ� źȯ�� ���� ������
                {
                    time = 0;
                    var aBullet = TowerManager.instance.AddBullet(bullet[0], transform);//źȯ Ǯ���� �Ҽ�źȯ�� ����

                    anim.SetTrigger("shot");
                    audio.Play();

                    var bulletCheck = aBullet.GetComponent<NormalBullet>();//źȯ�� ��ũ��Ʈ�� ������
                    if (bulletCheck)
                    {
                        var dir = (target.transform.position - towerPlacement).normalized;//������ �����Ѵ�
                        bulletCheck.targetPosition = dir;//źȯ ������
                        bulletCheck.SetDamage(bulletCheck.GetDamage(), plusDamage);
                        bulletCheck.SetPierce(bulletCheck.GetPierce(), plusPierce);
                        bulletCheck.SetBlast(bulletCheck.GetBlast(), plusExplosive);
                        //źȯ�� ���ط��� ������� Ÿ���� ���ʽ� ���ط��� ���ʽ� ������� źȯ�� ���� ���������Ѵ�.

                        var bulletRigid = bulletCheck.GetComponent<Rigidbody2D>();
                        if (bulletRigid)
                        {
                            bulletRigid.velocity = dir * (bulletCheck.GetShotSpeed() + plusShotSpeed);//�ӵ��� ������ �������� źȯ�� ������

                        }
                    }
                }
                
                if (Vector2.Distance(transform.position, enemy[0].transform.position) > range || !target.isActiveAndEnabled)
                {//���� ���ݹ����� �Ѿ�� �ٽ� ������ Ž���Ѵ�. ������ NULL �� �ȴ�.
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
        GameManager.instance.costPlus(totalTowerCost * 0.7f);//�Ǹű��� ���ű��� 70%
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
