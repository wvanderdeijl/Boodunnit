using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldSchoolPartyHat : MonoBehaviour
{
    public Text TextAboveHat;
    public List<string> TextAboveHatText;

    public int CycleInterval;

    private void Awake()
    {
        StartCoroutine(CycleText());
    }

    IEnumerator CycleText()
    {
        while (true)
        {
            TextAboveHat.text = TextAboveHatText[Random.Range(0, TextAboveHatText.Count)];
            yield return new WaitForSeconds(CycleInterval);           
        }
    }
}
