using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTestScript : MonoBehaviour
{
    public int number;
    void Start()
    {
        SaveNumber();
        int value = SaveHandler.Instance.GetPropertyValueFromUniqueKey<int>(transform.name, "number");
        print(value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void SaveNumber()
    {
        print(name);
        SaveHandler.Instance.SaveGameProperty(name, "number", number);
    }
}
