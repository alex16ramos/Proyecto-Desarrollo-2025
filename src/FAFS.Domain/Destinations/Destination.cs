using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Values;

namespace FAFS.Destinations
{
    public class Destination : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public Coordinates Coordinates { get; set; } = default!;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // EF requiere ctor protegido
        protected Destination() { }

        public Destination(
            Guid id,
            string name,
            string country,
            string city,
            string photoUrl,
            DateTime lastUpdated,
            Coordinates coordinates,
            string description = ""
        ) : base(id)
        {
            Name = name;
            Country = country;
            City = city;
            PhotoUrl = photoUrl;
            LastUpdated = lastUpdated;
            Coordinates = coordinates;
            Description = description;
        }
    }

    // ✅ Value Object correcto
    public class Coordinates : ValueObject
    {
        public string Latitude { get; private set; } = string.Empty;
        public string Longitude { get; private set; } = string.Empty;

        // EF necesita constructor protegido
        protected Coordinates() { }

        public Coordinates(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        // Esto hace que el ValueObject compare por contenido
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Latitude;
            yield return Longitude;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Coordinates other)
            {
                return Latitude == other.Latitude && Longitude == other.Longitude;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }

        public static bool operator ==(Coordinates? left, Coordinates? right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Coordinates? left, Coordinates? right) => !(left == right);
    }
}
