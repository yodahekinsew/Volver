using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EndPoint
{
    public Transform transform;
    public Vector3 position;
    public float angle;
}

struct RayWall
{
    public Vector3 firstPoint;
    public Vector3 secondPoint;
}

public class RaycastLight : MonoBehaviour
{
    public WallSpawner spawner;
    public Transform hook;
    public Transform wall;
    public Material lightMaterial;

    private float angleDiff = .05f;
    private bool once = false;
    private float calculationRate = 60f;
    // private float calculationRate = .00001f;
    private float nextTimeToCalculate = 0;
    private List<int> seen = new List<int>();

    Mesh lightingMesh;

    // Start is called before the first frame update
    void Start()
    {
        lightingMesh = new Mesh();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = lightMaterial;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        // CalculateLighting();
        meshFilter.mesh = lightingMesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (hook != null) transform.position = hook.position;
        // this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.transform.parent.rotation.z * -1.0f);
        if (Time.time > nextTimeToCalculate) CalculateLighting();
        // Calculate();
    }
    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        float angle360 = (signed_angle + 360) % 360;
        if (angle360 == 180) return 0;
        return angle360;
    }

    Vector3[] GetColliderCorners(BoxCollider2D collider)
    {
        Quaternion colliderRot = collider.transform.rotation;
        Vector3 colliderPos = collider.transform.position;
        Vector3 colliderScale = collider.transform.localScale / 2f;
        Bounds bounds = collider.bounds;
        Vector3 topLeft = colliderPos + colliderRot * new Vector3(-colliderScale.x, colliderScale.y, 0);
        Vector3 topRight = colliderPos + colliderRot * new Vector3(colliderScale.x, colliderScale.y, 0);
        Vector3 bottomLeft = colliderPos + colliderRot * new Vector3(-colliderScale.x, -colliderScale.y, 0);
        Vector3 bottomRight = colliderPos + colliderRot * new Vector3(colliderScale.x, -colliderScale.y, 0);
        // print("box collider corners: " + collider.size + " " + collider.transform.eulerAngles + " " + topLeft + " " + topRight + " " + bottomLeft + " " + bottomRight);
        return new Vector3[4] {
            topLeft,
            topRight,
            bottomLeft,
            bottomRight
        };
    }

    Vector3[] GetAnchors(Transform wall)
    {
        Vector3 pos = wall.position;
        Vector3 scale = wall.localScale / 2f;
        Vector3 topLeft = pos + new Vector3(-scale.x, scale.y, 0);
        Vector3 topRight = pos + new Vector3(scale.x, scale.y, 0);
        Vector3 bottomLeft = pos + new Vector3(-scale.x, -scale.y, 0);
        Vector3 bottomRight = pos + new Vector3(scale.x, -scale.y, 0);
        return new Vector3[4] {
            topLeft,
            topRight,
            bottomLeft,
            bottomRight
        };
    }

    void CalculateLighting()
    {
        // nextTimeToCalculate += 1 / 120f;
        // Get the current game wall
        // Transform wall = spawner.GetCurrentWall();
        if (wall == null) return;
        print(wall.position + " " + wall.eulerAngles);
        float rotationFromUp = 360 - SignedAngleBetween(Vector3.up, wall.transform.up, -Vector3.forward);
        // float rotationFromDown = Vector3.SignedAngle(Vector3.down, wall.transform.position, Vector3.forward);
        // float rotation = 0;
        // if (Mathf.Abs(rotationFromUp) < Mathf.Abs(rotationFromDown)) rotation = rotationFromUp;
        // else rotation = rotationFromDown;
        float rotation = rotationFromUp;
        print(rotation);
        // float rotation = Vector3.SignedAngle(Vector3.up, wall.transform.position, -Vector3.forward);
        wall.RotateAround(transform.position, Vector3.forward, -rotation);

        // For both the left and right side of each wall, get the second closest and farthest points to the light
        print("----------------------------");
        Vector3[] leftAnchors = GetAnchors(wall.Find("LeftWall"));
        Vector3[] rightAnchors = GetAnchors(wall.Find("RightWall"));
        print("Left Anchors: " + leftAnchors[0] + " " + leftAnchors[1] + " " + leftAnchors[2] + " " + leftAnchors[3]);
        print("Right Anchors: " + rightAnchors[0] + " " + rightAnchors[1] + " " + rightAnchors[2] + " " + rightAnchors[3]);
        wall.RotateAround(transform.position, Vector3.forward, rotation);
        transform.eulerAngles = new Vector3(0, 0, rotation);

        float gameSize = 2 * Camera.main.orthographicSize;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 norm = new Vector3(0, 0, 1);
        List<Vector3> normals = new List<Vector3>();
        if (transform.position.y < rightAnchors[0].y)
        {
            print("below right");
            vertices.Add(rightAnchors[1]);
            vertices.Add(rightAnchors[0]);
            vertices.Add(rightAnchors[0].normalized * gameSize);
        }
        if (transform.position.y > rightAnchors[2].y)
        {
            print("above right");
            vertices.Add(rightAnchors[2]);
            vertices.Add(rightAnchors[3]);
            vertices.Add(rightAnchors[2].normalized * gameSize);
        }
        if (transform.position.y < leftAnchors[1].y)
        {
            print("below left");
            vertices.Add(leftAnchors[1]);
            vertices.Add(leftAnchors[0]);
            vertices.Add(leftAnchors[1].normalized * gameSize);
        }
        if (transform.position.y > leftAnchors[3].y)
        {
            print("above left");
            vertices.Add(leftAnchors[2]);
            vertices.Add(leftAnchors[3]);
            vertices.Add(leftAnchors[3].normalized * gameSize);
        }
        for (int i = 0; i < vertices.Count; i++)
        {
            triangles.Add(i);
            normals.Add(norm);
        }

        lightingMesh.Clear();
        lightingMesh.vertices = vertices.ToArray();
        lightingMesh.triangles = triangles.ToArray();
        lightingMesh.normals = normals.ToArray();
        lightingMesh.RecalculateNormals();
    }

    void CalculateLightingGeneral()
    {
        nextTimeToCalculate = Time.time + 1f / calculationRate;
        List<EndPoint> endpoints = new List<EndPoint>();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 10 * Camera.main.orthographicSize, Vector3.forward, Mathf.Infinity, ~(LayerMask.GetMask("Player")));
        // print(hits.Length);

        // Get the end points for all colliders in range
        for (int i = 0; i < hits.Length; i++)
        {
            Bounds bounds = hits[i].collider.bounds;
            // print("box collider at " + hits[i].transform.name + " " + hits[i].transform.position + " " + bounds.min + " " + bounds.max);
            Vector3[] corners = GetColliderCorners(hits[i].transform.gameObject.GetComponent<BoxCollider2D>());
            endpoints.Add(new EndPoint() { transform = hits[i].transform, position = corners[0], angle = SignedAngleBetween(Vector3.up, corners[0], -Vector3.forward) });
            endpoints.Add(new EndPoint() { transform = hits[i].transform, position = corners[1], angle = SignedAngleBetween(Vector3.up, corners[1], -Vector3.forward) });
            endpoints.Add(new EndPoint() { transform = hits[i].transform, position = corners[2], angle = SignedAngleBetween(Vector3.up, corners[2], -Vector3.forward) });
            endpoints.Add(new EndPoint() { transform = hits[i].transform, position = corners[3], angle = SignedAngleBetween(Vector3.up, corners[3], -Vector3.forward) });
        }

        // Sort the endpoints by their angle clockwise from Vector3.up
        MergeSort<EndPoint>.Sort(endpoints, SortEndpointsByAngle);

        // Merge endpoints
        List<EndPoint> mergedEndpoints = new List<EndPoint>();
        mergedEndpoints.Add(endpoints[0]);
        for (int i = 1; i < endpoints.Count; i++)
        {
            if (Mathf.Approximately(endpoints[i - 1].position.x, endpoints[i].position.x)
            && Mathf.Approximately(endpoints[i - 1].position.y, endpoints[i].position.y)) continue;
            mergedEndpoints.Add(endpoints[i]);
        }
        endpoints = mergedEndpoints;

        List<RayWall> walls = new List<RayWall>();
        Vector3 upPosition = Vector3.zero;
        Transform upTransform = endpoints[0].transform;
        int layerMask = ~(LayerMask.GetMask("Player"));
        float rayDist = 10 * Camera.main.orthographicSize;
        RaycastHit2D up = Physics2D.Raycast(transform.position, Vector3.up, rayDist, layerMask);
        if (up != null)
        {
            upPosition = up.point;
            upTransform = up.transform;
        }
        RayWall currentWall = new RayWall() { firstPoint = upPosition };
        EndPoint currentPoint = new EndPoint() { transform = upTransform, position = upPosition, angle = 360f };
        Transform currentTransform = upTransform;
        endpoints.Add(currentPoint);
        int counter = 0;
        print("-----------------------");
        for (int i = 0; i < endpoints.Count; i++)
        {
            // If we can't reach the endpoint (blocked by a closer wall) then skip
            print("Endpoint: " + endpoints[i].position + " " + endpoints[i].transform.name + " " + endpoints[i].angle);
            Vector3 rayDir = Quaternion.Euler(0, 0, endpoints[i].angle) * Vector3.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, endpoints[i].position - transform.position, Vector3.Distance(endpoints[i].position, transform.position), layerMask);
            if (hit.collider != null
                && hit.transform != endpoints[i].transform
                && (!Mathf.Approximately(hit.point.x, endpoints[i].position.x)
                || !Mathf.Approximately(hit.point.y, endpoints[i].position.y)))
            {
                print("Skipping");
                counter++;
                continue;
            }
            if (endpoints[i].transform != currentTransform)
            {
                float angleValA = Mathf.Min(360f - currentPoint.angle, currentPoint.angle);
                float angleDistanceA = Mathf.Max(180f - angleValA, angleValA);
                float angleValB = Mathf.Min(360f - endpoints[i].angle, endpoints[i].angle);
                float angleDistanceB = Mathf.Max(180f - angleValB, angleValB);
                print("Different transforms: " + endpoints[i].transform.name + " " + currentTransform.name + " " + angleDistanceA + " " + angleDistanceB);
                currentWall.secondPoint = endpoints[i].position;

                currentTransform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                RaycastHit2D currentHit = Physics2D.Raycast(transform.position, currentWall.firstPoint - transform.position, rayDist, layerMask);
                currentTransform.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                endpoints[i].transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                RaycastHit2D newHit = Physics2D.Raycast(transform.position, endpoints[i].position - transform.position, rayDist, layerMask);
                endpoints[i].transform.gameObject.GetComponent<BoxCollider2D>().enabled = true;

                if (Mathf.Approximately(angleDistanceA, angleDistanceB))
                {
                    print("Same angle");
                    if (currentHit.collider != null) currentWall.firstPoint = currentHit.point;
                    if (newHit.collider != null) currentWall.secondPoint = newHit.point;
                }
                else if (angleDistanceA < angleDistanceB)
                {
                    print("angle a is less");
                    Transform newTransform = endpoints[i].transform;
                    if (newHit.collider != null)
                    {
                        newTransform = newHit.transform;
                        currentWall.secondPoint = newHit.point;
                    }

                    if (newTransform != currentTransform)
                    {
                        if (currentHit.collider != null) currentWall.firstPoint = currentHit.point;
                    }
                }
                else if (angleDistanceA > angleDistanceB)
                {
                    print("angle b is less");
                    Transform newTransform = currentTransform;
                    if (currentHit.collider != null)
                    {
                        currentWall.firstPoint = currentHit.point;
                        newTransform = currentHit.transform;
                    }
                    if (newTransform != endpoints[i].transform)
                    {
                        if (newHit.collider != null) currentWall.secondPoint = newHit.point;
                    }
                }
                walls.Add(currentWall);
                print("Creating a wall between different transforms " + currentWall.firstPoint + " " + currentWall.secondPoint);
                currentWall = new RayWall() { firstPoint = endpoints[i].position };
                currentTransform = endpoints[i].transform;
                currentPoint = endpoints[i];
            }
            else
            {
                print("Same transforms: " + endpoints[i].transform.name + " " + currentTransform.name);
                currentWall.secondPoint = endpoints[i].position;
                walls.Add(currentWall);
                print("Creating a wall in the same transform " + currentWall.firstPoint + " " + currentWall.secondPoint);
                currentWall = new RayWall() { firstPoint = endpoints[i].position };
                currentTransform = endpoints[i].transform;
                currentPoint = endpoints[i];
            }
        }
        print("We skipped " + counter + " endpoints");

        Vector3[] vertices = new Vector3[1 + 2 * walls.Count];
        vertices[0] = transform.position;
        int[] triangles = new int[3 * walls.Count];
        Vector3 norm = new Vector3(0, 0, 1);
        Vector3[] normals = new Vector3[1 + 2 * walls.Count];
        normals[0] = new Vector3(0, 0, 1);
        for (int i = 0; i < walls.Count; i++)
        {
            vertices[1 + 2 * i] = walls[i].firstPoint;
            vertices[1 + 2 * i + 1] = walls[i].secondPoint;
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = 1 + 2 * i;
            triangles[3 * i + 2] = 1 + 2 * i + 1;
            normals[2 * i] = new Vector3(0, 0, 1);
            normals[2 * i + 1] = new Vector3(0, 0, 1);
        }
        lightingMesh.Clear();
        lightingMesh.vertices = vertices;
        lightingMesh.triangles = triangles;
        lightingMesh.normals = normals;
        lightingMesh.RecalculateNormals();
    }

    static int SortEndpointsByAngle(EndPoint a, EndPoint b)
    {
        if (a.angle > b.angle) return 1;
        if (a.angle < b.angle) return -1;
        return 0;
    }
}
