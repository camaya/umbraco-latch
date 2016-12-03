using System.IO;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Services;
using UmbracoLatch.Core.Checkers;
using Umbraco.Core.Persistence;
using System.Collections.Generic;
using System;

namespace UmbracoLatch.Core
{
    public class UmbracoLatchStartup : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ConfigureUmbracoLatchSection(applicationContext);
            ConfigureUmbracoLatchDatabaseTables(umbracoApplication, applicationContext);
            ConfigureUmbracoLatchEventHandlers(applicationContext);
        }

        private void ConfigureUmbracoLatchSection(ApplicationContext applicationContext)
        {
            var sectionService = applicationContext.Services.SectionService;
            var latchSection = sectionService.GetSections().SingleOrDefault(x => x.Name.Equals(LatchConstants.SectionAlias, StringComparison.InvariantCultureIgnoreCase));
            if (latchSection == null)
            {
                sectionService.MakeNew(LatchConstants.SectionName, LatchConstants.SectionAlias, LatchConstants.SectionIcon);
            }
        }

        private void ConfigureUmbracoLatchDatabaseTables(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var ctx = applicationContext.DatabaseContext;
            var db = new DatabaseSchemaHelper(ctx.Database, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);
            var umbracoLatchTables = new []
            {
                LatchConstants.Tables.LatchApplication,
                LatchConstants.Tables.LatchPairedAccount,
                LatchConstants.Tables.LatchOperation,
                LatchConstants.Tables.LatchOperationUser,
                LatchConstants.Tables.LatchOperationNode
            };

            var sqlScriptsPath = umbracoApplication.Server.MapPath("~/App_Plugins/Latch/sql");
            var createScripts = new List<string>();

            foreach(var table in umbracoLatchTables)
            {
                if (!db.TableExist(table))
                {
                    var scriptPath = string.Format("{0}/{1}Table.sql", sqlScriptsPath, table);
                    var script = File.ReadAllText(scriptPath);
                    createScripts.Add(script);
                }
            }

            if (createScripts.Any())
            {
                using(var scope = ctx.Database.GetTransaction())
                {
                    foreach(var createScript in createScripts)
                    {
                        ctx.Database.Execute(createScript);
                    }
                    scope.Complete();
                }
            }
        }

        private void ConfigureUmbracoLatchEventHandlers(ApplicationContext applicationContext)
        {
            var latchOperationSvc = new LatchOperationService(applicationContext.DatabaseContext.Database, applicationContext.Services.TextService, applicationContext.Services.UserService);

            var contentChecker = new LatchContentChecker(latchOperationSvc, applicationContext.Services.TextService);
            ContentService.Publishing += contentChecker.LatchContentPublishing;
            ContentService.UnPublishing += contentChecker.LatchContentUnPublishing;
            ContentService.Trashing += contentChecker.LatchContentTrashing;

            var mediaChecker = new LatchMediaChecker(latchOperationSvc, applicationContext.Services.TextService);
            MediaService.Trashing += mediaChecker.LatchMediaTrashing;

            var localizationChecker = new LatchLocalizationChecker(latchOperationSvc, applicationContext.Services.TextService);
            LocalizationService.DeletingDictionaryItem += localizationChecker.LatchDictionaryItemDeleting;
        }

    }
}