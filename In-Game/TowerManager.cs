using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ÿ���� Ÿ���� ����ϴ� źȯ�� ����ϴ� �Ŵ���
//���׷��̵� â�� ����Ѵ�.

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance { get; private set; }//�ܺο��� ���� ��´�/���� �ִ´�. ���⼭�� �� �ִ´�

    [SerializeField] GameObject[] towerPrefab;//Ÿ���� ������ ���������� �����Ѵ�
    //public Queue<GameObject> listCurrentSpawnTower;//���� �����ִ� Ÿ���� ������� �ִ´�
    Dictionary<string, Queue<GameObject>> TowerPool = new Dictionary<string, Queue<GameObject>>();
    //string �� Ű������ ����ϴ� ť���� towerPool �� ��´�.
    //�� ť���� ����ؼ� ���� ������Ʈ�� �����?

    [SerializeField] GameObject[] bulletPrefab;
    Dictionary<string, Queue<GameObject>> bulletPool = new Dictionary<string, Queue<GameObject>>();
    //źȯ ���� Ǯ������ �δ��� ���δ�.

    [SerializeField] GameObject[] effectPrefab;
    Dictionary<string, Queue<GameObject>> effectPool = new Dictionary<string, Queue<GameObject>>();

    public int towerPrefabNumber = 5;
    public int bulletPrefabNumber = 30;
    public int effectPrefabNumber = 1000;

    [SerializeField] Tower selectTower = null;
    bool isClicked = false;

    // Start is called before the first frame update
    void Awake()
    {
        var towerKind = towerPrefab.Length;
        for(int i = 0; i < towerKind; i++)
        {
            TowerPool.Add(towerPrefab[i].name, new Queue<GameObject>());//Ÿ�� �������� �̸��� �������� ���ο� ť �迭�� Ÿ��Ǯ�� �����Ѵ�.
            for (int j = 0; towerPrefabNumber > j; j++)
            {
                MakeTower(towerPrefab[i]);
            }
        }

        var bulletKind = bulletPrefab.Length;
        for(int i = 0; i < bulletKind; i++)
        {
            bulletPool.Add(bulletPrefab[i].name, new Queue<GameObject>());
            for(int j = 0; j < bulletPrefabNumber; j++)
            {
                MakeBullet(bulletPrefab[i]);
            }
        }

        var effectKind = effectPrefab.Length;
        for(int i = 0; i < effectKind; i++)
        {
            effectPool.Add(effectPrefab[i].name, new Queue<GameObject>());
            for(int j = 0; j < effectPrefabNumber; j++)
            {
                MakeEffect(effectPrefab[i]);
            }
        }

        if (null == instance)//�Ŵ����� �ߺ������� ���´�
        {
            instance = this;

            return;
        }
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))//���콺�� Ŭ���ϸ� Ŭ���� ���� Ȯ���Ѵ�.
        {
            ClickDetect();
        }
    }
    
    void MakeTower(GameObject prefab)//Ÿ���� ������ ����� �Լ�
    {
        //if (TowerPool.TryGetValue(towerPrefab[0].name, out Queue<Tower> value)){
        if (TowerPool.ContainsKey(prefab.name)) {//������ �̸��� Ÿ��Ǯ�� ������
            var tower = Instantiate(prefab);//�� �������� �����ϰ�
            tower.SetActive(false);
            TowerPool[prefab.name].Enqueue(tower);//Ÿ��Ǯ[�̸� Ű]�ʿ� ��ü�� ����Ѵ�.
        }

        //for (int j = 0; 5 > j; j++)
        //{
        //    var tower = Instantiate(towerPrefab[i]);
        //    TowerPool[towerPrefab[i].name].Enqueue(tower);
        //}
    }

    public void AddTower(int ID,Transform spawnerPosition)//Ÿ���� Ÿ��Ǯ���� ������. �����ð� ������ Make.
    {//���� �������� �������� Ÿ���� �߰��Ѵ�
        if (TowerPool[towerPrefab[ID].name].Count <= 0)
        {
            MakeTower(towerPrefab[ID]);
        }
        var tower = TowerPool[towerPrefab[ID].name].Dequeue();
        tower.transform.position = spawnerPosition.position;
        tower.SetActive(true);
    }

    public void DestoryTower(GameObject tower,int ID)//Ÿ���� Ÿ��Ǯ�� �ִ´�.
    {//���� ID ���ڸ� �������� Ÿ���� �����Ѵ�
        tower.SetActive(false);
        TowerPool[towerPrefab[ID].name].Enqueue(tower);
    }

    void MakeBullet(GameObject prefab)//�Ʒ� Bullet �ø���� Bullet �� Ǯ��
    {
        if (bulletPool.ContainsKey(prefab.name))
        {
            var bullet = Instantiate(prefab);
            bullet.SetActive(false);
            bulletPool[prefab.name].Enqueue(bullet);
        }
    }

    public GameObject AddBullet(GameObject getBullet,Transform spawnBullet)
    {
        if (bulletPool[getBullet.name].Count <= 0)
        {
            MakeBullet(getBullet);
        }
        var bullet = bulletPool[getBullet.name].Dequeue();
        bullet.transform.position = spawnBullet.position;
        bullet.SetActive(true);

        return bullet;
    }

    public void DestroyBullet(GameObject bullet,int id)
    {
        bullet.SetActive(false);
        bulletPool[bulletPrefab[id].name].Enqueue(bullet);
    }

    void MakeEffect(GameObject prefab)
    {
        if (effectPool.ContainsKey(prefab.name))
        {
            var effect = Instantiate(prefab);
            effect.SetActive(false);
            effectPool[prefab.name].Enqueue(effect);
        }
    }

    public GameObject AddEffect(GameObject GetEffect,Transform spawnEffect)
    {
        if (effectPool[GetEffect.name].Count <= 0)
        {
            MakeEffect(GetEffect);
        }
        var effect = effectPool[GetEffect.name].Dequeue();
        effect.transform.position = spawnEffect.position;
        effect.SetActive(true);

        return effect;
    }

    public void DestroyEffect(GameObject effect, int ID)
    {
        effect.SetActive(false);
        effectPool[effectPrefab[ID].name].Enqueue(effect);
    }

    void ClickDetect()//������ Ŭ���ߴ°��� ����
    {
        bool onHit = false;

         RaycastHit2D isRoad = Physics2D.Raycast(GetMousePos(), transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("PlayerAttackTower"));
        RaycastHit2D isUI = Physics2D.Raycast(GetMousePos(), transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI"));
        //���� ����� �÷��̾� Ÿ��
        if (isUI.collider != null)
        {
            return;
        }
        if (isRoad.collider != null)//Ÿ���� �����Ǿ��ٸ�
        {
            if (selectTower != null)//���õ� Ÿ���� �����ش�
            {
                TowerSelect();//isClick �� true ���� false �� ����ȴ�.
            }

            selectTower = isRoad.transform.gameObject.GetComponent<Tower>();
            TowerSelect();//isClick �� false ���� true �� ����ȴ�.
            //���õǾ��ٸ� isClick �� true ���°� �Ǿ��ֱ� ������ ������ �̸� �����ϴ°�.
            onHit = true;
        }
        if (onHit == false)//������ ���ٸ�
        {
            if (selectTower != null)
            {
                TowerSelect();//���õ� Ÿ���� �ִٸ� isClick �� ture ���� false �� �ٲٰ�
            }
            selectTower = null;//����Ÿ���� �����.
        }
    }

    void TowerSelect()
    {
        if (isClicked)//���õǾ��ٴ� ǥ��
        {
            isClicked = false;//������ Ǯ���ٴ� ǥ��
            UIManager.instance.RangeOff();
            UIManager.instance.UpgradeOff();//���׷��̵� ����
        }
        else if(!isClicked)//���õȰ� ���ٴ� ǥ��
        {
            isClicked = true;//���õǾ��ٴ� ǥ��
            UIManager.instance.RangeOn(selectTower.GetTowerRange(),selectTower.transform.position);
            UIManager.instance.UpgradeOn(selectTower);//���׷��̵� ���
            SoundManager.instance.ButtonSound(0);
        }
    }

    Vector3 GetMousePos()
    {//���콺 ��ġ�� ����
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    public Tower GetTower(int i)
    {
        Tower tower = towerPrefab[i].GetComponent<Tower>();
        return tower;
    }

    public void PierceUpgrade()
    {
        if (selectTower != null)
        {
            if (selectTower.GetPierceCost <= GameManager.instance.getCost())
            {
                GameManager.instance.costMinus(selectTower.GetPierceCost);
                selectTower.totalTowerCostPlus(selectTower.GetPierceCost);
                selectTower.LevelUPPierceLevel();
                selectTower.UpgradeMath();

                UIManager.instance.RangeOn(selectTower.GetTowerRange(), selectTower.transform.position);
                UIManager.instance.UpgradeOn(selectTower);//���׷��̵� ���
            }
        }
    }
    public void RangeUpgrade()
    {
        if (selectTower != null)
        {
            if (selectTower.GetRangeCost <= GameManager.instance.getCost())
            {
                GameManager.instance.costMinus(selectTower.GetRangeCost);
                selectTower.totalTowerCostPlus(selectTower.GetRangeCost);
                selectTower.LevelUPRangeLevel();
                selectTower.UpgradeMath();

                UIManager.instance.RangeOn(selectTower.GetTowerRange(), selectTower.transform.position);
                UIManager.instance.UpgradeOn(selectTower);//���׷��̵� ���
            }
        }
    }
    public void DamageUpgrade()
    {
        if (selectTower != null)
        {
            if (selectTower.GetdamageCost <= GameManager.instance.getCost())
            {
                GameManager.instance.costMinus(selectTower.GetdamageCost);
                selectTower.totalTowerCostPlus(selectTower.GetdamageCost);
                selectTower.LevelUPDamageLevel();
                selectTower.UpgradeMath();

                UIManager.instance.RangeOn(selectTower.GetTowerRange(), selectTower.transform.position);
                UIManager.instance.UpgradeOn(selectTower);//���׷��̵� ���
            }
        }
    }
    public void SPUpgrade()
    {
        if (selectTower != null)
        {
            if (selectTower.GetSPCost <= GameManager.instance.getCost())
            {
                GameManager.instance.costMinus(selectTower.GetSPCost);
                selectTower.totalTowerCostPlus(selectTower.GetSPCost);
                selectTower.LevelUPSPLevel();
                selectTower.UpgradeMath();

                UIManager.instance.RangeOn(selectTower.GetTowerRange(), selectTower.transform.position);
                UIManager.instance.UpgradeOn(selectTower);//���׷��̵� ���
            }
        }
    }

    public void TowerSell()
    {
        if (selectTower != null)
        {
            selectTower.TowerSell();
        }
    }
}
