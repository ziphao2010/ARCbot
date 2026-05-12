namespace ARCbot.Models;

public class AppSettings
{
    public bool IsDarkMode { get; set; } = true;
    public double BgOpacity { get; set; } = 0.6;
    public string BackgroundImagePath { get; set; } = "pack://application:,,,/Assets/bg2.png";

    // --- 基础包设置 ---
    public bool UseCustomBaseAgent { get; set; } = false;
    public string CustomBaseAgentPath { get; set; } = string.Empty;

    // --- 弹窗设置 ---
    public bool DisableAfdianPopup { get; set; } = false;

    // --- 更新设置 ---
    public string SkippedVersion { get; set; } = string.Empty;
    public string UpdateCheckUrl { get; set; } = "http://localhost:3000/update";

    // --- 下载源设置 ---
    public string NodeJsDownloadUrl { get; set; } = "https://nodejs.org/dist/v20.11.1/node-v20.11.1-win-x64.zip";
    public string BaseAgentDownloadUrl { get; set; } = "https://zip1.webgetstore.com/2026/04/13/6b22070a3b7dc42cc840faf020ff0ff4.zip?sg=4ae7284f4af496ed2f49bfca2b8d17d1&e=69dc6ea3&fileName=minecraft-ai-agent.zip&fi=282406185";
}
