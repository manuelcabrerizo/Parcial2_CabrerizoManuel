using UnityEngine;

public class Ring : Signable
{
    [SerializeField] private SoundClipsSO clips;
    [SerializeField] private MeshRenderer meshRenderer;
    private Collider collision;

    private bool isSignal;

    private void Awake()
    {
        isSignal = false;
        collision = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Dragon>(out _))
        {
            collision.enabled = false;
            meshRenderer.enabled = false;
            isSignal = true;
            AudioManager.onPlayClip3D(clips.onRingCatch, transform.position, 10, 40);
        }
    }

    public override bool IsSignal()
    {
        return isSignal;
    }
}
