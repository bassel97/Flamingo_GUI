using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    [Header("Stones postionning")]
    [SerializeField] private Transform stonesParent = null;

    [Header("LayOut Data")]
    [SerializeField] private float lengthInUnityScale = 0;
    //[SerializeField] private float squareLengthInUnityScale;

    [Header("Stones Prefabs")]
    [SerializeField] private GameObject whiteStonePrefab = null;
    [SerializeField] private GameObject blackStonePrefab = null;

    [Header("TEST")]
    public int rowTest;
    public int colTest;

    private BoardCell[,] grid = new BoardCell[19, 19];
    private GameObject[,] stones = new GameObject[19, 19];

    private void Awake()
    {
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                grid[i, j] = new BoardCell(i, j, lengthInUnityScale, stonesParent);
            }
        }
    }

    Ray ray;
    Vector3 hitPoint;
    private void Update()
    {
        MouseInputs();

        //Test
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlaceStone(rowTest, colTest, true);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            PlaceStone(rowTest, colTest, false);
        }
    }

    public void MouseInputs()
    {
        float minSqrDist = Mathf.Pow((lengthInUnityScale / 38.0f), 2);

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane m_Plane = new Plane(Vector3.up, stonesParent.up);

            float enter;
            if (m_Plane.Raycast(ray, out enter))
            {
                hitPoint = ray.GetPoint(enter);

                float x = hitPoint.x;
                float z = hitPoint.z;

                for (int i = 0; i < 19; i++)
                {
                    for (int j = 0; j < 19; j++)
                    {
                        Vector3 gridPos = grid[i, j].GetPosition();

                        Vector2 distance = new Vector2(gridPos.x - x, gridPos.z - z);

                        if (distance.sqrMagnitude < minSqrDist)
                        {
                            if (!Input.GetMouseButtonDown(2))
                                PlaceStone(i, j, Input.GetMouseButtonDown(0));
                            else
                                grid[i, j].RemoveStone();
                        }
                    }
                }

            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
    }

    public void PlaceStone(int row, int col, bool white)
    {
        if (row < 0 || row > 18)
        {
            Debug.LogWarning("Row is not correct");
            return;
        }
        if (col < 0 || col > 18)
        {
            Debug.LogWarning("Col is not correct");
            return;
        }
        if (grid[row, col].cellVacancy != CellVacancy.Free)
        {
            Debug.LogWarning("Cell is not free");
            return;
        }

        if (white)
        {
            grid[row, col].CreateStone(whiteStonePrefab, stonesParent);
            grid[row, col].ChangeState(CellVacancy.White);
        }

        if (!white)
        {
            grid[row, col].CreateStone(blackStonePrefab, stonesParent);
            grid[row, col].ChangeState(CellVacancy.Black);
        }
    }

    public void RemoveStone(int row,int col)
    {
        grid[row, col].RemoveStone();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hitPoint, 0.25f);

        if (grid[0, 0] != null)
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    Vector3 stonePosition = grid[i, j].GetPosition();

                    float squareLengthInUnityScale = lengthInUnityScale / 19f;
                    Vector3 stoneScale = Vector3.one * (squareLengthInUnityScale);

                    Gizmos.color = new Color(0, 255.0f, 255.0f);
                    Gizmos.DrawWireCube(stonePosition, stoneScale);
                }
            }
    }
}