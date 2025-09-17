using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager instance;

    [Header("Customer Setting")]
    public List<GameObject> customerPrefabs;

    [Header("스폰 지점 (A)")]
    public Transform spawnPoint;

    [Header("주문 지점 (B)")]
    public Transform orderPoint;

    [Header("퇴장 지점 (C)")]
    public Transform exitPoint;

    [Header("소환 주기")]
    public Vector2 easySapwnInterval = new Vector2(15f, 20f);
    public Vector2 hardSpawnInterval = new Vector2(10f, 12f);
    float interval;

    [Header("난이도 설정")]
    public bool hardMode = false;

    [Header("Waiting Setting")]
    public float queueSpacing = 1f;
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
            if (customerPrefabs.Count > 0 && spawnPoint != null)
            {
                int index = Random.Range(0, customerPrefabs.Count);
                GameObject newCustomerObj = Instantiate(customerPrefabs[index], spawnPoint.position, spawnPoint.rotation);
                Customer newCustomer = newCustomerObj.GetComponent<Customer>();

                customerQueue.Add(newCustomer);

                UpdateQueue();

                Debug.Log("[Customer Manager] 손님 생성!");
            }


            // 손님 대기
            Debug.Log($"[Customer Manager] 다음 손님까지 {interval}초");
            yield return new WaitForSeconds(interval);
        }
    }

    private void UpdateQueue()
    {
        for (int i = 0; i < customerQueue.Count; i++)
        {
            Customer cust = customerQueue[i];

            if (i == 0)
            {
                // 맨 앞 손님은 카운터로
                cust.currentState = Customer.CustomerState.Enter;
            }
            else
            {
                // 뒷 손님 줄 세우기
                Vector3 waitPos = orderPoint.position - orderPoint.forward * (queueSpacing * i);

                GameObject temp = new GameObject("QueuePos");
                temp.transform.position = waitPos;
                cust.SetTarget(temp.transform);

                Destroy(temp);
            }
        }
    }

    public void RemoveCustomer(Customer cust)
    {
        if (customerQueue.Contains(cust))
        {
            customerQueue.Remove(cust);
            UpdateQueue();
        }
    }
}
