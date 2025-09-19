using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [Header("Plate Setting")]
    public Transform stackOrigin;   // 기준점
    public float heightStep = 0.05f;    // 토핑 높이 증가

    [Header("Order List")]
    private List<Ingredient> stackedIngredients = new List<Ingredient>();   // 도마
    private List<GameObject> stackedToppingObjects = new List<GameObject>();// 주문

    private Collider plateCollider;

    void Awake()
    {
        plateCollider = GetComponent<Collider>();
    }

    public void AddTopping(GameObject topping, Ingredient ingredientType)
    {
        // 부모 변경
        topping.transform.SetParent(stackOrigin);

        // 다음 층 계산
        Vector3 localPos = Vector3.zero;
        localPos.y = heightStep * stackedToppingObjects.Count;

        // 재료 배치
        topping.transform.localPosition = localPos;
        topping.transform.localRotation = Quaternion.identity;

        // 물리 정리 : 도마에 올린 뒤엔 움직이지 않게
        Rigidbody rb = topping.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 리스트에 기록
        stackedToppingObjects.Add(topping);
        stackedIngredients.Add(ingredientType);
    }

    public bool IsPositionOnPlate(Vector3 worldPos)
    {
        // 도마에 닿았는지 판단
        if (plateCollider == null) return false;
        return plateCollider.bounds.Contains(worldPos);
    }

    public bool CompareOrder(List<Ingredient> order)
    {
        // 주문이랑 동일한지 판단 (순서상관 O)
        if (order.Count != stackedIngredients.Count) return false;
        for (int i = 0; i < order.Count; i++)
        {
            if (order[i] != stackedIngredients[i]) return false;
        }
        return true;
    }

    public bool CompareOrderUnordered(List<Ingredient> order)
    {
        // 주문이랑 동일한지 판단 (순서상관 X)
        if (order.Count != stackedIngredients.Count) return false;

        var dict = new Dictionary<Ingredient, int>();
        foreach (var ing in stackedIngredients)
        {
            if (!dict.ContainsKey(ing)) dict[ing] = 0;
            dict[ing]++;
        }
        foreach (var ing in order)
        {
            if (!dict.ContainsKey(ing)) return false;
            dict[ing]--;
            if (dict[ing] < 0) return false;
        }
        return true;
    }
}
