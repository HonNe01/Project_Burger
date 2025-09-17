using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour
{
    public enum CustomerState { Enter, Wait, Order, OrderWait, Exit }
    public CustomerState currentState = CustomerState.Enter;

    [Header("이동 관련")]
    private NavMeshAgent agent;
    [SerializeField] private Transform targetPoint;
    

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
                Check_Enter();
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
                Check_Exit();
                break;
        }
    }

    private void Check_Enter()  // OrderPoint 도착 판단
    {
        if (!agent.pathPending && agent.remainingDistance <= CustomerManager.instance.stopDistance)
        {
            // OrderPoint 도착 시 주문
            if (targetPoint == CustomerManager.instance.orderPoint)
            {
                currentState = CustomerState.Order;
            }
            else
            {
                currentState = CustomerState.Wait;
            }
        }
    }

    void EnterOrderState()      // Order 시작
    {
        agent.isStopped = true;

        // 주문 생성
        List<Ingredient> myOrder = OrderManager.instance.Order();
        Debug.Log("주문 시작 : " + string.Join(", ", myOrder));

        currentState = CustomerState.OrderWait;
        orderTimer = orderTimeLimit;
    }

    void UpdateOrderState()
    {
        orderTimer -= Time.deltaTime;

        if (orderTimer <= 0f)
        {
            Debug.Log("손님 퇴장!! (시간 초과)");
            // 감점 코드 추가

            EnterExitState();
            currentState = CustomerState.Exit;
        }
    }

    public void CompleteOrder()
    {
        Debug.Log("손님 퇴장!! (주문 완료)");
        EnterExitState();
    }

    void EnterExitState()
    {
        currentState = CustomerState.Exit;
        SetTarget(CustomerManager.instance.exitPoint);
    }

    void Check_Exit()
    {
        if (!agent.pathPending && agent.remainingDistance <= CustomerManager.instance.stopDistance)
        {
            CustomerManager.instance.RemoveCustomer(this);
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform point)
    {
        targetPoint = point;
        agent.isStopped = false;
        agent.SetDestination(point.position);
    }
}
