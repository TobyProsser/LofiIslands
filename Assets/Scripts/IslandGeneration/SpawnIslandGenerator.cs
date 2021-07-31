using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnIslandGenerator : MonoBehaviour
{
    public GameObject cube;
    public GameObject dock;
    public GameObject ladder;

    public Material material;
    public List<Color> grassColors = new List<Color>();
    public List<Color> stoneColors = new List<Color>();

    public bool storeIsland;

    public bool hollowIsland;
    public bool simpleIslands;
    public float height;
    public bool seperateColors;
    public bool stoneSides;
    public bool stoneMeshCollider;

    public int size;
    public float amp;
    public float freq;

    public float groundAmp;

    public Vector2Int offset;

    public int groundLevels = 0;

    [HideInInspector]
    public List<Transform> spawnPoints = new List<Transform>();

    List<GameObject> meshObjectList = new List<GameObject>();

    List<CombineInstance> floor1Combine = new List<CombineInstance>();
    List<CombineInstance> floor2Combine = new List<CombineInstance>();
    List<CombineInstance> floor3Combine = new List<CombineInstance>();
    List<CombineInstance> floor4Combine = new List<CombineInstance>();

    List<CombineInstance> stone1Combine = new List<CombineInstance>();
    List<CombineInstance> stone2Combine = new List<CombineInstance>();
    List<CombineInstance> stone3Combine = new List<CombineInstance>();
    List<CombineInstance> stone4Combine = new List<CombineInstance>();

    List<CombineInstance> shortGrassCombine = new List<CombineInstance>();
    List<CombineInstance> tallGrassCombine = new List<CombineInstance>();

    MeshFilter floorBlockMesh;
    MeshFilter blockMesh;

    MeshFilter shortGrassMesh;
    MeshFilter tallGrassMesh;

    Vector3 dockSpawnPoint;

    bool checkIfOuterRadius;

    public NavMeshSurface navSurface;

    public void GenerateIsland()
    {
        floorBlockMesh = Instantiate(cube, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();
        blockMesh = Instantiate(cube, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        //hortGrassMesh = Instantiate(shortGrass, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();
        //tallGrassMesh = Instantiate(tallGrass, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        GenerateBase();
    }

    void GenerateBase()
    {
        Vector3 pos = this.transform.position - new Vector3(size/2, 0, size/2);
        int cols = size;
        int rows = size;

        float closestXval = -Mathf.Infinity;

        for (int x = 0; x < cols; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                float y = Mathf.PerlinNoise(x / freq + offset.x, z / freq + offset.y) * amp;

                float radius = size / 2;

                if (Vector2.Distance(new Vector2(cols / 2, rows / 2), new Vector2(x, z)) < radius)
                {
                    bool insideBuildingRadius = Vector2.Distance(new Vector2(cols / 2, rows / 2), new Vector2(x, z)) < (radius * 0.8f);

                    if (Mathf.Abs(y) > 20)
                    {
                        Vector3 scale = new Vector3(1, height, 1);
                        Vector3 position = new Vector3(pos.x + x, -height, pos.z + z);

                        //checks to see if stone is on outer radius of island
                        //helps prevent island base being filled
                        checkIfOuterRadius = Vector2.Distance(new Vector2(cols / 2, rows / 2), new Vector2(x, z)) > (radius * 0.9f);
                        AddStoneMesh(scale, position, blockMesh, y, false);


                        if (closestXval < pos.x)
                        {
                            closestXval = pos.x;
                            dockSpawnPoint = position;
                        }

                        if (Mathf.Abs(y) > 23)
                        {
                            float groundYScale = FindGroundYScale(y);

                            bool stone = false;
                            if (groundYScale >= 4) stone = true;

                            float groundY = blockMesh.transform.position.y + (blockMesh.transform.localScale.y / 2) + (groundYScale / 2);

                            if (!stone)
                            {
                                SpawnTopCube(pos, groundY, groundYScale, x, z, groundY, insideBuildingRadius);
                            }
                            else
                            {
                                scale = new Vector3(1, groundYScale, 1);
                                position = new Vector3(pos.x + x, groundY, pos.z + z);
                                AddStoneMesh(scale, position, blockMesh, y, true);

                                SpawnTopCube(pos, groundY, 1, x, z, groundY + (groundYScale / 2) + 0.5f, insideBuildingRadius);
                            }
                        }
                        else
                        {
                            if (stoneSides)
                            {
                                float yIncrease = Random.Range(.5f, 4);

                                scale = new Vector3(1, yIncrease, 1);
                                //take old block, get is position, add its size to the hieght, then add new height
                                position = new Vector3(pos.x + x, -y, pos.z + z) + new Vector3(0, (1 + (y * 2) - 11.5f) / 2, 0) + new Vector3(0, yIncrease/2, 0);
                                AddStoneMesh(scale, position, blockMesh, y, false);
                            }
                        }
                    }
                }
            }
        }

        if (seperateColors)
        {
            CombineMesh(stone1Combine, "BottomStone");
            CombineMesh(stone2Combine, "BottomStone");
            CombineMesh(stone3Combine, "BottomStone");
            CombineMesh(stone4Combine, "BottomStone");

            CombineMesh(floor1Combine, "Grass");
            CombineMesh(floor2Combine, "Grass");
            CombineMesh(floor3Combine, "Grass");
            CombineMesh(floor4Combine, "Grass");
        }
        else
        {
            CombineMesh(stone1Combine, "Stone");
            CombineMesh(floor1Combine, "Grass");
        }

        if (!storeIsland)
        {
            IslandObjectSpawner islandObjectSpawner = this.GetComponent<IslandObjectSpawner>();
            islandObjectSpawner.IslandSpawned(spawnPoints);
        }
        else
        {
            //navSurface.BuildNavMesh();
        }

        //spawn dock
        GameObject dockObject = Instantiate(dock, new Vector3(dockSpawnPoint.x - 1.5f, -10f, dockSpawnPoint.z), Quaternion.identity);
        dockObject.transform.parent = this.transform;
        GameObject ladderObject = Instantiate(ladder, new Vector3(dockSpawnPoint.x - .75f, -5f, dockSpawnPoint.z), Quaternion.identity);
        ladderObject.transform.parent = this.transform;

        Destroy(floorBlockMesh);
        Destroy(blockMesh);
    }

    float FindGroundYScale(float y)
    {
        float groundYScale = (y / amp);

        if (groundLevels == 0)
        {
            groundYScale = 1;
        }
        else if (groundLevels == 1)
        {
            if (groundYScale < .6f) groundYScale = 1;
            else if (groundYScale < .7f) groundYScale = 2;
        }
        else if (groundLevels == 2)
        {
            if (groundYScale < .6f) groundYScale = 1;
            else if (groundYScale < .7f) groundYScale = 2;
            else groundYScale = 3;
        }
        else if (groundLevels == 3)
        {
            if (groundYScale < .6f) groundYScale = 1;
            else if (groundYScale < .7f) groundYScale = 2;
            else if (groundYScale < .83f) groundYScale = 3;
            else if (groundYScale < .85f) groundYScale = 5;
            else if (groundYScale < .88f) groundYScale = 6;
            else if (groundYScale < .9f) groundYScale = 7;
            else if (groundYScale < .93f) groundYScale = 9;
            else groundYScale = 10;
        }

        return groundYScale;
    }

    void CombineMesh(List<CombineInstance> combine, string tag)
    {
        // combine meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray());

        GameObject meshObject = new GameObject();
        //meshObject.isStatic = true;
        meshObject.name = "meshObject";
        meshObject.tag = tag;
        meshObject.transform.parent = this.transform;
        MeshFilter mf = meshObject.AddComponent<MeshFilter>();
        MeshRenderer mr = meshObject.AddComponent<MeshRenderer>();
        
        mf.mesh = combinedMesh;
        mr.material = material;
        mr.material.color = (tag == "Stone" || tag == "BottomStone") ? stoneColors[Random.Range(0, stoneColors.Count)] : grassColors[Random.Range(0, grassColors.Count)];

        if (stoneMeshCollider)
        {
            MeshCollider mc = meshObject.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
        }
        else if (tag != "BottomStone")
        {
            MeshCollider mc = meshObject.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
            meshObject.layer = 6;
        }
        else
        {
            meshObject.layer = 8;
        }


        meshObject.transform.position += new Vector3(0, 6, 0);
    }

    void SpawnTopCube(Vector3 pos, float groundY, float yScale, int x, int z, float yDifferential, bool insideBuildingRadius)
    {
        floorBlockMesh.transform.localScale = new Vector3(1f, yScale, 1f);
        floorBlockMesh.transform.position = new Vector3(pos.x + x, yDifferential, pos.z + z);

        if (insideBuildingRadius && yScale < 4)
        {
            GameObject tempObject = new GameObject();
            if (tempObject != null)
            {
                tempObject.transform.localScale = floorBlockMesh.transform.localScale;
                tempObject.transform.position = floorBlockMesh.transform.position + new Vector3(0,6,0);
                spawnPoints.Add(tempObject.transform);
                //Instantiate(cube, tempObject.transform.position, Quaternion.identity);
                Destroy(tempObject);
            }
        }

        int color;
        if (seperateColors) color = Random.Range(0, grassColors.Count);
        else color = 0;

        if (color == 0)
        {
            floor1Combine.Add(new CombineInstance
            {
                mesh = floorBlockMesh.sharedMesh,
                transform = floorBlockMesh.transform.localToWorldMatrix
            });
        }
        else if (color == 1)
        {
            floor2Combine.Add(new CombineInstance
            {
                mesh = floorBlockMesh.sharedMesh,
                transform = floorBlockMesh.transform.localToWorldMatrix
            });
        }
        else if (color == 2)
        {
            floor3Combine.Add(new CombineInstance
            {
                mesh = floorBlockMesh.sharedMesh,
                transform = floorBlockMesh.transform.localToWorldMatrix
            });
        }
        else if (color == 3)
        {
            floor4Combine.Add(new CombineInstance
            {
                mesh = floorBlockMesh.sharedMesh,
                transform = floorBlockMesh.transform.localToWorldMatrix
            });
        }
    }

    void AddStoneMesh(Vector3 scale, Vector3 pos, MeshFilter blockMesh, float y, bool aboveGroundStone)
    {
        blockMesh.transform.localScale = scale;
        blockMesh.transform.position = pos;

        //Only Spawns nessessary stone
        if (checkIfOuterRadius || Mathf.Abs(y) < 30 || !hollowIsland || aboveGroundStone)
        {
            int color;
            if (seperateColors) color = Random.Range(0, grassColors.Count);
            else color = 0;

            if (color == 0)
            {
                stone1Combine.Add(new CombineInstance
                {
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix
                });
            }
            else if (color == 1)
            {
                stone2Combine.Add(new CombineInstance
                {
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix
                });
            }
            else if (color == 2)
            {
                stone3Combine.Add(new CombineInstance
                {
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix
                });
            }
            else if (color == 3)
            {
                stone4Combine.Add(new CombineInstance
                {
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix
                });
            }
        }
    }
}
