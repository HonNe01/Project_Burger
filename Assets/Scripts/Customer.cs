using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Customer : MonoBehaviour
{
    public enum CustomerState { Enter, Wait, Order, OrderWait, Exit }
    public CustomerState currentState = CustomerState.Enter;

    [Header("이동 관련")]
    private NavMeshAgent agent;
    public Transform targetPoint;
    public float stopDistance = 0.1f;

    [Header("주문 관련")]
    public float orderTimeLimit = 20f;
    private float orderTimer;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (targetPoint == null)
        {
            GameObject counter = GameObject.FindGameObjectWithTag("CounterPoint");

            if (counter != null)
            {
                targetPoint = counter.transform;
            }
        }

        SetTarget(targetPoint);
    }

    void Update()
    {
        switch (currentState)
        {
            case CustomerState.Enter:
                CheckArrival_Enter();

                break;
            case CustomerState.Wait:
                // 주문 대기열 상태에 따라 변경
                break;
            case CustomerState.Order:
                EnterOrderState();
                break;
            case CustomerState.OrderWait:
                UpdateOrderState();
                break;
            case CustomerState.Exit:
                CheckArrival_Exit();
                break;
        }
    }

    void CheckArrival_Enter()
    {
        if (!agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            // 대기열에 따라 Order, Wait 결정
            if (targetPoint.CompareTag("CounterPoint"))
            {
                currentState = CustomerState.Order;
            }
            else
            {
                currentState = CustomerState.Wait;
            }
        }
    }

    void EnterOrderState()
    {
        agent.isStopped = true;
        orderTimer = orderTimeLimit;

        List<Ingredient> myOrder = OrderManager.instance.Order();

        // 손님 주문 데이터 생성 코드 추가 필요
        Debug.Log("주문 시작 : " + string.Join(", ", myOrder));
        currentState = CustomerState.OrderWait;
    }

    void UpdateOrderState()
    {
        orderTimer -= Time.deltaTime;

        if (orderTimer <= 0f)
        {
            Debug.Log("손님 퇴장!!");
            // 감점 코드 추가
            
            EnterExitState();
            currentState = CustomerState.Exit;
        }
    }

    void EnterExitState()
    {
        SetTarget(CustomerManager.instance.exitPoint);
    }

    void CheckArrival_Exit()
    {
        if (!agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform point)
    {
        targetPoint = point;
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (agent != null && targetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPoint.position);
        }
    }
}
