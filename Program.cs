using System; // Standard tools (like printing text to the screen).
using System.Net.Http; // Tools for talking to the internet (HTTP requests).
using System.Threading.Tasks; // Tools for "Async" work (letting the app wait for data without freezing).
using Newtonsoft.Json; // The external library that turns messy API text into readable data.

// ===========================================================================
// NATIONAL WEATHER SERVICE - IMPLEMENTATION ASSIGNMENT
// Purpose: Demonstrate API consumption, JSON parsing, and CLI formatting.
// ===========================================================================

//running dotnet takes program.cs and turns it into intermediate language for universal code



// 1. Setup the "Messenger" (HttpClient)
// The 'using' keyword ensures the connection is closed properly once the app finishes.
using HttpClient client = new HttpClient();

// The NWS API requires a 'User-Agent' header. Without this, the request is rejected with a 403 error.
// This identifies your app to their servers—a common requirement in professional implementations. Using email to identify in case of issue
client.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherAppAssignment/1.0 (a.shelton735@gmail.com)");

//equivalent of console.log. This outputs to terminal
Console.WriteLine("=== NATIONAL WEATHER SERVICE DATA TOOL ===");

// 2. Requirement: User Input for State
// We ask for a state code to filter the results from the NWS database.
Console.Write("\nEnter a State Code (e.g., CO, NY, FL, MI): ");

// Read input, convert to uppercase (co -> CO), and default to "CO" if input is empty.
// Lowercase will be rejected/ API needs uppercase 
// Conditional - if state is not defined, default to CO
string state = Console.ReadLine()?.ToUpper() ?? "CO";

// We wrap the logic in a try/catch block to handle internet outages or API errors gracefully.
try 
{
    // 3. Requirement: Retrieve List of Local Entities
    // We build a URL to fetch "public" zones for the chosen state. 
    Console.WriteLine($"\nFetching zones for {state}...");
    string zoneUrl = $"https://api.weather.gov/zones?area={state}&type=public";
    
    // 'await' tells the app to wait for the data to download before moving to the next line.
    string zoneJson = await client.GetStringAsync(zoneUrl);
    
    // 'JsonConvert' turns the raw text string into a dynamic object we can navigate like a folder structure.
    dynamic zoneData = JsonConvert.DeserializeObject(zoneJson);

    Console.WriteLine("\nLOCAL ENTITIES (Zones) - First 10:");
    // visual divider only
    Console.WriteLine("----------------------------------");
    
    // We loop through the first 10 zones returned to provide the user with specific options.
    for (int i = 0; i < 10; i++)
    {
        // The '?' (null-conditional) prevents a crash if the API returns fewer than 10 results.
        // properties has @id /zones/forecast/{state}. 
        // props.id is the key to narrowing done filter to city/ county
        // props.id provides the unique Zone Code (e.g., COZ039).
        // props.name provides the human-readable region name (e.g., Boulder).
        // We print these so the user knows which ID to type in for the next step.

        var props = zoneData?.features[i]?.properties;
        if (props != null) 
        {
            // Prints the Zone ID (used for the next API call) and the name (for the user).
            // First call gets state info -> second gets city/ county info

            // string interpolation - $ means code variables inside
            // props.id gets zone id
            // props.name get city name
            // hyphen is just visual helper

            Console.WriteLine($"[{props.id}] - {props.name}");
        }
    }

    // 4. Requirement: Detailed Forecast based on Selection
    Console.Write("\nSelect a Zone ID from the list (e.g., COZ039): ");
    string selectedId = Console.ReadLine()?.Trim() ?? "";

    // We now hit a different endpoint specifically for the forecast of the chosen ID.
    //https://api.weather.gov/zones/public/NYZ001/forecast

    string forecastUrl = $"https://api.weather.gov/zones/public/{selectedId}/forecast";
    string forecastJson = await client.GetStringAsync(forecastUrl);
    dynamic forecastData = JsonConvert.DeserializeObject(forecastJson);

    // 5. Requirement: Nicely Formatted Output (Highs/Lows)
    // Defensive check: If the API doesn't have "periods" (days), we check for a general text block.
    if (forecastData?.properties?.periods == null)
    {
        Console.WriteLine("\nNote: This zone does not provide a period-based forecast.");
        // Fallback: If 'periods' is missing, try to show the general 'forecast' field.
        Console.WriteLine($"Forecast: {forecastData?.properties?.detailedForecast ?? "No data available."}");
    }
    else
{
        Console.WriteLine("\n--- DETAILED FORECAST ---");
        
        // We loop through each "period" (Tonight, Tomorrow, etc.)
        foreach (var period in forecastData.properties.periods)
        {
            // 1. Get the name (e.g., "Tonight")
            string name = period.name;
            
            // 2. Get that long text summary 
            string detailed = period.detailedForecast;

            // 3. Print it to the screen
            Console.WriteLine($"\n[{name.ToUpper()}]");
            Console.WriteLine(detailed);
            Console.WriteLine(new string('-', 30)); 
        }
    }
}
catch (Exception ex)
{
    // If the API is down or the ID is invalid, we catch the error here instead of crashing.
    Console.WriteLine($"\nImplementation Error: {ex.Message}");
    Console.WriteLine("Tip: Ensure you entered a valid Zone ID (State abbreviation) and have an internet connection.");
}

Console.WriteLine("\nProcess complete. Press any key to exit.");
Console.ReadKey();