using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace Ares.Quartz.Extensions
{
    public static class QuartzExtensions
    {
        /// <summary>
        /// 配置Quartz
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IScheduler>(pro =>
            {
                var scheduler = pro.GetService<ISchedulerFactory>().GetScheduler().Result;
                scheduler.JobFactory = new QuartzJobFactory(pro);
                return scheduler;
            });
            return services;
        }

        /// <summary>
        /// 配置Quartz
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">配置作业</param>
        /// <returns></returns>
        public static IServiceCollection AddQuartz(this IServiceCollection services, Action<QuartzJobOptions> configure)
        {
            var jobs = new List<QuartzJobConfig>();
            var options = new QuartzJobOptions(jobs);
            configure(options);

            foreach (var item in jobs)
            {
                services.Add(new ServiceDescriptor(item.JobType, item.JobType, item.ServiceLifetime));
            }

            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IScheduler>(pro =>
            {
                var scheduler = pro.GetService<ISchedulerFactory>().GetScheduler().Result;
                scheduler.JobFactory = new QuartzJobFactory(pro);
                foreach (var item in jobs)
                {
                    scheduler.ScheduleJob(item.JobDetail, item.Trigger);
                }
                return scheduler;
            });
            return services;
        }

        /// <summary>
        /// 使用 Quartz
        /// </summary>
        /// <param name="services"></param>
        //public static void UseQuartz(this IServiceProvider services)
        //{
        //    services.GetRequiredService<IScheduler>().Start();
        //}

        /// <summary>
        /// 使用 Quartz
        /// </summary>
        /// <param name="app"></param>
        public static void UseQuartz(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<IScheduler>().Start();
        }
    }

    /// <summary>
    /// Quartz 配置参数
    /// </summary>
    public class QuartzJobOptions
    {
        private readonly IList<QuartzJobConfig> _jobs;

        public QuartzJobOptions(IList<QuartzJobConfig> jobs)
        {
            _jobs = jobs;
        }

        /// <summary>
        /// 配置执行需要执行的作业
        /// </summary>
        /// <param name="cron">cron 表达式</param>
        /// <typeparam name="T">IJob</typeparam>
        public void ScheduleJob<T>(ServiceLifetime serviceLifetime, string cron) where T : class, IJob
        {
            var name = typeof(T).FullName;
            var job = JobBuilder.Create<T>().WithIdentity(name, "jobs").Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_" + name, "jobs")
                .WithCronSchedule(cron)
                .Build();

            _jobs.Add(new QuartzJobConfig { JobType = typeof(T), JobDetail = job, Trigger = trigger, ServiceLifetime = serviceLifetime });
        }

        /// <summary>
        /// 调试执行作业
        /// </summary>
        /// <param name="ignore">忽视所有参数, 只执行一次作业</param>
        /// <typeparam name="T">IJob</typeparam>
        [Conditional("DEBUG")]
        public void Debug<T>(ServiceLifetime serviceLifetime, params object[] ignore) where T : class, IJob
        {
            var name = typeof(T).FullName;
            var job = JobBuilder.Create<T>().WithIdentity(name, "jobs").Build();
            var trigger = TriggerBuilder.Create()
                     .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).WithRepeatCount(0)).StartNow()
                     .Build();

            _jobs.Add(new QuartzJobConfig { JobType = typeof(T), JobDetail = job, Trigger = trigger, ServiceLifetime = serviceLifetime });
        }
    }

    /// <summary>
    /// Quartz 作业创建工厂
    /// </summary>
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<object, IServiceScope> ServiceDictionary = new ConcurrentDictionary<object, IServiceScope>();

        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = _serviceProvider.CreateScope();
            try
            {
                var job = (IJob)scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType);
                if (!ServiceDictionary.TryAdd(job, scope))
                {
                    scope.Dispose();
                    scope.ServiceProvider.GetService<ILogger<QuartzJobFactory>>()?.LogWarning("ServiceDictionary 加入失败, 依赖注入生命周期自动回收");
                }
                return job;
            }
            catch (System.Exception ex)
            {
                scope.ServiceProvider.GetService<ILogger<QuartzJobFactory>>()?.LogError(ex, "Quartz 作业创建工厂异常");
                scope.Dispose();
                throw ex;
            }
        }

        public void ReturnJob(IJob job)
        {
            if (ServiceDictionary.TryRemove(job, out var scope))
            {
                scope.Dispose();
            }
            else
            {
                System.Console.WriteLine("Remove scope 异常");
            }
        }
    }

    public class QuartzJobConfig
    {
        /// <summary>
        /// 对象生命周期
        /// </summary>
        /// <value></value>
        public ServiceLifetime ServiceLifetime { get; set; }

        /// <summary>
        /// IJob 对象的type
        /// </summary>
        /// <value></value>
        public Type JobType { get; set; }

        /// <summary>
        /// 作业详细
        /// </summary>
        /// <value></value>
        public IJobDetail JobDetail { get; set; }

        /// <summary>
        /// 调度配置
        /// </summary>
        /// <value></value>
        public ITrigger Trigger { get; set; }
    }
}