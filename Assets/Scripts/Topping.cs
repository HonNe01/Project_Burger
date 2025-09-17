using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topping : MonoBehaviour
{
    public Ingredient ingredientType;
    public GameObject ingredientPrefab;

    public GameObject GetIngredient()
    {
        return Instantiate(ingredientPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
}
