using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dynamically updates a Grid Layout Group to adapt between mobile screen heights and wide displays.
/// Handles responsive row/column stacking cleanly.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGridLayout : MonoBehaviour
{
    [Header("Grid Size Constraints")]
    public Vector2 cardSize = new Vector2(400f, 220f); // Width and Height of an individual difficulty card
    public Vector2 spacing = new Vector2(20f, 20f);

    private GridLayoutGroup _gridLayout;
    private RectTransform _rectTransform;
    private float _lastWidth;

    void Awake()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float currentWidth = _rectTransform.rect.width;

        // Only trigger layout shifts if the container width physically alters
        if (Mathf.Approximately(currentWidth, _lastWidth)) return;
        _lastWidth = currentWidth;

        UpdateLayoutStructure(currentWidth);
    }

    void UpdateLayoutStructure(float containerWidth)
    {
        if (_gridLayout == null) return;

        _gridLayout.cellSize = cardSize;
        _gridLayout.spacing = spacing;

        // Calculate if all three cards can fit horizontally side-by-side with padding margins
        float totalRequiredHorizontalWidth = (cardSize.x * 3f) + (spacing.x * 2f);

        if (containerWidth < totalRequiredHorizontalWidth)
        {
            // PHONE/PORTRAIT STATE: Force vertical single-column stacking layout rows
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = 1;
        }
        else
        {
            // WIDE SCREEN STATE: Force a horizontal row layout configuration
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _gridLayout.constraintCount = 1;
        }
    }
}