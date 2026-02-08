// ============================================================
// FILE: VRInputManager.cs
// PURPOSE: Handles Meta Quest 2 controller input for the VR
//          phobia management system. Manages button detection,
//          menu navigation, and session control via controllers.
// 
// AUTHOR: DulakshiniDharmarathne
// DATE CREATED: 08 February 2026
// LAST MODIFIED: 08 February 2026
// 
// DEPENDENCIES:
//   - UnityEngine
//   - Meta XR SDK (for OVRInput) - Optional, has fallback
//   - LevelManager.cs
//   - SessionManager.cs
// 
// NOTE: This script works with or without Meta XR SDK.
//       When SDK is not installed, keyboard controls work for testing.
//       When SDK is installed, VR controller input works.
// 
// CONTROLLER MAPPING (when SDK installed):
//   - A Button: Confirm / Select
//   - B Button: Back / Exit to Menu
//   - X Button: Previous Level (during session)
//   - Y Button: Next Level (during session)
//   - Right Trigger: UI Click
//   - Right Thumbstick Press: Toggle Pause
// 
// KEYBOARD MAPPING (for testing without VR):
//   - Space: Confirm / Select
//   - Escape: Back / Exit to Menu
//   - Left Arrow: Previous Level
//   - Right Arrow: Next Level
//   - P: Toggle Pause
//   - 1, 2, 3, 4: Start session at that level
// ============================================================

using UnityEngine;
using UnityEngine.Events;

public class VRInputManager : MonoBehaviour
{
    // ========================================
    // INSPECTOR VARIABLES
    // ========================================

    [Header("Manager References")]
    [Tooltip("Reference to the LevelManager script")]
    public LevelManager levelManager;

    [Tooltip("Reference to the SessionManager script")]
    public SessionManager sessionManager;

    [Header("Input Settings")]
    [Tooltip("Enable vibration feedback on button press")]
    public bool enableHapticFeedback = true;

    [Tooltip("Haptic feedback duration in seconds")]
    [Range(0.01f, 0.5f)]
    public float hapticDuration = 0.1f;

    [Tooltip("Haptic feedback strength")]
    [Range(0f, 1f)]
    public float hapticStrength = 0.5f;

    [Header("Debug")]
    [Tooltip("Show input debug messages in console")]
    public bool debugMode = false;

    [Header("Events")]
    [Tooltip("Called when A button (or Space key) is pressed")]
    public UnityEvent onAButtonPressed;

    [Tooltip("Called when B button (or Escape key) is pressed")]
    public UnityEvent onBButtonPressed;

    [Tooltip("Called when trigger (or mouse click) is pressed")]
    public UnityEvent onTriggerPressed;

    // ========================================
    // PRIVATE VARIABLES
    // ========================================

    private bool ovrInputAvailable = false;

    // ========================================
    // UNITY LIFECYCLE METHODS
    // ========================================

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Check if OVRInput is available (Meta XR SDK installed)
        CheckOVRInputAvailability();
    }

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        // Try to find managers if not assigned
        FindManagers();

        if (ovrInputAvailable)
        {
            Debug.Log("VRInputManager: Initialized with Meta XR SDK support");
        }
        else
        {
            Debug.Log("VRInputManager: Initialized in keyboard-only mode (Meta XR SDK not found)");
            Debug.Log("VRInputManager: Use keyboard for testing - Space=Select, Escape=Back, 1-4=Levels");
        }
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    private void Update()
    {
        // Process VR controller input if available
        if (ovrInputAvailable)
        {
            ProcessOVRControllerInput();
        }

        // Always process keyboard input (for editor testing)
        ProcessKeyboardInput();
    }

    // ========================================
    // OVR INPUT AVAILABILITY CHECK
    // ========================================

    /// <summary>
    /// Checks if OVRInput (Meta XR SDK) is available.
    /// </summary>
    private void CheckOVRInputAvailability()
    {
        // Try to find OVRInput type using reflection
        System.Type ovrInputType = System.Type.GetType("OVRInput, Oculus.VR");

        if (ovrInputType != null)
        {
            ovrInputAvailable = true;
        }
        else
        {
            // Also check assembly qualified name variations
            ovrInputType = System.Type.GetType("OVRInput, Meta.XR.Core");
            if (ovrInputType != null)
            {
                ovrInputAvailable = true;
            }
            else
            {
                ovrInputAvailable = false;
            }
        }
    }

    // ========================================
    // VR CONTROLLER INPUT PROCESSING
    // ========================================

    /// <summary>
    /// Processes VR controller input using OVRInput.
    /// This method only runs if Meta XR SDK is installed.
    /// </summary>
    private void ProcessOVRControllerInput()
    {
#if OCULUS_XR_AVAILABLE
        // A Button - Confirm / Select
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            OnConfirmPress();
            TriggerHapticPulse(true); // Right controller
        }
        
        // B Button - Back / Exit
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            OnBackPress();
            TriggerHapticPulse(true); // Right controller
        }
        
        // X Button - Previous Level
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            OnPreviousLevelPress();
            TriggerHapticPulse(false); // Left controller
        }
        
        // Y Button - Next Level
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            OnNextLevelPress();
            TriggerHapticPulse(false); // Left controller
        }
        
        // Right Trigger - UI Click
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            OnTriggerPress();
            TriggerHapticPulse(true); // Right controller
        }
        
        // Right Thumbstick Press - Toggle Pause
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            OnPausePress();
            TriggerHapticPulse(true); // Right controller
        }
        
        // Start Button - Menu
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            OnPausePress();
            TriggerHapticPulse(false); // Left controller
        }
#endif
    }

    /// <summary>
    /// Triggers haptic feedback on specified controller.
    /// </summary>
    /// <param name="rightController">True for right controller, false for left</param>
    private void TriggerHapticPulse(bool rightController)
    {
        if (!enableHapticFeedback) return;

#if OCULUS_XR_AVAILABLE
        OVRInput.Controller controller = rightController ? 
            OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;
        OVRInput.SetControllerVibration(hapticStrength, hapticStrength, controller);
        Invoke(nameof(StopHaptics), hapticDuration);
#endif
    }

    /// <summary>
    /// Stops haptic feedback on both controllers.
    /// </summary>
    private void StopHaptics()
    {
#if OCULUS_XR_AVAILABLE
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
#endif
    }

    // ========================================
    // KEYBOARD INPUT PROCESSING
    // ========================================

    /// <summary>
    /// Processes keyboard input for editor testing.
    /// </summary>
    private void ProcessKeyboardInput()
    {
        // Space = Confirm (like A button)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnConfirmPress();
        }

        // Escape = Back (like B button)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackPress();
        }

        // Left Arrow = Previous Level (like X button)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnPreviousLevelPress();
        }

        // Right Arrow = Next Level (like Y button)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnNextLevelPress();
        }

        // P = Toggle Pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnPausePress();
        }

        // Mouse click = Trigger
        if (Input.GetMouseButtonDown(0))
        {
            OnTriggerPress();
        }

        // Number keys for direct level selection
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartSessionAtLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartSessionAtLevel(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            StartSessionAtLevel(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartSessionAtLevel(4);
        }

        // Q = Quit application
        if (Input.GetKeyDown(KeyCode.Q) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            QuitApplication();
        }
    }

    // ========================================
    // INPUT ACTION HANDLERS
    // ========================================

    /// <summary>
    /// Called when confirm action is triggered (A button or Space).
    /// </summary>
    private void OnConfirmPress()
    {
        if (debugMode) Debug.Log("VRInputManager: Confirm pressed");

        // Invoke event for UI or other systems to respond
        onAButtonPressed?.Invoke();
    }

    /// <summary>
    /// Called when back action is triggered (B button or Escape).
    /// </summary>
    private void OnBackPress()
    {
        if (debugMode) Debug.Log("VRInputManager: Back pressed");

        // If in session, end it
        if (sessionManager != null && sessionManager.IsSessionActive)
        {
            sessionManager.EndSession();
        }

        // Invoke event
        onBButtonPressed?.Invoke();
    }

    /// <summary>
    /// Called when previous level action is triggered (X button or Left Arrow).
    /// </summary>
    private void OnPreviousLevelPress()
    {
        if (debugMode) Debug.Log("VRInputManager: Previous Level pressed");

        // Only change level if in active session and not paused
        if (sessionManager != null && sessionManager.IsSessionActive && !sessionManager.IsSessionPaused)
        {
            if (levelManager != null)
            {
                levelManager.PreviousLevel();
            }
        }
    }

    /// <summary>
    /// Called when next level action is triggered (Y button or Right Arrow).
    /// </summary>
    private void OnNextLevelPress()
    {
        if (debugMode) Debug.Log("VRInputManager: Next Level pressed");

        // Only change level if in active session and not paused
        if (sessionManager != null && sessionManager.IsSessionActive && !sessionManager.IsSessionPaused)
        {
            if (levelManager != null)
            {
                levelManager.NextLevel();
            }
        }
    }

    /// <summary>
    /// Called when trigger action is triggered (Trigger or Mouse Click).
    /// </summary>
    private void OnTriggerPress()
    {
        if (debugMode) Debug.Log("VRInputManager: Trigger pressed");

        // Invoke event
        onTriggerPressed?.Invoke();
    }

    /// <summary>
    /// Called when pause action is triggered (Thumbstick press or P key).
    /// </summary>
    private void OnPausePress()
    {
        if (debugMode) Debug.Log("VRInputManager: Pause pressed");

        // Toggle pause if in session
        if (sessionManager != null && sessionManager.IsSessionActive)
        {
            sessionManager.TogglePause();
        }
    }

    // ========================================
    // PUBLIC METHODS
    // ========================================

    /// <summary>
    /// Starts a session at the specified level.
    /// Can be called from UI buttons.
    /// </summary>
    /// <param name="level">Level to start (1-4)</param>
    public void StartSessionAtLevel(int level)
    {
        if (debugMode) Debug.Log("VRInputManager: Starting session at level " + level);

        if (sessionManager != null)
        {
            sessionManager.StartSession(level);
        }
        else
        {
            Debug.LogError("VRInputManager: SessionManager reference is null!");
        }
    }

    /// <summary>
    /// Ends the current session and returns to menu.
    /// Can be called from UI buttons.
    /// </summary>
    public void ReturnToMenu()
    {
        if (sessionManager != null)
        {
            sessionManager.EndSession();
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("VRInputManager: Quitting application");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Returns whether VR input (Meta XR SDK) is available.
    /// </summary>
    public bool IsVRInputAvailable()
    {
        return ovrInputAvailable;
    }

    // ========================================
    // HELPER METHODS
    // ========================================

    /// <summary>
    /// Attempts to find manager references if not assigned.
    /// </summary>
    private void FindManagers()
    {
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogWarning("VRInputManager: LevelManager not found in scene.");
            }
        }

        if (sessionManager == null)
        {
            sessionManager = FindObjectOfType<SessionManager>();
            if (sessionManager == null)
            {
                Debug.LogWarning("VRInputManager: SessionManager not found in scene.");
            }
        }
    }
}