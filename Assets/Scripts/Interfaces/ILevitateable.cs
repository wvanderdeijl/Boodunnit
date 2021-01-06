using DefaultNamespace.Enums;

public interface ILevitateable
{
    bool CanBeLevitated { get; set; }
    bool CanRespawnWhenOutOfRange { get; set; }
    LevitationState State { get; set; }
    void Freeze();
    void Release();
    int TimesLevitated { get; set; }
}
