using System.Diagnostics;

namespace GameManager.Domain.ValueObjects
{
    [DebuggerDisplay("{Value}")]
    public class TrackerName
    {
        public const int MinimumLength = 1;

        public const int MaximumLength = 20;

        public string Value { get; }

        public static Result<TrackerName> From(string value)
        {
            string trimmedValue = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmedValue))
            {
                return Result.Failure<TrackerName>("Name is required");
            }

            if (trimmedValue.Length < MinimumLength)
            {
                return Result.Failure<TrackerName>("Name is too short");
            }

            if (trimmedValue.Length > MaximumLength)
            {
                return Result.Failure<TrackerName>("Name is too long");
            }

            return new TrackerName(trimmedValue);
        }

        private TrackerName(string value)
        {
            Value = value;
        }

        private bool Equals(TrackerName other)
            => string.Equals(other.Value, Value, StringComparison.CurrentCultureIgnoreCase);

        public override bool Equals(object? obj)
            => ReferenceEquals(this, obj) || obj is TrackerName other && Equals(other);

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}