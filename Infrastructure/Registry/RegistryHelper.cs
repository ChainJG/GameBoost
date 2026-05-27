using GameBoost.Shared.Results;
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
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Registry OpenKey() Error: {ex.Message}");
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
                return RegistryResult.Failed("Administrator Permission Required", ResultType.AdministratorProtection);
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

                result.Key?.SetValue(edit.Key, value);

                result.Key?.Close();
                result.Key?.Dispose();

                return RegistryResult.Successful($"Successfully Set {edit.Key}");
            }
            catch (UnauthorizedAccessException)
            {
                return RegistryResult.Failed("Administrator Permission Required", ResultType.AdministratorProtection);
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
    }
}
