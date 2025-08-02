using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum RoomPosition
    {
        Top,
        Right,
        Bottom,
        Left,
    }

    public enum RoomType
    {
        Laser,
        Action,
        Main,
        End,
    }

    public class Room
    {
        public GameObject Prefab;
        public RoomPosition Position;
        public RoomType Type;
        public int Index;
        public bool Finished = false;
    }

    public class ActionRoom : Room
    {
        public List<GameObject> Enemies = new();

        public bool AllEnemyAreDead()
        {
            foreach (GameObject enemy in Enemies)
            {
                if (!enemy.GetComponent<Mortal>().IsDead())
                    return false;
            }

            return true;
        }
    }

    [SerializeField] private GameObject _room, _door, _laser, _laserRoomCollectible, _mainRoom, _endRoom;
    [SerializeField] private bool _forceRoomType = false;
    [SerializeField] private RoomType _forcedRoomType = RoomType.Laser;
    [SerializeField] private int _laserNumber;
    [SerializeField] private PlayerController _player;
    private List<Room> _roomsList;
    private readonly int _roomLength = 30, _wallHeight = 16;
    public Room CurrentRoom;


    private void Awake()
    {
        _roomsList = new List<Room>();
    }

    private void Start()
    {
        GenerateRooms();
        RemoveWallsBetweenRooms();
        PopulateRooms();

        // TODO: Should not be set before entering the first room.
        CurrentRoom = _roomsList[0];
    }

    private void Update()
    {
        if (CurrentRoom is ActionRoom room && !room.Finished)
        {
            if (room.AllEnemyAreDead())
            {
                if (room.Index < 5)
                {
                    Transform door = _roomsList[room.Index + 1].Prefab.transform.Find("Door Wall(Clone)");
                    door.GetComponent<Door>().IsOpening = true;
                    room.Finished = true;
                }
                else
                {
                    // TODO: open door to last room
                }
            }
        }
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

            RoomType roomType = _forceRoomType ? _forcedRoomType : roomTypes[Random.Range(0, roomTypes.Count)];

            if (i == 0)
            {
                GameObject prefab = Instantiate(_mainRoom, Vector3.zero, _mainRoom.transform.rotation);
                prefab.name = "Room" + i;

                Room room = new()
                {
                    Prefab = prefab,
                    Position = RoomPosition.Top,
                    Type = RoomType.Main,
                    Index = i,
                };
                _roomsList.Add(room);
            }
            else if (i == 5)
            {
                RoomPosition roomPosition = (RoomPosition)Random.Range(0, 4);
                Vector3 previousPosition = _roomsList[i - 1].Prefab.transform.position;
                Vector3 position = GenerateRoomPosition(roomPosition, previousPosition);

                while (IsPositionDouble(position))
                {
                    roomPosition = (RoomPosition)Random.Range(0, 3);
                    position = GenerateRoomPosition(roomPosition, previousPosition);
                }
                GameObject prefab = Instantiate(_endRoom, position, _endRoom.transform.rotation);
                prefab.name = "Room" + i;
                Room room = new()
                {
                    Prefab = prefab,
                    Position = roomPosition,
                    Type = RoomType.End,
                    Index = i,
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
                GameObject prefab = Instantiate(_room, position, _room.transform.rotation);
                prefab.name = "Room" + i;

                Room room = null;
                switch (roomType)
                {
                    case RoomType.Action:
                        {
                            room = new ActionRoom()
                            {
                                Prefab = prefab,
                                Position = roomPosition,
                                Type = roomType,
                                Index = i,
                            };
                        }
                        break;
                    case RoomType.Laser:
                        {
                            room = new()
                            {
                                Prefab = prefab,
                                Position = roomPosition,
                                Type = roomType,
                                Index = i,
                            };
                        }
                        break;
                }
                _roomsList.Add(room);
            }

            previousRoom = roomType;
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

            switch (currentRoom.Position)
            {
                case RoomPosition.Top:
                    Transform wall = _roomsList[i].Prefab.transform.Find("Bottom Wall");
                    Vector3 position = new(wall.transform.position.x + 9, 8.0f, wall.transform.position.z);
                    GameObject doorWall = Instantiate(_door, position, _door.transform.rotation);
                    doorWall.transform.parent = _roomsList[i].Prefab.transform;
                    doorWall.GetComponent<Door>().Room = currentRoom;

                    RemoveWallByName(currentRoom.Prefab, "Bottom Wall");
                    RemoveWallByName(previousRoom.Prefab, "Top Wall");
                    break;
                case RoomPosition.Bottom:
                    wall = _roomsList[i].Prefab.transform.Find("Top Wall");
                    position = new(wall.transform.position.x + 9, 8.0f, wall.transform.position.z);
                    doorWall = Instantiate(_door, position, _door.transform.rotation);
                    doorWall.transform.parent = _roomsList[i].Prefab.transform;
                    doorWall.GetComponent<Door>().Room = currentRoom;

                    RemoveWallByName(currentRoom.Prefab, "Top Wall");
                    RemoveWallByName(previousRoom.Prefab, "Bottom Wall");
                    break;
                case RoomPosition.Left:
                    wall = _roomsList[i].Prefab.transform.Find("Right Wall");
                    position = new(wall.transform.position.x, 8.0f, wall.transform.position.z - 9);
                    doorWall = Instantiate(_door, position, Quaternion.Euler(0.0f, 90.0f, 0.0f));
                    doorWall.transform.parent = _roomsList[i].Prefab.transform;
                    doorWall.GetComponent<Door>().Room = currentRoom;

                    RemoveWallByName(currentRoom.Prefab, "Right Wall");
                    RemoveWallByName(previousRoom.Prefab, "Left Wall");
                    break;
                case RoomPosition.Right:
                    wall = _roomsList[i].Prefab.transform.Find("Left Wall");
                    position = new(wall.transform.position.x, 8.0f, wall.transform.position.z - 9);
                    doorWall = Instantiate(_door, position, Quaternion.Euler(0.0f, 90.0f, 0.0f));
                    doorWall.transform.parent = _roomsList[i].Prefab.transform;
                    doorWall.GetComponent<Door>().Room = currentRoom;

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
            switch (room.Type)
            {
                case RoomType.Laser:
                    GenerateLaserRoom(room);
                    break;
                case RoomType.Action:
                    PopulateActionRoom((ActionRoom)room);
                    break;
            }
        }
    }

    enum ActionObstacleType
    {
        Empty,
        FlyingEnemy,
        Crates,
        ExplosiveBarrel,
        ConcreteSlab,
    }

    [SerializeField] private GameObject _flyingEnemy;
    [SerializeField] private GameObject _crates;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _concreteSlab;

    private void PopulateActionRoom(ActionRoom room)
    {
        ActionObstacleType[] obstacles = new ActionObstacleType[15 * 15];

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                bool hasObstacle = Random.Range(0, 10) == 0;

                if (!hasObstacle)
                {
                    obstacles[x + y * 15] = ActionObstacleType.Empty;
                    continue;
                }

                int obstacleType = Random.Range(0, 8);
                if (obstacleType == 0)
                {
                    obstacles[x + y * 15] = ActionObstacleType.FlyingEnemy;
                }
                else if (obstacleType < 3)
                {
                    obstacles[x + y * 15] = ActionObstacleType.ExplosiveBarrel;
                }
                else if (obstacleType < 5)
                {
                    obstacles[x + y * 15] = ActionObstacleType.Crates;
                }
                else
                {
                    obstacles[x + y * 15] = ActionObstacleType.ConcreteSlab;
                }
            }
        }

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                ActionObstacleType obstacle = obstacles[x + y * 15];

                switch (obstacle)
                {
                    case ActionObstacleType.Empty:
                        break;
                    case ActionObstacleType.FlyingEnemy:
                        {
                            Vector2 randomness = new(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                            float angle = Random.Range(-180.0f, 180.0f);
                            GameObject enemy = Instantiate(_flyingEnemy, room.Prefab.transform.position + new Vector3(x * 2.0f + 1.0f + randomness.x - _roomLength / 2.0f, 2.1f, y * 2.0f + 1.0f + randomness.y - _roomLength / 2.0f), Quaternion.Euler(0.0f, angle, 0.0f));
                            enemy.GetComponent<FlyingDrone>().Target = _player.transform;

                            room.Enemies.Add(enemy);
                        }
                        break;
                    case ActionObstacleType.Crates:
                        {
                            Vector2 randomness = new(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                            float angle = Random.Range(-180.0f, 180.0f);
                            Instantiate(_crates, room.Prefab.transform.position + new Vector3(x * 2.0f + 1.0f + randomness.x - _roomLength / 2.0f, 1.0f, y * 2.0f + 1.0f + randomness.y - _roomLength / 2.0f), Quaternion.Euler(0.0f, angle, 0.0f));
                        }
                        break;
                    case ActionObstacleType.ExplosiveBarrel:
                        {
                            Vector2 randomness = new(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                            Instantiate(_barrel, room.Prefab.transform.position + new Vector3(x * 2.0f + 1.0f + randomness.x - _roomLength / 2.0f, 1.0f, y * 2.0f + 1.0f + randomness.y - _roomLength / 2.0f), Quaternion.identity);
                        }
                        break;
                    case ActionObstacleType.ConcreteSlab:
                        {
                            Vector2 randomness = new(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                            float angle = Random.Range(-180.0f, 180.0f);
                            Instantiate(_concreteSlab, room.Prefab.transform.position + new Vector3(x * 2.0f + 1.0f + randomness.x - _roomLength / 2.0f, 2.5f, y * 2.0f + 1.0f + randomness.y - _roomLength / 2.0f), Quaternion.Euler(-90.0f, angle, 0.0f));
                        }
                        break;
                }
            }
        }
    }

    private void GenerateLaserRoom(Room room)
    {
        for (int i = 0; i < _laserNumber; i++)
        {
            GameObject laser = Instantiate(_laser, room.Prefab.transform);
            Vector3 position = laser.transform.localPosition;
            int x = Random.Range(-_roomLength / 2, _roomLength / 2);
            int y = Random.Range(0, _wallHeight / 2);
            position.x = x;
            position.y = y;
            laser.transform.localPosition = position;
            laser.GetComponent<Laser>().Room = room.Prefab;
        }

        GameObject collectible = Instantiate(_laserRoomCollectible, room.Prefab.transform);
        Vector3 collectiblePosition = collectible.transform.localPosition;
        collectiblePosition.y = 1.8f;
        collectible.transform.localPosition = collectiblePosition;
    }


}
