// ============================================================
// FILE: LevelManager.cs
// PURPOSE: Controls the visibility of student groups based on 
//          the current exposure level in the VR classroom
// 
// AUTHOR: Dharmarathne I D D R
// DATE CREATED: January 15, 2025
// LAST MODIFIED: January 15, 2025
// 
// DEPENDENCIES:
//   - UnityEngine
//   - TMPro (TextMeshPro package)
// 
// PUBLIC VARIABLES (Set in Inspector):
//   - level2Group: GameObject containing 5 students
//   - level3Additional: GameObject containing 7 more students
//   - level4Additional: GameObject containing 8 more students
//   - levelText: TMP_Text for displaying level name
//   - studentCountText: TMP_Text for displaying student count
//   - levelChangeSound: AudioSource for level change feedback
// 
// USAGE:
//   1. Attach this script to a GameObject named "GameManager"
//   2. Assign all public references in Inspector
//   3. Call SetLevel(1-4) to change exposure levels
// 
// LEVELS:
//   Level 1: Empty classroom (0 students)
//   Level 2: Small group (5 students)
//   Level 3: Medium class (12 students)
//   Level 4: Full classroom (20 students)
// ============================================================

using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Student Groups")]
    public GameObject level2Group;
    public GameObject level3Additional;
    public GameObject level4Additional;

    [Header("UI References")]
    public TMP_Text levelText;
    public TMP_Text studentCountText;

    [Header("Audio")]
    public AudioSource levelChangeSound;

    private int currentLevel = 1;

    void Start()
    {
        SetLevel(1);
    }

    /// <summary>
    /// Sets the current exposure level and updates student visibility
    /// </summary>
    /// <param name="level">Level number (1-4)</param>
    public void SetLevel(int level)
    {
        currentLevel = level;

        // Hide all students first
        if (level2Group != null)
            level2Group.SetActive(false);
        if (level3Additional != null)
            level3Additional.SetActive(false);
        if (level4Additional != null)
            level4Additional.SetActive(false);

        // Show students based on level
        switch (level)
        {
            case 1:
                // Empty classroom - all hidden
                Debug.Log("Level 1: Empty Classroom");
                break;
            case 2:
                level2Group.SetActive(true);
                Debug.Log("Level 2: 5 Students");
                break;
            case 3:
                level2Group.SetActive(true);
                level3Additional.SetActive(true);
                Debug.Log("Level 3: 12 Students");
                break;
            case 4:
                level2Group.SetActive(true);
                level3Additional.SetActive(true);
                level4Additional.SetActive(true);
                Debug.Log("Level 4: 20 Students");
                break;
        }

        // Update UI
        UpdateUI();

        // Play sound
        if (levelChangeSound != null)
            levelChangeSound.Play();
    }

    /// <summary>
    /// Updates the UI text elements with current level info
    /// </summary>
    private void UpdateUI()
    {
        string[] levelNames = {
            "",
            "Empty Classroom",
            "Small Group",
            "Medium Class",
            "Full Classroom"
        };
        int[] studentCounts = { 0, 0, 5, 12, 20 };

        if (levelText != null)
            levelText.text = levelNames[currentLevel];

        if (studentCountText != null)
            studentCountText.text = "Students: " + studentCounts[currentLevel];
    }

    /// <summary>
    /// Returns the current level number
    /// </summary>
    /// <returns>Current level (1-4)</returns>
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}