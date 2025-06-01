using UnityEngine;

public class AudioObjects : MonoBehaviour
{

    public static AudioObjects instance { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public GameObject machineDialysis { get; private set; }
    [field: SerializeField] public GameObject airconditioner { get; private set; }
    [field: SerializeField] public GameObject window { get; private set; }
    [field: SerializeField] public GameObject radio { get; private set; }
    [field: SerializeField] public GameObject machineEKG { get; private set; }
}
