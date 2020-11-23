using Entities;

public class PossessableSanboxBox : BaseEntity
{
    private CameraController _cameraController;

    private void Awake()
    {
        _cameraController = UnityEngine.Camera.main.GetComponent<CameraController>();
    }

    public override void UseFirstAbility()
    {
        /* TODO PossessableSanboxBox ability. I mean... tf... wtf is a PossessableSanboxBox...
         * I mean wtf it isn't even spelled correctly. PossessableSANBOXBox. Seriously?
         * Who pays for this shit?
         */
    }
}
