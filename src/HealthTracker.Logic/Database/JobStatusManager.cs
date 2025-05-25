using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class JobStatusManager : DatabaseManagerBase, IJobStatusManager
    {
        internal JobStatusManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Get the first job status matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<JobStatus> GetAsync(Expression<Func<JobStatus, bool>> predicate)
        {
            List<JobStatus> statuses = await ListAsync(predicate, 1, 1);
            return statuses.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<JobStatus>> ListAsync(Expression<Func<JobStatus, bool>> predicate, int pageNumber, int pageSize)
            => await Context.JobStatuses
                            .Where(predicate)
                            .OrderByDescending(x => x.Start)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Create a new job status
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<JobStatus> AddAsync(string name, string parameters)
        {
            JobStatus status = new JobStatus
            {
                Name = name,
                Parameters = parameters,
                Start = DateTime.Now
            };

            await Context.JobStatuses.AddAsync(status);
            await Context.SaveChangesAsync();

            return status;
        }

        /// <summary>
        /// Update a job status, setting the end timestamp and error result
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public async Task<JobStatus> UpdateAsync(long id, string error)
        {
            JobStatus status = await GetAsync(x => x.Id == id);
            if (status != null)
            {
                status.End = DateTime.Now;
                status.Error = error;
                await Context.SaveChangesAsync();
            }

            return status;
        }
    }
}
