using System.Collections.Generic;
using UnityEngine;
using PoolManagerModule;

namespace Gameplay
{
    public class BonusSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> bonusList;
        [SerializeField]
        [Range(0f, 10f)]
        private float spawnBonusDelay = 6.5f;
        private float nextBonusSpawnTime = 0;
        [SerializeField]
        [Range(0f, 10f)]
        private float spawnFoodDelay = 3.5f;
        private float nextFoodSpawnTime = 0;
        private List<Vector3> freeCordsList = new List<Vector3>();
        private List<GameObject> bonusFilledCords = new List<GameObject>();

        private void OnDisable()
        {
            freeCordsList.Clear();
            bonusFilledCords.Clear();
        }

        private void Update()
        {
            if (Time.time > nextBonusSpawnTime)
                SpawnBonus(false);
            if (Time.time > nextFoodSpawnTime)
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

        private void InstantiateBonus(bool _food)
        {
            if (bonusList.Count > 1 && freeCordsList.Count > 0)
            {
                int randBonus;
                if (!_food)
                {
                    randBonus = Random.Range(1, bonusList.Count);
                    nextBonusSpawnTime = Time.time + spawnBonusDelay;
                }
                else
                {
                    randBonus = 0;
                    nextFoodSpawnTime = Time.time + spawnFoodDelay;
                }
                int randCord = Random.Range(0, freeCordsList.Count);
                Transform bonus = PoolManager.Instance.Spawn(bonusList[randBonus].transform, freeCordsList[randCord], Quaternion.identity);
                bonusFilledCords.Add(bonus.gameObject);

            }
        }
        private void RemoveCollectedBonus(GameObject _collectedBonusPos)
        {
            bonusFilledCords.Remove(_collectedBonusPos);
        }
        public bool CheckBonusList(Vector2 nextSnakePosition)
        {
            foreach (GameObject gam in bonusFilledCords)
            {
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
}
