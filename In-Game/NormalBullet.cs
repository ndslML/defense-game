using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//źȯ�� ����Ѵ�

enum NeedRotate
{
    NoRotate,//źȯ�� ������ �ٶ���� �ϴ°�?
    YesRotate,
}

enum BulletType
{//źȯ�� Ÿ��
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
    [SerializeField] float damage = 0;//źȯ ���ط�
    [SerializeField] int pierce = 0;//źȯ �����
    [SerializeField] int shotSpeed = 3;//źȯ �ӵ�
    [SerializeField] float blastRadius = 0;//���߹���

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
        
        if (needRotate == NeedRotate.YesRotate)//źȯ Ÿ���� ȸ�� Ÿ���̶��
        {
            Vector3 dir = targetPosition;
            // Ÿ�� �������� ȸ����
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        

        if (time > bulletLifeTime)//źȯ�� ������ �ð��� ������ ������ Ǯ �ȿ� �ִ´�
        {
            TowerManager.instance.DestroyBullet(this.gameObject, bulletID);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)//������ ��Ҵٸ� �� ���� ����.
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
