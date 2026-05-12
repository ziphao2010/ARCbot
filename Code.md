# ARCbot — 项目代码深度分析文档

> **项目名称**: ARCbot  
> **版本**: v1.0.3  
> **描述**: 新一代 Minecraft AI 机器人多开启动器（Windows WPF 桌面应用）  
> **作者**: FENTAI (GitHub: FENTAIIII / 爱发电: fentai2333)  
> **交流群**: QQ 1082669385  
> **文档生成日期**: 2026-05-12

---

## 目录

1. [项目概览](#1-项目概览)
2. [技术栈与依赖](#2-技术栈与依赖)
3. [项目目录结构](#3-项目目录结构)
4. [架构设计](#4-架构设计)
5. [应用启动流程](#5-应用启动流程)
6. [核心模块详解](#6-核心模块详解)
   - 6.1 [入口与配置 (App.xaml / App.xaml.cs)](#61-入口与配置-appxaml--appxamlcs)
   - 6.2 [主窗口 (MainWindow)](#62-主窗口-mainwindow)
   - 6.3 [页面服务 (PageService)](#63-页面服务-pageservice)
   - 6.4 [数据模型层 (Models)](#64-数据模型层-models)
   - 6.5 [服务层 (Services)](#65-服务层-services)
   - 6.6 [视图模型层 (ViewModels)](#66-视图模型层-viewmodels)
   - 6.7 [视图层 (Views)](#67-视图层-views)
   - 6.8 [工具辅助层 (Helpers)](#68-工具辅助层-helpers)
   - 6.9 [静态资源 (Assets)](#69-静态资源-assets)
7. [数据流与交互](#7-数据流与交互)
8. [关键设计模式](#8-关键设计模式)
9. [全局资源与主题系统](#9-全局资源与主题系统)
10. [安全与隐私考虑](#10-安全与隐私考虑)
11. [扩展性与可维护性](#11-扩展性与可维护性)
12. [文件清单](#12-文件清单)

---

## 1. 项目概览

ARCbot 是一个基于 **WPF (.NET 8)** 的 Windows 桌面应用程序，用于管理和运行 **Minecraft AI 机器人实例**。它的核心功能是：

- **多实例管理**: 创建、编辑、删除、启动/停止多个 Minecraft AI 机器人实例
- **环境配置**: 为每个实例配置独立的 `.env` 文件，包含 Minecraft 服务器连接信息、LLM API 密钥、行为开关等
- **Node.js 运行时管理**: 内置 Node.js 便携版的下载与解压功能，无需用户预先安装
- **基础包管理**: 支持从云端下载机器人基础包，也支持导入自定义本地 zip 包
- **控制台监控**: 实时查看每个运行中实例的 stdout/stderr 输出，支持向进程 stdin 发送指令
- **微软登录拦截**: 自动检测 Node.js 进程输出中的微软设备码登录提示，弹出全局验证码窗口
- **插件管理**: 为每个实例管理 `.js` 插件文件，支持导入和删除
- **内置 WebView2 页面**: 插件市场、免费体验、帮助文档均通过嵌入式 WebView2 加载远程网页
- **自动更新**: 启动时检查服务器版本，支持下载更新并通过批处理脚本替换当前 exe
- **个性化设置**: 暗色/亮色主题切换、背景图片（预设 6 张 + 自定义）、背景遮罩透明度调节
- **爱发电弹窗**: 启动时显示支持作者弹窗（可永久关闭）

---

## 2. 技术栈与依赖

### 2.1 .NET SDK 与目标框架

| 配置项 | 值 |
|--------|-----|
| SDK | `Microsoft.NET.Sdk` |
| 输出类型 | `WinExe` (Windows 可执行文件) |
| 目标框架 | `net8.0-windows` |
| UI 框架 | WPF (`UseWPF` = true) |
| 可空引用类型 | 启用 (`Nullable` = enable) |
| 隐式 Using | 启用 (`ImplicitUsings` = enable) |
| 程序集名 | `ARCbot` |
| 根命名空间 | `ARCbot` |
| 应用图标 | `Assets/icon.ico` |
| 版本号 | `1.0.3` |

### 2.2 NuGet 包依赖

| 包名 | 版本 | 用途 |
|------|------|------|
| `Microsoft.Web.WebView2` | 1.0.3912.50 | 内嵌 Chromium 浏览器控件，用于加载插件市场、体验页、文档页等远程网页 |
| `WPF-UI` | 3.* | 现代化 WPF UI 库，提供 Fluent Design 风格的导航、按钮、图标、主题等控件 |
| `CommunityToolkit.Mvvm` | 8.* | MVVM 工具包，提供 `ObservableObject`、`[ObservableProperty]`、`[RelayCommand]` 等源代码生成器 |
| `Microsoft.Extensions.DependencyInjection` | 8.* | 依赖注入容器 (DI)，用于服务注册与解析 |
| `Microsoft.Extensions.Hosting` | 8.* | 通用主机基础设施（本项目中主要用于引入 DI 相关扩展） |

### 2.3 构建配置特性

- **单文件发布**: `IncludeNativeLibrariesForSelfExtract` = true，将 C++ 原生库（WPF 底层和 WebView2）嵌入 EXE
- **无调试符号**: `DebugType` = None, `DebugSymbols` = false
- **无 XML 文档**: `PublishDocumentationFiles` = false

---

## 3. 项目目录结构

```
ARCbot/
├── App.xaml                          # 应用程序全局资源定义（主题、样式、颜色）
├── App.xaml.cs                       # 应用程序入口，DI 容器配置，启动流程
├── MainWindow.xaml                   # 主窗口布局（导航、弹窗、标题栏）
├── MainWindow.xaml.cs                # 主窗口逻辑（导航、背景、弹窗、页面服务）
├── ARCbot.csproj                     # 项目文件（依赖、构建配置）
├── MemberChecker.cs                  # 已废弃的调试辅助类（反射检查 Wpf.Ui 类型成员）
│
├── Assets/                           # 静态资源
│   ├── bg1.png ~ bg6.png             # 6 张预设背景图片（作为 Resource 嵌入）
│   └── icon.ico                      # 应用程序图标
│
├── Helpers/                          # 工具辅助类
│   ├── Converters.cs                 # WPF 值转换器（布尔取反、布尔转 Visibility）
│   └── PathHelper.cs                 # 全局路径常量（AppData 目录结构）
│
├── Models/                           # 数据模型
│   ├── AppSettings.cs                # 应用设置模型（主题、透明度、背景、基础包等）
│   ├── BotInstance.cs                # 机器人实例模型（含所有 .env 配置字段）
│   ├── ConsoleEntry.cs               # 控制台日志条目模型（时间戳、级别、颜色）
│   └── UpdateInfo.cs                 # 更新信息模型（版本、下载地址、更新日志）
│
├── Services/                         # 业务服务层
│   ├── AuthInterceptor.cs            # 微软登录验证码拦截器（正则匹配 stdout）
│   ├── DownloadService.cs            # 文件下载服务（Node.js、基础包、解压）
│   ├── EnvManager.cs                 # .env 文件读写服务（双向转换）
│   ├── InstanceManager.cs            # 实例 CRUD 与生命周期管理
│   ├── NodeProcessManager.cs         # 单个 Node.js 进程管理器
│   ├── SettingsService.cs            # 应用设置持久化服务（JSON 读写）
│   └── UpdateService.cs              # 自动更新检查服务
│
├── ViewModels/                       # 视图模型（MVVM 的 VM 层）
│   ├── MainWindowViewModel.cs        # 主窗口 VM（标题、当前页面）
│   ├── HomeViewModel.cs              # 首页 VM（实例列表、CRUD 命令）
│   ├── ConsoleViewModel.cs           # 控制台 VM（运行实例列表、日志、输入）
│   ├── CreateInstanceViewModel.cs    # 创建/编辑实例 VM（表单、验证）
│   ├── PluginsViewModel.cs           # 插件管理 VM（列表、导入、删除）
│   └── SettingsViewModel.cs          # 设置页 VM（主题、背景、下载、关于）
│
└── Views/                            # 视图层
    ├── Dialogs/                      # 对话框窗口
    │   ├── CreateInstanceDialog.xaml      # 创建/编辑实例对话框布局
    │   ├── CreateInstanceDialog.xaml.cs   # 对话框逻辑（模式切换、密码同步）
    │   ├── UpdateDialog.xaml             # 更新提示对话框布局
    │   └── UpdateDialog.xaml.cs          # 更新下载与安装逻辑
    │
    └── Pages/                        # 导航页面
        ├── HomePage.xaml             # 首页（实例卡片网格、空状态、FAB 按钮）
        ├── HomePage.xaml.cs          # 首页逻辑
        ├── ConsolePage.xaml          # 控制台页（实例列表、日志输出、输入框）
        ├── ConsolePage.xaml.cs       # 控制台逻辑（自动滚动、回车发送）
        ├── PluginsPage.xaml          # 插件管理页（实例选择、插件列表）
        ├── PluginsPage.xaml.cs       # 插件页逻辑
        ├── SettingsPage.xaml         # 设置页（运行环境、个性化、关于）
        ├── SettingsPage.xaml.cs      # 设置页逻辑（进度条控制）
        ├── MarketPage.xaml           # 插件市场页（内嵌 WebPage）
        ├── MarketPage.xaml.cs        # 加载 https://bot.myarc.icu/market
        ├── ExperiencePage.xaml       # 免费体验页（内嵌 WebPage）
        ├── ExperiencePage.xaml.cs    # 加载 https://bot.myarc.icu/experience
        ├── DocsPage.xaml             # 帮助文档页（内嵌 WebPage）
        ├── DocsPage.xaml.cs          # 加载 https://bot.myarc.icu/docs
        ├── WebPage.xaml              # 通用 WebView2 包装控件
        └── WebPage.xaml.cs           # WebView2 初始化与环境配置
```

---

## 4. 架构设计

### 4.1 整体架构

ARCbot 采用经典的 **MVVM (Model-View-ViewModel)** 架构模式，结合 **依赖注入 (DI)** 实现松耦合。

```
┌─────────────────────────────────────────────────────────────┐
│                        Views Layer                          │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │ HomePage │  │ConsolePage│ │PluginsPage│ │SettingsPage│  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘   │
│       │              │              │              │         │
│  ┌────┴─────┐  ┌────┴─────┐  ┌────┴─────┐  ┌────┴─────┐   │
│  │  Dialogs  │  │ WebPage  │  │  Dialogs  │  │MainWindow│   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
├─────────────────────────────────────────────────────────────┤
│                      ViewModels Layer                       │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │  HomeVM  │  │ConsoleVM │  │PluginsVM │  │SettingsVM│   │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘   │
│  ┌────┴──────────────────────────────────────────┴─────┐   │
│  │              MainWindowViewModel                     │   │
│  │              CreateInstanceViewModel                 │   │
│  └──────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│                      Services Layer                         │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│  │InstanceMgr│ │EnvManager│ │AuthInterc│ │DownloadSvc│     │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘      │
│  ┌────┴─────┐ ┌────┴─────┐ ┌────┴─────┐ ┌────┴─────┐      │
│  │NodeProcMgr│ │SettingsSvc│ │UpdateSvc │                      │
│  └──────────┘ └──────────┘ └──────────┘                      │
├─────────────────────────────────────────────────────────────┤
│                       Models Layer                          │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│  │BotInstance│ │AppSettings│ │ConsoleEntry│ │UpdateInfo│     │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘      │
├─────────────────────────────────────────────────────────────┤
│                      Helpers Layer                          │
│  ┌──────────┐ ┌──────────┐                                  │
│  │PathHelper │ │Converters│                                  │
│  └──────────┘ └──────────┘                                  │
└─────────────────────────────────────────────────────────────┘
```

### 4.2 依赖注入设计

应用启动时在 `App.OnStartup()` 中配置 DI 容器，服务的生命周期策略如下：

| 生命周期 | 服务/组件 | 理由 |
|----------|-----------|------|
| **Singleton** | `AuthInterceptor` | 全局唯一，所有实例共享一个拦截器 |
| **Singleton** | `EnvManager` | 无状态工具类 |
| **Singleton** | `DownloadService` | 全局唯一，避免并发下载冲突 |
| **Singleton** | `InstanceManager` | 全局实例注册中心，维护所有实例状态 |
| **Singleton** | `SettingsService` | 全局设置，单例加载避免重复 IO |
| **Singleton** | `UpdateService` | 仅启动时使用一次 |
| **Singleton** | `MainWindowViewModel` | 主窗口唯一 |
| **Singleton** | `ConsoleViewModel` | 需跨页面保持运行实例日志状态 |
| **Singleton** | `SettingsViewModel` | 需保持下载状态等 |
| **Singleton** | `SettingsPage` | 设置页面唯一实例 |
| **Singleton** | `MainWindow` | 主窗口唯一 |
| **Transient** | `HomeViewModel` | 每次导航创建新实例 |
| **Transient** | `CreateInstanceViewModel` | 每次打开对话框创建新实例 |
| **Transient** | `PluginsViewModel` | 每次导航创建新实例 |
| **Transient** | 所有 Page 类（除 SettingsPage） | 按需创建，支持导航 |

### 4.3 数据存储架构

所有数据存储在 `%APPDATA%/ARCbot/` 下：

```
%APPDATA%/ARCbot/
├── settings.json                    # 应用设置（JSON 格式）
├── runtime/
│   └── node.exe                     # 便携版 Node.js 运行时
├── downloads/
│   ├── base_agent.zip               # 云端基础包
│   └── custom_base_agent.zip        # 自定义基础包
└── Instances/
    └── <实例名>/
        ├── .env                     # 实例环境配置
        ├── src/                     # 机器人源代码（从基础包解压）
        │   └── index.js             # Node.js 入口文件
        └── plugins/                 # 实例插件目录
            └── *.js                 # 自定义技能插件
```

---

## 5. 应用启动流程

应用启动遵循以下严格顺序：

```
1. App.OnStartup()
   │
   ├─ 2. PathHelper.EnsureDirectories()
   │     └─ 创建 %APPDATA%/ARCbot/{runtime, Instances, downloads}
   │
   ├─ 3. ConfigureServices() → BuildServiceProvider()
   │     └─ 注册所有服务、ViewModel、Page、Window 到 DI 容器
   │
   ├─ 4. SettingsService.LoadSettings() → 获取 AppSettings
   │
   ├─ 5. ApplicationThemeManager.Apply(暗色/亮色)
   │     └─ 应用 WPF-UI 主题
   │
   ├─ 6. Application.Current.Resources["GlobalOverlayOpacity"] = settings.BgOpacity
   │     └─ 应用全局透明度
   │
   ├─ 7. 创建 MainWindow 实例
   │
   ├─ 8. MainWindow.Loaded 事件
   │     ├─ 8a. UpdateBackground(settings.BackgroundImagePath)
   │     │     └─ 加载背景图（pack URI 或本地路径），设置 Mica/None 背景类型
   │     │
   │     └─ 8b. UpdateService.CheckUpdateAsync()
   │           └─ HTTP GET http://localhost:3000/update
   │              └─ 比较版本号 → 弹出 UpdateDialog（如有新版本）
   │
   ├─ 9. mainWindow.Show()
   │
   ├─ 10. MainWindow.Loaded (内部)
   │      ├─ RootNavigation.Navigate(HomePage)
   │      └─ ShowAfdianPopupIfNeeded()
   │            └─ 检查 DisableAfdianPopup 设置
   │               └─ 延迟 1 秒淡入爱发电弹窗
   │
   └─ 11. 用户交互开始
```

### 应用退出流程

```
App.OnExit()
  ├─ 遍历 InstanceManager.RunningProcesses.Values
  │    └─ 每个 NodeProcessManager.Dispose()
  │         └─ 强制 Kill 进程树 → Dispose Process 对象
  └─ base.OnExit()
```

---

## 6. 核心模块详解

### 6.1 入口与配置 (App.xaml / App.xaml.cs)

#### App.xaml — 全局资源字典

文件位置: [App.xaml](file:///workspace/ARCbot/App.xaml)

定义了应用程序级别的全局资源：

**主题资源合并**:
```xml
<ui:ThemesDictionary />   <!-- WPF-UI 主题系统 -->
<ui:ControlsDictionary />  <!-- WPF-UI 控件样式 -->
```

**全局变量与样式**:

| 资源 Key | 类型 | 默认值 | 用途 |
|----------|------|--------|------|
| `GlobalOverlayOpacity` | `Double` | `0.6` | 全局背景遮罩透明度（运行时可通过代码修改） |
| `UnifiedOverlayBorderStyle` | `Style(Border)` | — | 统一背景遮罩样式（圆角 12、背景跟随主题、透明度绑定全局变量） |
| `LargeCornerRadius` | `CornerRadius` | `16` | 大圆角常量 |
| `MediumCornerRadius` | `CornerRadius` | `12` | 中圆角常量 |
| `SmallCornerRadius` | `CornerRadius` | `8` | 小圆角常量 |
| `CardBackgroundBrush` | `SolidColorBrush` | `#1AFFFFFF` | 卡片半透明白色背景 |
| `CardBackgroundBrushDark` | `SolidColorBrush` | `#1A000000` | 卡片半透明黑色背景 |
| `AccentBrush` | `SolidColorBrush` | `#0078D7` | 主题强调色（蓝色） |
| `AccentHoverBrush` | `SolidColorBrush` | `#106EBE` | 悬停强调色 |
| `ConsoleBackgroundBrush` | `SolidColorBrush` | `#0D1117` | 控制台背景色（GitHub 风格深色） |
| `ConsoleInfoBrush` | `SolidColorBrush` | `#DCE4EE` | 控制台信息文字色 |
| `ConsoleWarnBrush` | `SolidColorBrush` | `#FFC107` | 控制台警告文字色 |
| `ConsoleErrorBrush` | `SolidColorBrush` | `#FF5252` | 控制台错误文字色 |
| `ConsoleStdinBrush` | `SolidColorBrush` | `#69F0AE` | 控制台输入回显色 |
| `DefaultDropShadowEffect` | `DropShadowEffect` | Blur=24, Depth=4, Opacity=0.15 | 默认阴影效果 |
| `BoolToVis` | `BooleanToVisibilityConverter` | — | 布尔转可见性转换器 |

**全局 TextBlock 样式**: 强制所有 TextBlock 的 Foreground 绑定到 `TextFillColorPrimaryBrush`（跟随主题变化）。

#### App.xaml.cs — 应用程序入口

文件位置: [App.xaml.cs](file:///workspace/ARCbot/App.xaml.cs)

关键职责：
- `Services` 静态属性：全局 DI 容器访问点（Service Locator 模式）
- `ConfigureServices()`: 注册所有服务、ViewModel、Page、Window
- `OnStartup()`: 完整的启动编排流程
- `OnExit()`: 清理所有运行中的 Node.js 进程

---

### 6.2 主窗口 (MainWindow)

#### MainWindow.xaml — 布局结构

文件位置: [MainWindow.xaml](file:///workspace/ARCbot/MainWindow.xaml)

主窗口继承自 `Wpf.Ui.Controls.FluentWindow`，整体布局层次如下：

```
FluentWindow (1200×780, Min 900×600)
├── Grid (主容器)
│   ├── Image "BackgroundImage" (Z=-2)        → 背景图，Stretch=UniformToFill
│   ├── Border "BackgroundOverlay" (Z=-1)      → 左侧 290px 半透明遮罩
│   ├── NavigationView "RootNavigation"        → 主导航控件
│   │   ├── PaneHeader: "ARCbot" 标题          → 字号 32 Bold
│   │   ├── MenuItems (上部导航):
│   │   │   ├── 🏠 首页     → HomePage
│   │   │   ├── 📟 控制台   → ConsolePage
│   │   │   ├── 🧩 插件扩展 → PluginsPage
│   │   │   ├── 🏪 插件市场 → MarketPage
│   │   │   └── ✨ 免费体验 → ExperiencePage
│   │   └── FooterMenuItems (底部导航):
│   │       ├── ❓ 帮助文档 → DocsPage
│   │       └── ⚙️ 设置    → SettingsPage
│   ├── Border "AuthOverlay" (Z=999)          → 微软登录验证码弹窗（默认隐藏）
│   ├── Border "AfdianOverlay" (Z=1000)       → 爱发电支持弹窗（默认隐藏）
│   └── TitleBar (Z=100)                       → 窗口标题栏（透明背景）
```

**FluentWindow 配置**:
- `ExtendsContentIntoTitleBar="True"`: 内容延伸到标题栏区域
- `WindowBackdropType="Mica"`: 默认使用 Mica 材质背景
- `WindowCornerPreference="Round"`: 圆角窗口

**两个全局弹窗的设计**:

1. **微软登录验证弹窗 (AuthOverlay)**:
   - 半透明黑色遮罩 (`#80000000`)
   - 居中白色卡片（圆角 20，阴影效果）
   - 显示: 盾牌图标 + 标题 + 实例名 + 验证码（大号等宽字体）+ 说明文字
   - 按钮: "复制验证码并打开浏览器" (Info) + "关闭" (Secondary)
   - 入场动画: 淡入 (250ms) + 缩放入 (300ms)

2. **爱发电支持弹窗 (AfdianOverlay)**:
   - 半透明黑色遮罩 (`#80000000`)
   - 居中卡片（圆角 20）
   - 显示: 爱心图标 + 标题 + 文案（含 TIPS 说明可永久关闭）
   - 按钮: "爱发电支持" + "GitHub Star" + "但是我拒绝！"
   - 入场动画: 延迟 1s 淡入 (500ms) + 缩放入 (600ms)

#### MainWindow.xaml.cs — 核心交互逻辑

文件位置: [MainWindow.xaml.cs](file:///workspace/ARCbot/MainWindow.xaml.cs)

**构造函数流程**:
1. 从 DI 获取 `AuthInterceptor` 单例
2. 订阅 `AuthCodeDetected` 事件 → `OnAuthCodeDetected`
3. 创建 `PageService` 并设置到 `RootNavigation`
4. 订阅 `Loaded` 事件: 导航到首页 + 显示爱发电弹窗

**关键方法**:

| 方法 | 功能 |
|------|------|
| `UpdateBackground(string)` | 切换背景图：空字符串→恢复 Mica；有效路径→禁用 Mica，显示图片 |
| `UpdateBackgroundOverlayOpacity(double)` | 更新左侧遮罩透明度 |
| `NavigateToConsole(string)` | 导航到控制台页并聚焦指定实例 |
| `ShowAfdianPopupIfNeeded()` | 检查设置后显示爱发电弹窗（延迟 1s + 缩放动画） |
| `OnAuthCodeDetected(...)` | 收到拦截事件后在 UI 线程显示验证码弹窗（自动置顶、激活窗口） |
| `AuthCopyAndOpen_Click(...)` | 复制验证码到剪贴板 + 用默认浏览器打开微软登录 URL |
| `OpenUrl(string)` | 通过 `Process.Start` + `UseShellExecute` 打开 URL |

---

### 6.3 页面服务 (PageService)

文件位置: [MainWindow.xaml.cs#L243-L262](file:///workspace/ARCbot/MainWindow.xaml.cs#L243-L262)

实现了 `Wpf.Ui.IPageService` 接口，为 `NavigationView` 提供页面实例解析：

```csharp
public class PageService : Wpf.Ui.IPageService
{
    public FrameworkElement? GetPage(Type pageType)
    {
        // 1. 优先从 DI 容器解析（支持构造函数注入）
        return App.Services.GetService(pageType) as FrameworkElement
               // 2. 回退到 Activator.CreateInstance
               ?? Activator.CreateInstance(pageType) as FrameworkElement;
    }
}
```

设计意图：优先使用 DI 容器解析，使 Page 可以通过构造函数注入服务；若 DI 未注册，则回退到反射创建。

---

### 6.4 数据模型层 (Models)

#### 6.4.1 AppSettings — 应用设置模型

文件位置: [AppSettings.cs](file:///workspace/ARCbot/Models/AppSettings.cs)

```csharp
public class AppSettings
{
    bool IsDarkMode = true;              // 暗色模式（默认开启）
    double BgOpacity = 0.6;             // 背景遮罩不透明度
    string BackgroundImagePath          // 背景图路径（默认 bg2.png）
        = "pack://application:,,,/Assets/bg2.png";
    bool UseCustomBaseAgent = false;    // 是否使用自定义基础包
    string CustomBaseAgentPath = "";    // 自定义基础包路径
    bool DisableAfdianPopup = false;    // 禁用爱发电弹窗
    string SkippedVersion = "";         // 跳过的更新版本号
}
```

持久化方式: JSON 序列化到 `%APPDATA%/ARCbot/settings.json`

#### 6.4.2 BotInstance — 机器人实例模型

文件位置: [BotInstance.cs](file:///workspace/ARCbot/Models/BotInstance.cs)

核心模型，继承 `ObservableObject`，所有属性使用 `[ObservableProperty]` 源代码生成器实现双向绑定：

**状态枚举**:
```csharp
public enum BotStatus { Stopped, Running, Starting, Error }
```

**实例字段分类**:

| 分类 | 字段 | 类型 | 默认值 | 对应 .env 键 |
|------|------|------|--------|-------------|
| **标识** | `InstanceName` | `string` | `""` | — |
| **状态** | `Status` | `BotStatus` | `Stopped` | — |
| **Minecraft 服务器** | `McHost` | `string` | `"myarc.fun"` | `MC_HOST` |
| | `McPort` | `string` | `"25565"` | `MC_PORT` |
| | `McVersion` | `string` | `"1.20.1"` | `MC_VERSION` |
| | `McUsername` | `string` | `""` | `MC_USERNAME` |
| | `McAuthType` | `string` | `"microsoft"` | `MC_AUTH_TYPE` |
| | `McLoginPassword` | `string` | `""` | `MC_LOGIN_PASSWORD` |
| | `McOwnerName` | `string` | `"_FENTAI_"` | `MC_OWNER_NAME` |
| **LLM AI 配置** | `LlmApiKey` | `string` | `""` | `LLM_API_KEY` |
| | `LlmApiUrl` | `string` | `"https://ark.cn-beijing.volces.com/api/v3"` | `LLM_API_URL` |
| | `LlmModel` | `string` | `"doubao-seed-2-0-lite-260215"` | `LLM_MODEL` |
| | `AiStylePrompt` | `string` | `"你说话要像一个活泼可爱的猫娘..."` | `AI_STYLE_PROMPT` |
| **行为设置** | `TellMode` | `string` | `"whisper"` | `TELL_MODE` |
| | `AutoDefendEnabled` | `bool` | `false` | `AUTO_DEFEND_ENABLED` |
| | `InstinctAutoTpLogin` | `bool` | `false` | `INSTINCT_AUTO_TP_LOGIN` |
| | `InstinctAutoEat` | `bool` | `false` | `INSTINCT_AUTO_EAT` |
| | `InstinctAutoTool` | `bool` | `false` | `INSTINCT_AUTO_TOOL` |
| | `InstinctAutoDump` | `bool` | `false` | `INSTINCT_AUTO_DUMP` |
| | `DebugMode` | `bool` | `false` | `DEBUG_MODE` |

**Clone() 方法**: 创建实例的深拷贝副本，用于编辑模式时不影响原始数据。

> **注意**: 默认 LLM 配置指向 **火山引擎豆包模型** (Doubao Seed 2.0 Lite)，API 地址为北京 Ark 平台。

#### 6.4.3 ConsoleEntry — 控制台日志条目

文件位置: [ConsoleEntry.cs](file:///workspace/ARCbot/Models/ConsoleEntry.cs)

```csharp
public enum LogLevel { Info, Warn, Error, Stdin }

public class ConsoleEntry
{
    DateTime Timestamp = DateTime.Now;    // 时间戳
    LogLevel Level = Info;                // 日志级别
    string Text = "";                     // 日志文本
    string InstanceName = "";             // 来源实例名
    string ColorHex => ...                // 根据级别返回 Hex 颜色
    string Display => $"[{HH:mm:ss}] {Text}"; // 格式化显示
}
```

颜色映射: Info→`#DCE4EE`, Warn→`#FFC107`, Error→`#FF5252`, Stdin→`#69F0AE`

#### 6.4.4 UpdateInfo — 更新信息

文件位置: [UpdateInfo.cs](file:///workspace/ARCbot/Models/UpdateInfo.cs)

```csharp
public class UpdateInfo
{
    [JsonPropertyName("version")]      string Version;
    [JsonPropertyName("downloadUrl")]  string DownloadUrl;
    [JsonPropertyName("releaseNotes")] string ReleaseNotes;
}
```

从更新服务器 JSON 反序列化。

---

### 6.5 服务层 (Services)

#### 6.5.1 AuthInterceptor — 微软登录拦截器

文件位置: [AuthInterceptor.cs](file:///workspace/ARCbot/Services/AuthInterceptor.cs)

**设计模式**: 观察者模式（事件驱动）

**核心职责**: 持续监听所有运行实例的 stdout/stderr，当检测到微软设备码登录提示时，通过事件通知 UI 层弹出验证码窗口。

**事件定义**:
```csharp
public class AuthCodeEventArgs : EventArgs
{
    string InstanceName;   // 来源实例
    string Url;            // 微软登录 URL
    string Code;           // 验证码
    DateTime DetectedAt;   // 检测时间
}

event EventHandler<AuthCodeEventArgs>? AuthCodeDetected;
```

**正则表达式匹配**:

| 正则 | 匹配目标 | 说明 |
|------|----------|------|
| `MsaCodeRegex` | `microsoft.com` URL + `code XXXXXXXX` | 主正则，匹配标准微软设备码格式 |
| `MsaCodeFallbackRegex` | 含 `microsoft` 的 URL + 8 位大写/数字 | 备用正则，兼容其他格式 |

**AnalyzeLine() 方法**: 由 `NodeProcessManager` 在每次收到 stdout/stderr 行时调用，先尝试主正则，再尝试备用正则，匹配成功则触发事件。

> 这是一个 **横切关注点 (Cross-Cutting Concern)** 设计：单个拦截器服务注入到所有 NodeProcessManager，实现全局统一的登录验证码检测。

#### 6.5.2 DownloadService — 文件下载服务

文件位置: [DownloadService.cs](file:///workspace/ARCbot/Services/DownloadService.cs)

**核心职责**:
1. 下载 Node.js 便携版 (v20.11.1 Windows x64) 并解压到 `runtime/`
2. 下载云端基础包 (Minecraft AI Agent zip) 到 `downloads/`
3. 将基础包解压到指定实例目录

**下载源**:
| 资源 | URL |
|------|-----|
| Node.js | `https://nodejs.org/dist/v20.11.1/node-v20.11.1-win-x64.zip` |
| 基础包 | WebGetStore CDN 链接（含签名参数） |

**关键方法**:

| 方法 | 功能 |
|------|------|
| `IsNodeInstalled()` | 检查 `runtime/node.exe` 是否存在 |
| `IsBaseAgentDownloaded()` | 检查 `downloads/base_agent.zip` 是否存在 |
| `DownloadFileAsync(...)` | 通用下载方法，支持进度回调 `IProgress<(long, long)>`、CancellationToken |
| `DownloadNodeRuntimeAsync(...)` | 下载 → 解压 → 查找 node.exe → 复制到 runtime/ → 清理临时文件 |
| `DownloadBaseAgentAsync(...)` | 下载基础包 zip（始终重新下载最新版本） |
| `ExtractBaseAgentToInstance(name, useCustom)` | 解压基础包到实例目录，支持自定义包路径 |

**下载实现细节**:
- 使用 `HttpClient`，超时 10 分钟，启用自动重定向
- 缓冲区大小 81920 字节 (80KB)
- 流式写入 `FileStream` (8KB buffer, async)
- Node.js 解压：先解压到临时目录 → 递归搜索 `node.exe` → 复制整个 node 目录到 `runtime/` → 删除临时文件

#### 6.5.3 EnvManager — 环境变量管理器

文件位置: [EnvManager.cs](file:///workspace/ARCbot/Services/EnvManager.cs)

**核心职责**: `BotInstance` ↔ `.env` 文件的双向转换

**WriteEnv(BotInstance)** → `.env` 文件:
```
# ═══════════════════════════════════════
# ARCbot Instance Config: <实例名>
# Generated at: yyyy-MM-dd HH:mm:ss
# ═══════════════════════════════════════

# === Minecraft Server ===
MC_HOST=myarc.fun
MC_PORT=25565
...

# === AI Configuration ===
LLM_API_KEY=...
LLM_API_URL=...
...

# === Behavior ===
AUTO_DEFEND_ENABLED=true/false
...
```

**ReadEnv(instanceName)** → `BotInstance`:
- 解析 `KEY=VALUE` 格式，忽略注释行（`#` 开头）和空行
- 支持引号包裹的值（`"..."` 或 `'...'`）
- 键名大小写不敏感 (`OrdinalIgnoreCase`)
- **向后兼容**: `DEEPSEEK_API_KEY` → `LLM_API_KEY`（旧版本 DeepSeek API Key 自动映射）
- 布尔值解析: `true`/`1`/`yes`（不区分大小写）→ true

#### 6.5.4 InstanceManager — 实例管理器

文件位置: [InstanceManager.cs](file:///workspace/ARCbot/Services/InstanceManager.cs)

**核心职责**: 实例的 CRUD 操作与运行状态管理

**数据结构**:
```csharp
ObservableCollection<BotInstance> Instances;          // UI 绑定的实例列表
Dictionary<string, NodeProcessManager> RunningProcesses; // 运行中进程字典
event Action<string, NodeProcessManager>? ProcessStarted; // 进程启动事件
```

**关键方法**:

| 方法 | 功能 |
|------|------|
| `LoadAllInstances()` | 扫描 `Instances/` 目录，读取 `.env` 重建实例列表 |
| `CreateInstance(BotInstance)` | 检查目录不存在 → 解压基础包 → 写 `.env` → 加入列表 |
| `UpdateInstance(BotInstance)` | 重写 `.env` → 替换列表中对应项 |
| `DeleteInstanceAsync(string)` | 停止进程 → 删除整个实例目录 → 从列表移除 |
| `StartInstance(string)` | 检查未运行 → 创建 NodeProcessManager → 订阅 Exited 事件 → 启动 → 加入 RunningProcesses |
| `StopInstanceAsync(string)` | 调用 NodeProcessManager.StopAsync() → Dispose → 从 RunningProcesses 移除 |

**设计细节**:
- `StartInstance` 中订阅 `ProcessExited` 事件，自动清理 `RunningProcesses` 并更新实例状态
- `LoadAllInstances` 对于有目录但无 `.env` 的实例创建默认实例对象
- 所有 CRUD 操作直接操作磁盘文件

#### 6.5.5 NodeProcessManager — Node.js 进程管理器

文件位置: [NodeProcessManager.cs](file:///workspace/ARCbot/Services/NodeProcessManager.cs)

**核心职责**: 管理单个 Node.js 进程的完整生命周期

**进程配置**:
```csharp
ProcessStartInfo {
    FileName = "runtime/node.exe",
    Arguments = "index.js",
    WorkingDirectory = "Instances/<name>/src/",
    UseShellExecute = false,       // 不使用系统 Shell
    CreateNoWindow = true,         // 不创建控制台窗口
    RedirectStandardOutput = true, // 重定向 stdout
    RedirectStandardError = true,  // 重定向 stderr
    RedirectStandardInput = true,  // 重定向 stdin
    StandardOutputEncoding = UTF8, // 强制 UTF-8 编码
    StandardErrorEncoding = UTF8,
}
```

**环境变量设置**:
| 变量 | 值 | 用途 |
|------|-----|------|
| `NODE_OPTIONS` | `--max-old-space-size=512` | 限制 Node.js 堆内存 512MB |
| `LANG` | `zh_CN.UTF-8` | 系统语言环境 |
| `CHCP` | `65001` | Windows 控制台代码页 (UTF-8) |
| `PYTHONIOENCODING` | `utf-8` | Python IO 编码（如果机器人用到 Python） |

**关键方法**:

| 方法 | 功能 |
|------|------|
| `Start()` | 检查运行时 → 异步读取 stdout/stderr → 日志分类 → 触发 OutputReceived + AuthInterceptor |
| `WriteInputAsync(string)` | 向进程 stdin 写入一行 + 回显到日志 |
| `StopAsync(timeoutMs=5000)` | 优雅关闭 stdin → 等待 5s → 超时则 Kill 整个进程树 |
| `Dispose()` | 强制 Kill → Dispose Process → SuppressFinalize |

**日志级别自动分类 (ClassifyLogLevel)**:
- 含 `warn`/`WARNING` → `LogLevel.Warn`
- 含 `error`/`ERR!`/`FATAL` → `LogLevel.Error`
- 其他 → `LogLevel.Info`

**stdin 输入流程**:
1. `ConsoleViewModel.SendInputCommand` → 获取输入文本
2. → `NodeProcessManager.WriteInputAsync(text)`
3. → `process.StandardInput.WriteLineAsync(text)` → FlushAsync
4. → 同时触发 `OutputReceived`（回显绿色 `> text`）

#### 6.5.6 SettingsService — 设置持久化服务

文件位置: [SettingsService.cs](file:///workspace/ARCbot/Services/SettingsService.cs)

```csharp
public class SettingsService
{
    AppSettings Settings;                          // 当前设置
    AppSettings LoadSettings();                    // 从 JSON 文件加载
    void SaveSettings();                           // 保存到 JSON 文件
    void UpdateSettings(Action<AppSettings>);      // 原子更新 + 保存
}
```

**容错设计**: JSON 反序列化失败时返回默认 `AppSettings`，保存失败时静默忽略。

#### 6.5.7 UpdateService — 自动更新服务

文件位置: [UpdateService.cs](file:///workspace/ARCbot/Services/UpdateService.cs)

**更新检查流程**:
1. HTTP GET `http://localhost:3000/update` (5s 超时)
2. 反序列化 `UpdateInfo`
3. 比较 `remoteVersion > localVersion`
4. 检查是否跳过该版本 (`SkippedVersion`)
5. UI 线程弹出 `UpdateDialog`
6. 用户选择:
   - **立即更新** → 下载 exe → 批处理替换当前 exe → 重启
   - **暂不更新** → 若勾选"跳过"则保存版本号到设置
   - **关闭** → 同上

**批处理安装脚本** (`ARCbot_Updater.bat`):
```batch
@echo off
chcp 65001 >nul
echo 正在安装更新...
timeout /t 2 /nobreak >nul
:retry
del /f /q "当前exe路径" 2>nul
if exist "当前exe路径" (timeout /t 1 & goto retry)
move /y "下载的exe" "当前exe路径"
start "" "当前exe路径"
del /f /q "%~f0" & exit
```

设计要点：
- 先尝试删除旧 exe（带重试循环，防止文件被占用）
- 移动新 exe 到原位置
- 启动新 exe
- 自删除批处理文件

---

### 6.6 视图模型层 (ViewModels)

所有 ViewModel 继承 `ObservableObject`，使用 CommunityToolkit.Mvvm 源代码生成器。

#### 6.6.1 MainWindowViewModel

文件位置: [MainWindowViewModel.cs](file:///workspace/ARCbot/ViewModels/MainWindowViewModel.cs)

简单的容器 VM：`ApplicationTitle`、`IsInitialized`、`CurrentPage`（当前未大量使用，为扩展预留）。

#### 6.6.2 HomeViewModel — 首页视图模型

文件位置: [HomeViewModel.cs](file:///workspace/ARCbot/ViewModels/HomeViewModel.cs)

**依赖**: `InstanceManager`

**数据绑定**:
- `Instances` → 直接绑定 `InstanceManager.Instances`
- `IsLoading` → 加载状态
- `StatusMessage` → 状态消息

**事件 (View 订阅)**:
- `RequestCreateDialog` → 打开创建对话框
- `RequestEditDialog(BotInstance)` → 打开编辑对话框
- `RequestNavigateToConsole(string)` → 导航到控制台

**RelayCommand 命令**:

| 命令 | 类型 | 功能 |
|------|------|------|
| `LoadInstancesCommand` | `void` | 调用 `InstanceManager.LoadAllInstances()` |
| `StartInstanceCommand` | `void(BotInstance)` | 启动实例 → 导航到控制台 |
| `StopInstanceCommand` | `Task(BotInstance)` | 异步停止实例 |
| `DeleteInstanceCommand` | `Task(BotInstance)` | 确认删除 → 异步删除 |
| `OpenCreateDialogCommand` | `void` | 触发 `RequestCreateDialog` 事件 |
| `EditInstanceCommand` | `void(BotInstance)` | 触发 `RequestEditDialog` 事件 |

**启动流程**: `StartInstance` → `InstanceManager.StartInstance()` → 更新 Status → 触发 `RequestNavigateToConsole`

#### 6.6.3 ConsoleViewModel — 控制台视图模型

文件位置: [ConsoleViewModel.cs](file:///workspace/ARCbot/ViewModels/ConsoleViewModel.cs)

**依赖**: `InstanceManager` + `Dispatcher`（UI 线程调度）

**核心数据**:
```csharp
ObservableCollection<string> RunningInstances;         // 运行实例名列表
string? SelectedInstance;                              // 当前选中实例
ObservableCollection<ConsoleEntry> ConsoleEntries;     // 当前显示的日志
Dictionary<string, ObservableCollection<ConsoleEntry>> _logCache;  // 所有实例日志缓存
HashSet<string> _subscribedInstances;                  // 已订阅事件防止重复
```

**核心机制 — 日志缓存与订阅**:

1. **进程启动订阅**: 构造函数中订阅 `InstanceManager.ProcessStarted`，自动将新实例加入列表并订阅输出
2. **实例切换**: `OnSelectedInstanceChanged` → 切换 `ConsoleEntries` 到对应缓存
3. **日志收集**: `SubscribeToProcess` → 订阅 `pm.OutputReceived` → Dispatcher.Invoke 写入缓存
4. **进程退出处理**: 订阅 `pm.ProcessExited` → 写入退出日志 → 取消订阅 → 刷新列表

**RelayCommand 命令**:

| 命令 | 功能 |
|------|------|
| `RefreshRunningInstancesCommand` | 刷新运行实例列表 |
| `SendInputCommand` | 发送 stdin 输入 |
| `ForceStopCommand` | 确认后强制停止 |
| `ClearConsoleCommand` | 清空当前日志缓存 |

**FocusInstance(string)**: 由 `MainWindow.NavigateToConsole` 调用，刷新列表并自动选中指定实例。

#### 6.6.4 CreateInstanceViewModel — 创建/编辑实例 VM

文件位置: [CreateInstanceViewModel.cs](file:///workspace/ARCbot/ViewModels/CreateInstanceViewModel.cs)

**两种模式**:
- **新建模式**: `IsEditMode = false`, 所有字段使用默认值
- **编辑模式**: `IsEditMode = true`, 从 `BotInstance` 加载数据

**表单字段**: 完整映射 `BotInstance` 的所有配置属性

**下拉选项**:
- `AuthTypes`: `["microsoft", "offline"]`
- `TellModes`: `["whisper", "public"]`

**表单验证 (SaveCommand)**:
1. 实例名不能为空
2. 实例名不能包含非法文件名字符 (`Path.GetInvalidFileNameChars()`)
3. Minecraft 用户名不能为空
4. 通过验证 → 构建 `BotInstance` → 调用 `CreateInstance` 或 `UpdateInstance`
5. 触发 `SaveCompleted` 事件（View 订阅来关闭对话框）

**ResetForCreate()**: 重置所有字段为默认值

#### 6.6.5 PluginsViewModel — 插件管理 VM

文件位置: [PluginsViewModel.cs](file:///workspace/ARCbot/ViewModels/PluginsViewModel.cs)

**功能**:
- 列出指定实例 `plugins/` 目录下的 `.js` 文件
- 支持多选导入插件文件（`OpenFileDialog`）
- 支持删除插件文件
- 显示文件大小、最后修改时间
- "怎么开发技能？"按钮打开 `https://docs.myarc.icu/dev/skills`

**PluginFileInfo 模型**: `FileName`, `FullPath`, `SizeKB`, `LastModified`, `DisplaySize`

#### 6.6.6 SettingsViewModel — 设置页 VM

文件位置: [SettingsViewModel.cs](file:///workspace/ARCbot/ViewModels/SettingsViewModel.cs)

**依赖**: `DownloadService` + `SettingsService`

**设置分组**:

| 分组 | 属性 | 类型 | 联动效果 |
|------|------|------|----------|
| **个性化** | `IsDarkMode` | `bool` | 即时切换 WPF-UI 主题 + 持久化 |
| | `BgOpacity` | `double` | 即时更新 `GlobalOverlayOpacity` 资源 + 持久化 |
| | `BackgroundImagePath` | `string` | 即时更新 MainWindow 背景 + 持久化 |
| | `PresetImages` | `ObservableCollection<string>` | 6 张预设 + 空（无背景） |
| **Node.js 运行时** | `IsNodeInstalled` | `bool` | 检测 `node.exe` 是否存在 |
| | `IsDownloading` | `bool` | 控制进度条显示 |
| | `DownloadProgress` | `double` | 0-100 百分比 |
| | `DownloadStatusText` | `string` | 下载状态描述 |
| **基础包** | `IsBaseAgentDownloaded` | `bool` | 检测基础包 zip 是否存在 |
| | `IsDownloadingBaseAgent` | `bool` | 控制进度条显示 |
| | `BaseAgentProgress` | `double` | 0-100 百分比 |
| | `BaseAgentStatusText` | `string` | 下载状态描述 |
| **自定义基础包** | `UseCustomBaseAgent` | `bool` | 切换是否使用自定义包 |
| | `CustomBaseAgentPath` | `string` | 自定义包路径 |
| | `IsCustomBaseAgentImported` | `bool` | 检测自定义包是否存在 |
| **弹窗设置** | `DisableAfdianPopup` | `bool` | 禁用爱发电弹窗 |
| **关于** | `AppVersion` | `string` | `"1.0.3"` |
| | `DotNetVersion` | `string` | `RuntimeInformation.FrameworkDescription` |
| | `AppDataPath` | `string` | `PathHelper.RootDir` |

**关键联动**:
- `OnIsDarkModeChanged` → `ApplicationThemeManager.Apply()` + 保存设置
- `OnBgOpacityChanged` → `Application.Current.Resources["GlobalOverlayOpacity"]` + 保存设置
- `OnBackgroundImagePathChanged` → `MainWindow.UpdateBackground()` + 保存设置

**下载命令**:
- `DownloadNodeCommand` → `DownloadService.DownloadNodeRuntimeAsync(progress)`
- `DownloadBaseAgentCommand` → `DownloadService.DownloadBaseAgentAsync(progress)`
- 两者都使用 `Progress<(long, long)>` 报告进度到 UI

---

### 6.7 视图层 (Views)

#### 6.7.1 页面总览

| 页面 | XAML 文件 | Code-Behind | 核心功能 |
|------|-----------|-------------|----------|
| HomePage | [HomePage.xaml](file:///workspace/ARCbot/Views/Pages/HomePage.xaml) | [HomePage.xaml.cs](file:///workspace/ARCbot/Views/Pages/HomePage.xaml.cs) | 实例卡片网格、状态指示器、空状态提示、FAB 创建按钮 |
| ConsolePage | [ConsolePage.xaml](file:///workspace/ARCbot/Views/Pages/ConsolePage.xaml) | [ConsolePage.xaml.cs](file:///workspace/ARCbot/Views/Pages/ConsolePage.xaml.cs) | 左侧实例列表、右侧终端输出、底部命令输入、自动滚动 |
| PluginsPage | [PluginsPage.xaml](file:///workspace/ARCbot/Views/Pages/PluginsPage.xaml) | [PluginsPage.xaml.cs](file:///workspace/ARCbot/Views/Pages/PluginsPage.xaml.cs) | 实例选择下拉框、插件列表、导入/删除按钮 |
| SettingsPage | [SettingsPage.xaml](file:///workspace/ARCbot/Views/Pages/SettingsPage.xaml) | [SettingsPage.xaml.cs](file:///workspace/ARCbot/Views/Pages/SettingsPage.xaml.cs) | 运行环境、个性化、关于分组设置 |
| MarketPage | [MarketPage.xaml](file:///workspace/ARCbot/Views/Pages/MarketPage.xaml) | [MarketPage.xaml.cs](file:///workspace/ARCbot/Views/Pages/MarketPage.xaml.cs) | 内嵌 WebView2 加载插件市场 |
| ExperiencePage | [ExperiencePage.xaml](file:///workspace/ARCbot/Views/Pages/ExperiencePage.xaml) | [ExperiencePage.xaml.cs](file:///workspace/ARCbot/Views/Pages/ExperiencePage.xaml.cs) | 内嵌 WebView2 加载免费体验页 |
| DocsPage | [DocsPage.xaml](file:///workspace/ARCbot/Views/Pages/DocsPage.xaml) | [DocsPage.xaml.cs](file:///workspace/ARCbot/Views/Pages/DocsPage.xaml.cs) | 内嵌 WebView2 加载帮助文档 |

#### 6.7.2 HomePage 详细设计

**布局结构**:
```
Grid (Margin 40,48,40,24)
├── Row 0: 标题区
│   └── Border(UnifiedOverlayBorderStyle) + "我的机器人" 标题 + 状态标签
├── Row 1: ScrollViewer
│   └── ItemsControl (WrapPanel 布局)
│       └── 每个 BotInstance → 320px 宽卡片
│           ├── 状态指示灯 (绿/红/黄/灰) + 状态文字
│           ├── 实例名 (FontSize 18 SemiBold)
│           ├── 🌐 服务器地址:端口
│           ├── 👤 用户名 | 认证方式
│           └── 按钮组: ▶ 启动 | ✏️ 编辑 | 🗑 删除
├── EmptyStatePanel (无实例时显示)
│   └── 🤖 图标 + 提示文字
└── FAB 按钮 (+)
    └── 56×56 圆形 Info 按钮 (右下角, Z=10)
```

**状态指示灯颜色映射**:
- `Running` → `#4CAF50` (绿色)
- `Error` → `#FF5252` (红色)
- `Starting` → `#FFC107` (黄色)
- `Stopped` → `#9E9E9E` (灰色)

**交互流程**:
1. `Page_Loaded` → `HomeViewModel.LoadInstancesCommand` → 扫描目录加载实例
2. 点击卡片按钮 → 通过 `Tag="{Binding}"` 传递 BotInstance → ViewModel 命令
3. FAB 按钮 → `ShowCreateDialog(null)` → 新建模式
4. 编辑按钮 → `ShowCreateDialog(instance)` → 编辑模式

#### 6.7.3 ConsolePage 详细设计

**三栏布局**:
```
Grid (Margin 32,32,32,24)
├── Col 0 (220px): 运行实例列表
│   └── ListBox (PclListItemStyle)
│       └── 每个实例: 🟢 绿点 + 实例名
├── Col 1 (24px): 间距
└── Col 2 (*): 控制台终端
    └── Grid (三行)
        ├── Row 0: 工具栏 (#252526 背景)
        │   ├── Left: 终端图标 + 实例名
        │   └── Right: 清屏按钮 + 强制停止按钮
        ├── Row 1: 日志输出区
        │   └── ListBox (VirtualizingStackPanel, Cascadia Code 字体)
        │       └── 每条日志: TextBlock (按 LogLevel 着色)
        └── Row 2: 输入区 (#252526 背景)
            └── Grid
                ├── Col 0: TextBox (暗色背景, Cascadia Code)
                └── Col 1: "发送" 按钮 (Send24 图标)
```

**智能自动滚动**:
- 订阅 `ConsoleEntries.CollectionChanged`
- 检查 `ScrollViewer.VerticalOffset >= ScrollableHeight - 50`（用户是否在底部）
- 在底部时自动 `ScrollIntoView(最后一项)`
- 用户手动向上滚动时停止自动滚动

**递归获取 ScrollViewer**:
```csharp
ScrollViewer? GetScrollViewer(DependencyObject element)
{
    // 递归遍历 VisualTree 查找 ScrollViewer
}
```

#### 6.7.4 WebPage — WebView2 封装

文件位置: [WebPage.xaml](file:///workspace/ARCbot/Views/Pages/WebPage.xaml) / [WebPage.xaml.cs](file:///workspace/ARCbot/Views/Pages/WebPage.xaml.cs)

**功能**:
- 封装 `Microsoft.Web.WebView2.Wpf.WebView2` 控件
- 自动创建 WebView2 环境（用户数据目录: `%TEMP%/ARCbot_WebView2`）
- 禁用密码自动保存和通用自动填充
- 导航时显示顶部加载进度条

**SetUrl(string)**: 供父页面（MarketPage/ExperiencePage/DocsPage）调用设置 URL

**加载的远程页面**:
| 页面 | URL |
|------|-----|
| 插件市场 | `https://bot.myarc.icu/market` |
| 免费体验 | `https://bot.myarc.icu/experience` |
| 帮助文档 | `https://bot.myarc.icu/docs` |

#### 6.7.5 对话框

##### CreateInstanceDialog

文件位置: [CreateInstanceDialog.xaml](file:///workspace/ARCbot/Views/Dialogs/CreateInstanceDialog.xaml) / [.cs](file:///workspace/ARCbot/Views/Dialogs/CreateInstanceDialog.xaml.cs)

**窗口配置**: 560×760, 居中, 无边框+透明背景+自绘圆角阴影卡片

**表单区域**:
1. **实例名称** (TextBox)
2. **服务器配置** (3 列网格: 地址 + 端口 + 版本)
3. **用户名** (TextBox)
4. **认证方式** (ComboBox: microsoft/offline)
5. **登录密码** (PasswordBox, 手动同步到 VM)
6. **主人名称** (TextBox)
7. **AI 配置** (API Key + API URL + 模型名)
8. **对话模式** (ComboBox: whisper/public)
9. **AI 风格提示词** (多行 TextBox, AcceptsReturn)
10. **行为开关** (6 个 ToggleSwitch: 自动防御/登录传送/进食/工具/丢弃/调试)

**模式切换逻辑**:
- 编辑模式: 标题改为"编辑实例"、禁用实例名输入、按钮文字改为"保存更改"、密码框预填充
- 新建模式: 标题"创建新实例"、实例名可编辑、按钮"保存并创建"

**PasswordBox 同步**: WPF PasswordBox 不支持数据绑定，通过 `PasswordChanged` 事件手动同步到 `ViewModel.McLoginPassword`

##### UpdateDialog

文件位置: [UpdateDialog.xaml](file:///workspace/ARCbot/Views/Dialogs/UpdateDialog.xaml) / [.cs](file:///workspace/ARCbot/Views/Dialogs/UpdateDialog.xaml.cs)

**窗口配置**: 450×400, 居中, 置顶 (Topmost), 不可调整大小

**内容**:
- 标题: "发现新版本：v{version}"
- 更新日志 (只读 TextBox, 可滚动)
- 下载进度区 (ProgressBar + 状态文字, 默认隐藏)
- "跳过此版本"复选框
- 按钮: "暂不更新" + "立即更新"

**下载与安装流程**:
1. 禁用按钮 → 显示进度条 → `DownloadUpdateAsync()`
2. 流式下载到 `%TEMP%/ARCbot_Update_{version}.exe`
3. 实时更新进度条百分比和字节数
4. 下载完成 → `InstallUpdate()`
5. 生成批处理脚本 `ARCbot_Updater.bat`
6. 启动批处理 → `Application.Current.Shutdown()`

**FormatBytes**: 辅助方法，将字节数格式化为 `B/KB/MB/GB`。

---

### 6.8 工具辅助层 (Helpers)

#### 6.8.1 PathHelper — 全局路径常量

文件位置: [PathHelper.cs](file:///workspace/ARCbot/Helpers/PathHelper.cs)

集中管理 `%APPDATA%/ARCbot/` 下的所有路径，确保全项目使用一致的目录结构：

| 属性 | 路径 | 说明 |
|------|------|------|
| `RootDir` | `%APPDATA%/ARCbot/` | 应用根目录 |
| `RuntimeDir` | `RootDir/runtime/` | Node.js 运行时目录 |
| `NodeExePath` | `RuntimeDir/node.exe` | Node.js 可执行文件 |
| `InstancesDir` | `RootDir/Instances/` | 实例根目录 |
| `DownloadsDir` | `RootDir/downloads/` | 下载缓存目录 |
| `BaseAgentZipPath` | `DownloadsDir/base_agent.zip` | 云端基础包 |
| `CustomBaseAgentZipPath` | `DownloadsDir/custom_base_agent.zip` | 自定义基础包 |
| `AppSettingsPath` | `RootDir/settings.json` | 设置文件 |

**实例路径方法**:

| 方法 | 返回 |
|------|------|
| `GetInstanceDir(name)` | `InstancesDir/{name}/` |
| `GetInstanceSrcDir(name)` | `InstancesDir/{name}/src/` |
| `GetInstanceEnvPath(name)` | `InstancesDir/{name}/.env` |
| `GetInstancePluginsDir(name)` | `InstancesDir/{name}/plugins/` |

`EnsureDirectories()`: 确保所有必要目录存在。

#### 6.8.2 Converters — 值转换器

文件位置: [Converters.cs](file:///workspace/ARCbot/Helpers/Converters.cs)

两个备用转换器（当前未在 XAML 中引用）:

| 转换器 | 输入 | 输出 | 用途 |
|--------|------|------|------|
| `InverseBoolConverter` | `bool` | `!bool` | 布尔取反 |
| `InverseBoolToVisibilityConverter` | `bool` | `true→Collapsed, false→Visible` | 布尔反向转可见性 |

---

### 6.9 静态资源 (Assets)

| 文件 | 用途 | 构建操作 |
|------|------|----------|
| `icon.ico` | 应用程序图标 | ApplicationIcon |
| `bg1.png` ~ `bg6.png` | 6 张预设背景图 | Resource (嵌入程序集) |

背景图通过 `pack://application:,,,/Assets/bgX.png` URI 引用。

---

## 7. 数据流与交互

### 7.1 创建实例流程

```
用户点击 FAB "+" 按钮
  → HomePage.CreateInstance_Click
    → ShowCreateDialog(null)
      → new CreateInstanceDialog()
      → DI 获取 CreateInstanceViewModel
      → vm.ResetForCreate() (设置默认值)
      → dialog.DataContext = vm
      → dialog.ShowDialog()

用户填写表单 → 点击"保存并创建"
  → CreateInstanceViewModel.SaveCommand
    → 表单验证 (实例名/用户名非空、无非法字符)
    → 构建 BotInstance
    → InstanceManager.CreateInstance(instance)
      → DownloadService.ExtractBaseAgentToInstance(name)
        → ZipFile.ExtractToDirectory(zip, instanceDir)
        → Directory.CreateDirectory(pluginsDir)
      → EnvManager.WriteEnv(instance)
        → 生成 .env 文件
      → Instances.Add(instance)
    → vm.SaveCompleted?.Invoke()
      → dialog.Close()
      → HomeViewModel.LoadInstancesCommand (刷新列表)
```

### 7.2 启动实例 → 控制台监控流程

```
HomePage 点击 "▶ 启动"
  → HomeViewModel.StartInstanceCommand(instance)
    → InstanceManager.StartInstance(instanceName)
      → new NodeProcessManager(name, authInterceptor)
      → 订阅 ProcessExited (自动清理状态)
      → pm.Start()
        → 检查 node.exe 存在
        → 检查 index.js 存在
        → 配置 ProcessStartInfo (UTF-8, 流重定向, 环境变量)
        → 复制 .env 到 src/ 目录
        → Process.Start()
        → BeginOutputReadLine / BeginErrorReadLine
        → OutputDataReceived:
          → ClassifyLogLevel → ConsoleEntry
          → OutputReceived?.Invoke (通知 ConsoleViewModel)
          → authInterceptor.AnalyzeLine (检查微软登录码)
        → ErrorDataReceived:
          → ConsoleEntry(Error)
          → OutputReceived + authInterceptor
      → RunningProcesses[name] = pm
      → ProcessStarted?.Invoke (通知 ConsoleViewModel)
      → 更新实例 Status = Running
    → 触发 RequestNavigateToConsole
      → MainWindow.NavigateToConsole(name)
        → RootNavigation.Navigate(ConsolePage)
        → ConsoleViewModel.FocusInstance(name)
          → RefreshRunningInstances
          → SelectedInstance = name
          → SubscribeToProcess(name)
            → 订阅 pm.OutputReceived
              → Dispatcher.Invoke → _logCache.Add(entry)
```

### 7.3 微软登录拦截流程

```
NodeProcessManager.OutputDataReceived
  → AuthInterceptor.AnalyzeLine(instanceName, line)
    → MsaCodeRegex.Match(line)  // 主正则
    → MsaCodeFallbackRegex.Match(line)  // 备用正则
    → 匹配成功:
      → RaiseAuthCodeDetected(name, url, code)
        → AuthCodeDetected?.Invoke(this, args)
          → MainWindow.OnAuthCodeDetected (UI 线程)
            → 更新 UI: AuthInstanceLabel, AuthCodeText
            → AuthOverlay.Visibility = Visible
            → 淡入动画 (250ms) + 缩放动画 (300ms)
            → 自动激活窗口 + 临时置顶
```

### 7.4 设置变更即时生效流程

```
SettingsPage ToggleSwitch 切换
  → SettingsViewModel.OnIsDarkModeChanged(value)
    → ApplicationThemeManager.Apply(Dark/Light)
    → SettingsService.UpdateSettings(s => s.IsDarkMode = value)
      → settings.IsDarkMode = value
      → SaveSettings() → JSON 序列化 → 写入文件

SettingsPage Slider 拖动
  → SettingsViewModel.OnBgOpacityChanged(value)
    → Application.Current.Resources["GlobalOverlayOpacity"] = value
    → (所有绑定 {DynamicResource GlobalOverlayOpacity} 的控件自动更新)
    → SettingsService.UpdateSettings(s => s.BgOpacity = value)
```

---

## 8. 关键设计模式

### 8.1 MVVM 模式

- **Model**: `BotInstance`, `AppSettings`, `ConsoleEntry`, `UpdateInfo`
- **View**: XAML 页面/对话框 + Code-Behind（仅处理纯 UI 逻辑）
- **ViewModel**: 继承 `ObservableObject`，使用 `[ObservableProperty]` + `[RelayCommand]` 源代码生成器

### 8.2 依赖注入 (DI)

- 使用 `Microsoft.Extensions.DependencyInjection`
- 在 `App.ConfigureServices()` 中集中注册
- 通过 `App.Services` 静态属性实现 Service Locator 模式（View 层获取 VM/Service）
- 合理使用 Singleton（全局状态）和 Transient（临时表单）

### 8.3 观察者模式

- `AuthInterceptor.AuthCodeDetected` 事件 → MainWindow 订阅
- `InstanceManager.ProcessStarted` 事件 → ConsoleViewModel 订阅
- `NodeProcessManager.OutputReceived` / `ProcessExited` 事件 → ConsoleViewModel 订阅

### 8.4 策略模式

- `AuthInterceptor` 使用两级正则策略（主正则 + 备用正则）匹配不同格式的登录提示

### 8.5 外观模式

- `InstanceManager` 封装了 `EnvManager` + `DownloadService` + `SettingsService` + `NodeProcessManager` 的复杂交互

### 8.6 模板方法模式

- `WebPage` 封装 WebView2 初始化 → MarketPage/ExperiencePage/DocsPage 只需调用 `SetUrl()`

---

## 9. 全局资源与主题系统

### 9.1 主题系统

- 使用 **WPF-UI** 的主题系统 (`ui:ThemesDictionary` + `ui:ControlsDictionary`)
- 支持亮色/暗色模式切换 (`ApplicationThemeManager.Apply()`)
- 全局 `TextBlock` 样式绑定 `TextFillColorPrimaryBrush` 确保文字跟随主题

### 9.2 统一背景遮罩

所有页面使用 `UnifiedOverlayBorderStyle`：

```xml
<Style x:Key="UnifiedOverlayBorderStyle" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource ApplicationBackgroundBrush}" />
    <Setter Property="CornerRadius" Value="12" />
    <Setter Property="Opacity" Value="{DynamicResource GlobalOverlayOpacity}" />
</Style>
```

使用方式：
```xml
<Grid>
    <Border Style="{StaticResource UnifiedOverlayBorderStyle}" />
    <!-- 内容层 -->
    <Border CornerRadius="12" ...> ... </Border>
</Grid>
```

### 9.3 控制台终端风格

- 背景: `#1E1E1E`（VS Code 风格深色）
- 字体: `Cascadia Code, Consolas, Courier New`
- 工具栏: `#252526`（VS Code 标题栏色）
- 输入区: `#1E1E1E` + `#3E3E42` 边框
- 日志着色: 白色(info) / 黄色(warn) / 红色(error) / 绿色(stdin)

---

## 10. 安全与隐私考虑

### 10.1 敏感数据处理

| 数据 | 存储方式 | 风险等级 |
|------|----------|----------|
| `LLM_API_KEY` | 明文存储在 `%APPDATA%/ARCbot/Instances/{name}/.env` | **高** — 任何可读取 AppData 的进程都能获取 |
| `MC_LOGIN_PASSWORD` | 同上 | **高** |
| `LLM_API_URL` | 同上 | 低 |
| `MC_USERNAME` | 同上 | 低 |

> **⚠️ 安全建议**: `.env` 文件中的 API Key 和密码以明文存储，建议未来版本考虑加密存储或使用 Windows DPAPI。

### 10.2 WebView2 安全配置

```csharp
WebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
WebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
```

禁用了 WebView2 的密码保存和自动填充功能。

### 10.3 更新安全

- 更新 URL 硬编码为 `http://localhost:3000/update`（开发/内网环境）
- 更新下载通过 HTTP（非 HTTPS），存在中间人攻击风险
- 更新安装通过批处理脚本替换 exe，无签名验证

---

## 11. 扩展性与可维护性

### 11.1 当前架构优点

1. **清晰的 MVVM 分层**: Model/ViewModel/View 职责分明
2. **DI 容器**: 服务注册集中管理，易于替换实现
3. **源代码生成器**: CommunityToolkit.Mvvm 减少样板代码
4. **插件化页面**: WebView2 内嵌远程页面，可独立更新内容
5. **事件驱动**: 进程输出、登录拦截等通过事件解耦

### 11.2 可改进点

1. **硬编码 URL**: 更新服务器 URL、基础包 CDN URL 硬编码在代码中，应提取到配置文件
2. **密码明文存储**: 敏感信息应加密
3. **错误处理**: 多处使用空 catch 块静默吞异常
4. **日志系统**: 缺少结构化日志（目前仅控制台 UI 日志）
5. **单元测试**: 项目中无测试代码
6. **MemberChecker.cs**: 已废弃的调试类仍保留在项目中

### 11.3 扩展方向建议

1. **插件市场集成**: WebView2 页面可通过 `window.external` 与 WPF 通信，实现一键安装插件
2. **多语言支持**: 当前所有 UI 文本硬编码为中文，可引入资源文件
3. **远程配置**: 可通过 WebView2 或 HTTP 实现远程功能开关
4. **实例导入/导出**: 支持将实例配置打包分享

---

## 12. 文件清单

| 文件路径 | 行数 | 类型 | 核心职责 |
|----------|------|------|----------|
| `ARCbot.csproj` | 51 | 项目配置 | SDK/框架/依赖/构建选项 |
| `App.xaml` | 63 | XAML 资源 | 全局主题/样式/颜色/转换器 |
| `App.xaml.cs` | 98 | C# 入口 | DI 配置/启动流程/退出清理 |
| `MainWindow.xaml` | 279 | XAML 布局 | 导航结构/弹窗/标题栏/背景 |
| `MainWindow.xaml.cs` | 262 | C# 逻辑 | 导航/弹窗动画/背景切换/PageService |
| `MemberChecker.cs` | 20 | C# (废弃) | 反射调试 Wpf.Ui 类型成员 |
| **Models** | | | |
| `Models/AppSettings.cs` | 18 | 模型 | 应用设置数据结构 |
| `Models/BotInstance.cs` | 109 | 模型 | 机器人实例（含 Clone） |
| `Models/ConsoleEntry.cs` | 32 | 模型 | 控制台日志条目 |
| `Models/UpdateInfo.cs` | 15 | 模型 | 更新信息 JSON 映射 |
| **Services** | | | |
| `Services/AuthInterceptor.cs` | 89 | 服务 | 微软登录码正则拦截 |
| `Services/DownloadService.cs` | 165 | 服务 | Node.js/基础包下载解压 |
| `Services/EnvManager.cs` | 128 | 服务 | .env ↔ BotInstance 双向转换 |
| `Services/InstanceManager.cs` | 173 | 服务 | 实例 CRUD + 生命周期 |
| `Services/NodeProcessManager.cs` | 226 | 服务 | Node.js 进程管理 |
| `Services/SettingsService.cs` | 54 | 服务 | JSON 设置持久化 |
| `Services/UpdateService.cs` | 88 | 服务 | 版本检查 + 更新对话框 |
| **ViewModels** | | | |
| `ViewModels/MainWindowViewModel.cs` | 19 | VM | 主窗口状态 |
| `ViewModels/HomeViewModel.cs` | 133 | VM | 首页实例列表 CRUD |
| `ViewModels/ConsoleViewModel.cs` | 171 | VM | 控制台日志/输入 |
| `ViewModels/CreateInstanceViewModel.cs` | 234 | VM | 创建/编辑表单验证 |
| `ViewModels/PluginsViewModel.cs` | 160 | VM | 插件文件管理 |
| `ViewModels/SettingsViewModel.cs` | 306 | VM | 设置/下载/主题 |
| **Views/Pages** | | | |
| `Views/Pages/HomePage.xaml` | 229 | View | 实例卡片网格 |
| `Views/Pages/HomePage.xaml.cs` | 113 | Code-Behind | 对话框/导航交互 |
| `Views/Pages/ConsolePage.xaml` | 252 | View | 终端界面 |
| `Views/Pages/ConsolePage.xaml.cs` | 110 | Code-Behind | 自动滚动/回车发送 |
| `Views/Pages/PluginsPage.xaml` | 164 | View | 插件管理界面 |
| `Views/Pages/PluginsPage.xaml.cs` | 23 | Code-Behind | 加载实例列表 |
| `Views/Pages/SettingsPage.xaml` | 326 | View | 设置界面 |
| `Views/Pages/SettingsPage.xaml.cs` | 40 | Code-Behind | 进度条控制 |
| `Views/Pages/MarketPage.xaml` | 10 | View | WebView2 包装 |
| `Views/Pages/MarketPage.xaml.cs` | 12 | Code-Behind | 设置市场 URL |
| `Views/Pages/ExperiencePage.xaml` | 10 | View | WebView2 包装 |
| `Views/Pages/ExperiencePage.xaml.cs` | 12 | Code-Behind | 设置体验 URL |
| `Views/Pages/DocsPage.xaml` | 10 | View | WebView2 包装 |
| `Views/Pages/DocsPage.xaml.cs` | 12 | Code-Behind | 设置文档 URL |
| `Views/Pages/WebPage.xaml` | 24 | View | WebView2 控件 |
| `Views/Pages/WebPage.xaml.cs` | 51 | Code-Behind | WebView2 初始化 |
| **Views/Dialogs** | | | |
| `Views/Dialogs/CreateInstanceDialog.xaml` | 290 | View | 实例表单 |
| `Views/Dialogs/CreateInstanceDialog.xaml.cs` | 57 | Code-Behind | 模式切换/密码同步 |
| `Views/Dialogs/UpdateDialog.xaml` | 70 | View | 更新提示 |
| `Views/Dialogs/UpdateDialog.xaml.cs` | 202 | Code-Behind | 下载/安装/批处理 |
| **Helpers** | | | |
| `Helpers/PathHelper.cs` | 59 | 工具 | 全局路径常量 |
| `Helpers/Converters.cs` | 31 | 工具 | 值转换器（备用） |

**总计**: 约 50 个源文件，估计 ~5000 行代码（含 XAML）。

---

> **文档结束** — 本文档由对 ARCbot v1.0.3 源代码的完整阅读与分析生成，覆盖了项目的每一个源文件、每一个类、每一个方法的核心逻辑与设计意图。