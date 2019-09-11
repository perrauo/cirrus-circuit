using UnityEngine;
using System.Collections;

public class TestChar : TestBlock
{

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _gridPosition += Vector3.forward;
            transform.position = _level.WorldToGrid(_gridPosition);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _gridPosition += Vector3.back;
            transform.position = _level.WorldToGrid(_gridPosition);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _gridPosition += Vector3.left;
            transform.position = _level.WorldToGrid(_gridPosition);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _gridPosition += Vector3.right;
            transform.position = _level.WorldToGrid(_gridPosition);
        }

        
    }

}
