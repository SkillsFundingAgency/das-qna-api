param(
    [string]$RootPath,
    [string]$SqlServer,
    [string]$SqlDatabase,
    [string]$SqlUsername,
    [string]$SqlPassword
)

# Create a table to hold all the .json files names and their contents
$table = New-Object System.Data.DataTable
$table.Columns.Add("RelativeFilePath", [string])
$table.Columns.Add("JsonContent", [string])

# Recursively load all .json files from the RootPath into the table
Get-ChildItem $RootPath -Recurse -Filter *.json | ForEach-Object {
    $relativePath = $_.FullName.Substring($RootPath.Length).TrimStart('\','/')
    $json = Get-Content $_.FullName -Raw

    $row = $table.NewRow()
    $row.RelativeFilePath = $relativePath
    $row.JsonContent = $json
    $table.Rows.Add($row)
}

$connectionString = "Server=$SqlServer;Database=$SqlDatabase;User Id=$SqlUsername;Password=$SqlPassword"

# Using a SQL connection and command to call LoadProjects
$connection = New-Object System.Data.SqlClient.SqlConnection $connectionString
$command = $connection.CreateCommand()
$command.CommandText = "dbo.LoadProjects"
$command.CommandType = [System.Data.CommandType]::StoredProcedure

# Pass .json files as a TVP parameter
$param = $command.Parameters.Add("@JsonFiles", [System.Data.SqlDbType]::Structured)
$param.TypeName = "dbo.JsonFileTable"
$param.Value = $table

$projectPathParam = $command.Parameters.Add("@ProjectPath", [System.Data.SqlDbType]::NVarChar, 4000)
$projectPathParam.Value = [System.DBNull]::Value

# Execute LoadProjects
$connection.Open()
$command.ExecuteNonQuery()
$connection.Close()
