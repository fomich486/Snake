using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _bonusList;
    [SerializeField]
    [Range(0f, 10f)]
    private float _spawnBonusDelay = 6.5f;
    private float _nextBonusSpawnTime = 0;
    [SerializeField]
    [Range(0f, 10f)]
    private float _spawnFoodDelay = 3.5f;
    private float _nextFoodSpawnTime = 0;
    private List<Vector3> freeCordsList = new List<Vector3>();
    private List<GameObject> bonusFilledCords = new List<GameObject>();

    private void Update()
    {
        if(Time.time > _nextBonusSpawnTime)
            SpawnBonus(false);
        if (Time.time > _nextFoodSpawnTime)
            SpawnBonus(true);
    }

    private void SpawnBonus(bool food)
    {
        FillCordList();
        //TODO :Check for max length of snake and not null list of bonuses
        //and instantiate if any free space
        InstantiateBonus(food);
        
    }
    private void FillCordList()
    {
        freeCordsList.Clear();
        for (int i = -Settings.Instance.X_Border; i <= Settings.Instance.X_Border; i++)
            for (int j = -Settings.Instance.Y_Border; j <= Settings.Instance.Y_Border; j++)
            {
                Vector3 tempVectr = new Vector3(i, j, 0f);
                freeCordsList.Add(tempVectr);
            }
        foreach (Transform transform in Settings.Instance.Snake.SnakeList)
        {
            freeCordsList.Remove(transform.position);
        }
        foreach (GameObject gam in bonusFilledCords)
        {
            freeCordsList.Remove(gam.transform.position);
        }
    }
    private void InstantiateBonus(bool food)
    {
        if (_bonusList.Count > 1 && freeCordsList.Count > 0)
        {
            int randBonus;
            if (!food)
            {
                randBonus = Random.Range(1, _bonusList.Count);
                _nextBonusSpawnTime = Time.time + _spawnBonusDelay;
            }
            else
            {
                randBonus = 0;
                _nextFoodSpawnTime = Time.time + _spawnFoodDelay;
            }
            int randCord = Random.Range(0, freeCordsList.Count);
            GameObject bonus = Instantiate(_bonusList[randBonus], freeCordsList[randCord], Quaternion.identity);
            bonusFilledCords.Add(bonus);
            
        }
    }
    private void RemoveCollectedBonus(GameObject collectedBonusPos)
    {
        bonusFilledCords.Remove(collectedBonusPos);
    }
    public bool CheckBonusList(Vector2 nextSnakePosition)
    {
        foreach (GameObject gam in bonusFilledCords) {
            if (gam.transform.position == (Vector3)nextSnakePosition)
            {
                gam.GetComponent<Bonus>().OnBonusEnter();
                RemoveCollectedBonus(gam);
                return true;
            }
        }
        return false;
    }
}
