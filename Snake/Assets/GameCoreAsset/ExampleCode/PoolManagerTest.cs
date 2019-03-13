using PoolManagerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManagerTest : MonoBehaviour
{
    public Transform SpherePrefab;
    public Transform CubePrefab;

    private void Start()
    {
        StartCoroutine(AnimatePoolCubes());
        StartCoroutine(AnimatePoolSpheres());
    } 
    
    IEnumerator AnimatePoolCubes()
    {
        int _counter = 0;
        int _maxCounter = 10;
        List<Transform> _curList = new List<Transform>();
        while (true)
        {
            _counter = 0;
            _curList = new List<Transform>();
             
            for(int i = 0; i < _maxCounter;i++)
            {
                Transform _new = PoolManager.Instance.Spawn(CubePrefab);
                _new.position = new Vector3(-8f + 1.5f * i, 0f, 0f);
                _curList.Add(_new);
                yield return new WaitForSeconds(0.2f);
            }

            for(int i = 0; i < _maxCounter; i++)
            {
                PoolManager.Instance.Despawn(_curList[_maxCounter - i - 1]);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }  

    IEnumerator AnimatePoolSpheres()
    {
        while (true)
        {
            int _counter = 0;
            //ATTENTION! HARDCODE. DONT DO LIKE ME
            int _maxCounter = 150;
            int _rowSize = (int)Mathf.Sqrt(_maxCounter);
            float _offset = 16f / _rowSize;
            
            List<Transform> _curList = new List<Transform>();
            while (true)
            {
                _counter = 0;
                _curList = new List<Transform>();

                for (int j = 0; j < _rowSize; j++)
                {
                    for (int i = 0; i < _rowSize; i++)
                    {
                        Transform _new = PoolManager.Instance.Spawn(SpherePrefab);
                        _new.position = new Vector3(-8f + _offset * i, 2f, _offset * j);
                        _curList.Add(_new);
                        yield return new WaitForEndOfFrame();
                    }
                }

                int _count = _curList.Count;
                for (int i = 0; i < _count; i++)
                {
                    PoolManager.Instance.Despawn(_curList[_count - i - 1]);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}
