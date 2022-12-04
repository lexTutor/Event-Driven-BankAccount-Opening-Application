using static BankAccount.Shared.Domain.RecordTypes;

namespace BankAccount.Shared.Utilities
{
    public static class Helper
    {
        public static IEnumerable<EnumModel> ToEnumModelList(this Enum value)
        {
            var type = value.GetType();
            IEnumerable<int> values = Enum.GetValues(type).Cast<int>();

            return values.Select(x => new EnumModel(x, Enum.GetName(type, x).Replace("_", " "))).ToList();
        }

        public static string RandomDigits()
        {
            var random = new Random();
            string digits = string.Empty;
            for (int i = 0; i < 10; i++)
                digits = string.Concat(digits, random.Next(10).ToString());

            return digits;
        }
    }
}
