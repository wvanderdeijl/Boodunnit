using System.Collections;

public interface ILevitateable
{
    bool CanBeLevitated { get; set; }
    LevitationState State { get; set; }
    IEnumerator LevitateForSeconds(float seconds);
}
