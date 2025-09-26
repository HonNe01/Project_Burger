using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [Tooltip("재료가 쌓이기 시작할 중심 위치")]
    public Transform plateCenter;
    [Tooltip("재료가 접시 위에 있는지 판별할 감지 영역")]
    public Collider plateTrigger;

    private List<GameObject> stackedToppings = new List<GameObject>();

    public bool IsPositionOnPlate(Vector3 position)
    {
        if (plateTrigger == null) return false;
        return plateTrigger.bounds.Contains(position);
    }

    public void AddTopping(GameObject toppingObj)
    {
        Topping topping = toppingObj.GetComponent<Topping>();
        if (topping == null || stackedToppings.Contains(toppingObj)) return;

        topping.CurrentPlate = this;
        stackedToppings.Add(toppingObj);
        ReStackAllToppings();
        Debug.Log($"[Plate] {topping.ingredientType} 추가됨. 현재 {stackedToppings.Count}개 쌓임.");
    }

    public void RemoveTopping(GameObject toppingObj)
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

    private void ReStackAllToppings()
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
}

