using System;
using System.Collections.Generic;
using System.Data.SQLite;

internal class DBClient
{
  private static string db_name;
  private string sql = "";
  private readonly List<Tuple<string, object>> parameters = new List<Tuple<string, object>>();

  internal static void Init(string dbname)
  {
    db_name = dbname;
  }

  internal void Add(string _sql)
  {
    sql += $" {_sql} ";
  }

  internal void AddParam(string key, object data)
  {
    parameters.Add(new Tuple<string, object>(key, data));
  }

  internal Dictionary<string, object> Select()
  {
    if (db_name == null)
    {
      throw new Exception("使用するデータベースを指定してください。");
    }

    SQLiteConnectionStringBuilder sqlConnectionSb = new SQLiteConnectionStringBuilder() { DataSource = db_name };

    var connection = new SQLiteConnection(sqlConnectionSb.ToString());
    connection.Open();

    var cmd = new SQLiteCommand(connection)
    {
      CommandText = sql
    };

    foreach (var parameter in parameters)
    {
      cmd.Parameters.Add(new SQLiteParameter(parameter.Item1, parameter.Item2));
    }

    Dictionary<string, object> result = new Dictionary<string, object>();

    var reader = cmd.ExecuteReader();

    for (int i = 0; i < reader.FieldCount; i++)
    {
      result[reader.GetFieldValue<string>(i)] = reader.GetValue(i);
    }

    Reset();

    reader.Close();
    connection.Close();

    return result;
  }


  internal List<Dictionary<string, object>> SelectAll()
  {
    if (db_name == null)
    {
      throw new Exception("使用するデータベースを指定してください。");
    }

    SQLiteConnectionStringBuilder sqlConnectionSb = new SQLiteConnectionStringBuilder() { DataSource = db_name };

    var connection = new SQLiteConnection(sqlConnectionSb.ToString());
    connection.Open();

    var cmd = new SQLiteCommand(connection)
    {
      CommandText = sql
    };

    foreach (var parameter in parameters)
    {
      cmd.Parameters.Add(new SQLiteParameter(parameter.Item1, parameter.Item2));
    }

    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

    var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
      Dictionary<string, object> tmp_result = new Dictionary<string, object>();
      for (int i = 0; i < reader.FieldCount; i++)
      {
        tmp_result[reader.GetName(i)] = reader.GetValue(i);
      }
      result.Add(tmp_result);
    }
    Reset();

    reader.Close();
    connection.Close();

    return result;
  }


  internal void Execute()
  {
    if (db_name == null)
    {
      throw new Exception("使用するデータベースを指定してください。");
    }

    SQLiteConnectionStringBuilder sqlConnectionSb = new SQLiteConnectionStringBuilder() { DataSource = db_name };

    var connection = new SQLiteConnection(sqlConnectionSb.ToString());
    connection.Open();

    var cmd = new SQLiteCommand(connection)
    {
      CommandText = sql
    };

    foreach (var parameter in parameters)
    {
      cmd.Parameters.Add(new SQLiteParameter(parameter.Item1, parameter.Item2));
    }

    if (cmd.ExecuteNonQuery() == 0)
    {
      throw new Exception("having no record to execute the query specified.");
    }
    Reset();

    connection.Close();

    return;
  }

  private void Reset()
  {
    sql = "";
    parameters.Clear();
  }

}
