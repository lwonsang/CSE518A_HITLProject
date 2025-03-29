using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MomentumBar_BG : MonoBehaviour
{
    public Image redFill;
    public Image blueFill;
    [Range(-1f, 1f)] public float momentum = 0f;

    // Update is called once per frame
    void Update()
    {
        float normalizedMomentum = (momentum + 1f) / 2f; // Convert to 0 to 1 scale
        redFill.fillAmount = 1f - normalizedMomentum;
        blueFill.fillAmount = normalizedMomentum;
        //Debug.Log("Momentum: " + momentum + " | Red Fill: " + redFill.fillAmount + " | Blue Fill: " + blueFill.fillAmount);
    }
}
