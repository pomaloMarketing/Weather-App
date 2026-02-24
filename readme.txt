===========================================================
NWS WEATHER DATA IMPLEMENTATION TOOL
Author: [Andrew Shelton]
Date: February 2026
===========================================================

--- PROJECT OVERVIEW ---
This is a .NET Console Application designed to demonstrate 
the consumption of the National Weather Service (NWS) API.
The tool allows users to browse local weather zones by state
and retrieve real-time, detailed forecasts.

--- TECHNICAL IMPLEMENTATION ---
1. Language/Framework: C# / .NET
2. Dependencies: Newtonsoft.Json (High-performance JSON parsing)
3. Architecture: 
   - Step 1: Query zones by State Code (Area-based filtering).
   - Step 2: Extract Zone ID and Name for user selection.
   - Step 3: Query the specific Zone Forecast endpoint for detailed text.

--- KEY ENGINEERING DECISIONS ---

* DATA NORMALIZATION:
  The API requires uppercase state codes (e.g., "CO"). The app 
  automatically sanitizes user input to ensure request success 
  regardless of how the user types the code.

* API COMPLIANCE (User-Agent):
  Implemented custom HttpClient headers. The NWS API rejects 
  anonymous traffic - email added; the app identifies itself via a professional 
  User-Agent string to ensure reliable data fetching.

* DEFENSIVE PROGRAMMING:
  Used C# Null-Conditional operators (?. and ??) throughout the 
  JSON navigation logic. This prevents application crashes if 
  the API returns incomplete data or empty forecast periods.

* UI/UX DESIGN:
  Utilized string interpolation and fixed-width padding to 
  create a readable "table" layout within the terminal, 
  ensuring data is scannable for the end-user.

--- HOW TO RUN ---
1. Ensure .NET SDK is installed.
2. Open terminal in the project root.
3. Run command: dotnet run
4. Enter a 2-letter state code when prompted.
5. Select a local entity zone when prompted
===========================================================