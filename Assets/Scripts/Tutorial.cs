using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public void StartTutorial()
    {
        GameManager.instance.isTutorial = true;
        Debug.Log("[Tutorial] 버거를 만들어볼까요?");
    }

    public void EndTutorial()
    {
        GameManager.instance.isTutorial = false;
        Debug.Log("[Tutorial] 튜토리얼 종료");
    }
}
