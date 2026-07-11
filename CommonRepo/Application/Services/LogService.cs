using CommonLibrary.RequestInformation;
using CommonLibrary.Tracking;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Domain.Interfaces.SystemLogsInterfaces;
using Microsoft.EntityFrameworkCore;

namespace CommonRepo.Application.Services
{
    public class LogService<TLog, TView> : ILogService<TLog, TView> where TLog : class, new() where TView : class, new()
    {
        private readonly IRequestInfoService _requestInfoService;
        protected readonly IRepository<TLog> _logRepository;
        protected readonly IRepository<TView> _viewRepository;
        protected readonly EntityChangeOptions _entityChangeOptions;
        public LogService(IRepository<TLog> logRepository, IRepository<TView> viewRepository, IRequestInfoService requestInfoService)
        {
            _requestInfoService = requestInfoService;
            _logRepository = logRepository;
            _viewRepository = viewRepository;
            _entityChangeOptions = new EntityChangeOptions() 
            { IgnoreContains = ["id", 
                "createdby", 
                "Attach", 
                "CreationDate", 
                "CreatedDate", 
                "UpdatedDate", 
                "StakeholderStatusCode",
                "PortCode", 
                "RepresentativeNameLocalized",
                "RepresentativeEmail",
                "RepresentativeAreaCode",
                "RepresentativeMobileNo",
                "RegistrationType",
                "RepresentativeNameForeign",
                "ShipType",
                "portType",
                "BerthCode",
                "BerthType","BlacklistTypeCode","BlacklistReasonCode","BlacklistProcedureCode"
            ], NotIgnoreContains = ["RepresentativeIddocumentType", "representativeIddocumentNo", "ShipTypeNameAr", "ShipTypeNameEn"] };
        }

        public async Task<bool> LogTransaction<TEntity>(dynamic before, TEntity after, string actionName = null, int? userId = null) where TEntity : class, new()
        {
            if (before == null)
            {
                return await LogTransaction(originalEntity: null, after, actionName, userId);
            }

            if (before is TView view)
            {
                return await LogTransaction(originalView: view, after, actionName, userId);
            }

            return await LogTransaction(originalEntity: before, newEntity: after, actionName: actionName, userId: userId);
        }

        public async Task<bool> LogTransaction(TView originalView, TView newView, string actionName = null, int? userId = null)
        {
            var changeResult = EntityChangeTracker.GetChanges(originalView, newView, _entityChangeOptions);
            if (changeResult.OriginalDataJson == changeResult.NewDataJson && actionName == null) return true;
            var serverInfo = _requestInfoService.GetRequestInfo();
            // If actionName is null, use the value from serverInfo.ActionName
            actionName ??= serverInfo.ActionName;
            userId ??= serverInfo.UserId;

            var logEntry = new TLog();
            logEntry.SetPropertyValue("OldData", changeResult.OriginalDataJson);
            logEntry.SetPropertyValue("NewData", changeResult.NewDataJson);
            logEntry.SetPropertyValue("ServerInfo", serverInfo.ServerIpAddress);
            logEntry.SetPropertyValue("ActionName", actionName);
            logEntry.SetPropertyValue("EntityId", newView.GetObjectValue("Id"));
            logEntry.SetPropertyValue("UserId", userId);
            return (await _logRepository.AddAsync(logEntry)) != null;
        }

        private async Task<bool> LogTransaction<TEntity>(TEntity originalEntity, TEntity newEntity, string actionName = null, int? userId = null) where TEntity : class, new()
        {
            TView originalView = null;
            TView newView = null;
            var id = newEntity.GetObjectValue("Id");

            if (originalEntity != null)
            {
                originalView = await _viewRepository.GetAsync(x => EF.Property<int>(x, "Id").Equals(id));
            }

            if (newEntity != null)
            {
                newView = await _viewRepository.GetAsync(x => EF.Property<int>(x, "Id").Equals(id));
            }

            return await LogTransaction(originalView, newView, actionName, userId);
        }

        private async Task<bool> LogTransaction<TEntity>(TView originalView, TEntity newEntity, string actionName = null, int? userId = null) where TEntity : class, new()
        {
            TView newView = null;
            var id = newEntity.GetObjectValue("Id");

            if (newEntity != null)
            {
                newView = await _viewRepository.GetAsync(x => EF.Property<int>(x, "Id").Equals(id));
            }

            return await LogTransaction(originalView, newView, actionName, userId);
        }

        public async Task<TEntity> LogTransaction<TEntity>(TEntity originalEntity, Func<Task<TEntity>> ActionFunc, string actionName = null, int? userId = null) where TEntity : class, new()
        {
            TView originalView = null;
            TView newView = null;
            var id = originalEntity.GetObjectValue("Id");

            if (originalEntity != null)
            {
                originalView = await _viewRepository.GetAsync(x => EF.Property<int>(x, "Id").Equals(id));
            }

            var newEntity = await ActionFunc();

            if (newEntity != null)
            {
                newView = await _viewRepository.GetAsync(x => EF.Property<int>(x, "Id").Equals(id));
            }

            _ = await LogTransaction(originalView, newView, actionName, userId);

            return newEntity;
        }

    }
}
