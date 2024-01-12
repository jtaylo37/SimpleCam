using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public GameObject prefab;

    // Start is called before the first frame update
    /*void Start()
    {
        Vector3 start = new Vector3(0, 0, -1);

        Instantiate(prefab, start, Quaternion.identity);
    }*/

    public void move(int x, int y, int z) 
    {
        Vector3 pos = new Vector3(x, y, z);
        transform.position = pos;
    }

}
