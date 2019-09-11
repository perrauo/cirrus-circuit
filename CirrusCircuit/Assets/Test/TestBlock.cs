using UnityEngine;
using System.Collections;

public class TestBlock : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _gridPosition = new Vector3Int(0, 0, 0);

    [SerializeField]
    protected TestLevel _level;

    public void  OnValidate()
    {
        if (_level == null)
            _level = GetComponentInParent<TestLevel>();
    }


    // Use this for initialization
    void Start()
    {
        (transform.position, _gridPosition) = _level.RegisterBlock(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {

        }
    }


}
