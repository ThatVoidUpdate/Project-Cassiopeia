using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Shipyard/Room Group")]
public class RoomGroupSO : ScriptableObject
{
    public bool AllowGeneratingFromHere;
    public RoomSO[] Rooms;
}
