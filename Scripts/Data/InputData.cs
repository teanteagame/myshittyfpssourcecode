using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Inputs")]
public class InputData : ScriptableObject
{
    public List<InputKey> Keys = new List<InputKey>();

    public bool GetKey(string keyName)
    {
        return Input.GetKey(GetInputKey(keyName).keyCode);
    }

    public bool GetKeyDown(string keyName)
    {
        return Input.GetKeyDown(GetInputKey(keyName).keyCode);
    }

    public bool GetKeyUp(string keyName)
    {
        return Input.GetKeyUp(GetInputKey(keyName).keyCode);
    }

    InputKey GetInputKey(string name)
    {        
        for (int i = 0; i < Keys.Count; i++)
        {
            InputKey key = Keys[i]; 
            if (key.KeyName == name)
            {
                return key;
            }
        }

        return null;
    }
}

[System.Serializable]
public class InputKey
{
    public string KeyName;
    public KeyCode keyCode;
}