using UnityEngine;
using TMPro;
using System;

public class ShowDateTime : MonoBehaviour
{
    public TextMeshProUGUI dateTimeText; // Reference to the Text component

    // Boolean flags to toggle date components
    public bool showDate = true;
    public bool showDay = true;
    public bool showMonth = true;
    public bool showYear = true;

    // Boolean flags to toggle time components
    public bool showTime = true;
    public bool showHours = true;
    public bool showMinutes = true;
    public bool showSeconds = true;

    void Update()
    {
        DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = "";

        // Build the date string based on toggles
        if (showDate)
        {
            string formattedDate = "";

            if (showDay)
            {
                formattedDate += currentDateTime.ToString("dd") + " ";
            }

            if (showMonth)
            {
                formattedDate += currentDateTime.ToString("MMM") + " ";
            }

            if (showYear)
            {
                formattedDate += currentDateTime.ToString("yyyy");
            }

            // Remove trailing slash if present
            formattedDate = formattedDate.TrimEnd(' ');

            formattedDateTime += formattedDate;
        }

        // Build the time string based on toggles
        if (showTime)
        {
            string formattedTime = "";

            if (showHours)
            {
                formattedTime += currentDateTime.ToString("HH") + ":";
            }

            if (showMinutes)
            {
                formattedTime += currentDateTime.ToString("mm") + ":";
            }

            if (showSeconds)
            {
                formattedTime += currentDateTime.ToString("ss");
            }

            // Remove trailing colon if present
            formattedTime = formattedTime.TrimEnd(':');

            // Add space between date and time if both are shown
            if (showDate && showTime)
            {
                formattedDateTime += " ";
            }

            formattedDateTime += formattedTime;
        }

        // Set the final formatted text
        dateTimeText.text = formattedDateTime;
    }
}
