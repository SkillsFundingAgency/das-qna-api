# Define root path (e.g., artifacts from pipeline)
$rootPath = "C:/Source/Repos/SFA/das-qna-api/src/SFA.DAS.QnA.Database/projects"

# Create table to send to SQL
$table = New-Object System.Data.DataTable
$table.Columns.Add("RelativeFilePath", [string])
$table.Columns.Add("JsonContent", [string])

# Recursively load all .json files
Get-ChildItem $rootPath -Recurse -Filter *.json | ForEach-Object {
    $relativePath = $_.FullName.Substring($rootPath.Length).TrimStart('\','/')
    $json = Get-Content $_.FullName -Raw

    $row = $table.NewRow()
    $row.RelativeFilePath = $relativePath
    $row.JsonContent = $json
    $table.Rows.Add($row)
}

$server = "HAUMEA\MSSQLSERVER2022"
$user = "Chris"
$pass = "Password01"
$db = "SFA.DAS.QnA.Database"

# Connection string
$connectionString = "Server=$server;Database=$db;User Id=$user;Password=$pass"

# Create SQL connection and command
$connection = New-Object System.Data.SqlClient.SqlConnection $connectionString
$command = $connection.CreateCommand()
$command.CommandText = "dbo.LoadProjectsJsonFiles"
$command.CommandType = [System.Data.CommandType]::StoredProcedure

# Add TVP parameter
$param = $command.Parameters.Add("@Files", [System.Data.SqlDbType]::Structured)
$param.TypeName = "dbo.JsonFileTable"
$param.Value = $table

# Execute
$connection.Open()
$command.ExecuteNonQuery()
$connection.Close()
