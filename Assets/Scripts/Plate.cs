using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [Tooltip("재료가 쌓이기 시작할 중심 위치")]
    public Transform plateCenter;

    [Tooltip("재료가 접시 위에 있는지 판별할 감지 영역")]
    public Collider plateTrigger;

    private readonly List<GameObject> stackedToppings = new List<GameObject>();

    // 접시 위에 있는 위치인지 확인
    public bool IsPositionOnPlate(Vector3 position)
    {
        if (plateTrigger == null) return false;
        return plateTrigger.bounds.Contains(position);
    }

    // 재료 추가
    public void AddTopping(GameObject toppingObj)
    {
        if (toppingObj == null || stackedToppings.Contains(toppingObj)) return;

        // Topping 또는 Patty 스크립트를 모두 인식 가능하게 처리
        var topping = toppingObj.GetComponent<Topping>();
        var patty = toppingObj.GetComponent<Patty>();

        if (topping != null) topping.CurrentPlate = this;
        if (patty != null) patty.CurrentPlate = this;

        stackedToppings.Add(toppingObj);
        ReStackAllToppings();

        Debug.Log($"[Plate] {toppingObj.name} 추가됨. 현재 {stackedToppings.Count}개 쌓임.");
    }

    // 재료 제거
    public void RemoveTopping(GameObject toppingObj)
    {
        if (stackedToppings.Contains(toppingObj))
        {
            stackedToppings.Remove(toppingObj);

            Rigidbody rb = toppingObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            ReStackAllToppings();

            Debug.Log($"[Plate] {toppingObj.name} 제거됨. 현재 {stackedToppings.Count}개 남음.");
        }
    }

    // 전체 재정렬
    private void ReStackAllToppings()
    {
        float currentStackHeight = 0f;

        foreach (GameObject toppingObj in stackedToppings)
        {
            if (toppingObj == null) continue;

            Rigidbody rb = toppingObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = toppingObj.GetComponent<Collider>();
            if (col == null) continue;

            float height = col.bounds.size.y;
            float pivotOffset = toppingObj.transform.position.y - col.bounds.min.y;
            Vector3 targetPos = plateCenter.position + new Vector3(0, currentStackHeight + pivotOffset, 0);

            StartCoroutine(SmoothMove(toppingObj.transform, targetPos, plateCenter.rotation, 0.15f));

            currentStackHeight += height;
        }
    }

    // 부드러운 이동 (정렬 애니메이션)
    private IEnumerator SmoothMove(Transform obj, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t); // smoothstep

            obj.position = Vector3.Lerp(startPos, targetPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        obj.position = targetPos;
        obj.rotation = targetRot;
    }
}
