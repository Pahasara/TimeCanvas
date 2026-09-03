using System.Text.Json.Serialization;

namespace TimeCanvas.Core.Models;

[JsonSerializable(typeof(AppSettings))]
internal partial class AppSettingsJsonContext : JsonSerializerContext;
