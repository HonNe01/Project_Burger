using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance;

    [Header("Customer Setting")]
    public List<GameObject> customerPrefabs;
    public float stopDistance = 0.1f;


    [Header("Spawn Point (A)")]
    public Transform spawnPoint;

    [Header("Order Point (B)")]
    public Transform orderPoint;

    [Header("Exit Point (C)")]
    public Transform exitPoint;


    [Header("Spawn Interval")]
    public Vector2 easySpawnInterval = new Vector2(15f, 20f);
    public Vector2 hardSpawnInterval = new Vector2(10f, 12f);
    float interval;


    [Header("Level Setting")]
    public bool hardMode = false;


    [Header("Waiting Setting")]
    [Tooltip("손님 기다리는 줄 간격")]
    public float queueSpacing = 3f;
    private List<Customer> customerQueue = new List<Customer>();


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (hardMode)
        {
            Debug.Log($"[Customer Manager] 경영 시작! 현재 모드 : Hard, 손님 간격 : {hardSpawnInterval}");
        }
        else
        {
            Debug.Log($"[Customer Manager] 경영 시작! 현재 모드 : Easy, 손님 간격 : {easySpawnInterval}");
        }

        customerQueue.Clear();
        StartCoroutine(Co_SpawnCustomer());
    }

    private IEnumerator Co_SpawnCustomer()
    {
        while (true)
        {
            // 현재 난이도 판단
            interval = hardMode ?
                Random.Range(hardSpawnInterval.x, hardSpawnInterval.y) :
                Random.Range(easySpawnInterval.x, easySpawnInterval.y);

            // 손님 랜덤 선택 + 소환
            SpawnCustomer();
            Debug.Log("[Customer Manager] 손님 생성!");

            // 손님 대기
            Debug.Log($"[Customer Manager] 다음 손님까지 {interval}초");
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefabs.Count == 0) return;

        int index = Random.Range(0, customerPrefabs.Count);
        GameObject obj = Instantiate(customerPrefabs[index], spawnPoint.position, spawnPoint.rotation);
        Customer cust = obj.GetComponent<Customer>();

        customerQueue.Add(cust);
        UpdateQueue();
    }

    private void UpdateQueue()
    {
        for (int i = 0; i < customerQueue.Count; i++)
        {
            Customer cust = customerQueue[i];

            if (i == 0)
            {
                // 맨 앞 손님은 카운터로
                cust.SetTarget(orderPoint);

                if (cust.currentState == Customer.CustomerState.Enter ||
                    cust.currentState == Customer.CustomerState.Wait)
                    cust.currentState = Customer.CustomerState.Enter;
            }
            else
            {
                // 뒷 손님 줄 세우기
                Vector3 waitPos = orderPoint.position + orderPoint.right * (queueSpacing * i);
                GameObject temp = new GameObject("QueuePos");
                temp.transform.position = waitPos;
                cust.SetTarget(temp.transform);
                Destroy(temp);
            }
        }
    }

    public void RemoveCustomer(Customer cust)   // 손님 퇴장 -> 큐에서 제거 -> 큐 재정렬
    {
        if (customerQueue.Contains(cust))
        {
            customerQueue.Remove(cust);
            UpdateQueue();
        }
    }
}
