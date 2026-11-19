TYT — TrackYourTime
Product Requirements Document (PRD)
Document Information
Details
Product Name
TrackYourTime (TYT)
Document Type
Product Requirements Document
Version
1.0
Status
Draft
Platform
Windows Desktop
Product Category
Personal Computer Activity Tracking


1. Product Overview
1.1 Introduction
TrackYourTime (TYT) is a windows desktop application designed to help uses to understand how they spend their time on their computers. 
The application monitors active application usage and user inactivity in the background, collecting activity data and transforming it into meaningful insights. 
TYT provides users with an overview of their computer usage through daily and weekly statistics, application usage breakdowns, and activity timelines. 
All data is stored locally on the user’s device.
1.2 Problem Statement
People spend a significant portion of their day using computers, but often have little understanding of how that time is distributed across different applications and activities.
Operating systems provide some usage information, but it is not always sufficient for users who want detailed historical insights into their computer activity.
Users may want to answer questions such as:
How many hours did I actively spend on my computer today?
Which applications consume most of my time?
How much time do I spend programming compared to browsing or entertainment?
When am I most active during the day?
How much of my computer usage is actual activity versus idle time?
1.4 Product Objectives
The primary objectives of TYT are to:
Automatically track application usage without requiring manual input.
Measure active computer usage separately from idle time.
Provide historical usage statistics and visual insights.
Help users understand how their time is distributed across applications.
Operate efficiently in the background without significantly affecting system performance.
Keep user activity data private and stored locally.
2. Target Audience
2.1 Primary Users
TYT is primarily designed for individuals who use Windows computers regularly and want to better understand their digital habits.
Examples include:
Software developers who want to understand how they distribute their working time.
Students who want to monitor their computer usage while studying.
Remote workers who want visibility into their daily computer activity.
Individuals interested in productivity and digital wellbeing.









3. Product Scope
3.2 Future Features
The following features are outside the initial MVP scope but may be considered for future releases.
Website and browser tab tracking.
Automatic application categorization.
Custom application categories.
Productivity scoring.
Monthly and yearly reports.
Data export (CSV/JSON).
Advanced usage analytics.
Custom tracking schedules.
Application-specific usage goals.
Data synchronization across devices.
3.3 Out of Scope
The following capabilities will not be part of TYT's intended functionality:
Screenshot capturing.
Keystroke logging.
Recording user input or message content.
Employee surveillance or remote monitoring.
Collecting personal activity data on external servers.
Monitoring other users without their knowledge.

4. Functional Requirements
Functional requirements describe what the application must do.
FR-01: Application Detection
The system shall:
Detect the currently active foreground application.
Identify the associated process.
Retrieve the application name and executable information where available.
Detect changes when the user switches between applications.
Avoid recording duplicate activity events when the foreground application has not changed.
FR-02: Active Usage Tracking
The system shall:
Track the duration for which each application remains in the foreground.
Calculate total active usage time.
Associate recorded durations with the corresponding application.
Maintain accurate time measurements across application switches.
Avoid counting the same time interval more than once.
FR-03: Idle Detection
The system shall:
Detect periods of user inactivity.
Use operating system input activity information to determine inactivity.
Apply a configurable idle threshold.
Stop counting foreground application time as active usage once the idle threshold is reached.
Resume active tracking when user activity is detected again.
FR-04: Usage Statistics
The system shall display:
Total active computer usage.
Total idle time.
Time spent in individual applications.
The most frequently used applications.
Usage distribution across the selected period.
FR-05: Daily and Weekly Reports
The system shall:
Provide a daily usage overview.
Provide a weekly usage overview.
Allow users to review previous days.
Aggregate application usage over the selected period.
Present usage information through visual charts and summaries.
FR-06: Activity Timeline
The system shall:
Display a chronological representation of recorded application activity.
Show when an application became active.
Show when the application stopped being active.
Represent idle periods separately.
Allow users to inspect activity throughout a selected day.
FR-07: Local Data Storage
The system shall:
Store tracking records in a local database.
Preserve historical activity between application restarts.
Retrieve historical records for reporting.
Avoid requiring an online account or cloud connection.
Provide a mechanism for users to delete their stored activity data.
FR-08: Background Operation
The system shall:
Allow activity tracking to continue when the main dashboard is closed.
Support minimizing to the system tray.
Provide an option to launch tracking automatically when Windows starts.
Allow users to pause and resume tracking.






5. Non-Functional Requirements
Non-functional requirements describe the expected quality and behavior of the system.
NFR-01: Performance
Background tracking should consume minimal CPU and memory resources.
Activity detection should not noticeably affect normal computer usage.
The dashboard should load historical statistics efficiently.
NFR-02: Accuracy
Application usage durations must be calculated consistently.
Idle periods must not be counted as active application usage.
Application switches must not result in overlapping tracking intervals.
System sleep and shutdown events must not create incorrect usage durations.
NFR-03: Privacy
Activity records must remain on the user's local device.
No screenshots or keystrokes shall be collected.
No personal activity information shall be transmitted to external services.
Tracking must be visible and controllable by the user.
NFR-04: Reliability
Tracking should recover correctly after application restarts.
Previously recorded activity must remain available.
Unexpected termination must not corrupt existing tracking records.
The system must handle missing or unavailable application information gracefully.
NFR-05: Usability
The interface should be intuitive and easy to navigate.
Usage statistics should be understandable without technical knowledge.
Users should be able to access the main usage overview quickly.
NFR-06: Compatibility
The initial release shall target Windows 10 and Windows 11.
The application shall support common desktop resolutions.
Tracking functionality shall work without requiring administrator privileges where technically possible.
8. Success Criteria
The MVP will be considered successful when:
TYT can reliably detect foreground application changes.
Application usage durations are calculated without overlapping intervals.
Idle time is correctly distinguished from active usage.
Tracking continues when the dashboard is closed.
Activity data persists across application restarts.
Daily and weekly statistics reflect the stored activity records.
The activity timeline represents recorded sessions chronologically.
Users can pause tracking and delete their local data.
The application operates without requiring an internet connection.

9. Assumptions and Constraints
Assumptions
The initial target platform is Windows.
The application is designed for a single local user.
Users have permission to run the application on their computers.
Windows provides the necessary APIs to detect foreground applications and user inactivity.
SQLite is a suitable initial database for local activity records.
Constraints
Windows-specific APIs may require native integration with the desktop application.
Background tracking must work independently of the dashboard lifecycle.
Browser website tracking is not included in the initial version.
Application identification may be incomplete for certain protected or system processes.
Tracking accuracy may be affected by sleep, hibernation, system clock changes, and unexpected shutdowns.
10. Future Considerations
Potential future development areas include:
Browser integration for website-level tracking.
More advanced application categorization.
Custom productivity rules.
Detailed historical analytics.
Data export and backup functionality.
Cross-platform support.
These considerations will be evaluated after the initial MVP has been implemented and tested.

Document Status: Draft — Subject to refinement as technical research and development progress.

