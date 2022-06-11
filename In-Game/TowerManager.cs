using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//타워와 타워가 사용하는 탄환을 담당하는 매니저
//업그레이드 창도 담당한다.

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance { get; private set; }//외부에서 값을 얻는다/값을 넣는다. 여기서는 못 넣는다

    [SerializeField] GameObject[] towerPrefab;//타워의 종류를 프리팹으로 저장한다
    //public Queue<GameObject> listCurrentSpawnTower;//현재 나와있는 타워를 순서대로 넣는다
    Dictionary<string, Queue<GameObject>> TowerPool = new Dictionary<string, Queue<GameObject>>();
    //string 을 키값으로 사용하는 큐들을 towerPool 에 담는다.
    //각 큐들을 사용해서 여러 오브젝트를 만든다?

    [SerializeField] GameObject[] bulletPrefab;
    Dictionary<string, Queue<GameObject>> bulletPool = new Dictionary<string, Queue<GameObject>>();
    //탄환 역시 풀링으로 부담을 줄인다.

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
            TowerPool.Add(towerPrefab[i].name, new Queue<GameObject>());//타워 프리펩의 이름을 기준으로 새로운 큐 배열을 타워풀에 저장한다.
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

        if (null == instance)//매니저의 중복생성을 막는다
        {
            instance = this;

            return;
        }
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))//마우스를 클릭하면 클릭한 곳을 확인한다.
        {
            ClickDetect();
        }
    }
    
    void MakeTower(GameObject prefab)//타워를 실제로 만드는 함수
    {
        //if (TowerPool.TryGetValue(towerPrefab[0].name, out Queue<Tower> value)){
        if (TowerPool.ContainsKey(prefab.name)) {//프리팹 이름이 타워풀에 있으면
            var tower = Instantiate(prefab);//그 프리팹을 생성하고
            tower.SetActive(false);
            TowerPool[prefab.name].Enqueue(tower);//타워풀[이름 키]쪽에 객체를 등록한다.
        }

        //for (int j = 0; 5 > j; j++)
        //{
        //    var tower = Instantiate(towerPrefab[i]);
        //    TowerPool[towerPrefab[i].name].Enqueue(tower);
        //}
    }

    public void AddTower(int ID,Transform spawnerPosition)//타워를 타워풀에서 꺼내옴. 꺼내올게 없으면 Make.
    {//받은 프리펩을 기준으로 타워를 추가한다
        if (TowerPool[towerPrefab[ID].name].Count <= 0)
        {
            MakeTower(towerPrefab[ID]);
        }
        var tower = TowerPool[towerPrefab[ID].name].Dequeue();
        tower.transform.position = spawnerPosition.position;
        tower.SetActive(true);
    }

    public void DestoryTower(GameObject tower,int ID)//타워를 타워풀에 넣는다.
    {//받은 ID 숫자를 기준으로 타워를 삭제한다
        tower.SetActive(false);
        TowerPool[towerPrefab[ID].name].Enqueue(tower);
    }

    void MakeBullet(GameObject prefab)//아래 Bullet 시리즈는 Bullet 의 풀링
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

    void ClickDetect()//무엇을 클릭했는가를 본다
    {
        bool onHit = false;

         RaycastHit2D isRoad = Physics2D.Raycast(GetMousePos(), transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("PlayerAttackTower"));
        RaycastHit2D isUI = Physics2D.Raycast(GetMousePos(), transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI"));
        //보는 대상은 플레이어 타워
        if (isUI.collider != null)
        {
            return;
        }
        if (isRoad.collider != null)//타워가 지정되었다면
        {
            if (selectTower != null)//선택된 타워로 갈아준다
            {
                TowerSelect();//isClick 가 true 에서 false 로 변경된다.
            }

            selectTower = isRoad.transform.gameObject.GetComponent<Tower>();
            TowerSelect();//isClick 가 false 에서 true 로 변경된다.
            //선택되었다면 isClick 는 true 상태가 되어있기 때문에 위에서 미리 변경하는것.
            onHit = true;
        }
        if (onHit == false)//맞은게 없다면
        {
            if (selectTower != null)
            {
                TowerSelect();//선택된 타워가 있다면 isClick 를 ture 에서 false 로 바꾸고
            }
            selectTower = null;//선택타워를 지운다.
        }
    }

    void TowerSelect()
    {
        if (isClicked)//선택되었다는 표시
        {
            isClicked = false;//선택을 풀었다는 표시
            UIManager.instance.RangeOff();
            UIManager.instance.UpgradeOff();//업그레이드 해제
        }
        else if(!isClicked)//선택된게 없다는 표시
        {
            isClicked = true;//선택되었다는 표시
            UIManager.instance.RangeOn(selectTower.GetTowerRange(),selectTower.transform.position);
            UIManager.instance.UpgradeOn(selectTower);//업그레이드 토글
            SoundManager.instance.ButtonSound(0);
        }
    }

    Vector3 GetMousePos()
    {//마우스 위치를 알음
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
                UIManager.instance.UpgradeOn(selectTower);//업그레이드 토글
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
                UIManager.instance.UpgradeOn(selectTower);//업그레이드 토글
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
                UIManager.instance.UpgradeOn(selectTower);//업그레이드 토글
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
                UIManager.instance.UpgradeOn(selectTower);//업그레이드 토글
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
