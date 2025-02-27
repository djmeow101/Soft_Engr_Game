using UnityEngine;
using UnityEngine.InputSystem;
public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRadius = .5f;
    [SerializeField] private LayerMask _interactionMask;
    [SerializeField] private InteractionPrompltUI _interactionPromptUI; 
    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private IInteractable _interactable;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius,_colliders,_interactionMask);

        if(_numFound>0)
        {
            var _interactable = _colliders[0].GetComponent<IInteractable>();

            if(_interactable != null)
            {
                if(!_interactionPromptUI.IsDisplayed) _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                
                if(Keyboard.current.eKey.wasPressedThisFrame) _interactable.Interact(this);
                
            }
        }
        else
        {
            if (_interactable != null) _interactable = null;
            if(_interactionPromptUI.IsDisplayed) _interactionPromptUI.Close();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position,_interactionRadius);
    }

}
