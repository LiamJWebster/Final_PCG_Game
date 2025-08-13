using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bar_Slider : MonoBehaviour
{
    public Slider slider;
    public Image Bar;
    public TMP_Text ValueText;

    public Gradient gradient;
    

    public int maxvalue;
    public int value;

    private void Awake()
    {
        
    }
    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        slider.value = value;

        Bar.color = gradient.Evaluate(1f);

        ValueText.text = (value + "/" + maxvalue);
    }

    public void SetCurrentValue(int value)
    {
        slider.value = value;

        Bar.color= gradient.Evaluate(slider.normalizedValue);

        ValueText.text = (slider.value + "/" + maxvalue);
    }
}
