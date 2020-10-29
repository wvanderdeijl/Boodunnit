using System.Collections;
using DefaultNamespace.Enums;

public interface ILevitateable
{
    bool CanBeLevitated { get; set; }
    
    LevitationState State { get; set; }
    IEnumerator LevitateForSeconds(float seconds);
}
