﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Formula.SimpleRepo;

namespace Dapper;

/// <summary>
/// Main class for Dapper.SimpleCRUD extensions
/// </summary>
public partial class SimpleCRUD
{
    /// <summary>
    /// <para>By default queries the table matching the class name asynchronously </para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>By default filters on the Id column</para>
    /// <para>-Id column name can be overridden by adding an attribute on your primary key property [Key]</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>Returns a single entity by a single id from table T</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="id"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Returns a single entity by a single id from table T.</returns>
    public async Task<T> GetAsync<T>(IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var idProps = GetIdProperties(currenttype).ToList();

        if (!idProps.Any())
        {
            throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");
        }

        var name = GetTableName(currenttype);
        var sb = new StringBuilder();
        sb.Append("Select ");
        //create a new empty instance of the type to get the base properties
        BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
        sb.AppendFormat(" from {0} where ", name);

        for (var i = 0; i < idProps.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(" and ");
            }
            sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
        }

        var dynParms = new DynamicParameters();
        if (idProps.Count == 1)
        {
            dynParms.Add("@" + idProps.First().Name, id);
        }
        else
        {
            foreach (var prop in idProps)
            {
                dynParms.Add("@" + prop.Name, id.GetType().GetProperty(prop.Name).GetValue(id, null));
            }
        }

        _logQuery(string.Format("Get<{0}>: {1} with Id: {2}", currenttype, sb, id));

        var query = await connection.QueryAsync<T>(sb.ToString(), dynParms, transaction, commandTimeout);
        return query.FirstOrDefault();
    }

    /// <summary>
    /// <para>By default queries the table matching the class name asynchronously</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>Returns a list of entities that match where conditions</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="whereConditions"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Gets a list of entities with optional exact match where conditions</returns>
    public Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        var whereprops = GetAllProperties(whereConditions).ToArray();
        sb.Append("Select ");
        //create a new empty instance of the type to get the base properties
        BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
        sb.AppendFormat(" from {0}", name);

        if (whereprops.Any())
        {
            sb.Append(" where ");
            BuildWhere<T>(sb, whereprops, whereConditions);
        }

        _logQuery(string.Format("GetList<{0}>: {1}", currenttype, sb));

        return connection.QueryAsync<T>(sb.ToString(), whereConditions, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>By default queries the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>conditions is an SQL where clause and/or order by clause ex: "where name='bob'" or "where age>=@Age"</para>
    /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>Returns a list of entities that match where conditions</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="conditions"></param>
    /// <param name="parameters"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Gets a list of entities with optional SQL where conditions</returns>
    public Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        sb.Append("Select ");
        //create a new empty instance of the type to get the base properties
        BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
        sb.AppendFormat(" from {0}", name);

        sb.Append(" " + conditions);

        _logQuery(string.Format("GetList<{0}>: {1}", currenttype, sb));

        return connection.QueryAsync<T>(sb.ToString(), parameters, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>By default queries the table matching the class name asynchronously</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Returns a list of all entities</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <returns>Gets a list of all entities</returns>
    public Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection)
    {
        return GetListAsync<T>(connection, new { });
    }

    /// <summary>
    /// <para>By default queries the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </para>
    /// <para>orderby is a column or list of columns to order by ex: "lastname, age desc" - not required - default is by primary key</para>
    /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
    /// <para>Returns a list of entities that match where conditions</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="pageNumber"></param>
    /// <param name="rowsPerPage"></param>
    /// <param name="conditions"></param>
    /// <param name="orderby"></param>
    /// <param name="parameters"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Gets a list of entities with optional exact match where conditions</returns>
    public Task<IEnumerable<T>> GetListPagedAsync<T>(IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        if (string.IsNullOrEmpty(_getPagedListSql))
        {
            throw new Exception("GetListPage is not supported with the current SQL Dialect");
        }

        var currenttype = typeof(T);
        var idProps = GetIdProperties(currenttype).ToList();
        if (!idProps.Any())
        {
            throw new ArgumentException("Entity must have at least one [Key] property");
        }

        var name = GetTableName(currenttype);
        var sb = new StringBuilder();
        var query = _getPagedListSql;
        if (string.IsNullOrEmpty(orderby))
        {
            orderby = GetColumnName(idProps.First());
        }

        //create a new empty instance of the type to get the base properties
        BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
        query = query.Replace("{SelectColumns}", sb.ToString());
        query = query.Replace("{TableName}", name);
        query = query.Replace("{PageNumber}", pageNumber.ToString());
        query = query.Replace("{RowsPerPage}", rowsPerPage.ToString());
        query = query.Replace("{OrderBy}", orderby);
        query = query.Replace("{WhereClause}", conditions);
        query = query.Replace("{Offset}", ((pageNumber - 1) * rowsPerPage).ToString());

        _logQuery(string.Format("GetListPaged<{0}>: {1}", currenttype, query));

        return connection.QueryAsync<T>(query, parameters, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>Inserts a row into the database asynchronously</para>
    /// <para>By default inserts into the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
    /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>Returns the ID (primary key) of the newly inserted record if it is identity using the int? type, otherwise null</para>
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="entityToInsert"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The ID (primary key) of the newly inserted record if it is identity using the int? type, otherwise null</returns>
    public Task<int?> InsertAsync<TEntity>(IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        return InsertAsync<int?, TEntity>(connection, entityToInsert, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>Inserts a row into the database, using ONLY the properties defined by TEntity</para>
    /// <para>By default inserts into the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
    /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>Returns the ID (primary key) of the newly inserted record if it is identity using the defined type, otherwise null</para>
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="entityToInsert"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The ID (primary key) of the newly inserted record if it is identity using the defined type, otherwise null</returns>
    public async Task<TKey> InsertAsync<TKey, TEntity>(IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var idProps = GetIdProperties(entityToInsert).ToList();

        if (!idProps.Any())
        {
            throw new ArgumentException("Insert<T> only supports an entity with a [Key] or Id property");
        }

        var keyHasPredefinedValue = false;
        var baseType = typeof(TKey);
        var underlyingType = Nullable.GetUnderlyingType(baseType);
        var keytype = underlyingType ?? baseType;
        if (keytype != typeof(int) && keytype != typeof(uint) && keytype != typeof(long) && keytype != typeof(ulong) && keytype != typeof(short) && keytype != typeof(ushort) && keytype != typeof(Guid) && keytype != typeof(string))
        {
            throw new Exception("Invalid return type");
        }

        QueryLogger.Log(entityToInsert);

        var name = GetTableName(entityToInsert);
        var sb = new StringBuilder();
        sb.AppendFormat("insert into {0}", name);
        sb.Append(" (");
        BuildInsertParameters<TEntity>(sb);
        sb.Append(") ");
        sb.Append("values");
        sb.Append(" (");
        BuildInsertValues<TEntity>(sb);
        sb.Append(")");

        if (keytype == typeof(Guid))
        {
            var guidvalue = (Guid)idProps.First().GetValue(entityToInsert, null);
            if (guidvalue == Guid.Empty)
            {
                var newguid = SequentialGuid();
                idProps.First().SetValue(entityToInsert, newguid, null);
            }
            else
            {
                keyHasPredefinedValue = true;
            }
        }

        if ((keytype == typeof(int) || keytype == typeof(long)) && Convert.ToInt64(idProps.First().GetValue(entityToInsert, null)) == 0)
        {
            sb.Append(";" + _getIdentitySql);
        }
        else
        {
            keyHasPredefinedValue = true;
        }

        var insertSuffixAttribute = typeof(TEntity).GetCustomAttribute<InsertSuffixAttribute>();
        if (insertSuffixAttribute != null)
        {
            sb.Append(" "); // Ensure there is space before appending the suffix
            sb.Append(insertSuffixAttribute.Suffix);
        }

        _logQuery(string.Format("Insert: {0}", sb));

        if (keytype == typeof(Guid) || keyHasPredefinedValue)
        {
            await connection.ExecuteAsync(sb.ToString(), entityToInsert, transaction, commandTimeout);
            return (TKey)idProps.First().GetValue(entityToInsert, null);
        }
        var r = await connection.QueryAsync(sb.ToString(), entityToInsert, transaction, commandTimeout);
        return (TKey)r.First().id;
    }

    /// <summary>
    ///  <para>Updates a record or records in the database asynchronously</para>
    ///  <para>By default updates records in the table matching the class name</para>
    ///  <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    ///  <para>Updates records where the Id property and properties with the [Key] attribute match those in the database.</para>
    ///  <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
    ///  <para>Supports transaction and command timeout</para>
    ///  <para>Returns number of rows affected</para>
    ///  </summary>
    ///  <param name="connection"></param>
    ///  <param name="entityToUpdate"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The number of affected records</returns>
    public Task<int> UpdateAsync<TEntity>(IDbConnection connection, TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, System.Threading.CancellationToken? token = null)
    {
        var idProps = GetIdProperties(entityToUpdate).ToList();

        if (!idProps.Any())
        {
            throw new ArgumentException("Entity must have at least one [Key] or Id property");
        }

        QueryLogger.Log(entityToUpdate);

        var name = GetTableName(entityToUpdate);

        var sb = new StringBuilder();
        sb.AppendFormat("update {0}", name);

        sb.AppendFormat(" set ");
        BuildUpdateSet(entityToUpdate, sb);
        sb.Append(" where ");
        BuildWhere<TEntity>(sb, idProps, entityToUpdate);

        _logQuery(string.Format("Update: {0}", sb));

        System.Threading.CancellationToken cancelToken = token ?? default(System.Threading.CancellationToken);
        return connection.ExecuteAsync(new CommandDefinition(sb.ToString(), entityToUpdate, transaction, commandTimeout, cancellationToken: cancelToken));
    }

    public Task<int> UpdateAsync<TEntity>(IDbConnection connection, TEntity entityToUpdate, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        if (string.IsNullOrEmpty(conditions))
        {
            throw new ArgumentException("UpdateAsync<TEntity> requires a where clause");
        }
        if (!conditions.ToLower().Contains("where"))
        {
            throw new ArgumentException("UpdateAsync<TEntity> requires a where clause and must contain the WHERE keyword");
        }

        var idProps = GetIdProperties(entityToUpdate).ToList();

        if (!idProps.Any())
        {
            throw new ArgumentException("Entity must have at least one [Key] or Id property");
        }

        QueryLogger.Log(entityToUpdate);

        var currenttype = typeof(TEntity);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        sb.AppendFormat("update {0}", name);

        sb.AppendFormat(" set ");
        BuildUpdateSet(entityToUpdate, sb);
        sb.Append(" " + conditions.Replace("\n", " AND "));
        BuildWhere<TEntity>(sb, idProps, entityToUpdate);

        _logQuery(string.Format("Update: {0}", sb));

        return connection.ExecuteAsync(sb.ToString(), parameters, transaction, commandTimeout);
    }


    /// <summary>
    /// <para>Deletes a record or records in the database that match the object passed in asynchronously</para>
    /// <para>-By default deletes records in the table matching the class name</para>
    /// <para>Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>Returns the number of records affected</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="entityToDelete"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The number of records affected</returns>
    public Task<int> DeleteAsync<T>(IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var idProps = GetIdProperties(entityToDelete).ToList();

        if (!idProps.Any())
        {
            throw new ArgumentException("Entity must have at least one [Key] or Id property");
        }

        QueryLogger.Log(entityToDelete);

        var name = GetTableName(entityToDelete);

        var sb = new StringBuilder();
        sb.AppendFormat("delete from {0}", name);

        sb.Append(" where ");
        BuildWhere<T>(sb, idProps, entityToDelete);

        _logQuery(string.Format("Delete: {0}", sb));

        return connection.ExecuteAsync(sb.ToString(), entityToDelete, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>Deletes a record or records in the database by ID asynchronously</para>
    /// <para>By default deletes records in the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Deletes records where the Id property and properties with the [Key] attribute match those in the database</para>
    /// <para>The number of records affected</para>
    /// <para>Supports transaction and command timeout</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="id"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The number of records affected</returns>
    public Task<int> DeleteAsync<T>(IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var idProps = GetIdProperties(currenttype).ToList();

        if (!idProps.Any())
        {
            throw new ArgumentException("Delete<T> only supports an entity with a [Key] or Id property");
        }

        QueryLogger.Log(id);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        sb.AppendFormat("Delete from {0} where ", name);

        for (var i = 0; i < idProps.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(" and ");
            }
            sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
        }

        var dynParms = new DynamicParameters();
        if (idProps.Count == 1)
        {
            dynParms.Add("@" + idProps.First().Name, id);
        }
        else
        {
            foreach (var prop in idProps)
            {
                dynParms.Add("@" + prop.Name, prop.GetValue(id));
            }
        }

        _logQuery(string.Format("Delete<{0}> {1}", currenttype, sb));

        return connection.ExecuteAsync(sb.ToString(), dynParms, transaction, commandTimeout);
    }


    /// <summary>
    /// <para>Deletes a list of records in the database</para>
    /// <para>By default deletes records in the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Deletes records where that match the where clause</para>
    /// <para>whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}</para>
    /// <para>The number of records affected</para>
    /// <para>Supports transaction and command timeout</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="whereConditions"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The number of records affected</returns>
    public Task<int> DeleteListAsync<T>(IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        var whereprops = GetAllProperties(whereConditions).ToArray();
        sb.AppendFormat("Delete from {0}", name);
        if (whereprops.Any())
        {
            sb.Append(" where ");
            BuildWhere<T>(sb, whereprops);
        }

        _logQuery(string.Format("DeleteList<{0}> {1}", currenttype, sb));

        return connection.ExecuteAsync(sb.ToString(), whereConditions, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>Deletes a list of records in the database</para>
    /// <para>By default deletes records in the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Deletes records where that match the where clause</para>
    /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age"</para>
    /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="conditions"></param>
    /// <param name="parameters"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>The number of records affected</returns>
    public Task<int> DeleteListAsync<T>(IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        if (string.IsNullOrEmpty(conditions))
        {
            throw new ArgumentException("DeleteList<T> requires a where clause");
        }
        if (!conditions.ToLower().Contains("where"))
        {
            throw new ArgumentException("DeleteList<T> requires a where clause and must contain the WHERE keyword");
        }

        var currenttype = typeof(T);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        sb.AppendFormat("Delete from {0}", name);
        sb.Append(" " + conditions);

        _logQuery(string.Format("DeleteList<{0}> {1}", currenttype, sb));

        return connection.ExecuteAsync(sb.ToString(), parameters, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>By default queries the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </para>
    /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>   
    /// <para>Supports transaction and command timeout</para>
    /// /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="conditions"></param>
    /// <param name="parameters"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Returns a count of records.</returns>
    public Task<int> RecordCountAsync<T>(IDbConnection connection, string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var name = GetTableName(currenttype);
        var sb = new StringBuilder();
        sb.Append("Select count(1)");
        sb.AppendFormat(" from {0}", name);
        sb.Append(" " + conditions);

        _logQuery(string.Format("RecordCount<{0}>: {1}", currenttype, sb));

        return connection.ExecuteScalarAsync<int>(sb.ToString(), parameters, transaction, commandTimeout);
    }

    /// <summary>
    /// <para>By default queries the table matching the class name</para>
    /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
    /// <para>Returns a number of records entity by a single id from table T</para>
    /// <para>Supports transaction and command timeout</para>
    /// <para>whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="whereConditions"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Returns a count of records.</returns>
    public Task<int> RecordCountAsync<T>(IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
    {
        var currenttype = typeof(T);
        var name = GetTableName(currenttype);

        var sb = new StringBuilder();
        var whereprops = GetAllProperties(whereConditions).ToArray();
        sb.Append("Select count(1)");
        sb.AppendFormat(" from {0}", name);
        if (whereprops.Any())
        {
            sb.Append(" where ");
            BuildWhere<T>(sb, whereprops);
        }

        _logQuery(string.Format("RecordCount<{0}>: {1}", currenttype, sb));

        return connection.ExecuteScalarAsync<int>(sb.ToString(), whereConditions, transaction, commandTimeout);
    }
}

