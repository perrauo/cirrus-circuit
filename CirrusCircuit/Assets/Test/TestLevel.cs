using UnityEngine;
using System.Collections;

public class TestLevel : MonoBehaviour
{
    [SerializeField]
    private int GridSize = 2;

    [SerializeField]
    private Vector3Int Dimension = new Vector3Int(10, 10, 10);

    private TestBlock[,,] Blocks;

    public void Awake()
    {
        Blocks = new TestBlock[Dimension.x, Dimension.y, Dimension.z];
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3Int WorldToGrid(Vector3 pos)
    {
        return new Vector3Int(
            Mathf.RoundToInt(pos.x / GridSize), 
            Mathf.RoundToInt(pos.y / GridSize), 
            Mathf.RoundToInt(pos.z / GridSize));
    }

    public Vector3 GridToWorld(Vector3Int pos)
    {
        return new Vector3Int(
            pos.x * GridSize,
            pos.y * GridSize,
            pos.z * GridSize);            
    }


    public (Vector3, Vector3Int) RegisterBlock(TestBlock block)
    {
        Vector3Int gridPos = WorldToGrid(block.transform.position);
        Blocks[gridPos.x, gridPos.y, gridPos.z] = block;
        return (GridToWorld(gridPos), gridPos);
    }
}
