using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : BaseDataContainer
{
    public float PlayerPositionX, PlayerPositionY, PlayerPositionZ;
    public float PlayerRotationX, PlayerRotationY, PlayerRotationZ;

    public override void ValidateData()
    {
        throw new System.NotImplementedException();
    }
}
