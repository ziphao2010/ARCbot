using System.IO;

namespace ARCbot.Helpers;

/// <summary>
/// ���й��� %APPDATA%/ARCbot/ �µ�����·������֤��ɫ���С�
/// </summary>
public static class PathHelper
{
    /// <summary>��Ŀ¼ %APPDATA%/ARCbot/</summary>
    public static string RootDir { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ARCbot");

    /// <summary>Node.js ��Я����ʱĿ¼</summary>
    public static string RuntimeDir => Path.Combine(RootDir, "runtime");

    /// <summary>node.exe ����·��</summary>
    public static string NodeExePath => Path.Combine(RuntimeDir, "node.exe");

    /// <summary>ʵ����Ŀ¼</summary>
    public static string InstancesDir => Path.Combine(RootDir, "Instances");

    /// <summary>���ػ���Ŀ¼</summary>
    public static string DownloadsDir => Path.Combine(RootDir, "downloads");

    /// <summary>������ zip ����·��</summary>
    public static string BaseAgentZipPath => Path.Combine(DownloadsDir, "base_agent.zip");

    /// <summary>自定义基础包 zip 存储路径</summary>
    public static string CustomBaseAgentZipPath => Path.Combine(DownloadsDir, "custom_base_agent.zip");

    /// <summary>Ӧ�������ļ�</summary>
    public static string AppSettingsPath => Path.Combine(RootDir, "settings.json");

    /// <summary>��ȡָ��ʵ���ĸ�Ŀ¼</summary>
    public static string GetInstanceDir(string instanceName) =>
        Path.Combine(InstancesDir, instanceName);

    /// <summary>��ȡָ��ʵ���� src Ŀ¼��Node.js ����Ŀ¼��</summary>
    public static string GetInstanceSrcDir(string instanceName) =>
        Path.Combine(GetInstanceDir(instanceName), "src");

    /// <summary>��ȡָ��ʵ���� .env �ļ�·��</summary>
    public static string GetInstanceEnvPath(string instanceName) =>
        Path.Combine(GetInstanceDir(instanceName), ".env");

    /// <summary>��ȡָ��ʵ���� plugins Ŀ¼</summary>
    public static string GetInstancePluginsDir(string instanceName) =>
        Path.Combine(GetInstanceDir(instanceName), "plugins");

    /// <summary>日志目录</summary>
    public static string LogsDir => Path.Combine(RootDir, "logs");

    /// <summary>确保所有必要目录存在</summary>
    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(RootDir);
        Directory.CreateDirectory(RuntimeDir);
        Directory.CreateDirectory(InstancesDir);
        Directory.CreateDirectory(DownloadsDir);
        Directory.CreateDirectory(LogsDir);
    }
}