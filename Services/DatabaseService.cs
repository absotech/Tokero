using SQLite;
using Tokero.Data;
using Tokero.Models;

namespace Tokero.Services
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection? db;
        private static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Tokero.db");
            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<UserDb>();
            await db.CreateTableAsync<InvestmentPlanDb>();
            await db.CreateTableAsync<ConfiguredAssetDb>();
            var defaultUser = new UserDb
            {
                Id = Guid.NewGuid().ToString(),
                Username = "admin",
                Password = "admin" // i wanted to bcrypt this but i didn't want to add that in this simple app
            };
            await db.InsertOrReplaceAsync(defaultUser);
        }
        public static async Task SavePlanAsync(InvestmentPlan plan)
        {
            await Init();
            var planDb = new InvestmentPlanDb
            {
                Id = Guid.NewGuid().ToString(),
                PlanName = plan.PlanName,
                AllocationStrategy = plan.AllocationStrategy,
                Notes = plan.Notes
            };
            await db.InsertAsync(planDb);
            foreach (var asset in plan.ConfiguredAssets)
            {
                var assetDb = new ConfiguredAssetDb
                {
                    InvestmentPlanId = planDb.Id,
                    Symbol = asset.Symbol,
                    Name = asset.Name,
                    MonthlyInvestment = asset.MonthlyInvestment,
                    StartDate = asset.StartDate
                };
                await db.InsertAsync(assetDb);
            }
        }
        public static async Task<List<InvestmentPlan>> GetAllPlansAsync()
        {
            await Init();
            var plans = new List<InvestmentPlan>();

            var dbPlans = await db.Table<InvestmentPlanDb>().ToListAsync();

            foreach (var dbPlan in dbPlans)
            {
                var dbAssets = await db.Table<ConfiguredAssetDb>()
                                        .Where(a => a.InvestmentPlanId == dbPlan.Id)
                                        .ToListAsync();
                var plan = new InvestmentPlan
                {
                    Id = dbPlan.Id,
                    PlanName = dbPlan.PlanName,
                    AllocationStrategy = dbPlan.AllocationStrategy,
                    Notes = dbPlan.Notes,
                    ConfiguredAssets = [.. dbAssets.Select(dba => new ConfiguredAsset
                    {
                        Symbol = dba.Symbol,
                        Name = dba.Name,
                        MonthlyInvestment = dba.MonthlyInvestment,
                        StartDate = dba.StartDate
                    })]
                };
                plans.Add(plan);
            }
            return plans;
        }
        public static async Task DeletePlanAsync(InvestmentPlan plan)
        {
            await Init();
            var dbPlan = await db.Table<InvestmentPlanDb>()
                                 .Where(p => p.Id == plan.Id)
                                 .FirstOrDefaultAsync();
            if (dbPlan != null)
            {
                await db.DeleteAsync(dbPlan);
                var dbAssets = await db.Table<ConfiguredAssetDb>()
                                       .Where(a => a.InvestmentPlanId == dbPlan.Id)
                                       .ToListAsync();
                foreach (var asset in dbAssets)
                {
                    await db.DeleteAsync(asset);
                }
            }
        }
        public static async Task<bool> Login(string username, string password)
        {
            await Init();
            var user = await db.Table<UserDb>()
                              .Where(u => u.Username == username && u.Password == password)
                              .FirstOrDefaultAsync();
            return user != null;
        }
    }
}
