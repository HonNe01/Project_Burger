using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public Transform plateCenter;        // 중앙 정렬 기준
    public Collider plateTrigger;        // 올려졌는지 판정용 Trigger

    private List<Ingredient> stackedIngredients = new List<Ingredient>();
    private float stackHeight = 0f;      // 현재까지 쌓인 높이

    public bool IsPositionOnPlate(Vector3 position)
    {
        return plateTrigger.bounds.Contains(position);
    }

    public void AddTopping(GameObject toppingObj, Ingredient ingredientType)
    {
        // 쌓을 높이 계산
        Collider col = toppingObj.GetComponent<Collider>();
        float height = col != null ? col.bounds.size.y : 0.2f;

        // 목표 위치 계산
        Vector3 targetPos = plateCenter.position + new Vector3(0, stackHeight + height / 2f, 0);

        // Rigidbody 제어 (중력 끄기)
        Rigidbody rb = toppingObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 부드럽게 이동 시작
        StartCoroutine(SmoothMove(toppingObj.transform, targetPos, plateCenter.rotation, 0.3f));

        // 리스트에 저장
        stackedIngredients.Add(ingredientType);

        // 높이 갱신
        stackHeight += height;

        Debug.Log($"[Plate] {ingredientType} 추가됨. 현재 {stackedIngredients.Count}개 쌓임");
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

            // 부드러운 곡선 (ease out)
            t = t * t * (3f - 2f * t);

            obj.position = Vector3.Lerp(startPos, targetPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        // 최종 위치 고정
        obj.position = targetPos;
        obj.rotation = targetRot;
    }
}
