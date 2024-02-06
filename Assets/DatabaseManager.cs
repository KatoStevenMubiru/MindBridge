using UnityEngine;
using SQLite4Unity3d;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection _connection;
    private string dbPath;

    void Awake()
    {
        Debug.Log("DatabaseManager Awake started.");

        // Set the database path
        dbPath = Application.persistentDataPath + "/MyDatabase.db";
        Debug.Log("Database path set to: " + dbPath);

        // Initialize the database connection
        Debug.Log("Initializing database connection...");
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // Create the table if it doesn't exist
        Debug.Log("Creating table if it doesn't exist...");
        _connection.CreateTable<Interaction>();

        // Check if the database and table were created successfully
        CheckDatabase();
    }

    public void LogInteraction(string objectName, string interactionType)
    {
        Debug.Log($"Logging interaction: ObjectName={objectName}, InteractionType={interactionType}");

        // Create a new interaction record
        var interaction = new Interaction
        {
            ObjectName = objectName,
            InteractionType = interactionType,
            Timestamp = System.DateTime.UtcNow
        };

        // Insert the record into the database
        Debug.Log("Inserting interaction record into the database...");
        _connection.Insert(interaction);
        Debug.Log("Record inserted successfully.");
    }

    void OnDestroy()
    {
        // Close the connection when the script is destroyed
        Debug.Log("Closing database connection...");
        _connection?.Close();
        Debug.Log("Database connection closed.");
    }

    // Method to check if the database and table exist
    private void CheckDatabase()
    {
        try
        {
            Debug.Log("Checking if the 'Interaction' table exists...");
            // Check if the 'interactions' table exists
            var tableInfo = _connection.GetTableInfo("Interaction");
            if (tableInfo.Any())
            {
                Debug.Log("The 'Interaction' table exists.");
            }
            else
            {
                Debug.LogWarning("The 'Interaction' table does not exist. It should have been created during Awake.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred while checking the database: " + ex.Message);
        }
    }
}

public class Interaction
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string ObjectName { get; set; }
    public string InteractionType { get; set; }
    public System.DateTime Timestamp { get; set; }
}