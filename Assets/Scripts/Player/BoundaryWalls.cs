using UnityEngine;

public class FourWallBoundary : MonoBehaviour
{
    [Header("正方形边界设置")]
    public float boundarySize = 20f;   
    public float wallHeight = 5f;       
    public float wallThickness = 1f;   

    private void Start()
    {
        CreateWalls();
    }

    private void CreateWalls()
    {
        float halfSize = boundarySize / 2f;

       
        CreateWall("LeftWall", new Vector3(-halfSize, wallHeight / 2f, 0),
            new Vector3(wallThickness, wallHeight, boundarySize));

        
        CreateWall("RightWall", new Vector3(halfSize, wallHeight / 2f, 0),
            new Vector3(wallThickness, wallHeight, boundarySize));

        
        CreateWall("FrontWall", new Vector3(0, wallHeight / 2f, halfSize),
            new Vector3(boundarySize, wallHeight, wallThickness));

        
        CreateWall("BackWall", new Vector3(0, wallHeight / 2f, -halfSize),
            new Vector3(boundarySize, wallHeight, wallThickness));
    }

    private void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(name);
        wall.transform.parent = this.transform;
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;

        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.isTrigger = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float halfSize = boundarySize / 2f;

        Vector3 topLeft = transform.position + new Vector3(-halfSize, 0, halfSize);
        Vector3 topRight = transform.position + new Vector3(halfSize, 0, halfSize);
        Vector3 bottomRight = transform.position + new Vector3(halfSize, 0, -halfSize);
        Vector3 bottomLeft = transform.position + new Vector3(-halfSize, 0, -halfSize);

        Gizmos.DrawLine(topLeft, topRight);      
        Gizmos.DrawLine(topRight, bottomRight);   
        Gizmos.DrawLine(bottomRight, bottomLeft); 
        Gizmos.DrawLine(bottomLeft, topLeft);     
    }
}
