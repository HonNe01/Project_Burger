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
    
    void Update()
    {
        switch (currentState)
        {
            case CustomerState.Enter:
                EnterOrderPoint();
                break;
            case CustomerState.Wait:
                
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

    public void EnterOrderPoint()
    {
        SetTarget(CustomerManager.instance.orderPoint);
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
