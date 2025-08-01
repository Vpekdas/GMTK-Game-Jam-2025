using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum RoomPosition
    {
        Top,
        Right,
        Bottom,
        Left,
        Main,
    }

    public enum RoomType
    {
        Laser,
        Action,
    }

    public class Room
    {
        public GameObject Prefab;
        public RoomPosition RoomPosition;
        public RoomType Type;
    }

    [SerializeField] private GameObject _room;
    [SerializeField] private GameObject _door;
    private List<Room> _roomsList;
    private int _roomLength = 30;


    private void Awake()
    {
        _roomsList = new List<Room>();
    }

    private void Start()
    {
        GenerateRooms();
        RemoveWallsBetweenRooms();
        PopulateRooms();
    }


    private void GenerateRooms()
    {
        RoomType previousRoom = RoomType.Laser;

        for (int i = 0; i < 6; i++)
        {
            List<RoomType> roomTypes = new() { RoomType.Laser, RoomType.Action };

            if (i > 0)
            {
                roomTypes.Remove(previousRoom);
            }

            RoomType roomType = roomTypes[Random.Range(0, roomTypes.Count)];

            if (i == 0)
            {
                GameObject prefab = Instantiate(_room, Vector3.zero, _room.transform.rotation);
                Room room = new()
                {
                    Prefab = prefab,
                    RoomPosition = RoomPosition.Main,
                    Type = roomType,
                };
                _roomsList.Add(room);
            }
            else
            {
                RoomPosition roomPosition = (RoomPosition)Random.Range(0, 4);
                Vector3 previousPosition = _roomsList[i - 1].Prefab.transform.position;
                Vector3 position = GenerateRoomPosition(roomPosition, previousPosition);

                while (IsPositionDouble(position))
                {
                    roomPosition = (RoomPosition)Random.Range(0, 3);
                    position = GenerateRoomPosition(roomPosition, previousPosition);
                }
                Debug.Log(roomPosition);
                GameObject prefab = Instantiate(_room, position, _room.transform.rotation);
                Room room = new()
                {
                    Prefab = prefab,
                    RoomPosition = roomPosition,
                    Type = roomType,
                };
                _roomsList.Add(room);
            }

        }
    }

    private Vector3 GenerateRoomPosition(RoomPosition roomPosition, Vector3 previousPosition)
    {
        return roomPosition switch
        {
            RoomPosition.Top => previousPosition + new Vector3(0.0f, 0.0f, _roomLength),
            RoomPosition.Right => previousPosition + new Vector3(_roomLength, 0.0f, 0.0f),
            RoomPosition.Bottom => previousPosition + new Vector3(0.0f, 0.0f, -_roomLength),
            RoomPosition.Left => previousPosition + new Vector3(-_roomLength, 0.0f, 0.0f),
            _ => new Vector3(0, 0, 0),
        };
    }

    private bool IsPositionDouble(Vector3 position)
    {
        for (int i = 0; i < _roomsList.Count; i++)
        {
            if (_roomsList[i].Prefab.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveWallsBetweenRooms()
    {
        for (int i = 1; i < _roomsList.Count; i++)
        {
            Room currentRoom = _roomsList[i];
            Room previousRoom = _roomsList[i - 1];

            switch (currentRoom.RoomPosition)
            {
                case RoomPosition.Top:
                    Transform wall = _roomsList[i].Prefab.transform.Find("Bottom Wall");
                    Vector3 position = new(wall.transform.position.x + 8, 8.0f, wall.transform.position.z);
                    Instantiate(_door, position, _door.transform.rotation);

                    RemoveWallByName(currentRoom.Prefab, "Bottom Wall");
                    RemoveWallByName(previousRoom.Prefab, "Top Wall");
                    break;
                case RoomPosition.Bottom:
                    wall = _roomsList[i].Prefab.transform.Find("Top Wall");
                    position = new(wall.transform.position.x + 8, 8.0f, wall.transform.position.z);
                    Instantiate(_door, position, _door.transform.rotation);

                    RemoveWallByName(currentRoom.Prefab, "Top Wall");
                    RemoveWallByName(previousRoom.Prefab, "Bottom Wall");
                    break;
                case RoomPosition.Left:
                    wall = _roomsList[i].Prefab.transform.Find("Right Wall");
                    position = new(wall.transform.position.x, 8.0f, wall.transform.position.z - 9);
                    Instantiate(_door, position, Quaternion.Euler(0.0f, 90.0f, 0.0f));

                    RemoveWallByName(currentRoom.Prefab, "Right Wall");
                    RemoveWallByName(previousRoom.Prefab, "Left Wall");
                    break;
                case RoomPosition.Right:
                    wall = _roomsList[i].Prefab.transform.Find("Left Wall");
                    position = new(wall.transform.position.x, 8.0f, wall.transform.position.z - 9);
                    Instantiate(_door, position, Quaternion.Euler(0.0f, 90.0f, 0.0f));

                    RemoveWallByName(currentRoom.Prefab, "Left Wall");
                    RemoveWallByName(previousRoom.Prefab, "Right Wall");
                    break;
            }
        }
    }

    private void RemoveWallByName(GameObject room, string wallName)
    {
        Transform wall = room.transform.Find(wallName);
        if (wall != null)
        {
            Destroy(wall.gameObject);
        }
    }

    private void PopulateRooms()
    {
        foreach (Room room in _roomsList)
        {
            PopulateRoom(room);
        }
    }

    private void PopulateRoom(Room room)
    {
        switch (room.Type)
        {
            case RoomType.Laser:
                break;
            case RoomType.Action:
                break;
        }
    }
}
