#region

using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface ITemplatesCacheService
    {
        IReadOnlyCollection<TemplateModel> GetTemplateModels();
        bool UpsertTemplates(params TemplateModel[] accounts);
        bool Delete(params TemplateModel[] accounts);
        TemplateModel this[string template] { get; }
    }

    public class TemplatesCacheService : ITemplatesCacheService
    {
        private readonly object _syncContext = new object();
        private readonly Dictionary<string, TemplateModel> _cache;
        private readonly IBinFileHelper _binFileHelper;

        public TemplatesCacheService(IBinFileHelper binFileHelper)
        {
            _binFileHelper = binFileHelper;
            _cache = new Dictionary<string, TemplateModel>();
        }

        public IReadOnlyCollection<TemplateModel> GetTemplateModels()
        {
            lock (_syncContext)
            {
                if (_cache.Count == 0)
                    foreach (var template in _binFileHelper.GetTemplateDetails())
                        _cache.Add(template.Id, template);

                return _cache.Values;
            }
        }

        public bool UpsertTemplates(params TemplateModel[] accounts)
        {
            lock (_syncContext)
            {
                var cacheCopy = _cache.ToDictionary(a => a.Key, a => a.Value);
                UpsertData(cacheCopy, accounts);
                var result = _binFileHelper.UpdateAllAccounts(cacheCopy.Values.ToList());
                if (result)
                {
                    _cache.Clear();
                    foreach (var model in cacheCopy) _cache.Add(model.Key, model.Value);
                }

                return result;
            }
        }

        public bool Delete(params TemplateModel[] templates)
        {
            lock (_syncContext)
            {
                var cacheCopy = _cache.ToDictionary(a => a.Key, a => a.Value);
                foreach (var template in templates)
                    if (cacheCopy.ContainsKey(template.Id))
                        cacheCopy[template.Id] = template;

                var result = _binFileHelper.UpdateAllAccounts(cacheCopy.Values.ToList());
                if (result)
                {
                    _cache.Clear();
                    foreach (var model in cacheCopy) _cache.Add(model.Key, model.Value);
                }

                return result;
            }
        }

        public TemplateModel this[string template]
        {
            get
            {
                lock (_syncContext)
                {
                    if (_cache.ContainsKey(template))
                        return _cache[template];
                    return null;
                }
            }
        }

        private void UpsertData(IDictionary<string, TemplateModel> target, params TemplateModel[] source)
        {
            lock (_syncContext)
            {
                foreach (var template in source)
                    if (target.ContainsKey(template.Id))
                        target[template.Id] = template;
                    else
                        target.Add(template.Id, template);
            }
        }
    }
}