using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleEffect : MonoBehaviour
{
    public GameObject circle;

    private Coroutine routine;

    public static CircleEffect instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Effect(float start, float end, float time, Action OnEnd = default)
    {
        if (routine != null) StopCoroutine(routine);

        routine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            float re = 1f / time;
            for (float t = 0; t < 1; t += Time.fixedDeltaTime * re)
            {
                circle.transform.localScale = Vector3.one * Mathf.Lerp(start, end, t);
                yield return Waits.fixedWait;
            }

            circle.transform.localScale = Vector3.one * end;
            OnEnd?.Invoke();
        }

    }

    public void LoadScene(int n)
    {
        Effect(1, 0, 0.5f, () =>
        {
            SceneController.Instance.SceneLoad(n);
            Effect(0, 1, 0.5f);
        });
    }


}

