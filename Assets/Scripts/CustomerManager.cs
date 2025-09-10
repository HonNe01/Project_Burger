using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("손님 프리펩")]
    public List<GameObject> customerPrefabs;

    [Header("스폰 지점 (A)")]
    public Transform spawnPointA;

    [Header("소환 주기 범위")]
    public Vector2 easySapwnInterval = new Vector2(15f, 20f);
    public Vector2 hardSpawnInterval = new Vector2(10f, 12f);
    float interval;

    [Header("난이도 설정")]
    public bool hardMode = false;

    
    // Start is called before the first frame update
    void Start()
    {
        if (hardMode)
        {
            Debug.Log($"[Customer Manager] 경영 시작! 현재 모드 : Hard, 손님 간격 : {hardSpawnInterval}");
        }
        else
        {
            Debug.Log($"[Customer Manager] 경영 시작! 현재 모드 : Easy, 손님 간격 : {easySapwnInterval}");
        }

        StartCoroutine(SpawnCustomer());
    }

    private IEnumerator SpawnCustomer()
    {
        while (true)
        {
            // 현재 난이도 판단
            interval = hardMode ?
                Random.Range(hardSpawnInterval.x, hardSpawnInterval.y) :
                Random.Range(easySapwnInterval.x, easySapwnInterval.y);

            // 손님 랜덤 선택 + 소환
            if (customerPrefabs.Count > 0 && spawnPointA != null)
            {
                int index = Random.Range(0, customerPrefabs.Count);
                Instantiate(customerPrefabs[index], spawnPointA.position, spawnPointA.rotation);

                Debug.Log("[Customer Manager] 손님 생성!");
            }


            // 손님 대기
            Debug.Log($"[Customer Manager] 다음 손님까지 {interval}초");
            yield return new WaitForSeconds(interval);
        }
    }
}
