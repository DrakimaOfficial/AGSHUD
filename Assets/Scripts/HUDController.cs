using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    // References to UI elements
    private VisualElement root;
    private Label navModeLabel, coreLabel, taskLabel, waypointsLabel, threatLabel, heatMapLabel, subtitlesLabel, expressionLabel, powerMeterLabel;
    private VisualElement powerMeter, moodDial;
    private Button skillsButton, mapButton;

    // Public fields for Inspector testing, editable in Play Mode
    [Header("Test Fields for Inspector (Editable in Play Mode)")]
    [SerializeField] private float testPowerPercentage = 86f;
    [SerializeField] private string testMood = "playful";
    [SerializeField] private string testDialogue = "Akari: Ready to make you smile, Alex!";

    private bool isInitialized = false; // Flag to track initialization

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root VisualElement not found. Ensure UIDocument component is set up correctly.");
            return;
        }

        navModeLabel = root.Q<Label>("NavMode");
        coreLabel = root.Q<Label>("CoreLabel");
        taskLabel = root.Q<Label>("CurrentTask");
        waypointsLabel = root.Q<Label>("Waypoints");
        threatLabel = root.Q<Label>("ThreatScan");
        heatMapLabel = root.Q<Label>("HeatMap");
        subtitlesLabel = root.Q<Label>("Subtitles");
        expressionLabel = root.Q<Label>("ExpressionLabel");
        powerMeterLabel = root.Q<Label>("PowerMeterLabel");
        powerMeter = root.Q<VisualElement>("PowerMeter");
        moodDial = root.Q<VisualElement>("MoodDial");
        skillsButton = root.Q<Button>("SkillsButton");
        mapButton = root.Q<Button>("MapButton");

        // Debug logs to confirm queries
        Debug.Log("PowerMeterLabel found: " + (powerMeterLabel != null));
        Debug.Log("PowerMeter found: " + (powerMeter != null));
        Debug.Log("SubtitlesLabel found: " + (subtitlesLabel != null));
        Debug.Log("MoodDial found: " + (moodDial != null));

        // Register button click and hover events
        if (skillsButton != null)
        {
            skillsButton.RegisterCallback<ClickEvent>(OnSkillsButtonClicked);
            skillsButton.RegisterCallback<MouseEnterEvent>(evt => OnButtonHover(skillsButton));
            skillsButton.RegisterCallback<MouseLeaveEvent>(evt => OnButtonLeave(skillsButton));
        }
        if (mapButton != null)
        {
            mapButton.RegisterCallback<ClickEvent>(OnMapButtonClicked);
            mapButton.RegisterCallback<MouseEnterEvent>(evt => OnButtonHover(mapButton));
            mapButton.RegisterCallback<MouseLeaveEvent>(evt => OnButtonLeave(mapButton));
        }

        isInitialized = true; // Mark as initialized
        InitializeHUD();

        // Start Power Meter pulsing animation
        StartPowerMeterPulse();
    }

    private void InitializeHUD()
    {
        if (!isInitialized) return;

        if (navModeLabel != null) navModeLabel.text = "NAV MODE: IDLE";
        if (coreLabel != null) coreLabel.text = "AKARI SYSTEM CORE v1.0";
        if (taskLabel != null) taskLabel.text = "Task: Escort Alex to Lab";
        if (waypointsLabel != null) waypointsLabel.text = "Waypoint: Lab Entrance (50m)";
        if (threatLabel != null) threatLabel.text = "Threats: None";
        if (heatMapLabel != null) heatMapLabel.text = "Temp: 22°C";
        if (subtitlesLabel != null)
        {
            subtitlesLabel.style.opacity = 0; // Start with invisible text
            UpdateDialogue(testDialogue); // This will trigger the fade-in
        }
        if (expressionLabel != null) expressionLabel.text = "Mood: " + testMood;
        if (moodDial != null)
        {
            // Set an initial color to avoid null/invalid color issues
            moodDial.style.backgroundColor = new StyleColor(new Color(1f, 0f, 1f, 0.5f)); // Playful color (magenta)
        }
        UpdatePowerMeter(testPowerPercentage);
    }

    private void StartPowerMeterPulse()
    {
        if (powerMeter == null) return;

        float pulseDuration = testPowerPercentage > 50 ? 1f : 0.5f; // Faster pulse when low
        powerMeter.style.scale = new StyleScale(new Vector2(1f, 1f)); // Reset scale

        // Animate scale using DOValue
        float scaleValue = 1f;
        DOTween.To(() => scaleValue, x => scaleValue = x, 1.1f, pulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                powerMeter.style.scale = new StyleScale(new Vector2(scaleValue, scaleValue));
            });
    }

    private void OnButtonHover(Button button)
    {
        if (button == null) return;

        // Stop any existing animation
        DOTween.Kill(button);

        // Get the current color (default to stylesheet color if not set)
        Color baseColor = button.resolvedStyle.backgroundColor;
        if (baseColor == Color.clear) // Fallback if no color is set
        {
            baseColor = new Color(128f / 255f, 0f, 255f / 255f, 0.5f); // rgba(128, 0, 255, 0.5)
        }

        // Animate the alpha to create a shimmer effect (0.5 to 0.8 and back)
        float alpha = baseColor.a;
        DOTween.To(() => alpha, x =>
        {
            alpha = x;
            button.style.backgroundColor = new StyleColor(new Color(baseColor.r, baseColor.g, baseColor.b, alpha));
        }, 0.8f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetId(button);
    }

    private void OnButtonLeave(Button button)
    {
        if (button == null) return;

        // Stop the shimmer animation
        DOTween.Kill(button);

        // Reset to the original color
        Color baseColor = button.resolvedStyle.backgroundColor;
        if (baseColor == Color.clear)
        {
            baseColor = new Color(128f / 255f, 0f, 255f / 255f, 0.5f);
        }
        button.style.backgroundColor = new StyleColor(new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f));
    }

    public void UpdatePowerMeter(float percentage)
    {
        if (powerMeter == null)
        {
            Debug.LogWarning("PowerMeter is null. Cannot update.");
            return;
        }

        percentage = Mathf.Clamp(percentage, 0f, 100f);
        powerMeter.style.width = new StyleLength(Length.Percent(percentage));
        powerMeter.style.backgroundColor = percentage > 50
            ? new StyleColor(new Color(1f, 0f, 1f, 0.5f))
            : new StyleColor(new Color(1f, 0f, 0f, 0.5f));

        // Update pulse speed based on new percentage
        DOTween.Kill(powerMeter); // Stop existing animation
        StartPowerMeterPulse(); // Restart with updated speed
    }

    public void UpdateMoodDial(string mood)
    {
        if (moodDial == null || expressionLabel == null)
        {
            Debug.LogWarning("MoodDial or ExpressionLabel is null. Cannot update.");
            return;
        }

        Color targetColor = mood.ToLower() switch
        {
            "playful" => new Color(1f, 0f, 1f, 0.5f),
            "happy" => new Color(0f, 1f, 0f, 0.5f),
            "sad" => new Color(0f, 0f, 1f, 0.5f),
            _ => new Color(1f, 1f, 1f, 0.5f)
        };

        // Get the current color, default to white if not set
        Color currentColor = moodDial.resolvedStyle.backgroundColor;
        if (currentColor == Color.clear)
        {
            currentColor = new Color(1f, 1f, 1f, 0.5f); // Default to white
        }

        // Animate the color transition
        float t = 0f;
        DOTween.To(() => t, x =>
        {
            t = x;
            Color lerpedColor = Color.Lerp(currentColor, targetColor, t);
            moodDial.style.backgroundColor = new StyleColor(lerpedColor);
        }, 1f, 0.5f)
            .SetEase(Ease.InOutSine);

        expressionLabel.text = $"Mood: {mood}";
    }

    public void UpdateDialogue(string text)
    {
        if (subtitlesLabel == null)
        {
            Debug.LogWarning("SubtitlesLabel is null. Cannot update dialogue.");
            return;
        }

        // Reset scale and opacity
        subtitlesLabel.style.scale = new StyleScale(new Vector2(1f, 1f));
        subtitlesLabel.style.opacity = 1;

        // Create a sequence for fade-out, text update, and fade-in with a subtle pulse
        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => subtitlesLabel.style.opacity.value, x => subtitlesLabel.style.opacity = x, 0f, 0.3f)) // Fade out
                .AppendCallback(() => subtitlesLabel.text = text) // Update text
                .Append(DOTween.To(() => subtitlesLabel.style.opacity.value, x => subtitlesLabel.style.opacity = x, 1f, 0.3f)); // Fade in

        // Add subtle scale pulse during fade-in
        float scaleValue = 1f;
        sequence.Join(DOTween.To(() => scaleValue, x =>
        {
            scaleValue = x;
            subtitlesLabel.style.scale = new StyleScale(new Vector2(scaleValue, scaleValue));
        }, 1.05f, 0.3f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));
    }

    private void OnSkillsButtonClicked(ClickEvent evt)
    {
        UpdateDialogue("Akari: Accessing my skills... I’m great at dancing, Alex!");
        UpdateMoodDial("happy");
    }

    private void OnMapButtonClicked(ClickEvent evt)
    {
        UpdateDialogue("Akari: Let me show you the map to the lab!");
        UpdateMoodDial("playful");
    }

    [ContextMenu("Test Power Meter")]
    public void TestPowerMeter()
    {
        UpdatePowerMeter(testPowerPercentage);
    }

    [ContextMenu("Test Mood Dial")]
    public void TestMoodDial()
    {
        UpdateMoodDial(testMood);
    }

    [ContextMenu("Test Dialogue")]
    public void TestDialogueUpdate()
    {
        UpdateDialogue(testDialogue);
    }

    private void OnValidate()
    {
        if (!isInitialized) return;

        UpdatePowerMeter(testPowerPercentage);
        UpdateMoodDial(testMood);
        UpdateDialogue(testDialogue);
    }
}