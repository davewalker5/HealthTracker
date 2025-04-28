using System.Text.Json;

// Tool to create the payload for a data import job based on an existing file
const string FilePath = @"glucose.csv";
const string JobName = @"Blood Glucose Import";

// Construct the payload
dynamic data = new{ Content = File.ReadAllText(FilePath), JobName = JobName };
var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
var json = JsonSerializer.Serialize(data, options);

// Write the result to a JSON file
File.WriteAllText("payload.json", json);
