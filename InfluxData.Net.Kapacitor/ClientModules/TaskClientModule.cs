﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.Kapacitor.Constants;
using InfluxData.Net.Kapacitor.Models;
using InfluxData.Net.Kapacitor.Models.Responses;
using InfluxData.Net.Kapacitor.RequestClients;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public class TaskClientModule : ClientModuleBase, ITaskClientModule
    {
        public TaskClientModule(IKapacitorRequestClient requestClient)
            : base(requestClient)
        {
        }

        public virtual async Task<KapacitorTask> GetTaskAsync(string taskName)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskName) }
            };
            var response = await base.RequestClient.GetAsync(RequestPaths.Task, requestParams);
            var task = response.ReadAs<KapacitorTask>();

            return task;
        }

        public virtual async Task<IEnumerable<KapacitorTask>> GetTasksAsync()
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Tasks, String.Empty }
            };
            var response = await base.RequestClient.GetAsync(RequestPaths.Tasks, requestParams);
            var tasks = response.ReadAs<KapacitorTasks>();

            return tasks.Tasks;
        }

        public virtual async Task<IInfluxDataApiResponse> DefineTaskAsync(DefineTaskParams taskParams)
        {
            var dbrps = String.Format("[{{\"{0}\":\"{1}\", \"{2}\":\"{3}\"}}]", 
                QueryParams.Db, taskParams.DBRPsParams.DbName, QueryParams.RetentionPolicy, taskParams.DBRPsParams.RetentionPolicy);

            var requestParams  = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskParams.TaskName) },
                { QueryParams.Type, taskParams.TaskType.ToString().ToLower() },
                { QueryParams.Dbrps, HttpUtility.UrlEncode(dbrps) }
            };

            return await base.RequestClient.PostAsync(RequestPaths.Task, requestParams, taskParams.TickScript);
        }

        public virtual async Task<IInfluxDataApiResponse> DeleteTaskAsync(string taskName)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskName) }
            };

            return await base.RequestClient.DeleteAsync(RequestPaths.Task, requestParams);
        }

        public virtual async Task<IInfluxDataApiResponse> EnableTaskAsync(string taskName)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskName) }
            };

            return await base.RequestClient.PostAsync(RequestPaths.Enable, requestParams, String.Empty);
        }

        public virtual async Task<IInfluxDataApiResponse> DisableTaskAsync(string taskName)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskName) }
            };

            return await base.RequestClient.PostAsync(RequestPaths.Disable, requestParams, String.Empty);
        }
    }
}