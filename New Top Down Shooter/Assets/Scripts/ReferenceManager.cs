using UnityEngine;

[CreateAssetMenu(fileName = "Reference Manager", menuName = "Scriptable Objects/Reference Manager")]
public class ReferenceManager : ScriptableObject
{
    public static ReferenceManager Instance
    {
        get
        {
            if (s_instance is null)
            {
                var loadedRefManager = Resources.Load("Reference Manager") as ReferenceManager;
                if (loadedRefManager is null) throw new System.NullReferenceException("No reference manager found in Resources/Reference Manager!");
                s_instance = loadedRefManager;
            }
            return s_instance;
        }
    }
    private static ReferenceManager s_instance;

    [field: SerializeField] public Bullet Bullet { get; private set; }
}