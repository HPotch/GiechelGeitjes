using UnityEngine;
using UnityEngine.UI;

public class ImageColor : MonoBehaviour
{
    [SerializeField] private Color[] colours;
    [SerializeField] private float smooth = 10f;
    
    private Image _image;
    private RawImage _rawImage;
    
    private Color _targetColor;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rawImage = GetComponent<RawImage>();
        if (_image) _targetColor = _image.color;
        if (_rawImage) _targetColor = _rawImage.color;
    }

    public void SetColor(int i)
    {
        if (i >= colours.Length)
        {
            Debug.LogWarning($"No colour found at index: {i}, aborting.");
            return;
        }
        _targetColor = colours[i];
    }

    private void Update()
    {
        if (_image) _image.color = Color.Lerp(_image.color, _targetColor, Time.deltaTime * smooth);
        if (_rawImage) _rawImage.color = Color.Lerp(_rawImage.color, _targetColor, Time.deltaTime * smooth);
    }
}
