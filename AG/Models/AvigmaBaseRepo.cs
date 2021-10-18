using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AG.Models
{
    public class AvigmaBaseRepo
    {
        public AvigmaAGContext avigmaAGContext { get; set; }

        public AvigmaBaseRepo(AvigmaAGContext _avigmaAGContext)
        {
            avigmaAGContext = _avigmaAGContext;
        }

        public async Task<TResult> StoreProcedureAsync<TResult>(string spName, params object[] parameters) where TResult : new()
        {
            TResult result = new TResult();
            try
            {
                PropertyInfo[] propertyInfos; IList lstData = null; IDictionary dictionary = null;
                object instance = result;
                if (result.GetType().IsGenericType)
                {
                    if (result.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        dictionary = (IDictionary)result;
                    else
                        lstData = (IList)result;

                    propertyInfos = result.GetType().GetGenericArguments()[0].GetProperties();
                }
                else
                    propertyInfos = result.GetType().GetProperties();

                avigmaAGContext.Database.GetDbConnection().Open();
                DbCommand cmd = avigmaAGContext.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                IDataReader dr = await cmd.ExecuteReaderAsync();
                while (dr.Read())
                {
                    if (result.GetType().IsGenericType && result.GetType().GetGenericTypeDefinition() != typeof(Dictionary<,>))
                        instance = Activator.CreateInstance(result.GetType().GetGenericArguments()[0]);

                    if (typeof(int) == instance.GetType() || typeof(string) == instance.GetType())
                    {
                        instance = dr.GetValue(0);
                    }
                    else if (result.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        instance.GetType().GetProperty("Item").SetValue(instance, dr.GetValue(1), new[] { dr.GetValue(0) });
                    }
                    else
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            PropertyInfo property = propertyInfos.FirstOrDefault(p => p.Name.ToLower().Equals(dr.GetName(i).ToLower()));
                            if (property != null)
                            {
                                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                object value = dr.GetValue(i);
                                if (!targetType.IsEnum)
                                {
                                    if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
                                        value = Convert.ChangeType(value, targetType);
                                    else
                                        value = null;

                                    property.SetValue(instance, value);
                                }
                                else
                                {
                                    property.SetValue(instance, Enum.ToObject(property.PropertyType, value), null);
                                }
                            }
                        }
                    }
                    if (lstData != null)
                        lstData.Add(instance);
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (avigmaAGContext.Database.GetDbConnection().State == ConnectionState.Open)
                    avigmaAGContext.Database.CloseConnection();
            }

            return result;
        }




        // TO DO : to b made genric later
        public async Task<int> StoreProcedureAsync(string spName, params object[] parameters) 
        {
            try
            {
                avigmaAGContext.Database.GetDbConnection().Open();
                DbCommand cmd = avigmaAGContext.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                IDataReader dr = await cmd.ExecuteReaderAsync();
                int id_out = Convert.ToInt32(cmd.Parameters["@Id_Out"].Value);
                int rv = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                int er = Convert.ToInt32(cmd.Parameters["@ExecutionResult"].Value);

                return id_out;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (avigmaAGContext.Database.GetDbConnection().State == ConnectionState.Open)
                    avigmaAGContext.Database.CloseConnection();
            }

        }

    }
}
