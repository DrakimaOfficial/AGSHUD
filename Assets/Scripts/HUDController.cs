using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    // References to UI elements
    private VisualElement root;
    private Label navModeLabel, coreLabel, taskLabel, waypointsLabel, threatLabel, heatMapLabel, subtitlesLabel, expressionLabel;
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
        powerMeter = root.Q<VisualElement>("PowerMeter");
        moodDial = root.Q<VisualElement>("MoodDial");
        skillsButton = root.Q<Button>("SkillsButton");
        mapButton = root.Q<Button>("MapButton");

        // Debug logs to confirm queries
        Debug.Log("PowerMeter found: " + (powerMeter != null));
        Debug.Log("SubtitlesLabel found: " + (subtitlesLabel != null));
        Debug.Log("MoodDial found: " + (moodDial != null));

        // Register button click events
        if (skillsButton != null) skillsButton.RegisterCallback<ClickEvent>(OnSkillsButtonClicked);
        if (mapButton != null) mapButton.RegisterCallback<ClickEvent>(OnMapButtonClicked);

        isInitialized = true; // Mark as initialized
        InitializeHUD();
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
        if (subtitlesLabel != null) subtitlesLabel.text = testDialogue;
        if (expressionLabel != null) expressionLabel.text = "Mood: " + testMood;
        UpdatePowerMeter(testPowerPercentage);
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
    }

    public void UpdateMoodDial(string mood)
    {
        if (moodDial == null || expressionLabel == null)
        {
            Debug.LogWarning("MoodDial or ExpressionLabel is null. Cannot update.");
            return;
        }

        Color moodColor = mood.ToLower() switch
        {
            "playful" => new Color(1f, 0f, 1f, 0.5f),
            "happy" => new Color(0f, 1f, 0f, 0.5f),
            "sad" => new Color(0f, 0f, 1f, 0.5f),
            _ => new Color(1f, 1f, 1f, 0.5f)
        };
        moodDial.style.backgroundColor = new StyleColor(moodColor);
        expressionLabel.text = $"Mood: {mood}";
    }

    public void UpdateDialogue(string text)
    {
        if (subtitlesLabel == null)
        {
            Debug.LogWarning("SubtitlesLabel is null. Cannot update dialogue.");
            return;
        }

        subtitlesLabel.text = text;
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

    // Inspector button methods for testing
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

    // Update method called when test fields change
    private void OnValidate()
    {
        if (!isInitialized) return; // Skip updates until initialized

        UpdatePowerMeter(testPowerPercentage);
        UpdateMoodDial(testMood);
        UpdateDialogue(testDialogue);
    }
}