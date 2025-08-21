using UnityEngine;

public class DontDestroyOnLoadObject : MonoBehaviour
{
    private void Awake()
    {
        // Make sure this object is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
}
