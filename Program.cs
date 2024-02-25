using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlite("Data Source=database.db"), ServiceLifetime.Singleton);

// builder.Services.AddOutputCache(options =>
// {
//     options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
// });

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// app.UseOutputCache();

app.MapGet("/", () => "Hello World!");
app.MapGet("/rura/times", (DatabaseContext db, CancellationToken token) =>
{
    return db.SplitTimes.Where(s => s.RaceId == 15).ToListAsync(token);
});
app.MapGet("/rura/results", async (DatabaseContext db, CancellationToken token) =>
{
    var raceId = 15;
    var classificationId = 31;

    var allPlayers = await db
        .Players
        .Where(p => p.RaceId == raceId && p.ClassificationId == classificationId)
        .Include(p => p.Absences)
        .Include(p => p.PlayerProfile)
        .ToListAsync(token);

    var splitTimes = await db.SplitTimes
        .Where(st => st.RaceId == raceId && st.Player.ClassificationId == classificationId)
        .ToListAsync(token);

    var manualSplitTimes = await db.ManualSplitTimes
        .Where(st => st.RaceId == raceId && st.Player.ClassificationId == classificationId)
        .ToListAsync(token);

    var disqualifications = await db.Disqualifications
        .Where(d => d.RaceId == raceId)// && d.Player.ClassificationId == classificationId)
        .ToDictionaryAsync(d => d.BibNumber, token);

    var timePenalties = (await db.TimePenalties
        .Where(p => p.RaceId == raceId)// && p.Player.ClassificationId == classificationId)
        .ToListAsync(token))
        .ToLookup(p => p.BibNumber);

    var unorderTimingPoints = await db.TimingPoints
        .Where(tp => tp.RaceId == raceId)
        .ToListAsync(token);

    var timingPointsOrder = await db.TimingPointOrders
        .Where(to => to.RaceId == raceId)
        .SingleAsync(token);

    var timingPoints = JsonSerializer.Deserialize<List<int>>(timingPointsOrder.Order)
        .Select(p => unorderTimingPoints.FirstOrDefault(tp => tp.Id == p))
        .ToList();

    var race = await db.Races
        .Where(r => r.Id == raceId)
        .Select(r => new { date = r.Date })
        .SingleAsync(token);

    var classification = await db.Classifications
        .Where(c => c.RaceId == raceId && c.Id == classificationId)
        .Include(c => c.Categories)
        .SingleAsync(token);

    var raceDateStart = race!.date;

    var startTimingPoint = timingPoints.First();
    var endTimingPoint = timingPoints.Last();

    var splitTimesMap = splitTimes
        .Select(st => new KeyValuePair<string, long>($"{st.BibNumber}.{st.TimingPointId}.{st.Lap}", (int)st.Time))
        .ToArray();

    var manualSplitTimesMap = manualSplitTimes
        .Select(st => new KeyValuePair<string, long>($"{st.BibNumber}.{st.TimingPointId}.{st.Lap}", (int)st.Time!))
        .ToArray();

    var startTimesMap = allPlayers
        .Select(p => new KeyValuePair<string, long>($"{p.BibNumber}.{startTimingPoint.Id}.0", raceDateStart + p.StartTime!.Value))
        .ToArray();

    var allTimesMap = Utils.ConvertToNestedDictionary(startTimesMap.Concat(splitTimesMap).Concat(manualSplitTimesMap).ToArray());

    var playersWithTimes = allPlayers
        .Select(p => new RaceResultItem
        {
            Id = p.Id,
            BibNumber = p.BibNumber,
            Name = p.PlayerProfile.Name,
            LastName = p.PlayerProfile.LastName,
            ClassificationId = p.ClassificationId,
            Team = p.PlayerProfile.Team,
            Gender = p.PlayerProfile.Gender,
            Age = Utils.CalculateAgeFromEpochMilliseconds(p.PlayerProfile.BirthDate),
            YearOfBirth = Utils.GetBirthYearFromEpochMilliseconds(p.PlayerProfile.BirthDate),
            Times = allTimesMap.GetValueOrDefault(p.BibNumber, new Dictionary<string, Dictionary<string, long>>()),
            Absences = p.Absences.ToDictionary(a => a.TimingPointId, a => true),
            Disqualification = disqualifications.GetValueOrDefault(p.BibNumber),
            TimePenalties = timePenalties[p.BibNumber].ToList() ?? new List<TimePenalty>(),
            TotalTimePenalty = (timePenalties[p.BibNumber] ?? new List<TimePenalty>()).Sum(curr => curr.Time)
        }).ToList();

    var times = playersWithTimes.Where(p => p.Disqualification == null).ToList();

    var disqualifiedPlayers = playersWithTimes
        .Where(d => d.Disqualification != null)
        .Select(t => t with
        {
            InvalidState = "dsq",
            Start = null,
            Finish = null,
            Result = int.MaxValue,
            AgeCategory = null,
            OpenCategory = null
        }).ToList(); ;

    var absentPlayers = times
        .Where(t => t.Absences.ContainsKey(startTimingPoint!.Id) || t.Absences.ContainsKey(endTimingPoint!.Id))
        .Select(t => t with
        {
            InvalidState = t.Absences.ContainsKey(startTimingPoint!.Id) ? "dns" : t.Absences.ContainsKey(endTimingPoint!.Id) ? "dnf" : null,
            Start = null,
            Finish = null,
            Result = int.MaxValue,
            AgeCategory = null,
            OpenCategory = null
        }).ToList(); ;

    var results = times
        .Where(t => t.Times.ContainsKey(startTimingPoint!.Id.ToString()) && t.Times.ContainsKey(endTimingPoint!.Id.ToString()))
        .Select(t => t with
        {
            Start = t.Times[startTimingPoint.Id.ToString()]["0"],
            Finish = t.Times[endTimingPoint.Id.ToString()]["0"],
            Result = t.Times[endTimingPoint.Id.ToString()]["0"] - t.Times[startTimingPoint.Id.ToString()]["0"] + t.TotalTimePenalty,
            InvalidState = (string)null
        }).ToList(); ;

    var resultsWithCategories = results
        .Select(r => r with
        {
            AgeCategory = classification.Categories.FirstOrDefault(c => c.MinAge != null && c.MaxAge != null && c.MinAge <= r.Age && c.MaxAge >= r.Age && (c.Gender == null || c.Gender == r.Gender)),
            OpenCategory = classification.Categories.FirstOrDefault(c => c.MinAge == null && c.MaxAge == null && c.Gender != null && c.Gender == r.Gender)
        }).ToList(); ;

    var playersByAgeCategories = resultsWithCategories
        .Where(r => r.AgeCategory != null)
        .GroupBy(r => r.AgeCategory.Id.ToString())
        .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Result).ToList());

    var playersByOpenCategories = resultsWithCategories
        .Where(r => r.OpenCategory != null)
        .GroupBy(r => r.OpenCategory.Id.ToString())
        .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Result).ToList());

    var sorted = resultsWithCategories
        .Concat(absentPlayers)
        .Concat(disqualifiedPlayers)
        .OrderBy(r => r.Result)
        .Select(r => r with
        {
            AgeCategoryPlace = r.AgeCategory != null ? playersByAgeCategories[r.AgeCategory.Id.ToString()].IndexOf(r) + 1 : (int?)null,
            OpenCategoryPlace = r.OpenCategory != null ? playersByOpenCategories[r.OpenCategory.Id.ToString()].IndexOf(r) + 1 : (int?)null
        }).ToList(); ;

    var winningResult = sorted.FirstOrDefault()?.Result;

    var result = sorted
        .Select(s => s with
        {
            Gap = winningResult != null && s.Result != null ? s.Result - winningResult : (int?)null
        })
        .ToList();

    return result;
});

app.Run();

public record RaceResultItem
{
    public int Id { get; init; }
    public string BibNumber { get; init; }
    public string Name { get; init; }
    public string LastName { get; init; }
    public int ClassificationId { get; init; }
    public string Team { get; init; }
    public string Gender { get; init; }
    public int Age { get; init; }
    public int YearOfBirth { get; init; }
    public Dictionary<string, Dictionary<string, long>> Times { get; init; }
    public Dictionary<int, bool> Absences { get; init; }
    public Disqualification Disqualification { get; init; }
    public List<TimePenalty> TimePenalties { get; init; }
    public int TotalTimePenalty { get; init; }
    public long? Start { get; init; }
    public long? Finish { get; init; }
    public long Result { get; init; }
    public string InvalidState { get; init; }
    public Category AgeCategory { get; init; }
    public Category OpenCategory { get; init; }
    public int? AgeCategoryPlace { get; init; }
    public int? OpenCategoryPlace { get; init; }
    public long? Gap { get; init; }
}

public record DisqualificationInfo
{
    public int Id { get; init; }
    public string Reason { get; init; }
}

public record TimePenaltyInfo
{
    public int Id { get; init; }
    public int Time { get; init; }
    public string Reason { get; init; }
}

public record AgeCategoryInfo
{
    public int Id { get; init; }
    // Add other properties as needed
}

public record OpenCategoryInfo
{
    public int Id { get; init; }
    // Add other properties as needed
}

static class Utils
{
    public static Dictionary<string, long> MergeKeyValuePairs(params KeyValuePair<string, long>[] keyValuePairs)
    {
        var mergedDictionary = new Dictionary<string, long>();

        foreach (var kvp in keyValuePairs)
        {
            // If the key already exists, override the value
            if (mergedDictionary.ContainsKey(kvp.Key))
            {
                mergedDictionary[kvp.Key] = kvp.Value;
            }
            else
            {
                // If the key doesn't exist, add it to the dictionary
                mergedDictionary.Add(kvp.Key, kvp.Value);
            }
        }

        return mergedDictionary;
    }

    public static int CalculateAgeFromEpochMilliseconds(long epochMilliseconds)
    {
        // Convert epoch milliseconds to a DateTime object
        DateTime birthDate = DateTimeOffset.FromUnixTimeMilliseconds(epochMilliseconds).UtcDateTime;

        // Calculate the age
        int age = CalculateAge(birthDate);

        return age;
    }

    public static int GetBirthYearFromEpochMilliseconds(long epochMilliseconds)
    {
        // Convert epoch milliseconds to a DateTime object
        DateTime birthDate = DateTimeOffset.FromUnixTimeMilliseconds(epochMilliseconds).UtcDateTime;

        // Extract the birth year
        int birthYear = birthDate.Year;

        return birthYear;
    }

    public static int CalculateAge(DateTime birthDate)
    {
        DateTime currentDate = DateTime.UtcNow;
        int age = currentDate.Year - birthDate.Year;

        // Check if the birthday has occurred this year
        if (birthDate > currentDate.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public static Dictionary<string, Dictionary<string, Dictionary<string, long>>> ConvertToNestedDictionary(IEnumerable<KeyValuePair<string, long>> keyValuePairs)
    {
        var resultDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();

        foreach (var kvp in keyValuePairs)
        {
            var keys = kvp.Key.Split('.');
            var outerKey = keys[0];
            var middleKey = keys[1];
            var innerKey = keys[2];

            if (!resultDictionary.ContainsKey(outerKey))
            {
                resultDictionary[outerKey] = new Dictionary<string, Dictionary<string, long>>();
            }

            var middleDictionary = resultDictionary[outerKey];

            if (!middleDictionary.ContainsKey(middleKey))
            {
                middleDictionary[middleKey] = new Dictionary<string, long>();
            }

            var innerDictionary = middleDictionary[middleKey];


            innerDictionary[innerKey] = kvp.Value;
        }

        return resultDictionary;
    }
}
