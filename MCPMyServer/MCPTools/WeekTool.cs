using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPMyServer.MCPTools
{
    [McpServerToolType]
    public static class WeekTool
    {
        [McpServerTool, Description("Get the current day of the week")]
        public static string GetDayOfWeek()
        {
            var today = DateTime.Now.ToString("dddd");
            return $"Today is {today}.";
        }
    }
}
