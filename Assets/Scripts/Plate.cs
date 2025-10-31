using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Plate : MonoBehaviour
{
    [Header("Plate Setting")]
    [Tooltip("재료가 쌓이기 시작할 중심 위치")]
    public Transform plateCenter;
    [Tooltip("재료가 접시 위에 있는지 판별할 감지 영역")]
    public Collider plateTrigger;

    private List<GameObject> stackedToppings = new List<GameObject>();

    [Header("Order Setting")]
    [SerializeField] private Customer curCustomer;
    private readonly List<Ingredient> _tempSeq = new();
    private XRBaseInputInteractor _lastController;

    public bool IsPositionOnPlate(Vector3 position)     // 재료가 도마 위에 있는지
    {
        if (plateTrigger == null) return false;
        return plateTrigger.bounds.Contains(position);
    }

    public void AddTopping(GameObject toppingObj)       // 도마에 재료 추가, 쌓기
    {
        Topping topping = toppingObj.GetComponent<Topping>();
        if (topping == null || stackedToppings.Contains(toppingObj)) return;

        topping.CurrentPlate = this;
        stackedToppings.Add(toppingObj);
        ReStackAllToppings();
        Debug.Log($"[Plate] {topping.ingredientType} 추가됨. 현재 {stackedToppings.Count}개 쌓임.");
    }

    public void RemoveTopping(GameObject toppingObj)    // 도마 위 재료 제거
    {
        if (stackedToppings.Contains(toppingObj))
        {
            Topping topping = toppingObj.GetComponent<Topping>();
            Debug.Log($"[Plate] {topping.ingredientType} 제거됨.");
            stackedToppings.Remove(toppingObj);

            // --- 여기가 수정된 부분입니다 ---
            // 제거된 재료의 물리 효과를 즉시 다시 켜줍니다.
            Rigidbody rb = toppingObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            // 남은 재료들을 다시 정렬합니다.
            ReStackAllToppings();
        }
    }

    private void ReStackAllToppings()                   // 재료 재정렬
    {
        float currentStackHeight = 0f;

        foreach (GameObject toppingObj in stackedToppings)
        {
            Rigidbody rb = toppingObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 쌓여있는 동안에는 물리 효과를 끕니다.
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = toppingObj.GetComponent<Collider>();
            float height = col != null ? col.bounds.size.y : 0.1f;

            float pivotToBottomOffset = toppingObj.transform.position.y - col.bounds.min.y;
            Vector3 targetPos = plateCenter.position + new Vector3(0, currentStackHeight + pivotToBottomOffset, 0);

            StartCoroutine(SmoothMove(toppingObj.transform, targetPos, plateCenter.rotation, 0.2f));

            currentStackHeight += height;
        }
    }

    private IEnumerator SmoothMove(Transform obj, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // ease-out

            obj.position = Vector3.Lerp(startPos, targetPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        obj.position = targetPos;
        obj.rotation = targetRot;
    }

    public IReadOnlyList<Ingredient> GetStackSequence()     // 재료 확인
    {
        _tempSeq.Clear();

        foreach (var go in stackedToppings)
        {
            var t = go.GetComponent<Topping>();

            if (t != null)
            {
                _tempSeq.Add(t.ingredientType);
            }
        }

        return _tempSeq;
    }

    private bool IsSameOrder(IReadOnlyList<Ingredient> expected, IReadOnlyList<Ingredient> actual)  // 주문과 재료 비교
    {
        if (expected == null || actual == null) return false;
        if (expected.Count != actual.Count) return false;

        Debug.Log("[Plate] 주문: " + string.Join(",", expected));
        Debug.Log("[Plate] 제작:   " + string.Join(",", actual));

        for (int i = 0; i < expected.Count; i++)
        {
            if (expected[i] != actual[i]) return false;
        }

        return true;
    }

    public void CompleteOrder()                             // 주문 제작 완료
    {
        curCustomer = OrderManager.instance.GetCurrentCustomer();
        var expected = OrderManager.instance?.GetCurrentOrder();
        var actual = GetStackSequence();

        if (expected == null || expected.Count == 0)
        {
            Debug.LogWarning("[Plate] 현재 주문이 없습니다!");

            return;
        }

        bool ok = IsSameOrder(expected, actual);

        if (ok)
        {
            Debug.Log("[Plate] 서빙 성공!");

            SoundManager.instance?.PlayCustomerSFX(SoundManager.SFX.Success, curCustomer != null ? curCustomer.customerType : CustomerType.Default);
            
            // GameManager.instance. +100
            curCustomer?.CompleteOrder();
            SendHaptics(_lastController, 0.5f, 0.12f);
        }
        else
        {
            Debug.LogWarning("[Plate] 서빙 실패, 주문과 다릅니다!");

            SoundManager.instance?.PlayCustomerSFX(SoundManager.SFX.Fail, curCustomer != null ? curCustomer.customerType : CustomerType.Default);

            // GameManager.instance. -50
            curCustomer?.FailOrder();
            SendHaptics(_lastController, 0.1f, 0.12f);
        }

        // 도마 정리
        ClearPlate();
    }

    public void SetController(XRBaseInputInteractor interactor) // 진동할 컨트롤러 세팅
    {
        _lastController = interactor;
    }

    private void SendHaptics(XRBaseInputInteractor interactor, float amplitude, float duration) // 컨트롤러 진동
    {
        if (interactor != null)
        {
            interactor.SendHapticImpulse(amplitude, duration);
        }
    }

    public void ClearPlate()                // 도마 초기화
    {
        foreach (var go in stackedToppings)
        {
            if (go == null) continue;

            var rb = go.GetComponent<Rigidbody>();
            var coll = go.GetComponent<Collider>();

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (coll != null)
            {
                coll.enabled = true;
            }

            Destroy(go);
        }

        stackedToppings.Clear();
    }
}