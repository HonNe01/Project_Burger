using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public Transform stackOrigin;
    public float heightStep = 0.05f;

    private List<Ingredient> stackedIngredients = new List<Ingredient>();
    private List<GameObject> stackedToppingObjects = new List<GameObject>();

    private Collider plateCollider;

    void Awake()
    {
        plateCollider = GetComponent<Collider>();
    }

    public void AddTopping(GameObject topping, Ingredient ingredientType)
    {
        topping.transform.SetParent(stackOrigin);
        Vector3 localPos = Vector3.zero;
        localPos.y = heightStep * stackedToppingObjects.Count;
        topping.transform.localPosition = localPos;
        topping.transform.localRotation = Quaternion.identity;

        Rigidbody rb = topping.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        stackedToppingObjects.Add(topping);
        stackedIngredients.Add(ingredientType);
    }

    public bool IsPositionOnPlate(Vector3 worldPos)
    {
        if (plateCollider == null) return false;
        return plateCollider.bounds.Contains(worldPos);
    }

    public bool CompareOrder(List<Ingredient> order)
    {
        if (order.Count != stackedIngredients.Count) return false;
        for (int i = 0; i < order.Count; i++)
        {
            if (order[i] != stackedIngredients[i]) return false;
        }
        return true;
    }

    public bool CompareOrderUnordered(List<Ingredient> order)
    {
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
