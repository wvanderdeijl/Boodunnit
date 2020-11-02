using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;
    public HighlightBehaviour HighlightBehaviour;
    public DialogueManager DialogueManager;

    [Header("Player Interaction Radius")]
    public Transform InteractPoint;
    public float InteractRadius;

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

        //Dialogue behaviour
        if (Input.GetKey(KeyCode.F))
        {
            if (!DialogueManager.hasDialogueStarted)
            {
                DialogueManager.TriggerDialogue(InteractPoint, InteractRadius);
            }
        }

        //Dash behaviour
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!DashBehaviour.IsDashing && !DashBehaviour.DashOnCooldown)
            {
                DashBehaviour.Dash();
            }
        }

        //Levitate behaviour
        if (Input.GetMouseButtonDown(0))
        {
            print("Key H was hit");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(InteractPoint.position, InteractRadius);
    }
}
