using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InsightMed.Infrastructure.Data.Converters;

public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            // Write to DB as-is
            v => v,
            // Read from DB and force Kind=Utc
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}