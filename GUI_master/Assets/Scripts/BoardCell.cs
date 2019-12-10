using UnityEngine;

public class BoardCell
{
    public CellVacancy cellVacancy;

    private int row;
    private int col;
    private float boardSize;
    private Transform stonesParent;

    private GameObject stonePrefab;

    public BoardCell(int row, int col, float boardSize, Transform stonesParent)
    {
        cellVacancy = CellVacancy.Free;

        this.row = row;
        this.col = col;
        this.boardSize = boardSize;
        this.stonesParent = stonesParent;

        stonePrefab = null;
    }

    public void ChangeState(CellVacancy newState)
    {
        if (newState == cellVacancy)
            return;

        cellVacancy = newState;
    }

    public void CreateStone(GameObject cellPrefab, Transform parent)
    {
        if (stonePrefab != null)
        {
            Debug.LogWarning("Cell is not free");
            return;
        }
        stonePrefab = GameObject.Instantiate(cellPrefab, GetPosition(), Quaternion.identity, parent);
    }

    public void RemoveStone()
    {
        if (stonePrefab == null)
        {
            Debug.LogWarning("Cell is free");
            return;
        }
        GameObject.Destroy(stonePrefab);
        ChangeState(CellVacancy.Free);
    }

    public Vector3 GetPosition()
    {
        float squareLengthInUnityScale = boardSize / 19f;

        Vector3 stonePosition = stonesParent.position;

        stonePosition += new Vector3(row * squareLengthInUnityScale, 0, -col * squareLengthInUnityScale);

        Vector3 stoneScale = Vector3.one * (squareLengthInUnityScale);

        return stonePosition;
    }
}

public enum CellVacancy
{
    Free,
    White,
    Black
}