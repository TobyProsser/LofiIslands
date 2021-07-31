using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFadeInController : MonoBehaviour
{
    List<GameObject> children = new List<GameObject>();

    void Start()
    {
        AddChildren();

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        
    }


    IEnumerator FadeIn()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(true);
            yield return new WaitForSeconds(.2f);
        }
        yield return null;
    }
    void AddChildren()
    {
        foreach (Transform child in this.transform)
        {
            children.Add(child.gameObject);
        }
    }
}
