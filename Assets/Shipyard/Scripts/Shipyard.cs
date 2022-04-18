using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Shipyard : MonoBehaviour
{
    public RoomGroupSO DefaultGroup;
    public EdgeAvailabilitySO IgnoreEdge;

    public UnityEvent DoneGenerating;

    private Dictionary<Vector3Int, RoomSO> CreatedRooms = new Dictionary<Vector3Int, RoomSO>();
    private Dictionary<Vector3Int, RoomSO[]> Domains = new Dictionary<Vector3Int, RoomSO[]>();

    private void Start()
    {
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        //create a base room
        RoomSO BaseRoom = DefaultGroup.Rooms[Random.Range(0, DefaultGroup.Rooms.Length)];
        Instantiate(BaseRoom.Prefab, Vector3.zero, Quaternion.identity);
        //add it to the created rooms dict
        CreatedRooms.Add(Vector3Int.zero, BaseRoom);


        while (RecalculateDomains())
        {
            Vector3Int NextLocation = FindCollapsablePoint();
            RoomSO NextRoom = Domains[NextLocation][Random.Range(0, Domains[NextLocation].Length)];
            Instantiate(NextRoom.Prefab, NextLocation * 6, Quaternion.identity);
            CreatedRooms.Add(NextLocation, NextRoom);

            yield return null;
        }        
    }

    public bool RecalculateDomains()
    {
        Domains.Clear();

        foreach (KeyValuePair<Vector3Int, RoomSO> CreatedRoom in CreatedRooms)
        {
            //check if each side is already in CreatedRooms
            if (!CreatedRooms.ContainsKey(CreatedRoom.Key + Vector3Int.forward) && CreatedRoom.Value.NorthAllowedRooms.AllowGeneratingFromHere)
            {
                //The area to the north is not occupied
                if (!Domains.ContainsKey(CreatedRoom.Key + Vector3Int.forward))
                {
                    Domains.Add(CreatedRoom.Key + Vector3Int.forward, GetValidRoomsAtPosition(CreatedRoom.Key + Vector3Int.forward));
                }
            }

            if (!CreatedRooms.ContainsKey(CreatedRoom.Key - Vector3Int.forward) && CreatedRoom.Value.SouthAllowedRooms.AllowGeneratingFromHere)
            {
                //The area to the south is not occupied
                if (!Domains.ContainsKey(CreatedRoom.Key - Vector3Int.forward))
                {
                    Domains.Add(CreatedRoom.Key - Vector3Int.forward, GetValidRoomsAtPosition(CreatedRoom.Key - Vector3Int.forward));
                }
            }

            if (!CreatedRooms.ContainsKey(CreatedRoom.Key + Vector3Int.right) && CreatedRoom.Value.EastAllowedRooms.AllowGeneratingFromHere)
            {
                //The area to the east is not occupied
                if (!Domains.ContainsKey(CreatedRoom.Key + Vector3Int.right))
                {
                    Domains.Add(CreatedRoom.Key + Vector3Int.right, GetValidRoomsAtPosition(CreatedRoom.Key + Vector3Int.right));
                }
            }

            if (!CreatedRooms.ContainsKey(CreatedRoom.Key - Vector3Int.right) && CreatedRoom.Value.WestAllowedRooms.AllowGeneratingFromHere)
            {
                //The area to the west is not occupied
                if (!Domains.ContainsKey(CreatedRoom.Key - Vector3Int.right))
                {
                    Domains.Add(CreatedRoom.Key - Vector3Int.right, GetValidRoomsAtPosition(CreatedRoom.Key - Vector3Int.right));
                }
            }
        }

        Domains = Domains.Where(x => x.Value.Length != 0).ToDictionary(x => x.Key, x => x.Value);

        return Domains.Count != 0;
    }

    public RoomSO[] GetValidRoomsAtPosition(Vector3Int Position)
    {

        //get the room lists of each side of the position
        RoomGroupSO NorthRooms;
        RoomGroupSO SouthRooms;
        RoomGroupSO EastRooms;
        RoomGroupSO WestRooms;
        RoomSO NorthRoom;
        RoomSO SouthRoom;
        RoomSO EastRoom;
        RoomSO WestRoom;

        CreatedRooms.TryGetValue(Position + Vector3Int.forward, out NorthRoom);
        NorthRooms = NorthRoom?.SouthAllowedRooms;
        CreatedRooms.TryGetValue(Position + Vector3Int.right, out EastRoom);
        EastRooms = EastRoom?.WestAllowedRooms;
        CreatedRooms.TryGetValue(Position - Vector3Int.forward, out SouthRoom);
        SouthRooms = SouthRoom?.NorthAllowedRooms;
        CreatedRooms.TryGetValue(Position - Vector3Int.right, out WestRoom);
        WestRooms = WestRoom?.EastAllowedRooms;

        //find everything that is in all of them
        //initialise ret to have all of one of a list in
        List<RoomSO> ret = new List<RoomSO>();
        if (NorthRooms != null)
        {
            ret.AddRange(NorthRooms.Rooms);
        }
        else if (SouthRooms != null)
        {
            ret.AddRange(SouthRooms.Rooms);
        }
        else if (EastRooms != null)
        {
            ret.AddRange(EastRooms.Rooms);
        }
        else if (WestRooms != null)
        {
            ret.AddRange(WestRooms.Rooms);
        }

        if (NorthRooms != null)
        {
            ret = ret.Where(x => NorthRooms.Rooms.Contains(x)).ToList();
        }

        if (SouthRooms != null)
        {
            ret = ret.Where(x => SouthRooms.Rooms.Contains(x)).ToList();
        }

        if (EastRooms != null)
        {
            ret = ret.Where(x => EastRooms.Rooms.Contains(x)).ToList();
        }

        if (WestRooms != null)
        {
            ret = ret.Where(x => WestRooms.Rooms.Contains(x)).ToList();
        }

        return ret.ToArray();

    }

    public Vector3Int FindCollapsablePoint()
    {
        /*
        Vector3Int ret = Vector3Int.zero;
        int SmallestDomainSize = int.MaxValue;

        foreach (KeyValuePair<Vector3Int, RoomSO[]> domain in Domains)
        {
            
            if (domain.Value.Length < SmallestDomainSize)
            {
                ret = domain.Key;
                SmallestDomainSize = domain.Value.Length;
            }
            
        }

        return ret;
        */

        return Domains.ElementAt(Random.Range(0, Domains.Count)).Key;

    }
}
