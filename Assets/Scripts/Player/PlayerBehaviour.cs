using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Posses behaviour
        if (Input.GetKeyDown(KeyCode.F))
        {
            print("Key F was hit");
        }

        //Scare behaviour
        if (Input.GetKeyDown(KeyCode.G))
        {
            print("Key G was hit");
        }

        //Levitate behaviour
        if (Input.GetKeyDown(KeyCode.H))
        {
            print("Key H was hit");
        }
    }
}
