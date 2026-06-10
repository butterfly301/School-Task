using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public GameObject optionPrefab;
    public GameObject[] OptionGroup;

    private Transform[] options;
    [Range(0, 5)] public int optionNum;
    private float halfNum;
    
    Dictionary<Transform, Vector3> optionPositions = new Dictionary<Transform, Vector3>();
    
    Vector3 center = Vector3.zero;
    private float R = 500f;
    [Range(1f,10f)]
    public float speed;

    private void Awake()
    {
        for (int i = 0; i < optionNum; i++)
        {
            GameObject go = GameObject.Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, OptionGroup[i].transform);
            go.name = i.ToString();
            
        }

        InitPos();
    }

    private void InitPos()
    {
        float angle = 0;
        for (int i = 0; i < optionNum; i++)
        {
            angle = (360.0f/(float)optionNum) * i * Mathf.Deg2Rad;
            
            float x = Mathf.Sin(angle) * R;
            float z = -Mathf.Cos(angle) * R;
            
            Vector3 temp = options[i].localPosition = new Vector3(x, 0, z);
            optionPositions.Add(options[i], temp);
        }
    }
}