using UnityEngine;

public class Test : MonoBehaviour, IEntity
{
    public Dialogue M_dialogue;
    public Dialogue Dialogue
    {
        get
        {
            return M_dialogue;
        }
    }
}
