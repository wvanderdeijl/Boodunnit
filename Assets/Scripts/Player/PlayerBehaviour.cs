using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public PossessionBehaviour PossessionBehaviour;
    public HighlightBehaviour HighlightBehaviour;

    // Update is called once per frame
    void Update()
    {
        HighlightBehaviour.HighlightGameobjectsInRadius();

        //Posses behaviour
        if (Input.GetKey(KeyCode.E))
        {
            if (PossessionBehaviour.IsPossessing)
            {
                PossessionBehaviour.LeavePossessedTarget();
            } 
            else
            {
                PossessionBehaviour.PossessTarget();
            }
        }

        //Dash behaviour
        if (Input.GetKey(KeyCode.LeftShift))
        {
            print("Key LeftShift was hit");
        }

        //Levitate behaviour
        if (Input.GetMouseButtonDown(0))
        {
            print("Key H was hit");
        }
    }
}
