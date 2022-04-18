using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Shipyard/Room")]
public class RoomSO : ScriptableObject
{
    public GameObject Prefab;

    public RoomGroupSO NorthAllowedRooms;
    public RoomGroupSO SouthAllowedRooms;
    public RoomGroupSO EastAllowedRooms;
    public RoomGroupSO WestAllowedRooms;
}
