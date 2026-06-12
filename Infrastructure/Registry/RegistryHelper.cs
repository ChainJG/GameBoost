using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Infrastructure.Registry
{
    public static class RegistryHelper
    {
        public static RegistryResult OpenKey(RegistryEditInfo edit, bool writable = false)
        {
            try
            {
                var baseResult =
                    RegistryKey.OpenBaseKey(
                        edit.Hive,
                        RegistryView.Registry64) ?? throw new Exception("Registry hive not found");

                var key = baseResult.OpenSubKey(
                    edit.Path,
                    writable) ?? throw new Exception("Registry path not found");

                return new RegistryResult
                {
                    Success = true,
                    Key = key,
                };
            }
            catch (UnauthorizedAccessException)
            {
                return RegistryResult.Failed("Administrator Permission Required", ResultType.Administrator);
            }
            catch (System.Security.SecurityException)
            {
                return RegistryResult.Failed("Administrator Permission Required");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Registry OpenKey({edit.Key}) Error: {ex.Message}");
#endif
                return RegistryResult.Failed(ex.Message);
            }
        }
        public static RegistryResult DeleteKey(RegistryEditInfo edit)
        {
            try
            {
                var result = OpenKey(edit, true);

                if (!result.Success || result.Key == null)
                    return RegistryResult.Failed(result.Message);

                using var key = result.Key;

                // Nothing to delete, so this operation can be treated as successful
                if (!key.GetValueNames().Contains(edit.Key))
                    return RegistryResult.Successful($"{edit.Key} does not exist");

                key.DeleteValue(edit.Key, throwOnMissingValue: false);

                return RegistryResult.Successful($"Successfully deleted {edit.Key}");
            }
            catch (UnauthorizedAccessException)
            {
                return RegistryResult.Failed("Administrator Permission Required", ResultType.Administrator);
            }
            catch (System.Security.SecurityException)
            {
                return RegistryResult.Failed("Administrator Permission Required");
            }
            catch (Exception ex)
            {
                return RegistryResult.Failed(ex.Message);
            }
        }
        public static RegistryResult GetValue(RegistryEditInfo edit)
        {
            try
            {
                var result = OpenKey(edit);

                if (!result.Success || result.Key is null)
                    return RegistryResult.Failed(result.Message);

                using var key = result.Key;

                var value = key.GetValue(edit.Key);

                return RegistryResult.Successful(
                    $"Successfully Retrieved {edit.Key}",
                    value);

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Registry GetValue() Error: {ex.Message}");
#endif
                return RegistryResult.Failed(ex.Message);
            }
        }
        public static RegistryResult SetValue(RegistryEditInfo edit, object value)
        {
            try
            {
                if (edit == null)
                    throw new Exception("Data is null");

                var result = OpenKey(edit, true);

                if (!result.Success)
                    return result;

                if (edit.Kind is RegistryValueKind kind)
                    result.Key?.SetValue(edit.Key, value, kind);
                else
                    result.Key?.SetValue(edit.Key, value);

                result.Key?.Close();
                result.Key?.Dispose();

                return RegistryResult.Successful($"Successfully Set {edit.Key}");
            }
            catch (UnauthorizedAccessException)
            {
                return RegistryResult.Failed("Administrator Permission Required", ResultType.Administrator);
            }
            catch (System.Security.SecurityException)
            {
                return RegistryResult.Failed("Administrator Permission Required");
            }
            catch (Exception ex)
            {
                return RegistryResult.Failed(ex.Message);
            }
        }

        public static ToggleType GetGroupedEnabledStatus(IEnumerable<RegistryEditInfo> edits, bool treatReadFailureAsDisabled = true)
        {
            var editList = edits.ToList();

            if (editList.Count == 0)
                return ToggleType.Unknown;

            foreach (var edit in editList)
            {
                var result = GetValue(edit);

                if (!result.Success)
                    return treatReadFailureAsDisabled ? ToggleType.Disabled : ToggleType.Unknown;

                if (!RegistryValuesMatch(result.Value, edit.EnabledValue))
                    return ToggleType.Disabled;
            }

            return ToggleType.Enabled;
        }


        public static bool RegistryStateMatches(
            object? currentValue,
            bool valueExists,
            RegistryValueAction expectedAction,
            object? expectedValue)
        {
            return expectedAction switch
            {
                RegistryValueAction.Set => valueExists && RegistryValuesMatch(currentValue, expectedValue),
                RegistryValueAction.Delete => !valueExists,
                RegistryValueAction.Ignore => false,

                _ => false
            };
        }
        public static bool RegistryValuesMatch(object? currentValue, object? expectedValue)
        {
            if (currentValue is null || expectedValue is null)
                return false;

            if (currentValue is byte[] currentBytes && expectedValue is byte[] expectedBytes)
                return currentBytes.SequenceEqual(expectedBytes);

            return string.Equals(
                currentValue.ToString(),
                expectedValue.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
