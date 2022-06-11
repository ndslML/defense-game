using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//탄환을 담당한다

enum NeedRotate
{
    NoRotate,//탄환이 정면을 바라봐야 하는가?
    YesRotate,
}

enum BulletType
{//탄환의 타입
    Normal,
    Explosive,
    Piece,
}

public class NormalBullet : MonoBehaviour
{
    float time = 0;
    [SerializeField] NeedRotate needRotate;
    [SerializeField] BulletType bulletType;

    public Vector3 targetPosition = Vector3.zero;

    [SerializeField] int bulletID = 0;
    [SerializeField] GameObject effect;

    //public float shotSpeed = 3.0f;
    public float bulletLifeTime = 3.0f;
    [SerializeField] float damage = 0;//탄환 피해량
    [SerializeField] int pierce = 0;//탄환 관통력
    [SerializeField] int shotSpeed = 3;//탄환 속도
    [SerializeField] float blastRadius = 0;//폭발범위

    float trueDamage = 0;
    int truePierce = 0;
    float trueBlast = 0;

    AudioSource audio;

    private void OnEnable()
    {
        time = 0;
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, trueBlast);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        
        if (needRotate == NeedRotate.YesRotate)//탄환 타입이 회전 타입이라면
        {
            Vector3 dir = targetPosition;
            // 타겟 방향으로 회전함
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        

        if (time > bulletLifeTime)//탄환이 지정된 시간을 넘으면 강제로 풀 안에 넣는다
        {
            TowerManager.instance.DestroyBullet(this.gameObject, bulletID);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)//적에게 닿았다면 할 것을 결정.
    {
        if (collision.tag.Equals("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy && truePierce > 0)
            {
                if (bulletType == BulletType.Normal)
                {
                    SoundManager.instance.ShotBoomSoundLow(1);
                    TowerManager.instance.AddEffect(effect, transform);
                    enemy.MinusHitPoint(trueDamage);
                }
                else if (bulletType == BulletType.Explosive)
                {
                    SoundManager.instance.ShotBoomSoundLow(3);
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, trueBlast);
                    foreach(Collider2D hit in colliders)
                    {
                        var blastEnemy = hit.GetComponent<Enemy>();
                        if (blastEnemy != null)
                        {
                            var blastEffect = TowerManager.instance.AddEffect(effect, transform);
                            blastEffect.transform.localScale = new Vector3(trueBlast, trueBlast, 1);
                            blastEnemy.MinusHitPoint(trueDamage);
                        }
                    }
                }
            }
            truePierce--;

            if (truePierce <= 0)
            {
                Destroy(gameObject);
            }
        }
    }


    public float GetDamage()
    {
        return damage;
    }
    public int GetPierce()
    {
        return pierce;
    }
    public int GetShotSpeed()
    {
        return shotSpeed;
    }
    public float GetBlast()
    {
        return blastRadius;
    }
    public void SetDamage(float bulletDamage, float towerDamage)
    {
        trueDamage = bulletDamage + towerDamage;
    }
    public void SetPierce(int bulletPierce, int towerPierce)
    {
        truePierce = bulletPierce + towerPierce;
    }
    public void SetBlast(float bulletBlast, float towerBlast)
    {
        trueBlast = bulletBlast + towerBlast;
    }
}
