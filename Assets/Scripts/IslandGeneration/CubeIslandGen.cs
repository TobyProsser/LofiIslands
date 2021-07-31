using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubeIslandGen : MonoBehaviour
{
    public GameObject cube;
    public GameObject plane;
    public GameObject tree;
    public GameObject shortGrass;
    public GameObject tallGrass;
    public GameObject dock;
    public GameObject ladder;

    public int treesAmount = 50;  //Higher means less amount of trees
    public Material material;
    public List<Color> grassColors = new List<Color>();
    public List<Color> stoneColors = new List<Color>();

    public bool simpleIslands;
    public float height;
    public bool seperateColors;
    public bool stoneSides;
    public bool stoneMeshCollider;

    public int size;
    public float amp;
    public float freq;

    public float groundAmp;

    public Vector2 offset;

    public int groundLevels = 0;

    [HideInInspector]
    public List<Transform> spawnPoints = new List<Transform>();

    List<GameObject> floorCubes = new List<GameObject>();

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

    public NavMeshSurface surface;

    private void Start()
    {
        GenerateIsland();
    }

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
        Vector3 pos = this.transform.position - new Vector3(size / 2, 0, size / 2);
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
                        AddStoneMesh(scale, position, blockMesh);

                        if (closestXval < pos.x)
                        {
                            closestXval = pos.x;
                            dockSpawnPoint = position;
                        }

                        if (Mathf.Abs(y) > 21)
                        {
                            float groundYScale = FindGroundYScale(y);

                            bool stone = false;
                            if (groundYScale >= 4) stone = true;

                            float groundY = blockMesh.transform.position.y + (blockMesh.transform.localScale.y / 2) + (groundYScale / 2);

                            GameObject topCube = GameObject.Instantiate(cube);
                            topCube.transform.position = new Vector3(pos.x + x, groundY + 6, pos.z + z);
                            topCube.transform.localScale = new Vector3(1, groundYScale, 1);
                            topCube.transform.parent = this.transform;

                            SpawnTree(new Vector3(pos.x + x, groundY + 5.5f, pos.z + z));

                            if (!stone)
                            {
                                topCube.GetComponent<Renderer>().material.color = grassColors[Random.Range(0, grassColors.Count)];
                                topCube.tag = "Grass";
                                spawnPoints.Add(topCube.transform);
                            }
                            else
                            {
                                topCube.GetComponent<Renderer>().material.color = stoneColors[Random.Range(0, stoneColors.Count)];
                                topCube.tag = "Stone";

                                topCube = GameObject.Instantiate(cube);
                                topCube.transform.position = new Vector3(pos.x + x, groundY + (groundYScale / 2) + 0.5f, pos.z + z);
                                topCube.transform.localScale = new Vector3(1, 1, 1);
                                topCube.transform.parent = this.transform;
                                topCube.GetComponent<Renderer>().material.color = grassColors[Random.Range(0, grassColors.Count)];
                                topCube.tag = "Grass";

                                if (groundYScale <= 3 || groundYScale == 10) spawnPoints.Add(topCube.transform);
                            }
                        }
                        else
                        {
                            if (stoneSides)
                            {
                                float yIncrease = Random.Range(.5f, 4);

                                scale = new Vector3(1, yIncrease, 1);
                                //take old block, get is position, add its size to the hieght, then add new height
                                position = new Vector3(pos.x + x, -y, pos.z + z) + new Vector3(0, (1 + (y * 2) - 11.5f) / 2, 0) + new Vector3(0, yIncrease / 2, 0);
                                AddStoneMesh(scale, position, blockMesh);
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
        }
        else
        {
            CombineMesh(stone1Combine, "Stone");
        }

        //CombineGrassMesh(shortGrassCombine);
        //CombineGrassMesh(tallGrassCombine);

        //IslandObjectSpawner islandObjectSpawner = this.GetComponent<IslandObjectSpawner>();
        //islandObjectSpawner.IslandSpawned(spawnPoints);

        //spawn dock
        Instantiate(dock, new Vector3(dockSpawnPoint.x - 1.5f, -10f, dockSpawnPoint.z), Quaternion.identity);
        Instantiate(ladder, new Vector3(dockSpawnPoint.x - .75f, -5f, dockSpawnPoint.z), Quaternion.identity);

        Destroy(floorBlockMesh);
        Destroy(blockMesh);

        surface.BuildNavMesh();
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

    void SpawnTree(Vector3 pos)
    {
        int treeChance = Random.Range(0, treesAmount);
        if (treeChance == 0)
        {
            GameObject curTree = GameObject.Instantiate(tree);
            curTree.transform.position = pos;
            curTree.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            curTree.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));

            curTree.transform.parent = this.transform;
        }
    }

    void CombineMesh(List<CombineInstance> combine, string tag)
    {
        // combine meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray());

        GameObject meshObject = new GameObject();
        meshObject.isStatic = true;
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
                tempObject.transform.parent = this.transform;
                tempObject.transform.localScale = new Vector3(1f, yScale, 1f);
                tempObject.transform.position = new Vector3(pos.x + x, yDifferential, pos.z + z);
                spawnPoints.Add(tempObject.transform);
                Destroy(tempObject);
            }
        }

        int color;
        if (seperateColors) color = Random.Range(0, grassColors.Count);
        else color = 0;


        SpawnDecoration(pos, x, z, floorBlockMesh.transform.position, floorBlockMesh.transform.localScale.y);
    }

    void SpawnDecoration(Vector3 pos, int x, int z, Vector3 location, float yScale)
    {
        AddGrassMesh(new Vector3(pos.x + x, location.y + yScale / 2, pos.z + z));

        int treeChance = Random.Range(0, treesAmount);
        if (treeChance == 0)
        {
            GameObject curTree = GameObject.Instantiate(tree);
            curTree.transform.position = new Vector3(pos.x + x, location.y + yScale / 2, pos.z + z);
            curTree.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));

            curTree.transform.parent = this.transform;
        }
    }

    void AddStoneMesh(Vector3 scale, Vector3 pos, MeshFilter blockMesh)
    {
        blockMesh.transform.localScale = scale;
        blockMesh.transform.position = pos;

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

    void AddGrassMesh(Vector3 pos)
    {
        int grassChance = Random.Range(0, 5);

        if (grassChance == 0)
        {
            int tallChance = Random.Range(0, 10);
            if (tallChance != 0)
            {
                shortGrassMesh.transform.position = pos;
                shortGrassMesh.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));
                tallGrassCombine.Add(new CombineInstance
                {
                    mesh = shortGrassMesh.sharedMesh,
                    transform = shortGrassMesh.transform.localToWorldMatrix
                });
            }
            else
            {
                tallGrassMesh.transform.position = pos;
                tallGrassMesh.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));
                shortGrassCombine.Add(new CombineInstance
                {
                    mesh = tallGrassMesh.sharedMesh,
                    transform = tallGrassMesh.transform.localToWorldMatrix
                });
            }
        }
    }

    void CombineGrassMesh(List<CombineInstance> combine)
    {
        // combine meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray());

        GameObject meshObject = new GameObject();
        meshObject.isStatic = true;
        meshObject.name = "grassMeshObject";
        meshObject.transform.parent = this.transform;
        MeshFilter mf = meshObject.AddComponent<MeshFilter>();
        MeshRenderer mr = meshObject.AddComponent<MeshRenderer>();
        mf.mesh = combinedMesh;
        mr.material = material;
        mr.material.color = grassColors[Random.Range(0, grassColors.Count)];

        meshObject.layer = 10;
    }
}
