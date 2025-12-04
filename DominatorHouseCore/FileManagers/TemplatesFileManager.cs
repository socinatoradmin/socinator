#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface ITemplatesFileManager
    {
        void ApplyFunc(Func<TemplateModel, bool> funcToApply);
        void UpdateActivitySettings(string templateId, string activitySettingsJson);
        List<TemplateModel> Get();
        TemplateModel GetTemplateById(string id);
        void Save(List<TemplateModel> templates);
        void Add(TemplateModel template);
        void Delete(TemplateModel template);
        void Delete(Func<TemplateModel, bool> match);
        void Edit(TemplateModel template);
        TemplateModel this[string template] { get; }
    }

    public class TemplatesFileManager : ITemplatesFileManager
    {
        private readonly ITemplatesCacheService _templatesCacheService;

        public TemplatesFileManager(ITemplatesCacheService cacheService)
        {
            _templatesCacheService = cacheService;
            _templatesCacheService.GetTemplateModels();
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public void ApplyFunc(Func<TemplateModel, bool> funcToApply)
        {
            var templates = Get();
            var updated = false;

            foreach (var t in templates)
                updated |= funcToApply(t);

            if (updated)
                _templatesCacheService.UpsertTemplates(templates.ToArray());
        }

        private void ApplyActionForId(string templateId, Action<TemplateModel> actionToApply)
        {
            ApplyFunc(t =>
            {
                if (t.Id == templateId)
                {
                    actionToApply(t);
                    return true;
                }

                return false;
            });
        }

        public void UpdateActivitySettings(string templateId, string activitySettingsJson)
        {
            ApplyActionForId(templateId, t => t.ActivitySettings = activitySettingsJson);
        }


        public List<TemplateModel> Get()
        {
            return _templatesCacheService.GetTemplateModels().ToList();
        }

        public TemplateModel GetTemplateById(string id)
        {
            var templates = Get();
            var result = templates.FirstOrDefault(t => t.Id == id);

            return result;
        }

        public void Save(List<TemplateModel> templates)
        {
            _templatesCacheService.UpsertTemplates(templates.ToArray());
        }


        public void Add(TemplateModel template)
        {
            _templatesCacheService.UpsertTemplates(template);
        }

        // finds by id and delete
        public void Delete(TemplateModel template)
        {
            var templates = Get();
            var toDelete = templates.FirstOrDefault(t => t.Id == template.Id);
            if (toDelete != null)
            {
                templates.Remove(toDelete);
                Save(templates);
            }
        }

        public void Delete(Func<TemplateModel, bool> match)
        {
            var templates = Get();
            _templatesCacheService.Delete(templates.Where(match).ToArray());
        }

        public void Edit(TemplateModel template)
        {
            var templates = Get();
            var index = templates.FindIndex(t => t.Id == template.Id);
            if (index != -1)
            {
                templates[index] = template;
                Save(templates);
            }
        }

        public TemplateModel this[string template] => _templatesCacheService[template];
    }
}