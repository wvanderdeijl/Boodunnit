using System.Collections;
using DefaultNamespace.Enums;

public interface ILevitateable
{
    bool CanBeLevitated { get; set; }
    bool CanRespawnWhenOutOfRange { get; set; }
    LevitationState State { get; set; }
    IEnumerator LevitateForSeconds(float seconds);
}
