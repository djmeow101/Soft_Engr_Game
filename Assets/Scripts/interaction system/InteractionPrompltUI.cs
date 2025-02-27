using UnityEngine;
using TMPro;
public class InteractionPrompltUI : MonoBehaviour
{
    private Camera _mainCam;
    [SerializeField] private GameObject _uiPanal;
    [SerializeField] private TextMeshProUGUI _promptText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _mainCam = Camera.main;
        _uiPanal.SetActive(false);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        var rotation = _mainCam.transform.rotation;
        transform.LookAt(transform.position+rotation*Vector3.forward,rotation*Vector3.up);
    }
    public bool IsDisplayed = false;
    
    public void SetUp(string promptText)
    {
        _promptText.text = promptText;
        _uiPanal.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        _uiPanal.SetActive(false);
        IsDisplayed = false;
    }

}
