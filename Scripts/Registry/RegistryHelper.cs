using GameBoost.Scripts.Registry.Model;
using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Scripts.Registry
{
    public static class RegistryHelper
    {
        public static RegistryResult OpenKey(RegistryEditInfo edit, bool writable = false)
        {
            try
            {
                var baseReuslt =
                    RegistryKey.OpenBaseKey(
                        edit.Hive,
                        RegistryView.Registry64) ?? throw new Exception("Registry hive not found");

                var key = baseReuslt.OpenSubKey(
                    edit.Path,
                    writable) ?? throw new Exception("Registry path not found");

                return new RegistryResult
                {
                    Success = true,
                    key = key,
                };
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Registry OpenKey() Error: {ex.Message}");
#endif
                return new RegistryResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public static RegistryResult DeleteKey(RegistryEditInfo edit)
        {
            try
            {
                var baseKey = OpenKey(edit) ?? throw new Exception("Registry hive not found");

                using var key = baseKey.key?.OpenSubKey(
                    edit.Path,
                    writable: true) ?? throw new Exception("Registry path not found");

                // Check if value exists
                if (!key.GetValueNames().Contains(edit.Key))
                {
                    return new RegistryResult
                    {
                        Success = true,
                    };
                }

                key.DeleteValue(edit.Key, throwOnMissingValue: false);

                return new RegistryResult
                {
                    Success = true,
                    Message = $"Successfully deleted {edit.Key}"
                };

            }
            catch (UnauthorizedAccessException)
            {
                return new RegistryResult
                {
                    Success = false,
                    Message = "Administrator permission required"
                };
            }
            catch (System.Security.SecurityException)
            {
                return new RegistryResult
                {
                    Success = false,
                    Message = "Security policy blocked registry access"
                };
            }
            catch (Exception ex)
            {
                return new RegistryResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public static object? GetValue(RegistryEditInfo edit)
        {
            try
            {
                var value = OpenKey(edit);

                return value.Success ? (value.key?.GetValue(edit.Key)) : null;

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Registry GetValue() Error: {ex.Message}");
#endif
                return null;
            }
        }
        public static RegistryResult SetValue(RegistryEditInfo edit, object value)
        {
            try
            {
                if (edit == null)
                    throw new Exception("Daata is null");

                var reuslt = OpenKey(edit, true);

                if (!reuslt.Success)
                    return reuslt;

                reuslt.key?.SetValue(edit.Key, value);

                reuslt.key?.Close();
                reuslt.key?.Dispose();

                return new RegistryResult
                {
                    Success = true,
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new RegistryResult
                {
                    Success = false,
                    Message = "Administrator permission required",
                };
            }
            catch (System.Security.SecurityException)
            {
                return new RegistryResult
                {
                    Success = false,
                    Message = "Security policy blocked access",
                };
            }
            catch (Exception ex)
            {
                return new RegistryResult
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
