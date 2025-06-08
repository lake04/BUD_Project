using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    public RawImage[] images;
    public Color selectedColor;
    public Color pressColor;
    public Color normalColor;

    public void SelectButton(int val)
    {
        images[val].color = selectedColor;
    }
    public void pressButton(int val)
    {
        images[val].color = pressColor;
    }
    public void NormalButton(int val)
    {
        images[val].color = normalColor;
    }
}
